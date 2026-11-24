using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class car_location : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int currentVehicleId = 0;
        private int currentCarNumberId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlCarInfo.Visible = false;
                pnlRouteForm.Visible = false;
                pnlVehicleRoutes.Visible = false;
                LoadAllRoutes();
                SetDefaultDates();
            }

            // Handle postback from modal edit
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

        private void SetDefaultDates()
        {
            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            }
        }

        private void UpdateRoute(int routeId, string routeName, string description, string startLoc, string endLoc, decimal km, string direction, int subscriptionCount, bool isActive, DateTime startDate, DateTime endDate, string driverName, string driverPhone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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

                if (currentVehicleId > 0)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        LoadVehicleRoutes(conn, currentVehicleId);
                    }
                }

                ShowSuccess("تم تحديث المسار بنجاح");
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeModal();", true);
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحديث المسار: " + ex.Message);
            }
        }

        private void LoadAllRoutes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT route_id, route_name, description, start_loc, end_loc, 
                               KM, direction, subscription_count, is_active,
                               start_date, end_date, driver_name, driver_phone,
                               CASE 
                                   WHEN start_date <= GETDATE() AND end_date >= GETDATE() THEN 'نشط حالياً'
                                   WHEN start_date > GETDATE() THEN 'قادم'
                                   ELSE 'منتهي'
                               END as status
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
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل المسارات: {ex.Message}");
                lblNoRoutes.Visible = true;
                lblNoRoutes.Text = "حدث خطأ في تحميل المسارات";
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

                    currentCarNumberId = carNumberId;
                    currentVehicleId = GetVehicleId(conn, carNumberId);

                    if (currentVehicleId > 0)
                    {
                        ViewState["VehicleId"] = currentVehicleId;
                        ViewState["CarNumberId"] = currentCarNumberId;
                        lblCarNumber.Text = carNumberDisplay;

                        pnlCarInfo.Visible = true;
                        pnlRouteForm.Visible = true;

                        LoadVehicleRoutes(conn, currentVehicleId);
                        SetDefaultDates();

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
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في GetCarNumberId: {ex.Message}");
                return 0;
            }
        }

        private int GetVehicleId(SqlConnection conn, int carNumberId)
        {
            try
            {
                string query = "SELECT TOP 1 v_id FROM main_vehicle_details WHERE v_num = @CarNumberId ORDER BY v_id DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في GetVehicleId: {ex.Message}");
                return 0;
            }
        }

        private void LoadVehicleRoutes(SqlConnection conn, int vehicleId)
        {
            try
            {
                string query = @"
                    SELECT DISTINCT 
                        r.route_id, r.route_name, r.description, r.start_loc, r.end_loc, 
                        r.KM, r.direction, r.subscription_count, r.is_active,
                        r.start_date, r.end_date, r.driver_name, r.driver_phone,
                        CASE 
                            WHEN r.start_date <= GETDATE() AND r.end_date >= GETDATE() THEN 'نشط حالياً'
                            WHEN r.start_date > GETDATE() THEN 'قادم'
                            ELSE 'منتهي'
                        END as status
                    FROM routes r
                    INNER JOIN vehicle_route_assignment vra ON r.route_id = vra.route_id
                    WHERE vra.vehicle_id = @VehicleId
                    ORDER BY r.start_date DESC, r.route_name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvVehicleRoutes.DataSource = dt;
                        gvVehicleRoutes.DataBind();
                        pnlVehicleRoutes.Visible = dt.Rows.Count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل مسارات السيارة: {ex.Message}");
                pnlVehicleRoutes.Visible = false;
            }
        }

        protected void btnSaveRoute_Click(object sender, EventArgs e)
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

                if (string.IsNullOrEmpty(txtKM.Text))
                {
                    ShowMessage("الرجاء إدخال المسافة");
                    return;
                }

                DateTime startDate, endDate;
                if (!DateTime.TryParse(txtStartDate.Text, out startDate))
                {
                    startDate = DateTime.Now;
                }

                if (!DateTime.TryParse(txtEndDate.Text, out endDate))
                {
                    endDate = DateTime.Now.AddYears(1);
                }

                if (endDate < startDate)
                {
                    ShowMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            decimal km = Convert.ToDecimal(txtKM.Text);
                            int subscriptionCount = string.IsNullOrEmpty(txtSubscriptionCount.Text) ? 0 : Convert.ToInt32(txtSubscriptionCount.Text);

                            // Insert into routes table ONLY (no subscription_details)
                            string insertRoute = @"
                                INSERT INTO routes (route_name, description, start_loc, end_loc, KM, direction, subscription_count, is_active, start_date, end_date, driver_name, driver_phone)
                                VALUES (@RouteName, @Description, @StartLoc, @EndLoc, @KM, @Direction, @SubscriptionCount, 1, @StartDate, @EndDate, @DriverName, @DriverPhone);
                                SELECT SCOPE_IDENTITY();";

                            int routeId;
                            using (SqlCommand cmd = new SqlCommand(insertRoute, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RouteName", ddlRouteName.SelectedValue);
                                cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim());
                                cmd.Parameters.AddWithValue("@StartLoc", ddlStartLoc.SelectedValue);
                                cmd.Parameters.AddWithValue("@EndLoc", ddlEndLoc.SelectedValue);
                                cmd.Parameters.AddWithValue("@KM", km);
                                cmd.Parameters.AddWithValue("@Direction", string.IsNullOrEmpty(txtDirection.Text) ? (object)DBNull.Value : txtDirection.Text.Trim());
                                cmd.Parameters.AddWithValue("@SubscriptionCount", subscriptionCount);
                                cmd.Parameters.AddWithValue("@StartDate", startDate);
                                cmd.Parameters.AddWithValue("@EndDate", endDate);
                                cmd.Parameters.AddWithValue("@DriverName", string.IsNullOrEmpty(txtDriverName.Text) ? (object)DBNull.Value : txtDriverName.Text.Trim());
                                cmd.Parameters.AddWithValue("@DriverPhone", string.IsNullOrEmpty(txtDriverPhone.Text) ? (object)DBNull.Value : txtDriverPhone.Text.Trim());

                                routeId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Link route to vehicle using vehicle_route_assignment table
                            string assignRouteToVehicle = @"
                                INSERT INTO vehicle_route_assignment (vehicle_id, route_id, assigned_date)
                                VALUES (@VehicleId, @RouteId, GETDATE())";

                            using (SqlCommand cmd = new SqlCommand(assignRouteToVehicle, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LoadVehicleRoutes(conn, currentVehicleId);
                            LoadAllRoutes();
                            ClearForm();
                            SetDefaultDates();
                            ShowSuccess("تم حفظ المسار بنجاح وربطه بالسيارة");
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

        protected void EditRoute_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int routeId = Convert.ToInt32(btn.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Check if route has any subscriptions
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

                            // Delete from vehicle_route_assignment first
                            string deleteAssignment = "DELETE FROM vehicle_route_assignment WHERE route_id = @RouteId";
                            using (SqlCommand cmd = new SqlCommand(deleteAssignment, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete from routes
                            string deleteRoute = "DELETE FROM routes WHERE route_id = @RouteId";
                            using (SqlCommand cmd = new SqlCommand(deleteRoute, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LoadAllRoutes();

                            if (currentVehicleId > 0)
                            {
                                LoadVehicleRoutes(conn, currentVehicleId);
                            }

                            ShowSuccess("تم حذف المسار بنجاح");
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

                if (currentVehicleId == 0 && ViewState["VehicleId"] != null)
                {
                    currentVehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                }

                if (currentVehicleId == 0)
                {
                    ShowMessage("الرجاء البحث عن السيارة أولاً");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Check if already assigned
                            string checkQuery = @"
                                SELECT COUNT(*) 
                                FROM vehicle_route_assignment 
                                WHERE route_id = @RouteId AND vehicle_id = @VehicleId";

                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@RouteId", routeId);
                                checkCmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);

                                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (exists > 0)
                                {
                                    ShowMessage("هذا المسار مرتبط بالفعل بهذه السيارة");
                                    return;
                                }
                            }

                            // Assign route to vehicle
                            string assignRoute = @"
                                INSERT INTO vehicle_route_assignment (vehicle_id, route_id, assigned_date)
                                VALUES (@VehicleId, @RouteId, GETDATE())";

                            using (SqlCommand cmd = new SqlCommand(assignRoute, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LoadVehicleRoutes(conn, currentVehicleId);

                            // Get route name for message
                            string routeName = "";
                            string getNameQuery = "SELECT route_name FROM routes WHERE route_id = @RouteId";
                            using (SqlCommand getNameCmd = new SqlCommand(getNameQuery, conn))
                            {
                                getNameCmd.Parameters.AddWithValue("@RouteId", routeId);
                                object result = getNameCmd.ExecuteScalar();
                                if (result != null) routeName = result.ToString();
                            }

                            ShowSuccess($"تم ربط المسار '{routeName}' بالسيارة بنجاح");
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

        protected void gvVehicleRoutes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVehicleRoutes.PageIndex = e.NewPageIndex;
            if (currentVehicleId > 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    LoadVehicleRoutes(conn, currentVehicleId);
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("vehicle_management.aspx");
        }

        private void ClearForm()
        {
            ddlRouteName.SelectedIndex = 0;
            txtDescription.Text = "";
            ddlStartLoc.SelectedIndex = 0;
            ddlEndLoc.SelectedIndex = 0;
            txtKM.Text = "";
            txtDirection.Text = "";
            txtSubscriptionCount.Text = "";
            txtDriverName.Text = "";
            txtDriverPhone.Text = "";
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