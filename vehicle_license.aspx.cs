using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class vehicle_license : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlVehicleInfo.Visible = false;
                pnlCurrentLicense.Visible = false;
                pnlLicenseForm.Visible = false;
                pnlLicenseHistory.Visible = false;
                pnlActiveLicenseWarning.Visible = false;
                pnlMandatoryInsuranceInfo.Visible = false;

                LoadDropDowns();
                SetDefaultDates();
            }
        }

        private void SetDefaultDates()
        {
            txtIssueDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtExpiryDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
        }

        private void LoadDropDowns()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Load license types
                    string licenseQuery = "SELECT id_LT, type FROM v_license_type WHERE is_active = 1 ORDER BY type";
                    SqlDataAdapter da = new SqlDataAdapter(licenseQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlLicenseType.DataSource = dt;
                    ddlLicenseType.DataTextField = "type";
                    ddlLicenseType.DataValueField = "id_LT";
                    ddlLicenseType.DataBind();
                    ddlLicenseType.Items.Insert(0, new ListItem("-- اختر نوع الترخيص --", ""));

                    // Load traffic units
                    string trafficQuery = "SELECT id_TF, name FROM traffic_unit WHERE is_active = 1 ORDER BY name";
                    da = new SqlDataAdapter(trafficQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);
                    ddlTrafficUnit.DataSource = dt;
                    ddlTrafficUnit.DataTextField = "name";
                    ddlTrafficUnit.DataValueField = "id_TF";
                    ddlTrafficUnit.DataBind();
                    ddlTrafficUnit.Items.Insert(0, new ListItem("-- اختر وحدة المرور --", ""));

                    // Load mandatory insurance companies
                    LoadMandatoryInsuranceCompanies();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message);
                }
            }
        }

        private void LoadMandatoryInsuranceCompanies()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DISTINCT company_name FROM car_insurance WHERE company_name IS NOT NULL AND company_name != '' ORDER BY company_name";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlMandatoryInsurance.DataSource = dt;
                    ddlMandatoryInsurance.DataTextField = "company_name";
                    ddlMandatoryInsurance.DataValueField = "company_name";
                    ddlMandatoryInsurance.DataBind();
                    ddlMandatoryInsurance.Items.Insert(0, new ListItem("-- اختر شركة التأمين --", ""));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"خطأ في تحميل شركات التأمين: {ex.Message}");
                }
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string letter1 = txtCarLetter1.Text.Trim();
                string letter2 = txtCarLetter2.Text.Trim();
                string letter3 = txtCarLetter3.Text.Trim();
                string numbers = txtCarNumbers.Text.Trim();

                if (string.IsNullOrEmpty(letter1) || string.IsNullOrEmpty(numbers))
                {
                    ShowMessage("❌ الرجاء إدخال رقم السيارة كاملاً");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);
                    if (carNumberId == 0)
                    {
                        ShowMessage("❌ رقم السيارة غير موجود في قاعدة البيانات");
                        return;
                    }

                    int vehicleId = GetVehicleId(conn, carNumberId);

                    if (vehicleId == 0)
                    {
                        ShowMessage("❌ لا توجد بيانات للسيارة المحددة");
                        return;
                    }

                    // Store in ViewState
                    ViewState["CurrentVehicleId"] = vehicleId;
                    ViewState["CurrentCarNumberId"] = carNumberId;

                    // Display vehicle info
                    string displayNumber = $"{letter1} {letter2} {(string.IsNullOrEmpty(letter3) ? "" : letter3)} {numbers}";
                    lblCarNumber.Text = displayNumber;

                    // Load data
                    LoadCurrentLicense(conn, vehicleId);
                    LoadLicenseHistory(conn, vehicleId);

                    pnlVehicleInfo.Visible = true;
                    pnlLicenseHistory.Visible = true;

                    ShowSuccess("✅ تم العثور على السيارة");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("❌ حدث خطأ أثناء البحث: " + ex.Message);
            }
        }

        private int GetCarNumberId(SqlConnection conn, string letter1, string letter2, string letter3, string numbers)
        {
            string query = @"
                SELECT car_number_id 
                FROM car_numbers 
                WHERE letter1 = @Letter1 
                  AND (letter2 = @Letter2 OR (letter2 IS NULL AND @Letter2 IS NULL))
                  AND (letter3 = @Letter3 OR (letter3 IS NULL AND @Letter3 IS NULL))
                  AND plate_numbers = @Numbers";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Letter1", letter1);
                cmd.Parameters.AddWithValue("@Letter2", string.IsNullOrEmpty(letter2) ? DBNull.Value : (object)letter2);
                cmd.Parameters.AddWithValue("@Letter3", string.IsNullOrEmpty(letter3) ? DBNull.Value : (object)letter3);
                cmd.Parameters.AddWithValue("@Numbers", numbers);
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

        private void LoadCurrentLicense(SqlConnection conn, int vehicleId)
        {
            string query = @"
                SELECT TOP 1 
                    vl.license_id, vl.license_number, lt.type AS license_type,
                    vl.issue_date, vl.expiry_date,
                    tu.name AS traffic_unit,
                    vl.renewal_cost, vl.registration_number, vl.notes
                FROM vehicle_licenses vl
                LEFT JOIN v_license_type lt ON vl.license_type_id = lt.id_LT
                LEFT JOIN traffic_unit tu ON vl.traffic_unit_id = tu.id_TF
                WHERE vl.vehicle_id = @VehicleId AND (vl.deleted = 0 OR vl.deleted IS NULL)
                ORDER BY vl.issue_date DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pnlCurrentLicense.Visible = true;
                        lblCurrentLicenseNumber.Text = reader["license_number"]?.ToString() ?? "-";
                        lblCurrentLicenseType.Text = reader["license_type"]?.ToString() ?? "-";
                        lblCurrentIssueDate.Text = reader["issue_date"] != DBNull.Value ?
                            Convert.ToDateTime(reader["issue_date"]).ToString("yyyy-MM-dd") : "-";

                        DateTime expiryDate = Convert.ToDateTime(reader["expiry_date"]);
                        lblCurrentExpiryDate.Text = expiryDate.ToString("yyyy-MM-dd");

                        lblCurrentTrafficUnit.Text = reader["traffic_unit"]?.ToString() ?? "-";
                        lblCurrentRenewCost.Text = reader["renewal_cost"] != DBNull.Value ?
                            Convert.ToDecimal(reader["renewal_cost"]).ToString("N2") + " ج.م" : "-";
                        lblCurrentRegistrationNumber.Text = reader["registration_number"]?.ToString() ?? "-";
                        lblCurrentNotes.Text = reader["notes"]?.ToString() ?? "-";

                        int licenseId = Convert.ToInt32(reader["license_id"]);
                        ViewState["CurrentLicenseId"] = licenseId;

                        reader.Close();

                        // Load mandatory insurance for this license
                        LoadMandatoryInsuranceForLicense(conn, licenseId);

                        // Check if license is still active
                        if (expiryDate > DateTime.Now)
                        {
                            pnlActiveLicenseWarning.Visible = true;
                            lblActiveLicenseWarning.Text = "⚠️ يوجد ترخيص ساري حتى " + expiryDate.ToString("yyyy-MM-dd") +
                                ". لا يمكن إضافة ترخيص جديد إلا بعد انتهاء الترخيص الحالي.";
                            btnNewLicense.Visible = false;
                        }
                        else
                        {
                            pnlActiveLicenseWarning.Visible = false;
                            btnNewLicense.Visible = true;
                        }
                    }
                    else
                    {
                        pnlCurrentLicense.Visible = false;
                        pnlMandatoryInsuranceInfo.Visible = false;
                        pnlActiveLicenseWarning.Visible = false;
                        btnNewLicense.Visible = true;
                        ViewState["CurrentLicenseId"] = null;
                    }
                }
            }
        }

        private void LoadMandatoryInsuranceForLicense(SqlConnection conn, int licenseId)
        {
            try
            {
                string query = @"
                    SELECT TOP 1 ci.company_name, ci.cost, ci.notes, ci.start_date, ci.end_date
                    FROM car_insurance ci
                    INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                    WHERE vi.vehicle_id = (SELECT vehicle_id FROM vehicle_licenses WHERE license_id = @LicenseId)
                      AND ci.insurance_name = N'تأمين إجباري'
                      AND (ci.deleted = 0 OR ci.deleted IS NULL)
                    ORDER BY ci.reg_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LicenseId", licenseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pnlMandatoryInsuranceInfo.Visible = true;
                            lblMandatoryInsuranceCompany.Text = reader["company_name"]?.ToString() ?? "-";
                            lblMandatoryInsuranceCost.Text = reader["cost"] != DBNull.Value ?
                                Convert.ToDecimal(reader["cost"]).ToString("N2") + " ج.م" : "-";
                            lblMandatoryInsuranceNotes.Text = reader["notes"]?.ToString() ?? "-";
                        }
                        else
                        {
                            pnlMandatoryInsuranceInfo.Visible = false;
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading mandatory insurance: {ex.Message}");
                pnlMandatoryInsuranceInfo.Visible = false;
            }
        }

        private void LoadLicenseHistory(SqlConnection conn, int vehicleId)
        {
            string query = @"
                SELECT 
                    vl.license_id, vl.license_number, lt.type AS license_type,
                    vl.issue_date, vl.expiry_date,
                    tu.name AS traffic_unit, vl.renewal_cost, vl.registration_number,
                    vl.notes,
                    (SELECT TOP 1 ci.company_name 
                     FROM car_insurance ci
                     INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                     WHERE vi.vehicle_id = vl.vehicle_id AND ci.insurance_name = N'تأمين إجباري'
                     ORDER BY ci.reg_date DESC) AS mandatory_insurance
                FROM vehicle_licenses vl
                LEFT JOIN v_license_type lt ON vl.license_type_id = lt.id_LT
                LEFT JOIN traffic_unit tu ON vl.traffic_unit_id = tu.id_TF
                WHERE vl.vehicle_id = @VehicleId AND (vl.deleted = 0 OR vl.deleted IS NULL)
                ORDER BY vl.issue_date DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvLicenseHistory.DataSource = dt;
                gvLicenseHistory.DataBind();
            }
        }

        protected void BtnSaveLicense_Click(object sender, EventArgs e)
        {
            try
            {
                int currentVehicleId = ViewState["CurrentVehicleId"] != null ? Convert.ToInt32(ViewState["CurrentVehicleId"]) : 0;

                if (currentVehicleId == 0)
                {
                    ShowMessage("❌ الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (!CanAddNewLicense(currentVehicleId))
                {
                    ShowMessage("❌ لا يمكن إضافة ترخيص جديد. يوجد ترخيص ساري حتى تاريخ لاحق.");
                    return;
                }

                if (!ValidateLicenseForm()) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            DateTime issueDate = DateTime.Parse(txtIssueDate.Text);
                            DateTime expiryDate = DateTime.Parse(txtExpiryDate.Text);

                            string insertLicenseQuery = @"
                                INSERT INTO vehicle_licenses (
                                    vehicle_id, license_number, license_type_id, issue_date, expiry_date,
                                    traffic_unit_id, renewal_cost, registration_number,
                                    technical_inspection_date, technical_inspection_expiry,
                                    notes, is_active)
                                VALUES (
                                    @VehicleId, @LicenseNumber, @LicenseTypeId, @IssueDate, @ExpiryDate,
                                    @TrafficUnitId, @RenewalCost, @RegistrationNumber,
                                    @TechInspectionDate, @TechInspectionExpiry,
                                    @Notes, 1);
                                SELECT SCOPE_IDENTITY();";

                            int newLicenseId;
                            using (SqlCommand cmd = new SqlCommand(insertLicenseQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);
                                cmd.Parameters.AddWithValue("@LicenseNumber", txtLicenseNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@LicenseTypeId", string.IsNullOrEmpty(ddlLicenseType.SelectedValue) ? DBNull.Value : (object)Convert.ToInt32(ddlLicenseType.SelectedValue));
                                cmd.Parameters.AddWithValue("@IssueDate", issueDate);
                                cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                                cmd.Parameters.AddWithValue("@TrafficUnitId", string.IsNullOrEmpty(ddlTrafficUnit.SelectedValue) ? DBNull.Value : (object)Convert.ToInt32(ddlTrafficUnit.SelectedValue));
                                cmd.Parameters.AddWithValue("@RenewalCost", string.IsNullOrEmpty(txtRenewCost.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtRenewCost.Text));
                                cmd.Parameters.AddWithValue("@RegistrationNumber", string.IsNullOrEmpty(txtRegistrationNumber.Text) ? DBNull.Value : (object)txtRegistrationNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@TechInspectionDate", string.IsNullOrEmpty(txtTechInspectionDate.Text) ? DBNull.Value : (object)DateTime.Parse(txtTechInspectionDate.Text));
                                cmd.Parameters.AddWithValue("@TechInspectionExpiry", string.IsNullOrEmpty(txtTechInspectionExpiry.Text) ? DBNull.Value : (object)DateTime.Parse(txtTechInspectionExpiry.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? DBNull.Value : (object)txtNotes.Text.Trim());
                                newLicenseId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (!string.IsNullOrEmpty(ddlMandatoryInsurance.SelectedValue))
                            {
                                SaveMandatoryInsurance(conn, transaction, currentVehicleId, issueDate, expiryDate);
                            }

                            transaction.Commit();

                            ShowSuccess("✅ تم حفظ الترخيص" + (string.IsNullOrEmpty(ddlMandatoryInsurance.SelectedValue) ? "" : " والتأمين الإجباري") + " بنجاح");
                            ClearForm();
                            LoadCurrentLicense(conn, currentVehicleId);
                            LoadLicenseHistory(conn, currentVehicleId);
                            pnlLicenseForm.Visible = false;
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
                ShowMessage("❌ حدث خطأ أثناء حفظ البيانات: " + ex.Message);
            }
        }

        private void SaveMandatoryInsurance(SqlConnection conn, SqlTransaction transaction, int vehicleId, DateTime issueDate, DateTime expiryDate)
        {
            string insertInsurance = @"
                INSERT INTO car_insurance (
                    company_name, insurance_name, start_date, end_date, cost, notes, reg_date, deleted, hide
                ) VALUES (
                    @CompanyName, N'تأمين إجباري', @StartDate, @EndDate, @Cost, @Notes, GETDATE(), 0, 0
                );
                SELECT SCOPE_IDENTITY();";

            int insuranceId;
            using (SqlCommand cmd = new SqlCommand(insertInsurance, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@CompanyName", ddlMandatoryInsurance.SelectedValue);
                cmd.Parameters.AddWithValue("@StartDate", issueDate);
                cmd.Parameters.AddWithValue("@EndDate", expiryDate);
                cmd.Parameters.AddWithValue("@Cost", string.IsNullOrEmpty(txtMandatoryInsuranceCost.Text) ? 0 : Convert.ToDecimal(txtMandatoryInsuranceCost.Text));
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtMandatoryInsuranceNotes.Text) ? DBNull.Value : (object)txtMandatoryInsuranceNotes.Text.Trim());
                insuranceId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            string insertJunction = @"
                INSERT INTO vehicle_insurance (vehicle_id, insurance_id)
                VALUES (@VehicleId, @InsuranceId)";

            using (SqlCommand cmd = new SqlCommand(insertJunction, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                cmd.Parameters.AddWithValue("@InsuranceId", insuranceId);
                cmd.ExecuteNonQuery();
            }
        }

        private bool CanAddNewLicense(int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) FROM vehicle_licenses 
                    WHERE vehicle_id = @VehicleId 
                      AND expiry_date > GETDATE() 
                      AND (deleted = 0 OR deleted IS NULL)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                    int activeCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return activeCount == 0;
                }
            }
        }

        protected void BtnNewLicense_Click(object sender, EventArgs e)
        {
            int currentVehicleId = ViewState["CurrentVehicleId"] != null ? Convert.ToInt32(ViewState["CurrentVehicleId"]) : 0;

            if (currentVehicleId == 0)
            {
                ShowMessage("❌ الرجاء البحث عن السيارة أولاً");
                return;
            }

            if (!CanAddNewLicense(currentVehicleId))
            {
                ShowMessage("❌ لا يمكن إضافة ترخيص جديد. يوجد ترخيص ساري حتى تاريخ لاحق. يمكنك إضافة ترخيص جديد فقط بعد انتهاء الترخيص الحالي.");
                return;
            }

            pnlLicenseForm.Visible = true;
            btnNewLicense.Visible = false;
            lblFormTitle.Text = "إضافة ترخيص جديد";
            ClearForm();
        }

        protected void BtnCancelForm_Click(object sender, EventArgs e)
        {
            pnlLicenseForm.Visible = false;
            btnNewLicense.Visible = true;
            ClearForm();
        }

        protected void ViewLicense_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int licenseId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string licenseQuery = @"
                    SELECT vl.license_number, lt.type AS license_type,
                           vl.issue_date, vl.expiry_date,
                           tu.name AS traffic_unit, vl.renewal_cost, vl.registration_number,
                           vl.technical_inspection_date, vl.technical_inspection_expiry,
                           vl.notes
                    FROM vehicle_licenses vl
                    LEFT JOIN v_license_type lt ON vl.license_type_id = lt.id_LT
                    LEFT JOIN traffic_unit tu ON vl.traffic_unit_id = tu.id_TF
                    WHERE vl.license_id = @LicenseId";

                string licenseNumber = "";
                string licenseType = "";
                string issueDate = "";
                string expiryDate = "";
                string trafficUnit = "";
                string renewalCost = "0";
                string registrationNumber = "";
                string techInspectionDate = "";
                string techInspectionExpiry = "";
                string notes = "";

                using (SqlCommand cmd = new SqlCommand(licenseQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@LicenseId", licenseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            licenseNumber = reader["license_number"]?.ToString() ?? "-";
                            licenseType = reader["license_type"]?.ToString() ?? "-";
                            issueDate = reader["issue_date"] != DBNull.Value ? Convert.ToDateTime(reader["issue_date"]).ToString("yyyy-MM-dd") : "-";
                            expiryDate = reader["expiry_date"] != DBNull.Value ? Convert.ToDateTime(reader["expiry_date"]).ToString("yyyy-MM-dd") : "-";
                            trafficUnit = reader["traffic_unit"]?.ToString() ?? "-";
                            renewalCost = reader["renewal_cost"] != DBNull.Value ? Convert.ToDecimal(reader["renewal_cost"]).ToString("N2") : "0";
                            registrationNumber = reader["registration_number"]?.ToString() ?? "-";
                            techInspectionDate = reader["technical_inspection_date"] != DBNull.Value ? Convert.ToDateTime(reader["technical_inspection_date"]).ToString("yyyy-MM-dd") : "-";
                            techInspectionExpiry = reader["technical_inspection_expiry"] != DBNull.Value ? Convert.ToDateTime(reader["technical_inspection_expiry"]).ToString("yyyy-MM-dd") : "-";
                            notes = reader["notes"]?.ToString() ?? "-";
                        }
                        reader.Close();
                    }
                }

                string insuranceCompany = "";
                string insuranceCost = "0";
                string insuranceNotes = "";
                string insuranceStartDate = "";
                string insuranceEndDate = "";

                string insuranceQuery = @"
                    SELECT TOP 1 ci.company_name, ci.cost, ci.notes, ci.start_date, ci.end_date
                    FROM car_insurance ci
                    INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                    WHERE vi.vehicle_id = (SELECT vehicle_id FROM vehicle_licenses WHERE license_id = @LicenseId)
                      AND ci.insurance_name = N'تأمين إجباري'
                    ORDER BY ci.reg_date DESC";

                using (SqlCommand insuranceCmd = new SqlCommand(insuranceQuery, conn))
                {
                    insuranceCmd.Parameters.AddWithValue("@LicenseId", licenseId);
                    using (SqlDataReader insuranceReader = insuranceCmd.ExecuteReader())
                    {
                        if (insuranceReader.Read())
                        {
                            insuranceCompany = insuranceReader["company_name"]?.ToString() ?? "-";
                            insuranceCost = insuranceReader["cost"] != DBNull.Value ? Convert.ToDecimal(insuranceReader["cost"]).ToString("N2") : "0";
                            insuranceNotes = insuranceReader["notes"]?.ToString() ?? "-";
                            insuranceStartDate = insuranceReader["start_date"] != DBNull.Value ? Convert.ToDateTime(insuranceReader["start_date"]).ToString("yyyy-MM-dd") : "-";
                            insuranceEndDate = insuranceReader["end_date"] != DBNull.Value ? Convert.ToDateTime(insuranceReader["end_date"]).ToString("yyyy-MM-dd") : "-";
                        }
                        insuranceReader.Close();
                    }
                }

                var data = new
                {
                    license_number = licenseNumber,
                    license_type = licenseType,
                    issue_date = issueDate,
                    expiry_date = expiryDate,
                    traffic_unit = trafficUnit,
                    renewal_cost = renewalCost,
                    registration_number = registrationNumber,
                    tech_inspection_date = techInspectionDate,
                    tech_inspection_expiry = techInspectionExpiry,
                    notes = notes,
                    mandatory_insurance_company = insuranceCompany,
                    mandatory_insurance_cost = insuranceCost,
                    mandatory_insurance_notes = insuranceNotes,
                    mandatory_insurance_start_date = insuranceStartDate,
                    mandatory_insurance_end_date = insuranceEndDate
                };

                string script = $"showLicenseDetailsWithInsurance({new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data)}); $('#viewLicenseModal').modal('show');";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
            }
        }

        protected void DeleteLicense_Click(object sender, EventArgs e)
        {
            int currentVehicleId = ViewState["CurrentVehicleId"] != null ? Convert.ToInt32(ViewState["CurrentVehicleId"]) : 0;

            if (currentVehicleId == 0)
            {
                ShowMessage("❌ الرجاء البحث عن السيارة أولاً");
                return;
            }

            LinkButton btn = (LinkButton)sender;
            int licenseId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteQuery = "UPDATE vehicle_licenses SET deleted = 1 WHERE license_id = @LicenseId";
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@LicenseId", licenseId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        ShowSuccess("✅ تم حذف الترخيص بنجاح");
                        LoadCurrentLicense(conn, currentVehicleId);
                        LoadLicenseHistory(conn, currentVehicleId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowMessage("❌ حدث خطأ أثناء الحذف: " + ex.Message);
                    }
                }
            }
        }

        protected void GvLicenseHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int currentVehicleId = ViewState["CurrentVehicleId"] != null ? Convert.ToInt32(ViewState["CurrentVehicleId"]) : 0;

            if (currentVehicleId == 0)
            {
                ShowMessage("❌ الرجاء البحث عن السيارة أولاً");
                return;
            }

            gvLicenseHistory.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                LoadLicenseHistory(conn, currentVehicleId);
            }
        }

        protected void GvLicenseHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView row = (DataRowView)e.Row.DataItem;
                DateTime expiryDate = Convert.ToDateTime(row["expiry_date"]);

                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                if (lblStatus != null)
                {
                    int daysRemaining = (expiryDate - DateTime.Now).Days;
                    if (daysRemaining < 0)
                    {
                        lblStatus.Text = "منتهي";
                        lblStatus.CssClass = "status-expired";
                    }
                    else if (daysRemaining <= 30)
                    {
                        lblStatus.Text = $"ينتهي بعد {daysRemaining} يوم";
                        lblStatus.CssClass = "status-warning";
                    }
                    else
                    {
                        lblStatus.Text = "ساري";
                        lblStatus.CssClass = "status-active";
                    }
                }
            }
        }

        private bool ValidateLicenseForm()
        {
            if (string.IsNullOrEmpty(txtLicenseNumber.Text))
            {
                ShowMessage("❌ الرجاء إدخال رقم الترخيص");
                return false;
            }
            if (ddlLicenseType.SelectedValue == "")
            {
                ShowMessage("❌ الرجاء اختيار نوع الترخيص");
                return false;
            }
            if (string.IsNullOrEmpty(txtIssueDate.Text))
            {
                ShowMessage("❌ الرجاء إدخال تاريخ الإصدار");
                return false;
            }
            if (string.IsNullOrEmpty(txtExpiryDate.Text))
            {
                ShowMessage("❌ الرجاء إدخال تاريخ الانتهاء");
                return false;
            }

            DateTime issueDate = DateTime.Parse(txtIssueDate.Text);
            DateTime expiryDate = DateTime.Parse(txtExpiryDate.Text);

            if (expiryDate <= issueDate)
            {
                ShowMessage("❌ تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtLicenseNumber.Text = "";
            ddlLicenseType.SelectedValue = "";
            txtIssueDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtExpiryDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            ddlTrafficUnit.SelectedValue = "";
            txtRenewCost.Text = "";
            txtRegistrationNumber.Text = "";
            txtTechInspectionDate.Text = "";
            txtTechInspectionExpiry.Text = "";
            txtNotes.Text = "";
            ddlMandatoryInsurance.SelectedValue = "";
            txtMandatoryInsuranceCost.Text = "";
            txtMandatoryInsuranceNotes.Text = "";
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

        protected string GetStatusCss(DateTime expiryDate)
        {
            int daysRemaining = (expiryDate - DateTime.Now).Days;
            if (daysRemaining < 0) return "status-expired";
            if (daysRemaining <= 30) return "status-warning";
            return "status-active";
        }

        protected string GetStatusText(DateTime expiryDate)
        {
            int daysRemaining = (expiryDate - DateTime.Now).Days;
            if (daysRemaining < 0) return "منتهي";
            if (daysRemaining <= 30) return $"ينتهي بعد {daysRemaining} يوم";
            return "ساري";
        }
    }
}