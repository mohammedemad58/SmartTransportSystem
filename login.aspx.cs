using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace cars_WebApplication1
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            pnlMessage.Visible = false;

            if (Session["username"] != null && !IsPostBack)
            {
                Response.Redirect("dashboard.aspx");
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string username = inputEmail.Text.Trim();
            string password = inputPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("الرجاء إدخال اسم المستخدم وكلمة المرور");
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = @"SELECT COUNT(*) FROM [login] WHERE user_name = @Username AND user_password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        con.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 1)
                        {
                            Session["username"] = username;
                            Response.Redirect("dashboard.aspx");
                        }
                        else
                        {
                            ShowMessage("اسم المستخدم أو كلمة المرور غير صحيحة");
                            inputPassword.Text = "";
                            inputPassword.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ في الاتصال بقاعدة البيانات");
            }
        }

        private void ShowMessage(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
        }
    }
}