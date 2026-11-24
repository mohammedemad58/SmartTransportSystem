using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    // CarNumberSuggestion class
    public class CarNumberSuggestion
    {
        public string plate_number { get; set; }
        public string description { get; set; }
    }

    // SubscriberItem class
    [Serializable]
    public class SubscriberItem
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public partial class work_order : System.Web.UI.Page
    {
        // Suppresses popups while a saved order is being loaded
        private bool _loadingSaved = false;

        // Persist selected employees across postbacks
        private List<SubscriberItem> SelectedEmployeesList
        {
            get
            {
                if (ViewState["SelectedEmployeesList"] == null)
                    ViewState["SelectedEmployeesList"] = new List<SubscriberItem>();
                return (List<SubscriberItem>)ViewState["SelectedEmployeesList"];
            }
            set
            {
                ViewState["SelectedEmployeesList"] = value;
            }
        }

        #region Small helpers
        // ===== Driver license check =====
        private const int LicenseWarningDays = 30;
        private const bool BlockExpiredLicense = true;   // false = only warn, never block

        // Returns 0 = fine, 1 = warning (expires soon / today / no license data), 2 = expired
        private int CheckDriverLicense(string employeeId, out string message)
        {
            message = "";
            if (string.IsNullOrEmpty(employeeId) || employeeId == "0") return 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    // the driver is identified by employee_id (that is what ddlDriverName holds)
                    string query = @"
                SELECT TOP 1 e.employee_name, d.license_end_date
                FROM employees e
                LEFT JOIN drivers d ON d.employee_id = e.employee_id AND d.is_active = 1
                WHERE e.employee_id = @id
                ORDER BY d.license_end_date DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(employeeId));

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    if (dt.Rows.Count == 0) return 0;

                    string name = dt.Rows[0]["employee_name"].ToString();

                    if (dt.Rows[0]["license_end_date"] == DBNull.Value)
                    {
                        message = $"تنبيه: لا توجد بيانات رخصة مسجلة للسائق {name}";
                        return 1;
                    }

                    DateTime end = Convert.ToDateTime(dt.Rows[0]["license_end_date"]).Date;
                    int daysLeft = (int)(end - DateTime.Today).TotalDays;
                    string dateText = end.ToString("yyyy-MM-dd");

                    if (daysLeft < 0)
                    {
                        message = $"رخصة السائق {name} منتهية منذ {-daysLeft} يوم (بتاريخ {dateText})";
                        if (BlockExpiredLicense)
                            message += ". لا يمكن اختياره حتى يتم تجديد الرخصة.";
                        return 2;
                    }

                    if (daysLeft == 0)
                    {
                        message = $"تنبيه: تنتهي رخصة السائق {name} اليوم ({dateText})";
                        return 1;
                    }

                    if (daysLeft <= LicenseWarningDays)
                    {
                        message = $"تنبيه: تنتهي رخصة السائق {name} بعد {daysLeft} يوم (بتاريخ {dateText})";
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking driver license: {ex.Message}");
            }

            return 0;   // a failed check must not lock the page
        }

        // Shows the alert. Returns true when the driver was blocked (and un-selected).
        private bool BlockIfLicenseProblem(string employeeId)
        {
            string message;
            int level = CheckDriverLicense(employeeId, out message);
            if (level == 0) return false;

            ScriptManager.RegisterStartupScript(this, GetType(), "licenseAlert",
                "alert(" + new JavaScriptSerializer().Serialize(message) + ");", true);

            if (level == 2 && BlockExpiredLicense)
            {
                ddlDriverName.SelectedIndex = 0;   // back to "-- اختر اسم السائق --"
                return true;
            }

            return false;
        }
        // ===== Trip status helpers (0=Pending, 1=OnRoad, 2=Finished, 3=Broken) =====

        private enum TripStatus
        {
            Pending = 0,
            OnRoad = 1,
            Finished = 2,
            Broken = 3
        }

        private static TripStatus ToTripStatus(object status)
        {
            if (status == null || status == DBNull.Value) return TripStatus.Pending;
            int s = Convert.ToInt32(status);
            if (s < 0 || s > 3) return TripStatus.Pending;
            return (TripStatus)s;
        }

        protected string GetTripStatusText(object status)
        {
            switch (ToTripStatus(status))
            {
                case TripStatus.OnRoad: return "في الطريق";
                case TripStatus.Finished: return "منتهية";
                case TripStatus.Broken: return "معطلة";
                default: return "لم تبدأ";
            }
        }

        protected string GetTripStatusClass(object status)
        {
            switch (ToTripStatus(status))
            {
                case TripStatus.OnRoad: return "trip-badge trip-onroad";
                case TripStatus.Finished: return "trip-badge trip-finished";
                case TripStatus.Broken: return "trip-badge trip-broken";
                default: return "trip-badge trip-pending";
            }
        }
        // An order is ACTIVE when it has started and has not ended yet:
        // start <= now AND (no end time yet OR end time is still in the future)
        // A car is BUSY while it has an order that is not closed yet:
        // no return time AND no end kilometer
        // A car is BUSY while it has an order that is not closed yet:
        // no return time AND no end kilometer
        private DataRow GetActiveOrderForCar(string carNumberId)
        {
            if (string.IsNullOrEmpty(carNumberId) || carNumberId == "0") return null;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT TOP 1 
                    oc.mission_id,
                    oc.date AS order_date,
                    oc.mission_starting_time,
                    oc.mission_ending_time,
                    e.employee_name AS driver_name
                FROM operation_command oc
                LEFT JOIN employees e ON oc.driver_name = e.employee_id
                WHERE oc.v_number = @carId
                  AND oc.deleted = 0
                  AND ISNULL(oc.hide, 0) = 0
                  AND oc.mission_ending_time IS NULL
                  AND (oc.kilometer_end IS NULL OR oc.kilometer_end = 0)
                ORDER BY ISNULL(oc.mission_starting_time, oc.date) DESC, oc.mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carId", Convert.ToInt32(carNumberId));

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking active order: {ex.Message}");
                return null;   // a failed check must not lock the page
            }
        }

        // Every value is checked for NULL, because open orders have no start/return time
        private string BuildBusyMessage(DataRow r)
        {
            string start;
            if (r["mission_starting_time"] != DBNull.Value)
                start = Convert.ToDateTime(r["mission_starting_time"]).ToString("yyyy-MM-dd HH:mm");
            else if (r["order_date"] != DBNull.Value)
                start = Convert.ToDateTime(r["order_date"]).ToString("yyyy-MM-dd") + " (تاريخ الأمر - لم يُسجل وقت البدء)";
            else
                start = "غير محدد";

            string end = r["mission_ending_time"] != DBNull.Value
                ? Convert.ToDateTime(r["mission_ending_time"]).ToString("yyyy-MM-dd HH:mm")
                : "لم يُغلق بعد (لم يُسجل وقت العودة)";

            string driver = r["driver_name"] != DBNull.Value ? r["driver_name"].ToString() : "غير معروف";

            return $"هذه السيارة في أمر شغل جاري رقم {r["mission_id"]} (السائق: {driver}) - " +
                   $"بدأ: {start} - ينتهي: {end}. لا يمكن اختيارها حتى يُغلق الأمر.";
        }

        // Shows the message and un-selects the car. Returns true when the car is busy.
        private bool BlockIfCarBusy(string carNumberId)
        {
            DataRow active = GetActiveOrderForCar(carNumberId);
            if (active == null) return false;

            string message;
            try
            {
                message = BuildBusyMessage(active);
            }
            catch (Exception ex)
            {
                // even if the details cannot be built, the car must still be blocked
                System.Diagnostics.Debug.WriteLine($"Error building busy message: {ex.Message}");
                message = "هذه السيارة في أمر شغل جاري ولا يمكن اختيارها حتى يُغلق الأمر.";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "carBusy",
                "alert(" + new JavaScriptSerializer().Serialize(message) + ");", true);

            ddlCarNumber.SelectedIndex = 0;   // back to "-- اختر رقم العربية --"
            ClearCarDetails();
            return true;
        }

        private static string ConnString
        {
            get { return ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString; }
        }

        // The page JS restores the tab from this hidden field after every postback
        private void SetTab(string tabId)
        {
            hfActiveTab.Value = tabId;
        }

        // Distance of a route (routes.KM) as text. Uses the grouped route list kept in Session.
        private string GetRouteKmText(int routeId, object fallbackKm)
        {
            DataTable dt = Session["RoutesData"] as DataTable;
            if (dt != null)
            {
                DataRow[] rows = dt.Select($"route_id = {routeId}");
                if (rows.Length > 0 && rows[0]["KM"] != DBNull.Value)
                    return Convert.ToDecimal(rows[0]["KM"]).ToString("0.##", CultureInfo.InvariantCulture);
            }

            if (fallbackKm != null && fallbackKm != DBNull.Value)
                return Convert.ToDecimal(fallbackKm).ToString("0.##", CultureInfo.InvariantCulture);

            return "";
        }

        // Fills the fuel card dropdown with ONLY the cards assigned to the chosen car.
        // A card is linked to a car through fuel_card_transactions.v_id.
        // A car can have several cards and a card can be shared by several cars.
        private void LoadFuelCardsForCar(string carNumberId)
        {
            ddlFuelCard.Items.Clear();

            if (string.IsNullOrEmpty(carNumberId) || carNumberId == "0")
            {
                ddlFuelCard.Items.Add(new ListItem("-- اختر السيارة أولاً --", "0"));
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT DISTINCT fc.card_id, fc.card_number, fc.liter_balance, fc.cubic_balance
                        FROM fuel_card_transactions t
                        INNER JOIN fuel_cards fc ON t.card_id = fc.card_id
                        INNER JOIN main_vehicle_details mvd ON t.v_id = mvd.v_id
                        WHERE mvd.v_num = @carNumberId
                          AND mvd.deleted = 0
                        ORDER BY fc.card_number";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carNumberId", Convert.ToInt32(carNumberId));
                    conn.Open();

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        ddlFuelCard.Items.Add(new ListItem("-- لا يوجد كارت وقود لهذه السيارة --", "0"));
                    }
                    else
                    {
                        ddlFuelCard.Items.Add(new ListItem("-- اختر كارت الوقود --", "0"));

                        foreach (DataRow row in dt.Rows)
                        {
                            string text = row["card_number"].ToString()
                                + " (" + Convert.ToDecimal(row["liter_balance"]).ToString("0.##") + " لتر"
                                + " / " + Convert.ToDecimal(row["cubic_balance"]).ToString("0.##") + " م³)";
                            ddlFuelCard.Items.Add(new ListItem(text, row["card_id"].ToString()));
                        }

                        // One card only: select it for the user (with several cards the user chooses)
                        if (dt.Rows.Count == 1)
                            ddlFuelCard.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading fuel cards: {ex.Message}");
                ddlFuelCard.Items.Clear();
                ddlFuelCard.Items.Add(new ListItem("-- خطأ في تحميل كروت الوقود --", "0"));
            }
        }

        // Empty / 0 / invalid -> NULL in the database
        private object ToDbDecimal(string text)
        {
            if (decimal.TryParse(text, out decimal v) && v > 0)
                return v;
            return DBNull.Value;
        }

        // Makes sure the value exists in the list, then selects it
        private void EnsureListItem(DropDownList ddl, string value, string text)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (ddl.Items.FindByValue(value) == null)
                ddl.Items.Add(new ListItem(text, value));
            ddl.SelectedValue = value;
        }

        private void SafeSetDropdownValue(DropDownList dropdown, string value)
        {
            if (!string.IsNullOrEmpty(value) && value != "0")
            {
                ListItem item = dropdown.Items.FindByValue(value);
                if (item != null)
                {
                    dropdown.SelectedValue = value;
                    System.Diagnostics.Debug.WriteLine($"Successfully set {dropdown.ID} to value: {value}");
                }
                else
                {
                    dropdown.SelectedIndex = 0;
                    System.Diagnostics.Debug.WriteLine($"Warning: Value '{value}' not found in {dropdown.ID}");
                }
            }
            else
            {
                dropdown.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"No value to set for {dropdown.ID}");
            }
        }

        // Car details WITHOUT the is_active filter, so old orders still load
        private DataRow GetCarDetails(string carNumberId)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                string query = @"
                    SELECT TOP 1 ct.car_type_id, ct.car_type_name, ct.car_brand,
                           mvd.v_year AS model_year, ft.type AS fuel_type_name
                    FROM main_vehicle_details mvd
                    LEFT JOIN car_types ct ON mvd.v_type = ct.car_type_id
                    LEFT JOIN fuel_type ft ON mvd.gas_type = ft.fuel_id
                    WHERE mvd.v_num = @id AND mvd.deleted = 0
                    ORDER BY mvd.v_id DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", carNumberId);
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        private string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text)) return "--";
            return System.Security.SecurityElement.Escape(text);
        }

        #endregion

        #region Dropdown change handlers (debug only)

        protected void ddlDriverName_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Driver name changed to: " + ddlDriverName.SelectedValue);

            if (ddlDriverName.SelectedValue != "0")
                BlockIfLicenseProblem(ddlDriverName.SelectedValue);
        }

        protected void ddlGarageWriter_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Garage writer changed to: " + ddlGarageWriter.SelectedValue);
        }

        protected void ddlVehicleObserver_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Vehicle observer changed to: " + ddlVehicleObserver.SelectedValue);
        }

        protected void ddlOperationManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Operation manager changed to: " + ddlOperationManager.SelectedValue);
        }

        #endregion

        #region Print

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string carNumber = ddlCarNumber.SelectedItem?.Text ?? "";
            string carType = ddlCarType.SelectedItem?.Text ?? "";
            string driverName = ddlDriverName.SelectedItem?.Text ?? "";
            string carBrand = txtCarBrand.Text;
            string modelYear = txtModelYear.Text;
            string fuelType = txtFuelType.Text;
            string fuelCard = (ddlFuelCard.SelectedValue != "0" && ddlFuelCard.SelectedItem != null)
                ? ddlFuelCard.SelectedItem.Text : "";
            string missionType = ddlMissionType.SelectedItem?.Text ?? "";
            string routeName = ddlRoute.SelectedItem?.Text ?? "";
            string routeKm = txtRouteKM.Text;
            string missionText = txtMissionText.Text;
            string employees = txtEmployees.Text;
            string startTime = txtStartTime.Text;
            string returnTime = txtReturnTime.Text;
            string startKM = txtStartKM.Text;
            string endKM = txtEndKM.Text;
            string garageWriter = ddlGarageWriter.SelectedItem?.Text ?? "";
            string vehicleObserver = ddlVehicleObserver.SelectedItem?.Text ?? "";
            string operationManager = ddlOperationManager.SelectedItem?.Text ?? "";

            string subscribers = "";
            string[] subscriberIds = hdnSelectedSubscribers.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (subscriberIds.Length > 0)
            {
                subscribers = string.Join(", ", subscriberIds);
            }

            string printHtml = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <title>أمر شغل</title>
        <style>
            * {{ direction: rtl; text-align: right; font-family: Arial, sans-serif; }}
            body {{ padding: 20px; margin: 0; }}
            .page-title {{ color: #c8102e; text-align: center; font-size: 24px; margin-bottom: 20px; border-bottom: 2px solid #c8102e; padding-bottom: 10px; }}
            .section-header {{ background: #c8102e; color: white; padding: 8px 15px; margin: 20px 0 15px 0; font-weight: bold; font-size: 16px; border-radius: 5px; }}
            .row {{ display: flex; flex-wrap: wrap; margin-bottom: 15px; }}
            .col-md-4, .col-md-3 {{ flex: 1; padding: 0 10px; min-width: 200px; }}
            .form-label {{ font-weight: bold; display: block; margin-bottom: 5px; font-size: 13px; color: #555; }}
            .value-display {{ border-bottom: 1px solid #ddd; padding: 8px 5px; margin-top: 5px; font-size: 14px; }}
            @media print {{
                body {{ padding: 10px; }}
                .section-header {{ -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
            }}
        </style>
    </head>
    <body>
        <h2 class='page-title'>أمر شغل</h2>
        
        <!-- البيانات الأساسية -->
        <div class='section-header'>البيانات الأساسية</div>
        <div class='row'>
            <div class='col-md-4'><div class='form-label'>رقم العربية</div><div class='value-display'>{EscapeHtml(carNumber)}</div></div>
            <div class='col-md-4'><div class='form-label'>نوع السيارة</div><div class='value-display'>{EscapeHtml(carType)}</div></div>
            <div class='col-md-4'><div class='form-label'>اسم السائق</div><div class='value-display'>{EscapeHtml(driverName)}</div></div>
        </div>
        <div class='row'>
            <div class='col-md-4'><div class='form-label'>ماركة السيارة</div><div class='value-display'>{EscapeHtml(carBrand)}</div></div>
            <div class='col-md-4'><div class='form-label'>سنة الصنع</div><div class='value-display'>{EscapeHtml(modelYear)}</div></div>
            <div class='col-md-4'><div class='form-label'>نوع الوقود</div><div class='value-display'>{EscapeHtml(fuelType)}</div></div>
            <div class='col-md-4'><div class='form-label'>كارت الوقود</div><div class='value-display'>{EscapeHtml(fuelCard)}</div></div>
        </div>
        
        <!-- بيانات المهمة -->
        <div class='section-header'>بيانات المهمة</div>
        <div class='row'>
            <div class='col-md-4'><div class='form-label'>نوع المأمورية</div><div class='value-display'>{EscapeHtml(missionType)}</div></div>";

            if (missionType == "خط")
            {
                printHtml += $@"
            <div class='col-md-4'><div class='form-label'>خط السير</div><div class='value-display'>{EscapeHtml(routeName)}</div></div>
            <div class='col-md-4'><div class='form-label'>مسافة الخط (كم)</div><div class='value-display'>{EscapeHtml(routeKm)}</div></div>";
            }
            else
            {
                printHtml += $@"
            <div class='col-md-4'><div class='form-label'>نص المأمورية</div><div class='value-display'>{EscapeHtml(missionText)}</div></div>";
            }

            printHtml += $@"
        </div>";

            if (missionType != "خط" && !string.IsNullOrEmpty(employees))
            {
                printHtml += $@"
        <div class='row'>
            <div class='col-md-12'><div class='form-label'>الموظفين</div><div class='value-display'>{EscapeHtml(employees)}</div></div>
        </div>";
            }

            if (missionType == "خط" && !string.IsNullOrEmpty(subscribers))
            {
                printHtml += $@"
        <div class='row'>
            <div class='col-md-12'><div class='form-label'>المشتركين</div><div class='value-display'>{EscapeHtml(subscribers)}</div></div>
        </div>";
            }

            printHtml += $@"
        <!-- بيانات الوقت والمسافة -->
        <div class='section-header'>بيانات الوقت والمسافة</div>
        <div class='row'>
            <div class='col-md-3'><div class='form-label'>بداية المأمورية</div><div class='value-display'>{EscapeHtml(startTime)}</div></div>
            <div class='col-md-3'><div class='form-label'>وقت العودة</div><div class='value-display'>{EscapeHtml(returnTime)}</div></div>
            <div class='col-md-3'><div class='form-label'>عداد البداية (كم)</div><div class='value-display'>{EscapeHtml(startKM)}</div></div>
            <div class='col-md-3'><div class='form-label'>عداد العودة (كم)</div><div class='value-display'>{EscapeHtml(endKM)}</div></div>
        </div>
        
        <!-- بيانات الموظفين -->
        <div class='section-header'>بيانات الموظفين</div>
        <div class='row'>
            <div class='col-md-4'><div class='form-label'>اسم كاتب الجراج</div><div class='value-display'>{EscapeHtml(garageWriter)}</div></div>
            <div class='col-md-4'><div class='form-label'>اسم ملاحظ المركبات</div><div class='value-display'>{EscapeHtml(vehicleObserver)}</div></div>
            <div class='col-md-4'><div class='form-label'>اسم مدير التشغيل</div><div class='value-display'>{EscapeHtml(operationManager)}</div></div>
        </div>
    </body>
    </html>";

            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write(printHtml);
            Response.End();
        }

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAllData();
                LoadSubscribersList();
                LoadCarNumbers();
                LoadAllCarTypes();
                LoadMissionTypes();
                LoadEmployeesList();

                if (SelectedEmployeesList.Count > 0)
                {
                    BindSelectedEmployees();
                }
            }
            else
            {
                if (Session["EmployeesList"] != null)
                {
                    ReloadEmployeesScript();
                }
                if (Session["SubscribersList"] != null)
                {
                    ReloadSubscribersScript();
                }

                // After any postback the route's subscribers must be sent again,
                // otherwise the subscribers dropdown falls back to "all subscribers".
                ReloadRouteSubscribersScript();
            }

            if (IsPostBack && Request["__EVENTTARGET"] == "UpdateSelectedEmployees")
            {
                if (!string.IsNullOrEmpty(hdnSelectedEmployees.Value))
                {
                    var ids = hdnSelectedEmployees.Value.Split(',');
                    SelectedEmployeesList.Clear();

                    if (Session["EmployeesList"] != null)
                    {
                        var allEmployees = Session["EmployeesList"] as List<SubscriberItem>;
                        foreach (var id in ids)
                        {
                            var employeeItem = allEmployees.FirstOrDefault(emp => emp.id == id);
                            if (employeeItem != null)
                            {
                                SelectedEmployeesList.Add(employeeItem);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Search Methods

        protected void btnSearchOperations_Click(object sender, EventArgs e)
        {
            try
            {
                SetTab("tabSearch");

                string carNumber = txtSearchCarNumber.Text.Trim();
                string driverName = txtSearchDriverName.Text.Trim();
                string searchDate = txtSearchDate.Text.Trim();

                SearchOperationCommands(carNumber, driverName, searchDate);

                if (!string.IsNullOrEmpty(carNumber))
                {
                    int carId = GetCarIdFromNumber(carNumber);
                    if (carId > 0)
                    {
                        LoadCarStatistics(carId);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search button error: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "searchError",
                    $"alert('خطأ في البحث: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private int GetCarIdFromNumber(string carNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT TOP 1 car_number_id
                FROM car_numbers
                WHERE REPLACE(letter1 + letter2 + ISNULL(letter3, '') + plate_numbers, ' ', '') 
                    LIKE '%' + REPLACE(@carNumber, ' ', '') + '%'";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carNumber", carNumber);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting car ID: {ex.Message}");
            }

            return 0;
        }

        private void ReloadEmployeesScript()
        {
            try
            {
                var employees = Session["EmployeesList"] as List<SubscriberItem>;
                if (employees != null)
                {
                    string json = new JavaScriptSerializer().Serialize(employees);
                    string script = $@"
                window.employeesList = {json};
                console.log('Employees reloaded on postback:', window.employeesList);
                if (typeof loadEmployeesDropdown === 'function') {{
                    loadEmployeesDropdown();
                }}
                if (typeof updateEmployeesList === 'function') {{
                    updateEmployeesList();
                }}";

                    ScriptManager.RegisterStartupScript(this, GetType(), "reloadEmployees", script, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reloading employees script: {ex.Message}");
            }
        }

        private void ReloadSubscribersScript()
        {
            try
            {
                var subscribers = Session["SubscribersList"] as List<SubscriberItem>;
                if (subscribers != null)
                {
                    string json = new JavaScriptSerializer().Serialize(subscribers);
                    string script = $@"
                window.subscribersList = {json};
                console.log('Subscribers reloaded on postback:', window.subscribersList);
                if (typeof loadSubscribersDropdown === 'function') {{
                    loadSubscribersDropdown();
                }}
                if (typeof updateSubscribersList === 'function') {{
                    updateSubscribersList();
                }}";

                    ScriptManager.RegisterStartupScript(this, GetType(), "reloadSubscribers", script, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reloading subscribers script: {ex.Message}");
            }
        }

        // Re-send the subscribers of the currently selected route after a postback
        private void ReloadRouteSubscribersScript()
        {
            try
            {
                if (ddlMissionType.SelectedValue != "route") return;
                if (string.IsNullOrEmpty(ddlRoute.SelectedValue) || ddlRoute.SelectedValue == "0") return;

                var subscribers = Session["RouteSubscribers"] as List<SubscriberItem>;
                if (subscribers == null) return;

                string json = new JavaScriptSerializer().Serialize(subscribers);
                string script = $@"
            window.routeSubscribers = {json};
            if (typeof loadSubscribersDropdown === 'function') {{
                loadSubscribersDropdown();
            }}";

                ScriptManager.RegisterStartupScript(this, GetType(), "reloadRouteSubscribers", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reloading route subscribers script: {ex.Message}");
            }
        }

        private void SearchOperationCommands(string carNumber, string driverName, string searchDate)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    string query = @"
        SELECT 
            oc.mission_id,
            ISNULL(cn.letter1, '') + ' ' + 
            ISNULL(cn.letter2, '') + ' ' + 
            ISNULL(cn.letter3 + ' ', '') + 
            ISNULL(cn.plate_numbers, '') as car_number,
            oc.date as mission_date,
            ISNULL(e.employee_name, N'غير معروف') as driver_name,
            CASE 
                WHEN oc.mission_type = 1 THEN N'مأمورية'
                WHEN oc.mission_type = 2 THEN N'خط'
                ELSE N'غير معروف'
            END as mission_type,
            CASE 
                WHEN oc.mission_type = 1 THEN oc.mission_description
                WHEN oc.mission_type = 2 THEN 
                    r.route_name + 
                    CASE 
                        WHEN r.start_loc IS NOT NULL AND r.start_loc != '' 
                        THEN ' - ' + r.start_loc + ' - ' + ISNULL(r.end_loc, '')
                        ELSE '' 
                    END
                ELSE NULL
            END as mission_details,
            oc.kilometer_start,
            oc.kilometer_end,
            (oc.kilometer_end - oc.kilometer_start) as trip_distance,
            CAST((oc.kilometer_end - oc.kilometer_start) AS NVARCHAR(10)) + N' كم' as distance_display,
            MAX(oc.kilometer_end) OVER (PARTITION BY oc.v_number) as latest_car_km,
            ISNULL(oc.trip_status, 0) AS trip_status,
            oc.mission_starting_time,
            oc.mission_ending_time
        FROM operation_command oc
        LEFT JOIN car_numbers cn ON oc.v_number = cn.car_number_id
        LEFT JOIN employees e ON oc.driver_name = e.employee_id
        LEFT JOIN routes r ON oc.location_point = r.route_id AND oc.mission_type = 2
        WHERE oc.deleted = 0 
          AND ISNULL(oc.hide, 0) = 0";

                    if (!string.IsNullOrEmpty(carNumber))
                    {
                        query += @" AND (cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers LIKE @carNumber 
                    OR cn.letter1 + ' ' + cn.letter2 + ' ' + ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers LIKE @carNumber)";
                    }

                    if (!string.IsNullOrEmpty(driverName))
                    {
                        query += " AND e.employee_name LIKE @driverName";
                    }

                    DateTime searchStart = DateTime.MinValue;
                    bool hasDate = false;

                    if (!string.IsNullOrEmpty(searchDate))
                    {
                        if (DateTime.TryParseExact(searchDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchStart))
                        {
                            hasDate = true;
                            query += " AND oc.date >= @searchStart AND oc.date < @searchEnd";
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "invalidDate",
                                "alert('صيغة التاريخ غير صحيحة. استخدم yyyy-MM-dd');", true);
                            return;
                        }
                    }

                    query += " ORDER BY oc.date DESC, oc.mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(carNumber))
                    {
                        cmd.Parameters.AddWithValue("@carNumber", "%" + carNumber + "%");
                    }

                    if (!string.IsNullOrEmpty(driverName))
                    {
                        cmd.Parameters.AddWithValue("@driverName", "%" + driverName + "%");
                    }

                    if (hasDate)
                    {
                        cmd.Parameters.AddWithValue("@searchStart", searchStart);
                        cmd.Parameters.AddWithValue("@searchEnd", searchStart.AddDays(1));
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSearchResults.DataSource = dt;
                    gvSearchResults.DataBind();
                    searchResults.Visible = true;

                    if (dt.Rows.Count == 0)
                    {
                        string msg = "لا توجد نتائج للبحث";
                        if (!string.IsNullOrEmpty(carNumber) || !string.IsNullOrEmpty(driverName) || !string.IsNullOrEmpty(searchDate))
                        {
                            msg = "لا توجد نتائج للبحث بالمعايير المحددة";
                        }
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            $"alert('{msg}');", true);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "searchError",
                        $"alert('خطأ في البحث: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        protected void gvSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectOperation")
            {
                int missionId = Convert.ToInt32(e.CommandArgument);
                LoadOperationCommand(missionId);
            }
        }
        protected void gvSearchResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DataRowView row = e.Row.DataItem as DataRowView;
            if (row == null) return;
            if (row.Row.Table.Columns.Contains("trip_status") == false) return;

            TripStatus status = ToTripStatus(row["trip_status"]);
            switch (status)
            {
                case TripStatus.OnRoad:
                    e.Row.BackColor = System.Drawing.Color.FromName("#e7f1ff");
                    break;
                case TripStatus.Finished:
                    e.Row.BackColor = System.Drawing.Color.FromName("#e9f7ef");
                    break;
                case TripStatus.Broken:
                    e.Row.BackColor = System.Drawing.Color.FromName("#fdecea");
                    break;
            }
        }
        // ===== LOAD A SAVED ORDER INTO THE FORM =====
        private void LoadOperationCommand(int missionId)
        {
            _loadingSaved = true;
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadOperationCommand START for mission {missionId} ===");

                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
SELECT 
    oc.*,
    cn.car_number_id AS cn_id,
    LTRIM(RTRIM(ISNULL(cn.letter1,'') + ' ' + ISNULL(cn.letter2 + ' ','') +
                ISNULL(cn.letter3 + ' ','') + ISNULL(cn.plate_numbers,''))) AS full_plate_number,
    e_driver.employee_id AS driver_id,
    e_driver.employee_name AS driver_name_text,
    r.start_loc + ' - ' + ISNULL(r.end_loc, '') AS route_description,
    r.KM AS route_km,
    fc.card_number AS fuel_card_text
FROM operation_command oc
INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
INNER JOIN employees e_driver ON oc.driver_name = e_driver.employee_id
LEFT JOIN routes r ON oc.location_point = r.route_id AND oc.mission_type = 2
LEFT JOIN fuel_cards fc ON oc.fuel_card_id = fc.card_id
WHERE oc.mission_id = @missionId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@missionId", missionId);
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Rows loaded: {dt.Rows.Count}");

                if (dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "loadError",
                        $"alert('لم يتم العثور على بيانات أمر الشغل رقم {missionId}');", true);
                    return;
                }

                // ✅ تأكد أن الـ tab ينتقل إلى النموذج
                SetTab("tabForm");

                DataRow r = dt.Rows[0];
                string carNumberId = r["cn_id"].ToString();
                string fullPlate = r["full_plate_number"].ToString();
                string driverId = r["driver_id"].ToString();
                string driverText = r["driver_name_text"].ToString();
                string missionTypeValue = r["mission_type"]?.ToString() ?? "0";

                System.Diagnostics.Debug.WriteLine($"Mission type: {missionTypeValue}");

                // ===== 1. Load lists =====
                int routeId = 0;
                if (missionTypeValue == "1")
                {
                    LoadCarNumbers();
                    LoadDrivers();
                }
                else if (missionTypeValue == "2")
                {
                    int saved = r["location_point"] != DBNull.Value ? Convert.ToInt32(r["location_point"]) : 0;
                    int resolved = saved > 0 ? ResolveRouteGroupId(saved) : 0;

                    if (resolved > 0 && ddlRoute.Items.FindByValue(resolved.ToString()) != null)
                    {
                        routeId = resolved;
                        LoadCarsByRoute(routeId);
                        LoadDriversByRoute(routeId);
                    }
                    else
                    {
                        LoadCarNumbers();
                        LoadDrivers();
                    }
                }

                // ===== 2. Car + driver =====
                EnsureListItem(ddlCarNumber, carNumberId, fullPlate);
                EnsureListItem(ddlDriverName, driverId, driverText);

                // ===== 3. Car details =====
                DataRow car = GetCarDetails(carNumberId);
                if (car != null)
                {
                    if (car["car_type_id"] != DBNull.Value)
                        EnsureListItem(ddlCarType, car["car_type_id"].ToString(), car["car_type_name"].ToString());
                    txtCarBrand.Text = car["car_brand"]?.ToString() ?? "--";
                    txtModelYear.Text = car["model_year"]?.ToString() ?? "--";
                    txtFuelType.Text = car["fuel_type_name"]?.ToString() ?? "--";
                }

                // ===== 3b. Fuel card =====
                LoadFuelCardsForCar(carNumberId);
                if (r["fuel_card_id"] != DBNull.Value)
                {
                    EnsureListItem(ddlFuelCard, r["fuel_card_id"].ToString(),
                        r["fuel_card_text"] != DBNull.Value ? r["fuel_card_text"].ToString() : r["fuel_card_id"].ToString());
                }

                // ===== 4. Personnel =====
                SafeSetDropdownValue(ddlGarageWriter, r["grage_writer_name"]?.ToString());
                SafeSetDropdownValue(ddlVehicleObserver, r["vehicle_observar"]?.ToString());
                SafeSetDropdownValue(ddlOperationManager, r["opration_manager_name"]?.ToString());

                // ===== 5. Times and kilometers =====
                txtStartTime.Text = r["mission_starting_time"] != DBNull.Value
                    ? Convert.ToDateTime(r["mission_starting_time"]).ToString("yyyy-MM-ddTHH:mm") : "";
                txtReturnTime.Text = r["mission_ending_time"] != DBNull.Value
                    ? Convert.ToDateTime(r["mission_ending_time"]).ToString("yyyy-MM-ddTHH:mm") : "";

                txtStartKM.Text = r["kilometer_start"] != DBNull.Value && Convert.ToDecimal(r["kilometer_start"]) > 0
                    ? Convert.ToDecimal(r["kilometer_start"]).ToString("0.0") : "";
                txtEndKM.Text = r["kilometer_end"] != DBNull.Value && Convert.ToDecimal(r["kilometer_end"]) > 0
                    ? Convert.ToDecimal(r["kilometer_end"]).ToString("0.0") : "";

                if (hdnLastKilometer != null)
                    hdnLastKilometer.Value = GetLastCarKilometer(Convert.ToInt32(carNumberId)).ToString("0.0");

                // ===== 6. Mission type =====
                txtRouteKM.Text = "";

                if (missionTypeValue == "1")
                {
                    ddlMissionType.SelectedValue = "mission";
                    Session["RouteSubscribers"] = null;

                    routeDropdownContainer.Visible = false;
                    missionTextContainer.Visible = true;
                    employeesContainer.Visible = true;
                    employeesContainer.Style["display"] = "block";
                    subscribersContainer.Visible = false;
                    subscribersContainer.Style["display"] = "none";

                    txtMissionText.Text = r["mission_description"]?.ToString() ?? "";
                    txtEmployees.Text = r["employees_text"]?.ToString() ?? "";
                    routeDescriptionContainer.Visible = false;
                    lblRouteDescription.Text = "";

                    // ✅ تأكد أن الـ tab يتحول قبل الرسالة
                    ScriptManager.RegisterStartupScript(this, GetType(), "activateFormTab",
                        "activateTab('tabForm'); alert('تم تحميل بيانات أمر الشغل (مأمورية) بنجاح');", true);
                }
                else if (missionTypeValue == "2")
                {
                    ddlMissionType.SelectedValue = "route";

                    routeDropdownContainer.Visible = true;
                    missionTextContainer.Visible = false;
                    subscribersContainer.Visible = true;
                    subscribersContainer.Style["display"] = "block";
                    employeesContainer.Visible = false;
                    employeesContainer.Style["display"] = "none";
                    txtMissionText.Text = "";

                    if (routeId > 0)
                    {
                        SafeSetDropdownValue(ddlRoute, routeId.ToString());
                        txtRouteKM.Text = GetRouteKmText(routeId, r["route_km"]);
                        LoadSubscribersByRoute(routeId);
                        LoadOperationSubscribers(missionId);

                        string desc = r["route_description"]?.ToString();
                        routeDescriptionContainer.Visible = !string.IsNullOrEmpty(desc);
                        lblRouteDescription.Text = string.IsNullOrEmpty(desc) ? "" : "📌 " + desc;
                    }
                    else
                    {
                        ddlRoute.SelectedIndex = 0;
                        routeDescriptionContainer.Visible = false;
                        ClearSubscribersList();
                        LoadOperationSubscribers(missionId);
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "activateFormTab",
                        "activateTab('tabForm'); alert('تم تحميل بيانات أمر الشغل (خط) بنجاح');", true);
                }
                else
                {
                    ddlMissionType.SelectedIndex = 0;
                    routeDropdownContainer.Visible = false;
                    missionTextContainer.Visible = false;
                    subscribersContainer.Visible = false;
                    employeesContainer.Visible = false;
                    routeDescriptionContainer.Visible = false;
                }

                System.Diagnostics.Debug.WriteLine($"=== LoadOperationCommand END for mission {missionId} ===");
            }
            catch (Exception ex)
            {
                // ✅ هذا يعرض الخطأ للمستخدم بدل "الصفحة تعيد التحميل فقط"
                System.Diagnostics.Debug.WriteLine($"ERROR in LoadOperationCommand: {ex}");
                string errMsg = ex.Message.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ");
                ScriptManager.RegisterStartupScript(this, GetType(), "loadError",
                    $"alert('خطأ في تحميل أمر الشغل: {errMsg}');", true);
            }
            finally
            {
                _loadingSaved = false;
            }
        }

        // A route can be stored as several rows in 'routes' (one per car/driver) that share the same
        // name + start + end. The route dropdown lists one entry per group (the lowest route_id).
        // This maps any route_id of a group to that lowest route_id.
        private int ResolveRouteGroupId(int routeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT MIN(r.route_id)
                FROM routes r0
                INNER JOIN routes r
                        ON r.route_name = r0.route_name
                       AND ISNULL(r.start_loc, '') = ISNULL(r0.start_loc, '')
                       AND ISNULL(r.end_loc, '')   = ISNULL(r0.end_loc, '')
                       AND r.is_active = 1
                WHERE r0.route_id = @routeId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resolving route group id: {ex.Message}");
            }

            // route no longer exists / inactive
            return 0;
        }

        private void LoadSubscribersByRoute(int routeId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadSubscribersByRoute called for route ID: {routeId}");

                List<SubscriberItem> subscribers = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    // A route may be several rows in 'routes' (several cars/drivers) with the same name/start/end.
                    // Subscribers can be attached to any of those rows, so match the whole group.
                    string query = @"
                SELECT 
                    n_id as id,
                    name,
                    department,
                    position
                FROM subscription_details 
                WHERE route_id IN (
                        SELECT r.route_id
                        FROM routes r0
                        INNER JOIN routes r
                                ON r.route_name = r0.route_name
                               AND ISNULL(r.start_loc, '') = ISNULL(r0.start_loc, '')
                               AND ISNULL(r.end_loc, '')   = ISNULL(r0.end_loc, '')
                        WHERE r0.route_id = @routeId)
                  AND (end_date >= GETDATE() OR end_date IS NULL)
                  AND (hide = 0 OR hide IS NULL)
                ORDER BY name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["name"].ToString();
                            string department = reader["department"]?.ToString();
                            string position = reader["position"]?.ToString();

                            if (!string.IsNullOrEmpty(department) || !string.IsNullOrEmpty(position))
                            {
                                displayName = $"{displayName} - {department} ({position})";
                            }

                            subscribers.Add(new SubscriberItem
                            {
                                id = reader["id"].ToString(),
                                name = displayName
                            });
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Total subscribers found for route {routeId}: {subscribers.Count}");

                Session["RouteSubscribers"] = subscribers;

                string json = new JavaScriptSerializer().Serialize(subscribers);
                string script = $@"
            window.routeSubscribers = {json};
            console.log('Route subscribers loaded:', window.routeSubscribers);
            console.log('Number of subscribers:', {subscribers.Count});
            if (typeof loadSubscribersDropdown === 'function') {{
                loadSubscribersDropdown();
            }}";

                ScriptManager.RegisterStartupScript(this, GetType(), "loadRouteSubscribers_" + routeId, script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscribers by route: {ex.Message}");
            }
        }

        // ===== ROUTE-FIRST FLOW: load ALL cars assigned to this route (every row of the route group) =====
        private void LoadCarsByRoute(int routeId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadCarsByRoute called for route ID: {routeId}");

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
        SELECT DISTINCT
            cn.car_number_id,
            CASE 
                WHEN cn.letter2 IS NULL AND cn.letter3 IS NULL 
                    THEN cn.letter1 + ' ' + cn.plate_numbers
                WHEN cn.letter3 IS NULL 
                    THEN cn.letter1 + ' ' + cn.letter2 + ' ' + cn.plate_numbers
                ELSE cn.letter1 + ' ' + cn.letter2 + ' ' + cn.letter3 + ' ' + cn.plate_numbers
            END as full_plate_number,
            ct.car_type_id,
            ct.car_type_name,
            ct.car_brand,
            mvd.v_year as model_year,
            ft.type as fuel_type_name,
            mvd.initial_km,
            mvd.current_odometer
        FROM routes r0
        INNER JOIN routes r
                ON r.route_name = r0.route_name
               AND ISNULL(r.start_loc, '') = ISNULL(r0.start_loc, '')
               AND ISNULL(r.end_loc, '')   = ISNULL(r0.end_loc, '')
               AND r.is_active = 1
       INNER JOIN car_numbers cn ON r.car_id = cn.car_number_id
INNER JOIN main_vehicle_details mvd ON cn.car_number_id = mvd.v_num
        INNER JOIN car_types ct ON mvd.v_type = ct.car_type_id
        LEFT JOIN fuel_type ft ON mvd.gas_type = ft.fuel_id
        WHERE r0.route_id = @routeId
          AND mvd.deleted = 0
          AND (mvd.hide = 0 OR mvd.hide IS NULL)
          AND cn.is_active = 1
        ORDER BY full_plate_number";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Same Session key so ddlCarNumber_SelectedIndexChanged keeps working unchanged
                    Session["CarDetailsData"] = dt;

                    ddlCarNumber.Items.Clear();
                    ddlCarNumber.Items.Add(new ListItem("-- اختر رقم العربية --", ""));
                    ddlCarNumber.DataSource = dt;
                    ddlCarNumber.DataTextField = "full_plate_number";
                    ddlCarNumber.DataValueField = "car_number_id";
                    ddlCarNumber.DataBind();

                    System.Diagnostics.Debug.WriteLine($"Total cars found for route {routeId}: {dt.Rows.Count}");

                    if (dt.Rows.Count == 0)
                    {
                        if (!_loadingSaved)
                            ScriptManager.RegisterStartupScript(this, GetType(), "noCarsForRoute_" + routeId,
                                "alert('لا توجد سيارة مسندة لخط السير هذا. الرجاء تعيين سيارة لهذا الخط أولاً.');", true);
                    }
                    else if (dt.Rows.Count == 1 && !_loadingSaved)
                    {
                        // Only one car for this route: auto-select it and load its details immediately.
                        // (When the route has several cars, the user chooses one from the list.)
                        string singleCarId = dt.Rows[0]["car_number_id"].ToString();
                        ddlCarNumber.SelectedValue = singleCarId;

                        if (!BlockIfCarBusy(singleCarId))
                            AutoLoadCarDetails(singleCarId);

                        System.Diagnostics.Debug.WriteLine($"Auto-selected route's car: {singleCarId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cars by route: {ex.Message}");
            }
        }

        private void AutoLoadCarDetails(string selectedCarNumberId)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedCarNumberId) || selectedCarNumberId == "0")
                {
                    ClearCarDetails();
                    return;
                }

                DataTable dt = Session["CarDetailsData"] as DataTable;

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rows = dt.Select($"car_number_id = {selectedCarNumberId}");

                    if (rows.Length > 0)
                    {
                        ApplyCarRow(rows[0], selectedCarNumberId);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"AutoLoadCarDetails: no row found for {selectedCarNumberId}");
                        ClearCarDetails();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("AutoLoadCarDetails: CarDetailsData is null or empty");
                    ClearCarDetails();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AutoLoadCarDetails: {ex.Message}");
                ClearCarDetails();
            }
        }

        // Shared by AutoLoadCarDetails and ddlCarNumber_SelectedIndexChanged:
        // fills car type / brand / year / fuel and the start kilometer from a CarDetailsData row
        private void ApplyCarRow(DataRow row, string selectedCarNumberId)
        {
            // Only the fuel cards assigned to this car are offered
            LoadFuelCardsForCar(selectedCarNumberId);

            if (row["car_type_id"] != DBNull.Value)
            {
                string carTypeId = row["car_type_id"].ToString();
                ListItem item = ddlCarType.Items.FindByValue(carTypeId);
                if (item != null)
                {
                    ddlCarType.SelectedValue = carTypeId;
                }
                else
                {
                    string displayText = row["car_type_name"]?.ToString();
                    ddlCarType.Items.Add(new ListItem(displayText, carTypeId));
                    ddlCarType.SelectedValue = carTypeId;
                }
            }

            txtCarBrand.Text = row["car_brand"]?.ToString() ?? "--";
            txtModelYear.Text = row["model_year"]?.ToString() ?? "--";
            txtFuelType.Text = row["fuel_type_name"]?.ToString() ?? "--";

            int carNumberIdInt = Convert.ToInt32(selectedCarNumberId);
            float lastEndKilometer = GetLastCarKilometer(carNumberIdInt);
            decimal initialKilometer = GetVehicleInitialKilometer(carNumberIdInt);

            float startKMValue = lastEndKilometer > 0 ? lastEndKilometer : (float)initialKilometer;

            if (hdnLastKilometer != null)
                hdnLastKilometer.Value = startKMValue.ToString("0.0");

            if (startKMValue > 0)
            {
                txtStartKM.Text = startKMValue.ToString("0.0");
                string message = $"تم تحميل عداد البداية: {startKMValue:N0} كم";
                if (lastEndKilometer > 0)
                    message += " (من آخر أمر شغل)";
                else if (initialKilometer > 0)
                    message += " (العداد الابتدائي من إدارة المركبات)";
                else
                    message += " (يرجى التأكد من صحة القراءة)";

                if (!_loadingSaved)
                    ScriptManager.RegisterStartupScript(this, GetType(), "kmLoaded", $"alert('{message}');", true);
            }
            else
            {
                txtStartKM.Text = "";
                if (hdnLastKilometer != null) hdnLastKilometer.Value = "0";
                if (!_loadingSaved)
                    ScriptManager.RegisterStartupScript(this, GetType(), "kmWarning",
                        "alert('تنبيه: لم يتم تسجيل عداد ابتدائي لهذه السيارة. يرجى إدخال قراءة العداد يدوياً.');", true);
            }

            if (Session["EmployeesList"] != null) ReloadEmployeesScript();
            if (Session["SubscribersList"] != null) ReloadSubscribersScript();
        }

        // ===== ROUTE-FIRST FLOW: load ALL drivers assigned to this route (routes.driver_id -> drivers.driver_id -> employees) =====
        // ===== ROUTE-FIRST FLOW: load ALL drivers assigned to this route.
        // routes.driver_id stores employees.employee_id (NOT drivers.driver_id).
        private void LoadDriversByRoute(int routeId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadDriversByRoute called for route ID: {routeId}");

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT DISTINCT
                    e.employee_id  AS driver_id,
                    e.employee_name AS driver_name
                FROM routes r0
                INNER JOIN routes r
                        ON r.route_name = r0.route_name
                       AND ISNULL(r.start_loc, '') = ISNULL(r0.start_loc, '')
                       AND ISNULL(r.end_loc, '')   = ISNULL(r0.end_loc, '')
                       AND r.is_active = 1
                INNER JOIN employees e ON r.driver_id = e.employee_id
                WHERE r0.route_id = @routeId
                  AND e.is_active = 1
                ORDER BY e.employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlDriverName.Items.Clear();
                    ddlDriverName.Items.Add(new ListItem("-- اختر اسم السائق --", "0"));
                    ddlDriverName.DataSource = dt;
                    ddlDriverName.DataTextField = "driver_name";
                    ddlDriverName.DataValueField = "driver_id";
                    ddlDriverName.DataBind();

                    System.Diagnostics.Debug.WriteLine($"Total drivers found for route {routeId}: {dt.Rows.Count}");

                    if (dt.Rows.Count == 0)
                    {
                        if (!_loadingSaved)
                            ScriptManager.RegisterStartupScript(this, GetType(), "noDriversForRoute_" + routeId,
                                "alert('لا يوجد سائق مسند لخط السير هذا. الرجاء تعيين سائق لهذا الخط أولاً.');", true);
                    }
                    else if (dt.Rows.Count == 1 && !_loadingSaved)
                    {
                        ddlDriverName.SelectedValue = dt.Rows[0]["driver_id"].ToString();
                        System.Diagnostics.Debug.WriteLine($"Auto-selected route's driver: {dt.Rows[0]["driver_id"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading drivers by route: {ex.Message}");
            }
        }

        private void ClearSubscribersList()
        {
            hdnSelectedSubscribers.Value = "";
            string script = @"
        selectedSubscribers = [];
        if (typeof updateSubscribersList === 'function') {
            updateSubscribersList();
        }
        if (typeof updateHiddenField === 'function') {
            updateHiddenField();
        }";
            ScriptManager.RegisterStartupScript(this, GetType(), "clearSubscribers", script, true);
        }

        private void LoadOperationSubscribers(int operationCommandId)
        {
            try
            {
                List<string> selectedIds = new List<string>();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT subscriber_id 
                FROM operation_command_subscribers 
                WHERE operation_command_id = @opId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@opId", operationCommandId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // keep only numeric ids (safe to concatenate into IN (...))
                            if (int.TryParse(reader["subscriber_id"].ToString(), out int sid))
                                selectedIds.Add(sid.ToString());
                        }
                    }

                    if (selectedIds.Count > 0)
                    {
                        string ids = string.Join(",", selectedIds);
                        hdnSelectedSubscribers.Value = ids;

                        string namesQuery = @"
                    SELECT n_id, name, department, position 
                    FROM subscription_details 
                    WHERE n_id IN (" + ids + @")
                      AND (end_date >= GETDATE() OR end_date IS NULL)";

                        List<SubscriberItem> subscribers = new List<SubscriberItem>();
                        SqlCommand namesCmd = new SqlCommand(namesQuery, conn);
                        using (SqlDataReader namesReader = namesCmd.ExecuteReader())
                        {
                            while (namesReader.Read())
                            {
                                string displayName = namesReader["name"].ToString();
                                string department = namesReader["department"]?.ToString();
                                string position = namesReader["position"]?.ToString();

                                if (!string.IsNullOrEmpty(department) || !string.IsNullOrEmpty(position))
                                {
                                    displayName = $"{displayName} - {department} ({position})";
                                }

                                subscribers.Add(new SubscriberItem
                                {
                                    id = namesReader["n_id"].ToString(),
                                    name = displayName
                                });
                            }
                        }

                        string json = new JavaScriptSerializer().Serialize(subscribers);
                        string script = $@"
                    setTimeout(function() {{
                        selectedSubscribers = {json};
                        console.log('Loading previously selected subscribers:', selectedSubscribers);
                        if (typeof updateSubscribersList === 'function') {{
                            updateSubscribersList();
                        }}
                        if (typeof updateHiddenField === 'function') {{
                            updateHiddenField();
                        }}
                    }}, 200);";

                        ScriptManager.RegisterStartupScript(this, GetType(), "loadSelectedSubscribers", script, true);
                    }
                    else
                    {
                        hdnSelectedSubscribers.Value = "";
                        string script = @"
                    setTimeout(function() {
                        selectedSubscribers = [];
                        if (typeof updateSubscribersList === 'function') {
                            updateSubscribersList();
                        }
                        if (typeof updateHiddenField === 'function') {
                            updateHiddenField();
                        }
                    }, 200);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "clearSelectedSubscribers", script, true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading operation subscribers: {ex.Message}");
            }
        }

        private void LoadOperationEmployees(int operationCommandId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== Loading employees for mission {operationCommandId} ===");

                List<SubscriberItem> employeesWithNames = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT e.employee_id as id, e.employee_name as name, e.employee_position as position
                FROM operation_command_employees oce
                INNER JOIN employees e ON oce.employee_id = e.employee_id
                WHERE oce.operation_command_id = @opId
                ORDER BY e.employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@opId", operationCommandId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["name"].ToString();
                            string position = reader["position"]?.ToString();

                            if (!string.IsNullOrEmpty(position))
                            {
                                displayName = $"{displayName} - ({position})";
                            }

                            employeesWithNames.Add(new SubscriberItem
                            {
                                id = reader["id"].ToString(),
                                name = displayName
                            });
                        }
                    }
                }

                if (employeesWithNames.Count > 0)
                {
                    string ids = string.Join(",", employeesWithNames.Select(e => e.id));
                    hdnSelectedEmployees.Value = ids;

                    SelectedEmployeesList.Clear();
                    foreach (var emp in employeesWithNames)
                    {
                        SelectedEmployeesList.Add(emp);
                    }

                    string json = new JavaScriptSerializer().Serialize(employeesWithNames);

                    string script = $@"
                    setTimeout(function() {{
                        var employeesContainer = document.getElementById('employeesContainer');
                        if (employeesContainer) {{
                            employeesContainer.style.display = 'block';
                        }}
                        
                        selectedEmployees = {json};
                        
                        if (typeof updateEmployeesList === 'function') {{
                            updateEmployeesList();
                        }}
                        if (typeof updateEmployeesHiddenField === 'function') {{
                            updateEmployeesHiddenField();
                        }}
                    }}, 200);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "loadSelectedEmployees_" + operationCommandId, script, true);
                }
                else
                {
                    hdnSelectedEmployees.Value = "";
                    SelectedEmployeesList.Clear();
                    string script = @"
                    setTimeout(function() {
                        selectedEmployees = [];
                        if (typeof updateEmployeesList === 'function') {
                            updateEmployeesList();
                        }
                        if (typeof updateEmployeesHiddenField === 'function') {
                            updateEmployeesHiddenField();
                        }
                    }, 200);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "clearSelectedEmployees_" + operationCommandId, script, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading operation employees: {ex.Message}");
            }
        }

        #endregion

        #region Load Data Methods

        private void LoadAllData()
        {
            LoadDrivers();
            LoadRoutes();
            LoadGarageWriters();
            LoadVehicleObservers();
            LoadOperationManagers();
        }

        private void LoadCarStatistics(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    MAX(kilometer_end) as latest_km,
                    SUM(kilometer_end - kilometer_start) as total_distance,
                    COUNT(*) as trip_count
                FROM operation_command
                WHERE v_number = @CarId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CarId", carId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Statistics are not displayed anywhere yet.
                        // Read them here (latest_km, total_distance, trip_count) when a label exists for them.
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading car statistics: {ex.Message}");
            }
        }

        private void LoadCarNumbers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT DISTINCT
                    cn.car_number_id,
                    CASE 
                        WHEN cn.letter2 IS NULL AND cn.letter3 IS NULL 
                            THEN cn.letter1 + ' ' + cn.plate_numbers
                        WHEN cn.letter3 IS NULL 
                            THEN cn.letter1 + ' ' + cn.letter2 + ' ' + cn.plate_numbers
                        ELSE cn.letter1 + ' ' + cn.letter2 + ' ' + cn.letter3 + ' ' + cn.plate_numbers
                    END as full_plate_number,
                    ct.car_type_id,
                    ct.car_type_name,
                    ct.car_brand,
                    mvd.v_year as model_year,
                    ft.type as fuel_type_name,
                    mvd.initial_km,
                    mvd.current_odometer
                FROM main_vehicle_details mvd
                INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                INNER JOIN car_types ct ON mvd.v_type = ct.car_type_id
                LEFT JOIN fuel_type ft ON mvd.gas_type = ft.fuel_id
                WHERE mvd.deleted = 0 
                  AND (mvd.hide = 0 OR mvd.hide IS NULL)
                  AND cn.is_active = 1
                ORDER BY full_plate_number";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} cars");

                    Session["CarDetailsData"] = dt;

                    ddlCarNumber.Items.Clear();
                    ddlCarNumber.Items.Add(new ListItem("-- اختر رقم العربية --", ""));

                    ddlCarNumber.DataSource = dt;
                    ddlCarNumber.DataTextField = "full_plate_number";
                    ddlCarNumber.DataValueField = "car_number_id";
                    ddlCarNumber.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading car numbers: {ex.Message}");
                ddlCarNumber.Items.Clear();
                ddlCarNumber.Items.Add(new ListItem("-- خطأ في تحميل أرقام السيارات --", ""));
            }
        }

        private void LoadAllCarTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = "SELECT car_type_id, car_type_name FROM car_types WHERE is_active = 1 ORDER BY car_type_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlCarType.DataSource = dt;
                    ddlCarType.DataTextField = "car_type_name";
                    ddlCarType.DataValueField = "car_type_id";
                    ddlCarType.DataBind();

                    ddlCarType.Items.Insert(0, new ListItem("-- اختر نوع السيارة --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading car types: {ex.Message}");
            }
        }

        private void LoadMissionTypes()
        {
            try
            {
                ddlMissionType.Items.Clear();
                ddlMissionType.Items.Add(new ListItem("-- اختر نوع المأمورية --", "0"));
                ddlMissionType.Items.Add(new ListItem("مأمورية", "mission"));
                ddlMissionType.Items.Add(new ListItem("خط", "route"));
                ddlMissionType.AutoPostBack = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading mission types: {ex.Message}");
                ddlMissionType.Items.Clear();
                ddlMissionType.Items.Add(new ListItem("-- خطأ في تحميل البيانات --", "0"));
            }
        }

        private void LoadSubscribersList()
        {
            try
            {
                List<SubscriberItem> subscribers = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
        SELECT DISTINCT
            MIN(n_id) as n_id,
            name,
            MAX(phone) as phone,
            MAX(department) as department,
            MAX(position) as position
        FROM subscription_details 
        WHERE (hide = 0 OR hide IS NULL)
        GROUP BY name
        ORDER BY name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["name"].ToString();
                            string department = reader["department"]?.ToString();
                            string position = reader["position"]?.ToString();

                            if (!string.IsNullOrEmpty(department) || !string.IsNullOrEmpty(position))
                            {
                                displayName = $"{displayName} - {department} ({position})";
                            }

                            subscribers.Add(new SubscriberItem
                            {
                                id = reader["n_id"].ToString(),
                                name = displayName
                            });
                        }
                    }
                }

                Session["SubscribersList"] = subscribers;

                string json = new JavaScriptSerializer().Serialize(subscribers);
                hdnSubscribersJson.Value = json;

                string script = $@"
            window.subscribersList = {json};
            console.log('Subscribers loaded:', window.subscribersList);
            if (typeof loadSubscribersDropdown === 'function') {{
                loadSubscribersDropdown();
            }}";

                ScriptManager.RegisterStartupScript(this, GetType(), "loadSubscribers", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscribers: {ex.Message}");
            }
        }

        private void LoadEmployeesList()
        {
            try
            {
                List<SubscriberItem> employees = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    employee_id as id,
                    employee_name as name,
                    employee_position as position
                FROM employees 
                WHERE is_active = 1
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["name"].ToString();
                            string position = reader["position"]?.ToString();

                            if (!string.IsNullOrEmpty(position))
                            {
                                displayName = $"{displayName} - ({position})";
                            }

                            employees.Add(new SubscriberItem
                            {
                                id = reader["id"].ToString(),
                                name = displayName
                            });
                        }
                    }
                }

                Session["EmployeesList"] = employees;

                string json = new JavaScriptSerializer().Serialize(employees);
                string script = $@"
            window.employeesList = {json};
            console.log('Employees loaded into dropdown:', window.employeesList.length);
            if (typeof loadEmployeesDropdown === 'function') {{
                loadEmployeesDropdown();
            }}";

                ScriptManager.RegisterStartupScript(this, GetType(), "loadEmployees", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employees: {ex.Message}");
            }
        }

        private void BindSelectedEmployees()
        {
            string json = new JavaScriptSerializer().Serialize(SelectedEmployeesList);
            string script = $@"
                selectedEmployees = {json};
                if (typeof updateEmployeesList === 'function') {{ updateEmployeesList(); }}
                if (typeof updateEmployeesHiddenField === 'function') {{ updateEmployeesHiddenField(); }}";
            ScriptManager.RegisterStartupScript(this, GetType(), "bindSelectedEmployees", script, true);
        }

        private void SaveSelectedSubscribers(int operationCommandId, string selectedIds)
        {
            if (string.IsNullOrEmpty(selectedIds))
                return;

            string[] ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length == 0)
                return;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                // Make sure the table exists BEFORE deleting from it
                string checkTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'operation_command_subscribers')
                    BEGIN
                        CREATE TABLE operation_command_subscribers (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            operation_command_id INT NOT NULL,
                            subscriber_id INT NOT NULL,
                            created_date DATETIME NOT NULL DEFAULT GETDATE()
                        )
                    END";

                new SqlCommand(checkTableQuery, conn).ExecuteNonQuery();

                SqlCommand deleteCmd = new SqlCommand(
                    "DELETE FROM operation_command_subscribers WHERE operation_command_id = @opId", conn);
                deleteCmd.Parameters.AddWithValue("@opId", operationCommandId);
                deleteCmd.ExecuteNonQuery();

                foreach (string id in ids)
                {
                    if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int subscriberId))
                    {
                        try
                        {
                            string insertQuery = @"
                                INSERT INTO operation_command_subscribers 
                                (operation_command_id, subscriber_id, created_date)
                                VALUES (@opId, @subId, GETDATE())";

                            SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                            insertCmd.Parameters.AddWithValue("@opId", operationCommandId);
                            insertCmd.Parameters.AddWithValue("@subId", subscriberId);
                            insertCmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error saving subscriber {subscriberId}: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void SaveSelectedEmployees(int operationCommandId, string selectedIds)
        {
            if (string.IsNullOrEmpty(selectedIds))
                return;

            string[] ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length == 0)
                return;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'operation_command_employees')
                    BEGIN
                        CREATE TABLE operation_command_employees (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            operation_command_id INT NOT NULL,
                            employee_id INT NOT NULL,
                            created_date DATETIME NOT NULL DEFAULT GETDATE()
                        )
                    END";

                new SqlCommand(createTableQuery, conn).ExecuteNonQuery();

                SqlCommand deleteCmd = new SqlCommand(
                    "DELETE FROM operation_command_employees WHERE operation_command_id = @opId", conn);
                deleteCmd.Parameters.AddWithValue("@opId", operationCommandId);
                deleteCmd.ExecuteNonQuery();

                foreach (string id in ids)
                {
                    if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int employeeId))
                    {
                        string insertQuery = @"
                            INSERT INTO operation_command_employees 
                            (operation_command_id, employee_id, created_date)
                            VALUES (@opId, @empId, GETDATE())";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@opId", operationCommandId);
                        insertCmd.Parameters.AddWithValue("@empId", employeeId);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void LoadDrivers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    employee_id as driver_id, 
                    employee_name as driver_name
                FROM employees 
                WHERE is_active = 1
                  AND employee_position = N'سائق'
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlDriverName.Items.Clear();
                    ddlDriverName.DataSource = dt;
                    ddlDriverName.DataTextField = "driver_name";
                    ddlDriverName.DataValueField = "driver_id";
                    ddlDriverName.DataBind();
                    ddlDriverName.Items.Insert(0, new ListItem("-- اختر اسم السائق --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading drivers: {ex.Message}");
                ddlDriverName.Items.Clear();
                ddlDriverName.Items.Add(new ListItem("-- خطأ في تحميل السائقين --", "0"));
            }
        }

        private void LoadGarageWriters()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    employee_id, 
                    employee_name,
                    employee_position
                FROM employees 
                WHERE is_active = 1
                  AND (employee_position = N'كاتب جراج' 
                       OR employee_position = N'كاتب الجراج'
                       OR employee_position LIKE N'%كاتب%')
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlGarageWriter.DataSource = dt;
                        ddlGarageWriter.DataTextField = "employee_name";
                        ddlGarageWriter.DataValueField = "employee_id";
                        ddlGarageWriter.DataBind();
                    }
                    ddlGarageWriter.Items.Insert(0, new ListItem("-- اختر كاتب الجراج --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadGarageWriters: {ex.Message}");
                ddlGarageWriter.Items.Clear();
                ddlGarageWriter.Items.Add(new ListItem("-- خطأ في تحميل الكتاب --", "0"));
            }
        }

        private void LoadVehicleObservers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    employee_id, 
                    employee_name,
                    employee_position
                FROM employees 
                WHERE is_active = 1
                  AND (employee_position = N'ملاحظ مركبات' 
                       OR employee_position LIKE N'%ملاحظ%')
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlVehicleObserver.DataSource = dt;
                        ddlVehicleObserver.DataTextField = "employee_name";
                        ddlVehicleObserver.DataValueField = "employee_id";
                        ddlVehicleObserver.DataBind();
                    }
                    ddlVehicleObserver.Items.Insert(0, new ListItem("-- اختر ملاحظ المركبات --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadVehicleObservers: {ex.Message}");
                ddlVehicleObserver.Items.Clear();
                ddlVehicleObserver.Items.Add(new ListItem("-- خطأ في تحميل الملاحظين --", "0"));
            }
        }

        private void LoadOperationManagers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    employee_id, 
                    employee_name,
                    employee_position
                FROM employees 
                WHERE is_active = 1
                  AND (employee_position = N'مدير تشغيل' 
                       OR employee_position = N'مدير إدارة التشغيل'
                       OR employee_position LIKE N'%مدير%')
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ddlOperationManager.DataSource = dt;
                        ddlOperationManager.DataTextField = "employee_name";
                        ddlOperationManager.DataValueField = "employee_id";
                        ddlOperationManager.DataBind();
                    }
                    ddlOperationManager.Items.Insert(0, new ListItem("-- اختر مدير التشغيل --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadOperationManagers: {ex.Message}");
                ddlOperationManager.Items.Clear();
                ddlOperationManager.Items.Add(new ListItem("-- خطأ في تحميل المدراء --", "0"));
            }
        }

        // Reads routes directly from the 'routes' table.
        // A route can be several rows (one per car/driver) with the same name/start/end,
        // so they are grouped: ONE entry per route, using the lowest route_id of the group.
        private void LoadRoutes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT 
                    MIN(route_id) AS route_id, 
                    route_name,
                    start_loc,
                    end_loc,
                    MAX(description) AS description,
                    MAX(KM) AS KM,
                    route_name + ' - ' + ISNULL(start_loc, '') + ' - ' + ISNULL(end_loc, '') AS display_text
                FROM routes 
                WHERE is_active = 1
                GROUP BY route_name, start_loc, end_loc
                ORDER BY route_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    Session["RoutesData"] = dt;

                    ddlRoute.DataSource = dt;
                    ddlRoute.DataTextField = "display_text";
                    ddlRoute.DataValueField = "route_id";
                    ddlRoute.DataBind();

                    ddlRoute.Items.Insert(0, new ListItem("-- اختر خط السير --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading routes: {ex.Message}");
                ddlRoute.Items.Clear();
                ddlRoute.Items.Add(new ListItem("-- خطأ في تحميل خطوط السير --", "0"));
            }
        }

        #endregion

        #region Event Handlers

        private void ClearCarDetails()
        {
            try
            {
                if (ddlCarType != null && ddlCarType.Items.Count > 0)
                    ddlCarType.SelectedIndex = 0;

                if (txtCarBrand != null)
                    txtCarBrand.Text = "";

                if (txtModelYear != null)
                    txtModelYear.Text = "";

                if (txtFuelType != null)
                    txtFuelType.Text = "";

                if (ddlDriverName != null && ddlDriverName.Items.Count > 0)
                    ddlDriverName.SelectedIndex = 0;

                // No car chosen -> no fuel cards
                LoadFuelCardsForCar("");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ClearCarDetails: {ex.Message}");
            }
        }

        protected void ddlCarNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedCarNumberId = ddlCarNumber.SelectedValue;

                if (string.IsNullOrEmpty(selectedCarNumberId) || selectedCarNumberId == "0")
                {
                    ClearCarDetails();
                    return;
                }

                // ===== NEW: stop here if the car is in an active order =====
                if (BlockIfCarBusy(selectedCarNumberId))
                    return;
                // ===========================================================

                DataTable dt = Session["CarDetailsData"] as DataTable;

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rows = dt.Select($"car_number_id = {selectedCarNumberId}");

                    if (rows.Length > 0)
                    {
                        // NOTE: in the route-first flow, the driver dropdown is already populated
                        // (and auto-selected when there is only one) by LoadDriversByRoute,
                        // so the driver is not reset here.
                        ApplyCarRow(rows[0], selectedCarNumberId);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"No row found for car_number_id: {selectedCarNumberId}");
                        ClearCarDetails();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("CarDetailsData Session is null or empty");
                    ClearCarDetails();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ddlCarNumber_SelectedIndexChanged: {ex.Message}");
                ClearCarDetails();
            }
        }

        protected void ddlRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedRouteId = ddlRoute.SelectedValue;

                // The distance is re-filled below from the chosen route
                txtRouteKM.Text = "";

                if (string.IsNullOrEmpty(selectedRouteId) || selectedRouteId == "0")
                {
                    routeDescriptionContainer.Visible = false;
                    lblRouteDescription.Text = "";
                    Session["RouteSubscribers"] = null;
                    ClearSubscribersList();

                    ddlCarNumber.Items.Clear();
                    ddlCarNumber.Items.Add(new ListItem("-- اختر خط السير أولاً --", ""));
                    ddlDriverName.Items.Clear();
                    ddlDriverName.Items.Add(new ListItem("-- اختر خط السير أولاً --", "0"));
                    ClearCarDetails();
                    return;
                }

                // Get route details from Session (start_loc / end_loc / description)
                DataTable dt = Session["RoutesData"] as DataTable;
                if (dt != null)
                {
                    DataRow[] rows = dt.Select($"route_id = {selectedRouteId}");
                    if (rows.Length > 0)
                    {
                        string startLoc = rows[0]["start_loc"]?.ToString() ?? "";
                        string endLoc = rows[0]["end_loc"]?.ToString() ?? "";

                        // Route distance from routes.KM (read-only field)
                        if (rows[0]["KM"] != DBNull.Value)
                            txtRouteKM.Text = Convert.ToDecimal(rows[0]["KM"]).ToString("0.##", CultureInfo.InvariantCulture);

                        if (!string.IsNullOrEmpty(startLoc) || !string.IsNullOrEmpty(endLoc))
                        {
                            lblRouteDescription.Text = $"📌 {startLoc} - {endLoc}";
                            routeDescriptionContainer.Visible = true;
                        }
                        else
                        {
                            routeDescriptionContainer.Visible = false;
                        }
                    }
                }

                int routeId = Convert.ToInt32(selectedRouteId);

                // Reset car fields first, then load - auto-select (single car/driver) populates them last
                ClearCarDetails();
                txtStartKM.Text = "";

                // A different route means different subscribers: drop the previous selection
                ClearSubscribersList();

                LoadSubscribersByRoute(routeId);
                LoadCarsByRoute(routeId);     // all cars of the route -> auto-selects when there is only one
                LoadDriversByRoute(routeId);  // all drivers of the route -> auto-selects when there is only one
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ddlRoute_SelectedIndexChanged: {ex.Message}");
            }
        }

        protected void ddlMissionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedValue = ddlMissionType.SelectedValue;
                txtRouteKM.Text = "";

                Session["RouteSubscribers"] = null;

                routeDropdownContainer.Visible = false;
                missionTextContainer.Visible = false;

                if (subscribersContainer != null)
                    subscribersContainer.Visible = false;
                if (employeesContainer != null)
                    employeesContainer.Visible = false;

                if (selectedValue == "route")
                {
                    routeDropdownContainer.Visible = true;
                    if (subscribersContainer != null)
                        subscribersContainer.Visible = true;
                    txtMissionText.Text = "";
                    LoadSubscribersList();

                    if (ddlRoute != null)
                        ddlRoute.SelectedIndex = 0;

                    ddlCarNumber.Items.Clear();
                    ddlCarNumber.Items.Add(new ListItem("-- اختر خط السير أولاً --", ""));
                    ddlDriverName.Items.Clear();
                    ddlDriverName.Items.Add(new ListItem("-- اختر خط السير أولاً --", "0"));
                    ClearCarDetails();
                }
                else if (selectedValue == "mission")
                {
                    missionTextContainer.Visible = true;
                    if (employeesContainer != null)
                        employeesContainer.Visible = true;

                    if (ddlRoute != null)
                        ddlRoute.SelectedIndex = 0;

                    LoadCarNumbers();
                    LoadDrivers();

                    ClearCarDetails();

                    LoadEmployeesList();
                    ReloadEmployeesScript();

                    if (SelectedEmployeesList.Count > 0)
                    {
                        BindSelectedEmployees();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ddlMissionType_SelectedIndexChanged: {ex.Message}");
            }
        }

        #endregion

        #region Save Methods

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetTab("tabForm");

                int? currentMissionId = null;

                if (!ValidateWorkOrder(currentMissionId))
                    return;

                string missionType = ddlMissionType.SelectedValue;
                string selectedItems = "";

                if (missionType == "route")
                {
                    selectedItems = hdnSelectedSubscribers.Value;
                    if (string.IsNullOrEmpty(selectedItems))
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "warning",
                            "alert('الرجاء اختيار مشترك واحد على الأقل');", true);
                        return;
                    }
                }
                else if (missionType == "mission")
                {
                    selectedItems = txtEmployees.Text.Trim();

                    if (string.IsNullOrEmpty(selectedItems))
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "warning",
                            "alert('الرجاء إدخال موظف واحد على الأقل');", true);
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "warning",
                        "alert('الرجاء اختيار نوع المأمورية');", true);
                    return;
                }

                string carNumberId = ddlCarNumber.SelectedValue;
                string routeId = ddlRoute.SelectedValue;
                string missionText = txtMissionText.Text.Trim();

                int missionId = SaveOperationCommand(
                    missionType,
                    ddlDriverName.SelectedValue,
                    ddlDriverName.SelectedItem.Text,
                    carNumberId,
                    ddlCarType.SelectedValue,
                    ddlCarType.SelectedItem.Text,
                    routeId,
                    missionText,
                    selectedItems,
                    txtStartTime.Text,
                    txtReturnTime.Text,
                    txtStartKM.Text,
                    txtEndKM.Text,
                    ddlGarageWriter.SelectedValue,
                    ddlGarageWriter.SelectedItem.Text,
                    ddlVehicleObserver.SelectedValue,
                    ddlVehicleObserver.SelectedItem.Text,
                    ddlOperationManager.SelectedValue,
                    ddlOperationManager.SelectedItem.Text
                );

                if (missionId > 0)
                {
                    if (missionType == "route")
                    {
                        SaveSelectedSubscribers(missionId, selectedItems);
                    }
                    // mission type: employees are already saved in the employees_text column
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "btnSaveError",
                    $"alert('خطأ أثناء الحفظ: {ex.Message.Replace("'", "\\'")}');",
                    true
                );
            }
        }

        private bool ValidateWorkOrder(int? currentMissionId = null)
        {
            bool isValid = true;
            string errorMessage = "";
            string missionType = ddlMissionType.SelectedValue;

            if (ddlMissionType.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار نوع المأمورية<br/>";

            if (string.IsNullOrEmpty(ddlCarNumber.SelectedValue))
                errorMessage += "• الرجاء اختيار رقم العربية<br/>";

            if (ddlCarType.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار نوع السيارة<br/>";

            if (ddlDriverName.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار اسم السائق<br/>";

            if (ddlMissionType.SelectedValue == "route" && ddlRoute.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار خط السير<br/>";

            if (ddlMissionType.SelectedValue == "mission" && string.IsNullOrEmpty(txtMissionText.Text.Trim()))
                errorMessage += "• الرجاء إدخال نص المأمورية<br/>";

            if (ddlGarageWriter.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار كاتب الجراج<br/>";

            if (ddlVehicleObserver.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار ملاحظ المركبات<br/>";

            if (ddlOperationManager.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار مدير التشغيل<br/>";

            if (missionType == "route" && string.IsNullOrEmpty(hdnSelectedSubscribers.Value))
                errorMessage += "• الرجاء اختيار مشترك واحد على الأقل<br/>";

            if (missionType == "mission" && string.IsNullOrEmpty(txtEmployees.Text.Trim()))
                errorMessage += "• الرجاء إدخال موظف واحد على الأقل<br/>";
            if (ddlDriverName.SelectedValue != "0")
            {
                string licMsg;
                int licLevel = CheckDriverLicense(ddlDriverName.SelectedValue, out licMsg);
                if (licLevel == 2 && BlockExpiredLicense)
                    errorMessage += "• " + licMsg.Replace("'", "\\'") + "<br/>";
            }
            if (!string.IsNullOrEmpty(ddlCarNumber.SelectedValue))
            {
                DataRow busy = GetActiveOrderForCar(ddlCarNumber.SelectedValue);
                if (busy != null)
                    errorMessage += "• " + BuildBusyMessage(busy) + "<br/>";
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validationError",
                    $"alert('الرجاء تصحيح الأخطاء التالية:\\n\\n{errorMessage.Replace("<br/>", "\\n")}');", true);
                isValid = false;
            }

            return isValid;
        }

        private int SaveOperationCommand(
            string missionType,
            string driverId, string driverName, string carNumberId,
            string carTypeId, string carTypeName,
            string routeId,
            string missionText,
            string selectedItems,
            string startTime, string returnTime,
            string startKm, string endKm, string garageWriterId, string garageWriterName,
            string vehicleObserverId, string vehicleObserverName,
            string operationManagerId, string operationManagerName)
        {
            int missionId = 0;

            try
            {
                System.Diagnostics.Debug.WriteLine("=== SAVE OPERATION COMMAND STARTED ===");

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    int vNumberId = int.Parse(carNumberId);
                    int driverIdInt = int.Parse(driverId);
                    int carTypeIdInt = int.Parse(carTypeId);
                    int garageWriterIdInt = int.Parse(garageWriterId);
                    int vehicleObserverIdInt = int.Parse(vehicleObserverId);
                    int operationManagerIdInt = int.Parse(operationManagerId);

                    object dbStartTime = DBNull.Value;
                    object dbEndTime = DBNull.Value;

                    if (!string.IsNullOrEmpty(startTime))
                    {
                        if (DateTime.TryParse(startTime, out DateTime parsedStartTime))
                            dbStartTime = parsedStartTime;
                    }

                    if (!string.IsNullOrEmpty(returnTime))
                    {
                        if (DateTime.TryParse(returnTime, out DateTime parsedEndTime))
                            dbEndTime = parsedEndTime;
                    }

                    DateTime currentDate = DateTime.Today;

                    int? finalMissionTypeId = null;
                    int? finalRouteId = null;
                    string finalMissionDescription = null;

                    if (missionType == "route")
                    {
                        finalMissionTypeId = 2;
                        if (!string.IsNullOrEmpty(routeId) && routeId != "0")
                        {
                            finalRouteId = int.Parse(routeId);
                        }
                    }
                    else if (missionType == "mission")
                    {
                        finalMissionTypeId = 1;
                        if (!string.IsNullOrEmpty(missionText))
                        {
                            finalMissionDescription = missionText;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "missionTextWarning",
                                "alert('الرجاء إدخال نص المأمورية');", true);
                            return 0;
                        }
                    }

                    if (!finalMissionTypeId.HasValue)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "missionTypeError",
                            "alert('الرجاء اختيار نوع المأمورية');", true);
                        return 0;
                    }

                    string insertQuery = @"
                INSERT INTO operation_command
                (
                    v_number,
                    v_type,
                    date,
                    driver_name,
                    mission_type,
                    location_point,
                    mission_description,
                    employees_text,
                    subscribers_name,
                    mission_starting_time,
                    mission_ending_time,
                    kilometer_start,
                    kilometer_end,
                    grage_writer_name,
                    vehicle_observar,
                    opration_manager_name,
                    fuel_card_id,
                    reg_date,
                    deleted,
                    hide
                )
                VALUES
                (
                    @vNumber,
                    @vType,
                    @date,
                    @driverId,
                    @missionTypeId,
                    @routeId,
                    @missionDescription,
                    @employeesText,
                    NULL,
                    @startTime,
                    @endTime,
                    @kmStart,
                    @kmEnd,
                    @garageWriterId,
                    @observerId,
                    @managerId,
                    @fuelCardId,
                    GETDATE(),
                    0,
                    0
                );
                SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);

                    cmd.Parameters.Add("@vNumber", SqlDbType.Int).Value = vNumberId;
                    cmd.Parameters.Add("@vType", SqlDbType.Int).Value = carTypeIdInt;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = currentDate;
                    cmd.Parameters.Add("@driverId", SqlDbType.Int).Value = driverIdInt;
                    cmd.Parameters.Add("@missionTypeId", SqlDbType.Int).Value = finalMissionTypeId.Value;

                    if (finalRouteId.HasValue)
                        cmd.Parameters.Add("@routeId", SqlDbType.Int).Value = finalRouteId.Value;
                    else
                        cmd.Parameters.Add("@routeId", SqlDbType.Int).Value = DBNull.Value;

                    if (!string.IsNullOrEmpty(finalMissionDescription))
                        cmd.Parameters.Add("@missionDescription", SqlDbType.NVarChar).Value = finalMissionDescription;
                    else
                        cmd.Parameters.Add("@missionDescription", SqlDbType.NVarChar).Value = DBNull.Value;

                    cmd.Parameters.Add("@employeesText", SqlDbType.NVarChar).Value =
                        missionType == "mission" ? (object)selectedItems : DBNull.Value;

                    cmd.Parameters.Add("@startTime", SqlDbType.DateTime).Value = dbStartTime;
                    cmd.Parameters.Add("@endTime", SqlDbType.DateTime).Value = dbEndTime;

                    // Kilometers: empty or 0 is saved as NULL
                    var pStart = cmd.Parameters.Add("@kmStart", SqlDbType.Decimal);
                    pStart.Precision = 18; pStart.Scale = 2;
                    pStart.Value = ToDbDecimal(startKm);

                    var pEnd = cmd.Parameters.Add("@kmEnd", SqlDbType.Decimal);
                    pEnd.Precision = 18; pEnd.Scale = 2;
                    pEnd.Value = ToDbDecimal(endKm);

                    cmd.Parameters.Add("@garageWriterId", SqlDbType.Int).Value = garageWriterIdInt;
                    cmd.Parameters.Add("@observerId", SqlDbType.Int).Value = vehicleObserverIdInt;
                    cmd.Parameters.Add("@managerId", SqlDbType.Int).Value = operationManagerIdInt;

                    // Fuel card (optional): NULL when none is chosen
                    cmd.Parameters.Add("@fuelCardId", SqlDbType.Int).Value =
                        (!string.IsNullOrEmpty(ddlFuelCard.SelectedValue) && ddlFuelCard.SelectedValue != "0")
                            ? (object)int.Parse(ddlFuelCard.SelectedValue) : DBNull.Value;

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        missionId = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine($"SUCCESS: Inserted operation_command with mission_id: {missionId}");

                        string successMessage = $"تم حفظ أمر الشغل رقم {missionId} بنجاح";
                        ScriptManager.RegisterStartupScript(
                            this, GetType(), "success",
                            $"alert('{successMessage.Replace("'", "\\'")}');",
                            true
                        );
                    }

                    // Subscribers must be saved BEFORE the form is cleared (the hidden field is wiped by ClearForm)
                    if (missionId > 0 && missionType == "route")
                    {
                        SaveSelectedSubscribers(missionId, selectedItems);
                    }

                    ClearForm();
                    System.Diagnostics.Debug.WriteLine("=== SAVE OPERATION COMMAND COMPLETED ===");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in SaveOperationCommand: {ex.Message}");
                string errorDetails = $"خطأ في الحفظ: {ex.Message}";
                ScriptManager.RegisterStartupScript(
                    this, GetType(), "error",
                    $"alert('{errorDetails.Replace("'", "\\'")}');", true);
            }

            return missionId;
        }

        #endregion

        #region Helper Methods

        protected void btnClear_Click(object sender, EventArgs e)
        {
            SetTab("tabForm");
            ClearForm();
        }

        private void ClearForm()
        {
            try
            {
                ddlMissionType.SelectedIndex = 0;
                ddlCarType.SelectedIndex = 0;
                ddlGarageWriter.SelectedIndex = 0;
                ddlVehicleObserver.SelectedIndex = 0;
                ddlOperationManager.SelectedIndex = 0;
                ddlRoute.SelectedIndex = 0;

                Session["RouteSubscribers"] = null;

                ddlCarNumber.Items.Clear();
                ddlCarNumber.Items.Add(new ListItem("-- اختر نوع المأمورية أولاً --", ""));
                ddlDriverName.Items.Clear();
                ddlDriverName.Items.Add(new ListItem("-- اختر نوع المأمورية أولاً --", "0"));

                routeDropdownContainer.Visible = false;
                missionTextContainer.Visible = false;
                routeDescriptionContainer.Visible = false;

                if (subscribersContainer != null)
                    subscribersContainer.Visible = false;
                if (employeesContainer != null)
                    employeesContainer.Visible = false;

                txtMissionText.Text = "";
                txtEmployees.Text = "";
                lblRouteDescription.Text = "";
                txtRouteKM.Text = "";
                LoadFuelCardsForCar("");

                txtCarBrand.Text = "";
                txtModelYear.Text = "";
                txtFuelType.Text = "";

                hdnSelectedSubscribers.Value = "";
                hdnSelectedEmployees.Value = "";
                hdnSubscribersJson.Value = "";

                SelectedEmployeesList.Clear();

                txtStartTime.Text = "";
                txtReturnTime.Text = "";
                txtStartKM.Text = "";
                txtEndKM.Text = "";

                string script = @"
                    selectedSubscribers = []; 
                    selectedEmployees = [];
                    routeSubscribers = [];
                    if (typeof updateSubscribersList === 'function') { updateSubscribersList(); }
                    if (typeof updateEmployeesList === 'function') { updateEmployeesList(); }
                    if (typeof updateHiddenField === 'function') { updateHiddenField(); }
                    if (typeof updateEmployeesHiddenField === 'function') { updateEmployeesHiddenField(); }
                    if (typeof loadSubscribersDropdown === 'function') {
                        loadSubscribersDropdown();
                    }";

                ScriptManager.RegisterStartupScript(this, GetType(), "clearSubscribers", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ClearForm: {ex.Message}");
            }
        }

        [WebMethod]
        public static string GetCarNumberSuggestions(string searchText)
        {
            try
            {
                List<CarNumberSuggestion> suggestions = new List<CarNumberSuggestion>();

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT TOP 10 plate_number, description
                        FROM car_numbers 
                        WHERE (plate_number LIKE @searchText 
                           OR description LIKE @searchText)
                           AND is_active = 1
                        ORDER BY plate_number";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suggestions.Add(new CarNumberSuggestion
                            {
                                plate_number = reader["plate_number"].ToString(),
                                description = reader["description"].ToString()
                            });
                        }
                    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(suggestions);
            }
            catch
            {
                return "[]";
            }
        }

        // Last real end-kilometer of the car (NULL / 0 are ignored)
        private float GetLastCarKilometer(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT TOP 1 kilometer_end
                FROM operation_command
                WHERE v_number = @CarId
                  AND kilometer_end IS NOT NULL
                  AND kilometer_end > 0
                  AND deleted = 0
                ORDER BY ISNULL(mission_ending_time, date) DESC, mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CarId", carId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToSingle(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting last car kilometer: {ex.Message}");
            }

            return 0;
        }

        private bool IsCarActive(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = "SELECT COUNT(*) FROM car_numbers WHERE car_number_id = @carId AND is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carId", carId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking car active status: {ex.Message}");
                return true;
            }
        }

        private bool IsDriverActive(int driverId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = "SELECT COUNT(*) FROM employees WHERE employee_id = @driverId AND is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@driverId", driverId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking driver active status: {ex.Message}");
                return true;
            }
        }

        private decimal GetVehicleInitialKilometer(int carNumberId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"SELECT TOP 1 initial_km FROM main_vehicle_details WHERE v_num = @CarNumberId AND deleted = 0 ORDER BY v_id DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting vehicle initial kilometer: {ex.Message}");
            }
            return 0;
        }

        private bool HasTimeConflict(int carId, int driverId, DateTime startTime, DateTime endTime, int? currentMissionId = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                SELECT COUNT(*)
                FROM operation_command
                WHERE (v_number = @CarId OR driver_name = @DriverId)
                  AND (
                      (mission_starting_time <= @EndTime AND mission_ending_time >= @StartTime)
                  )";

                    if (currentMissionId.HasValue)
                    {
                        query += " AND mission_id != @MissionId";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CarId", carId);
                    cmd.Parameters.AddWithValue("@DriverId", driverId);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);

                    if (currentMissionId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@MissionId", currentMissionId.Value);
                    }

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking time conflict: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}