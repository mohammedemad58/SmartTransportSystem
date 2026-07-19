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
        // Add this property to persist selected employees across postbacks
        private List<SubscriberItem> SelectedEmployeesList
        {
            get
            {
                if (ViewState["SelectedEmployeesList"] == null)
                    return new List<SubscriberItem>();
                return (List<SubscriberItem>)ViewState["SelectedEmployeesList"];
            }
            set
            {
                ViewState["SelectedEmployeesList"] = value;
            }
        }
        protected void ddlDriverName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // This will be called when driver name is changed
            // You can add logic here if needed
            // For example, load missions or update other fields
            System.Diagnostics.Debug.WriteLine("Driver name changed to: " + ddlDriverName.SelectedValue);
        }

        protected void ddlGarageWriter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle garage writer change if needed
            System.Diagnostics.Debug.WriteLine("Garage writer changed to: " + ddlGarageWriter.SelectedValue);
        }

        protected void ddlVehicleObserver_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle vehicle observer change if needed
            System.Diagnostics.Debug.WriteLine("Vehicle observer changed to: " + ddlVehicleObserver.SelectedValue);
        }

        protected void ddlOperationManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle operation manager change if needed
            System.Diagnostics.Debug.WriteLine("Operation manager changed to: " + ddlOperationManager.SelectedValue);
        }
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            // Get the current values from the form
            string carNumber = ddlCarNumber.SelectedItem?.Text ?? "";
            string carType = ddlCarType.SelectedItem?.Text ?? "";
            string driverName = ddlDriverName.SelectedItem?.Text ?? "";
            string carBrand = txtCarBrand.Text;
            string modelYear = txtModelYear.Text;
            string fuelType = txtFuelType.Text;
            string missionType = ddlMissionType.SelectedItem?.Text ?? "";
            string routeName = ddlRoute.SelectedItem?.Text ?? "";
            string missionText = txtMissionText.Text;
            string employees = txtEmployees.Text;
            string startTime = txtStartTime.Text;
            string returnTime = txtReturnTime.Text;
            string startKM = txtStartKM.Text;
            string endKM = txtEndKM.Text;
            string garageWriter = ddlGarageWriter.SelectedItem?.Text ?? "";
            string vehicleObserver = ddlVehicleObserver.SelectedItem?.Text ?? "";
            string operationManager = ddlOperationManager.SelectedItem?.Text ?? "";

            // Get subscribers list
            string subscribers = "";
            string[] subscriberIds = hdnSelectedSubscribers.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (subscriberIds.Length > 0)
            {
                // You may want to get subscriber names from database
                subscribers = string.Join(", ", subscriberIds);
            }

            // Build HTML for printing
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
        </div>
        
        <!-- بيانات المهمة -->
        <div class='section-header'>بيانات المهمة</div>
        <div class='row'>
            <div class='col-md-4'><div class='form-label'>نوع المأمورية</div><div class='value-display'>{EscapeHtml(missionType)}</div></div>";

            if (missionType == "خط")
            {
                printHtml += $@"
            <div class='col-md-4'><div class='form-label'>خط السير</div><div class='value-display'>{EscapeHtml(routeName)}</div></div>";
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

            // Write the HTML to the response and print
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write(printHtml);
            Response.End();
        }

        private string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text)) return "--";
            return System.Security.SecurityElement.Escape(text);
        }
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

                // Initialize selected employees from ViewState
                if (SelectedEmployeesList.Count > 0)
                {
                    BindSelectedEmployees();
                }
            }
            else
            {
                // IMPORTANT: Reload employees list on every postback to maintain JavaScript variable
                if (Session["EmployeesList"] != null)
                {
                    ReloadEmployeesScript();
                }
                if (Session["SubscribersList"] != null)
                {
                    ReloadSubscribersScript();
                }
            }

            // Handle postback from employee selection
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

        #region Search Methods

        protected void btnSearchOperations_Click(object sender, EventArgs e)
        {
            try
            {
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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

        private void SearchOperationCommands(string carNumber, string driverName, string searchDate)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
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
                                WHEN r.description IS NOT NULL AND r.description != '' 
                                THEN ' - ' + r.description 
                                ELSE '' 
                            END
                        ELSE NULL
                    END as mission_details,
                    oc.kilometer_start,
                    oc.kilometer_end,
                    (oc.kilometer_end - oc.kilometer_start) as trip_distance,
                    CAST((oc.kilometer_end - oc.kilometer_start) AS NVARCHAR(10)) + N' كم' as distance_display,
                    MAX(oc.kilometer_end) OVER (PARTITION BY oc.v_number) as latest_car_km
                FROM operation_command oc
                LEFT JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                LEFT JOIN employees e ON oc.driver_name = e.employee_id
                LEFT JOIN routes r ON oc.location_point = r.route_id AND oc.mission_type = 2
                WHERE 1=1";

                    if (!string.IsNullOrEmpty(carNumber))
                    {
                        query += @" AND (cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers LIKE @carNumber 
                    OR cn.letter1 + ' ' + cn.letter2 + ' ' + ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers LIKE @carNumber)";
                    }

                    if (!string.IsNullOrEmpty(driverName))
                    {
                        query += " AND e.employee_name LIKE @driverName";
                    }

                    if (!string.IsNullOrEmpty(searchDate))
                    {
                        if (DateTime.TryParseExact(searchDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchStart))
                        {
                            DateTime searchEnd = searchStart.AddDays(1);
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

                    if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParseExact(searchDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchStartValid))
                    {
                        DateTime searchEnd = searchStartValid.AddDays(1);
                        cmd.Parameters.AddWithValue("@searchStart", searchStartValid);
                        cmd.Parameters.AddWithValue("@searchEnd", searchEnd);
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

        private void LoadOperationCommand(int missionId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
SELECT 
    oc.*,
    cn.letter1,
    cn.letter2,
    cn.letter3,
    cn.plate_numbers,
    cn.car_number_id,
    e_driver.employee_id as driver_id,
    e_driver.employee_name as driver_name_text,
    e_garage.employee_name as garage_writer_text,
    e_observer.employee_name as vehicle_observer_text,
    e_manager.employee_name as operation_manager_text,
    r.route_name,
    r.description as route_description
FROM operation_command oc
INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
INNER JOIN employees e_driver ON oc.driver_name = e_driver.employee_id
LEFT JOIN employees e_garage ON oc.grage_writer_name = e_garage.employee_id
LEFT JOIN employees e_observer ON oc.vehicle_observar = e_observer.employee_id
LEFT JOIN employees e_manager ON oc.opration_manager_name = e_manager.employee_id
LEFT JOIN routes r ON oc.location_point = r.route_id AND oc.mission_type = 2
WHERE oc.mission_id = @missionId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", missionId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // ===== 1. LOAD CAR NUMBER =====
                        string carNumberId = reader["car_number_id"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(carNumberId))
                        {
                            ddlCarNumber.SelectedValue = carNumberId;

                            DataTable dt = Session["CarDetailsData"] as DataTable;
                            if (dt != null)
                            {
                                DataRow[] rows = dt.Select($"car_number_id = {carNumberId}");
                                if (rows.Length > 0)
                                {
                                    if (rows[0]["car_type_id"] != DBNull.Value)
                                    {
                                        string carTypeId = rows[0]["car_type_id"].ToString();
                                        ListItem item = ddlCarType.Items.FindByValue(carTypeId);
                                        if (item != null)
                                        {
                                            ddlCarType.SelectedValue = carTypeId;
                                        }
                                    }

                                    string carBrand = GetCarBrand(carNumberId);
                                    txtCarBrand.Text = carBrand ?? "--";
                                    txtModelYear.Text = rows[0]["model_year"]?.ToString() ?? "--";
                                    txtFuelType.Text = rows[0]["fuel_type_name"]?.ToString() ?? "--";
                                }
                            }
                        }

                        // ===== 2. LOAD DRIVER NAME =====
                        string driverId = reader["driver_id"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(driverId) && driverId != "0")
                        {
                            SafeSetDropdownValue(ddlDriverName, driverId);
                            System.Diagnostics.Debug.WriteLine($"Driver ID set to: {driverId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No driver ID found in the record");
                        }

                        // ===== 3. LOAD PERSONNEL =====
                        SafeSetDropdownValue(ddlGarageWriter, reader["grage_writer_name"]?.ToString());
                        SafeSetDropdownValue(ddlVehicleObserver, reader["vehicle_observar"]?.ToString());
                        SafeSetDropdownValue(ddlOperationManager, reader["opration_manager_name"]?.ToString());

                        // ===== 4. LOAD DATES AND TIMES =====
                        if (reader["mission_starting_time"] != DBNull.Value)
                            txtStartTime.Text = Convert.ToDateTime(reader["mission_starting_time"]).ToString("yyyy-MM-ddTHH:mm");

                        if (reader["mission_ending_time"] != DBNull.Value)
                        {
                            string endTime = reader["mission_ending_time"].ToString();
                            if (DateTime.TryParse(endTime, out DateTime endDateTime))
                            {
                                txtReturnTime.Text = endDateTime.ToString("yyyy-MM-ddTHH:mm");
                            }
                        }

                        // ===== 5. LOAD KILOMETERS =====
                        txtStartKM.Text = reader["kilometer_start"]?.ToString() ?? "";
                        txtEndKM.Text = reader["kilometer_end"]?.ToString() ?? "";

                        // Store last kilometer for validation
                        int carId = Convert.ToInt32(carNumberId);
                        float lastKilometer = GetLastCarKilometer(carId);
                        if (hdnLastKilometer != null)
                        {
                            hdnLastKilometer.Value = lastKilometer.ToString("0.0");
                        }

                        // ===== 6. LOAD MISSION TYPE AND RELATED DATA =====
                        string missionTypeValue = reader["mission_type"]?.ToString() ?? "0";

                        if (missionTypeValue == "1") // مأمورية
                        {
                            System.Diagnostics.Debug.WriteLine("Loading MISSION type work order");

                            // Set dropdown value
                            ddlMissionType.SelectedValue = "mission";

                            // Show/hide containers
                            routeDropdownContainer.Visible = false;
                            missionTextContainer.Visible = true;

                            if (employeesContainer != null)
                            {
                                employeesContainer.Visible = true;
                                employeesContainer.Style.Add("display", "block");
                            }
                            if (subscribersContainer != null)
                            {
                                subscribersContainer.Visible = false;
                                subscribersContainer.Style.Add("display", "none");
                            }

                            // Load mission text
                            string missionText = reader["mission_description"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(missionText))
                            {
                                txtMissionText.Text = missionText;
                                System.Diagnostics.Debug.WriteLine($"Mission text loaded: {txtMissionText.Text}");
                            }
                            else
                            {
                                txtMissionText.Text = "";
                            }

                            // ===== IMPORTANT: Load employees text =====
                            string employeesText = reader["employees_text"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(employeesText))
                            {
                                txtEmployees.Text = employeesText;
                                System.Diagnostics.Debug.WriteLine($"Employees text loaded: {txtEmployees.Text}");
                            }
                            else
                            {
                                txtEmployees.Text = "";
                                System.Diagnostics.Debug.WriteLine("No employees text found");
                            }

                            // Hide route description
                            routeDescriptionContainer.Visible = false;
                            lblRouteDescription.Text = "";

                            ScriptManager.RegisterStartupScript(this, GetType(), "loadSuccess",
                                "alert('تم تحميل بيانات أمر الشغل (مأمورية) بنجاح');", true);
                        }
                        else if (missionTypeValue == "2") // خط
                        {
                            System.Diagnostics.Debug.WriteLine("Loading ROUTE type work order");

                            // Set dropdown value
                            ddlMissionType.SelectedValue = "route";

                            // Show/hide containers
                            routeDropdownContainer.Visible = true;
                            missionTextContainer.Visible = false;

                            if (subscribersContainer != null)
                            {
                                subscribersContainer.Visible = true;
                                subscribersContainer.Style.Add("display", "block");
                            }
                            if (employeesContainer != null)
                            {
                                employeesContainer.Visible = false;
                                employeesContainer.Style.Add("display", "none");
                            }

                            txtMissionText.Text = "";

                            // Load the route from location_point
                            if (reader["location_point"] != DBNull.Value)
                            {
                                string routeValue = reader["location_point"].ToString();
                                SafeSetDropdownValue(ddlRoute, routeValue);

                                // ===== ADD THIS: Load subscribers for this route into dropdown =====
                                int routeId = Convert.ToInt32(routeValue);
                                LoadSubscribersByRoute(routeId);

                                // Show route description
                                if (reader["route_description"] != DBNull.Value)
                                {
                                    string description = reader["route_description"].ToString();
                                    if (!string.IsNullOrEmpty(description))
                                    {
                                        lblRouteDescription.Text = $"📌 {description}";
                                        routeDescriptionContainer.Visible = true;
                                    }
                                    else
                                    {
                                        routeDescriptionContainer.Visible = false;
                                    }
                                }
                                else
                                {
                                    routeDescriptionContainer.Visible = false;
                                }

                                // ===== ADD THIS: Load previously selected subscribers for this mission =====
                                LoadOperationSubscribers(missionId);
                            }
                            else
                            {
                                ddlRoute.SelectedIndex = 0;
                                routeDescriptionContainer.Visible = false;
                                ClearSubscribersList();
                            }

                            ScriptManager.RegisterStartupScript(this, GetType(), "loadSuccess",
                                "alert('تم تحميل بيانات أمر الشغل (خط) بنجاح');", true);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Unknown mission type: {missionTypeValue}");
                            ddlMissionType.SelectedIndex = 0;
                            routeDropdownContainer.Visible = false;
                            missionTextContainer.Visible = false;
                            if (subscribersContainer != null) subscribersContainer.Visible = false;
                            if (employeesContainer != null) employeesContainer.Visible = false;
                            routeDescriptionContainer.Visible = false;
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "loadError",
                            "alert('لم يتم العثور على بيانات أمر الشغل');", true);
                    }
                }
            }
        }

        // Add this helper method to load subscribers by route
        // Add this helper method to load subscribers by route (add this once)
        private void LoadSubscribersByRoute(int routeId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadSubscribersByRoute called for route ID: {routeId}");

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                List<SubscriberItem> subscribers = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    n_id as id,
                    name,
                    department,
                    position
                FROM subscription_details 
                WHERE route_id = @routeId
                  AND (end_date >= GETDATE() OR end_date IS NULL)
                  AND (hide = 0 OR hide IS NULL)
                ORDER BY name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

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

                        System.Diagnostics.Debug.WriteLine($"Found subscriber: {displayName} (ID: {reader["id"]})");
                    }
                    reader.Close();
                }

                System.Diagnostics.Debug.WriteLine($"Total subscribers found for route {routeId}: {subscribers.Count}");

                // Store in Session for later use
                Session["RouteSubscribers"] = subscribers;

                // Send to JavaScript
                string json = new JavaScriptSerializer().Serialize(subscribers);
                string script = $@"
            window.routeSubscribers = {json};
            console.log('Route subscribers loaded:', window.routeSubscribers);
            console.log('Number of subscribers:', {subscribers.Count});
            if (typeof loadRouteSubscribersDropdown === 'function') {{
                console.log('Calling loadRouteSubscribersDropdown');
                loadRouteSubscribersDropdown();
            }} else {{
                console.log('loadRouteSubscribersDropdown function not found');
            }}";

                ScriptManager.RegisterStartupScript(this, GetType(), "loadRouteSubscribers_" + routeId, script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscribers by route: {ex.Message}");
            }
        }

        // Add this helper method to clear subscribers list (add this once)
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
        // Add this helper method to clear subscribers list

        private void LoadOperationSubscribers(int operationCommandId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                List<string> selectedIds = new List<string>();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT subscriber_id 
                FROM operation_command_subscribers 
                WHERE operation_command_id = @opId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@opId", operationCommandId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        selectedIds.Add(reader["subscriber_id"].ToString());
                    }
                    reader.Close();

                    if (selectedIds.Count > 0)
                    {
                        string ids = string.Join(",", selectedIds);
                        hdnSelectedSubscribers.Value = ids;

                        string namesQuery = @"
                    SELECT n_id, name, department, position 
                    FROM subscription_details 
                    WHERE n_id IN (" + ids + @")
                      AND (end_date >= GETDATE() OR end_date IS NULL)";

                        SqlCommand namesCmd = new SqlCommand(namesQuery, conn);
                        SqlDataReader namesReader = namesCmd.ExecuteReader();

                        List<SubscriberItem> subscribers = new List<SubscriberItem>();
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
                        namesReader.Close();

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
                        // Clear subscribers if none selected
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

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                List<SubscriberItem> employeesWithNames = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Direct query to get employee details without relying on Session
                    string query = @"
                SELECT e.employee_id as id, e.employee_name as name, e.employee_position as position
                FROM operation_command_employees oce
                INNER JOIN employees e ON oce.employee_id = e.employee_id
                WHERE oce.operation_command_id = @opId
                ORDER BY e.employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@opId", operationCommandId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

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

                        System.Diagnostics.Debug.WriteLine($"Found employee: ID={reader["id"]}, Name={displayName}");
                    }
                    reader.Close();

                    System.Diagnostics.Debug.WriteLine($"Total employees found: {employeesWithNames.Count}");

                    if (employeesWithNames.Count > 0)
                    {
                        // Store the selected employee IDs in hidden field
                        string ids = string.Join(",", employeesWithNames.Select(e => e.id));
                        hdnSelectedEmployees.Value = ids;

                        // Clear and repopulate SelectedEmployeesList
                        SelectedEmployeesList.Clear();
                        foreach (var emp in employeesWithNames)
                        {
                            SelectedEmployeesList.Add(emp);
                        }

                        string json = new JavaScriptSerializer().Serialize(employeesWithNames);
                        System.Diagnostics.Debug.WriteLine($"JSON to send: {json}");

                        // Make sure the employees container is visible and populate
                        string script = $@"
                    setTimeout(function() {{
                        var employeesContainer = document.getElementById('employeesContainer');
                        if (employeesContainer) {{
                            employeesContainer.style.display = 'block';
                            console.log('Employees container made visible');
                        }}
                        
                        // Clear existing and set new employees
                        selectedEmployees = {json};
                        console.log('Loading employees:', selectedEmployees);
                        console.log('Number of employees:', selectedEmployees.length);
                        
                        if (typeof updateEmployeesList === 'function') {{
                            updateEmployeesList();
                            console.log('updateEmployeesList called');
                        }}
                        if (typeof updateEmployeesHiddenField === 'function') {{
                            updateEmployeesHiddenField();
                        }}
                    }}, 200);";

                        ScriptManager.RegisterStartupScript(this, GetType(), "loadSelectedEmployees_" + operationCommandId, script, true);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"No employees found for mission {operationCommandId}");
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
                        console.log('Cleared employees list');
                    }, 200);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "clearSelectedEmployees_" + operationCommandId, script, true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading operation employees: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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
                    SqlDataReader reader = cmd.ExecuteReader();
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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

                    // Debug - print loaded data
                    System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} cars");
                    foreach (DataRow row in dt.Rows)
                    {
                        System.Diagnostics.Debug.WriteLine($"Car ID: {row["car_number_id"]}, Number: {row["full_plate_number"]}");
                    }

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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                List<SubscriberItem> subscribers = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(connString))
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
                    SqlDataReader reader = cmd.ExecuteReader();

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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                List<SubscriberItem> employees = new List<SubscriberItem>();

                using (SqlConnection conn = new SqlConnection(connString))
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
                    SqlDataReader reader = cmd.ExecuteReader();

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

                Session["EmployeesList"] = employees;

                // Also populate the employee dropdown selector
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
            string script = $"selectedEmployees = {json}; updateEmployeesList(); updateEmployeesHiddenField();";
            ScriptManager.RegisterStartupScript(this, GetType(), "bindSelectedEmployees", script, true);
        }

        private void SaveSelectedSubscribers(int operationCommandId, string selectedIds)
        {
            if (string.IsNullOrEmpty(selectedIds))
                return;

            string[] ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length == 0)
                return;

            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string deleteQuery = "DELETE FROM operation_command_subscribers WHERE operation_command_id = @opId";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@opId", operationCommandId);
                deleteCmd.ExecuteNonQuery();

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

                SqlCommand checkCmd = new SqlCommand(checkTableQuery, conn);
                checkCmd.ExecuteNonQuery();

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

            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string deleteQuery = "DELETE FROM operation_command_employees WHERE operation_command_id = @opId";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@opId", operationCommandId);
                deleteCmd.ExecuteNonQuery();

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

                SqlCommand createCmd = new SqlCommand(createTableQuery, conn);
                createCmd.ExecuteNonQuery();

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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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

        private void LoadRoutes()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    route_id, 
                    route_name,
                    description,
                    route_name + ' - ' + ISNULL(description, '') as display_text
                FROM routes 
                WHERE is_active = 1
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
                if (ddlCarType != null)
                    ddlCarType.SelectedIndex = 0;

                if (txtCarBrand != null)
                    txtCarBrand.Text = "";

                if (txtModelYear != null)
                    txtModelYear.Text = "";

                if (txtFuelType != null)
                    txtFuelType.Text = "";

                if (ddlDriverName != null)
                    ddlDriverName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ClearCarDetails: {ex.Message}");
            }
        }

        private string GetCarBrand(string carNumberId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT cm.car_brand
                FROM main_vehicle_details mvd
                INNER JOIN car_model cm ON mvd.v_model = cm.m_id
                WHERE mvd.v_num = @carNumberId
                  AND mvd.deleted = 0";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carNumberId", carNumberId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting car brand: {ex.Message}");
                return "";
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

                DataTable dt = Session["CarDetailsData"] as DataTable;

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rows = dt.Select($"car_number_id = {selectedCarNumberId}");

                    if (rows.Length > 0)
                    {
                        DataRow row = rows[0];

                        // Debug output
                        System.Diagnostics.Debug.WriteLine($"=== SELECTED CAR: {selectedCarNumberId} ===");
                        System.Diagnostics.Debug.WriteLine($"Car Brand: {row["car_brand"]}");
                        System.Diagnostics.Debug.WriteLine($"Car Type: {row["car_type_name"]}");
                        System.Diagnostics.Debug.WriteLine($"Model Year (from vehicle): {row["model_year"]}");
                        System.Diagnostics.Debug.WriteLine($"Fuel Type: {row["fuel_type_name"]}");

                        // Set Car Type dropdown (this is the type name)
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
                                // Add the item if not found
                                string displayText = row["car_type_name"]?.ToString();
                                ddlCarType.Items.Add(new ListItem(displayText, carTypeId));
                                ddlCarType.SelectedValue = carTypeId;
                            }
                        }

                        // Set Car Brand - this comes from car_types table (car_brand)
                        txtCarBrand.Text = row["car_brand"]?.ToString() ?? "--";

                        // Set Model Year - this now comes from mvd.v_year (the actual vehicle year)
                        txtModelYear.Text = row["model_year"]?.ToString() ?? "--";

                        // Set Fuel Type - this comes from fuel_type table
                        txtFuelType.Text = row["fuel_type_name"]?.ToString() ?? "--";

                        // Reset driver dropdown
                        ddlDriverName.SelectedIndex = 0;

                        // Auto-populate start kilometer
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

                            ScriptManager.RegisterStartupScript(this, GetType(), "kmLoaded", $"alert('{message}');", true);
                        }
                        else
                        {
                            txtStartKM.Text = "";
                            if (hdnLastKilometer != null) hdnLastKilometer.Value = "0";
                            ScriptManager.RegisterStartupScript(this, GetType(), "kmWarning", "alert('تنبيه: لم يتم تسجيل عداد ابتدائي لهذه السيارة. يرجى إدخال قراءة العداد يدوياً.');", true);
                        }

                        if (Session["EmployeesList"] != null) ReloadEmployeesScript();
                        if (Session["SubscribersList"] != null) ReloadSubscribersScript();
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
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ClearCarDetails();
            }
        }

        protected void ddlRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedRouteId = ddlRoute.SelectedValue;

                if (string.IsNullOrEmpty(selectedRouteId) || selectedRouteId == "0")
                {
                    routeDescriptionContainer.Visible = false;
                    lblRouteDescription.Text = "";
                    // Clear subscribers when no route selected
                    ClearSubscribersList();
                    return;
                }

                // Get route details from Session
                DataTable dt = Session["RoutesData"] as DataTable;
                if (dt != null)
                {
                    DataRow[] rows = dt.Select($"route_id = {selectedRouteId}");
                    if (rows.Length > 0)
                    {
                        string description = rows[0]["description"]?.ToString() ?? "";
                        string routeName = rows[0]["route_name"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(description))
                        {
                            lblRouteDescription.Text = $"📌 {description}";
                            routeDescriptionContainer.Visible = true;
                        }
                        else
                        {
                            routeDescriptionContainer.Visible = false;
                        }
                    }
                }

                // Load subscribers for this route
                int routeId = Convert.ToInt32(selectedRouteId);
                LoadSubscribersByRoute(routeId);
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
                }
                else if (selectedValue == "mission")
                {
                    missionTextContainer.Visible = true;
                    if (employeesContainer != null)
                        employeesContainer.Visible = true;

                    if (ddlRoute != null)
                        ddlRoute.SelectedIndex = 0;

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

                // Time and distance fields are optional - they can be null/empty
                // The driver will fill them later through the DriverTripEntry page

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
                    txtStartTime.Text,      // Can be empty
                    txtReturnTime.Text,     // Can be empty
                    txtStartKM.Text,        // Can be empty
                    txtEndKM.Text,          // Can be empty
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
                    else if (missionType == "mission")
                    {
                        // Employees are already saved in the employees_text column
                        // No additional save needed
                    }
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

            // ===== 1. REQUIRED FIELDS VALIDATION =====
            if (ddlMissionType.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار نوع المأمورية<br/>";

            if (string.IsNullOrEmpty(ddlCarNumber.SelectedValue))
                errorMessage += "• الرجاء اختيار رقم العربية<br/>";

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

            // ===== 2. DATE/TIME VALIDATION - COMMENTED OUT (Now optional, driver will enter) =====
            /*
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;
            bool hasValidDates = true;

            if (string.IsNullOrEmpty(txtStartTime.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال بداية المأمورية<br/>";
                hasValidDates = false;
            }
            else if (!DateTime.TryParse(txtStartTime.Text, out startTime))
            {
                errorMessage += "• صيغة تاريخ البداية غير صحيحة<br/>";
                hasValidDates = false;
            }

            if (string.IsNullOrEmpty(txtReturnTime.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال وقت العودة<br/>";
                hasValidDates = false;
            }
            else if (!DateTime.TryParse(txtReturnTime.Text, out endTime))
            {
                errorMessage += "• صيغة تاريخ العودة غير صحيحة<br/>";
                hasValidDates = false;
            }

            if (hasValidDates && endTime <= startTime)
            {
                errorMessage += "• وقت العودة يجب أن يكون بعد وقت البداية<br/>";
            }
            */

            // ===== 3. KILOMETER VALIDATION - COMMENTED OUT (Now optional, driver will enter) =====
            /*
            float startKM = 0, endKM = 0;
            bool hasValidKM = true;

            if (string.IsNullOrEmpty(txtStartKM.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال عداد البداية<br/>";
                hasValidKM = false;
            }
            else if (!float.TryParse(txtStartKM.Text, out startKM))
            {
                errorMessage += "• قيمة عداد البداية غير صحيحة<br/>";
                hasValidKM = false;
            }
            else if (startKM < 0)
            {
                errorMessage += "• عداد البداية لا يمكن أن يكون أقل من صفر<br/>";
                hasValidKM = false;
            }

            if (string.IsNullOrEmpty(txtEndKM.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال عداد العودة<br/>";
                hasValidKM = false;
            }
            else if (!float.TryParse(txtEndKM.Text, out endKM))
            {
                errorMessage += "• قيمة عداد العودة غير صحيحة<br/>";
                hasValidKM = false;
            }
            else if (endKM < 0)
            {
                errorMessage += "• عداد العودة لا يمكن أن يكون أقل من صفر<br/>";
                hasValidKM = false;
            }

            if (hasValidKM && endKM <= startKM)
            {
                errorMessage += "• عداد العودة يجب أن يكون أكبر من عداد البداية<br/>";
            }

            if (hasValidKM && !string.IsNullOrEmpty(ddlCarNumber.SelectedValue))
            {
                int carId = Convert.ToInt32(ddlCarNumber.SelectedValue);
                float lastEndKM = GetLastCarKilometer(carId);

                if (startKM < lastEndKM)
                {
                    errorMessage += $"• عداد البداية لا يمكن أن يكون أقل من آخر عداد مسجل ({lastEndKM} كم) لهذه السيارة<br/>";
                }
            }
            */

            // ===== 4. TIME CONFLICT VALIDATION - COMMENTED OUT (Only if dates are provided) =====
            /*
            if (hasValidDates &&
                !string.IsNullOrEmpty(ddlCarNumber.SelectedValue) &&
                ddlDriverName.SelectedValue != "0")
            {
                int carId = Convert.ToInt32(ddlCarNumber.SelectedValue);
                int driverId = Convert.ToInt32(ddlDriverName.SelectedValue);

                if (HasTimeConflict(carId, driverId, startTime, endTime, currentMissionId))
                {
                    errorMessage += "• يوجد تعارض في الوقت مع مهمة أخرى لنفس السيارة أو السائق<br/>";
                }
            }
            */

            // ===== SHOW ERRORS =====
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
                System.Diagnostics.Debug.WriteLine($"missionType: {missionType}");
                System.Diagnostics.Debug.WriteLine($"startTime: '{startTime}'");
                System.Diagnostics.Debug.WriteLine($"returnTime: '{returnTime}'");

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Parse integer values
                    int vNumberId = int.Parse(carNumberId);
                    int driverIdInt = int.Parse(driverId);
                    int carTypeIdInt = int.Parse(carTypeId);
                    int garageWriterIdInt = int.Parse(garageWriterId);
                    int vehicleObserverIdInt = int.Parse(vehicleObserverId);
                    int operationManagerIdInt = int.Parse(operationManagerId);

                    // Parse float values - handle empty values
                    float kilometerStart = 0;
                    float kilometerEnd = 0;

                    if (!string.IsNullOrEmpty(startKm))
                    {
                        float.TryParse(startKm, out kilometerStart);
                    }

                    if (!string.IsNullOrEmpty(endKm))
                    {
                        float.TryParse(endKm, out kilometerEnd);
                    }

                    // Parse dates - handle empty values safely
                    object dbStartTime = DBNull.Value;
                    object dbEndTime = DBNull.Value;

                    // Handle start time
                    if (!string.IsNullOrEmpty(startTime))
                    {
                        try
                        {
                            DateTime parsedStartTime = DateTime.Parse(startTime);
                            dbStartTime = parsedStartTime;
                            System.Diagnostics.Debug.WriteLine($"Start time parsed: {parsedStartTime}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error parsing start time: {ex.Message}");
                            dbStartTime = DBNull.Value;
                        }
                    }

                    // Handle end time
                    if (!string.IsNullOrEmpty(returnTime))
                    {
                        try
                        {
                            DateTime parsedEndTime = DateTime.Parse(returnTime);
                            dbEndTime = parsedEndTime;
                            System.Diagnostics.Debug.WriteLine($"End time parsed: {parsedEndTime}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error parsing end time: {ex.Message}");
                            dbEndTime = DBNull.Value;
                        }
                    }

                    DateTime currentDate = DateTime.Today;

                    // Initialize variables for the insert
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
                    GETDATE(),
                    0,
                    0
                );
                SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);

                    // Add parameters
                    cmd.Parameters.Add("@vNumber", SqlDbType.Int).Value = vNumberId;
                    cmd.Parameters.Add("@vType", SqlDbType.Int).Value = carTypeIdInt;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = currentDate;
                    cmd.Parameters.Add("@driverId", SqlDbType.Int).Value = driverIdInt;
                    cmd.Parameters.Add("@missionTypeId", SqlDbType.Int).Value = finalMissionTypeId.Value;

                    // Handle route ID (location_point) - only for خط
                    if (finalRouteId.HasValue)
                        cmd.Parameters.Add("@routeId", SqlDbType.Int).Value = finalRouteId.Value;
                    else
                        cmd.Parameters.Add("@routeId", SqlDbType.Int).Value = DBNull.Value;

                    // Handle mission description - only for مأمورية
                    if (!string.IsNullOrEmpty(finalMissionDescription))
                        cmd.Parameters.Add("@missionDescription", SqlDbType.NVarChar).Value = finalMissionDescription;
                    else
                        cmd.Parameters.Add("@missionDescription", SqlDbType.NVarChar).Value = DBNull.Value;

                    // Handle employees text
                    cmd.Parameters.Add("@employeesText", SqlDbType.NVarChar).Value =
                        missionType == "mission" ? (object)selectedItems : DBNull.Value;

                    // Handle start time - using object to handle DBNull
                    cmd.Parameters.Add("@startTime", SqlDbType.DateTime).Value = dbStartTime;

                    // Handle end time - using object to handle DBNull
                    cmd.Parameters.Add("@endTime", SqlDbType.DateTime).Value = dbEndTime;

                    cmd.Parameters.Add("@kmStart", SqlDbType.Float).Value = kilometerStart;
                    cmd.Parameters.Add("@kmEnd", SqlDbType.Float).Value = kilometerEnd;
                    cmd.Parameters.Add("@garageWriterId", SqlDbType.Int).Value = garageWriterIdInt;
                    cmd.Parameters.Add("@observerId", SqlDbType.Int).Value = vehicleObserverIdInt;
                    cmd.Parameters.Add("@managerId", SqlDbType.Int).Value = operationManagerIdInt;

                    // Execute the insert
                    System.Diagnostics.Debug.WriteLine("Executing main insert...");
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

                    ClearForm();
                    System.Diagnostics.Debug.WriteLine("=== SAVE OPERATION COMMAND COMPLETED ===");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in SaveOperationCommand: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
            ClearForm();
        }

        private void ClearForm()
        {
            try
            {
                ddlMissionType.SelectedIndex = 0;
                ddlCarType.SelectedIndex = 0;
                ddlCarNumber.SelectedIndex = 0;
                ddlDriverName.SelectedIndex = 0;
                ddlGarageWriter.SelectedIndex = 0;
                ddlVehicleObserver.SelectedIndex = 0;
                ddlOperationManager.SelectedIndex = 0;
                ddlRoute.SelectedIndex = 0;

                routeDropdownContainer.Visible = false;
                missionTextContainer.Visible = false;
                routeDescriptionContainer.Visible = false;

                if (subscribersContainer != null)
                    subscribersContainer.Visible = false;
                if (employeesContainer != null)
                    employeesContainer.Visible = false;

                txtMissionText.Text = "";
                lblRouteDescription.Text = "";

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
                    updateSubscribersList(); 
                    updateEmployeesList();
                    updateHiddenField();
                    updateEmployeesHiddenField();
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        suggestions.Add(new CarNumberSuggestion
                        {
                            plate_number = reader["plate_number"].ToString(),
                            description = reader["description"].ToString()
                        });
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

        private float GetLastCarKilometer(int carId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT TOP 1 kilometer_end
                FROM operation_command
                WHERE v_number = @CarId
                  AND kilometer_end IS NOT NULL
                ORDER BY mission_ending_time DESC, mission_id DESC";

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
        // Helper method to check if car is active
        private bool IsCarActive(int carId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
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
                return true; // Default to true if error
            }
        }

        // Helper method to check if driver is active
        private bool IsDriverActive(int driverId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
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
                return true; // Default to true if error
            }
        }
        private decimal GetVehicleInitialKilometer(int carNumberId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
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
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
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