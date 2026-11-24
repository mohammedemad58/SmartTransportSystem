using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class car_insurance : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int currentVehicleId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlCarInfo.Visible = false;
                pnlInsuranceForm.Visible = false;
                pnlInsuranceHistory.Visible = false;
                LoadInsuranceCompanies();
                SetDefaultDates();
            }
            else
            {
                // Check for refresh postback
                string target = Request.Form["__EVENTTARGET"];
                if (target == "refreshInsuranceCompanies")
                {
                    LoadInsuranceCompanies();
                }
            }
        }

        private void SetDefaultDates()
        {
            txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
        }

        private void LoadInsuranceCompanies()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // First try to load from insurance_companies table
                    string query = "SELECT company_name FROM insurance_companies WHERE is_active = 1 ORDER BY company_name";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // If no records in insurance_companies, fallback to car_insurance table
                    if (dt.Rows.Count == 0)
                    {
                        query = "SELECT DISTINCT company_name FROM car_insurance WHERE company_name IS NOT NULL AND company_name != '' ORDER BY company_name";
                        da = new SqlDataAdapter(query, conn);
                        dt = new DataTable();
                        da.Fill(dt);
                    }

                    ddlInsuranceCompany.DataSource = dt;
                    ddlInsuranceCompany.DataTextField = "company_name";
                    ddlInsuranceCompany.DataValueField = "company_name";
                    ddlInsuranceCompany.DataBind();
                    ddlInsuranceCompany.Items.Insert(0, new ListItem("-- اختر شركة التأمين --", ""));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"خطأ في تحميل شركات التأمين: {ex.Message}");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string letter1 = txtCarLetter1.Text.Trim();
                string letter2 = txtCarLetter2.Text.Trim();
                string letter3 = txtCarLetter3.Text.Trim();
                string numbers = txtCarNumbers.Text.Trim();

                if (string.IsNullOrEmpty(letter1) || string.IsNullOrEmpty(letter2) || string.IsNullOrEmpty(numbers))
                {
                    ShowMessage("الرجاء إدخال رقم السيارة كاملاً");
                    return;
                }

                string carNumberDisplay = $"{letter1} {letter2} {(string.IsNullOrEmpty(letter3) ? "" : letter3)} {numbers}";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);

                    if (carNumberId == 0)
                    {
                        ShowMessage("رقم السيارة غير موجود في قاعدة البيانات");
                        return;
                    }

                    currentVehicleId = GetVehicleId(conn, carNumberId);

                    if (currentVehicleId > 0)
                    {
                        ViewState["VehicleId"] = currentVehicleId;
                        ViewState["CarNumberId"] = carNumberId;
                        lblCarNumber.Text = carNumberDisplay;

                        pnlCarInfo.Visible = true;
                        pnlInsuranceForm.Visible = true;

                        // Load existing insurance history
                        LoadInsuranceHistory(conn, currentVehicleId);

                        lblMessage.Visible = false;
                        ShowSuccess("تم العثور على السيارة");
                    }
                    else
                    {
                        ShowMessage("لا توجد بيانات للسيارة المحددة");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message);
            }
        }

        private int GetCarNumberId(SqlConnection conn, string letter1, string letter2, string letter3, string numbers)
        {
            string query = @"
                SELECT car_number_id 
                FROM car_numbers 
                WHERE letter1 = @Letter1 
                  AND letter2 = @Letter2 
                  AND (letter3 = @Letter3 OR (@Letter3 IS NULL AND letter3 IS NULL))
                  AND plate_numbers = @Numbers
                  AND (is_active = 1 OR is_active IS NULL)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Letter1", letter1.Trim());
                cmd.Parameters.AddWithValue("@Letter2", letter2.Trim());
                cmd.Parameters.AddWithValue("@Letter3", string.IsNullOrEmpty(letter3) ? (object)DBNull.Value : letter3.Trim());
                cmd.Parameters.AddWithValue("@Numbers", numbers.Trim());

                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
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

        private void LoadInsuranceHistory(SqlConnection conn, int vehicleId)
        {
            try
            {
                // Load all comprehensive insurance records linked to this vehicle
                string query = @"
                    SELECT ci.company_name, ci.start_date, ci.end_date, ci.cost, ci.notes, ci.insurance_id
                    FROM car_insurance ci
                    INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                    WHERE vi.vehicle_id = @VehicleId
                      AND (ci.insurance_name = N'تأمين شامل' OR ci.insurance_name = N'تأمين تاملي' OR ci.insurance_name IS NULL)
                    ORDER BY ci.end_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvInsuranceHistory.DataSource = dt;
                        gvInsuranceHistory.DataBind();
                        pnlInsuranceHistory.Visible = dt.Rows.Count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل سجل التأمين: {ex.Message}");
                pnlInsuranceHistory.Visible = false;
            }
        }

        protected void btnSaveInsurance_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentVehicleId == 0 && ViewState["VehicleId"] != null)
                {
                    currentVehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                }

                if (currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(ddlInsuranceCompany.SelectedValue) ||
                    string.IsNullOrEmpty(txtStartDate.Text) ||
                    string.IsNullOrEmpty(txtEndDate.Text))
                {
                    ShowMessage("الرجاء إدخال جميع الحقول المطلوبة (شركة التأمين، تاريخ البداية، تاريخ النهاية)");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Insert into car_insurance table with insurance_name = 'تأمين شامل'
                            string insertInsurance = @"
                                INSERT INTO car_insurance (
                                    company_name, insurance_name, start_date, end_date, cost, 
                                    notes, reg_date, deleted, hide
                                ) VALUES (
                                    @CompanyName, N'تأمين شامل', @StartDate, @EndDate, @Cost,
                                    @Notes, GETDATE(), 0, 0
                                );
                                SELECT SCOPE_IDENTITY();";

                            using (SqlCommand cmd = new SqlCommand(insertInsurance, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CompanyName", ddlInsuranceCompany.SelectedValue);
                                cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(txtStartDate.Text));
                                cmd.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(txtEndDate.Text));
                                cmd.Parameters.AddWithValue("@Cost", string.IsNullOrEmpty(txtCost.Text) ? 0 : Convert.ToDecimal(txtCost.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());

                                int insuranceId = Convert.ToInt32(cmd.ExecuteScalar());

                                // Link to vehicle in junction table
                                string insertJunction = @"
                                    INSERT INTO vehicle_insurance (vehicle_id, insurance_id, created_date)
                                    VALUES (@VehicleId, @InsuranceId, GETDATE())";

                                using (SqlCommand jCmd = new SqlCommand(insertJunction, conn, transaction))
                                {
                                    jCmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);
                                    jCmd.Parameters.AddWithValue("@InsuranceId", insuranceId);
                                    jCmd.ExecuteNonQuery();
                                }

                                transaction.Commit();

                                // Reload history
                                LoadInsuranceHistory(conn, currentVehicleId);
                                ClearForm();
                                ShowSuccess("تم حفظ التأمين الشامل بنجاح");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء حفظ البيانات: " + ex.Message);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("main_vehicle_entry.aspx");
        }

        protected void gvInsuranceHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInsuranceHistory.PageIndex = e.NewPageIndex;
            if (currentVehicleId > 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    LoadInsuranceHistory(conn, currentVehicleId);
                }
            }
        }

        private void ClearForm()
        {
            ddlInsuranceCompany.SelectedValue = "";
            txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtCost.Text = "";
            txtNotes.Text = "";
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


    }
}