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

                // Load dropdowns
                LoadCarNumbers();
                LoadDrivers();
                LoadViolationTypes(); // Load violation types from database

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
                        System.Diagnostics.Debug.WriteLine($"Car ID: {carId}, Number: {carDisplay}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading car numbers: {ex.Message}");
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
                            employee_id,
                            employee_name
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
                    ddlDriverName.DataTextField = "employee_name";
                    ddlDriverName.DataValueField = "employee_id";
                    ddlDriverName.DataBind();

                    ddlDriverName.Items.Insert(0, new ListItem("-- اختر اسم السائق --", "0"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading drivers: {ex.Message}");
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

        // Load missions based on selected car and driver
        private void LoadMissions(int carId, int driverId, int? selectedMissionId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadMissions - CarId: {carId}, DriverId: {driverId}");

                // If no car or driver selected, clear dropdown
                if (carId == 0 || driverId == 0)
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

                    // FIXED: Check if driver_name exists in operation_command with this employee_id
                    // Also, show missions that are not completed (kilometer_end is NULL or 0)
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
                  AND oc.driver_name = @driverId
                  AND oc.deleted = 0
                ORDER BY oc.date DESC, oc.mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carId", carId);
                    cmd.Parameters.AddWithValue("@driverId", driverId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} missions");

                    ddlMission.Items.Clear();
                    ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));

                    // Also check if there are missions for this car with ANY driver (if no missions found)
                    if (dt.Rows.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"No missions found for CarId: {carId}, DriverId: {driverId}");
                        System.Diagnostics.Debug.WriteLine("Checking if car has missions with other drivers...");

                        // Check if car has missions with other drivers
                        string checkCarQuery = @"
                    SELECT COUNT(*) as mission_count
                    FROM operation_command oc
                    WHERE oc.v_number = @carId
                      AND oc.deleted = 0";

                        SqlCommand checkCarCmd = new SqlCommand(checkCarQuery, conn);
                        checkCarCmd.Parameters.AddWithValue("@carId", carId);
                        int carMissionCount = (int)checkCarCmd.ExecuteScalar();

                        if (carMissionCount > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Car has {carMissionCount} missions with other drivers");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Car has NO missions at all");
                        }

                        // Check if driver has missions with other cars
                        string checkDriverQuery = @"
                    SELECT COUNT(*) as mission_count
                    FROM operation_command oc
                    WHERE oc.driver_name = @driverId
                      AND oc.deleted = 0";

                        SqlCommand checkDriverCmd = new SqlCommand(checkDriverQuery, conn);
                        checkDriverCmd.Parameters.AddWithValue("@driverId", driverId);
                        int driverMissionCount = (int)checkDriverCmd.ExecuteScalar();

                        if (driverMissionCount > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Driver has {driverMissionCount} missions with other cars");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Driver has NO missions at all");
                        }
                    }

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
            LoadMissionsBasedOnSelection();
        }

        protected void ddlDriverName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMissionsBasedOnSelection();
        }

        private void LoadMissionsBasedOnSelection()
        {
            int carId = Convert.ToInt32(ddlCarNumber.SelectedValue);
            int driverId = Convert.ToInt32(ddlDriverName.SelectedValue);

            System.Diagnostics.Debug.WriteLine($"LoadMissionsBasedOnSelection - CarId: {carId}, DriverId: {driverId}");

            LoadMissions(carId, driverId);
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
            txtSearchDriver.Text = "";
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

                    if (!string.IsNullOrEmpty(txtSearchDriver.Text.Trim()))
                    {
                        query += " AND e.employee_name LIKE @driverName";
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

                    if (!string.IsNullOrEmpty(txtSearchDriver.Text.Trim()))
                        cmd.Parameters.AddWithValue("@driverName", "%" + txtSearchDriver.Text.Trim() + "%");

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

        protected void gvViolations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditViolation")
            {
                int violationId = Convert.ToInt32(e.CommandArgument);
                LoadViolationForEdit(violationId);
            }
            else if (e.CommandName == "DeleteViolation")
            {
                int violationId = Convert.ToInt32(e.CommandArgument);
                DeleteViolation(violationId);
            }
        }

        #endregion

        #region Save Methods

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                int violationId = Convert.ToInt32(hdnViolationId.Value);

                if (violationId > 0)
                {
                    UpdateViolation(violationId);
                }
                else
                {
                    InsertViolation();
                }

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

                string query = @"
                    INSERT INTO traffic_violations 
                        (vehicle_id, operation_command_id, violation_type, violation_amount, 
                         violation_date, location, notes)
                    VALUES 
                        (@missionId, @driverId, @type, @amount, @date, @location, @notes)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@missionId", Convert.ToInt32(ddlMission.SelectedValue));
                cmd.Parameters.AddWithValue("@driverId", Convert.ToInt32(ddlDriverName.SelectedValue));
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

        private void UpdateViolation(int violationId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Get the violation type name from the selected ID
                string violationTypeName = GetViolationTypeName(Convert.ToInt32(ddlViolationType.SelectedValue));

                string query = @"
                    UPDATE traffic_violations 
                    SET vehicle_id = @missionId,
                        operation_command_id = @driverId,
                        violation_type = @type,
                        violation_amount = @amount,
                        violation_date = @date,
                        location = @location,
                        notes = @notes
                    WHERE violation_id = @violationId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@violationId", violationId);
                cmd.Parameters.AddWithValue("@missionId", Convert.ToInt32(ddlMission.SelectedValue));
                cmd.Parameters.AddWithValue("@driverId", Convert.ToInt32(ddlDriverName.SelectedValue));
                cmd.Parameters.AddWithValue("@type", violationTypeName);
                cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtAmount.Text));
                cmd.Parameters.AddWithValue("@date", Convert.ToDateTime(txtViolationDate.Text));
                cmd.Parameters.AddWithValue("@location", string.IsNullOrEmpty(txtLocation.Text) ? (object)DBNull.Value : txtLocation.Text.Trim());
                cmd.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());

                conn.Open();
                cmd.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "alert('تم تحديث المخالفة بنجاح');", true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ddlCarNumber.SelectedIndex = 0;
            ddlDriverName.SelectedIndex = 0;
            ddlMission.Items.Clear();
            ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
            ddlViolationType.SelectedIndex = 0;
            txtAmount.Text = "";
            txtViolationDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtLocation.Text = "";
            txtNotes.Text = "";
            hdnViolationId.Value = "0";
        }

        private bool ValidateForm()
        {
            string errorMessage = "";

            if (ddlCarNumber.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار رقم العربية\n";

            if (ddlDriverName.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار اسم السائق\n";

            if (ddlMission.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار المهمة\n";

            if (ddlViolationType.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار نوع المخالفة\n";

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

        private void LoadViolationForEdit(int violationId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT 
                        tv.*,
                        oc.v_number as car_number_id,
                        oc.driver_name as driver_id,
                        oc.mission_id,
                        vt.violation_type_id
                    FROM traffic_violations tv
                    INNER JOIN operation_command oc ON tv.vehicle_id = oc.mission_id
                    LEFT JOIN violation_types vt ON tv.violation_type = vt.violation_type_name
                    WHERE tv.violation_id = @violationId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@violationId", violationId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string carId = reader["car_number_id"].ToString();
                    ListItem carItem = ddlCarNumber.Items.FindByValue(carId);
                    if (carItem != null)
                        ddlCarNumber.SelectedValue = carId;

                    string driverId = reader["driver_id"].ToString();
                    ListItem driverItem = ddlDriverName.Items.FindByValue(driverId);
                    if (driverItem != null)
                        ddlDriverName.SelectedValue = driverId;

                    int missionId = Convert.ToInt32(reader["mission_id"]);
                    LoadMissions(Convert.ToInt32(carId), Convert.ToInt32(driverId), missionId);

                    // Set violation type by finding the matching ID
                    string violationTypeName = reader["violation_type"].ToString();
                    foreach (ListItem item in ddlViolationType.Items)
                    {
                        if (item.Text == violationTypeName)
                        {
                            ddlViolationType.SelectedValue = item.Value;
                            break;
                        }
                    }

                    txtAmount.Text = reader["violation_amount"].ToString();
                    txtViolationDate.Text = Convert.ToDateTime(reader["violation_date"]).ToString("yyyy-MM-dd");
                    txtLocation.Text = reader["location"].ToString();
                    txtNotes.Text = reader["notes"].ToString();
                    hdnViolationId.Value = violationId.ToString();
                }
                reader.Close();
            }
        }

        private void DeleteViolation(int violationId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "DELETE FROM traffic_violations WHERE violation_id = @violationId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@violationId", violationId);

                conn.Open();
                cmd.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "deleteSuccess",
                    "alert('تم حذف المخالفة بنجاح');", true);

                SearchViolations();
                LoadStatistics();
            }
        }

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