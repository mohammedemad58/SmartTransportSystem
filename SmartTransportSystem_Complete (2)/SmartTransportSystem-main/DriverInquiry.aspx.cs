using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class DriverInquiry : System.Web.UI.Page
    {
        private static string ConnString
        {
            get { return ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAllDriversForSearch();
                LoadDriversGrid();
                LoadStatistics();
                LoadTrafficUnits();
                LoadSearchTrafficUnits();
            }
        }

        #region Load Data Methods

        private void LoadDriversGrid()
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY e.employee_name) AS RowNum,
                            d.driver_id,
                            e.employee_name as driver_name,
                            d.license_number,
                            e.phone_number as phone,
                            d.license_type,
                            FORMAT(d.license_start_date, 'yyyy-MM-dd') as license_start_date,
                            FORMAT(d.license_end_date, 'yyyy-MM-dd') as license_end_date,
                            CASE 
                                WHEN d.license_end_date IS NULL THEN N'غير محدد'
                                WHEN d.license_end_date < CAST(GETDATE() AS date) THEN N'منتهية'
                                WHEN d.license_end_date = CAST(GETDATE() AS date) THEN N'تنتهي اليوم'
                                WHEN d.license_end_date <= DATEADD(day, 30, CAST(GETDATE() AS date)) 
                                    THEN N'ينتهي خلال ' + CAST(DATEDIFF(day, CAST(GETDATE() AS date), d.license_end_date) AS NVARCHAR(10)) + N' يوم'
                                ELSE N'سارية'
                            END AS license_status,
                            CASE 
                                WHEN d.license_end_date IS NULL THEN 'none'
                                WHEN d.license_end_date <= CAST(GETDATE() AS date) THEN 'expired'
                                WHEN d.license_end_date <= DATEADD(day, 30, CAST(GETDATE() AS date)) THEN 'soon'
                                ELSE 'ok'
                            END AS license_state,
                            e.national_id,
                            ISNULL(tu.name, '') as traffic_unit,
                            ISNULL(FORMAT(d.renewal_amount, 'N2'), '0') as renewal_amount
                        FROM drivers d
                        INNER JOIN employees e ON d.employee_id = e.employee_id
                        LEFT JOIN traffic_unit tu ON d.traffic_unit_id = tu.id_TF
                        WHERE d.is_active = 1
                        ORDER BY e.employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDriversList.DataSource = dt;
                    gvDriversList.DataBind();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading drivers grid: {ex.Message}");
                }
            }
        }

        private void LoadAllDriversForSearch()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY e.employee_name) AS RowNum,
                            d.driver_id,
                            e.employee_name as driver_name,
                            e.phone_number as phone,
                            e.national_id,
                            ISNULL(tu.name, '') as traffic_unit,
                            ISNULL(FORMAT(d.renewal_amount, 'N2'), '0') as renewal_amount
                        FROM drivers d
                        INNER JOIN employees e ON d.employee_id = e.employee_id
                        LEFT JOIN traffic_unit tu ON d.traffic_unit_id = tu.id_TF
                        WHERE d.is_active = 1
                        ORDER BY e.employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ViewState["AllDrivers"] = dt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading all drivers: {ex.Message}");
            }
        }

        private void LoadStatistics()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT 
                            COUNT(*) as total_drivers
                        FROM drivers 
                        WHERE is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        lblTotalDrivers.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }

        private void LoadTrafficUnits()
        {
            try
            {
                if (ddlTrafficUnit == null) return;

                ddlTrafficUnit.Items.Clear();
                ddlTrafficUnit.Items.Add(new ListItem("-- اختر وحدة المرور --", ""));

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT id_TF, name
                        FROM traffic_unit
                        WHERE is_active = 1 
                          AND (deleted IS NULL OR deleted = 0x0)
                          AND (hide IS NULL OR hide = 0x0)
                        ORDER BY name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id_TF"]);
                            string name = reader["name"].ToString();
                            ddlTrafficUnit.Items.Add(new ListItem(name, id.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading traffic units: {ex.Message}");
                if (ddlTrafficUnit != null)
                {
                    ddlTrafficUnit.Items.Clear();
                    ddlTrafficUnit.Items.Add(new ListItem("-- اختر وحدة المرور --", ""));
                }
            }
        }

        private void LoadSearchTrafficUnits()
        {
            try
            {
                if (ddlSearchTrafficUnit == null) return;

                ddlSearchTrafficUnit.Items.Clear();
                ddlSearchTrafficUnit.Items.Add(new ListItem("-- كل وحدات المرور --", ""));

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT id_TF, name
                        FROM traffic_unit
                        WHERE is_active = 1 
                          AND (deleted IS NULL OR deleted = 0x0)
                          AND (hide IS NULL OR hide = 0x0)
                        ORDER BY name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id_TF"]);
                            string name = reader["name"].ToString();
                            ddlSearchTrafficUnit.Items.Add(new ListItem(name, id.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading search traffic units: {ex.Message}");
            }
        }

        #endregion

        #region GridView Events

        protected void gvDriversList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SetTab("tabDriverList");
            gvDriversList.PageIndex = e.NewPageIndex;
            LoadDriversGrid();
        }

        protected void gvDriversList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditDriver")
            {
                SetTab("tabAddDriver");   // the edit form is on the first tab
                int driverId = Convert.ToInt32(e.CommandArgument);
                LoadDriverForEdit(driverId);
            }
        }

        protected void gvDriverTasks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SetTab("tabSearchResults");
            gvDriverTasks.PageIndex = e.NewPageIndex;

            if (!string.IsNullOrEmpty(hdnDriverId.Value) && hdnDriverId.Value != "0")
            {
                int driverId = Convert.ToInt32(hdnDriverId.Value);
                LoadDriverData(driverId);
            }
        }

        #endregion

        #region Driver Search Methods

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SetTab("tabSearchResults");

                string searchText = txtDriverSearch.Text.Trim();
                string trafficUnitId = ddlSearchTrafficUnit.SelectedValue;

                if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(trafficUnitId))
                {
                    ShowAlert("الرجاء إدخال اسم السائق أو اختيار وحدة مرور للبحث");
                    return;
                }

                SearchDrivers(searchText, trafficUnitId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                ShowAlert("خطأ في البحث: " + ex.Message);
            }
        }

        private void SearchDrivers(string searchText, string trafficUnitId)
        {
            try
            {
                DataTable allDrivers = ViewState["AllDrivers"] as DataTable;

                if (allDrivers == null)
                {
                    LoadAllDriversForSearch();
                    allDrivers = ViewState["AllDrivers"] as DataTable;
                }

                if (allDrivers != null)
                {
                    string filterExpression = "";

                    if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(trafficUnitId))
                    {
                        string trafficUnitName = GetTrafficUnitNameById(trafficUnitId);
                        filterExpression = $"driver_name LIKE '%{searchText.Replace("'", "''")}%' AND traffic_unit = '{trafficUnitName.Replace("'", "''")}'";
                    }
                    else if (!string.IsNullOrEmpty(searchText))
                    {
                        filterExpression = $"driver_name LIKE '%{searchText.Replace("'", "''")}%'";
                    }
                    else if (!string.IsNullOrEmpty(trafficUnitId))
                    {
                        string trafficUnitName = GetTrafficUnitNameById(trafficUnitId);
                        filterExpression = $"traffic_unit = '{trafficUnitName.Replace("'", "''")}'";
                    }

                    DataRow[] results = allDrivers.Select(filterExpression);
                    DataTable filteredTable = allDrivers.Clone();

                    foreach (DataRow row in results)
                    {
                        filteredTable.ImportRow(row);
                    }

                    gvDriverSearchResults.DataSource = filteredTable;
                    gvDriverSearchResults.DataBind();
                    driverSearchResults.Visible = filteredTable.Rows.Count > 0;

                    if (filteredTable.Rows.Count == 0)
                    {
                        ShowAlert("لا توجد نتائج للبحث");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching drivers: {ex.Message}");
                ShowAlert("خطأ في البحث: " + ex.Message);
            }
        }

        private string GetTrafficUnitNameById(string trafficUnitId)
        {
            if (string.IsNullOrEmpty(trafficUnitId)) return "";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = "SELECT name FROM traffic_unit WHERE id_TF = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(trafficUnitId));
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

        protected void gvDriverSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectDriver")
            {
                int driverId = Convert.ToInt32(e.CommandArgument);
                SelectDriver(driverId);
            }
        }

        private void SelectDriver(int driverId)
        {
            try
            {
                LoadDriverForEdit(driverId);
                driverSearchResults.Visible = false;
                txtDriverSearch.Text = "";
                ddlSearchTrafficUnit.SelectedIndex = 0;

                // Edit form is on the "add driver" tab
                SetTab("tabAddDriver");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting driver: {ex.Message}");
                ShowAlert("خطأ في اختيار السائق: " + ex.Message);
            }
        }

        #endregion

        #region Driver CRUD Operations

        protected void btnAddDriver_Click(object sender, EventArgs e)
        {
            try
            {
                SetTab("tabAddDriver");

                if (!ValidateDriverForm())
                    return;

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    // Check if employee with this national_id already exists
                    string checkEmployeeQuery = @"
                        SELECT employee_id 
                        FROM employees 
                        WHERE national_id = @nationalId";

                    SqlCommand checkEmployeeCmd = new SqlCommand(checkEmployeeQuery, conn);
                    checkEmployeeCmd.Parameters.AddWithValue("@nationalId", txtNationalId.Text.Trim());

                    object existingEmployee = checkEmployeeCmd.ExecuteScalar();
                    int employeeId;

                    if (existingEmployee != null && existingEmployee != DBNull.Value)
                    {
                        employeeId = Convert.ToInt32(existingEmployee);

                        // This employee is already an active driver -> do not add a duplicate
                        SqlCommand dupCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM drivers WHERE employee_id = @employeeId AND is_active = 1", conn);
                        dupCmd.Parameters.AddWithValue("@employeeId", employeeId);

                        if ((int)dupCmd.ExecuteScalar() > 0)
                        {
                            ShowMessage("هذا السائق مسجل بالفعل (نفس الرقم القومي)");
                            return;
                        }
                    }
                    else
                    {
                        string insertEmployeeQuery = @"
                            INSERT INTO employees 
                                (employee_name, employee_position, phone_number, national_id, is_active, created_date)
                            VALUES 
                                (@name, @position, @phone, @nationalId, 1, GETDATE());
                            SELECT SCOPE_IDENTITY();";

                        SqlCommand insertEmployeeCmd = new SqlCommand(insertEmployeeQuery, conn);
                        insertEmployeeCmd.Parameters.AddWithValue("@name", txtDriverName.Text.Trim());
                        insertEmployeeCmd.Parameters.AddWithValue("@position", "سائق");
                        insertEmployeeCmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        insertEmployeeCmd.Parameters.AddWithValue("@nationalId", txtNationalId.Text.Trim());

                        employeeId = Convert.ToInt32(insertEmployeeCmd.ExecuteScalar());
                    }

                    // Get traffic unit ID
                    int? trafficUnitId = null;
                    if (!string.IsNullOrEmpty(ddlTrafficUnit.SelectedValue))
                    {
                        trafficUnitId = Convert.ToInt32(ddlTrafficUnit.SelectedValue);
                    }

                    // Get renewal amount
                    decimal renewalAmount = 0;
                    if (!string.IsNullOrEmpty(txtRenewalAmount.Text.Trim()))
                    {
                        decimal.TryParse(txtRenewalAmount.Text.Trim(), out renewalAmount);
                    }

                    // Insert new driver with traffic_unit_id
                    string insertDriverQuery = @"
                        INSERT INTO drivers 
                            (license_number, license_type, license_start_date, license_end_date, 
                             employee_id, is_active, created_date, traffic_unit_id, renewal_amount)
                        VALUES 
                            (@licenseNumber, @licenseType, @licenseStartDate, @licenseEndDate, 
                             @employeeId, 1, GETDATE(), @trafficUnitId, @renewalAmount)";

                    SqlCommand cmd = new SqlCommand(insertDriverQuery, conn);
                    cmd.Parameters.AddWithValue("@licenseNumber", txtLicenseNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@licenseType", ddlLicenseType.SelectedValue);
                    cmd.Parameters.AddWithValue("@licenseStartDate", Convert.ToDateTime(txtLicenseStartDate.Text));
                    cmd.Parameters.AddWithValue("@licenseEndDate", Convert.ToDateTime(txtLicenseEndDate.Text));
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@trafficUnitId", trafficUnitId.HasValue ? (object)trafficUnitId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@renewalAmount", renewalAmount);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ClearDriverForm();
                        LoadDriversGrid();
                        LoadAllDriversForSearch();
                        LoadStatistics();
                        LoadTrafficUnits();
                        LoadSearchTrafficUnits();
                        ShowSuccessMessage("تم إضافة السائق بنجاح");
                    }
                    else
                    {
                        ShowMessage("فشل في إضافة السائق");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في إضافة السائق: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in btnAddDriver_Click: {ex.Message}");
            }
        }

        protected void btnUpdateDriver_Click(object sender, EventArgs e)
        {
            try
            {
                SetTab("tabAddDriver");

                int driverId = string.IsNullOrEmpty(hdnDriverId.Value) ? 0 : Convert.ToInt32(hdnDriverId.Value);

                if (driverId == 0)
                {
                    ShowMessage("الرجاء اختيار سائق للتعديل");
                    return;
                }

                if (!ValidateDriverForm())
                    return;

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    // Get the employee_id from the driver
                    int employeeId = 0;
                    string getEmpQuery = "SELECT employee_id FROM drivers WHERE driver_id = @driverId";
                    SqlCommand getEmpCmd = new SqlCommand(getEmpQuery, conn);
                    getEmpCmd.Parameters.AddWithValue("@driverId", driverId);
                    object empObj = getEmpCmd.ExecuteScalar();

                    if (empObj != null && empObj != DBNull.Value)
                    {
                        employeeId = Convert.ToInt32(empObj);
                    }

                    // Update Employee Table
                    if (employeeId > 0)
                    {
                        string updateEmployeeQuery = @"
                            UPDATE employees 
                            SET employee_name = @name,
                                phone_number = @phone,
                                national_id = @nationalId
                            WHERE employee_id = @employeeId";

                        SqlCommand empCmd = new SqlCommand(updateEmployeeQuery, conn);
                        empCmd.Parameters.AddWithValue("@name", txtDriverName.Text.Trim());
                        empCmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        empCmd.Parameters.AddWithValue("@nationalId", txtNationalId.Text.Trim());
                        empCmd.Parameters.AddWithValue("@employeeId", employeeId);
                        empCmd.ExecuteNonQuery();
                    }

                    // Get traffic unit ID
                    int? trafficUnitId = null;
                    if (!string.IsNullOrEmpty(ddlTrafficUnit.SelectedValue))
                    {
                        trafficUnitId = Convert.ToInt32(ddlTrafficUnit.SelectedValue);
                    }

                    // Get renewal amount
                    decimal renewalAmount = 0;
                    if (!string.IsNullOrEmpty(txtRenewalAmount.Text.Trim()))
                    {
                        decimal.TryParse(txtRenewalAmount.Text.Trim(), out renewalAmount);
                    }

                    // Update Driver Table
                    string updateDriverQuery = @"
                        UPDATE drivers 
                        SET license_number = @licenseNumber,
                            license_type = @licenseType,
                            license_start_date = @licenseStartDate,
                            license_end_date = @licenseEndDate,
                            traffic_unit_id = @trafficUnitId,
                            renewal_amount = @renewalAmount
                        WHERE driver_id = @driverId";

                    SqlCommand cmd = new SqlCommand(updateDriverQuery, conn);
                    cmd.Parameters.AddWithValue("@driverId", driverId);
                    cmd.Parameters.AddWithValue("@licenseNumber", txtLicenseNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@licenseType", ddlLicenseType.SelectedValue);
                    cmd.Parameters.AddWithValue("@licenseStartDate", Convert.ToDateTime(txtLicenseStartDate.Text));
                    cmd.Parameters.AddWithValue("@licenseEndDate", Convert.ToDateTime(txtLicenseEndDate.Text));
                    cmd.Parameters.AddWithValue("@trafficUnitId", trafficUnitId.HasValue ? (object)trafficUnitId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@renewalAmount", renewalAmount);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ClearDriverForm();
                        LoadDriversGrid();
                        LoadAllDriversForSearch();
                        LoadStatistics();
                        LoadTrafficUnits();
                        LoadSearchTrafficUnits();
                        ShowSuccessMessage("تم تحديث بيانات السائق بنجاح");
                    }
                    else
                    {
                        ShowMessage("فشل في تحديث السائق - لم يتم تحديث أي بيانات");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحديث السائق: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in btnUpdateDriver_Click: {ex.Message}");
            }
        }

        protected void btnCancelDriver_Click(object sender, EventArgs e)
        {
            SetTab("tabAddDriver");
            ClearDriverForm();
        }

        private void LoadDriverForEdit(int driverId)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            d.driver_id,
                            d.license_number,
                            d.license_type,
                            d.license_start_date,
                            d.license_end_date,
                            d.employee_id,
                            d.traffic_unit_id,
                            d.renewal_amount,
                            e.national_id,
                            e.employee_name,
                            e.phone_number,
                            tu.name as traffic_unit_name
                        FROM drivers d
                        INNER JOIN employees e ON d.employee_id = e.employee_id
                        LEFT JOIN traffic_unit tu ON d.traffic_unit_id = tu.id_TF
                        WHERE d.driver_id = @driverId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@driverId", driverId);

                    bool found = false;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            found = true;

                            hdnDriverId.Value = reader["driver_id"].ToString();
                            hdnEmployeeId.Value = reader["employee_id"].ToString();

                            txtDriverName.Text = reader["employee_name"].ToString();
                            txtPhone.Text = reader["phone_number"].ToString();
                            txtNationalId.Text = reader["national_id"].ToString();
                            txtLicenseNumber.Text = reader["license_number"].ToString();

                            // Set license type
                            string licenseType = reader["license_type"].ToString().Trim();
                            if (ddlLicenseType.Items.FindByValue(licenseType) != null)
                            {
                                ddlLicenseType.SelectedValue = licenseType;
                            }
                            else
                            {
                                ddlLicenseType.SelectedIndex = 0;
                            }

                            if (reader["license_start_date"] != DBNull.Value)
                                txtLicenseStartDate.Text = Convert.ToDateTime(reader["license_start_date"]).ToString("yyyy-MM-dd");
                            else
                                txtLicenseStartDate.Text = "";

                            if (reader["license_end_date"] != DBNull.Value)
                                txtLicenseEndDate.Text = Convert.ToDateTime(reader["license_end_date"]).ToString("yyyy-MM-dd");
                            else
                                txtLicenseEndDate.Text = "";

                            // Number-type textbox: plain invariant format (no thousands separator)
                            if (reader["renewal_amount"] != DBNull.Value)
                            {
                                txtRenewalAmount.Text = Convert.ToDecimal(reader["renewal_amount"])
                                    .ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                txtRenewalAmount.Text = "";
                            }

                            // Set traffic unit
                            int? trafficUnitId = reader["traffic_unit_id"] != DBNull.Value ?
                                Convert.ToInt32(reader["traffic_unit_id"]) : (int?)null;

                            if (trafficUnitId.HasValue &&
                                ddlTrafficUnit.Items.FindByValue(trafficUnitId.Value.ToString()) != null)
                            {
                                ddlTrafficUnit.SelectedValue = trafficUnitId.Value.ToString();
                            }
                            else
                            {
                                ddlTrafficUnit.SelectedIndex = 0;
                            }

                            btnAddDriver.Visible = false;
                            btnUpdateDriver.Visible = true;
                            btnCancelDriver.Visible = true;
                        }
                    }

                    if (found)
                    {
                        LoadDriverData(driverId);

                        ScriptManager.RegisterStartupScript(this, GetType(), "scrollToForm",
                            "var f = document.getElementById('driverFormSection'); if (f) f.scrollIntoView({behavior: 'smooth'});", true);
                    }
                    else
                    {
                        ShowMessage("لم يتم العثور على بيانات السائق");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل بيانات السائق: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine($"Error in LoadDriverForEdit: {ex.Message}");
                }
            }
        }

        private void LoadDriverData(int driverId)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();

                    string getEmployeeIdQuery = @"
                        SELECT d.employee_id, e.employee_name, e.national_id, e.phone_number
                        FROM drivers d
                        INNER JOIN employees e ON d.employee_id = e.employee_id
                        WHERE d.driver_id = @DriverID";

                    SqlCommand getEmployeeCmd = new SqlCommand(getEmployeeIdQuery, conn);
                    getEmployeeCmd.Parameters.AddWithValue("@DriverID", driverId);

                    int employeeId;
                    string employeeName;
                    string nationalId;
                    string phoneNumber;

                    using (SqlDataReader reader = getEmployeeCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowMessage("السائق غير مرتبط بموظف");
                            return;
                        }

                        employeeId = Convert.ToInt32(reader["employee_id"]);
                        employeeName = reader["employee_name"].ToString();
                        nationalId = reader["national_id"].ToString();
                        phoneNumber = reader["phone_number"].ToString();
                    }

                    string query = @"
                        SELECT 
                            @EmployeeName as [اسم السائق],
                            d.license_number as [رقم الرخصة],
                            @PhoneNumber as [رقم الهاتف],
                            d.license_type as [نوع الرخصة],
                            FORMAT(d.license_start_date, 'yyyy-MM-dd') as [تاريخ بداية الرخصة],
                            FORMAT(d.license_end_date, 'yyyy-MM-dd') as [تاريخ نهاية الرخصة],
                            @NationalId as [الرقم القومي],
                            ISNULL(tu.name, '') as [وحدة المرور],
                            ISNULL(FORMAT(d.renewal_amount, 'N2'), '0') as [مبلغ التجديد],
                            ISNULL((
                                SELECT COUNT(*) 
                                FROM operation_command oc 
                                WHERE oc.driver_name = @EmployeeId
                            ), 0) AS [عدد المهام],
                            ISNULL((
                                SELECT SUM(CAST(oc.kilometer_end AS FLOAT) - CAST(oc.kilometer_start AS FLOAT))
                                FROM operation_command oc 
                                WHERE oc.driver_name = @EmployeeId
                                  AND oc.kilometer_end IS NOT NULL 
                                  AND oc.kilometer_start IS NOT NULL
                            ), 0) AS [إجمالي المسافة (كم)],
                            ISNULL((
                                SELECT COUNT(DISTINCT oc.v_number)
                                FROM operation_command oc 
                                WHERE oc.driver_name = @EmployeeId
                            ), 0) AS [عدد المركبات المسندة],
                            STUFF((
                                SELECT DISTINCT CHAR(10) + '• ' + r.route_name
                                FROM operation_command oc
                                INNER JOIN routes r ON oc.location_point = r.route_id
                                WHERE oc.driver_name = @EmployeeId
                                  AND r.route_name IS NOT NULL
                                FOR XML PATH(''), TYPE
                            ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS [الخطوط المسير عليها]
                        FROM drivers d
                        LEFT JOIN traffic_unit tu ON d.traffic_unit_id = tu.id_TF
                        WHERE d.driver_id = @DriverID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DriverID", driverId);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                    cmd.Parameters.AddWithValue("@NationalId", nationalId);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvDriverTasks.DataSource = dt;
                        gvDriverTasks.DataBind();
                        resultBox.Visible = true;
                        lblMessage.Visible = false;
                    }
                    else
                    {
                        ShowMessage("لم يتم العثور على بيانات للسائق");
                        resultBox.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in LoadDriverData: {ex.Message}");
                    ShowMessage("خطأ: " + ex.Message);
                }
            }
        }

        private void ClearDriverForm()
        {
            hdnDriverId.Value = "0";
            hdnEmployeeId.Value = "0";
            txtDriverName.Text = "";
            txtNationalId.Text = "";
            txtLicenseNumber.Text = "";
            txtPhone.Text = "";
            ddlLicenseType.SelectedIndex = 0;
            txtLicenseStartDate.Text = "";
            txtLicenseEndDate.Text = "";
            txtRenewalAmount.Text = "";
            ddlTrafficUnit.SelectedIndex = 0;

            btnAddDriver.Visible = true;
            btnUpdateDriver.Visible = false;
            btnCancelDriver.Visible = false;
        }

        private bool ValidateDriverForm()
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(txtDriverName.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال اسم السائق\n";
            }

            if (string.IsNullOrEmpty(txtNationalId.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال الرقم القومي\n";
            }
            else if (txtNationalId.Text.Trim().Length != 14)
            {
                errorMessage += "• الرقم القومي يجب أن يتكون من 14 رقم\n";
            }
            else if (!Regex.IsMatch(txtNationalId.Text.Trim(), @"^\d+$"))
            {
                errorMessage += "• الرقم القومي يجب أن يحتوي على أرقام فقط\n";
            }
            else
            {
                char firstDigit = txtNationalId.Text.Trim()[0];
                if (firstDigit != '2' && firstDigit != '3')
                {
                    errorMessage += "• الرقم القومي يجب أن يبدأ بـ 2 أو 3\n";
                }
            }

            if (string.IsNullOrEmpty(txtLicenseNumber.Text.Trim()))
            {
                errorMessage += "• الرجاء إدخال رقم الرخصة\n";
            }
            else if (txtLicenseNumber.Text.Trim().Length != 14)
            {
                errorMessage += "• رقم الرخصة يجب أن يتكون من 14 رقم\n";
            }
            else if (!Regex.IsMatch(txtLicenseNumber.Text.Trim(), @"^\d+$"))
            {
                errorMessage += "• رقم الرخصة يجب أن يحتوي على أرقام فقط\n";
            }

            if (string.IsNullOrEmpty(ddlLicenseType.SelectedValue))
            {
                errorMessage += "• الرجاء اختيار نوع الرخصة\n";
            }

            if (string.IsNullOrEmpty(txtLicenseStartDate.Text))
            {
                errorMessage += "• الرجاء إدخال تاريخ بداية الرخصة\n";
            }

            if (string.IsNullOrEmpty(txtLicenseEndDate.Text))
            {
                errorMessage += "• الرجاء إدخال تاريخ نهاية الرخصة\n";
            }

            if (!string.IsNullOrEmpty(txtLicenseStartDate.Text) && !string.IsNullOrEmpty(txtLicenseEndDate.Text))
            {
                if (DateTime.TryParse(txtLicenseStartDate.Text, out DateTime startDate) &&
                    DateTime.TryParse(txtLicenseEndDate.Text, out DateTime endDate))
                {
                    if (endDate <= startDate)
                    {
                        errorMessage += "• تاريخ نهاية الرخصة يجب أن يكون بعد تاريخ البداية\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(txtPhone.Text.Trim()))
            {
                string phone = txtPhone.Text.Trim();
                if (phone.Length != 11)
                {
                    errorMessage += "• رقم الهاتف يجب أن يتكون من 11 رقم بالضبط\n";
                }
                else if (!Regex.IsMatch(phone, @"^\d+$"))
                {
                    errorMessage += "• رقم الهاتف يجب أن يحتوي على أرقام فقط\n";
                }
                else
                {
                    string prefix = phone.Substring(0, 3);
                    if (prefix != "010" && prefix != "011" && prefix != "012" && prefix != "015")
                    {
                        errorMessage += "• رقم الهاتف يجب أن يبدأ بـ 010, 011, 012, أو 015\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Serialize() escapes line breaks and quotes, so the alert always works
                ShowAlert("الرجاء تصحيح الأخطاء التالية:\n\n" + errorMessage);
                SetTab("tabAddDriver");
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Methods

        // ===== License expiry alert =====
        // A license that expires within this many days (or has already expired) raises the alert
        private const int LicenseWarningDays = 30;

        // Runs after every button handler, so the banner always reflects the latest data
        protected void Page_PreRender(object sender, EventArgs e)
        {
            LoadLicenseAlerts();
        }

        private void LoadLicenseAlerts()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    string query = @"
                        SELECT 
                            e.employee_name,
                            d.license_end_date,
                            DATEDIFF(day, CAST(GETDATE() AS date), d.license_end_date) AS days_left
                        FROM drivers d
                        INNER JOIN employees e ON d.employee_id = e.employee_id
                        WHERE d.is_active = 1
                          AND d.license_end_date IS NOT NULL
                          AND d.license_end_date <= DATEADD(day, @days, CAST(GETDATE() AS date))
                        ORDER BY d.license_end_date";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@days", LicenseWarningDays);
                    conn.Open();

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        licenseAlertPanel.Visible = false;
                        return;
                    }

                    int expired = 0, soon = 0;
                    StringBuilder items = new StringBuilder();

                    foreach (DataRow row in dt.Rows)
                    {
                        string name = System.Web.HttpUtility.HtmlEncode(row["employee_name"].ToString());
                        DateTime endDate = Convert.ToDateTime(row["license_end_date"]);
                        int daysLeft = Convert.ToInt32(row["days_left"]);
                        string dateText = endDate.ToString("yyyy-MM-dd");

                        if (daysLeft < 0)
                        {
                            expired++;
                            items.Append("<li class='license-expired'>" + name + " — الرخصة منتهية منذ " + (-daysLeft) + " يوم (بتاريخ " + dateText + ")</li>");
                        }
                        else if (daysLeft == 0)
                        {
                            expired++;
                            items.Append("<li class='license-expired'>" + name + " — تنتهي الرخصة اليوم (" + dateText + ")</li>");
                        }
                        else
                        {
                            soon++;
                            items.Append("<li class='license-soon'>" + name + " — تنتهي الرخصة بعد " + daysLeft + " يوم (بتاريخ " + dateText + ")</li>");
                        }
                    }

                    StringBuilder html = new StringBuilder();
                    html.Append("<div class='license-alert-title'>⚠️ تنبيه: تراخيص سائقين تحتاج تجديداً");
                    html.Append(" — منتهية: " + expired + " | تنتهي خلال " + LicenseWarningDays + " يوماً: " + soon + "</div>");
                    html.Append("<ul>" + items + "</ul>");

                    lblLicenseAlert.Text = html.ToString();
                    licenseAlertPanel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading license alerts: {ex.Message}");
                licenseAlertPanel.Visible = false;
            }
        }

        // CSS class of the license status badge in the drivers list
        protected string GetLicenseClass(object state)
        {
            switch (Convert.ToString(state))
            {
                case "expired": return "lic-badge lic-expired";
                case "soon": return "lic-badge lic-soon";
                case "ok": return "lic-badge lic-ok";
                default: return "lic-badge lic-none";
            }
        }

        // The page JS restores the tab from this hidden field after every postback
        private void SetTab(string tabId)
        {
            hfActiveTab.Value = tabId;
        }

        // Safe alert: handles line breaks, quotes and Arabic text
        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "driverAlert",
                "alert(" + new JavaScriptSerializer().Serialize(message) + ");", true);
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            resultBox.Visible = false;
            lblSuccess.Visible = false;
            SetTab("tabAddDriver");
        }

        private void ShowSuccessMessage(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
            SetTab("tabAddDriver");
        }

        #endregion
    }
}
