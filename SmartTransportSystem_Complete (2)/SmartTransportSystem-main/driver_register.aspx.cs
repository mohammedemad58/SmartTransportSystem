using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class driver_register : System.Web.UI.Page
    {
        private string ConnString
        {
            get { return ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin-only page.
            AuthHelper.RequireAdmin(this.Page);

            if (!IsPostBack)
            {
                LoadDriverEmployeesDropdown();
                LoadDriverLogins();
            }
        }

        /// <summary>
        /// Employees whose position is "سائق" (driver) and who do NOT yet
        /// have a login account, so the same driver can't be registered twice.
        /// </summary>
        private void LoadDriverEmployeesDropdown()
        {
            ddlDriverEmployee.Items.Clear();
            ddlDriverEmployee.Items.Add(new ListItem("-- اختر السائق --", ""));

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT e.employee_id, e.employee_name
                                  FROM dbo.employees e
                                  WHERE e.employee_position = N'سائق'
                                    AND e.is_active = 1
                                    AND NOT EXISTS (SELECT 1 FROM dbo.login l WHERE l.employee_id = e.employee_id)
                                  ORDER BY e.employee_name";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlDriverEmployee.Items.Add(new ListItem(
                                reader["employee_name"].ToString(),
                                reader["employee_id"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadDriverLogins()
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT l.user_id, l.user_name, l.created_at,
                                         e.employee_name
                                  FROM dbo.login l
                                  INNER JOIN dbo.employees e ON e.employee_id = l.employee_id
                                  WHERE l.role = N'driver'
                                  ORDER BY l.created_at DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvDriverLogins.DataSource = dt;
                    gvDriverLogins.DataBind();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string employeeIdText = ddlDriverEmployee.SelectedValue;
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(employeeIdText))
            {
                ShowMessage("الرجاء اختيار السائق", false);
                return;
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ShowMessage("الرجاء إدخال اسم المستخدم وكلمة المرور", false);
                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage("كلمة المرور وتأكيدها غير متطابقين", false);
                return;
            }

            int employeeId = Convert.ToInt32(employeeIdText);

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();

                // 1) Username must be unique.
                using (SqlCommand chk = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.login WHERE user_name = @UserName", con))
                {
                    chk.Parameters.AddWithValue("@UserName", userName);
                    int existing = Convert.ToInt32(chk.ExecuteScalar());
                    if (existing > 0)
                    {
                        ShowMessage("اسم المستخدم مستخدم بالفعل، الرجاء اختيار اسم آخر", false);
                        return;
                    }
                }

                // 2) This employee should not already have a login.
                using (SqlCommand chk2 = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.login WHERE employee_id = @EmployeeId", con))
                {
                    chk2.Parameters.AddWithValue("@EmployeeId", employeeId);
                    int existing = Convert.ToInt32(chk2.ExecuteScalar());
                    if (existing > 0)
                    {
                        ShowMessage("هذا السائق لديه حساب مسجل بالفعل", false);
                        return;
                    }
                }

                // 3) Insert the new driver login.
                string insertQuery = @"INSERT INTO dbo.login (user_name, user_password, role, employee_id, created_at)
                                        VALUES (@UserName, @Password, N'driver', @EmployeeId, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.ExecuteNonQuery();
                }
            }

            ShowMessage("تم إنشاء حساب السائق بنجاح", true);
            ClearForm();
            LoadDriverEmployeesDropdown();
            LoadDriverLogins();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ddlDriverEmployee.SelectedIndex = 0;
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
        }

        protected void gvDriverLogins_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ResetPassword")
            {
                int userId = Convert.ToInt32(e.CommandArgument);

                // Simple, predictable reset password (admin should tell the driver
                // to change it after logging in). Kept in Arabic-ASCII form so the
                // driver can type it easily on a mobile keyboard.
                string newPassword = "Driver@" + DateTime.Now.Year;

                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = "UPDATE dbo.login SET user_password = @Password WHERE user_id = @UserId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Password", newPassword);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("تم تعيين كلمة مرور جديدة: " + newPassword, true);
                LoadDriverLogins();
            }
        }

        private void ShowMessage(string message, bool success)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "alert " + (success ? "alert-success" : "alert-danger");
            litMessage.Text = message;
        }
    }
}
