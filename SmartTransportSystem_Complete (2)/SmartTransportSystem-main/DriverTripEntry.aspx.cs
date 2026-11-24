using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class DriverTripEntry : System.Web.UI.Page
    {
        // operation_command.trip_status values
        private enum TripStatus
        {
            Pending = 0,   // لم تبدأ
            OnRoad = 1,    // في الطريق
            Finished = 2,  // منتهية
            Broken = 3     // معطلة
        }

        private static string ConnString
        {
            get { return ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString; }
        }

        // True when a driver account (any non-admin) is logged in
        private bool IsDriverMode
        {
            get { return AuthHelper.IsRestrictedUser; }
        }

        // SQL expression: the car plate shown in the car dropdown
        private const string CarDisplaySql =
            "RTRIM(cn.letter1) + ' ' + ISNULL(RTRIM(cn.letter2), '') + " +
            "CASE WHEN cn.letter3 IS NOT NULL AND RTRIM(cn.letter3) <> '' THEN ' ' + RTRIM(cn.letter3) ELSE '' END + " +
            "' ' + RTRIM(cn.plate_numbers)";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Driver only: hide mission-number / driver-name fields (runs on every request)
            if (IsDriverMode)
            {
                ApplyDriverMode();
            }

            if (!IsPostBack)
            {
                HideAllMessages();
                workOrdersPanel.Visible = false;
                missionInfo.Visible = false;

                // Driver only: load his cars, then list his own orders immediately
                if (IsDriverMode)
                {
                    LoadDriverCars(false);
                    SearchWorkOrders();
                }
            }
        }

        /// <summary>
        /// Driver account: hides mission number + driver name, and uses the name of the
        /// driver that was registered in the driver registration page.
        /// </summary>
        private void ApplyDriverMode()
        {
            colMissionNumber.Visible = false;
            colDriverName.Visible = false;
            rowSearchNote.Visible = false;
            searchHeader.InnerHtml = "<i class=\"fas fa-list\"></i> مأمورياتي";

            if (!IsPostBack)
            {
                int? empId = AuthHelper.CurrentEmployeeId;
                txtDriverNameSearch.Text = empId.HasValue ? GetEmployeeName(empId.Value) : "";
            }
        }

        private string GetEmployeeName(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT employee_name FROM employees WHERE employee_id = @id", conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = employeeId;
                    conn.Open();
                    object o = cmd.ExecuteScalar();
                    return (o == null || o == DBNull.Value) ? "" : o.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        #region Driver cars (الاستعلام)

        /// <summary>
        /// Fills the car dropdown with ONLY the cars of the driver.
        /// A car belongs to the driver when:
        ///   - a line (routes row) has routes.driver_id = driver and routes.car_id = that car, or
        ///   - the driver has an unfinished order on that car.
        /// Driver account: the driver comes from the login.
        /// Admin: the driver(s) are the ones matching the typed name.
        /// One car   -> only that car is listed (it is the filter).
        /// Many cars -> "all my cars" + each car, so the driver can choose.
        /// </summary>
        private void LoadDriverCars(bool keepSelection)
        {
            string previous = (keepSelection && ddlCar.Items.Count > 0) ? ddlCar.SelectedValue : null;

            ddlCar.Items.Clear();
            colCar.Visible = false;

            bool driverMode = IsDriverMode;
            int empId = 0;
            string driverName = txtDriverNameSearch.Text.Trim();

            if (driverMode)
            {
                int? id = AuthHelper.CurrentEmployeeId;
                if (!id.HasValue) return;
                empId = id.Value;
            }
            else
            {
                if (string.IsNullOrEmpty(driverName)) return;
            }

            try
            {
                string driverCond = driverMode ? "e.employee_id = @empId" : "e.employee_name LIKE @driverName";

                string query = @"
                    SELECT DISTINCT x.car_number_id, x.car_display
                    FROM (
                        SELECT cn.car_number_id, " + CarDisplaySql + @" AS car_display
                        FROM routes r
                        INNER JOIN employees e ON r.driver_id = e.employee_id
                        INNER JOIN main_vehicle_details mvd ON r.car_id = mvd.v_id
                        INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                        WHERE " + driverCond + @"
                          AND r.car_id IS NOT NULL
                          AND cn.is_active = 1

                        UNION ALL

                        SELECT cn.car_number_id, " + CarDisplaySql + @" AS car_display
                        FROM operation_command oc
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        WHERE " + driverCond + @"
                          AND oc.deleted = 0
                          AND ISNULL(oc.trip_status, 0) <> 2
                    ) x
                    ORDER BY x.car_display";

                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(ConnString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (driverMode)
                        cmd.Parameters.Add("@empId", SqlDbType.Int).Value = empId;
                    else
                        cmd.Parameters.Add("@driverName", SqlDbType.NVarChar, 200).Value = "%" + EscapeLike(driverName) + "%";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                if (dt.Rows.Count == 0) return; // no cars found: no car filter

                // More than one car: let the driver choose (or see all his cars)
                if (dt.Rows.Count > 1)
                    ddlCar.Items.Add(new ListItem("-- كل سياراتي --", "0"));

                foreach (DataRow row in dt.Rows)
                {
                    ddlCar.Items.Add(new ListItem(row["car_display"].ToString().Trim(), row["car_number_id"].ToString()));
                }

                if (previous != null && ddlCar.Items.FindByValue(previous) != null)
                    ddlCar.SelectedValue = previous;

                colCar.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading driver cars: {ex.Message}");
                ddlCar.Items.Clear();
                colCar.Visible = false;
            }
        }

        // Admin: when the driver name is typed, show ONLY the cars of that driver.
        // Nothing is searched here - the user chooses the car and presses the search button.
        protected void txtDriverNameSearch_TextChanged(object sender, EventArgs e)
        {
            if (IsDriverMode) return;

            missionInfo.Visible = false;
            workOrdersPanel.Visible = false;
            ClearForm();
            ViewState["CurrentMissionId"] = null;

            string name = txtDriverNameSearch.Text.Trim();

            LoadDriverCars(false);
            ViewState["LastDriverName"] = name;

            if (!string.IsNullOrEmpty(name) && !colCar.Visible)
                ShowMessage("لا توجد سيارات مسجلة لهذا السائق");
            else
                HideAllMessages();
        }

        #endregion

        #region Status helpers

        protected string GetStatusText(object status)
        {
            return GetStatusTextInternal(ToStatus(status));
        }

        protected string GetStatusClass(object status)
        {
            return GetStatusClassInternal(ToStatus(status));
        }

        private static TripStatus ToStatus(object status)
        {
            if (status == null || status == DBNull.Value)
                return TripStatus.Pending;
            int s = Convert.ToInt32(status);
            if (s < 0 || s > 3) return TripStatus.Pending;
            return (TripStatus)s;
        }

        private static string GetStatusTextInternal(TripStatus s)
        {
            switch (s)
            {
                case TripStatus.OnRoad: return "في الطريق";
                case TripStatus.Finished: return "منتهية";
                case TripStatus.Broken: return "معطلة";
                default: return "لم تبدأ";
            }
        }

        private static string GetStatusClassInternal(TripStatus s)
        {
            switch (s)
            {
                case TripStatus.OnRoad: return "trip-badge trip-onroad";
                case TripStatus.Finished: return "trip-badge trip-finished";
                case TripStatus.Broken: return "trip-badge trip-broken";
                default: return "trip-badge trip-pending";
            }
        }

        #endregion

        #region Search

        private void SearchWorkOrders()
        {
            try
            {
                bool driverMode = IsDriverMode;
                int driverEmpId = 0;
                string driverName = txtDriverNameSearch.Text.Trim();
                string missionNumber = txtMissionNumberSearch.Text.Trim();

                int missionNumberInt = 0;
                bool hasMissionNumber = false;

                if (driverMode)
                {
                    // Driver: always only his own orders, taken from the login (never from the page)
                    int? empId = AuthHelper.CurrentEmployeeId;
                    if (!empId.HasValue)
                    {
                        ShowMessage("حسابك غير مرتبط بسائق. الرجاء التواصل مع مدير النظام");
                        workOrdersPanel.Visible = false;
                        return;
                    }
                    driverEmpId = empId.Value;
                }
                else
                {
                    // ----- Admin: original behaviour -----
                    if (string.IsNullOrEmpty(driverName))
                    {
                        ShowMessage("الرجاء إدخال اسم السائق للبحث");
                        return;
                    }

                    hasMissionNumber = !string.IsNullOrEmpty(missionNumber);
                    if (hasMissionNumber)
                    {
                        if (!int.TryParse(missionNumber, out missionNumberInt) || missionNumberInt <= 0)
                        {
                            ShowMessage("رقم المأمورية غير صحيح");
                            return;
                        }
                    }
                }

                // Selected car (0 = no car filter). Only the cars of the driver are in the list.
                int carId = 0;
                bool hasCar = colCar.Visible
                              && int.TryParse(ddlCar.SelectedValue, out carId)
                              && carId > 0;

                // Escape LIKE wildcards in user input
                string safeDriverName = EscapeLike(driverName);

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    // Every order that is not finished is listed, whatever its date.
                    string query = @"
                        SELECT 
                            oc.mission_id,
                            e.employee_name as driver_name,
                            cn.letter1 + ' ' + ISNULL(cn.letter2, '') + ' ' + 
                            ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers as car_number,
                            CASE 
                                WHEN oc.mission_type = 1 THEN N'مأمورية'
                                WHEN oc.mission_type = 2 THEN N'خط'
                                ELSE N'غير معروف'
                            END as mission_type,
                            oc.date AS order_date,
                            ISNULL(oc.trip_status, 0) AS trip_status
                        FROM operation_command oc
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        WHERE oc.deleted = 0
                          AND ISNULL(oc.trip_status, 0) <> 2";

                    if (driverMode)
                        query += " AND oc.driver_name = @empId AND ISNULL(oc.hide, 0) = 0";
                    else
                        query += " AND e.employee_name LIKE @driverName";

                    if (!driverMode && hasMissionNumber)
                        query += " AND oc.mission_id = @missionNumber";

                    if (hasCar)
                        query += " AND oc.v_number = @carId";

                    query += " ORDER BY oc.mission_id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (driverMode)
                            cmd.Parameters.Add("@empId", SqlDbType.Int).Value = driverEmpId;
                        else
                            cmd.Parameters.Add("@driverName", SqlDbType.NVarChar, 200).Value = "%" + safeDriverName + "%";

                        if (!driverMode && hasMissionNumber)
                            cmd.Parameters.Add("@missionNumber", SqlDbType.Int).Value = missionNumberInt;

                        if (hasCar)
                            cmd.Parameters.Add("@carId", SqlDbType.Int).Value = carId;

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        gvWorkOrders.DataSource = dt;
                        gvWorkOrders.DataBind();

                        workOrdersPanel.Visible = dt.Rows.Count > 0;

                        string carText = hasCar ? " على السيارة " + ddlCar.SelectedItem.Text : "";

                        if (dt.Rows.Count == 0)
                        {
                            if (driverMode)
                                ShowMessage("لا توجد مأموريات غير منتهية مسجلة لك حالياً" + carText);
                            else if (hasMissionNumber)
                                ShowMessage($"لا توجد مأمورية غير منتهية برقم {missionNumber} للسائق '{driverName}'{carText}");
                            else
                                ShowMessage($"لا توجد مأموريات غير منتهية للسائق '{driverName}'{carText}");
                        }
                        else
                        {
                            if (driverMode)
                                ShowSuccessMessage($"لديك {dt.Rows.Count} مأمورية{carText}");
                            else
                                ShowSuccessMessage($"عُثر على {dt.Rows.Count} مأمورية للسائق '{driverName}'{carText}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching work orders: {ex.Message}");
                ShowMessage("خطأ في البحث: " + ex.Message);
            }
        }

        private static string EscapeLike(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            // Admin: (re)load the cars of the typed driver. The previous choice is kept
            // only when the driver name did not change.
            if (!IsDriverMode)
            {
                string currentName = txtDriverNameSearch.Text.Trim();
                string lastName = ViewState["LastDriverName"] as string;
                bool sameDriver = string.Equals(currentName, lastName, StringComparison.Ordinal);

                LoadDriverCars(sameDriver);
                ViewState["LastDriverName"] = currentName;
            }

            SearchWorkOrders();
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            missionInfo.Visible = false;
            ClearForm();
            ViewState["CurrentMissionId"] = null;

            if (IsDriverMode || !string.IsNullOrEmpty(txtDriverNameSearch.Text.Trim()))
                SearchWorkOrders();
        }

        protected void gvWorkOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SelectMission") return;

            try
            {
                int missionId = Convert.ToInt32(e.CommandArgument);

                // ---- Security: verify current user owns this mission ----
                if (!CanCurrentUserManageMission(missionId))
                {
                    ShowMessage("لا تملك صلاحية إدارة هذه المأمورية");
                    return;
                }

                int status = LoadMissionData(missionId);

                if (status == -1) return; // error already shown

                switch ((TripStatus)status)
                {
                    case TripStatus.Pending:
                        if (!lblWarning.Visible)
                            ShowInfo("اضغط «بدء الرحلة» عند انطلاق السيارة");
                        break;
                    case TripStatus.OnRoad:
                        ShowInfo("الرحلة في الطريق. أدخل عداد العودة ثم اضغط «إنهاء الرحلة»، أو سجّل عطلاً إن تعطلت السيارة");
                        break;
                    case TripStatus.Broken:
                        ShowInfo("الرحلة معطلة. اضغط «استئناف الرحلة» أو أدخل عداد العودة ثم «إنهاء الرحلة»");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في اختيار المأمورية: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in RowCommand: {ex.Message}");
            }
        }

        /// <summary>
        /// Driver account: STRICT - only a mission assigned to him (a driver with no linked employee is denied).
        /// Admin: the original rule (no employee in session = allowed; otherwise ownership is enforced).
        /// </summary>
        private bool CanCurrentUserManageMission(int missionId)
        {
            int? empIdNullable = AuthHelper.CurrentEmployeeId;

            if (IsDriverMode)
            {
                if (!empIdNullable.HasValue) return false;
            }
            else
            {
                // Original behaviour
                object sessionEmp = Session["employee_id"];
                if (sessionEmp == null) return true;
                if (!empIdNullable.HasValue) return false;
            }

            int employeeId = empIdNullable.Value;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT driver_name FROM operation_command WHERE mission_id = @id AND deleted = 0", conn))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = missionId;
                        object obj = cmd.ExecuteScalar();
                        if (obj == null || obj == DBNull.Value) return false;
                        return Convert.ToInt32(obj) == employeeId;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Load a mission into the panel

        // Fills the info panel and sets which buttons are enabled.
        // Returns the trip status, or -1 when the mission was not found.
        private int LoadMissionData(int missionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            oc.mission_id,
                            oc.mission_type,
                            oc.mission_description,
                            oc.location_point,
                            oc.kilometer_start,
                            oc.kilometer_end,
                            oc.mission_starting_time,
                            oc.mission_ending_time,
                            oc.date as work_order_date,
                            ISNULL(oc.trip_status, 0) AS trip_status,
                            cn.letter1 + ' ' + ISNULL(cn.letter2, '') + ' ' + 
                            ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers as car_number,
                            e.employee_name as driver_name,
                            r.route_name
                        FROM operation_command oc
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        LEFT JOIN routes r ON oc.location_point = r.route_id AND oc.mission_type = 2
                        WHERE oc.mission_id = @missionId
                          AND oc.deleted = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@missionId", SqlDbType.Int).Value = missionId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                ShowMessage("لم يتم العثور على المأمورية رقم " + missionId);
                                missionInfo.Visible = false;
                                return -1;
                            }

                            DateTime workOrderDate = Convert.ToDateTime(reader["work_order_date"]);
                            bool isToday = workOrderDate.Date == DateTime.Today;
                            TripStatus status = ToStatus(reader["trip_status"]);

                            // ----- mission info -----
                            txtCarNumber.Text = reader["car_number"].ToString();
                            lblDriverName.Text = reader["driver_name"].ToString();

                            string missionType = reader["mission_type"].ToString();
                            lblMissionType.Text = missionType == "1" ? "مأمورية" : "خط";
                            lblMissionDetails.Text = missionType == "1"
                                ? (reader["mission_description"] != DBNull.Value ? reader["mission_description"].ToString() : "")
                                : (reader["route_name"] != DBNull.Value ? reader["route_name"].ToString() : "");

                            lblWorkOrderDate.Text = workOrderDate.ToString("yyyy-MM-dd");

                            // ----- trip status badge -----
                            lblTripStatus.Text = GetStatusTextInternal(status);
                            lblTripStatus.CssClass = GetStatusClassInternal(status);

                            // ----- kilometers / times (display only; the buttons fill them) -----
                            txtStartKM.Text = (reader["kilometer_start"] != DBNull.Value && Convert.ToDecimal(reader["kilometer_start"]) > 0)
                                ? Convert.ToDecimal(reader["kilometer_start"]).ToString("0.##", CultureInfo.InvariantCulture) : "";

                            txtEndKM.Text = (reader["kilometer_end"] != DBNull.Value && Convert.ToDecimal(reader["kilometer_end"]) > 0)
                                ? Convert.ToDecimal(reader["kilometer_end"]).ToString("0.##", CultureInfo.InvariantCulture) : "";

                            txtStartTime.Text = reader["mission_starting_time"] != DBNull.Value
                                ? Convert.ToDateTime(reader["mission_starting_time"]).ToString("yyyy-MM-ddTHH:mm") : "";

                            txtReturnTime.Text = reader["mission_ending_time"] != DBNull.Value
                                ? Convert.ToDateTime(reader["mission_ending_time"]).ToString("yyyy-MM-ddTHH:mm") : "";

                            txtStartKM.ReadOnly = true;
                            txtStartKM.BackColor = Color.FromName("#F8F9FA");

                            // ----- which buttons are allowed -----
                            bool canStart = status == TripStatus.Pending || status == TripStatus.Broken;
                            bool canEnd = status == TripStatus.OnRoad || status == TripStatus.Broken;
                            bool canBreak = status == TripStatus.OnRoad;

                            btnStartTrip.Enabled = canStart;
                            btnStartTrip.Text = status == TripStatus.Broken ? "استئناف الرحلة" : "بدء الرحلة";
                            btnBreakdown.Enabled = canBreak;
                            btnEndTrip.Enabled = canEnd;

                            txtEndKM.Enabled = canEnd;
                            txtEndKM.ReadOnly = !canEnd;
                            txtEndKM.BackColor = canEnd ? Color.White : Color.FromName("#F8F9FA");

                            lblWarning.Visible = false;
                            lblInfo.Visible = false;

                            if (status == TripStatus.Finished)
                            {
                                ShowWarning("هذه الرحلة منتهية ولا يمكن تعديلها");
                            }
                            else if (status == TripStatus.Pending && !isToday)
                            {
                                ShowWarning($"تنبيه: تاريخ أمر الشغل ({workOrderDate:yyyy-MM-dd}) ليس اليوم. يمكنك بدء الرحلة أو حذف الأمر إن كان غير مستخدم");
                            }

                            ViewState["CurrentMissionId"] = missionId;

                            missionInfo.Visible = true;
                            workOrdersPanel.Visible = false;

                            return (int)status;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحميل البيانات: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in LoadMissionData: {ex.Message}");
                return -1;
            }
        }

        #endregion

        #region Trip buttons

        private bool TryGetCurrentMissionId(out int missionId)
        {
            missionId = 0;
            if (ViewState["CurrentMissionId"] == null)
            {
                ShowMessage("الرجاء اختيار المأمورية أولاً");
                return false;
            }
            missionId = Convert.ToInt32(ViewState["CurrentMissionId"]);
            return true;
        }

        // ---- بدء الرحلة / استئناف الرحلة ----
        protected void btnStartTrip_Click(object sender, EventArgs e)
        {
            try
            {
                int missionId;
                if (!TryGetCurrentMissionId(out missionId)) return;

                if (!CanCurrentUserManageMission(missionId))
                {
                    ShowMessage("لا تملك صلاحية إدارة هذه المأمورية");
                    return;
                }

                DateTime now = DateTime.Now;

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE operation_command
                        SET trip_status = 1,
                            mission_starting_time = ISNULL(mission_starting_time, @now)
                        WHERE mission_id = @id
                          AND deleted = 0
                          AND ISNULL(trip_status, 0) IN (0, 3)";

                    int rows;
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = missionId;
                        cmd.Parameters.Add("@now", SqlDbType.DateTime).Value = now;
                        rows = cmd.ExecuteNonQuery();
                    }

                    LoadMissionData(missionId);

                    if (rows > 0)
                        ShowSuccessMessage("تم بدء الرحلة. الحالة: في الطريق");
                    else
                        ShowMessage("تعذر بدء الرحلة. ربما تغيرت حالتها، راجع الحالة الظاهرة");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في بدء الرحلة: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in btnStartTrip_Click: {ex.Message}");
            }
        }

        // ---- تسجيل عطل ----
        protected void btnBreakdown_Click(object sender, EventArgs e)
        {
            try
            {
                int missionId;
                if (!TryGetCurrentMissionId(out missionId)) return;

                if (!CanCurrentUserManageMission(missionId))
                {
                    ShowMessage("لا تملك صلاحية إدارة هذه المأمورية");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    int rows;
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE operation_command SET trip_status = 3 WHERE mission_id = @id AND deleted = 0 AND trip_status = 1", conn))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = missionId;
                        rows = cmd.ExecuteNonQuery();
                    }

                    LoadMissionData(missionId);

                    if (rows > 0)
                        ShowSuccessMessage("تم تسجيل العطل. الحالة: معطلة");
                    else
                        ShowMessage("تعذر تسجيل العطل. يمكن تسجيله فقط للرحلة التي في الطريق");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تسجيل العطل: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in btnBreakdown_Click: {ex.Message}");
            }
        }

        // ---- إنهاء الرحلة ----
        protected void btnEndTrip_Click(object sender, EventArgs e)
        {
            try
            {
                int missionId;
                if (!TryGetCurrentMissionId(out missionId)) return;

                if (!CanCurrentUserManageMission(missionId))
                {
                    ShowMessage("لا تملك صلاحية إدارة هذه المأمورية");
                    return;
                }

                // Accept both '.' and ',' as decimal separator
                string rawEnd = (txtEndKM.Text ?? "").Trim().Replace(',', '.');
                decimal endKM;
                if (!decimal.TryParse(rawEnd, NumberStyles.Any, CultureInfo.InvariantCulture, out endKM) || endKM <= 0)
                {
                    ShowMessage("الرجاء إدخال عداد العودة قبل إنهاء الرحلة");
                    return;
                }

                // Reject more than 2 decimal places
                if (decimal.Round(endKM, 2) != endKM)
                {
                    ShowMessage("عداد العودة يقبل منزلتين عشريتين فقط");
                    return;
                }

                DateTime now = DateTime.Now;

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    decimal startKM = 0;
                    using (SqlCommand getCmd = new SqlCommand(
                        "SELECT ISNULL(kilometer_start, 0) FROM operation_command WHERE mission_id = @id AND deleted = 0", conn))
                    {
                        getCmd.Parameters.Add("@id", SqlDbType.Int).Value = missionId;
                        object startObj = getCmd.ExecuteScalar();
                        if (startObj != null && startObj != DBNull.Value)
                            startKM = Convert.ToDecimal(startObj);
                    }

                    if (startKM > 0 && endKM <= startKM)
                    {
                        ShowMessage("عداد العودة يجب أن يكون أكبر من عداد البداية");
                        return;
                    }

                    string sql = @"
                        UPDATE operation_command
                        SET mission_ending_time = @now,
                            kilometer_end = @endKM,
                            trip_status = 2
                        WHERE mission_id = @id
                          AND deleted = 0
                          AND trip_status IN (1, 3)";

                    int rows;
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = missionId;
                        cmd.Parameters.Add("@now", SqlDbType.DateTime).Value = now;
                        SqlParameter pEnd = cmd.Parameters.Add("@endKM", SqlDbType.Decimal);
                        pEnd.Precision = 18;
                        pEnd.Scale = 2;
                        pEnd.Value = endKM;

                        rows = cmd.ExecuteNonQuery();
                    }

                    LoadMissionData(missionId);

                    if (rows > 0)
                        ShowSuccessMessage("تم إنهاء الرحلة. الحالة: منتهية. السيارة متاحة الآن لأوامر شغل جديدة");
                    else
                        ShowMessage("تعذر إنهاء الرحلة. يجب أن تكون الرحلة قد بدأت أولاً");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في إنهاء الرحلة: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in btnEndTrip_Click: {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        private void ClearForm()
        {
            txtCarNumber.Text = "";
            lblDriverName.Text = "";
            lblMissionType.Text = "";
            lblMissionDetails.Text = "";
            lblWorkOrderDate.Text = "";
            lblTripStatus.Text = "";
            lblTripStatus.CssClass = "trip-badge trip-pending";
            txtStartTime.Text = "";
            txtReturnTime.Text = "";
            txtStartKM.Text = "";
            txtEndKM.Text = "";
            txtEndKM.ReadOnly = false;
            txtEndKM.BackColor = Color.White;
        }

        private void HideAllMessages()
        {
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
            lblWarning.Visible = false;
            lblInfo.Visible = false;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
            lblWarning.Visible = false;
            lblInfo.Visible = false;
        }

        private void ShowSuccessMessage(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
            lblWarning.Visible = false;
            lblInfo.Visible = false;
        }

        private void ShowWarning(string message)
        {
            lblWarning.Text = message;
            lblWarning.Visible = true;
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
            lblInfo.Visible = false;
        }

        private void ShowInfo(string message)
        {
            lblInfo.Text = message;
            lblInfo.Visible = true;
            // Do NOT hide lblWarning — warnings may be shown at the same time.
        }

        #endregion
    }
}