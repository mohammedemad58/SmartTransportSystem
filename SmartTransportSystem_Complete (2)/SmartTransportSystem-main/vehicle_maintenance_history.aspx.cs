using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;

namespace car_WebApplication1
{
    public partial class vehicle_maintenance_history : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int _currentVehicleId = 0;
        private int _currentCarNumberId = 0;
        private string _filterMaintenanceType = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlVehicleInfo.Visible = false;
                pnlAlerts.Visible = false;
                pnlMaintenanceHistory.Visible = false;
                pnlStatistics.Visible = false;
                pnlNewMaintenanceAlert.Visible = false;

                // Check for new maintenance alert from session
                if (Session["LastMaintenanceAdded"] != null)
                {
                    ShowNewMaintenanceAlert();
                    Session.Remove("LastMaintenanceAdded");
                }

                if (Session["LastMaintenanceType"] != null)
                {
                    _filterMaintenanceType = Session["LastMaintenanceType"].ToString();
                    Session.Remove("LastMaintenanceType");
                }

                if (Session["VehicleIdToLoad"] != null && Session["CarNumberToLoad"] != null)
                {
                    int vehicleId = Convert.ToInt32(Session["VehicleIdToLoad"]);
                    string carNumber = Session["CarNumberToLoad"].ToString();

                    Session.Remove("VehicleIdToLoad");
                    Session.Remove("CarNumberToLoad");
                    Session.Remove("RefreshRequired");

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

                        LoadVehicleData(letter1, letter2, letter3, numbers, carNumber);
                    }
                }
            }
        }

        private void ShowNewMaintenanceAlert()
        {
            try
            {
                dynamic lastMaintenance = Session["LastMaintenanceAdded"];
                if (lastMaintenance != null)
                {
                    string partNames = lastMaintenance.PartNames;
                    string maintenanceType = lastMaintenance.MaintenanceType;
                    int odometer = lastMaintenance.Odometer;
                    string maintenanceDate = lastMaintenance.MaintenanceDate;
                    string notes = lastMaintenance.Notes;

                    StringBuilder alertHtml = new StringBuilder();
                    alertHtml.AppendLine("<div class='mt-2' dir='rtl'>");
                    alertHtml.AppendLine($"<p><strong>📋 تفاصيل الصيانة:</strong></p>");
                    alertHtml.AppendLine($"<ul style='margin-bottom:0; list-style-type: none; padding-right: 0;'>");
                    alertHtml.AppendLine($"<li><strong>📅 تاريخ الصيانة:</strong> {maintenanceDate}</li>");
                    alertHtml.AppendLine($"<li><strong>🔧 نوع الصيانة:</strong> {maintenanceType}</li>");
                    alertHtml.AppendLine($"<li><strong>📊 قراءة العداد:</strong> {odometer:N0} كم</li>");
                    alertHtml.AppendLine($"<li><strong>🛠️ قطع الغيار المستبدلة:</strong> {partNames}</li>");
                    if (!string.IsNullOrEmpty(notes) && notes != "صيانة دورية")
                    {
                        alertHtml.AppendLine($"<li><strong>📝 ملاحظات:</strong> {notes}</li>");
                    }
                    alertHtml.AppendLine("</ul>");

                    // Add maintenance schedule info based on the parts
                    alertHtml.AppendLine("<hr class='my-2'>");
                    alertHtml.AppendLine("<p><strong>📌 جدول الصيانة القادم (حسب قطع الغيار المختارة):</strong></p>");
                    alertHtml.AppendLine("<ul style='list-style-type: none; padding-right: 0;'>");

                    var schedules = GetMaintenanceSchedulesForParts(partNames);
                    foreach (var schedule in schedules)
                    {
                        alertHtml.AppendLine($"<li><i class='fas fa-calendar-alt'></i> {schedule}</li>");
                    }

                    alertHtml.AppendLine("</ul>");
                    alertHtml.AppendLine("</div>");

                    litNewMaintenanceDetails.Text = alertHtml.ToString();
                    pnlNewMaintenanceAlert.Visible = true;

                    // Auto-hide after 10 seconds
                    string script = "setTimeout(function() { var alert = document.getElementById('" + pnlNewMaintenanceAlert.ClientID + "'); if(alert) alert.style.display='none'; }, 10000);";
                    ClientScript.RegisterStartupScript(this.GetType(), "HideAlert", script, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في عرض تنبيه الصيانة: {ex.Message}");
                litNewMaintenanceDetails.Text = "تم إضافة صيانة جديدة بنجاح";
                pnlNewMaintenanceAlert.Visible = true;
            }
        }

        private List<string> GetMaintenanceSchedulesForParts(string partNames)
        {
            var schedules = new List<string>();

            var maintenanceSchedules = new Dictionary<string, (int kmInterval, int timeMonths, string unit)>
            {
                { "بطاريات", (0, 18, "شهر") },
                { "الكاوتش", (35000, 0, "كم") },
                { "فلتر الزيت", (5000, 0, "كم") },
                { "فلتر الوقود", (10000, 0, "كم") },
                { "فلتر الهواء", (15000, 9, "كم/شهر") },
                { "طقم السيور", (15000, 9, "كم/شهر") },
                { "طقم كهرباء كامل", (10000, 0, "كم") },
                { "طرمبة حقن الوقود والرشاشات", (30000, 0, "كم") },
                { "طقم ديرباج كامل", (5000, 12, "كم/شهر") },
                { "تيل فرامل أمامي", (2000, 0, "كم") },
                { "تيل فرامل خلفي", (40000, 0, "كم") },
                { "ماستر فرامل عمومى", (50000, 0, "كم") },
                { "عفشة", (50000, 12, "كم/شهر") }
            };

            foreach (var schedule in maintenanceSchedules)
            {
                if (partNames.Contains(schedule.Key))
                {
                    string scheduleText = $"• {schedule.Key}: ";
                    if (schedule.Value.kmInterval > 0)
                        scheduleText += $"كل {schedule.Value.kmInterval:N0} كم ";
                    if (schedule.Value.timeMonths > 0)
                        scheduleText += $"أو كل {schedule.Value.timeMonths} شهر";
                    schedules.Add(scheduleText);
                }
            }

            if (schedules.Count == 0)
            {
                schedules.Add("• اتبع جدول الصيانة الدورية الموصى به");
            }

            return schedules;
        }

        public void LoadVehicleData(string letter1, string letter2, string letter3, string numbers, string carNumberDisplay)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    int carNumberId = GetCarNumberId(conn, letter1, letter2, letter3, numbers);

                    if (carNumberId == 0)
                    {
                        ShowMessage("رقم السيارة غير موجود في قاعدة البيانات");
                        return;
                    }

                    _currentCarNumberId = carNumberId;

                    int vehicleId = GetVehicleDetails(conn, carNumberId, carNumberDisplay);

                    if (vehicleId > 0)
                    {
                        _currentVehicleId = vehicleId;
                        ViewState["VehicleId"] = vehicleId;
                        ViewState["CarNumberId"] = carNumberId;

                        pnlVehicleInfo.Visible = true;

                        int workOrderOdometer = GetLatestWorkOrderOdometer(conn, carNumberId);
                        lblCurrentOdometer.Text = workOrderOdometer.ToString("N0");
                        lblLastOdometer.Text = workOrderOdometer.ToString("N0");

                        System.Diagnostics.Debug.WriteLine($"=== قراءة العداد من work_order: {workOrderOdometer} كم ===");

                        LoadMaintenanceHistory(conn, vehicleId);
                        pnlMaintenanceHistory.Visible = true;

                        LoadMaintenanceStatistics(conn, vehicleId);
                        pnlStatistics.Visible = true;

                        CheckMaintenanceAlerts(conn, vehicleId, workOrderOdometer, _filterMaintenanceType);

                        lblMessage.Visible = false;

                        if (!string.IsNullOrEmpty(_filterMaintenanceType))
                        {
                            ShowSuccess($"تم تحميل البيانات بنجاح - عرض تنبيهات الصيانة لنوع: {_filterMaintenanceType}");
                        }
                        else
                        {
                            ShowSuccess("تم تحميل البيانات بنجاح");
                        }
                    }
                    else
                    {
                        ShowMessage("لا توجد بيانات للسيارة المحددة");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine($"خطأ في LoadVehicleData: {ex.Message}");
                }
            }
        }

        protected string GetMaintenanceParts(object historyId)
        {
            if (historyId == DBNull.Value)
                return "—";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT STRING_AGG(mp.PartName, N'<br/>') AS PartsNames
                        FROM MaintenanceHistoryParts mhp
                        INNER JOIN MaintenanceParts mp ON mhp.PartID = mp.PartID
                        WHERE mhp.HistoryID = @HistoryID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HistoryID", Convert.ToInt32(historyId));
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            string parts = result.ToString();
                            if (!string.IsNullOrEmpty(parts))
                            {
                                string[] partsArray = parts.Split(new[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                                if (partsArray.Length > 0)
                                {
                                    StringBuilder html = new StringBuilder();
                                    html.Append("<ul style='margin:0; padding-right:20px; text-align:right;'>");
                                    foreach (string part in partsArray)
                                    {
                                        html.AppendFormat("<li style='margin:2px 0;'>{0}</li>", part);
                                    }
                                    html.Append("</ul>");
                                    return html.ToString();
                                }
                            }
                            return parts.Replace("<br/>", ", ");
                        }
                        return "—";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في جلب قطع الغيار: {ex.Message}");
                return "خطأ";
            }
        }

        private int GetLatestWorkOrderOdometer(SqlConnection conn, int carNumberId)
        {
            try
            {
                // Try to get from operation_command with kilometer_end
                string query = @"
            SELECT TOP 1 ISNULL(kilometer_end, 0)
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
                        int odometer = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine($"تم العثور على قراءة من operation_command: {odometer} كم");
                        return odometer;
                    }
                }

                // If no kilometer_end found, get from maintenance history
                string maintenanceQuery = @"
            SELECT TOP 1 ISNULL(OdometerReading, 0)
            FROM MaintenanceHistory mh
            INNER JOIN VehicleMaintenance vm ON mh.HistoryID = vm.history
            WHERE vm.CarNumberID = @CarNumberId
              AND mh.OdometerReading IS NOT NULL
              AND mh.OdometerReading > 0
            ORDER BY mh.ServiceDate DESC";

                using (SqlCommand cmd = new SqlCommand(maintenanceQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int odometer = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine($"تم العثور على قراءة من maintenance history: {odometer} كم");
                        return odometer;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في جلب قراءة العداد: {ex.Message}");
            }
            return 0;
        }

        private void LoadMaintenanceHistory(SqlConnection conn, int vehicleId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== بدء تحميل سجل الصيانة للسيارة ID: {vehicleId} ===");

                string query = @"
                    SELECT 
                        mh.ServiceDate,
                        vm.MaintenanceType,
                        ISNULL(mh.OdometerReading, 0) AS OdometerReading,
                        ISNULL(mh.Cost, 0) AS Cost,
                        ISNULL(mh.NextDueKM, 0) AS NextDueKM,
                        mh.NextDueDate,
                        ISNULL(mh.Notes, N'') AS Notes,
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

                        System.Diagnostics.Debug.WriteLine($"تم العثور على {dt.Rows.Count} سجل صيانة");

                        gvMaintenanceHistory.DataSource = dt;
                        gvMaintenanceHistory.DataBind();

                        if (dt.Rows.Count == 0)
                        {
                            gvMaintenanceHistory.EmptyDataText = "لا توجد سجلات صيانة لهذه السيارة";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل سجل الصيانة: {ex.Message}");
                gvMaintenanceHistory.EmptyDataText = $"خطأ: {ex.Message}";
                gvMaintenanceHistory.DataSource = null;
                gvMaintenanceHistory.DataBind();
            }
        }

        private void CheckMaintenanceAlerts(SqlConnection conn, int vehicleId, int currentOdometer, string maintenanceType)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"*** فحص التنبيهات - السيارة: {vehicleId}, العداد الحالي: {currentOdometer} كم ***");

                DataTable alertsDt = new DataTable();
                alertsDt.Columns.Add("ScheduleID", typeof(int));
                alertsDt.Columns.Add("PartName", typeof(string));
                alertsDt.Columns.Add("PartType", typeof(string));
                alertsDt.Columns.Add("MaintenanceType", typeof(string));
                alertsDt.Columns.Add("AlertLevel", typeof(string));
                alertsDt.Columns.Add("AlertMessage", typeof(string));
                alertsDt.Columns.Add("LastServiceDate", typeof(DateTime));
                alertsDt.Columns.Add("LastServiceKM", typeof(int));
                alertsDt.Columns.Add("KMInterval", typeof(int));
                alertsDt.Columns.Add("TimeIntervalMonths", typeof(int));
                alertsDt.Columns.Add("NextDueKM", typeof(int));
                alertsDt.Columns.Add("NextDueDate", typeof(DateTime));
                alertsDt.Columns.Add("RemainingKM", typeof(int));
                alertsDt.Columns.Add("RemainingDays", typeof(int));
                alertsDt.Columns.Add("SortOrder", typeof(int));
                alertsDt.Columns.Add("PrimaryAlert", typeof(string));
                alertsDt.Columns.Add("DueBasedOn", typeof(string));

                var maintenanceSchedules = new Dictionary<string, (int kmInterval, int timeMonths, string partType)>
        {
            { "بطاريات عدم تكييف", (0, 18, "كهرباء") },
            { "بطاريات بالتكييف", (0, 12, "كهرباء") },
            { "الكاوتش", (35000, 0, "إطارات") },
            { "فلتر الزيت", (5000, 0, "فلاتر") },
            { "فلتر الوقود", (10000, 0, "فلاتر") },
            { "فلتر الهواء", (15000, 9, "فلاتر") },
            { "طقم السيور", (15000, 9, "سيور") },
            { "طقم كهرباء كامل", (10000, 0, "كهرباء") },
            { "طرمبة حقن الوقود والرشاشات", (30000, 0, "حقن") },
            { "طقم ديرباج كامل", (5000, 12, "قير") },
            { "تيل فرامل أمامي", (2000, 0, "فرامل") },
            { "تيل فرامل خلفي", (40000, 0, "فرامل") },
            { "ماستر فرامل عمومى", (50000, 0, "فرامل") },
            { "عفشة", (50000, 12, "عفشة") }
        };

                foreach (var schedule in maintenanceSchedules)
                {
                    string partName = schedule.Key;
                    int kmInterval = schedule.Value.kmInterval;
                    int timeMonths = schedule.Value.timeMonths;
                    string partType = schedule.Value.partType;

                    int lastServiceKM = 0;
                    DateTime? lastServiceDate = null;

                    string lastServiceQuery = @"
                SELECT TOP 1 
                    ISNULL(mh.OdometerReading, 0) AS LastKM,
                    mh.ServiceDate AS LastDate
                FROM MaintenanceHistory mh
                INNER JOIN VehicleMaintenance vm ON mh.HistoryID = vm.history
                LEFT JOIN MaintenanceHistoryParts mhp ON mh.HistoryID = mhp.HistoryID
                LEFT JOIN MaintenanceParts mp ON mhp.PartID = mp.PartID
                WHERE mh.VehicleID = @VehicleID
                  AND (
                      mp.PartName = @PartName
                      OR vm.MaintenanceType LIKE @PartNameSearch
                  )
                ORDER BY mh.ServiceDate DESC";

                    using (SqlCommand cmd = new SqlCommand(lastServiceQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@VehicleID", vehicleId);
                        cmd.Parameters.AddWithValue("@PartName", partName);
                        cmd.Parameters.AddWithValue("@PartNameSearch", "%" + partName + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lastServiceKM = reader["LastKM"] != DBNull.Value ? Convert.ToInt32(reader["LastKM"]) : 0;
                                lastServiceDate = reader["LastDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastDate"]) : (DateTime?)null;
                            }
                        }
                    }

                    int nextDueKM = 0;
                    DateTime? nextDueDate = null;
                    int remainingKM = 0;
                    int remainingDays = 0;
                    string alertLevel = "success";
                    string alertMessage = "✓ في الموعد المحدد";
                    int sortOrder = 3;
                    string primaryAlert = "ok";
                    string dueBasedOn = "none";

                    bool hasPreviousService = (lastServiceKM > 0 || lastServiceDate.HasValue);

                    if (!hasPreviousService)
                    {
                        alertLevel = "warning";
                        sortOrder = 2;
                        primaryAlert = "no_history";
                        dueBasedOn = "no_history";

                        System.Text.StringBuilder messageBuilder = new System.Text.StringBuilder();
                        messageBuilder.Append("⚠️ لم يتم تسجيل صيانة سابقة - ");

                        bool hasKM = false;

                        if (kmInterval > 0)
                        {
                            nextDueKM = currentOdometer + kmInterval;
                            remainingKM = kmInterval;
                            messageBuilder.Append(string.Format("الصيانة القادمة عند {0:N0} كم (بعد {1:N0} كم)", nextDueKM, kmInterval));
                            hasKM = true;
                        }

                        if (timeMonths > 0)
                        {
                            nextDueDate = currentDate.AddMonths(timeMonths);
                            remainingDays = timeMonths * 30;

                            if (hasKM)
                            {
                                messageBuilder.Append(string.Format(" أو بعد {0} يوم", remainingDays));
                            }
                            else
                            {
                                messageBuilder.Append(string.Format("الصيانة القادمة بعد {0} شهر ({1} يوم)", timeMonths, remainingDays));
                            }
                        }

                        if (!hasKM && timeMonths == 0)
                        {
                            messageBuilder.Append("لا توجد فترات صيانة محددة لهذا الجزء");
                        }

                        alertMessage = messageBuilder.ToString();
                    }
                    else
                    {
                        if (kmInterval > 0 && lastServiceKM > 0)
                        {
                            nextDueKM = lastServiceKM + kmInterval;
                            remainingKM = nextDueKM - currentOdometer;
                            System.Diagnostics.Debug.WriteLine($"  - {partName}: آخر صيانة عند {lastServiceKM} كم, الصيانة القادمة عند {nextDueKM} كم, المتبقي {remainingKM} كم");
                        }

                        if (timeMonths > 0 && lastServiceDate.HasValue)
                        {
                            nextDueDate = lastServiceDate.Value.AddMonths(timeMonths);
                            remainingDays = (int)(nextDueDate.Value - currentDate).TotalDays;
                            System.Diagnostics.Debug.WriteLine($"  - {partName}: آخر صيانة في {lastServiceDate.Value:yyyy-MM-dd}, الصيانة القادمة في {nextDueDate.Value:yyyy-MM-dd}, المتبقي {remainingDays} يوم");
                        }

                        if (kmInterval > 0 && lastServiceKM > 0)
                        {
                            if (remainingKM <= 0)
                            {
                                alertLevel = "danger";
                                alertMessage = string.Format("⚠️ تجاوزت المسافة المحددة للصيانة بـ {0:N0} كم - صيانة فورية مطلوبة!", Math.Abs(remainingKM));
                                sortOrder = 1;
                                primaryAlert = "km_overdue";
                                dueBasedOn = "km_overdue";
                            }
                            else if (remainingKM <= 500)
                            {
                                alertLevel = "danger";
                                alertMessage = string.Format("⚠️ صيانة عاجلة! متبقي {0:N0} كم فقط (المسافة الكلية: {1:N0} كم)", remainingKM, kmInterval);
                                sortOrder = 1;
                                primaryAlert = "km_urgent";
                                dueBasedOn = "km_urgent";
                            }
                            else if (remainingKM <= 1000)
                            {
                                alertLevel = "warning";
                                alertMessage = string.Format("⚡ موعد الصيانة يقترب - متبقي {0:N0} كم فقط من أصل {1:N0} كم", remainingKM, kmInterval);
                                sortOrder = 2;
                                primaryAlert = "km_warning";
                                dueBasedOn = "km_warning";
                            }
                            else if (remainingKM <= 2000)
                            {
                                alertLevel = "warning";
                                alertMessage = string.Format("⚡ سيحتاج صيانة قريباً - متبقي {0:N0} كم (المسافة المتبقية {1}%)", remainingKM, (remainingKM * 100 / kmInterval));
                                sortOrder = 2;
                                primaryAlert = "km_notice";
                                dueBasedOn = "km_notice";
                            }
                        }

                        if (timeMonths > 0 && lastServiceDate.HasValue && (primaryAlert == "ok" || primaryAlert == "km_notice"))
                        {
                            if (remainingDays <= 0)
                            {
                                alertLevel = "danger";
                                alertMessage = string.Format("⚠️ تجاوز الموعد الزمني للصيانة بـ {0} يوم - صيانة فورية مطلوبة!", Math.Abs(remainingDays));
                                sortOrder = 1;
                                primaryAlert = "time_overdue";
                                dueBasedOn = "time_overdue";
                            }
                            else if (remainingDays <= 7)
                            {
                                if (primaryAlert != "danger" && primaryAlert != "km_urgent")
                                {
                                    alertLevel = "danger";
                                    alertMessage = string.Format("⚠️ الموعد الزمني يقترب جداً! متبقي {0} يوم فقط", remainingDays);
                                    sortOrder = 1;
                                    primaryAlert = "time_urgent";
                                    dueBasedOn = "time_urgent";
                                }
                            }
                            else if (remainingDays <= 30 && alertLevel != "danger" && alertLevel != "warning")
                            {
                                alertLevel = "warning";
                                alertMessage = string.Format("⚡ الموعد الزمني يقترب - متبقي {0} يوم", remainingDays);
                                sortOrder = 2;
                                primaryAlert = "time_warning";
                                dueBasedOn = "time_warning";
                            }
                        }

                        if (primaryAlert == "ok")
                        {
                            if (kmInterval > 0 && remainingKM > 2000)
                            {
                                alertLevel = "success";
                                alertMessage = string.Format("✓ في الموعد المحدد - متبقي {0:N0} كم حتى الصيانة القادمة عند {1:N0} كم", remainingKM, nextDueKM);
                                primaryAlert = "ok_km";
                                dueBasedOn = "ok_km";
                            }
                            else if (timeMonths > 0 && remainingDays > 30)
                            {
                                alertLevel = "success";
                                alertMessage = string.Format("✓ في الموعد المحدد - متبقي {0} يوم حتى الصيانة القادمة في {1:yyyy-MM-dd}", remainingDays, nextDueDate);
                                primaryAlert = "ok_time";
                                dueBasedOn = "ok_time";
                            }
                        }
                    }

                    DataRow alertRow = alertsDt.NewRow();
                    alertRow["ScheduleID"] = 0;
                    alertRow["PartName"] = partName;
                    alertRow["PartType"] = partType;
                    alertRow["MaintenanceType"] = partName;
                    alertRow["AlertLevel"] = alertLevel;
                    alertRow["AlertMessage"] = alertMessage;
                    alertRow["LastServiceDate"] = lastServiceDate.HasValue ? lastServiceDate.Value : (object)DBNull.Value;
                    alertRow["LastServiceKM"] = lastServiceKM;
                    alertRow["KMInterval"] = kmInterval;
                    alertRow["TimeIntervalMonths"] = timeMonths;
                    alertRow["NextDueKM"] = nextDueKM > 0 ? nextDueKM : 0;
                    alertRow["NextDueDate"] = nextDueDate.HasValue ? nextDueDate.Value : (object)DBNull.Value;
                    alertRow["RemainingKM"] = remainingKM > 0 ? remainingKM : (remainingKM < 0 ? 0 : remainingKM);
                    alertRow["RemainingDays"] = remainingDays > 0 ? remainingDays : 0;
                    alertRow["SortOrder"] = sortOrder;
                    alertRow["PrimaryAlert"] = primaryAlert;
                    alertRow["DueBasedOn"] = dueBasedOn;
                    alertsDt.Rows.Add(alertRow);
                }

                if (alertsDt.Rows.Count == 0)
                {
                    lblNoAlerts.Text = "✓ لا توجد تنبيهات - جميع قطع الغيار في الموعد المحدد";
                    pnlAlerts.Visible = true;
                    lblNoAlerts.Visible = true;
                    rptAlerts.Visible = false;
                }
                else
                {
                    DataView sortedView = alertsDt.DefaultView;
                    sortedView.Sort = "SortOrder ASC, RemainingKM ASC";
                    DataTable sortedAlerts = sortedView.ToTable();

                    rptAlerts.DataSource = sortedAlerts;
                    rptAlerts.DataBind();
                    pnlAlerts.Visible = true;
                    rptAlerts.Visible = true;
                    lblNoAlerts.Visible = false;
                }
            }
            catch (Exception)
            {
                pnlAlerts.Visible = true;
                lblNoAlerts.Visible = true;
                lblNoAlerts.Text = "حدث خطأ في فحص التنبيهات";
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

        private int GetVehicleDetails(SqlConnection conn, int carNumberId, string carNumberDisplay)
        {
            string query = @"
                SELECT TOP 1 
                    v.v_id,
                    ISNULL(ct.car_brand + ' - ' + ct.car_type_name, 'غير محدد') AS CarType,
                    ISNULL(cm.model, 'غير محدد') AS ModelName,
                    v.v_year
                FROM main_vehicle_details v
                LEFT JOIN car_types ct ON v.v_type = ct.car_type_id
                LEFT JOIN car_model cm ON v.v_model = cm.m_id
                WHERE v.v_num = @CarNumberId
                ORDER BY v.v_id DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int vehicleId = Convert.ToInt32(reader["v_id"]);

                        lblCarNumber.Text = carNumberDisplay;
                        lblCarType.Text = reader["CarType"].ToString();
                        lblCarModel.Text = reader["ModelName"].ToString();
                        lblCarYear.Text = reader["v_year"].ToString();

                        return vehicleId;
                    }
                }
            }
            return 0;
        }

        private void LoadMaintenanceStatistics(SqlConnection conn, int vehicleId)
        {
            try
            {
                string statsQuery = @"
                    SELECT 
                        ISNULL(SUM(TotalCost), 0) AS TotalCost,
                        COUNT(MaintenanceID) AS MaintenanceCount
                    FROM VehicleMaintenance
                    WHERE VehicleID = @VehicleID";

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل الإحصائيات: {ex.Message}");
                lblTotalCost.Text = "0";
                lblMaintenanceCount.Text = "0";
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
                _filterMaintenanceType = null;
                LoadVehicleData(letter1, letter2, letter3, numbers, carNumberDisplay);
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ: " + ex.Message);
            }
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            txtCarLetter1.Text = "";
            txtCarLetter2.Text = "";
            txtCarLetter3.Text = "";
            txtCarNumbers.Text = "";

            pnlVehicleInfo.Visible = false;
            pnlAlerts.Visible = false;
            pnlMaintenanceHistory.Visible = false;
            pnlStatistics.Visible = false;
            pnlNewMaintenanceAlert.Visible = false;

            gvMaintenanceHistory.DataSource = null;
            gvMaintenanceHistory.DataBind();
            rptAlerts.DataSource = null;
            rptAlerts.DataBind();

            ViewState["VehicleId"] = null;
            _currentVehicleId = 0;
            _filterMaintenanceType = null;
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
        }

        protected void RptAlerts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DataRowView rowView = (DataRowView)e.Item.DataItem;
            string alertLevel = rowView["AlertLevel"]?.ToString() ?? "success";
            string dueBasedOn = rowView["DueBasedOn"]?.ToString() ?? "none";

            HtmlGenericControl alertItem = (HtmlGenericControl)e.Item.FindControl("alertItem");
            HtmlGenericControl badgeSpan = (HtmlGenericControl)e.Item.FindControl("badgeSpan");
            Label lblLastServiceDate = (Label)e.Item.FindControl("lblLastServiceDate");
            Label lblAlertMessage = (Label)e.Item.FindControl("lblAlertMessage");
            Label lblAlertMessageText = (Label)e.Item.FindControl("lblAlertMessageText");
            Label lblServiceName = (Label)e.Item.FindControl("lblServiceName");
            Label lblKMInterval = (Label)e.Item.FindControl("lblKMInterval");
            Label lblTimeInterval = (Label)e.Item.FindControl("lblTimeInterval");
            Label lblRemainingKM = (Label)e.Item.FindControl("lblRemainingKM");
            Label lblRemainingDays = (Label)e.Item.FindControl("lblRemainingDays");
            Label lblNextDue = (Label)e.Item.FindControl("lblNextDue");
            Label lblMaintenanceType = (Label)e.Item.FindControl("lblMaintenanceType");

            if (lblServiceName != null && rowView["PartName"] != null)
                lblServiceName.Text = rowView["PartName"].ToString();

            // Hide Maintenance Type (optional, can keep visible)
            if (lblMaintenanceType != null)
            {
                lblMaintenanceType.Visible = true;
                lblMaintenanceType.Text = rowView["PartType"]?.ToString() ?? "";
            }

            // Separate alert message display
            if (lblAlertMessage != null && rowView["AlertMessage"] != null)
            {
                lblAlertMessage.Visible = false; // Hide the original label
            }

            // Show alert message in dedicated text label
            if (lblAlertMessageText != null && rowView["AlertMessage"] != null)
            {
                lblAlertMessageText.Text = rowView["AlertMessage"].ToString();
            }

            // Handle Last Service Date
            if (lblLastServiceDate != null)
            {
                if (rowView["LastServiceDate"] is DateTime lastDate && lastDate <= DateTime.Now && lastDate.Year > 2000 && lastDate.Year <= DateTime.Now.Year)
                {
                    lblLastServiceDate.Text = lastDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    lblLastServiceDate.Text = "لم يسبق";
                }
            }

            // Show KM Interval
            if (lblKMInterval != null && rowView["KMInterval"] is int kmInterval)
            {
                if (kmInterval > 0)
                {
                    lblKMInterval.Text = $"{kmInterval:N0} كم";
                }
                else
                {
                    lblKMInterval.Text = "غير محدد";
                }
            }

            // Show Time Interval
            if (lblTimeInterval != null && rowView["TimeIntervalMonths"] is int timeInterval)
            {
                if (timeInterval > 0)
                {
                    lblTimeInterval.Text = $"{timeInterval} شهر";
                }
                else
                {
                    lblTimeInterval.Text = "غير محدد";
                }
            }

            // Show remaining KM
            if (lblRemainingKM != null && rowView["RemainingKM"] is int remainingKM && rowView["KMInterval"] is int kmIntervalVal)
            {
                if (kmIntervalVal > 0)
                {
                    if (remainingKM > 0)
                    {
                        double percentageRemaining = ((double)remainingKM / kmIntervalVal) * 100;
                        lblRemainingKM.Text = $"{remainingKM:N0} كم ({percentageRemaining:F0}%)";

                        if (percentageRemaining <= 10)
                            lblRemainingKM.ForeColor = System.Drawing.Color.Red;
                        else if (percentageRemaining <= 25)
                            lblRemainingKM.ForeColor = System.Drawing.Color.Orange;
                        else
                            lblRemainingKM.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (remainingKM < 0)
                    {
                        lblRemainingKM.Text = $"متجاوز بـ {Math.Abs(remainingKM):N0} كم";
                        lblRemainingKM.ForeColor = System.Drawing.Color.Red;
                        lblRemainingKM.Font.Bold = true;
                    }
                    else
                    {
                        lblRemainingKM.Text = "يحتاج صيانة فورية";
                        lblRemainingKM.ForeColor = System.Drawing.Color.Red;
                        lblRemainingKM.Font.Bold = true;
                    }
                }
                else
                {
                    lblRemainingKM.Text = "غير محدد بالمسافة";
                }
            }

            // Show remaining days
            if (lblRemainingDays != null && rowView["RemainingDays"] is int remainingDays && rowView["TimeIntervalMonths"] is int timeIntervalVal)
            {
                if (timeIntervalVal > 0)
                {
                    if (remainingDays > 0)
                    {
                        double percentageRemaining = ((double)remainingDays / (timeIntervalVal * 30)) * 100;
                        lblRemainingDays.Text = $"{remainingDays} يوم ({percentageRemaining:F0}%)";

                        if (remainingDays <= 7)
                            lblRemainingDays.ForeColor = System.Drawing.Color.Red;
                        else if (remainingDays <= 30)
                            lblRemainingDays.ForeColor = System.Drawing.Color.Orange;
                        else
                            lblRemainingDays.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (remainingDays < 0)
                    {
                        lblRemainingDays.Text = $"متجاوز بـ {Math.Abs(remainingDays)} يوم";
                        lblRemainingDays.ForeColor = System.Drawing.Color.Red;
                        lblRemainingDays.Font.Bold = true;
                    }
                    else
                    {
                        lblRemainingDays.Text = "يحتاج صيانة فورية";
                        lblRemainingDays.ForeColor = System.Drawing.Color.Red;
                        lblRemainingDays.Font.Bold = true;
                    }
                }
                else
                {
                    lblRemainingDays.Text = "غير محدد بالزمن";
                }
            }

            // Show next due information
            if (lblNextDue != null)
            {
                string nextDueInfo = "";
                int kmIntervalVal2 = rowView["KMInterval"] as int? ?? 0;
                int nextDueKMVal = rowView["NextDueKM"] as int? ?? 0;
                int timeIntervalVal2 = rowView["TimeIntervalMonths"] as int? ?? 0;

                if (kmIntervalVal2 > 0 && nextDueKMVal > 0)
                {
                    nextDueInfo = $"{nextDueKMVal:N0} كم";
                }

                if (rowView["NextDueDate"] is DateTime nextDate && nextDate > DateTime.Now && nextDate.Year > 2000)
                {
                    if (!string.IsNullOrEmpty(nextDueInfo))
                        nextDueInfo += " أو ";

                    nextDueInfo += $"{nextDate:yyyy-MM-dd}";

                    int daysUntil = (int)(nextDate - DateTime.Now).TotalDays;
                    if (daysUntil > 0 && daysUntil <= 30)
                    {
                        nextDueInfo += $" (بعد {daysUntil} يوم)";
                    }
                }

                lblNextDue.Text = !string.IsNullOrEmpty(nextDueInfo) ? nextDueInfo : "غير محدد";
            }

            // Apply CSS classes based on alert level
            if (alertItem != null)
            {
                switch (alertLevel)
                {
                    case "danger":
                        alertItem.Attributes["class"] = "alert-item mb-3";
                        alertItem.Style.Add("border-right-color", "#dc3545");
                        alertItem.Style.Add("background-color", "#fff5f5");
                        break;
                    case "warning":
                        alertItem.Attributes["class"] = "alert-item alert-item-warning mb-3";
                        alertItem.Style.Add("border-right-color", "#ffc107");
                        alertItem.Style.Add("background-color", "#fffbf0");
                        break;
                    default:
                        alertItem.Attributes["class"] = "alert-item alert-item-success mb-3";
                        alertItem.Style.Add("border-right-color", "#28a745");
                        alertItem.Style.Add("background-color", "#f0fff4");
                        break;
                }
            }

            if (badgeSpan != null)
            {
                switch (alertLevel)
                {
                    case "danger":
                        badgeSpan.Attributes["class"] = "badge badge-danger";
                        break;
                    case "warning":
                        badgeSpan.Attributes["class"] = "badge badge-warning";
                        break;
                    default:
                        badgeSpan.Attributes["class"] = "badge badge-success";
                        break;
                }
            }
        }
        protected void GvMaintenanceHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMaintenanceHistory.PageIndex = e.NewPageIndex;
            if (ViewState["VehicleId"] != null)
            {
                int vehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    LoadMaintenanceHistory(conn, vehicleId);
                }
            }
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
            pnlVehicleInfo.Visible = false;
            pnlAlerts.Visible = false;
            pnlMaintenanceHistory.Visible = false;
            pnlStatistics.Visible = false;
            pnlNewMaintenanceAlert.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }
    }
}