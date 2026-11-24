using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using car_WebApplication1;

namespace cars_WebApplication1
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            pnlMessage.Visible = false;

            if (IsPostBack || !AuthHelper.IsLoggedIn) return;

            // Loop guard: a page keeps sending a logged-in user back here.
            // After 2 bounces, drop the session and show the login form.
            int bounces = Session["LoginBounce"] is int ? (int)Session["LoginBounce"] : 0;
            if (bounces >= 2)
            {
                Session.Clear();
                ShowMessage("انتهت الجلسة أو لا تملك صلاحية لهذه الصفحة، الرجاء تسجيل الدخول من جديد");
                return;
            }

            Session["LoginBounce"] = bounces + 1;
            Response.Redirect(AuthHelper.GetHomePageForRole(AuthHelper.CurrentRole), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string username = inputEmail.Text.Trim();
            string password = inputPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("الرجاء إدخال اسم المستخدم وكلمة المرور");
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            string targetRole = null;

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = @"SELECT user_id, role, employee_id
                                     FROM [login]
                                     WHERE user_name = @Username AND user_password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader["role"] == DBNull.Value
                                    ? AuthHelper.RoleAdmin
                                    : reader["role"].ToString().Trim();
                                object employeeIdObj = reader["employee_id"];

                                Session.Clear();   // also resets LoginBounce
                                Session["UserName"] = username;
                                Session["role"] = role;
                                Session["employee_id"] = employeeIdObj == DBNull.Value ? null : employeeIdObj;

                                targetRole = role;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login error: " + ex.Message);
                ShowMessage("حدث خطأ في الاتصال بقاعدة البيانات");
                return;
            }

            if (targetRole != null)
            {
                Response.Redirect(AuthHelper.GetHomePageForRole(targetRole), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                ShowMessage("اسم المستخدم أو كلمة المرور غير صحيحة");
                inputPassword.Text = "";
                inputPassword.Focus();
            }
        }

        private void ShowMessage(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
        }
    }
}