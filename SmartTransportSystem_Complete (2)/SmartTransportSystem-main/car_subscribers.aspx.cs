using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class car_subscribers : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int currentVehicleId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlCarInfo.Visible = false;
                pnlSubscriberForm.Visible = false;
                pnlSubscriberHistory.Visible = false;
                LoadSubscribers(); // Load subscribers from subscription_details
                LoadRoutes(); // Load routes from routes table
                SetDefaultDates();

                // Grid starts empty
                gvSubscriberHistory.DataSource = null;
                gvSubscriberHistory.DataBind();
                pnlSubscriberHistory.Visible = false;
            }
        }

        private void SetDefaultDates()
        {
            txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtEndDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Load subscribers from subscription_details table
        /// Only loads records with route_id = 0 (pure subscribers from employee management)
        /// </summary>
        /// <summary>
        /// Load subscribers from subscription_details table
        /// </summary>
        private void LoadSubscribers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Simply load ALL records from subscription_details
                    // No filter on route_id needed
                    string query = @"
                SELECT 
                    n_id,
                    name,
                    phone,
                    address,
                    department,
                    position
                FROM subscription_details 
                WHERE name IS NOT NULL 
                  AND name != ''
                  AND LTRIM(RTRIM(name)) != ''
                ORDER BY name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlSubscriber.DataSource = dt;
                    ddlSubscriber.DataTextField = "name";
                    ddlSubscriber.DataValueField = "n_id";
                    ddlSubscriber.DataBind();
                    ddlSubscriber.Items.Insert(0, new ListItem("-- اختر المشترك --", ""));

                    if (dt.Rows.Count == 0)
                    {
                        ShowMessage("لا يوجد مشتركين. الرجاء إضافة مشتركين أولاً.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"تم تحميل {dt.Rows.Count} مشترك");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"خطأ في تحميل المشتركين: {ex.Message}");
                    ShowMessage("حدث خطأ في تحميل قائمة المشتركين");
                }
            }
        }



        private void LoadRoutes()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            route_id, 
                            route_name,
                            start_loc,
                            end_loc,
                            KM,
                            direction,
                            driver_name,
                            driver_phone
                        FROM routes 
                        WHERE (is_active = 1 OR is_active IS NULL)
                        ORDER BY route_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlRoute.DataSource = dt;
                    ddlRoute.DataTextField = "route_name";
                    ddlRoute.DataValueField = "route_id";
                    ddlRoute.DataBind();
                    ddlRoute.Items.Insert(0, new ListItem("-- اختر المسار --", ""));

                    if (dt.Rows.Count == 0)
                    {
                        ShowMessage("لا يوجد مسارات. الرجاء إضافة مسارات أولاً.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"خطأ في تحميل المسارات: {ex.Message}");
                    ShowMessage("حدث خطأ في تحميل قائمة المسارات");
                }
            }
        }

        /// <summary>
        /// Display route details when a route is selected
        /// </summary>
        protected void ddlRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlRoute.SelectedValue))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
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
                    System.Diagnostics.Debug.WriteLine($"خطأ: {ex.Message}");
                }
            }
            else
            {
                pnlRouteDetails.Visible = false;
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
                        lblCarNumber.Text = carNumberDisplay;

                        pnlCarInfo.Visible = true;
                        pnlSubscriberForm.Visible = true;

                        // Load subscriptions from the new vehicle_subscriptions table
                        LoadVehicleSubscriptions(conn, currentVehicleId);

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
                  AND plate_numbers = @Numbers";

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

        /// <summary>
        /// Load vehicle subscriptions from the vehicle_subscriptions table
        /// These are the records saved from this page
        /// </summary>
        private void LoadVehicleSubscriptions(SqlConnection conn, int vehicleId)
        {
            try
            {
                string query = @"
                    SELECT 
                        vs.subscription_id,
                        sd.name AS subscriber_name,
                        sd.phone,
                        sd.address,
                        sd.department,
                        sd.position,
                        r.route_name,
                        r.start_loc,
                        r.end_loc,
                        r.KM,
                        r.driver_name,
                        r.driver_phone,
                        vs.start_date,
                        vs.end_date,
                        vs.cost,
                        vs.notes,
                        vs.created_date,
                        CASE 
                            WHEN vs.end_date >= GETDATE() AND vs.is_active = 1 THEN 'نشط'
                            WHEN vs.end_date < GETDATE() THEN 'منتهي'
                            ELSE 'غير نشط'
                        END as status
                    FROM vehicle_subscriptions vs
                    INNER JOIN subscription_details sd ON vs.subscriber_id = sd.n_id
                    INNER JOIN routes r ON vs.route_id = r.route_id
                    WHERE vs.vehicle_id = @VehicleId
                    ORDER BY vs.created_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvSubscriberHistory.DataSource = dt;
                        gvSubscriberHistory.DataBind();
                        pnlSubscriberHistory.Visible = dt.Rows.Count > 0;

                        if (dt.Rows.Count == 0)
                        {
                            gvSubscriberHistory.EmptyDataText = "لا توجد اشتراكات مسجلة لهذه السيارة";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل الاشتراكات: {ex.Message}");
                pnlSubscriberHistory.Visible = false;
            }
        }

        protected void btnSaveSubscriber_Click(object sender, EventArgs e)
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

                double cost = string.IsNullOrEmpty(txtCost.Text) ? 0 : Convert.ToDouble(txtCost.Text);
                string notes = txtNotes.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int subscriberId = Convert.ToInt32(ddlSubscriber.SelectedValue);
                            int routeId = Convert.ToInt32(ddlRoute.SelectedValue);

                            // Insert into vehicle_subscriptions table
                            string insertSubscription = @"
                                INSERT INTO vehicle_subscriptions 
                                (vehicle_id, subscriber_id, route_id, start_date, end_date, cost, notes, created_date, is_active)
                                VALUES 
                                (@VehicleId, @SubscriberId, @RouteId, @StartDate, @EndDate, @Cost, @Notes, GETDATE(), 1);
                                SELECT SCOPE_IDENTITY();";

                            int newSubscriptionId;

                            using (SqlCommand cmd = new SqlCommand(insertSubscription, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@VehicleId", currentVehicleId);
                                cmd.Parameters.AddWithValue("@SubscriberId", subscriberId);
                                cmd.Parameters.AddWithValue("@RouteId", routeId);
                                cmd.Parameters.AddWithValue("@StartDate", startDate);
                                cmd.Parameters.AddWithValue("@EndDate", endDate);
                                cmd.Parameters.AddWithValue("@Cost", cost);
                                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes);

                                newSubscriptionId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            transaction.Commit();

                            // Get subscriber name for success message
                            string subscriberName = ddlSubscriber.SelectedItem.Text;

                            // Refresh the grid
                            LoadVehicleSubscriptions(conn, currentVehicleId);

                            // Clear form for next entry
                            ddlRoute.SelectedIndex = 0;
                            pnlRouteDetails.Visible = false;
                            txtCost.Text = "";
                            txtNotes.Text = "";
                            SetDefaultDates();

                            ShowSuccess($"تم تسجيل اشتراك {subscriberName} في المسار بنجاح");
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

        protected void gvSubscriberHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscriberHistory.PageIndex = e.NewPageIndex;

            if (ViewState["VehicleId"] != null)
            {
                int vehicleId = Convert.ToInt32(ViewState["VehicleId"]);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    LoadVehicleSubscriptions(conn, vehicleId);
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
    }
}