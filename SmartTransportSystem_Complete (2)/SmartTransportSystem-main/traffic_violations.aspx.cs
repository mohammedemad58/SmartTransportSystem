using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class traffic_violations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set default date to today
                txtViolationDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                // Load dropdowns (driver loading removed)
                LoadCarNumbers();
                LoadViolationTypes();

                // Load initial statistics
                LoadStatistics();
            }
        }

        #region Load Dropdown Methods

        private void LoadCarNumbers()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    car_number_id,
                    RTRIM(letter1) + ' ' + RTRIM(letter2) + 
                    CASE 
                        WHEN letter3 IS NOT NULL AND RTRIM(letter3) != '' 
                        THEN ' ' + RTRIM(letter3) 
                        ELSE '' 
                    END + ' ' + 
                    RTRIM(plate_numbers) as car_number_display
                FROM car_numbers
                WHERE is_active = 1
                ORDER BY car_number_display";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} cars");

                    ddlCarNumber.Items.Clear();
                    ddlCarNumber.Items.Add(new ListItem("-- اختر رقم العربية --", "0"));

                    foreach (DataRow row in dt.Rows)
                    {
                        string carId = row["car_number_id"].ToString();
                        string carDisplay = row["car_number_display"].ToString();
                        ddlCarNumber.Items.Add(new ListItem(carDisplay, carId));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading car numbers: {ex.Message}");
            }
        }

        private void LoadViolationTypes()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT violation_type_id, violation_type_name
                        FROM violation_types
                        WHERE is_active = 1
                        ORDER BY violation_type_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlViolationType.Items.Clear();
                    ddlViolationType.Items.Add(new ListItem("-- اختر نوع المخالفة --", "0"));

                    foreach (DataRow row in dt.Rows)
                    {
                        string id = row["violation_type_id"].ToString();
                        string name = row["violation_type_name"].ToString();
                        ddlViolationType.Items.Add(new ListItem(name, id));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading violation types: {ex.Message}");
                // Fallback to hardcoded values if database fails
                ddlViolationType.Items.Clear();
                ddlViolationType.Items.Add(new ListItem("-- اختر نوع المخالفة --", "0"));
                ddlViolationType.Items.Add(new ListItem("تجاوز السرعة", "1"));
                ddlViolationType.Items.Add(new ListItem("وقوف خاطئ", "2"));
                ddlViolationType.Items.Add(new ListItem("تجاوز إشارة مرور", "3"));
                ddlViolationType.Items.Add(new ListItem("قيادة بدون رخصة", "4"));
                ddlViolationType.Items.Add(new ListItem("انتهاء رخصة", "5"));
                ddlViolationType.Items.Add(new ListItem("عدم ارتداء حزام الأمان", "6"));
                ddlViolationType.Items.Add(new ListItem("استخدام الهاتف أثناء القيادة", "7"));
            }
        }

        // Load missions based on selected car only (driver filter removed)
        private void LoadMissions(int carId, int? selectedMissionId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadMissions - CarId: {carId}");

                // If no car selected, clear dropdown
                if (carId == 0)
                {
                    ddlMission.Items.Clear();
                    ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
                    return;
                }

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Driver filter removed — missions are shown for the car only
                    string query = @"
                SELECT 
                    oc.mission_id,
                    cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers + N' - ' +
                    e.employee_name + N' - ' +
                    CONVERT(nvarchar, oc.date, 23) as mission_display,
                    oc.date as mission_date,
                    oc.kilometer_start,
                    oc.kilometer_end
                FROM operation_command oc
                INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                INNER JOIN employees e ON oc.driver_name = e.employee_id
                WHERE oc.v_number = @carId 
                  AND oc.deleted = 0
                ORDER BY oc.date DESC, oc.mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carId", carId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} missions");

                    ddlMission.Items.Clear();
                    ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));

                    HashSet<string> uniqueMissions = new HashSet<string>();

                    foreach (DataRow row in dt.Rows)
                    {
                        string missionId = row["mission_id"].ToString();
                        string missionDisplay = row["mission_display"].ToString();

                        if (!uniqueMissions.Contains(missionDisplay))
                        {
                            uniqueMissions.Add(missionDisplay);
                            ddlMission.Items.Add(new ListItem(missionDisplay, missionId));
                        }
                    }

                    if (selectedMissionId.HasValue && selectedMissionId.Value > 0)
                    {
                        ListItem item = ddlMission.Items.FindByValue(selectedMissionId.ToString());
                        if (item != null)
                        {
                            ddlMission.SelectedValue = selectedMissionId.ToString();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Added {ddlMission.Items.Count - 1} unique missions to dropdown");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading missions: {ex.Message}");
                ddlMission.Items.Clear();
                ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
            }
        }
        #endregion

        #region Event Handlers

        protected void ddlCarNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            int carId = Convert.ToInt32(ddlCarNumber.SelectedValue);
            LoadMissions(carId);
        }

        #endregion

        #region Search Methods

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchViolations();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchCar.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";

            SearchViolations();
        }

        private void SearchViolations()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string query = @"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY tv.violation_date DESC, tv.violation_id DESC) AS RowNum,
                    tv.violation_id,
                    tv.violation_type,
                    tv.violation_amount,
                    tv.violation_date,
                    tv.location,
                    tv.notes,
                    cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers as car_number,
                    e.employee_name as driver_name,
                    oc.date as mission_date
                FROM traffic_violations tv
                INNER JOIN operation_command oc ON tv.vehicle_id = oc.mission_id
                INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                INNER JOIN employees e ON oc.driver_name = e.employee_id
                WHERE 1=1";

                    if (!string.IsNullOrEmpty(txtSearchCar.Text.Trim()))
                    {
                        query += @" AND (cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers LIKE @carNumber)";
                    }

                    if (!string.IsNullOrEmpty(txtFromDate.Text))
                    {
                        query += " AND tv.violation_date >= @fromDate";
                    }

                    if (!string.IsNullOrEmpty(txtToDate.Text))
                    {
                        query += " AND tv.violation_date <= @toDate";
                    }

                    query += " ORDER BY tv.violation_date DESC, tv.violation_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(txtSearchCar.Text.Trim()))
                        cmd.Parameters.AddWithValue("@carNumber", "%" + txtSearchCar.Text.Trim() + "%");

                    if (!string.IsNullOrEmpty(txtFromDate.Text))
                        cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(txtFromDate.Text));

                    if (!string.IsNullOrEmpty(txtToDate.Text))
                        cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(txtToDate.Text));

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvViolations.DataSource = dt;
                    gvViolations.DataBind();
                    searchResults.Visible = true;

                    UpdateStatisticsFromDataTable(dt);

                    if (dt.Rows.Count == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            "alert('لا توجد نتائج للبحث');", true);
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

        protected void gvViolations_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvViolations.PageIndex = e.NewPageIndex;
            SearchViolations();
        }

        #endregion

        #region Save Methods

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                // Editing is not allowed for traffic violations - every save creates a new record
                InsertViolation();

                ClearForm();
                LoadStatistics();
                SearchViolations();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('خطأ في الحفظ: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private void InsertViolation()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Get the violation type name from the selected ID
                string violationTypeName = GetViolationTypeName(Convert.ToInt32(ddlViolationType.SelectedValue));

                int missionId = Convert.ToInt32(ddlMission.SelectedValue);

                // Driver is automatically derived from the selected mission
                int driverId = GetDriverIdFromMission(missionId);

                string query = @"
                    INSERT INTO traffic_violations 
                        (vehicle_id, operation_command_id, violation_type, violation_amount, 
                         violation_date, location, notes)
                    VALUES 
                        (@missionId, @driverId, @type, @amount, @date, @location, @notes)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@missionId", missionId);
                cmd.Parameters.AddWithValue("@driverId", driverId);
                cmd.Parameters.AddWithValue("@type", violationTypeName);
                cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtAmount.Text));
                cmd.Parameters.AddWithValue("@date", Convert.ToDateTime(txtViolationDate.Text));
                cmd.Parameters.AddWithValue("@location", string.IsNullOrEmpty(txtLocation.Text) ? (object)DBNull.Value : txtLocation.Text.Trim());
                cmd.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());

                conn.Open();
                cmd.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "alert('تم حفظ المخالفة بنجاح');", true);
            }
        }

        // Helper: get driver id from mission (driver selection has been removed)
        private int GetDriverIdFromMission(int missionId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT driver_name FROM operation_command WHERE mission_id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", missionId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting driver from mission: {ex.Message}");
                return 0;
            }
        }

        private string GetViolationTypeName(int violationTypeId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT violation_type_name FROM violation_types WHERE violation_type_id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", violationTypeId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ddlViolationType.SelectedIndex = 0;
            ddlCarNumber.SelectedIndex = 0;
            ddlMission.Items.Clear();
            ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
            txtAmount.Text = "";
            txtViolationDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtLocation.Text = "";
            txtNotes.Text = "";
        }

        private bool ValidateForm()
        {
            string errorMessage = "";

            if (ddlViolationType.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار نوع المخالفة\n";

            if (ddlCarNumber.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار رقم العربية\n";

            if (ddlMission.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار المهمة\n";

            if (string.IsNullOrEmpty(txtAmount.Text) || !decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                errorMessage += "• الرجاء إدخال قيمة المخالفة بشكل صحيح\n";

            if (string.IsNullOrEmpty(txtViolationDate.Text))
                errorMessage += "• الرجاء إدخال تاريخ المخالفة\n";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validationError",
                    $"alert('{errorMessage}');", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Methods

        private void LoadStatistics()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT 
                        COUNT(*) as total_count,
                        ISNULL(SUM(violation_amount), 0) as total_amount
                    FROM traffic_violations";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTotalViolations.Text = reader["total_count"].ToString();
                    lblTotalAmount.Text = Convert.ToDecimal(reader["total_amount"]).ToString("N2");
                }
                reader.Close();
            }
        }

        private void UpdateStatisticsFromDataTable(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                decimal totalAmount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["violation_amount"]);
                }

                lblTotalViolations.Text = dt.Rows.Count.ToString();
                lblTotalAmount.Text = totalAmount.ToString("N2");
            }
            else
            {
                lblTotalViolations.Text = "0";
                lblTotalAmount.Text = "0.00";
            }
        }

        #endregion
    }
}