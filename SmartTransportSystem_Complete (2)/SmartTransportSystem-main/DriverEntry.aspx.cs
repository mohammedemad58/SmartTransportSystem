using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace car_WebApplication1
{
    public partial class DriverEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRoutes();
            }
        }

        private void LoadRoutes()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT route_id, route_name 
                        FROM routes 
                        WHERE is_active = 1 
                        ORDER BY route_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    lstRoutes.DataSource = reader;
                    lstRoutes.DataTextField = "route_name";
                    lstRoutes.DataValueField = "route_id";
                    lstRoutes.DataBind();

                    reader.Close();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل قائمة الخطوط: " + ex.Message);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // التحقق من الحقول المطلوبة
                if (string.IsNullOrEmpty(txtDriverName.Text))
                {
                    ShowMessage("الرجاء إدخال اسم السائق");
                    return;
                }

                if (string.IsNullOrEmpty(txtDriverCode.Text))
                {
                    ShowMessage("الرجاء إدخال كود السائق");
                    return;
                }

                if (string.IsNullOrEmpty(txtLicenseNumber.Text))
                {
                    ShowMessage("الرجاء إدخال رقم الرخصة");
                    return;
                }

                if (string.IsNullOrEmpty(txtPhone.Text))
                {
                    ShowMessage("الرجاء إدخال رقم الهاتف");
                    return;
                }

                if (string.IsNullOrEmpty(ddlLicenseType.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار نوع الرخصة");
                    return;
                }

                if (string.IsNullOrEmpty(txtLicenseStartDate.Text))
                {
                    ShowMessage("الرجاء إدخال تاريخ بداية الرخصة");
                    return;
                }

                if (string.IsNullOrEmpty(txtLicenseEndDate.Text))
                {
                    ShowMessage("الرجاء إدخال تاريخ نهاية الرخصة");
                    return;
                }

                // التحقق من الرقم القومي
                if (string.IsNullOrEmpty(txtNationalID.Text))
                {
                    ShowMessage("الرجاء إدخال الرقم القومي");
                    return;
                }

                // التحقق من الرقم القومي (14 رقم بالضبط)
                if (!IsValidNationalID(txtNationalID.Text.Trim()))
                {
                    ShowMessage("الرجاء إدخال رقم قومي صحيح مكون من 14 رقم");
                    return;
                }

                // التحقق من مسافات البداية والنهاية
                if (!ValidateDistances())
                {
                    return;
                }

                // حساب نتيجة المسافة تلقائياً
                CalculateResultDistance();

                // حفظ البيانات
                int newDriverId = SaveDriverData();

                // إذا تم الحفظ بنجاح، احفظ الخطوط
                if (newDriverId > 0)
                {
                    SaveDriverRoutes(newDriverId);
                    ShowSuccessMessage("تم حفظ بيانات السائق والخطوط بنجاح");
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message);
            }
        }

        private bool IsValidNationalID(string nationalId)
        {
            // إزالة أي مسافات
            nationalId = nationalId.Trim();

            // التحقق باستخدام Regular Expression: 14 رقم بالضبط
            return Regex.IsMatch(nationalId, @"^\d{14}$");
        }

        private bool ValidateDistances()
        {
            double startKM = 0;
            double endKM = 0;

            // تحويل القيم إلى أرقام
            if (!double.TryParse(txtWriteStartKM.Text, out startKM))
            {
                ShowMessage("مسافة البداية يجب أن تكون رقماً صحيحاً");
                return false;
            }

            if (!double.TryParse(txtWriteEndKM.Text, out endKM))
            {
                ShowMessage("مسافة النهاية يجب أن تكون رقماً صحيحاً");
                return false;
            }

            // التحقق من أن مسافة النهاية أكبر من أو تساوي مسافة البداية
            if (endKM < startKM)
            {
                ShowMessage("خطأ: مسافة النهاية لا يمكن أن تكون أصغر من مسافة البداية");
                return false;
            }

            return true;
        }

        private void CalculateResultDistance()
        {
            double startKM = 0;
            double endKM = 0;

            double.TryParse(txtWriteStartKM.Text, out startKM);
            double.TryParse(txtWriteEndKM.Text, out endKM);

            // حساب نتيجة المسافة (النهاية - البداية)
            double result = endKM - startKM;
            txtResultKM.Text = result.ToString("0.0");
        }

        private int SaveDriverData()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
            int newDriverId = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO drivers (
                            driver_name, driver_code, license_number, phone,
                            is_active, created_date, write_sub_count, write_start_KM,
                            write_end_KM, result_KM, car_count, employee_id,
                            license_type, license_start_date, license_end_date,
                            nationalID
                        ) VALUES (
                            @driver_name, @driver_code, @license_number, @phone,
                            @is_active, GETDATE(), @write_sub_count, @write_start_KM,
                            @write_end_KM, @result_KM, @car_count, @employee_id,
                            @license_type, @license_start_date, @license_end_date,
                            @nationalID
                        );
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // إضافة معاملات النصوص
                    cmd.Parameters.AddWithValue("@driver_name", txtDriverName.Text.Trim());
                    cmd.Parameters.AddWithValue("@driver_code", txtDriverCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@license_number", txtLicenseNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@is_active", chkIsActive.Checked);
                    cmd.Parameters.AddWithValue("@license_type", ddlLicenseType.SelectedValue);

                    // الرقم القومي في الحقل المخصص nationalID
                    cmd.Parameters.AddWithValue("@nationalID", txtNationalID.Text.Trim());

                    // إضافة معاملات الأرقام
                    cmd.Parameters.AddWithValue("@write_sub_count",
                        string.IsNullOrEmpty(txtWriteSubCount.Text) ? 0 : Convert.ToInt32(txtWriteSubCount.Text));

                    cmd.Parameters.AddWithValue("@write_start_KM",
                        string.IsNullOrEmpty(txtWriteStartKM.Text) ? 0 : Convert.ToDouble(txtWriteStartKM.Text));

                    cmd.Parameters.AddWithValue("@write_end_KM",
                        string.IsNullOrEmpty(txtWriteEndKM.Text) ? 0 : Convert.ToDouble(txtWriteEndKM.Text));

                    cmd.Parameters.AddWithValue("@result_KM",
                        string.IsNullOrEmpty(txtResultKM.Text) ? 0 : Convert.ToDouble(txtResultKM.Text));

                    cmd.Parameters.AddWithValue("@car_count",
                        string.IsNullOrEmpty(txtCarCount.Text) ? 0 : Convert.ToInt32(txtCarCount.Text));

                    // معاملة رقم الموظف (يمكن أن يكون NULL)
                    if (string.IsNullOrEmpty(txtEmployeeID.Text))
                    {
                        cmd.Parameters.AddWithValue("@employee_id", DBNull.Value);
                    }
                    else
                    {
                        int empId = Convert.ToInt32(txtEmployeeID.Text);
                        if (CheckEmployeeExists(empId))
                        {
                            cmd.Parameters.AddWithValue("@employee_id", empId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@employee_id", DBNull.Value);
                            // رسالة تحذيرية (اختياري)
                            ShowMessage("رقم الموظف غير موجود، تم حفظ السائق بدون ربط موظف");
                        }
                    }

                    // إضافة معاملات التواريخ
                    cmd.Parameters.AddWithValue("@license_start_date", Convert.ToDateTime(txtLicenseStartDate.Text));
                    cmd.Parameters.AddWithValue("@license_end_date", Convert.ToDateTime(txtLicenseEndDate.Text));

                    // الحصول على الـ ID الجديد
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        newDriverId = Convert.ToInt32(result);
                    }
                }
                catch (SqlException sqlEx)
                {
                    // التعامل مع أخطاء قاعدة البيانات المحددة
                    if (sqlEx.Number == 2627) // خطأ المفتاح المكرر
                    {
                        ShowMessage("كود السائق أو الرقم القومي موجود بالفعل");
                    }
                    else
                    {
                        ShowMessage("خطأ في قاعدة البيانات: " + sqlEx.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message);
                }
            }
            return newDriverId;
        }
        private bool CheckEmployeeExists(int employeeId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM employees WHERE employee_id = @employee_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@employee_id", employeeId);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        private void SaveDriverRoutes(int driverId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // حذف الخطوط القديمة أولاً (إذا كانت موجودة)
                    string deleteQuery = "DELETE FROM operation_command WHERE driver_name = @driver_id";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@driver_id", driverId);
                    deleteCmd.ExecuteNonQuery();

                    // إضافة الخطوط الجديدة
                    foreach (ListItem item in lstRoutes.Items)
                    {
                        if (item.Selected)
                        {
                            string insertQuery = @"
                                INSERT INTO operation_command (driver_name, location_point, created_date)
                                VALUES (@driver_id, @route_id, GETDATE())";

                            SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                            insertCmd.Parameters.AddWithValue("@driver_id", driverId);
                            insertCmd.Parameters.AddWithValue("@route_id", Convert.ToInt32(item.Value));
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في حفظ الخطوط: " + ex.Message);
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        protected void txtWriteStartKM_TextChanged(object sender, EventArgs e)
        {
            CalculateResultDistance();
        }

        protected void txtWriteEndKM_TextChanged(object sender, EventArgs e)
        {
            CalculateResultDistance();
        }

        private void ClearFields()
        {
            txtDriverName.Text = "";
            txtDriverCode.Text = "";
            txtLicenseNumber.Text = "";
            txtPhone.Text = "";
            txtNationalID.Text = "";
            txtWriteSubCount.Text = "0";
            txtWriteStartKM.Text = "0";
            txtWriteEndKM.Text = "0";
            txtResultKM.Text = "0";
            txtCarCount.Text = "0";
            txtEmployeeID.Text = "";
            txtLicenseStartDate.Text = "";
            txtLicenseEndDate.Text = "";
            ddlLicenseType.SelectedIndex = 0;
            chkIsActive.Checked = true;

            // إلغاء تحديد جميع الخطوط
            foreach (ListItem item in lstRoutes.Items)
            {
                item.Selected = false;
            }

            // إخفاء الرسائل
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccessMessage(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }
    }
}