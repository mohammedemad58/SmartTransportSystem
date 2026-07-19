using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class employee_subscriber_management : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEmployees();
                LoadRoutes();
                LoadEmployeePositionsDropdown();
                LoadSearchPositions();
                LoadSearchType();
                LoadAllEmployeesForSearch();
                LoadEmployeesForGridView();

                // Hide statistics initially using JavaScript
                ScriptManager.RegisterStartupScript(this, GetType(), "hideStats",
                    "if(document.getElementById('statsCardsRow')) document.getElementById('statsCardsRow').style.display = 'none';", true);
            }

            if (IsPostBack && Request["__EVENTTARGET"] == "SearchEmployee")
            {
                string searchText = Request["__EVENTARGUMENT"];
                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchEmployeeByName(searchText);
                }
            }
        }

        #region Helper Methods

        protected string GetStatus(object endDateObj)
        {
            if (endDateObj == DBNull.Value || endDateObj == null)
                return "غير محدد";

            DateTime endDate = Convert.ToDateTime(endDateObj);
            return endDate >= DateTime.Today ? "نشط" : "منتهي";
        }

        protected string GetStatusClass(object endDateObj)
        {
            if (endDateObj == DBNull.Value || endDateObj == null)
                return "badge-secondary";

            DateTime endDate = Convert.ToDateTime(endDateObj);
            return endDate >= DateTime.Today ? "badge-subscribed" : "badge-pay-per-use";
        }

        private bool IsValidNationalId(string nationalId)
        {
            if (string.IsNullOrEmpty(nationalId)) return false;
            return Regex.IsMatch(nationalId, @"^[23]\d{13}$");
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return true;
            return Regex.IsMatch(phone, @"^(010|011|012|015)\d{8}$");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return true;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsNationalIdUnique(string nationalId, int excludeEmployeeId = 0)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT COUNT(*) FROM employees WHERE national_id = @nationalId AND employee_id != @excludeId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nationalId", nationalId);
                    cmd.Parameters.AddWithValue("@excludeId", excludeEmployeeId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count == 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking national ID uniqueness: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Load Data Methods

        private void LoadEmployees()
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
                        ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlEmployee.Items.Clear();
                    ddlEmployee.Items.Add(new ListItem("-- اختر موظف --", "0"));

                    if (dt.Rows.Count > 0)
                    {
                        ddlEmployee.DataSource = dt;
                        ddlEmployee.DataTextField = "employee_name";
                        ddlEmployee.DataValueField = "employee_id";
                        ddlEmployee.DataBind();
                    }

                    System.Diagnostics.Debug.WriteLine($"Loaded {ddlEmployee.Items.Count - 1} employees");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employees: {ex.Message}");
            }
        }

        private void LoadEmployeesForGridView()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY employee_name) AS RowNum,
                    employee_id,
                    employee_name,
                    employee_position,
                    ISNULL(phone_number, '') as phone_number,
                    ISNULL(email, '') as email,
                    ISNULL(national_id, '') as national_id
                FROM employees 
                WHERE is_active = 1
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvEmployees.DataSource = dt;
                    gvEmployees.DataBind();

                    // Hide the employee_id column (second column - index 1)
                    if (gvEmployees.Columns.Count > 1)
                    {
                        gvEmployees.Columns[1].Visible = false;
                    }

                    if (dt.Rows.Count > 0)
                    {
                        employeeResults.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employees grid: {ex.Message}");
            }
        }

        private void LoadAllEmployeesForSearch()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY employee_name) AS RowNum,
                    employee_id,
                    employee_name,
                    employee_position,
                    phone_number,
                    national_id
                FROM employees 
                WHERE is_active = 1
                ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ViewState["AllEmployees"] = dt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading all employees: {ex.Message}");
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
                            route_name + ' - ' + ISNULL(description, '') as route_name
                        FROM routes 
                        WHERE is_active = 1
                        ORDER BY route_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlRoute.Items.Clear();
                    ddlRoute.Items.Add(new ListItem("-- اختر خط السير --", "0"));

                    if (dt.Rows.Count > 0)
                    {
                        ddlRoute.DataSource = dt;
                        ddlRoute.DataTextField = "route_name";
                        ddlRoute.DataValueField = "route_id";
                        ddlRoute.DataBind();
                    }

                    System.Diagnostics.Debug.WriteLine($"Loaded {ddlRoute.Items.Count - 1} routes");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading routes: {ex.Message}");
            }
        }

        private void LoadEmployeePositionsDropdown()
        {
            try
            {
                ddlEmpPosition.Items.Clear();
                ddlEmpPosition.Items.Add(new ListItem("-- اختر الوظيفة --", ""));

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT position_name FROM employee_positions WHERE is_active = 1 ORDER BY position_name";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string positionName = reader["position_name"].ToString();
                        ddlEmpPosition.Items.Add(new ListItem(positionName, positionName));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee positions dropdown: {ex.Message}");
                ddlEmpPosition.Items.Add(new ListItem("سائق", "سائق"));
                ddlEmpPosition.Items.Add(new ListItem("كاتب جراج", "كاتب جراج"));
                ddlEmpPosition.Items.Add(new ListItem("ملاحظ مركبات", "ملاحظ مركبات"));
                ddlEmpPosition.Items.Add(new ListItem("مدير تشغيل", "مدير تشغيل"));
            }
        }

        private void LoadSearchPositions()
        {
            try
            {
                ddlSearchPosition.Items.Clear();
                ddlSearchPosition.Items.Add(new ListItem("-- الكل --", ""));

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT position_name FROM employee_positions WHERE is_active = 1 ORDER BY position_name";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string positionName = reader["position_name"].ToString();
                        ddlSearchPosition.Items.Add(new ListItem(positionName, positionName));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading search positions: {ex.Message}");
                ddlSearchPosition.Items.Add(new ListItem("سائق", "سائق"));
                ddlSearchPosition.Items.Add(new ListItem("كاتب جراج", "كاتب جراج"));
                ddlSearchPosition.Items.Add(new ListItem("ملاحظ مركبات", "ملاحظ مركبات"));
                ddlSearchPosition.Items.Add(new ListItem("مدير تشغيل", "مدير تشغيل"));
            }
        }

        private void LoadSearchType()
        {
            try
            {
                ddlSearchType.Items.Clear();
                ddlSearchType.Items.Add(new ListItem("الكل (موظفين ومشتركين)", "all"));
                ddlSearchType.Items.Add(new ListItem("موظفين فقط", "employees"));
                ddlSearchType.Items.Add(new ListItem("مشتركين فقط", "subscribers"));
                ddlSearchType.Items.Add(new ListItem("سائقين مشتركين فقط", "drivers_subscribers"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading search type: {ex.Message}");
            }
        }

        private void LoadStatistics()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            (SELECT COUNT(*) FROM employees WHERE is_active = 1) as total_employees,
                            (SELECT COUNT(*) FROM subscription_details) as total_subscribers,
                            (SELECT COUNT(*) FROM subscription_details 
                             WHERE end_date IS NOT NULL AND end_date >= GETDATE()) as active_subscriptions";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTotalEmployees.Text = reader["total_employees"].ToString();
                        lblTotalSubscribers.Text = reader["total_subscribers"].ToString();
                        lblActiveSubscriptions.Text = reader["active_subscriptions"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }

        #endregion

        #region Employee Search Methods

        private void SearchEmployeeByName(string searchText)
        {
            try
            {
                DataTable allEmployees = ViewState["AllEmployees"] as DataTable;

                if (allEmployees != null)
                {
                    DataRow[] results = allEmployees.Select($"employee_name LIKE '%{searchText.Replace("'", "''")}%'");
                    DataTable filteredTable = allEmployees.Clone();

                    if (!filteredTable.Columns.Contains("RowNum"))
                    {
                        filteredTable.Columns.Add("RowNum", typeof(int));
                    }

                    int rowNum = 1;
                    foreach (DataRow row in results)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        foreach (DataColumn col in filteredTable.Columns)
                        {
                            if (col.ColumnName == "RowNum")
                            {
                                newRow["RowNum"] = rowNum++;
                            }
                            else if (allEmployees.Columns.Contains(col.ColumnName))
                            {
                                newRow[col.ColumnName] = row[col.ColumnName];
                            }
                        }
                        filteredTable.Rows.Add(newRow);
                    }

                    gvEmployeeSearchResults.DataSource = filteredTable;
                    gvEmployeeSearchResults.DataBind();
                    employeeSearchResults.Visible = filteredTable.Rows.Count > 0;

                    if (filteredTable.Rows.Count == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            "alert('لا توجد نتائج للبحث عن الموظفين');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching employees: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "searchError",
                    $"alert('خطأ في البحث: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void gvEmployeeSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectEmployee")
            {
                int employeeId = Convert.ToInt32(e.CommandArgument);
                SelectEmployee(employeeId);
            }
        }

        private void SelectEmployee(int employeeId)
        {
            try
            {
                ddlEmployee.SelectedValue = employeeId.ToString();
                LoadEmployeeInfo(employeeId);
                employeeSearchResults.Visible = false;
                txtEmployeeSearch.Text = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting employee: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "selectError",
                    $"alert('خطأ في اختيار الموظف: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        #endregion

        #region Employee Methods

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                int editingEmployeeId = string.IsNullOrEmpty(hdnEmployeeId.Value) ? 0 : Convert.ToInt32(hdnEmployeeId.Value);

                if (!ValidateEmployeeFormServer(editingEmployeeId))
                    return;

                string employeePosition = ddlEmpPosition.SelectedValue;

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                if (editingEmployeeId > 0)
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = @"
                    UPDATE employees 
                    SET employee_name = @name,
                        employee_position = @position,
                        email = @email,
                        phone_number = @phone,
                        national_id = @nationalId
                    WHERE employee_id = @employeeId";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@name", txtEmpName.Text.Trim());
                        cmd.Parameters.AddWithValue("@position", employeePosition);
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(txtEmpEmail.Text.Trim()) ? (object)DBNull.Value : txtEmpEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(txtEmpPhone.Text.Trim()) ? (object)DBNull.Value : txtEmpPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@nationalId", txtEmpNationalId.Text.Trim());
                        cmd.Parameters.AddWithValue("@employeeId", editingEmployeeId);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccess.Text = "تم تحديث بيانات الموظف بنجاح";
                        lblSuccess.Visible = true;
                        lblMessage.Visible = false;
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = @"
                    INSERT INTO employees 
                        (employee_name, employee_position, email, phone_number, national_id, is_active, created_date, created_by)
                    VALUES 
                        (@name, @position, @email, @phone, @nationalId, 1, GETDATE(), @createdBy)";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@name", txtEmpName.Text.Trim());
                        cmd.Parameters.AddWithValue("@position", employeePosition);
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(txtEmpEmail.Text.Trim()) ? (object)DBNull.Value : txtEmpEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(txtEmpPhone.Text.Trim()) ? (object)DBNull.Value : txtEmpPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@nationalId", txtEmpNationalId.Text.Trim());
                        cmd.Parameters.AddWithValue("@createdBy", "System");

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        lblSuccess.Text = "تم حفظ الموظف بنجاح";
                        lblSuccess.Visible = true;
                        lblMessage.Visible = false;
                    }
                }

                hdnEmployeeId.Value = "0";
                btnSaveEmployee.Text = "حفظ الموظف";

                ClearEmployeeForm();
                LoadEmployees();
                LoadAllEmployeesForSearch();
                LoadEmployeesForGridView();
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"خطأ في حفظ الموظف: {ex.Message}";
                lblMessage.Visible = true;
                lblSuccess.Visible = false;
            }
        }

        protected void btnClearEmployee_Click(object sender, EventArgs e)
        {
            ClearEmployeeForm();
            hdnEmployeeId.Value = "0";
            btnSaveEmployee.Text = "حفظ الموظف";
        }

        private void ClearEmployeeForm()
        {
            txtEmpName.Text = "";
            ddlEmpPosition.SelectedIndex = 0;
            txtEmpEmail.Text = "";
            txtEmpPhone.Text = "";
            txtEmpNationalId.Text = "";
        }

        private bool ValidateEmployeeFormServer(int excludeEmployeeId = 0)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(txtEmpName.Text.Trim()))
                errorMessage += "• الرجاء إدخال اسم الموظف\n";

            if (ddlEmpPosition.SelectedValue == "")
                errorMessage += "• الرجاء اختيار الوظيفة\n";

            string nationalId = txtEmpNationalId.Text.Trim();
            if (string.IsNullOrEmpty(nationalId))
            {
                errorMessage += "• الرجاء إدخال الرقم القومي\n";
            }
            else if (!IsValidNationalId(nationalId))
            {
                errorMessage += "• الرقم القومي يجب أن يتكون من 14 رقم ويبدأ بـ 2 أو 3\n";
            }
            else if (!IsNationalIdUnique(nationalId, excludeEmployeeId))
            {
                errorMessage += "• هذا الرقم القومي موجود بالفعل في النظام\n";
            }

            string email = txtEmpEmail.Text.Trim();
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                errorMessage += "• البريد الإلكتروني غير صحيح\n";
            }

            string phone = txtEmpPhone.Text.Trim();
            if (!string.IsNullOrEmpty(phone) && !IsValidPhone(phone))
            {
                errorMessage += "• رقم الهاتف يجب أن يتكون من 11 رقم ويبدأ بـ 010, 011, 012, أو 015\n";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                lblMessage.Text = errorMessage.Replace("\n", "<br/>");
                lblMessage.Visible = true;
                lblSuccess.Visible = false;
                return false;
            }

            return true;
        }

        private void LoadEmployeeForEditing(int employeeId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT employee_name, employee_position, email, phone_number, national_id
                FROM employees 
                WHERE employee_id = @employeeId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtEmpName.Text = reader["employee_name"].ToString();
                        string position = reader["employee_position"].ToString();

                        foreach (ListItem item in ddlEmpPosition.Items)
                        {
                            if (item.Value == position)
                            {
                                ddlEmpPosition.SelectedValue = position;
                                break;
                            }
                        }

                        txtEmpEmail.Text = reader["email"].ToString();
                        txtEmpPhone.Text = reader["phone_number"].ToString();
                        txtEmpNationalId.Text = reader["national_id"].ToString();

                        hdnEmployeeId.Value = employeeId.ToString();
                        btnSaveEmployee.Text = "تحديث الموظف";
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee for editing: {ex.Message}");
            }
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditEmployee")
            {
                int employeeId = Convert.ToInt32(e.CommandArgument);
                LoadEmployeeForEditing(employeeId);

                ScriptManager.RegisterStartupScript(this, GetType(), "scrollToForm",
                    "document.getElementById('driverFormSection').scrollIntoView({behavior: 'smooth'});", true);
            }
        }

        #endregion

        #region Subscriber Methods

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);
            if (employeeId > 0)
            {
                LoadEmployeeInfo(employeeId);
            }
            else
            {
                employeeInfo.Visible = false;
            }
        }

        private void LoadEmployeeInfo(int employeeId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT employee_name, employee_position, phone_number
                        FROM employees 
                        WHERE employee_id = @employeeId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblSelectedEmployeeName.Text = reader["employee_name"].ToString();
                        lblSelectedEmployeePosition.Text = reader["employee_position"].ToString();
                        lblSelectedEmployeePhone.Text = reader["phone_number"].ToString();

                        txtSubPosition.Text = reader["employee_position"].ToString();

                        employeeInfo.Visible = true;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee info: {ex.Message}");
            }
        }

        protected void btnSaveSubscription_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateSubscriptionFormServer())
                    return;

                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);
                int routeId = Convert.ToInt32(ddlRoute.SelectedValue);
                DateTime startDate = Convert.ToDateTime(txtSubStartDate.Text);
                DateTime endDate = Convert.ToDateTime(txtSubEndDate.Text);

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string getEmployeeQuery = "SELECT employee_name, phone_number FROM employees WHERE employee_id = @employeeId";
                    SqlCommand getEmployeeCmd = new SqlCommand(getEmployeeQuery, conn);
                    getEmployeeCmd.Parameters.AddWithValue("@employeeId", employeeId);
                    conn.Open();

                    string employeeName = "";
                    string employeePhone = "";
                    SqlDataReader reader = getEmployeeCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        employeeName = reader["employee_name"].ToString();
                        employeePhone = reader["phone_number"].ToString();
                    }
                    reader.Close();

                    string query = @"
                        INSERT INTO subscription_details 
                            (name, phone, address, department, position, 
                             start_date, end_date, route_id, reg_date, sub_count)
                        VALUES 
                            (@name, @phone, @address, @department, @position, 
                             @startDate, @endDate, @routeId, GETDATE(), 1)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", employeeName);
                    cmd.Parameters.AddWithValue("@phone", employeePhone);
                    cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(txtAddress.Text.Trim()) ? (object)DBNull.Value : txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@department", string.IsNullOrEmpty(txtDepartment.Text.Trim()) ? (object)DBNull.Value : txtDepartment.Text.Trim());
                    cmd.Parameters.AddWithValue("@position", string.IsNullOrEmpty(txtSubPosition.Text.Trim()) ? (object)DBNull.Value : txtSubPosition.Text.Trim());
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@routeId", routeId);

                    cmd.ExecuteNonQuery();

                    lblSuccess.Text = "تم حفظ الاشتراك بنجاح";
                    lblSuccess.Visible = true;
                    lblMessage.Visible = false;

                    ClearSubscriptionForm();
                    SearchData();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"خطأ في حفظ الاشتراك: {ex.Message}";
                lblMessage.Visible = true;
                lblSuccess.Visible = false;
            }
        }

        protected void btnClearSubscription_Click(object sender, EventArgs e)
        {
            ClearSubscriptionForm();
        }

        private void ClearSubscriptionForm()
        {
            ddlEmployee.SelectedIndex = 0;
            ddlRoute.SelectedIndex = 0;
            txtSubStartDate.Text = "";
            txtSubEndDate.Text = "";
            txtDepartment.Text = "";
            txtSubPosition.Text = "";
            txtAddress.Text = "";
            txtNotes.Text = "";
            txtEmployeeSearch.Text = "";
            employeeInfo.Visible = false;
            employeeSearchResults.Visible = false;
        }

        private bool ValidateSubscriptionFormServer()
        {
            string errorMessage = "";

            if (ddlEmployee.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار الموظف\n";

            if (ddlRoute.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار خط السير\n";

            if (string.IsNullOrEmpty(txtSubStartDate.Text))
                errorMessage += "• الرجاء إدخال تاريخ البداية\n";

            if (string.IsNullOrEmpty(txtSubEndDate.Text))
                errorMessage += "• الرجاء إدخال تاريخ النهاية\n";

            if (!string.IsNullOrEmpty(txtSubStartDate.Text) && !string.IsNullOrEmpty(txtSubEndDate.Text))
            {
                if (DateTime.TryParse(txtSubStartDate.Text, out DateTime startDate) &&
                    DateTime.TryParse(txtSubEndDate.Text, out DateTime endDate))
                {
                    if (endDate <= startDate)
                    {
                        errorMessage += "• تاريخ النهاية يجب أن يكون بعد تاريخ البداية\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                lblMessage.Text = errorMessage.Replace("\n", "<br/>");
                lblMessage.Visible = true;
                lblSuccess.Visible = false;
                return false;
            }

            return true;
        }

        #endregion

        #region Search Methods

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchEmployee.Text = "";
            ddlSearchPosition.SelectedIndex = 0;
            txtSubFromDate.Text = "";
            txtSubToDate.Text = "";
            ddlSearchType.SelectedIndex = 0;

            SearchData();
        }

        private void SearchData()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            string searchName = txtSearchEmployee.Text.Trim();
            string searchPosition = ddlSearchPosition.SelectedValue;
            string subFromDate = txtSubFromDate.Text;
            string subToDate = txtSubToDate.Text;
            string searchType = ddlSearchType.SelectedValue;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    employeeResults.Visible = false;
                    subscriberResults.Visible = false;

                    // Show statistics when searching using JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "showStats",
                        "if(document.getElementById('statsCardsRow')) document.getElementById('statsCardsRow').style.display = '';", true);

                    // Load statistics with current filter context
                    LoadStatistics();

                    if (searchType != "subscribers")
                    {
                        string employeeQuery = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY employee_name) AS RowNum,
                        employee_id,
                        employee_name,
                        employee_position,
                        ISNULL(phone_number, '') as phone_number,
                        ISNULL(email, '') as email,
                        ISNULL(national_id, '') as national_id,
                        FORMAT(created_date, 'yyyy-MM-dd') as created_date
                    FROM employees 
                    WHERE is_active = 1";

                        if (!string.IsNullOrEmpty(searchName))
                        {
                            employeeQuery += " AND employee_name LIKE @searchName";
                        }

                        if (!string.IsNullOrEmpty(searchPosition))
                        {
                            employeeQuery += " AND employee_position = @searchPosition";
                        }

                        // Special case: Show only drivers who are subscribed
                        if (searchType == "drivers_subscribers" ||
                            (searchPosition == "سائق" && searchType == "subscribers"))
                        {
                            employeeQuery += " AND employee_id IN (SELECT DISTINCT employee_id FROM subscription_details)";
                        }

                        employeeQuery += " ORDER BY employee_name";

                        SqlCommand employeeCmd = new SqlCommand(employeeQuery, conn);

                        if (!string.IsNullOrEmpty(searchName))
                        {
                            employeeCmd.Parameters.AddWithValue("@searchName", "%" + searchName + "%");
                        }

                        if (!string.IsNullOrEmpty(searchPosition))
                        {
                            employeeCmd.Parameters.AddWithValue("@searchPosition", searchPosition);
                        }

                        SqlDataAdapter employeeDa = new SqlDataAdapter(employeeCmd);
                        DataTable employeeDt = new DataTable();
                        employeeDa.Fill(employeeDt);

                        gvEmployees.DataSource = employeeDt;
                        gvEmployees.DataBind();

                        // Hide the employee_id column (second column)
                        if (gvEmployees.Columns.Count > 1)
                        {
                            gvEmployees.Columns[1].Visible = false;
                        }

                        employeeResults.Visible = employeeDt.Rows.Count > 0;
                    }

                    if (searchType != "employees")
                    {
                        string subscriberQuery = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY sd.start_date DESC) AS RowNum,
                        sd.n_id as subscriber_id,
                        sd.name,
                        ISNULL(sd.position, '') as position,
                        ISNULL(sd.phone, '') as phone,
                        ISNULL(r.route_name, '') as route_name,
                        sd.start_date,
                        sd.end_date,
                        ISNULL(sd.department, '') as department,
                        ISNULL(sd.address, '') as address
                    FROM subscription_details sd
                    LEFT JOIN routes r ON sd.route_id = r.route_id
                    WHERE 1=1";

                        if (!string.IsNullOrEmpty(searchName))
                        {
                            subscriberQuery += " AND sd.name LIKE @searchName";
                        }

                        if (!string.IsNullOrEmpty(subFromDate))
                        {
                            subscriberQuery += " AND sd.start_date >= @fromDate";
                        }

                        if (!string.IsNullOrEmpty(subToDate))
                        {
                            subscriberQuery += " AND sd.end_date <= @toDate";
                        }

                        // If searching for specific position, filter subscribers
                        if (!string.IsNullOrEmpty(searchPosition) && searchType != "all")
                        {
                            subscriberQuery += " AND sd.position = @position";
                        }

                        subscriberQuery += " ORDER BY sd.start_date DESC";

                        SqlCommand subscriberCmd = new SqlCommand(subscriberQuery, conn);

                        if (!string.IsNullOrEmpty(searchName))
                        {
                            subscriberCmd.Parameters.AddWithValue("@searchName", "%" + searchName + "%");
                        }

                        if (!string.IsNullOrEmpty(subFromDate))
                        {
                            subscriberCmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(subFromDate));
                        }

                        if (!string.IsNullOrEmpty(subToDate))
                        {
                            subscriberCmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(subToDate));
                        }

                        if (!string.IsNullOrEmpty(searchPosition) && searchType != "all")
                        {
                            subscriberCmd.Parameters.AddWithValue("@position", searchPosition);
                        }

                        SqlDataAdapter subscriberDa = new SqlDataAdapter(subscriberCmd);
                        DataTable subscriberDt = new DataTable();
                        subscriberDa.Fill(subscriberDt);

                        gvSubscribers.DataSource = subscriberDt;
                        gvSubscribers.DataBind();
                        subscriberResults.Visible = subscriberDt.Rows.Count > 0;
                    }

                    bool hasEmployees = (searchType != "subscribers") && employeeResults.Visible;
                    bool hasSubscribers = (searchType != "employees") && subscriberResults.Visible;

                    if (!hasEmployees && !hasSubscribers)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            "alert('لا توجد نتائج للبحث');", true);
                        // Hide statistics if no results
                        ScriptManager.RegisterStartupScript(this, GetType(), "hideStats",
                            "if(document.getElementById('statsCardsRow')) document.getElementById('statsCardsRow').style.display = 'none';", true);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                    lblMessage.Text = $"خطأ في البحث: {ex.Message}";
                    lblMessage.Visible = true;
                }
            }
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            SearchData();
        }

        protected void gvSubscribers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscribers.PageIndex = e.NewPageIndex;
            SearchData();
        }

        #endregion

        protected void Page_Render(object sender, EventArgs e)
        {
            string script = @"
            function searchEmployee() {
                var searchText = document.getElementById('" + txtEmployeeSearch.ClientID + @"').value;
                if (searchText.trim() === '') {
                    alert('الرجاء إدخال اسم الموظف للبحث');
                    return false;
                }
                __doPostBack('SearchEmployee', searchText);
                return false;
            }";
            ClientScript.RegisterStartupScript(this.GetType(), "searchEmployeeScript", script, true);
        }

        protected void btnRefreshEmployees_Click(object sender, EventArgs e)
        {
            LoadEmployees();
            LoadAllEmployeesForSearch();
            LoadEmployeesForGridView();
            lblSuccess.Text = "تم تحديث قائمة الموظفين بنجاح";
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }
    }
}