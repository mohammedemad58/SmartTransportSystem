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
        // ===== Capacity thresholds for subscriber-count alerts =====
        private static readonly int[] CapacityThresholds = new int[] { 4, 8, 14, 26 };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEmployees();
                LoadRoutes();
                LoadEmployeePositionsDropdown();
                LoadSearchPositions();
                LoadSearchRoutes();

                LoadEmpPositionFilter();
                LoadEmployeesForPicker();

                ScriptManager.RegisterStartupScript(this, GetType(), "hideStats",
                    "if(document.getElementById('statsCardsRow')) document.getElementById('statsCardsRow').style.display = 'none';", true);
            }
        }

        #region Helper Methods
        protected string GetSubStatusClass(object status)
        {
            switch (Convert.ToString(status))
            {
                case "نشط": return "badge-subscribed";
                case "منتهي": return "badge-expired";
                default: return "badge-secondary";
            }
        }

        protected string GetStatus(object endDateObj)
        {
            if (endDateObj == DBNull.Value || endDateObj == null) return "غير محدد";
            DateTime endDate = Convert.ToDateTime(endDateObj);
            return endDate >= DateTime.Today ? "نشط" : "منتهي";
        }

        protected string GetStatusClass(object endDateObj)
        {
            if (endDateObj == DBNull.Value || endDateObj == null) return "badge-secondary";
            DateTime endDate = Convert.ToDateTime(endDateObj);
            return endDate >= DateTime.Today ? "badge-subscribed" : "badge-expired";
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
            catch { return false; }
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
                        SELECT employee_id, employee_name
                        FROM employees 
                        WHERE is_active = 1
                        ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employees: {ex.Message}");
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
                            route_name + ' - ' + start_loc + ' - ' + end_loc AS route_name
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
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading routes: {ex.Message}");
            }
        }

        private void LoadSearchRoutes()
        {
            try
            {
                ddlSearchRoute.Items.Clear();
                ddlSearchRoute.Items.Add(new ListItem("-- الكل --", "0"));

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            route_id,
                            route_name + ' - ' + start_loc + ' - ' + end_loc AS route_name
                        FROM routes
                        WHERE is_active = 1
                        ORDER BY route_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlSearchRoute.Items.Add(new ListItem(
                                reader["route_name"].ToString(),
                                reader["route_id"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading search routes: {ex.Message}");
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

        private void LoadEmpPositionFilter()
        {
            try
            {
                ddlEmpPositionFilter.Items.Clear();
                ddlEmpPositionFilter.Items.Add(new ListItem("كل الوظائف", ""));

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT position_name FROM employee_positions WHERE is_active = 1 ORDER BY position_name";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string positionName = reader["position_name"].ToString();
                            ddlEmpPositionFilter.Items.Add(new ListItem(positionName, positionName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading position filter: {ex.Message}");
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

        #region Employee Methods

        private const string DriverPosition = "سائق";

        private void EnsureDriverRow(SqlConnection conn, SqlTransaction tx, int employeeId)
        {
            string sql = @"
                IF EXISTS (SELECT 1 FROM drivers WHERE employee_id = @empId)
                    UPDATE drivers SET is_active = 1 WHERE employee_id = @empId AND is_active = 0;
                ELSE
                    INSERT INTO drivers (employee_id, is_active, created_date)
                    VALUES (@empId, 1, GETDATE());";

            SqlCommand cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@empId", employeeId);
            cmd.ExecuteNonQuery();
        }

        private void DeactivateDriverRow(SqlConnection conn, SqlTransaction tx, int employeeId)
        {
            SqlCommand cmd = new SqlCommand("UPDATE drivers SET is_active = 0 WHERE employee_id = @empId", conn, tx);
            cmd.Parameters.AddWithValue("@empId", employeeId);
            cmd.ExecuteNonQuery();
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                int editingEmployeeId = string.IsNullOrEmpty(hdnEmployeeId.Value) ? 0 : Convert.ToInt32(hdnEmployeeId.Value);

                if (!ValidateEmployeeFormServer(editingEmployeeId))
                    return;

                string employeePosition = ddlEmpPosition.SelectedValue;
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            int employeeId;

                            if (editingEmployeeId > 0)
                            {
                                string query = @"
                                    UPDATE employees 
                                    SET employee_name = @name,
                                        employee_position = @position,
                                        email = @email,
                                        phone_number = @phone,
                                        national_id = @nationalId,
                                        address = @address
                                    WHERE employee_id = @employeeId";

                                SqlCommand cmd = new SqlCommand(query, conn, tx);
                                cmd.Parameters.AddWithValue("@name", txtEmpName.Text.Trim());
                                cmd.Parameters.AddWithValue("@position", employeePosition);
                                cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(txtEmpEmail.Text.Trim()) ? (object)DBNull.Value : txtEmpEmail.Text.Trim());
                                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(txtEmpPhone.Text.Trim()) ? (object)DBNull.Value : txtEmpPhone.Text.Trim());
                                cmd.Parameters.AddWithValue("@nationalId", txtEmpNationalId.Text.Trim());
                                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(txtEmpAddress.Text.Trim()) ? (object)DBNull.Value : txtEmpAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("@employeeId", editingEmployeeId);
                                cmd.ExecuteNonQuery();

                                employeeId = editingEmployeeId;
                                lblSuccess.Text = "تم تحديث بيانات الموظف بنجاح";
                            }
                            else
                            {
                                string query = @"
                                    INSERT INTO employees 
                                        (employee_name, employee_position, email, phone_number, national_id, address, is_active, created_date, created_by)
                                    VALUES 
                                        (@name, @position, @email, @phone, @nationalId, @address, 1, GETDATE(), @createdBy);
                                    SELECT SCOPE_IDENTITY();";

                                SqlCommand cmd = new SqlCommand(query, conn, tx);
                                cmd.Parameters.AddWithValue("@name", txtEmpName.Text.Trim());
                                cmd.Parameters.AddWithValue("@position", employeePosition);
                                cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(txtEmpEmail.Text.Trim()) ? (object)DBNull.Value : txtEmpEmail.Text.Trim());
                                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(txtEmpPhone.Text.Trim()) ? (object)DBNull.Value : txtEmpPhone.Text.Trim());
                                cmd.Parameters.AddWithValue("@nationalId", txtEmpNationalId.Text.Trim());
                                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(txtEmpAddress.Text.Trim()) ? (object)DBNull.Value : txtEmpAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("@createdBy", "System");

                                employeeId = Convert.ToInt32(cmd.ExecuteScalar());
                                lblSuccess.Text = "تم حفظ الموظف بنجاح";
                            }

                            if (employeePosition == DriverPosition)
                                EnsureDriverRow(conn, tx, employeeId);
                            else if (editingEmployeeId > 0)
                                DeactivateDriverRow(conn, tx, employeeId);

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                lblSuccess.Visible = true;
                lblMessage.Visible = false;

                hdnEmployeeId.Value = "0";
                btnSaveEmployee.Text = "حفظ الموظف";
                lblEmployeeFormTitle.Text = "إضافة موظف جديد";

                ClearEmployeeForm();
                LoadEmployees();
                LoadEmployeesForPicker();
                SearchEmployeesOnly();
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
            lblEmployeeFormTitle.Text = "إضافة موظف جديد";
        }

        private void ClearEmployeeForm()
        {
            txtEmpName.Text = "";
            ddlEmpPosition.SelectedIndex = 0;
            txtEmpEmail.Text = "";
            txtEmpPhone.Text = "";
            txtEmpNationalId.Text = "";
            txtEmpAddress.Text = "";
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
                errorMessage += "• الرجاء إدخال الرقم القومي\n";
            else if (!IsValidNationalId(nationalId))
                errorMessage += "• الرقم القومي يجب أن يتكون من 14 رقم ويبدأ بـ 2 أو 3\n";
            else if (!IsNationalIdUnique(nationalId, excludeEmployeeId))
                errorMessage += "• هذا الرقم القومي موجود بالفعل في النظام\n";

            string email = txtEmpEmail.Text.Trim();
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
                errorMessage += "• البريد الإلكتروني غير صحيح\n";

            string phone = txtEmpPhone.Text.Trim();
            if (!string.IsNullOrEmpty(phone) && !IsValidPhone(phone))
                errorMessage += "• رقم الهاتف يجب أن يتكون من 11 رقم ويبدأ بـ 010, 011, 012, أو 015\n";

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
                        SELECT employee_name, employee_position, email, phone_number, national_id, ISNULL(address,'') AS address
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
                        txtEmpAddress.Text = reader["address"].ToString();

                        hdnEmployeeId.Value = employeeId.ToString();
                        btnSaveEmployee.Text = "تحديث الموظف";
                        lblEmployeeFormTitle.Text = "تعديل بيانات الموظف";

                        hfActiveTab.Value = "tabEmployees";
                        ScriptManager.RegisterStartupScript(this, GetType(), "switchToEmp",
                            "activateTab('tabEmployees');" +
                            "document.getElementById('tabEmployees').scrollIntoView({behavior: 'smooth'});", true);
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
            }
        }

        #endregion

        #region Employee Picker (for subscribers tab)

        private void LoadEmployeesForPicker()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                string positionFilter = ddlEmpPositionFilter?.SelectedValue ?? "";
                string search = (txtEmpPickerSearch?.Text ?? "").Trim();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT employee_id,
                               employee_name,
                               employee_position,
                               ISNULL(phone_number,'') AS phone_number
                        FROM employees
                        WHERE is_active = 1";

                    if (!string.IsNullOrEmpty(positionFilter))
                        query += " AND employee_position = @position";
                    if (!string.IsNullOrEmpty(search))
                        query += " AND employee_name LIKE @search";

                    query += " ORDER BY employee_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(positionFilter))
                        cmd.Parameters.AddWithValue("@position", positionFilter);
                    if (!string.IsNullOrEmpty(search))
                        cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    var previouslySelected = new HashSet<string>(
                        cblEmployees.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));

                    cblEmployees.Items.Clear();
                    foreach (DataRow row in dt.Rows)
                    {
                        string id = row["employee_id"].ToString();
                        string name = row["employee_name"].ToString();
                        string pos = row["employee_position"]?.ToString();
                        string phone = row["phone_number"]?.ToString();
                        string text = $"{name} - {pos}" + (string.IsNullOrEmpty(phone) ? "" : $" - {phone}");

                        var li = new ListItem(text, id);
                        li.Selected = previouslySelected.Contains(id);
                        cblEmployees.Items.Add(li);
                    }

                    UpdateSelectedCountLabel();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employees picker: {ex.Message}");
            }
        }

        protected void btnEmpPickerSearch_Click(object sender, EventArgs e)
        {
            LoadEmployeesForPicker();
            hfActiveTab.Value = "tabSubscribers";
        }

        protected void ddlEmpPositionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeesForPicker();
            hfActiveTab.Value = "tabSubscribers";
        }

        protected void btnSelectAllEmployees_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in cblEmployees.Items) item.Selected = true;
            UpdateSelectedCountLabel();
            hfActiveTab.Value = "tabSubscribers";
        }

        protected void btnClearSelectedEmployees_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in cblEmployees.Items) item.Selected = false;
            UpdateSelectedCountLabel();
            hfActiveTab.Value = "tabSubscribers";
        }

        private void UpdateSelectedCountLabel()
        {
            int count = cblEmployees.Items.Cast<ListItem>().Count(i => i.Selected);
            if (lblSelectedCount != null) lblSelectedCount.Text = $"عدد المحددين: {count}";
        }

        #endregion

        #region Subscriber Methods

        private void LoadSubscriptionForEditing(int subscriptionId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            n_id, name, phone, address, department, position,
                            start_date, end_date, route_id, employee_id
                        FROM subscription_details
                        WHERE n_id = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", subscriptionId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        LoadEmployeesForPicker();

                        int employeeId = reader["employee_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["employee_id"]);

                        foreach (ListItem it in cblEmployees.Items)
                        {
                            it.Selected = (employeeId > 0 && it.Value == employeeId.ToString());
                        }
                        UpdateSelectedCountLabel();

                        if (reader["start_date"] != DBNull.Value)
                            txtSubStartDate.Text = Convert.ToDateTime(reader["start_date"]).ToString("yyyy-MM-dd");
                        if (reader["end_date"] != DBNull.Value)
                            txtSubEndDate.Text = Convert.ToDateTime(reader["end_date"]).ToString("yyyy-MM-dd");

                        string routeId = reader["route_id"].ToString();
                        ListItem routeItem = ddlRoute.Items.FindByValue(routeId);
                        if (routeItem != null) ddlRoute.SelectedValue = routeId;

                        txtNotes.Text = "";

                        hdnSubscriptionId.Value = subscriptionId.ToString();
                        btnSaveSubscription.Text = "تحديث الاشتراك";
                        lblSubscriptionFormTitle.Text = "تعديل اشتراك";

                        hfActiveTab.Value = "tabSubscribers";
                        ScriptManager.RegisterStartupScript(this, GetType(), "switchToSub",
                            "activateTab('tabSubscribers');" +
                            "document.getElementById('tabSubscribers').scrollIntoView({behavior: 'smooth'});", true);

                        lblSuccess.Visible = false;
                        lblMessage.Visible = false;
                        reader.Close();
                        return;
                    }
                    reader.Close();

                    lblMessage.Text = "لم يتم العثور على الاشتراك المطلوب";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscription for editing: {ex.Message}");
                lblMessage.Text = $"خطأ في تحميل الاشتراك: {ex.Message}";
                lblMessage.Visible = true;
            }
        }

        protected void btnSaveSubscription_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateSubscriptionFormServer()) return;

                int routeId = Convert.ToInt32(ddlRoute.SelectedValue);
                DateTime startDate = Convert.ToDateTime(txtSubStartDate.Text);
                DateTime endDate = Convert.ToDateTime(txtSubEndDate.Text);
                string notes = string.IsNullOrEmpty(txtNotes.Text.Trim()) ? null : txtNotes.Text.Trim();

                int editingSubscriptionId = string.IsNullOrEmpty(hdnSubscriptionId.Value) ? 0 : Convert.ToInt32(hdnSubscriptionId.Value);
                bool isEditingSingle = editingSubscriptionId > 0;

                if (isEditingSingle)
                {
                    string err;
                    SaveOneSubscription(editingSubscriptionId, routeId, startDate, endDate, notes, out err);
                    if (!string.IsNullOrEmpty(err))
                    {
                        lblMessage.Text = err;
                        lblMessage.Visible = true;
                        return;
                    }
                    FinishSubscriptionSave("تم تحديث الاشتراك بنجاح");
                    return;
                }

                // ===== MULTI-EMPLOYEE SAVE =====
                var selectedEmployeeIds = cblEmployees.Items.Cast<ListItem>()
                                            .Where(i => i.Selected)
                                            .Select(i => int.Parse(i.Value))
                                            .ToList();

                if (selectedEmployeeIds.Count == 0)
                {
                    lblMessage.Text = "الرجاء اختيار موظف واحد على الأقل";
                    lblMessage.Visible = true;
                    return;
                }

                bool alreadyConfirmed = hdnCrossedThreshold.Value == "confirmed";

                if (!alreadyConfirmed)
                {
                    // 1. Check for overlaps to show errors before confirming
                    foreach (int empId in selectedEmployeeIds)
                    {
                        string err;
                        if (!CheckOverlapOnly(empId, startDate, endDate, out err))
                        {
                            lblMessage.Text = err;
                            lblMessage.Visible = true;
                            return;
                        }
                    }

                    // 2. Check Capacity
                    int currentRouteCount = GetRouteSubscriberCount(routeId);
                    int totalWillBe = currentRouteCount + selectedEmployeeIds.Count;

                    // --- HARD BLOCK AT 26 ---
                    if (totalWillBe > 26)
                    {
                        lblMessage.Text = $"لا يمكن الحفظ: الخط ({ddlRoute.SelectedItem.Text}) وصل إلى السعة القصوى. " +
                                          $"العدد الحالي: {currentRouteCount}، والمطلوب إضافته: {selectedEmployeeIds.Count}. " +
                                          $"الحد الأقصى المسموح به هو 26 مشترك.";
                        lblMessage.Visible = true;
                        lblSuccess.Visible = false;
                        return; // Blocks save completely
                    }
                    // ------------------------

                    int oldCat = GetCapacityCategory(currentRouteCount);
                    int newCat = GetCapacityCategory(totalWillBe);

                    string oldCatLabel = GetCapacityCategoryLabel(currentRouteCount);
                    string newCatLabel = GetCapacityCategoryLabel(totalWillBe);
                    string routeName = ddlRoute.SelectedItem.Text;

                    if (newCat > oldCat)
                    {
                        // Threshold crossed! Set hidden fields and let JS show the confirm box
                        string msg = $"تنبيه: عدد المشتركين على الخط ({routeName}) سينتقل إلى فئة سعة جديدة.\n\n" +
                                     $"كان العدد: {currentRouteCount} مشترك (فئة: {oldCatLabel}).\n" +
                                     $"سيصبح العدد: {totalWillBe} مشترك (فئة: {newCatLabel}).\n\n" +
                                     $"هل أنت متأكد من رغبتك في المتابعة والحفظ؟";

                        hdnCrossedThreshold.Value = "true";
                        hdnCapacityAlertMsg.Value = msg;
                        return; // Stop here. The JS will catch this and show the confirm box.
                    }
                    else
                    {
                        // No threshold crossed, let it pass
                        hdnCrossedThreshold.Value = "false";
                    }
                }
                else
                {
                    // User already clicked OK on the JS confirm box. Reset the flag and proceed to save.
                    hdnCrossedThreshold.Value = "false";
                }

                // ===== EXECUTE THE SAVE =====
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                int insertedCount = 0;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (int empId in selectedEmployeeIds)
                            {
                                string err;
                                if (!InsertOneSubscription(conn, tx, empId, routeId, startDate, endDate, notes, out err))
                                {
                                    tx.Rollback();
                                    lblMessage.Text = err;
                                    lblMessage.Visible = true;
                                    return;
                                }
                                insertedCount++;
                            }
                            tx.Commit();
                        }
                        catch (Exception ex)
                        {
                            tx.Rollback();
                            throw ex;
                        }
                    }
                }

                FinishSubscriptionSave($"تم حفظ {insertedCount} اشتراك بنجاح");
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"خطأ في حفظ الاشتراك: {ex.Message}";
                lblMessage.Visible = true;
                lblSuccess.Visible = false;
            }
        }

        private int GetCapacityCategory(int count)
        {
            if (count <= 3) return 3;      // 1, 2, 3
            if (count == 4) return 4;      // 4
            if (count <= 8) return 8;      // 5, 6, 7, 8
            if (count <= 14) return 14;    // 9 to 14
            if (count <= 26) return 26;    // 15 to 26
            return -1;                     // 27+ (Blocked)
        }

        private string GetCapacityCategoryLabel(int count)
        {
            int cat = GetCapacityCategory(count);

            switch (cat)
            {
                case 3: return "1 إلى 3";
                case 4: return "4 فقط";
                case 8: return "5 إلى 8";
                case 14: return "9 إلى 14";
                case 26: return "15 إلى 26";
                default: return "تجاوز الحد الأقصى";
            }
        }

        private bool CheckOverlapOnly(int employeeId, DateTime startDate, DateTime endDate, out string error)
        {
            error = null;
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string overlapSql = @"
                    SELECT TOP 1 sd.name, sd.start_date, sd.end_date, r.route_name
                    FROM subscription_details sd
                    LEFT JOIN routes r ON sd.route_id = r.route_id
                    WHERE sd.employee_id = @empId
                      AND sd.start_date <= @endDate 
                      AND sd.end_date >= @startDate
                      AND sd.end_date >= CAST(GETDATE() AS DATE)";

                using (SqlCommand chk = new SqlCommand(overlapSql, conn))
                {
                    chk.Parameters.AddWithValue("@empId", employeeId);
                    chk.Parameters.AddWithValue("@startDate", startDate);
                    chk.Parameters.AddWithValue("@endDate", endDate);

                    using (SqlDataReader rd = chk.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            string empName = rd["name"].ToString();
                            string existingRoute = rd["route_name"] != DBNull.Value ? rd["route_name"].ToString() : "غير محدد";
                            DateTime exStart = Convert.ToDateTime(rd["start_date"]);
                            DateTime exEnd = Convert.ToDateTime(rd["end_date"]);

                            error = $"لا يمكن الحفظ: الموظف ({empName}) لديه اشتراك ساري في هذه الفترة.\n" +
                                    $"• الخط: {existingRoute}\n" +
                                    $"• من: {exStart:yyyy-MM-dd} إلى: {exEnd:yyyy-MM-dd}";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool InsertOneSubscription(SqlConnection conn, SqlTransaction tx, int employeeId, int routeId,
            DateTime startDate, DateTime endDate, string notes, out string error)
        {
            error = null;

            string overlapSql = @"
                SELECT TOP 1 sd.name, sd.start_date, sd.end_date, r.route_name
                FROM subscription_details sd
                LEFT JOIN routes r ON sd.route_id = r.route_id
                WHERE sd.employee_id = @empId
                  AND sd.start_date <= @endDate 
                  AND sd.end_date >= @startDate
                  AND sd.end_date >= CAST(GETDATE() AS DATE)";

            using (SqlCommand chk = new SqlCommand(overlapSql, conn, tx))
            {
                chk.Parameters.AddWithValue("@empId", employeeId);
                chk.Parameters.AddWithValue("@startDate", startDate);
                chk.Parameters.AddWithValue("@endDate", endDate);

                using (SqlDataReader rd = chk.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        string empName = rd["name"].ToString();
                        string existingRoute = rd["route_name"] != DBNull.Value ? rd["route_name"].ToString() : "غير محدد";
                        DateTime exStart = Convert.ToDateTime(rd["start_date"]);
                        DateTime exEnd = Convert.ToDateTime(rd["end_date"]);

                        error = $"لا يمكن الحفظ: الموظف ({empName}) لديه اشتراك ساري في هذه الفترة.\n" +
                                $"• الخط: {existingRoute}\n" +
                                $"• من: {exStart:yyyy-MM-dd} إلى: {exEnd:yyyy-MM-dd}";

                        return false;
                    }
                }
            }

            string name = "", phone = "", position = "", address = "";
            using (SqlCommand getEmp = new SqlCommand(@"
                    SELECT employee_name, ISNULL(phone_number,'') AS phone,
                           ISNULL(employee_position,'') AS position,
                           ISNULL(address,'') AS address
                    FROM employees WHERE employee_id = @empId", conn, tx))
            {
                getEmp.Parameters.AddWithValue("@empId", employeeId);
                using (SqlDataReader rd = getEmp.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        name = rd["employee_name"].ToString();
                        phone = rd["phone"].ToString();
                        position = rd["position"].ToString();
                        address = rd["address"].ToString();
                    }
                    else
                    {
                        error = $"لم يتم العثور على الموظف صاحب المعرّف {employeeId}";
                        return false;
                    }
                }
            }

            string insertSql = @"
                INSERT INTO subscription_details
                    (name, phone, address, department, position, start_date, end_date, route_id,
                     reg_date, sub_count, employee_id)
                VALUES
                    (@name, @phone, @address, @department, @position, @startDate, @endDate, @routeId,
                     GETDATE(), 1, @employeeId)";

            using (SqlCommand ins = new SqlCommand(insertSql, conn, tx))
            {
                ins.Parameters.AddWithValue("@name", name);
                ins.Parameters.AddWithValue("@phone", phone);
                ins.Parameters.AddWithValue("@address", string.IsNullOrEmpty(address) ? (object)DBNull.Value : address);
                ins.Parameters.AddWithValue("@department", (object)DBNull.Value);
                ins.Parameters.AddWithValue("@position", string.IsNullOrEmpty(position) ? (object)DBNull.Value : position);
                ins.Parameters.AddWithValue("@startDate", startDate);
                ins.Parameters.AddWithValue("@endDate", endDate);
                ins.Parameters.AddWithValue("@routeId", routeId);
                ins.Parameters.AddWithValue("@employeeId", employeeId);
                ins.ExecuteNonQuery();
            }
            return true;
        }

        private void SaveOneSubscription(int subscriptionId, int routeId, DateTime startDate, DateTime endDate,
            string notes, out string error)
        {
            error = null;

            var selected = cblEmployees.Items.Cast<ListItem>().Where(i => i.Selected).ToList();
            if (selected.Count == 0) { error = "الرجاء اختيار موظف للتعديل"; return; }
            if (selected.Count > 1) { error = "يمكنك تعديل اشتراك موظف واحد فقط في المرة"; return; }

            int employeeId = int.Parse(selected[0].Value);

            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string name = "", phone = "", position = "", address = "";
                using (SqlCommand getEmp = new SqlCommand(@"
                        SELECT employee_name, ISNULL(phone_number,'') AS phone,
                               ISNULL(employee_position,'') AS position,
                               ISNULL(address,'') AS address
                        FROM employees WHERE employee_id = @empId", conn))
                {
                    getEmp.Parameters.AddWithValue("@empId", employeeId);
                    using (SqlDataReader rd = getEmp.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            name = rd["employee_name"].ToString();
                            phone = rd["phone"].ToString();
                            position = rd["position"].ToString();
                            address = rd["address"].ToString();
                        }
                    }
                }

                string query = @"
                    UPDATE subscription_details
                    SET name = @name, phone = @phone, address = @address,
                        department = @department, position = @position,
                        start_date = @startDate, end_date = @endDate,
                        route_id = @routeId, employee_id = @employeeId
                    WHERE n_id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(address) ? (object)DBNull.Value : address);
                    cmd.Parameters.AddWithValue("@department", (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@position", string.IsNullOrEmpty(position) ? (object)DBNull.Value : position);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@routeId", routeId);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@id", subscriptionId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void FinishSubscriptionSave(string successMessage)
        {
            lblSuccess.Text = successMessage;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;

            hdnSubscriptionId.Value = "0";
            btnSaveSubscription.Text = "حفظ الاشتراك";
            lblSubscriptionFormTitle.Text = "إضافة اشتراك جديد";

            ClearSubscriptionForm();
            SearchSubscribersOnly();
            hfActiveTab.Value = "tabSubscribers";
        }

        protected void btnClearSubscription_Click(object sender, EventArgs e)
        {
            ClearSubscriptionForm();
        }

        private void ClearSubscriptionForm()
        {
            ddlRoute.SelectedIndex = 0;
            foreach (ListItem it in cblEmployees.Items) it.Selected = false;
            txtSubStartDate.Text = "";
            txtSubEndDate.Text = "";
            txtNotes.Text = "";

            hdnSubscriptionId.Value = "0";
            btnSaveSubscription.Text = "حفظ الاشتراك";
            lblSubscriptionFormTitle.Text = "إضافة اشتراك جديد";

            UpdateSelectedCountLabel();
        }

        private bool ValidateSubscriptionFormServer()
        {
            string errorMessage = "";

            if (ddlRoute.SelectedValue == "0") errorMessage += "• الرجاء اختيار خط السير\n";
            if (string.IsNullOrEmpty(txtSubStartDate.Text)) errorMessage += "• الرجاء إدخال تاريخ البداية\n";
            if (string.IsNullOrEmpty(txtSubEndDate.Text)) errorMessage += "• الرجاء إدخال تاريخ النهاية\n";

            if (!string.IsNullOrEmpty(txtSubStartDate.Text) && !string.IsNullOrEmpty(txtSubEndDate.Text))
            {
                if (DateTime.TryParse(txtSubStartDate.Text, out DateTime s) && DateTime.TryParse(txtSubEndDate.Text, out DateTime en))
                {
                    if (en <= s) errorMessage += "• تاريخ النهاية يجب أن يكون بعد تاريخ البداية\n";
                }
            }

            int editingId = string.IsNullOrEmpty(hdnSubscriptionId.Value) ? 0 : Convert.ToInt32(hdnSubscriptionId.Value);
            if (editingId == 0)
            {
                int count = cblEmployees.Items.Cast<ListItem>().Count(i => i.Selected);
                if (count == 0) errorMessage += "• الرجاء اختيار موظف واحد على الأقل\n";
            }
            else
            {
                int count = cblEmployees.Items.Cast<ListItem>().Count(i => i.Selected);
                if (count != 1) errorMessage += "• في وضع التعديل يجب تحديد موظف واحد فقط\n";
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

        private int GetRouteSubscriberCount(int routeId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM subscription_details WHERE route_id = @rid AND ISNULL(hide,0) = 0 AND (end_date IS NULL OR end_date >= CAST(GETDATE() AS DATE))", conn))
                    {
                        cmd.Parameters.AddWithValue("@rid", routeId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch { return 0; }
        }

        #endregion

        #region Search Methods (per tab)

        protected void btnSearchEmployees_Click(object sender, EventArgs e) => SearchEmployeesOnly();

        protected void btnResetEmployees_Click(object sender, EventArgs e)
        {
            txtSearchEmployee.Text = "";
            ddlSearchPosition.SelectedIndex = 0;
            SearchEmployeesOnly();
        }

        protected void btnSearchSubscribers_Click(object sender, EventArgs e) => SearchSubscribersOnly();

        protected void btnResetSubscribers_Click(object sender, EventArgs e)
        {
            txtSearchSubscriber.Text = "";
            ddlSearchRoute.SelectedIndex = 0;
            ddlSearchStatus.SelectedIndex = 0;
            txtSubFromDate.Text = "";
            txtSubToDate.Text = "";
            SearchSubscribersOnly();
        }

        private void SearchEmployeesOnly()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            string searchName = txtSearchEmployee.Text.Trim();
            string searchPosition = ddlSearchPosition.SelectedValue;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    employeeResults.Visible = false;

                    ScriptManager.RegisterStartupScript(this, GetType(), "showStats",
                        "if(document.getElementById('statsCardsRow')) document.getElementById('statsCardsRow').style.display = '';", true);

                    LoadStatistics();

                    string employeeQuery = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY employee_name) AS RowNum,
                            employee_id,
                            employee_name,
                            employee_position,
                            ISNULL(phone_number, '') as phone_number,
                            ISNULL(email, '') as email,
                            ISNULL(national_id, '') as national_id,
                            ISNULL(address, '') as address
                        FROM employees 
                        WHERE is_active = 1";

                    if (!string.IsNullOrEmpty(searchName))
                        employeeQuery += " AND employee_name LIKE @searchName";
                    if (!string.IsNullOrEmpty(searchPosition))
                        employeeQuery += " AND employee_position = @searchPosition";

                    employeeQuery += " ORDER BY employee_name";

                    SqlCommand employeeCmd = new SqlCommand(employeeQuery, conn);
                    if (!string.IsNullOrEmpty(searchName))
                        employeeCmd.Parameters.AddWithValue("@searchName", "%" + searchName + "%");
                    if (!string.IsNullOrEmpty(searchPosition))
                        employeeCmd.Parameters.AddWithValue("@searchPosition", searchPosition);

                    SqlDataAdapter da = new SqlDataAdapter(employeeCmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvEmployees.DataSource = dt;
                    gvEmployees.DataBind();
                    employeeResults.Visible = dt.Rows.Count > 0;

                    if (dt.Rows.Count == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            "alert('لا توجد نتائج للبحث');", true);
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

        private void SearchSubscribersOnly()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            string searchName = txtSearchSubscriber.Text.Trim();
            string searchRoute = ddlSearchRoute.SelectedValue;
            string searchStatus = ddlSearchStatus.SelectedValue;
            string subFromDate = txtSubFromDate.Text;
            string subToDate = txtSubToDate.Text;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    subscriberResults.Visible = false;

                    ScriptManager.RegisterStartupScript(this, GetType(), "showStats",
                        "if(document.getElementById('statsCardsRow')) document.getElementById('statsCardsRow').style.display = '';", true);

                    LoadStatistics();

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
                            ISNULL(sd.address, '') as address,
                            CASE 
                                WHEN ISNULL(sd.hide, 0) = 1                    THEN N'مخفي'
                                WHEN sd.end_date IS NULL                       THEN N'غير محدد'
                                WHEN sd.end_date >= CAST(GETDATE() AS date)    THEN N'نشط'
                                ELSE N'منتهي'
                            END AS sub_status
                        FROM subscription_details sd
                        LEFT JOIN routes r ON sd.route_id = r.route_id
                        WHERE 1=1";

                    if (!string.IsNullOrEmpty(searchName))
                        subscriberQuery += " AND sd.name LIKE @searchName";
                    if (!string.IsNullOrEmpty(subFromDate))
                        subscriberQuery += " AND sd.start_date >= @fromDate";
                    if (!string.IsNullOrEmpty(subToDate))
                        subscriberQuery += " AND sd.end_date <= @toDate";
                    if (!string.IsNullOrEmpty(searchRoute) && searchRoute != "0")
                        subscriberQuery += " AND sd.route_id = @routeId";

                    if (searchStatus == "active")
                        subscriberQuery += " AND ISNULL(sd.hide, 0) = 0 AND sd.end_date >= CAST(GETDATE() AS date)";
                    else if (searchStatus == "expired")
                        subscriberQuery += " AND ISNULL(sd.hide, 0) = 0 AND sd.end_date < CAST(GETDATE() AS date)";
                    else if (searchStatus == "hidden")
                        subscriberQuery += " AND sd.hide = 1";

                    subscriberQuery += " ORDER BY sd.start_date DESC";

                    SqlCommand subscriberCmd = new SqlCommand(subscriberQuery, conn);
                    if (!string.IsNullOrEmpty(searchName))
                        subscriberCmd.Parameters.AddWithValue("@searchName", "%" + searchName + "%");
                    if (!string.IsNullOrEmpty(subFromDate))
                        subscriberCmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(subFromDate));
                    if (!string.IsNullOrEmpty(subToDate))
                        subscriberCmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(subToDate));
                    if (!string.IsNullOrEmpty(searchRoute) && searchRoute != "0")
                        subscriberCmd.Parameters.AddWithValue("@routeId", Convert.ToInt32(searchRoute));

                    SqlDataAdapter da = new SqlDataAdapter(subscriberCmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSubscribers.DataSource = dt;
                    gvSubscribers.DataBind();
                    subscriberResults.Visible = dt.Rows.Count > 0;

                    if (dt.Rows.Count == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            "alert('لا توجد نتائج للبحث');", true);
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
            SearchEmployeesOnly();
        }

        protected void gvSubscribers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscribers.PageIndex = e.NewPageIndex;
            SearchSubscribersOnly();
        }

        protected void gvSubscribers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditSubscription")
            {
                int subscriptionId = Convert.ToInt32(e.CommandArgument);
                LoadSubscriptionForEditing(subscriptionId);
            }
        }

        #endregion
    }
}