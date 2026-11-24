using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class SubscribersManagement : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

        // The employee position that counts as a driver (employees.employee_position)
        private const string DriverPosition = "سائق";

        // SQL fragment: every routes.route_id that belongs to the SAME route name as the selected route.
        // The route dropdown only holds MIN(route_id) per route_name, so every filter
        // (cars, drivers, subscribers, search) must resolve by route_name, not by that single id.
        private const string SameRouteIdsSql =
            "(SELECT rr.route_id FROM routes rr WHERE rr.route_name = (SELECT route_name FROM routes WHERE route_id = @RouteId))";

        // SQL fragment: the route_master ids of the line that was selected.
        // subscription_details.route_id holds route_master.route_id (the master list of lines),
        // NOT routes.route_id - the two tables use different ids for the same line, which is why
        // subscribers appeared under another line. The link between them is the line name.
        private const string SameMasterRouteIdsSql =
            "(SELECT rm.route_id FROM route_master rm WHERE rm.route_name = (SELECT route_name FROM routes WHERE route_id = @RouteId))";

        private List<SelectedSubscriber> SelectedSubscribers
        {
            get
            {
                if (ViewState["SelectedSubscribers"] == null)
                    ViewState["SelectedSubscribers"] = new List<SelectedSubscriber>();
                return (List<SelectedSubscriber>)ViewState["SelectedSubscribers"];
            }
            set { ViewState["SelectedSubscribers"] = value; }
        }

        [Serializable]
        public class SelectedSubscriber
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaultDates();
                LoadRoutes();
                LoadSearchRoutes();
                LoadSubscriberHistory();
                LoadSearchResults();
                UpdateFilterInfo("اختر الخط أولاً");

                SelectedSubscribers = new List<SelectedSubscriber>();
                BindSelectedSubscribers();

                LoadAllSubscribers();
                LoadAllCars();
                LoadAllDrivers();
            }
        }

        private void SetDefaultDates()
        {
            txtSubStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtSubEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
        }

        private void UpdateStepIndicator(int step)
        {
            string script = $"updateStepIndicator({step});";
            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateStep", script, true);
        }

        private void UpdateFilterInfo(string message)
        {
            string script = $"updateFilterInfo('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateFilterInfo", script, true);
        }

        private void UpdateSelectedCount()
        {
            string script = $"if(document.getElementById('selectedCount')) document.getElementById('selectedCount').textContent = '{SelectedSubscribers.Count}';";
            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateSelectedCount", script, true);
        }

        // ===== 1) LOAD ROUTES (only from routes table) =====
        private void LoadRoutes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        MIN(route_id) AS route_id, 
                        route_name, 
                        MAX(start_loc) AS start_loc, 
                        MAX(end_loc) AS end_loc
                    FROM routes 
                    WHERE is_active = 1 
                    GROUP BY route_name
                    ORDER BY route_name";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlRoute.Items.Clear();
                ddlRoute.Items.Add(new ListItem("-- اختر الخط --", ""));
                foreach (DataRow row in dt.Rows)
                {
                    string display = row["route_name"].ToString();
                    if (row["start_loc"] != DBNull.Value && row["end_loc"] != DBNull.Value)
                    {
                        display += $" ({row["start_loc"]} - {row["end_loc"]})";
                    }
                    ddlRoute.Items.Add(new ListItem(display, row["route_id"].ToString()));
                }
            }
            UpdateStepIndicator(1);
        }

        private void LoadSearchRoutes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT MIN(route_id) AS route_id, route_name 
                    FROM routes 
                    WHERE is_active = 1 
                    GROUP BY route_name
                    ORDER BY route_name";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlSearchRoute.Items.Clear();
                ddlSearchRoute.Items.Add(new ListItem("-- الكل --", ""));
                foreach (DataRow row in dt.Rows)
                {
                    ddlSearchRoute.Items.Add(new ListItem(row["route_name"].ToString(), row["route_id"].ToString()));
                }
            }
        }

        // ===== 2) LOAD ALL SUBSCRIBERS (only from subscription_details, no duplicates) =====
        // - hidden rows (hide = 1) are skipped
        // - one entry per name: the same person saved more than once shows only once
        private void LoadAllSubscribers()
        {
            ddlSubscriber.Items.Clear();
            ddlSubscriber.Items.Add(new ListItem("-- اختر المشترك --", ""));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT MIN(n_id) AS n_id, LTRIM(RTRIM(name)) AS name
                    FROM subscription_details
                    WHERE hide = 0
                      AND name IS NOT NULL AND LTRIM(RTRIM(name)) <> ''
                    GROUP BY LTRIM(RTRIM(name))
                    ORDER BY LTRIM(RTRIM(name))";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    ddlSubscriber.Items.Add(new ListItem(row["name"].ToString(), row["n_id"].ToString()));
                }
            }
        }

        // ===== LOAD SUBSCRIBERS BY ROUTE (only from subscription_details, no duplicates) =====
        // subscription_details.route_id is a route_master.route_id. The selected line (a routes row)
        // is translated to route_master through its route_name, so each subscriber appears only
        // under his own line.
        private void LoadSubscribersByRoute(string routeId)
        {
            ddlSubscriber.Items.Clear();
            ddlSubscriber.Items.Add(new ListItem("-- اختر المشترك --", ""));

            if (string.IsNullOrEmpty(routeId))
            {
                LoadAllSubscribers();
                UpdateFilterInfo("اختر الخط أولاً");
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT MIN(sd.n_id) AS n_id, LTRIM(RTRIM(sd.name)) AS name
                    FROM subscription_details sd
                    WHERE sd.hide = 0
                      AND sd.name IS NOT NULL AND LTRIM(RTRIM(sd.name)) <> ''
                      AND sd.route_id IN " + SameMasterRouteIdsSql + @"
                    GROUP BY LTRIM(RTRIM(sd.name))
                    ORDER BY LTRIM(RTRIM(sd.name))";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@RouteId", routeId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    UpdateFilterInfo("لا يوجد مشتركين مسجلين على هذا الخط");
                    UpdateStepIndicator(2);
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ddlSubscriber.Items.Add(new ListItem(row["name"].ToString(), row["n_id"].ToString()));
                    }
                    UpdateFilterInfo($"تم العثور على {ddlSubscriber.Items.Count - 1} مشترك على هذا الخط");
                    UpdateStepIndicator(2);
                }
            }
        }

        // ===== 3) LOAD ALL CARS =====
        private void LoadAllCars()
        {
            ddlCarNumber.Items.Clear();
            ddlCarNumber.Items.Add(new ListItem("-- اختر السيارة --", "0"));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT DISTINCT 
                        cn.car_number_id,
                        RTRIM(cn.letter1) + ' ' + RTRIM(cn.letter2) +
                        CASE
                            WHEN cn.letter3 IS NOT NULL AND RTRIM(cn.letter3) != ''
                            THEN ' ' + RTRIM(cn.letter3)
                            ELSE ''
                        END + ' ' +
                        RTRIM(cn.plate_numbers) as car_number_display
                    FROM car_numbers cn
                    WHERE cn.is_active = 1
                    ORDER BY car_number_display";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    ddlCarNumber.Items.Add(new ListItem(row["car_number_display"].ToString(), row["car_number_id"].ToString()));
                }
            }
        }

        // ===== LOAD CARS BY ROUTE (unchanged - already correct) =====
        private void LoadCarsByRoute(string routeId)
        {
            ddlCarNumber.Items.Clear();
            ddlCarNumber.Items.Add(new ListItem("-- اختر السيارة --", "0"));

            if (string.IsNullOrEmpty(routeId))
            {
                LoadAllCars();
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT DISTINCT 
                        cn.car_number_id,
                        RTRIM(cn.letter1) + ' ' + RTRIM(cn.letter2) +
                        CASE
                            WHEN cn.letter3 IS NOT NULL AND RTRIM(cn.letter3) != ''
                            THEN ' ' + RTRIM(cn.letter3)
                            ELSE ''
                        END + ' ' +
                        RTRIM(cn.plate_numbers) as car_number_display
                    FROM routes r
                    INNER JOIN main_vehicle_details mvd ON r.car_id = mvd.v_id
                    INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                    WHERE r.route_name = (SELECT route_name FROM routes WHERE route_id = @RouteId)
                    AND r.car_id IS NOT NULL
                    AND cn.is_active = 1
                    ORDER BY car_number_display";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@RouteId", routeId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    UpdateFilterInfo("لا توجد سيارات مسجلة على هذا الخط");
                    UpdateStepIndicator(3);
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ddlCarNumber.Items.Add(new ListItem(row["car_number_display"].ToString(), row["car_number_id"].ToString()));
                    }
                    UpdateFilterInfo($"تم العثور على {ddlCarNumber.Items.Count - 1} سيارة على هذا الخط");
                    UpdateStepIndicator(3);
                }
            }
        }

        // ===== 4) LOAD ALL DRIVERS =====
        // FIX: drivers now come from the employees table (position = سائق only),
        // the same source routes_management uses for routes.driver_id.
        private void LoadAllDrivers()
        {
            ddlDriver.Items.Clear();
            ddlDriver.Items.Add(new ListItem("-- اختر السائق --", "0"));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        employee_id AS id,
                        employee_name AS name,
                        phone_number AS phone
                    FROM employees
                    WHERE is_active = 1
                      AND LTRIM(RTRIM(employee_position)) = @Position
                    ORDER BY employee_name";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.Add("@Position", SqlDbType.NVarChar, 150).Value = DriverPosition;
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string display = row["name"].ToString();
                    if (row["phone"] != DBNull.Value && !string.IsNullOrEmpty(row["phone"].ToString()))
                        display += $" ({row["phone"]})";
                    ddlDriver.Items.Add(new ListItem(display, row["id"].ToString()));
                }
            }
        }

        // ===== LOAD DRIVERS BY ROUTE =====
        // FIX: routes.driver_id holds employees.employee_id, so join employees (not drivers).
        private void LoadDriversByRoute(string routeId)
        {
            ddlDriver.Items.Clear();
            ddlDriver.Items.Add(new ListItem("-- اختر السائق --", "0"));

            if (string.IsNullOrEmpty(routeId))
            {
                LoadAllDrivers();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT DISTINCT 
                            e.employee_id AS id,
                            e.employee_name AS name,
                            e.phone_number AS phone
                        FROM routes r
                        INNER JOIN employees e ON r.driver_id = e.employee_id
                        WHERE r.route_name = (SELECT route_name FROM routes WHERE route_id = @RouteId)
                        AND r.driver_id IS NOT NULL
                        AND e.is_active = 1
                        ORDER BY e.employee_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@RouteId", routeId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        UpdateFilterInfo("لا يوجد سائقين مسجلين على هذا الخط");
                        UpdateStepIndicator(4);
                    }
                    else
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string display = row["name"].ToString();
                            if (row["phone"] != DBNull.Value && !string.IsNullOrEmpty(row["phone"].ToString()))
                                display += $" ({row["phone"]})";
                            ddlDriver.Items.Add(new ListItem(display, row["id"].ToString()));
                        }
                        UpdateFilterInfo($"تم العثور على {ddlDriver.Items.Count - 1} سائق على هذا الخط");
                        UpdateStepIndicator(4);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading drivers: {ex.Message}");
                LoadAllDrivers();
            }
        }

        // ===== EVENT HANDLERS =====
        protected void ddlRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            string routeId = ddlRoute.SelectedValue;

            if (!string.IsNullOrEmpty(routeId))
            {
                ShowRouteDetails(routeId);
                LoadSubscribersByRoute(routeId);
                LoadCarsByRoute(routeId);
                LoadDriversByRoute(routeId);
                UpdateFilterInfo($"الخط المختار: {ddlRoute.SelectedItem.Text}");
            }
            else
            {
                pnlRouteDetails.Visible = false;
                LoadAllSubscribers();
                LoadAllCars();
                LoadAllDrivers();
                UpdateFilterInfo("اختر الخط أولاً");
                UpdateStepIndicator(1);
            }
        }

        // ===== ADD SUBSCRIBER TO LIST =====
        protected void BtnAddSubscriber_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlSubscriber.SelectedValue))
            {
                ShowMessage("الرجاء اختيار مشترك أولاً");
                return;
            }

            int subscriberId = Convert.ToInt32(ddlSubscriber.SelectedValue);
            string subscriberName = ddlSubscriber.SelectedItem.Text;

            var list = SelectedSubscribers;
            bool exists = false;
            foreach (var sub in list)
            {
                if (sub.Id == subscriberId)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                ShowMessage($"المشترك '{subscriberName}' مضاف بالفعل");
                return;
            }

            list.Add(new SelectedSubscriber { Id = subscriberId, Name = subscriberName });
            SelectedSubscribers = list;

            BindSelectedSubscribers();
            UpdateSelectedCount();

            ddlSubscriber.SelectedIndex = 0;

            ShowSuccess($"تم إضافة '{subscriberName}'");
        }

        protected void RptSelectedSubscribers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveSubscriber")
            {
                int subscriberId = Convert.ToInt32(e.CommandArgument);
                var list = SelectedSubscribers;
                list.RemoveAll(s => s.Id == subscriberId);
                SelectedSubscribers = list;

                BindSelectedSubscribers();
                UpdateSelectedCount();
            }
        }

        private void BindSelectedSubscribers()
        {
            var list = SelectedSubscribers;

            if (list.Count > 0)
            {
                rptSelectedSubscribers.DataSource = list;
                rptSelectedSubscribers.DataBind();
                pnlNoSelected.Visible = false;
            }
            else
            {
                rptSelectedSubscribers.DataSource = null;
                rptSelectedSubscribers.DataBind();
                pnlNoSelected.Visible = true;
            }

            UpdateSelectedCount();
        }

        // ===== ROUTE DETAILS =====
        // FIX: drivers read from employees, and ALL drivers / ALL cars of the route name are listed
        // (routes_management can save several of each).
        private void ShowRouteDetails(string routeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT 
                            r.route_name,
                            MAX(r.description) AS description,
                            MAX(r.start_loc) AS start_loc,
                            MAX(r.end_loc) AS end_loc,
                            MAX(r.KM) AS KM,
                            MAX(r.direction) AS direction,
                            ISNULL(STUFF((
                                SELECT N'، ' + e.employee_name
                                FROM employees e
                                WHERE e.employee_id IN (
                                    SELECT r2.driver_id FROM routes r2
                                    WHERE r2.route_name = r.route_name AND r2.driver_id IS NOT NULL)
                                ORDER BY e.employee_name
                                FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, ''), '') AS driver_name,
                            ISNULL(STUFF((
                                SELECT N'، ' + (ISNULL(cn.letter1 + ' ', '') + ISNULL(cn.letter2 + ' ', '') + ISNULL(cn.letter3 + ' ', '') + ISNULL(CAST(cn.plate_numbers AS NVARCHAR(50)), ''))
                                FROM main_vehicle_details mvd
                                INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                                WHERE mvd.v_id IN (
                                    SELECT r3.car_id FROM routes r3
                                    WHERE r3.route_name = r.route_name AND r3.car_id IS NOT NULL)
                                ORDER BY cn.plate_numbers
                                FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, ''), '') AS car_display
                        FROM routes r
                        WHERE r.route_name = (SELECT route_name FROM routes WHERE route_id = @RouteId)
                        GROUP BY r.route_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RouteId", routeId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string routeDetails = $"<strong>اسم الخط:</strong> {reader["route_name"]}<br />";
                        if (reader["start_loc"] != DBNull.Value && reader["end_loc"] != DBNull.Value)
                            routeDetails += $"<strong>المسار:</strong> {reader["start_loc"]} → {reader["end_loc"]}<br />";
                        if (reader["KM"] != DBNull.Value)
                            routeDetails += $"<strong>المسافة:</strong> {Convert.ToDecimal(reader["KM"]):N0} كم<br />";
                        if (reader["direction"] != DBNull.Value)
                            routeDetails += $"<strong>الاتجاه:</strong> {reader["direction"]}<br />";
                        if (reader["description"] != DBNull.Value)
                            routeDetails += $"<strong>الوصف:</strong> {reader["description"]}<br />";
                        if (!string.IsNullOrEmpty(reader["driver_name"].ToString()))
                            routeDetails += $"<strong>السائقون المسؤولون:</strong> {reader["driver_name"]}<br />";
                        if (!string.IsNullOrEmpty(reader["car_display"].ToString()))
                            routeDetails += $"<strong>السيارات المسؤولة:</strong> {reader["car_display"]}";

                        lblRouteDetails.Text = routeDetails;
                        pnlRouteDetails.Visible = true;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading route details: {ex.Message}");
            }
        }

        // ===== SAVE - source = 'subscribers_management' =====
        // One row holding every selected subscriber (subscriber_ids / subscriber_names).
        // vehicle_subscriptions.driver_id now stores employees.employee_id.
        protected void BtnSaveSubscriber_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlRoute.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار الخط");
                    return;
                }

                var selectedList = SelectedSubscribers;
                if (selectedList.Count == 0)
                {
                    ShowMessage("الرجاء إضافة مشترك واحد على الأقل");
                    return;
                }

                if (ddlCarNumber.SelectedValue == "0" || string.IsNullOrEmpty(ddlCarNumber.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار السيارة");
                    return;
                }
                if (ddlDriver.SelectedValue == "0" || string.IsNullOrEmpty(ddlDriver.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار السائق");
                    return;
                }

                DateTime startDate, endDate;
                if (!DateTime.TryParse(txtSubStartDate.Text, out startDate)) startDate = DateTime.Now;
                if (!DateTime.TryParse(txtSubEndDate.Text, out endDate)) endDate = DateTime.Now.AddYears(1);

                if (endDate < startDate)
                {
                    ShowMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
                    return;
                }

                string notes = txtSubNotes.Text.Trim();

                string subscriberIdsCsv = string.Join(",", selectedList.Select(s => s.Id));
                string subscriberNamesCsv = string.Join(",", selectedList.Select(s => s.Name));
                int firstSubscriberId = selectedList[0].Id; // kept in subscriber_id for backward compatibility

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    int carNumberId = Convert.ToInt32(ddlCarNumber.SelectedValue);
                    int vehicleId = GetVehicleId(conn, carNumberId);

                    if (vehicleId == 0)
                    {
                        ShowMessage("لا توجد بيانات مركبة مسجلة لهذه السيارة. الرجاء تسجيل بيانات السيارة أولاً.");
                        return;
                    }

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int routeIdInt = Convert.ToInt32(ddlRoute.SelectedValue);
                            int driverId = Convert.ToInt32(ddlDriver.SelectedValue); // employees.employee_id

                            string insertSubscription = @"
                                INSERT INTO vehicle_subscriptions
                                (vehicle_id, subscriber_id, subscriber_ids, subscriber_names, route_id, driver_id, start_date, end_date, notes, created_date, is_active, source)
                                VALUES
                                (@VehicleId, @SubscriberId, @SubscriberIds, @SubscriberNames, @RouteId, @DriverId, @StartDate, @EndDate, @Notes, GETDATE(), 1, @Source)";

                            using (SqlCommand cmd = new SqlCommand(insertSubscription, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                                cmd.Parameters.AddWithValue("@SubscriberId", firstSubscriberId);
                                cmd.Parameters.AddWithValue("@SubscriberIds", subscriberIdsCsv);
                                cmd.Parameters.AddWithValue("@SubscriberNames", subscriberNamesCsv);
                                cmd.Parameters.AddWithValue("@RouteId", routeIdInt);
                                cmd.Parameters.AddWithValue("@DriverId", driverId);
                                cmd.Parameters.AddWithValue("@StartDate", startDate);
                                cmd.Parameters.AddWithValue("@EndDate", endDate);
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes);
                                cmd.Parameters.AddWithValue("@Source", "subscribers_management");
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            ShowSuccess($"تم تسجيل اشتراك واحد يضم {selectedList.Count} مشترك بنجاح");

                            SelectedSubscribers = new List<SelectedSubscriber>();
                            BindSelectedSubscribers();

                            string currentRouteId = ddlRoute.SelectedValue;
                            LoadSubscribersByRoute(currentRouteId);
                            LoadCarsByRoute(currentRouteId);
                            LoadDriversByRoute(currentRouteId);
                            LoadSubscriberHistory();
                            LoadSearchResults();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء حفظ البيانات: " + ex.Message);
            }
        }

        private int GetVehicleId(SqlConnection conn, int carNumberId)
        {
            string query = "SELECT TOP 1 v_id FROM main_vehicle_details WHERE v_num = @CarNumberId ORDER BY v_id DESC";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        protected void BtnClearSubscriber_Click(object sender, EventArgs e)
        {
            ClearSubscriberForm();
        }

        private void ClearSubscriberForm()
        {
            ddlRoute.SelectedIndex = 0;
            ddlSubscriber.Items.Clear();
            ddlCarNumber.Items.Clear();
            ddlCarNumber.Items.Add(new ListItem("-- اختر السيارة --", "0"));
            ddlDriver.Items.Clear();
            ddlDriver.Items.Add(new ListItem("-- اختر السائق --", "0"));
            pnlRouteDetails.Visible = false;
            txtSubNotes.Text = "";
            SetDefaultDates();

            SelectedSubscribers = new List<SelectedSubscriber>();
            BindSelectedSubscribers();

            UpdateFilterInfo("اختر الخط أولاً");
            UpdateStepIndicator(1);
            LoadAllSubscribers();
            LoadAllCars();
            LoadAllDrivers();
        }

        // ===== HISTORY - source = 'subscribers_management' only =====
        // FIX: driver name comes from employees (vs.driver_id = employees.employee_id).
        private void LoadSubscriberHistory()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT
                            vs.subscription_id,
                            ISNULL(vs.subscriber_names, sd.name) AS subscriber_name,
                            r.route_name,
                            cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers AS car_number,
                            d.employee_name AS driver_name,
                            vs.start_date,
                            vs.end_date,
                            vs.cost
                        FROM vehicle_subscriptions vs
                        LEFT JOIN subscription_details sd ON vs.subscriber_id = sd.n_id
                        INNER JOIN routes r ON vs.route_id = r.route_id
                        LEFT JOIN main_vehicle_details mvd ON vs.vehicle_id = mvd.v_id
                        LEFT JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                        LEFT JOIN employees d ON vs.driver_id = d.employee_id
                        WHERE vs.source = 'subscribers_management'
                        ORDER BY vs.created_date DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvSubscriberHistory.DataSource = dt;
                    gvSubscriberHistory.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateHistoryCount",
                        $"if(document.getElementById('historyCount')) document.getElementById('historyCount').textContent = '{count}';", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscriber history: {ex.Message}");
                gvSubscriberHistory.EmptyDataText = "حدث خطأ في تحميل السجل";
                gvSubscriberHistory.DataSource = null;
                gvSubscriberHistory.DataBind();
            }
        }

        protected void GvSubscriberHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscriberHistory.PageIndex = e.NewPageIndex;
            LoadSubscriberHistory();
        }

        // ===== SEARCH =====
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadSearchResults();
        }

        protected void BtnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearchSubscriber.Text = "";
            ddlSearchRoute.SelectedValue = "";
            txtSearchCar.Text = "";
            txtSearchDriver.Text = "";
            txtSearchStartDateFrom.Text = "";
            txtSearchStartDateTo.Text = "";
            ddlSearchStatus.SelectedValue = "";
            LoadSearchResults();
        }

        // FIX: driver from employees; route filter matches every route row with the same name.
        private void LoadSearchResults()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    StringBuilder query = new StringBuilder(@"
                        SELECT
                            vs.subscription_id,
                            ISNULL(vs.subscriber_names, sd.name) AS subscriber_name,
                            r.route_name,
                            cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers AS car_number,
                            d.employee_name AS driver_name,
                            vs.start_date,
                            vs.end_date,
                            vs.cost
                        FROM vehicle_subscriptions vs
                        LEFT JOIN subscription_details sd ON vs.subscriber_id = sd.n_id
                        INNER JOIN routes r ON vs.route_id = r.route_id
                        LEFT JOIN main_vehicle_details mvd ON vs.vehicle_id = mvd.v_id
                        LEFT JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                        LEFT JOIN employees d ON vs.driver_id = d.employee_id
                        WHERE vs.source = 'subscribers_management'");

                    if (!string.IsNullOrEmpty(txtSearchSubscriber.Text))
                        query.Append(" AND ISNULL(vs.subscriber_names, sd.name) LIKE @SubscriberName");

                    if (!string.IsNullOrEmpty(ddlSearchRoute.SelectedValue))
                        query.Append(" AND vs.route_id IN " + SameRouteIdsSql);

                    if (!string.IsNullOrEmpty(txtSearchCar.Text))
                        query.Append(@" AND (cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers LIKE @CarNumber)");

                    if (!string.IsNullOrEmpty(txtSearchDriver.Text))
                        query.Append(" AND d.employee_name LIKE @DriverName");

                    if (!string.IsNullOrEmpty(txtSearchStartDateFrom.Text))
                        query.Append(" AND vs.start_date >= @StartDateFrom");

                    if (!string.IsNullOrEmpty(txtSearchStartDateTo.Text))
                        query.Append(" AND vs.start_date <= @StartDateTo");

                    if (!string.IsNullOrEmpty(ddlSearchStatus.SelectedValue))
                    {
                        if (ddlSearchStatus.SelectedValue == "active")
                            query.Append(" AND vs.start_date <= GETDATE() AND vs.end_date >= GETDATE()");
                        else if (ddlSearchStatus.SelectedValue == "expired")
                            query.Append(" AND vs.end_date < GETDATE()");
                        else if (ddlSearchStatus.SelectedValue == "upcoming")
                            query.Append(" AND vs.start_date > GETDATE()");
                    }

                    query.Append(" ORDER BY vs.created_date DESC");

                    SqlDataAdapter da = new SqlDataAdapter(query.ToString(), conn);

                    if (!string.IsNullOrEmpty(txtSearchSubscriber.Text))
                        da.SelectCommand.Parameters.AddWithValue("@SubscriberName", "%" + txtSearchSubscriber.Text.Trim() + "%");

                    if (!string.IsNullOrEmpty(ddlSearchRoute.SelectedValue))
                        da.SelectCommand.Parameters.AddWithValue("@RouteId", ddlSearchRoute.SelectedValue);

                    if (!string.IsNullOrEmpty(txtSearchCar.Text))
                        da.SelectCommand.Parameters.AddWithValue("@CarNumber", "%" + txtSearchCar.Text.Trim() + "%");

                    if (!string.IsNullOrEmpty(txtSearchDriver.Text))
                        da.SelectCommand.Parameters.AddWithValue("@DriverName", "%" + txtSearchDriver.Text.Trim() + "%");

                    if (!string.IsNullOrEmpty(txtSearchStartDateFrom.Text))
                        da.SelectCommand.Parameters.AddWithValue("@StartDateFrom", txtSearchStartDateFrom.Text);

                    if (!string.IsNullOrEmpty(txtSearchStartDateTo.Text))
                        da.SelectCommand.Parameters.AddWithValue("@StartDateTo", txtSearchStartDateTo.Text);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSearchResults.DataSource = dt;
                    gvSearchResults.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateSearchCount",
                        $"if(document.getElementById('searchResultCount')) document.getElementById('searchResultCount').textContent = '{count}';", true);

                    if (count == 0)
                        gvSearchResults.EmptyDataText = "لا توجد نتائج مطابقة للبحث";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading search results: {ex.Message}");
                gvSearchResults.EmptyDataText = "حدث خطأ في تحميل النتائج";
                gvSearchResults.DataSource = null;
                gvSearchResults.DataBind();
            }
        }

        protected void GvSearchResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearchResults.PageIndex = e.NewPageIndex;
            LoadSearchResults();
        }

        protected string GetStatusCss(DateTime startDate, DateTime endDate)
        {
            DateTime now = DateTime.Now;
            if (startDate <= now && endDate >= now)
                return "status-badge-active";
            else if (startDate > now)
                return "status-badge-upcoming";
            else
                return "status-badge-expired";
        }

        protected string GetStatusText(DateTime startDate, DateTime endDate)
        {
            DateTime now = DateTime.Now;
            if (startDate <= now && endDate >= now)
                return "نشط";
            else if (startDate > now)
                return "لم يبدأ";
            else
                return "منتهي";
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }

        protected void BtnTabEntry_Click(object sender, EventArgs e)
        {
            btnTabEntry.CssClass = "tab-btn active";
            btnTabSearch.CssClass = "tab-btn";
            string script = "showTab('Entry');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowEntryTab", script, true);
        }

        protected void BtnTabSearch_Click(object sender, EventArgs e)
        {
            btnTabSearch.CssClass = "tab-btn active";
            btnTabEntry.CssClass = "tab-btn";
            string script = "showTab('Search');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSearchTab", script, true);
        }
    }
}
