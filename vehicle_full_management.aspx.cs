using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace car_WebApplication1
{
    public partial class VehicleFullManagement : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int _currentVehicleId = 0;
        private int _currentCarNumberId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Hide all panels initially
                HideAllPanels();
                LoadDropDowns();
                LoadPartsToViewState();
                SetDefaultDates();

                // Load from session if coming from another page
                if (Session["VehicleIdToLoad"] != null && Session["CarNumberToLoad"] != null)
                {
                    int idFromSession = Convert.ToInt32(Session["VehicleIdToLoad"]);
                    string carNumber = Session["CarNumberToLoad"].ToString();
                    Session.Remove("VehicleIdToLoad");
                    Session.Remove("CarNumberToLoad");

                    // Parse car number and set fields
                    string[] parts = carNumber.Split(' ');
                    if (parts.Length >= 3)
                    {
                        txtCarLetter1.Text = parts[0];
                        txtCarLetter2.Text = parts[1];
                        txtCarNumbers.Text = parts[parts.Length - 1];
                        if (parts.Length > 3) txtCarLetter3.Text = parts[2];

                        // Trigger search
                        ScriptManager.RegisterStartupScript(this, GetType(), "loadData",
                            "setTimeout(function() { document.getElementById('" + btnSearch.ClientID + "').click(); }, 100);", true);
                    }
                }
            }
            else
            {
                // Restore vehicle IDs from ViewState on postback
                if (ViewState["VehicleId"] != null)
                {
                    _currentVehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                }
                if (ViewState["CarNumberId"] != null)
                {
                    _currentCarNumberId = Convert.ToInt32(ViewState["CarNumberId"]);
                }

                // Restore car number display from ViewState
                if (ViewState["CarNumberDisplay"] != null)
                {
                    lblCarNumber.Text = ViewState["CarNumberDisplay"].ToString();
                    pnlVehicleInfo.Visible = true;
                }

                // Restore panels if vehicle exists
                if (_currentVehicleId > 0)
                {
                    pnlCurrentOdometer.Visible = true;
                    pnlEngineOverhaul.Visible = true;
                    pnlStatistics.Visible = true;
                    pnlTabs.Visible = true;
                }
                RestoreActiveTab();
            }

            // Handle postback from modal edit for routes
            if (Request["__EVENTTARGET"] == "UpdateRoute")
            {
                string argument = Request["__EVENTARGUMENT"];
                if (!string.IsNullOrEmpty(argument))
                {
                    string[] parts = argument.Split('|');
                    if (parts.Length >= 13)
                    {
                        int routeId = Convert.ToInt32(parts[0]);
                        string routeName = parts[1];
                        string description = parts[2];
                        string startLoc = parts[3];
                        string endLoc = parts[4];
                        decimal km = Convert.ToDecimal(parts[5]);
                        string direction = parts[6];
                        int subscriptionCount = Convert.ToInt32(parts[7]);
                        bool isActive = Convert.ToBoolean(parts[8]);
                        DateTime startDate = Convert.ToDateTime(parts[9]);
                        DateTime endDate = Convert.ToDateTime(parts[10]);
                        string driverName = parts[11];
                        string driverPhone = parts[12];

                        UpdateRoute(routeId, routeName, description, startLoc, endLoc, km, direction, subscriptionCount, isActive, startDate, endDate, driverName, driverPhone);
                    }
                }
            }
        }

        private void HideAllPanels()
        {
            pnlVehicleInfo.Visible = false;
            pnlCurrentOdometer.Visible = false;
            pnlEngineOverhaul.Visible = false;
            pnlStatistics.Visible = false;
            pnlTabs.Visible = false;
            pnlActiveLicenseWarning.Visible = false;
            pnlLicenseForm.Visible = false;
            pnlMandatoryInsuranceInfo.Visible = false;
            pnlRouteDetails.Visible = false;
        }

        private void SetDefaultDates()
        {
            txtIssueDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtExpiryDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtInsuranceStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtInsuranceEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtSubStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtSubEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtRouteStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtRouteEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtMaintenanceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void LoadPartsToViewState()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT PartID, PartName FROM MaintenanceParts ORDER BY PartName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    var partsList = new List<object>();
                    foreach (DataRow row in dt.Rows)
                    {
                        partsList.Add(new { id = row["PartID"].ToString(), name = row["PartName"].ToString() });
                    }

                    var serializer = new JavaScriptSerializer();
                    ViewState["PartsData"] = serializer.Serialize(partsList);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading parts: {ex.Message}");
                ViewState["PartsData"] = "[]";
            }
        }

        private void LoadDropDowns()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // License Types
                string licenseQuery = "SELECT id_LT, type FROM v_license_type WHERE is_active = 1 ORDER BY type";
                SqlDataAdapter da = new SqlDataAdapter(licenseQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlLicenseType.DataSource = dt;
                ddlLicenseType.DataTextField = "type";
                ddlLicenseType.DataValueField = "id_LT";
                ddlLicenseType.DataBind();
                ddlLicenseType.Items.Insert(0, new ListItem("-- اختر نوع الترخيص --", ""));

                // Traffic Units
                string trafficQuery = "SELECT id_TF, name FROM traffic_unit WHERE is_active = 1 ORDER BY name";
                da = new SqlDataAdapter(trafficQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                ddlTrafficUnit.DataSource = dt;
                ddlTrafficUnit.DataTextField = "name";
                ddlTrafficUnit.DataValueField = "id_TF";
                ddlTrafficUnit.DataBind();
                ddlTrafficUnit.Items.Insert(0, new ListItem("-- اختر وحدة المرور --", ""));

                // Insurance Companies
                string insuranceQuery = "SELECT DISTINCT company_name FROM car_insurance WHERE company_name IS NOT NULL AND company_name != '' ORDER BY company_name";
                da = new SqlDataAdapter(insuranceQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                ddlInsuranceCompany.DataSource = dt;
                ddlInsuranceCompany.DataTextField = "company_name";
                ddlInsuranceCompany.DataValueField = "company_name";
                ddlInsuranceCompany.DataBind();
                ddlInsuranceCompany.Items.Insert(0, new ListItem("-- اختر شركة التأمين --", ""));

                // Mandatory Insurance Companies
                string mandatoryQuery = "SELECT DISTINCT company_name FROM car_insurance WHERE company_name IS NOT NULL AND company_name != '' ORDER BY company_name";
                da = new SqlDataAdapter(mandatoryQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                ddlMandatoryInsurance.DataSource = dt;
                ddlMandatoryInsurance.DataTextField = "company_name";
                ddlMandatoryInsurance.DataValueField = "company_name";
                ddlMandatoryInsurance.DataBind();
                ddlMandatoryInsurance.Items.Insert(0, new ListItem("-- اختر شركة التأمين --", ""));

                // Subscribers
                string subscriberQuery = "SELECT n_id, name FROM subscription_details WHERE name IS NOT NULL AND name != '' ORDER BY name";
                da = new SqlDataAdapter(subscriberQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                ddlSubscriber.DataSource = dt;
                ddlSubscriber.DataTextField = "name";
                ddlSubscriber.DataValueField = "n_id";
                ddlSubscriber.DataBind();
                ddlSubscriber.Items.Insert(0, new ListItem("-- اختر المشترك --", ""));

                // Routes for dropdown
                string routeQuery = "SELECT route_id, route_name FROM routes WHERE is_active = 1 ORDER BY route_name";
                da = new SqlDataAdapter(routeQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                ddlRoute.DataSource = dt;
                ddlRoute.DataTextField = "route_name";
                ddlRoute.DataValueField = "route_id";
                ddlRoute.DataBind();
                ddlRoute.Items.Insert(0, new ListItem("-- اختر المسار --", ""));
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

                if (string.IsNullOrEmpty(letter1) || string.IsNullOrEmpty(letter2) || string.IsNullOrEmpty(numbers))
                {
                    ShowMessage("الرجاء إدخال رقم السيارة كاملاً");
                    return;
                }

                string carNumberDisplay = $"{letter1} {letter2} {(string.IsNullOrEmpty(letter3) ? "" : letter3)} {numbers}";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);
                    if (carNumberId == 0)
                    {
                        ShowMessage("رقم السيارة غير موجود في قاعدة البيانات");
                        return;
                    }

                    _currentCarNumberId = carNumberId;
                    _currentVehicleId = GetVehicleId(conn, carNumberId);

                    if (_currentVehicleId > 0)
                    {
                        // Save to ViewState for postback persistence
                        ViewState["VehicleId"] = _currentVehicleId;
                        ViewState["CarNumberId"] = _currentCarNumberId;
                        ViewState["CarNumberDisplay"] = carNumberDisplay;
                        ViewState["CarLetter1"] = letter1;
                        ViewState["CarLetter2"] = letter2;
                        ViewState["CarLetter3"] = letter3;
                        ViewState["CarNumbers"] = numbers;

                        lblCarNumber.Text = carNumberDisplay;

                        // Show all panels
                        pnlVehicleInfo.Visible = true;
                        pnlCurrentOdometer.Visible = true;
                        pnlEngineOverhaul.Visible = true;
                        pnlStatistics.Visible = true;
                        pnlTabs.Visible = true;

                        // Load data
                        int workOrderKM = GetLastWorkOrderKilometer(_currentCarNumberId);
                        lblCurrentOdometer.Text = workOrderKM.ToString("N0");
                        ViewState["LastWorkOrderKM"] = workOrderKM;

                        LoadEngineOverhaulInfo(conn);
                        LoadStatistics(conn);
                        LoadCurrentLicense(conn);
                        LoadLicenseHistory(conn);
                        LoadMandatoryInsuranceForCurrentLicense(conn);
                        LoadInsuranceHistory(conn);
                        LoadMaintenanceHistory(conn);
                        LoadSubscriberHistory(conn);
                        LoadVehicleRoutes(conn);
                        LoadAllRoutes();

                        lblMessage.Visible = false;
                        ShowSuccess("تم العثور على السيارة");

                        // Update tab counts
                        UpdateTabCounts();
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
                System.Diagnostics.Debug.WriteLine($"Error in BtnSearch_Click: {ex.Message}");
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

        private int GetLastWorkOrderKilometer(int carNumberId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT TOP 1 ISNULL(kilometer_end, 0) as kilometer_end
                        FROM operation_command
                        WHERE v_number = @CarNumberId
                          AND kilometer_end IS NOT NULL
                          AND kilometer_end > 0
                        ORDER BY mission_id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return Convert.ToInt32(result);
                    }
                }
            }
            catch { }
            return 0;
        }

        // ===== ENGINE OVERHAUL =====
        private void LoadEngineOverhaulInfo(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT EngineOverhaulDate, EngineOverhaulOdometer, EngineOverhaulCost, EngineOverhaulNotes
                    FROM main_vehicle_details
                    WHERE v_id = @VehicleID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && reader["EngineOverhaulDate"] != DBNull.Value)
                        {
                            DateTime overhaulDate = Convert.ToDateTime(reader["EngineOverhaulDate"]);
                            txtEngineOverhaulDate.Text = overhaulDate.ToString("yyyy-MM-dd");

                            if (reader["EngineOverhaulOdometer"] != DBNull.Value)
                            {
                                int overhaulOdometer = Convert.ToInt32(reader["EngineOverhaulOdometer"]);
                                txtEngineOverhaulOdometer.Text = overhaulOdometer.ToString();
                            }

                            if (reader["EngineOverhaulCost"] != DBNull.Value)
                            {
                                decimal overhaulCost = Convert.ToDecimal(reader["EngineOverhaulCost"]);
                                txtEngineOverhaulCost.Text = overhaulCost.ToString("N2");
                            }

                            if (reader["EngineOverhaulNotes"] != DBNull.Value)
                            {
                                txtEngineOverhaulNotes.Text = reader["EngineOverhaulNotes"].ToString();
                            }

                            int currentOdometer = ViewState["LastWorkOrderKM"] != null ?
                                Convert.ToInt32(ViewState["LastWorkOrderKM"]) : 0;

                            int odometerAtOverhaul = 0;
                            if (!string.IsNullOrEmpty(txtEngineOverhaulOdometer.Text))
                            {
                                int.TryParse(txtEngineOverhaulOdometer.Text, out odometerAtOverhaul);
                            }

                            int kmSinceOverhaul = currentOdometer - odometerAtOverhaul;

                            string statusMessage;
                            string statusColor;
                            string statusClass;

                            if (kmSinceOverhaul <= 0)
                            {
                                statusMessage = "✅ تم إجراء عمرة الموتور مؤخراً";
                                statusColor = "#4caf50";
                                statusClass = "overhaul-good";
                            }
                            else if (kmSinceOverhaul <= 50000)
                            {
                                statusMessage = $"🟢 حالة الموتور جيدة - تم قطع {kmSinceOverhaul:N0} كم منذ العمرة";
                                statusColor = "#4caf50";
                                statusClass = "overhaul-good";
                            }
                            else if (kmSinceOverhaul <= 100000)
                            {
                                statusMessage = $"🟡 يحتاج إلى متابعة - تم قطع {kmSinceOverhaul:N0} كم منذ العمرة";
                                statusColor = "#ff9800";
                                statusClass = "overhaul-info-text";
                            }
                            else
                            {
                                statusMessage = $"🔴 يحتاج إلى عمرة جديدة - تم قطع {kmSinceOverhaul:N0} كم منذ العمرة";
                                statusColor = "#f44336";
                                statusClass = "overhaul-warning-text";
                            }

                            lblOverhaulStatus.Text = statusMessage;
                            lblOverhaulStatus.Style.Add("color", statusColor);
                            divOverhaulStatus.Attributes.Add("class", $"overhaul-status {statusClass}");
                        }
                        else
                        {
                            lblOverhaulStatus.Text = "⚠️ لم يتم تسجيل عمرة موتور لهذه السيارة";
                            lblOverhaulStatus.Style.Add("color", "#ff9800");
                            divOverhaulStatus.Attributes.Add("class", "overhaul-status overhaul-info-text");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading overhaul info: {ex.Message}");
            }
        }

        protected void BtnSaveEngineOverhaul_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(txtEngineOverhaulDate.Text) || string.IsNullOrEmpty(txtEngineOverhaulOdometer.Text))
                {
                    ShowMessage("الرجاء إدخال تاريخ وقراءة عداد العمرة");
                    return;
                }

                DateTime overhaulDate = Convert.ToDateTime(txtEngineOverhaulDate.Text);
                int engineOverhaulOdometer = Convert.ToInt32(txtEngineOverhaulOdometer.Text);
                decimal overhaulCost = string.IsNullOrEmpty(txtEngineOverhaulCost.Text) ? 0 : Convert.ToDecimal(txtEngineOverhaulCost.Text);
                string overhaulNotes = txtEngineOverhaulNotes.Text.Trim();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string updateQuery = @"
                        UPDATE main_vehicle_details 
                        SET EngineOverhaulDate = @OverhaulDate,
                            EngineOverhaulOdometer = @OverhaulOdometer,
                            EngineOverhaulCost = @OverhaulCost,
                            EngineOverhaulNotes = @OverhaulNotes
                        WHERE v_id = @VehicleID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OverhaulDate", overhaulDate);
                        cmd.Parameters.AddWithValue("@OverhaulOdometer", engineOverhaulOdometer);
                        cmd.Parameters.AddWithValue("@OverhaulCost", overhaulCost);
                        cmd.Parameters.AddWithValue("@OverhaulNotes", string.IsNullOrEmpty(overhaulNotes) ? DBNull.Value : (object)overhaulNotes);
                        cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            ShowSuccess("تم تسجيل بيانات عمرة الموتور بنجاح");
                            LoadEngineOverhaulInfo(conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message);
            }
        }

        protected void BtnClearEngineOverhaul_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string updateQuery = @"
                        UPDATE main_vehicle_details 
                        SET EngineOverhaulDate = NULL,
                            EngineOverhaulOdometer = NULL,
                            EngineOverhaulCost = NULL,
                            EngineOverhaulNotes = NULL
                        WHERE v_id = @VehicleID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                        cmd.ExecuteNonQuery();
                    }
                }

                txtEngineOverhaulDate.Text = "";
                txtEngineOverhaulOdometer.Text = "";
                txtEngineOverhaulCost.Text = "";
                txtEngineOverhaulNotes.Text = "";
                lblOverhaulStatus.Text = "⚠️ لم يتم تسجيل عمرة موتور لهذه السيارة";
                lblOverhaulStatus.Style.Add("color", "#ff9800");
                divOverhaulStatus.Attributes.Add("class", "overhaul-status overhaul-info-text");
                ShowSuccess("تم مسح بيانات عمرة الموتور");
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message);
            }
        }

        // ===== STATISTICS =====
        private void LoadStatistics(SqlConnection conn)
        {
            try
            {
                string statsQuery = @"
                    SELECT ISNULL(SUM(mh.Cost), 0) AS TotalCost, COUNT(mh.HistoryID) AS MaintenanceCount
                    FROM MaintenanceHistory mh WHERE mh.VehicleID = @VehicleID";

                using (SqlCommand cmd = new SqlCommand(statsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal totalCost = reader["TotalCost"] != DBNull.Value ? Convert.ToDecimal(reader["TotalCost"]) : 0;
                            int count = Convert.ToInt32(reader["MaintenanceCount"]);
                            lblTotalCost.Text = totalCost.ToString("N2");
                            lblMaintenanceCount.Text = count.ToString();
                        }
                        else
                        {
                            lblTotalCost.Text = "0";
                            lblMaintenanceCount.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
                lblTotalCost.Text = "0";
                lblMaintenanceCount.Text = "0";
            }
        }

        // ===== LICENSE =====
        private void LoadCurrentLicense(SqlConnection conn)
        {
            try
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
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int licenseId = Convert.ToInt32(reader["license_id"]);
                            ViewState["CurrentLicenseId"] = licenseId;

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

                            // Show the current license panel
                            pnlCurrentLicense.Visible = true;

                            // Load mandatory insurance for this license
                            LoadMandatoryInsuranceForLicense(conn, licenseId);

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
                            // Hide the current license panel when no license exists
                            pnlCurrentLicense.Visible = false;
                            pnlMandatoryInsuranceInfo.Visible = false;
                            pnlActiveLicenseWarning.Visible = false;
                           // pnlNoLicense.Visible = true;
                            btnNewLicense.Visible = true;
                            ViewState["CurrentLicenseId"] = null;
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current license: {ex.Message}");
                pnlCurrentLicense.Visible = false;
            }
        }

        private void LoadMandatoryInsuranceForLicense(SqlConnection conn, int licenseId)
        {
            try
            {
                string query = @"
                    SELECT TOP 1 ci.company_name, ci.cost, ci.notes
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

        private void LoadMandatoryInsuranceForCurrentLicense(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT TOP 1 ci.company_name, ci.cost, ci.notes
                    FROM car_insurance ci
                    INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                    WHERE vi.vehicle_id = @VehicleId 
                      AND ci.insurance_name = N'تأمين إجباري'
                      AND (ci.deleted = 0 OR ci.deleted IS NULL)
                    ORDER BY ci.reg_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
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

        private void LoadLicenseHistory(SqlConnection conn)
        {
            try
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
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvLicenseHistory.DataSource = dt;
                    gvLicenseHistory.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "licenseCount",
                        $"document.getElementById('licenseCount').textContent = '{count}';", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading license history: {ex.Message}");
            }
        }

        protected void BtnNewLicense_Click(object sender, EventArgs e)
        {
            if (!CanAddNewLicense())
            {
                ShowMessage("❌ لا يمكن إضافة ترخيص جديد. يوجد ترخيص ساري حتى تاريخ لاحق.");
                return;
            }
            pnlLicenseForm.Visible = true;
            btnNewLicense.Visible = false;
            lblFormTitle.Text = "إضافة ترخيص جديد";
            ClearLicenseForm();
        }

        private bool CanAddNewLicense()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT COUNT(*) FROM vehicle_licenses 
                    WHERE vehicle_id = @VehicleId AND expiry_date > GETDATE() AND (deleted = 0 OR deleted IS NULL)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) == 0;
                }
            }
        }

        protected void BtnSaveLicense_Click(object sender, EventArgs e)
        {
            ViewState["ActiveTab"] = "tabLicense";
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (!CanAddNewLicense())
                {
                    ShowMessage("❌ لا يمكن إضافة ترخيص جديد. يوجد ترخيص ساري حتى تاريخ لاحق.");
                    return;
                }

                if (!ValidateLicenseForm()) return;

                using (SqlConnection conn = new SqlConnection(_connectionString))
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
                                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@LicenseNumber", txtLicenseNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@LicenseTypeId", string.IsNullOrEmpty(ddlLicenseType.SelectedValue) ? DBNull.Value : (object)Convert.ToInt32(ddlLicenseType.SelectedValue));
                                cmd.Parameters.AddWithValue("@IssueDate", issueDate);
                                cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                                cmd.Parameters.AddWithValue("@TrafficUnitId", string.IsNullOrEmpty(ddlTrafficUnit.SelectedValue) ? DBNull.Value : (object)Convert.ToInt32(ddlTrafficUnit.SelectedValue));
                                cmd.Parameters.AddWithValue("@RenewalCost", string.IsNullOrEmpty(txtRenewCost.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtRenewCost.Text));
                                cmd.Parameters.AddWithValue("@RegistrationNumber", string.IsNullOrEmpty(txtRegistrationNumber.Text) ? DBNull.Value : (object)txtRegistrationNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@TechInspectionDate", string.IsNullOrEmpty(txtTechInspectionDate.Text) ? DBNull.Value : (object)DateTime.Parse(txtTechInspectionDate.Text));
                                cmd.Parameters.AddWithValue("@TechInspectionExpiry", string.IsNullOrEmpty(txtTechInspectionExpiry.Text) ? DBNull.Value : (object)DateTime.Parse(txtTechInspectionExpiry.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtLicenseNotes.Text) ? DBNull.Value : (object)txtLicenseNotes.Text.Trim());
                                newLicenseId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (!string.IsNullOrEmpty(ddlMandatoryInsurance.SelectedValue))
                            {
                                SaveMandatoryInsurance(conn, transaction, _currentVehicleId, issueDate, expiryDate);
                            }

                            transaction.Commit();

                            ShowSuccess("✅ تم حفظ الترخيص" + (string.IsNullOrEmpty(ddlMandatoryInsurance.SelectedValue) ? "" : " والتأمين الإجباري") + " بنجاح");
                            ClearLicenseForm();
                            LoadCurrentLicense(conn);
                            LoadLicenseHistory(conn);
                            LoadMandatoryInsuranceForCurrentLicense(conn);
                            pnlLicenseForm.Visible = false;

                            // Update counts
                            UpdateTabCounts();
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

        protected void BtnCancelForm_Click(object sender, EventArgs e)
        {
            pnlLicenseForm.Visible = false;
            btnNewLicense.Visible = true;
            ClearLicenseForm();
        }

        private void ClearLicenseForm()
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
            txtLicenseNotes.Text = "";
            ddlMandatoryInsurance.SelectedValue = "";
            txtMandatoryInsuranceCost.Text = "";
            txtMandatoryInsuranceNotes.Text = "";
        }

        protected void ViewLicense_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int licenseId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
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

                string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
                string script = $"showLicenseDetailsWithInsurance({json});";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
            }
        }

        protected void DeleteLicense_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int licenseId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Get the vehicle_id for this license first
                        int vehicleId = 0;
                        string getVehicleQuery = "SELECT vehicle_id FROM vehicle_licenses WHERE license_id = @LicenseId";
                        using (SqlCommand getCmd = new SqlCommand(getVehicleQuery, conn, transaction))
                        {
                            getCmd.Parameters.AddWithValue("@LicenseId", licenseId);
                            object result = getCmd.ExecuteScalar();
                            if (result != null)
                                vehicleId = Convert.ToInt32(result);
                        }

                        // Soft delete the license
                        string deleteLicenseQuery = "UPDATE vehicle_licenses SET deleted = 1 WHERE license_id = @LicenseId";
                        using (SqlCommand cmd = new SqlCommand(deleteLicenseQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@LicenseId", licenseId);
                            cmd.ExecuteNonQuery();
                        }

                        // Delete mandatory insurance associated with this license
                        if (vehicleId > 0)
                        {
                            // First, find the mandatory insurance IDs for this vehicle
                            string getInsuranceIds = @"
                        SELECT ci.insurance_id 
                        FROM car_insurance ci
                        INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                        WHERE vi.vehicle_id = @VehicleId 
                          AND ci.insurance_name = N'تأمين إجباري'
                          AND (ci.deleted = 0 OR ci.deleted IS NULL)";

                            List<int> insuranceIds = new List<int>();
                            using (SqlCommand cmd = new SqlCommand(getInsuranceIds, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        insuranceIds.Add(Convert.ToInt32(reader["insurance_id"]));
                                    }
                                }
                            }

                            // Soft delete each mandatory insurance
                            foreach (int insId in insuranceIds)
                            {
                                string deleteInsuranceQuery = "UPDATE car_insurance SET deleted = 1 WHERE insurance_id = @InsuranceId";
                                using (SqlCommand cmd = new SqlCommand(deleteInsuranceQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@InsuranceId", insId);
                                    cmd.ExecuteNonQuery();
                                }

                                // Also remove from vehicle_insurance junction table
                                string deleteJunctionQuery = "DELETE FROM vehicle_insurance WHERE insurance_id = @InsuranceId AND vehicle_id = @VehicleId";
                                using (SqlCommand cmd = new SqlCommand(deleteJunctionQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@InsuranceId", insId);
                                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();

                        // Clear the license display
                        pnlCurrentLicense.Visible = false;
                        pnlMandatoryInsuranceInfo.Visible = false;
                        pnlActiveLicenseWarning.Visible = false;
                        btnNewLicense.Visible = true;
                        ViewState["CurrentLicenseId"] = null;

                        // Reset all labels to default values - License
                        lblCurrentLicenseNumber.Text = "-";
                        lblCurrentLicenseType.Text = "-";
                        lblCurrentIssueDate.Text = "-";
                        lblCurrentExpiryDate.Text = "-";
                        lblCurrentTrafficUnit.Text = "-";
                        lblCurrentRenewCost.Text = "-";
                        lblCurrentRegistrationNumber.Text = "-";
                        lblCurrentNotes.Text = "-";

                        // Reset all mandatory insurance labels
                        lblMandatoryInsuranceCompany.Text = "-";
                        lblMandatoryInsuranceCost.Text = "-";
                        lblMandatoryInsuranceNotes.Text = "-";

                        ShowSuccess("✅ تم حذف الترخيص والتأمين الإجباري المرتبط به بنجاح");

                        // Reload data
                        LoadLicenseHistory(conn);
                        UpdateTabCounts();
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
            gvLicenseHistory.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadLicenseHistory(conn);
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

        // ===== INSURANCE =====
        private void LoadInsuranceHistory(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT ci.insurance_id, ci.company_name, ci.start_date, ci.end_date, ci.cost, ci.notes
                    FROM car_insurance ci
                    INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                    WHERE vi.vehicle_id = @VehicleId
                      AND (ci.insurance_name = N'تأمين شامل' OR ci.insurance_name = N'تأمين تاملي' OR ci.insurance_name IS NULL)
                    ORDER BY ci.end_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvInsuranceHistory.DataSource = dt;
                    gvInsuranceHistory.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "insuranceCount",
                        $"document.getElementById('insuranceCount').textContent = '{count}';", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading insurance history: {ex.Message}");
            }
        }
        protected void BtnSaveInsurance_Click(object sender, EventArgs e)
        {
            ViewState["ActiveTab"] = "tabMaintenance";
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(ddlInsuranceCompany.SelectedValue) ||
                    string.IsNullOrEmpty(txtInsuranceStartDate.Text) ||
                    string.IsNullOrEmpty(txtInsuranceEndDate.Text))
                {
                    ShowMessage("الرجاء إدخال جميع الحقول المطلوبة (شركة التأمين، تاريخ البداية، تاريخ النهاية)");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string insertInsurance = @"
                        INSERT INTO car_insurance (
                            company_name, insurance_name, start_date, end_date, cost, 
                            notes, reg_date, deleted, hide
                        ) VALUES (
                            @CompanyName, N'تأمين شامل', @StartDate, @EndDate, @Cost,
                            @Notes, GETDATE(), 0, 0
                        );
                        SELECT SCOPE_IDENTITY();";

                            int insuranceId;
                            using (SqlCommand cmd = new SqlCommand(insertInsurance, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CompanyName", ddlInsuranceCompany.SelectedValue);
                                cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtInsuranceStartDate.Text));
                                cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtInsuranceEndDate.Text));
                                cmd.Parameters.AddWithValue("@Cost", string.IsNullOrEmpty(txtInsuranceCost.Text) ? 0 : Convert.ToDecimal(txtInsuranceCost.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtInsuranceNotes.Text) ? DBNull.Value : (object)txtInsuranceNotes.Text.Trim());
                                insuranceId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            string insertJunction = @"
                        INSERT INTO vehicle_insurance (vehicle_id, insurance_id, created_date)
                        VALUES (@VehicleId, @InsuranceId, GETDATE())";

                            using (SqlCommand cmd = new SqlCommand(insertJunction, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@InsuranceId", insuranceId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            ShowSuccess("تم حفظ التأمين الشامل بنجاح");
                            LoadInsuranceHistory(conn);
                            ClearInsuranceForm();

                            // Update counts
                            UpdateTabCounts();
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

        protected void BtnClearInsurance_Click(object sender, EventArgs e)
        {
            ClearInsuranceForm();
        }

        private void ClearInsuranceForm()
        {
            ddlInsuranceCompany.SelectedValue = "";
            txtInsuranceCost.Text = "";
            txtInsuranceStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtInsuranceEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtInsuranceNotes.Text = "";
        }

        protected void GvInsuranceHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInsuranceHistory.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadInsuranceHistory(conn);
            }
        }

        // ===== MAINTENANCE =====
        private void LoadMaintenanceHistory(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT 
                        mh.ServiceDate,
                        vm.MaintenanceType,
                        mh.OdometerReading,
                        ISNULL(mh.Cost, 0) AS Cost,
                        mh.HistoryID
                    FROM MaintenanceHistory mh
                    INNER JOIN VehicleMaintenance vm ON mh.HistoryID = vm.history
                    WHERE mh.VehicleID = @VehicleID
                    ORDER BY mh.ServiceDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("PartsNames", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        int historyId = Convert.ToInt32(row["HistoryID"]);
                        string partsNames = GetMaintenanceParts(historyId);
                        row["PartsNames"] = partsNames;
                    }

                    dt.Columns.Remove("HistoryID");

                    gvMaintenanceHistory.DataSource = dt;
                    gvMaintenanceHistory.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "maintenanceCount",
                        $"document.getElementById('maintenanceCount').textContent = '{count}';", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading maintenance history: {ex.Message}");
                gvMaintenanceHistory.EmptyDataText = "حدث خطأ في تحميل البيانات";
                gvMaintenanceHistory.DataSource = null;
                gvMaintenanceHistory.DataBind();
            }
        }

        private string GetMaintenanceParts(int historyId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT STRING_AGG(mp.PartName, N', ') AS PartsNames
                        FROM MaintenanceHistoryParts mhp
                        INNER JOIN MaintenanceParts mp ON mhp.PartID = mp.PartID
                        WHERE mhp.HistoryID = @HistoryID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HistoryID", historyId);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value ? result.ToString() : "لا توجد قطع غيار";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting maintenance parts: {ex.Message}");
                return "خطأ في تحميل البيانات";
            }
        }

        private string GetPartNamesFromIds(string[] partIds)
        {
            if (partIds == null || partIds.Length == 0)
                return "";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var partNames = new List<string>();
                    foreach (string partId in partIds)
                    {
                        if (!string.IsNullOrEmpty(partId))
                        {
                            string query = "SELECT PartName FROM MaintenanceParts WHERE PartID = @PartID";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@PartID", Convert.ToInt32(partId));
                                object result = cmd.ExecuteScalar();
                                if (result != null)
                                    partNames.Add(result.ToString());
                            }
                        }
                    }
                    return string.Join(", ", partNames);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting part names: {ex.Message}");
                return "";
            }
        }

        protected void BtnSaveMaintenance_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(ddlMaintenanceType.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار نوع الصيانة");
                    return;
                }

                int newOdometer = 0;
                if (!string.IsNullOrEmpty(txtOdometerReading.Text))
                {
                    if (!int.TryParse(txtOdometerReading.Text, out newOdometer) || newOdometer < 0)
                    {
                        ShowMessage("الرجاء إدخال قراءة عداد صحيحة");
                        return;
                    }
                }
                else
                {
                    ShowMessage("الرجاء إدخال قراءة العداد");
                    return;
                }

                int lastWorkOrderKM = 0;
                if (ViewState["LastWorkOrderKM"] != null)
                {
                    lastWorkOrderKM = Convert.ToInt32(ViewState["LastWorkOrderKM"]);
                }

                if (lastWorkOrderKM == 0)
                {
                    lastWorkOrderKM = GetLastWorkOrderKilometer(_currentCarNumberId);
                }

                if (lastWorkOrderKM > 0 && newOdometer > lastWorkOrderKM)
                {
                    ShowMessage($"خطأ: قراءة العداد المدخلة ({newOdometer:N0} كم) لا يمكن أن تتجاوز آخر قراءة مسجلة في أوامر الشغل ({lastWorkOrderKM:N0} كم)");
                    return;
                }

                string[] partIds = hfPartsData.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string partNames = GetPartNamesFromIds(partIds);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string insertHistory = @"
                        INSERT INTO MaintenanceHistory (VehicleID, ServiceDate, OdometerReading, Cost, Notes, CreatedDate)
                        VALUES (@VehicleID, @ServiceDate, @OdometerReading, @Cost, @Notes, @CreatedDate);
                        SELECT SCOPE_IDENTITY();";

                            int historyId;
                            using (SqlCommand cmd = new SqlCommand(insertHistory, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@ServiceDate", DateTime.Parse(txtMaintenanceDate.Text));
                                cmd.Parameters.AddWithValue("@OdometerReading", newOdometer);
                                cmd.Parameters.AddWithValue("@Cost", string.IsNullOrEmpty(txtMaintenanceCost.Text) ? 0 : Convert.ToDecimal(txtMaintenanceCost.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtMaintenanceNotes.Text) ? "صيانة دورية" : txtMaintenanceNotes.Text.Trim());
                                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                historyId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (partIds.Length > 0)
                            {
                                foreach (string partId in partIds)
                                {
                                    if (!string.IsNullOrEmpty(partId))
                                    {
                                        string insertPart = @"
                                    INSERT INTO MaintenanceHistoryParts (HistoryID, PartID, CreatedDate)
                                    VALUES (@HistoryID, @PartID, @CreatedDate)";

                                        using (SqlCommand partCmd = new SqlCommand(insertPart, conn, transaction))
                                        {
                                            partCmd.Parameters.AddWithValue("@HistoryID", historyId);
                                            partCmd.Parameters.AddWithValue("@PartID", Convert.ToInt32(partId));
                                            partCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                            partCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            string serviceType = ddlMaintenanceType.SelectedValue == "صيانة دورية" ? "1" :
                                                 ddlMaintenanceType.SelectedValue == "اعطال" ? "2" : "3";

                            string insertMaintenance = @"
                        INSERT INTO VehicleMaintenance (VehicleID, MaintenanceDate, MaintenanceType, TotalCost, Notes, service_type, history, CreatedDate)
                        VALUES (@VehicleID, @MaintenanceDate, @MaintenanceType, @TotalCost, @Notes, @ServiceType, @HistoryID, @CreatedDate);
                        SELECT SCOPE_IDENTITY();";

                            int maintenanceId;
                            using (SqlCommand cmd = new SqlCommand(insertMaintenance, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@MaintenanceDate", DateTime.Parse(txtMaintenanceDate.Text));
                                cmd.Parameters.AddWithValue("@MaintenanceType", ddlMaintenanceType.SelectedValue);
                                cmd.Parameters.AddWithValue("@TotalCost", string.IsNullOrEmpty(txtMaintenanceCost.Text) ? 0 : Convert.ToDecimal(txtMaintenanceCost.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtMaintenanceNotes.Text) ? "صيانة دورية" : txtMaintenanceNotes.Text.Trim());
                                cmd.Parameters.AddWithValue("@ServiceType", serviceType);
                                cmd.Parameters.AddWithValue("@HistoryID", historyId);
                                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                maintenanceId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            string insertJunction = "INSERT INTO vehicle_maintenance (vehicle_id, maintenance_id, created_date) VALUES (@VehicleId, @MaintenanceId, GETDATE())";
                            using (SqlCommand jCmd = new SqlCommand(insertJunction, conn, transaction))
                            {
                                jCmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                jCmd.Parameters.AddWithValue("@MaintenanceId", maintenanceId);
                                jCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            ShowSuccess($"تم حفظ بيانات الصيانة بنجاح مع {partIds.Length} قطعة غيار");
                            ClearMaintenanceForm();
                            LoadMaintenanceHistory(conn);
                            LoadStatistics(conn);

                            // Update counts
                            UpdateTabCounts();
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

        protected void BtnClearMaintenance_Click(object sender, EventArgs e)
        {
            ClearMaintenanceForm();
        }

        private void ClearMaintenanceForm()
        {
            ddlMaintenanceType.SelectedValue = "";
            txtOdometerReading.Text = "";
            txtMaintenanceCost.Text = "";
            txtMaintenanceNotes.Text = "";
            hfPartsData.Value = "";

            // Clear parts container via JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearParts",
                "if(document.getElementById('partRowsContainer')) { document.getElementById('partRowsContainer').innerHTML = ''; } if(typeof addPartRow === 'function') { addPartRow(); }", true);
        }

        protected void GvMaintenanceHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMaintenanceHistory.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadMaintenanceHistory(conn);
            }
        }

        // ===== SUBSCRIBERS =====
        private void LoadSubscriberHistory(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT 
                        vs.subscription_id,
                        sd.name AS subscriber_name,
                        r.route_name,
                        r.start_loc,
                        r.end_loc,
                        r.KM,
                        vs.start_date,
                        vs.end_date,
                        vs.cost
                    FROM vehicle_subscriptions vs
                    INNER JOIN subscription_details sd ON vs.subscriber_id = sd.n_id
                    INNER JOIN routes r ON vs.route_id = r.route_id
                    WHERE vs.vehicle_id = @VehicleId
                    ORDER BY vs.created_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvSubscriberHistory.DataSource = dt;
                    gvSubscriberHistory.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "subscriberCount",
                        $"document.getElementById('subscriberCount').textContent = '{count}';", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscriber history: {ex.Message}");
                gvSubscriberHistory.EmptyDataText = "لا توجد اشتراكات سابقة";
                gvSubscriberHistory.DataSource = null;
                gvSubscriberHistory.DataBind();
            }
        }

        protected void ddlRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlRoute.SelectedValue))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        string query = @"
                            SELECT 
                                route_name, 
                                description, 
                                start_loc, 
                                end_loc, 
                                KM, 
                                direction,
                                driver_name,
                                driver_phone
                            FROM routes 
                            WHERE route_id = @RouteId";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@RouteId", ddlRoute.SelectedValue);
                        conn.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string routeDetails = $"<strong>اسم المسار:</strong> {reader["route_name"]}<br />";
                            if (reader["start_loc"] != DBNull.Value && reader["end_loc"] != DBNull.Value)
                            {
                                routeDetails += $"<strong>المسار:</strong> {reader["start_loc"]} → {reader["end_loc"]}<br />";
                            }
                            if (reader["KM"] != DBNull.Value)
                            {
                                routeDetails += $"<strong>المسافة:</strong> {Convert.ToDecimal(reader["KM"]):N0} كم<br />";
                            }
                            if (reader["direction"] != DBNull.Value)
                            {
                                routeDetails += $"<strong>الاتجاه:</strong> {reader["direction"]}<br />";
                            }
                            if (reader["driver_name"] != DBNull.Value)
                            {
                                routeDetails += $"<strong>السائق:</strong> {reader["driver_name"]}<br />";
                            }
                            if (reader["driver_phone"] != DBNull.Value)
                            {
                                routeDetails += $"<strong>هاتف السائق:</strong> {reader["driver_phone"]}<br />";
                            }
                            if (reader["description"] != DBNull.Value)
                            {
                                routeDetails += $"<strong>الوصف:</strong> {reader["description"]}";
                            }

                            lblRouteDetails.Text = routeDetails;
                            pnlRouteDetails.Visible = true;
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                pnlRouteDetails.Visible = false;
            }
        }

        protected void BtnSaveSubscriber_Click(object sender, EventArgs e)
        {
            ViewState["ActiveTab"] = "tabSubscribers";
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(ddlSubscriber.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار المشترك");
                    return;
                }

                if (string.IsNullOrEmpty(ddlRoute.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار المسار");
                    return;
                }

                DateTime startDate, endDate;
                if (!DateTime.TryParse(txtSubStartDate.Text, out startDate))
                {
                    startDate = DateTime.Now;
                }
                if (!DateTime.TryParse(txtSubEndDate.Text, out endDate))
                {
                    endDate = DateTime.Now.AddYears(1);
                }

                if (endDate < startDate)
                {
                    ShowMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
                    return;
                }

                double cost = string.IsNullOrEmpty(txtSubCost.Text) ? 0 : Convert.ToDouble(txtSubCost.Text);
                string notes = txtSubNotes.Text.Trim();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int subscriberId = Convert.ToInt32(ddlSubscriber.SelectedValue);
                            int routeId = Convert.ToInt32(ddlRoute.SelectedValue);

                            string insertSubscription = @"
                        INSERT INTO vehicle_subscriptions 
                        (vehicle_id, subscriber_id, route_id, start_date, end_date, cost, notes, created_date, is_active)
                        VALUES 
                        (@VehicleId, @SubscriberId, @RouteId, @StartDate, @EndDate, @Cost, @Notes, GETDATE(), 1);
                        SELECT SCOPE_IDENTITY();";

                            int newSubscriptionId;

                            using (SqlCommand cmd = new SqlCommand(insertSubscription, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@SubscriberId", subscriberId);
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.Parameters.AddWithValue("@StartDate", startDate);
                                cmd.Parameters.AddWithValue("@EndDate", endDate);
                                cmd.Parameters.AddWithValue("@Cost", cost);
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes);

                                newSubscriptionId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            transaction.Commit();

                            string subscriberName = ddlSubscriber.SelectedItem.Text;

                            LoadSubscriberHistory(conn);

                            ddlRoute.SelectedIndex = 0;
                            pnlRouteDetails.Visible = false;
                            txtSubCost.Text = "";
                            txtSubNotes.Text = "";
                            SetDefaultDates();

                            ShowSuccess($"تم تسجيل اشتراك {subscriberName} في المسار بنجاح");

                            // Update counts
                            UpdateTabCounts();
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

        protected void BtnClearSubscriber_Click(object sender, EventArgs e)
        {
            ClearSubscriberForm();
        }

        private void ClearSubscriberForm()
        {
            ddlSubscriber.SelectedValue = "";
            ddlRoute.SelectedValue = "";
            txtSubStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtSubEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtSubCost.Text = "";
            txtSubNotes.Text = "";
            pnlRouteDetails.Visible = false;
        }

        protected void GvSubscriberHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscriberHistory.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadSubscriberHistory(conn);
            }
        }

        // ===== ROUTES =====
        private void LoadAllRoutes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT route_id, route_name, description, start_loc, end_loc, 
                               KM, direction, subscription_count, is_active,
                               start_date, end_date, driver_name, driver_phone
                        FROM routes 
                        ORDER BY route_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptAllRoutes.DataSource = dt;
                    rptAllRoutes.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalCount",
                        $"if(document.getElementById('totalRoutesCount')) document.getElementById('totalRoutesCount').textContent = '{count}';", true);

                    lblNoRoutes.Visible = dt.Rows.Count == 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading all routes: {ex.Message}");
                lblNoRoutes.Visible = true;
                lblNoRoutes.Text = "حدث خطأ في تحميل المسارات";
            }
        }

        private void LoadVehicleRoutes(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT DISTINCT 
                        r.route_id, r.route_name, r.description, r.start_loc, r.end_loc, 
                        r.KM, r.direction, r.subscription_count, r.is_active,
                        r.start_date, r.end_date, r.driver_name, r.driver_phone
                    FROM routes r
                    INNER JOIN vehicle_route_assignment vra ON r.route_id = vra.route_id
                    WHERE vra.vehicle_id = @VehicleId
                    ORDER BY r.start_date DESC, r.route_name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvVehicleRoutes.DataSource = dt;
                    gvVehicleRoutes.DataBind();

                    int count = dt.Rows.Count;
                    ScriptManager.RegisterStartupScript(this, GetType(), "routesCount",
                        $"document.getElementById('routesCount').textContent = '{count}';", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading vehicle routes: {ex.Message}");
                gvVehicleRoutes.EmptyDataText = "لا توجد مسارات لهذه السيارة";
                gvVehicleRoutes.DataSource = null;
                gvVehicleRoutes.DataBind();
            }
        }

        protected void BtnSaveRoute_Click(object sender, EventArgs e)
        {
            ViewState["ActiveTab"] = "tabRoutes";
            try
            {
                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(ddlRouteName.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار اسم المسار");
                    return;
                }

                if (string.IsNullOrEmpty(ddlStartLoc.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار نقطة البداية");
                    return;
                }

                if (string.IsNullOrEmpty(ddlEndLoc.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار نقطة النهاية");
                    return;
                }

                if (string.IsNullOrEmpty(txtRouteKM.Text))
                {
                    ShowMessage("الرجاء إدخال المسافة");
                    return;
                }

                DateTime startDate, endDate;
                if (!DateTime.TryParse(txtRouteStartDate.Text, out startDate))
                {
                    startDate = DateTime.Now;
                }

                if (!DateTime.TryParse(txtRouteEndDate.Text, out endDate))
                {
                    endDate = DateTime.Now.AddYears(1);
                }

                if (endDate < startDate)
                {
                    ShowMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            decimal km = Convert.ToDecimal(txtRouteKM.Text);
                            int subscriptionCount = string.IsNullOrEmpty(txtSubscriptionCount.Text) ? 0 : Convert.ToInt32(txtSubscriptionCount.Text);

                            string insertRoute = @"
                        INSERT INTO routes (route_name, description, start_loc, end_loc, KM, direction, subscription_count, is_active, start_date, end_date, driver_name, driver_phone)
                        VALUES (@RouteName, @Description, @StartLoc, @EndLoc, @KM, @Direction, @SubscriptionCount, 1, @StartDate, @EndDate, @DriverName, @DriverPhone);
                        SELECT SCOPE_IDENTITY();";

                            int routeId;
                            using (SqlCommand cmd = new SqlCommand(insertRoute, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RouteName", ddlRouteName.SelectedValue);
                                cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(txtRouteDescription.Text) ? (object)DBNull.Value : txtRouteDescription.Text.Trim());
                                cmd.Parameters.AddWithValue("@StartLoc", ddlStartLoc.SelectedValue);
                                cmd.Parameters.AddWithValue("@EndLoc", ddlEndLoc.SelectedValue);
                                cmd.Parameters.AddWithValue("@KM", km);
                                cmd.Parameters.AddWithValue("@Direction", string.IsNullOrEmpty(txtRouteDirection.Text) ? (object)DBNull.Value : txtRouteDirection.Text.Trim());
                                cmd.Parameters.AddWithValue("@SubscriptionCount", subscriptionCount);
                                cmd.Parameters.AddWithValue("@StartDate", startDate);
                                cmd.Parameters.AddWithValue("@EndDate", endDate);
                                cmd.Parameters.AddWithValue("@DriverName", string.IsNullOrEmpty(txtRouteDriverName.Text) ? (object)DBNull.Value : txtRouteDriverName.Text.Trim());
                                cmd.Parameters.AddWithValue("@DriverPhone", string.IsNullOrEmpty(txtRouteDriverPhone.Text) ? (object)DBNull.Value : txtRouteDriverPhone.Text.Trim());

                                routeId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            string assignRouteToVehicle = @"
                        INSERT INTO vehicle_route_assignment (vehicle_id, route_id, assigned_date)
                        VALUES (@VehicleId, @RouteId, GETDATE())";

                            using (SqlCommand cmd = new SqlCommand(assignRouteToVehicle, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LoadVehicleRoutes(conn);
                            LoadAllRoutes();
                            ClearRouteForm();
                            ShowSuccess("تم حفظ المسار بنجاح وربطه بالسيارة");

                            // Update counts
                            UpdateTabCounts();
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

        private void UpdateRoute(int routeId, string routeName, string description, string startLoc, string endLoc, decimal km, string direction, int subscriptionCount, bool isActive, DateTime startDate, DateTime endDate, string driverName, string driverPhone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        UPDATE routes 
                        SET route_name = @RouteName,
                            description = @Description,
                            start_loc = @StartLoc,
                            end_loc = @EndLoc,
                            KM = @KM,
                            direction = @Direction,
                            subscription_count = @SubscriptionCount,
                            is_active = @IsActive,
                            start_date = @StartDate,
                            end_date = @EndDate,
                            driver_name = @DriverName,
                            driver_phone = @DriverPhone
                        WHERE route_id = @RouteId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RouteId", routeId);
                        cmd.Parameters.AddWithValue("@RouteName", routeName);
                        cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                        cmd.Parameters.AddWithValue("@StartLoc", startLoc);
                        cmd.Parameters.AddWithValue("@EndLoc", endLoc);
                        cmd.Parameters.AddWithValue("@KM", km);
                        cmd.Parameters.AddWithValue("@Direction", string.IsNullOrEmpty(direction) ? (object)DBNull.Value : direction);
                        cmd.Parameters.AddWithValue("@SubscriptionCount", subscriptionCount);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@DriverName", string.IsNullOrEmpty(driverName) ? (object)DBNull.Value : driverName);
                        cmd.Parameters.AddWithValue("@DriverPhone", string.IsNullOrEmpty(driverPhone) ? (object)DBNull.Value : driverPhone);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadAllRoutes();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    LoadVehicleRoutes(conn);
                }

                ShowSuccess("تم تحديث المسار بنجاح");
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeModal();", true);
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحديث المسار: " + ex.Message);
            }
        }

        protected void EditRoute_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int routeId = Convert.ToInt32(btn.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT route_id, route_name, description, start_loc, end_loc, 
                               KM, direction, subscription_count, is_active,
                               start_date, end_date, driver_name, driver_phone
                        FROM routes 
                        WHERE route_id = @RouteId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RouteId", routeId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string routeName = reader["route_name"].ToString();
                                string description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "";
                                string startLoc = reader["start_loc"].ToString();
                                string endLoc = reader["end_loc"].ToString();
                                decimal km = Convert.ToDecimal(reader["KM"]);
                                string direction = reader["direction"] != DBNull.Value ? reader["direction"].ToString() : "";
                                int subscriptionCount = Convert.ToInt32(reader["subscription_count"]);
                                bool isActive = Convert.ToBoolean(reader["is_active"]);
                                DateTime startDate = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]) : DateTime.Now;
                                DateTime endDate = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]) : DateTime.Now.AddYears(1);
                                string driverName = reader["driver_name"] != DBNull.Value ? reader["driver_name"].ToString() : "";
                                string driverPhone = reader["driver_phone"] != DBNull.Value ? reader["driver_phone"].ToString() : "";

                                string script = $"openEditModal('{routeId}', '{EscapeJsString(routeName)}', '{EscapeJsString(description)}', '{startLoc}', '{endLoc}', '{km}', '{EscapeJsString(direction)}', '{subscriptionCount}', '{isActive}', '{startDate:yyyy-MM-dd}', '{endDate:yyyy-MM-dd}', '{EscapeJsString(driverName)}', '{EscapeJsString(driverPhone)}');";
                                ScriptManager.RegisterStartupScript(this, GetType(), "OpenEditModal", script, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحميل بيانات المسار: " + ex.Message);
            }
        }

        private string EscapeJsString(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }

        protected void DeleteRoute_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int routeId = Convert.ToInt32(btn.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string checkQuery = @"
                                SELECT COUNT(*) 
                                FROM subscription_details 
                                WHERE route_id = @RouteId";

                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@RouteId", routeId);
                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    ShowMessage("لا يمكن حذف هذا المسار لأنه مرتبط باشتراكات. يرجى إلغاء ربط المسار أولاً.");
                                    return;
                                }
                            }

                            string deleteAssignment = "DELETE FROM vehicle_route_assignment WHERE route_id = @RouteId";
                            using (SqlCommand cmd = new SqlCommand(deleteAssignment, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            string deleteRoute = "DELETE FROM routes WHERE route_id = @RouteId";
                            using (SqlCommand cmd = new SqlCommand(deleteRoute, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LoadAllRoutes();
                            LoadVehicleRoutes(conn);

                            ShowSuccess("تم حذف المسار بنجاح");
                            UpdateTabCounts();
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
                ShowMessage("حدث خطأ أثناء حذف المسار: " + ex.Message);
            }
        }

        protected void LinkRoute_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                int routeId = Convert.ToInt32(btn.CommandArgument);

                if (_currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string checkQuery = @"
                                SELECT COUNT(*) 
                                FROM vehicle_route_assignment 
                                WHERE route_id = @RouteId AND vehicle_id = @VehicleId";

                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@RouteId", routeId);
                                checkCmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);

                                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (exists > 0)
                                {
                                    ShowMessage("هذا المسار مرتبط بالفعل بهذه السيارة");
                                    return;
                                }
                            }

                            string assignRoute = @"
                                INSERT INTO vehicle_route_assignment (vehicle_id, route_id, assigned_date)
                                VALUES (@VehicleId, @RouteId, GETDATE())";

                            using (SqlCommand cmd = new SqlCommand(assignRoute, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LoadVehicleRoutes(conn);

                            string routeName = "";
                            string getNameQuery = "SELECT route_name FROM routes WHERE route_id = @RouteId";
                            using (SqlCommand getNameCmd = new SqlCommand(getNameQuery, conn))
                            {
                                getNameCmd.Parameters.AddWithValue("@RouteId", routeId);
                                object result = getNameCmd.ExecuteScalar();
                                if (result != null) routeName = result.ToString();
                            }

                            ShowSuccess($"تم ربط المسار '{routeName}' بالسيارة بنجاح");
                            UpdateTabCounts();
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
                ShowMessage("حدث خطأ أثناء ربط المسار: " + ex.Message);
            }
        }

        protected void rptAllRoutes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                LinkButton btnLink = (LinkButton)e.Item.FindControl("btnLinkRoute");
                LinkButton btnEdit = (LinkButton)e.Item.FindControl("btnEditRoute");
                LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDeleteRoute");

                Label lblStartDate = (Label)e.Item.FindControl("lblStartDate");
                Label lblEndDate = (Label)e.Item.FindControl("lblEndDate");
                Label lblDriverName = (Label)e.Item.FindControl("lblDriverName");
                Label lblDriverPhone = (Label)e.Item.FindControl("lblDriverPhone");
                Label lblStatusDate = (Label)e.Item.FindControl("lblStatusDate");

                if (lblStartDate != null && row["start_date"] != DBNull.Value)
                {
                    lblStartDate.Text = Convert.ToDateTime(row["start_date"]).ToString("yyyy/MM/dd");
                }

                if (lblEndDate != null && row["end_date"] != DBNull.Value)
                {
                    lblEndDate.Text = Convert.ToDateTime(row["end_date"]).ToString("yyyy/MM/dd");
                }

                if (lblDriverName != null && row["driver_name"] != DBNull.Value)
                {
                    lblDriverName.Text = row["driver_name"].ToString();
                }
                else if (lblDriverName != null)
                {
                    lblDriverName.Text = "غير محدد";
                }

                if (lblDriverPhone != null && row["driver_phone"] != DBNull.Value)
                {
                    lblDriverPhone.Text = row["driver_phone"].ToString();
                }
                else if (lblDriverPhone != null)
                {
                    lblDriverPhone.Text = "غير محدد";
                }

                if (lblStatusDate != null)
                {
                    DateTime now = DateTime.Now;
                    DateTime startDate = row["start_date"] != DBNull.Value ? Convert.ToDateTime(row["start_date"]) : DateTime.Now;
                    DateTime endDate = row["end_date"] != DBNull.Value ? Convert.ToDateTime(row["end_date"]) : DateTime.Now;

                    if (startDate <= now && endDate >= now)
                    {
                        lblStatusDate.Text = "✓ نشط حالياً";
                        lblStatusDate.CssClass = "status-badge-active";
                    }
                    else if (startDate > now)
                    {
                        lblStatusDate.Text = $"⏰ يبدأ في {startDate:yyyy/MM/dd}";
                        lblStatusDate.CssClass = "status-badge-upcoming";
                    }
                    else if (endDate < now)
                    {
                        lblStatusDate.Text = "✗ منتهي";
                        lblStatusDate.CssClass = "status-badge-expired";
                    }
                }

                if (btnLink != null)
                {
                    btnLink.CommandArgument = row["route_id"].ToString();
                }
                if (btnEdit != null)
                {
                    btnEdit.CommandArgument = row["route_id"].ToString();
                }
                if (btnDelete != null)
                {
                    btnDelete.CommandArgument = row["route_id"].ToString();
                }
            }
        }

        protected void BtnClearRoute_Click(object sender, EventArgs e)
        {
            ClearRouteForm();
        }

        protected void BtnBackRoute_Click(object sender, EventArgs e)
        {
            ClearRouteForm();
        }

        private void ClearRouteForm()
        {
            ddlRouteName.SelectedIndex = 0;
            txtRouteDescription.Text = "";
            ddlStartLoc.SelectedIndex = 0;
            ddlEndLoc.SelectedIndex = 0;
            txtRouteKM.Text = "";
            txtRouteDirection.Text = "";
            txtSubscriptionCount.Text = "";
            txtRouteDriverName.Text = "";
            txtRouteDriverPhone.Text = "";
            txtRouteStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtRouteEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
        }

        protected void GvVehicleRoutes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVehicleRoutes.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadVehicleRoutes(conn);
            }
        }

        // ===== HELPER METHODS =====
        // ===== HELPER METHODS =====
        private void UpdateTabCounts()
        {
            // Force a full reload of all counts from the database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // License count
                string licenseQuery = "SELECT COUNT(*) FROM vehicle_licenses WHERE vehicle_id = @VehicleId AND (deleted = 0 OR deleted IS NULL)";
                using (SqlCommand cmd = new SqlCommand(licenseQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    ScriptManager.RegisterStartupScript(this, GetType(), "licenseCount",
                        $"document.getElementById('licenseCount').textContent = '{count}';", true);
                }

                // Insurance count
                string insuranceQuery = @"
            SELECT COUNT(*) 
            FROM car_insurance ci
            INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
            WHERE vi.vehicle_id = @VehicleId 
              AND (ci.insurance_name = N'تأمين شامل' OR ci.insurance_name = N'تأمين تاملي' OR ci.insurance_name IS NULL)
              AND (ci.deleted = 0 OR ci.deleted IS NULL)";
                using (SqlCommand cmd = new SqlCommand(insuranceQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    ScriptManager.RegisterStartupScript(this, GetType(), "insuranceCount",
                        $"document.getElementById('insuranceCount').textContent = '{count}';", true);
                }

                // Maintenance count
                string maintQuery = "SELECT COUNT(*) FROM MaintenanceHistory WHERE VehicleID = @VehicleID";
                using (SqlCommand cmd = new SqlCommand(maintQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", _currentVehicleId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    ScriptManager.RegisterStartupScript(this, GetType(), "maintenanceCount",
                        $"document.getElementById('maintenanceCount').textContent = '{count}';", true);
                }

                // Subscriber count
                string subQuery = "SELECT COUNT(*) FROM vehicle_subscriptions WHERE vehicle_id = @VehicleId";
                using (SqlCommand cmd = new SqlCommand(subQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    ScriptManager.RegisterStartupScript(this, GetType(), "subscriberCount",
                        $"document.getElementById('subscriberCount').textContent = '{count}';", true);
                }

                // Routes count
                string routesQuery = "SELECT COUNT(*) FROM vehicle_route_assignment WHERE vehicle_id = @VehicleId";
                using (SqlCommand cmd = new SqlCommand(routesQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    ScriptManager.RegisterStartupScript(this, GetType(), "routesCount",
                        $"document.getElementById('routesCount').textContent = '{count}';", true);
                }
            }
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
        private void RestoreActiveTab()
        {
            if (ViewState["ActiveTab"] != null)
            {
                string activeTab = ViewState["ActiveTab"].ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "RestoreTab",
                    $"setTimeout(function() {{ var tabs = document.querySelectorAll('.nav-tabs-custom .nav-link'); " +
                    $"tabs.forEach(function(t) {{ t.classList.remove('active'); if (t.getAttribute('data-tab') === '{activeTab}') {{ t.classList.add('active'); }} }}); " +
                    $"var panes = document.querySelectorAll('.tab-pane'); panes.forEach(function(p) {{ p.classList.remove('active'); }}); " +
                    $"var target = document.getElementById('{activeTab}'); if(target) target.classList.add('active'); " +
                    $"document.getElementById('{hfActiveTab.ClientID}').value = '{activeTab}'; " +
                    $"}}, 100);", true);
            }
        }
    }
}