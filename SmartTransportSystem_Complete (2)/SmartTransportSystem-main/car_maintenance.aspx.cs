using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Services;
using System.Collections.Generic;

namespace car_WebApplication1
{
    public partial class car_maintenance : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int currentVehicleId = 0;
        private int currentCarNumberId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlCarInfo.Visible = false;
                pnlCurrentOdometer.Visible = false;
                pnlMaintenanceForm.Visible = false;
                pnlMaintenanceHistory.Visible = false;
                pnlStatistics.Visible = false;
                pnlEngineOverhaul.Visible = false;

                LoadPartsToViewState();
                SetDefaultDates();

                if (Session["VehicleIdToLoad"] != null && Session["CarNumberToLoad"] != null)
                {
                    int vehicleId = Convert.ToInt32(Session["VehicleIdToLoad"]);
                    string carNumber = Session["CarNumberToLoad"].ToString();

                    Session.Remove("VehicleIdToLoad");
                    Session.Remove("CarNumberToLoad");

                    string[] parts = carNumber.Split(' ');
                    if (parts.Length >= 3)
                    {
                        string letter1 = parts[0];
                        string letter2 = parts[1];
                        string numbers = parts[parts.Length - 1];
                        string letter3 = parts.Length > 3 ? parts[2] : "";

                        txtCarLetter1.Text = letter1;
                        txtCarLetter2.Text = letter2;
                        txtCarLetter3.Text = letter3;
                        txtCarNumbers.Text = numbers;

                        System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "loadData",
                            "setTimeout(function() { document.getElementById('" + btnSearch.ClientID + "').click(); }, 100);", true);
                    }
                }
            }
        }

        private void LoadPartsToViewState()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT PartID, PartName FROM MaintenanceParts ORDER BY PartName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    var partsList = new System.Collections.Generic.List<object>();
                    foreach (DataRow row in dt.Rows)
                    {
                        partsList.Add(new { id = row["PartID"].ToString(), name = row["PartName"].ToString() });
                    }

                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    ViewState["PartsData"] = serializer.Serialize(partsList);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل قطع الغيار: {ex.Message}");
                ViewState["PartsData"] = "[]";
            }
        }

        private void SetDefaultDates()
        {
            txtMaintenanceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
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

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);

                    if (carNumberId == 0)
                    {
                        ShowMessage("رقم السيارة غير موجود في قاعدة البيانات");
                        return;
                    }

                    currentCarNumberId = carNumberId;
                    currentVehicleId = GetVehicleId(conn, carNumberId);

                    if (currentVehicleId > 0)
                    {
                        ViewState["VehicleId"] = currentVehicleId;
                        ViewState["CarNumberId"] = currentCarNumberId;
                        lblCarNumber.Text = carNumberDisplay;

                        pnlCarInfo.Visible = true;
                        pnlMaintenanceForm.Visible = true;
                        pnlStatistics.Visible = true;
                        pnlCurrentOdometer.Visible = true;
                        pnlMaintenanceHistory.Visible = true;
                        pnlEngineOverhaul.Visible = true;

                        int workOrderKM = GetLastWorkOrderKilometer(currentCarNumberId);
                        lblCurrentOdometer.Text = workOrderKM.ToString("N0");
                        ViewState["LastWorkOrderKM"] = workOrderKM;

                        if (workOrderKM == 0)
                        {
                            int otherKM = GetOtherSourcesOdometer(conn, currentVehicleId);
                            if (otherKM > 0)
                            {
                                lblCurrentOdometer.Text = otherKM.ToString("N0");
                                ViewState["LastWorkOrderKM"] = otherKM;
                            }
                        }

                        LoadMaintenanceHistory(conn, currentVehicleId);
                        LoadMaintenanceStatistics(conn, currentVehicleId);
                        LoadEngineOverhaulInfo(conn, currentVehicleId);

                        lblMessage.Visible = false;

                        if (workOrderKM > 0)
                        {
                            ShowSuccess($"تم العثور على السيارة - آخر قراءة عداد من أمر الشغل: {workOrderKM:N0} كم");
                        }
                        else
                        {
                            ShowSuccess($"تم العثور على السيارة - لم يتم العثور على قراءة عداد في أوامر الشغل");
                        }
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
                System.Diagnostics.Debug.WriteLine($"خطأ في BtnSearch_Click: {ex.Message}");
            }
        }

        private int GetLastWorkOrderKilometer(int carNumberId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT TOP 1 
                            ISNULL(kilometer_end, 0) as kilometer_end
                        FROM operation_command
                        WHERE v_number = @CarNumberId
                          AND kilometer_end IS NOT NULL
                          AND kilometer_end > 0
                        ORDER BY mission_id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int km = Convert.ToInt32(result);
                            return km;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في GetLastWorkOrderKilometer: {ex.Message}");
            }
            return 0;
        }

        private int GetOtherSourcesOdometer(SqlConnection conn, int vehicleId)
        {
            int maxOdometer = 0;
            try
            {
                string tripsQuery = @"SELECT ISNULL(MAX(EndOdometer), 0) FROM Trips WHERE VehicleID = @VehicleID";
                using (SqlCommand cmd = new SqlCommand(tripsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        maxOdometer = Convert.ToInt32(result);
                }

                string maintQuery = @"SELECT ISNULL(MAX(OdometerReading), 0) FROM MaintenanceHistory WHERE VehicleID = @VehicleID";
                using (SqlCommand cmd = new SqlCommand(maintQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int maintOdometer = Convert.ToInt32(result);
                        if (maintOdometer > maxOdometer) maxOdometer = maintOdometer;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في GetOtherSourcesOdometer: {ex.Message}");
            }
            return maxOdometer;
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

        private string GetFullCarNumber()
        {
            string letter1 = txtCarLetter1.Text.Trim();
            string letter2 = txtCarLetter2.Text.Trim();
            string letter3 = txtCarLetter3.Text.Trim();
            string numbers = txtCarNumbers.Text.Trim();
            return $"{letter1} {letter2} {(string.IsNullOrEmpty(letter3) ? "" : letter3)} {numbers}";
        }

        private void LoadMaintenanceHistory(SqlConnection conn, int vehicleId)
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
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
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
                        pnlMaintenanceHistory.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل سجل الصيانة: {ex.Message}");
                gvMaintenanceHistory.EmptyDataText = "حدث خطأ في تحميل البيانات";
                gvMaintenanceHistory.DataSource = null;
                gvMaintenanceHistory.DataBind();
            }
        }

        private string GetMaintenanceParts(int historyId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                System.Diagnostics.Debug.WriteLine($"خطأ في جلب قطع الغيار: {ex.Message}");
                return "خطأ في تحميل البيانات";
            }
        }

        private void LoadMaintenanceStatistics(SqlConnection conn, int vehicleId)
        {
            try
            {
                string statsQuery = @"
                    SELECT ISNULL(SUM(mh.Cost), 0) AS TotalCost, COUNT(mh.HistoryID) AS MaintenanceCount
                    FROM MaintenanceHistory mh WHERE mh.VehicleID = @VehicleID";

                using (SqlCommand cmd = new SqlCommand(statsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
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
                pnlStatistics.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل إحصائيات الصيانة: {ex.Message}");
                lblTotalCost.Text = "0";
                lblMaintenanceCount.Text = "0";
            }
        }


        private void LoadEngineOverhaulInfo(SqlConnection conn, int vehicleId)
        {
            try
            {
                string query = @"
            SELECT 
                EngineOverhaulDate,
                EngineOverhaulOdometer,
                EngineOverhaulCost,
                EngineOverhaulNotes
            FROM main_vehicle_details
            WHERE v_id = @VehicleID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["EngineOverhaulDate"] != DBNull.Value)
                            {
                                DateTime overhaulDate = Convert.ToDateTime(reader["EngineOverhaulDate"]);
                                txtEngineOverhaulDate.Text = overhaulDate.ToString("yyyy-MM-dd");

                                if (reader["EngineOverhaulOdometer"] != DBNull.Value)
                                {
                                    int overhaulOdometer = Convert.ToInt32(reader["EngineOverhaulOdometer"]);
                                    txtEngineOverhaulOdometer.Text = overhaulOdometer.ToString();

                                    // Load cost
                                    if (reader["EngineOverhaulCost"] != DBNull.Value)
                                    {
                                        decimal overhaulCost = Convert.ToDecimal(reader["EngineOverhaulCost"]);
                                        txtEngineOverhaulCost.Text = overhaulCost.ToString("N2");
                                    }

                                    int currentOdometer = 0;
                                    if (ViewState["LastWorkOrderKM"] != null)
                                    {
                                        currentOdometer = Convert.ToInt32(ViewState["LastWorkOrderKM"]);
                                    }

                                    int kmSinceOverhaul = currentOdometer - overhaulOdometer;

                                    string statusMessage = "";
                                    string statusColor = "";
                                    string statusClass = "";

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

                                if (reader["EngineOverhaulNotes"] != DBNull.Value)
                                {
                                    txtEngineOverhaulNotes.Text = reader["EngineOverhaulNotes"].ToString();
                                }
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل بيانات عمرة الموتور: {ex.Message}");
            }
        }

        protected void BtnSaveEngineOverhaul_Click(object sender, EventArgs e)
        {
            try
            {
                // Get vehicle ID from ViewState or from search
                if (currentVehicleId == 0)
                {
                    if (ViewState["VehicleId"] != null)
                    {
                        currentVehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                    }
                    else
                    {
                        // Try to get from the current search
                        string letter1 = txtCarLetter1.Text.Trim();
                        string letter2 = txtCarLetter2.Text.Trim();
                        string letter3 = txtCarLetter3.Text.Trim();
                        string numbers = txtCarNumbers.Text.Trim();

                        if (!string.IsNullOrEmpty(letter1) && !string.IsNullOrEmpty(letter2) && !string.IsNullOrEmpty(numbers))
                        {
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);
                                if (carNumberId > 0)
                                {
                                    currentCarNumberId = carNumberId;
                                    currentVehicleId = GetVehicleId(conn, carNumberId);
                                    ViewState["VehicleId"] = currentVehicleId;
                                    ViewState["CarNumberId"] = currentCarNumberId;
                                }
                            }
                        }
                    }
                }

                if (currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                // Validate inputs
                if (string.IsNullOrEmpty(txtEngineOverhaulDate.Text))
                {
                    ShowMessage("الرجاء إدخال تاريخ عمرة الموتور");
                    return;
                }

                if (string.IsNullOrEmpty(txtEngineOverhaulOdometer.Text))
                {
                    ShowMessage("الرجاء إدخال قراءة العداد عند عمرة الموتور");
                    return;
                }

                DateTime overhaulDate = Convert.ToDateTime(txtEngineOverhaulDate.Text);
                int overhaulOdometer = Convert.ToInt32(txtEngineOverhaulOdometer.Text);

                // Get cost value (default to 0 if empty)
                decimal overhaulCost = 0;
                if (!string.IsNullOrEmpty(txtEngineOverhaulCost.Text))
                {
                    decimal.TryParse(txtEngineOverhaulCost.Text, out overhaulCost);
                }

                string overhaulNotes = string.IsNullOrEmpty(txtEngineOverhaulNotes.Text) ? "" : txtEngineOverhaulNotes.Text.Trim();

                // Save to database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First check if columns exist
                    bool columnsExist = CheckEngineOverhaulColumnsExist(conn);

                    if (!columnsExist)
                    {
                        ShowMessage("أعمدة عمرة الموتور غير موجودة في قاعدة البيانات. يرجى تشغيل سكريبت SQL أولاً.");
                        return;
                    }

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
                        cmd.Parameters.AddWithValue("@OverhaulOdometer", overhaulOdometer);
                        cmd.Parameters.AddWithValue("@OverhaulCost", overhaulCost);
                        cmd.Parameters.AddWithValue("@OverhaulNotes", string.IsNullOrEmpty(overhaulNotes) ? DBNull.Value : (object)overhaulNotes);
                        cmd.Parameters.AddWithValue("@VehicleID", currentVehicleId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowSuccess("تم تسجيل بيانات عمرة الموتور بنجاح");

                            // Clear form
                            txtEngineOverhaulDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            txtEngineOverhaulOdometer.Text = "";
                            txtEngineOverhaulCost.Text = "";
                            txtEngineOverhaulNotes.Text = "";

                            // Reload the info to display the new data
                            LoadEngineOverhaulInfo(conn, currentVehicleId);
                        }
                        else
                        {
                            ShowMessage("حدث خطأ أثناء حفظ بيانات عمرة الموتور");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء حفظ بيانات عمرة الموتور: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            }
        }

        private bool CheckEngineOverhaulColumnsExist(SqlConnection conn)
        {
            try
            {
                string query = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'main_vehicle_details' 
              AND COLUMN_NAME IN ('EngineOverhaulDate', 'EngineOverhaulOdometer', 'EngineOverhaulCost', 'EngineOverhaulNotes')";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    return count == 4; // Now checking for 4 columns
                }
            }
            catch
            {
                return false;
            }
        }

        protected void BtnClearEngineOverhaul_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentVehicleId == 0)
                {
                    if (ViewState["VehicleId"] != null)
                    {
                        currentVehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                    }
                    else
                    {
                        ShowMessage("الرجاء البحث عن السيارة أولاً");
                        return;
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
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
                        cmd.Parameters.AddWithValue("@VehicleID", currentVehicleId);
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
                ShowMessage("حدث خطأ أثناء مسح بيانات عمرة الموتور: " + ex.Message);
            }
        }

        protected void BtnSaveMaintenance_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["CarNumberId"] != null)
                {
                    currentCarNumberId = Convert.ToInt32(ViewState["CarNumberId"]);
                }

                if (ViewState["VehicleId"] != null)
                {
                    currentVehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                }

                if (currentCarNumberId == 0)
                {
                    string letter1 = txtCarLetter1.Text.Trim();
                    string letter2 = txtCarLetter2.Text.Trim();
                    string letter3 = txtCarLetter3.Text.Trim();
                    string numbers = txtCarNumbers.Text.Trim();

                    if (string.IsNullOrEmpty(letter1) || string.IsNullOrEmpty(letter2) || string.IsNullOrEmpty(numbers))
                    {
                        ShowMessage("الرجاء البحث عن السيارة أولاً");
                        return;
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);
                        if (carNumberId > 0)
                        {
                            currentCarNumberId = carNumberId;
                            currentVehicleId = GetVehicleId(conn, carNumberId);
                            ViewState["VehicleId"] = currentVehicleId;
                            ViewState["CarNumberId"] = currentCarNumberId;
                        }
                    }
                }

                if (currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                if (string.IsNullOrEmpty(ddlMaintenanceType.SelectedValue))
                {
                    ShowMessage("الرجاء اختيار نوع الصيانة");
                    return;
                }

                string selectedMaintenanceType = ddlMaintenanceType.SelectedValue;

                int newOdometer = 0;
                if (!string.IsNullOrEmpty(txtOdometerReading.Text))
                {
                    if (!int.TryParse(txtOdometerReading.Text, out newOdometer) || newOdometer < 0)
                    {
                        ShowMessage("الرجاء إدخال قراءة عداد صحيحة");
                        txtOdometerReading.BackColor = System.Drawing.Color.LightPink;
                        return;
                    }
                }
                else
                {
                    ShowMessage("الرجاء إدخال قراءة العداد");
                    txtOdometerReading.BackColor = System.Drawing.Color.LightPink;
                    return;
                }

                int lastWorkOrderKM = 0;

                if (ViewState["LastWorkOrderKM"] != null)
                {
                    lastWorkOrderKM = Convert.ToInt32(ViewState["LastWorkOrderKM"]);
                }

                if (lastWorkOrderKM == 0)
                {
                    lastWorkOrderKM = GetLastWorkOrderKilometer(currentCarNumberId);
                }

                if (lastWorkOrderKM > 0 && newOdometer > lastWorkOrderKM)
                {
                    string errorMessage = $"خطأ: قراءة العداد المدخلة ({newOdometer:N0} كم) لا يمكن أن تتجاوز آخر قراءة مسجلة في أوامر الشغل ({lastWorkOrderKM:N0} كم)";
                    ShowMessage(errorMessage);
                    txtOdometerReading.BackColor = System.Drawing.Color.LightPink;
                    return;
                }

                if (lastWorkOrderKM == 0 && newOdometer > 0)
                {
                    int freshKM = GetLastWorkOrderKilometer(currentCarNumberId);
                    if (freshKM > 0 && newOdometer > freshKM)
                    {
                        string errorMessage = $"خطأ: قراءة العداد المدخلة ({newOdometer:N0} كم) لا يمكن أن تتجاوز آخر قراءة مسجلة في أوامر الشغل ({freshKM:N0} كم)";
                        ShowMessage(errorMessage);
                        txtOdometerReading.BackColor = System.Drawing.Color.LightPink;
                        return;
                    }
                }

                txtOdometerReading.BackColor = System.Drawing.Color.White;

                string[] partIds = hfPartsData.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string partNames = GetPartNamesFromIds(partIds);

                using (SqlConnection conn = new SqlConnection(connectionString))
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
                                cmd.Parameters.AddWithValue("@VehicleID", currentVehicleId);
                                cmd.Parameters.AddWithValue("@ServiceDate", Convert.ToDateTime(txtMaintenanceDate.Text));
                                cmd.Parameters.AddWithValue("@OdometerReading", newOdometer);
                                cmd.Parameters.AddWithValue("@Cost", string.IsNullOrEmpty(txtCost.Text) ? 0 : Convert.ToDecimal(txtCost.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? "صيانة دورية" : txtNotes.Text.Trim());
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

                            string serviceType = "";
                            switch (selectedMaintenanceType)
                            {
                                case "صيانة دورية":
                                    serviceType = "1";
                                    break;
                                case "اعطال":
                                    serviceType = "2";
                                    break;
                                case "صيانة طارئة":
                                    serviceType = "3";
                                    break;
                                default:
                                    serviceType = "1";
                                    break;
                            }

                            string insertMaintenance = @"
                                INSERT INTO VehicleMaintenance (VehicleID, MaintenanceDate, MaintenanceType, TotalCost, Notes, service_type, history, CreatedDate)
                                VALUES (@VehicleID, @MaintenanceDate, @MaintenanceType, @TotalCost, @Notes, @ServiceType, @HistoryID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            int maintenanceId;
                            using (SqlCommand cmd = new SqlCommand(insertMaintenance, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleID", currentVehicleId);
                                cmd.Parameters.AddWithValue("@MaintenanceDate", Convert.ToDateTime(txtMaintenanceDate.Text));
                                cmd.Parameters.AddWithValue("@MaintenanceType", selectedMaintenanceType);
                                cmd.Parameters.AddWithValue("@TotalCost", string.IsNullOrEmpty(txtCost.Text) ? 0 : Convert.ToDecimal(txtCost.Text));
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? "صيانة دورية" : txtNotes.Text.Trim());
                                cmd.Parameters.AddWithValue("@ServiceType", serviceType);
                                cmd.Parameters.AddWithValue("@HistoryID", historyId);
                                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                maintenanceId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            string insertJunction = "INSERT INTO vehicle_maintenance (vehicle_id, maintenance_id, created_date) VALUES (@VehicleId, @MaintenanceId, GETDATE())";
                            using (SqlCommand jCmd = new SqlCommand(insertJunction, conn, transaction))
                            {
                                jCmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);
                                jCmd.Parameters.AddWithValue("@MaintenanceId", maintenanceId);
                                jCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            // Store the last maintenance data for alert display in history page
                            var lastMaintenanceData = new
                            {
                                Parts = partIds,
                                PartNames = partNames,
                                Odometer = newOdometer,
                                MaintenanceDate = txtMaintenanceDate.Text,
                                MaintenanceType = selectedMaintenanceType,
                                Notes = txtNotes.Text
                            };
                            Session["LastMaintenanceAdded"] = lastMaintenanceData;

                            int updatedWorkOrderKM = GetLastWorkOrderKilometer(currentCarNumberId);
                            lblCurrentOdometer.Text = updatedWorkOrderKM.ToString("N0");
                            ViewState["LastWorkOrderKM"] = updatedWorkOrderKM;

                            LoadMaintenanceHistory(conn, currentVehicleId);
                            LoadMaintenanceStatistics(conn, currentVehicleId);

                            ClearForm();
                            ShowSuccess($"تم حفظ بيانات الصيانة بنجاح مع {partIds.Length} قطعة غيار");
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
                System.Diagnostics.Debug.WriteLine($"خطأ في حفظ الصيانة: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        private string GetPartNamesFromIds(string[] partIds)
        {
            if (partIds == null || partIds.Length == 0)
                return "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                System.Diagnostics.Debug.WriteLine($"خطأ في جلب أسماء قطع الغيار: {ex.Message}");
                return "";
            }
        }

        protected void BtnViewHistory_Click(object sender, EventArgs e)
        {
            Session["VehicleIdToLoad"] = currentVehicleId;
            Session["CarNumberToLoad"] = GetFullCarNumber();
            Session["RefreshRequired"] = true;
            Response.Redirect("vehicle_maintenance_history.aspx");
        }

        protected void GvMaintenanceHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMaintenanceHistory.PageIndex = e.NewPageIndex;
            if (ViewState["VehicleId"] != null)
            {
                int vehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    LoadMaintenanceHistory(conn, vehicleId);
                }
            }
        }

        private void ClearForm()
        {
            ddlMaintenanceType.SelectedValue = "";
            txtOdometerReading.Text = "";
            txtCost.Text = "";
            txtNotes.Text = "";
            hfPartsData.Value = "";
            SetDefaultDates();
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