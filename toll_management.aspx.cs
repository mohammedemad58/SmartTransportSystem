using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class toll_management : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMissions();
                LoadAllGateDropdowns();
                LoadGatesList();
                LoadCarsForSubscription();
                LoadMissionsForSubscription();

                // Hide statistics initially
                statsContainer.Visible = false;
                searchResults.Visible = false;
            }
            else
            {
                // On postback, restore the active tab
                if (!string.IsNullOrEmpty(hfActiveTab.Value))
                {
                    string script = $"activateTab('{hfActiveTab.Value}');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "RestoreTab", script, true);
                }
            }
        }

        #region Load Gate Dropdowns - Unified Method

        private void LoadAllGateDropdowns()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            gate_id,
                            gate_name,
                            ISNULL(location, '') as location,
                            ISNULL(toll_amount, 0) as toll_amount
                        FROM toll_gates
                        WHERE is_active = 1
                        ORDER BY gate_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                List<ListItem> gateItems = new List<ListItem>();
                foreach (DataRow row in dt.Rows)
                {
                    string gateId = row["gate_id"].ToString();
                    string gateName = row["gate_name"].ToString();
                    string location = row["location"].ToString();
                    decimal amount = Convert.ToDecimal(row["toll_amount"]);

                    string displayText = $"{gateName} - {location}";
                    if (amount > 0)
                    {
                        displayText += $" ({amount:N2} ج.م)";
                    }

                    gateItems.Add(new ListItem(displayText, gateId));
                }

                // Load all three dropdowns
                ddlGate.Items.Clear();
                ddlGate.Items.Add(new ListItem("-- اختر البوابة --", "0"));
                foreach (ListItem item in gateItems)
                {
                    ddlGate.Items.Add(item);
                }

                ddlSearchGate.Items.Clear();
                ddlSearchGate.Items.Add(new ListItem("-- الكل --", "0"));
                foreach (ListItem item in gateItems)
                {
                    ddlSearchGate.Items.Add(item);
                }

                ddlSubGate.Items.Clear();
                ddlSubGate.Items.Add(new ListItem("-- اختر البوابة --", "0"));
                foreach (ListItem item in gateItems)
                {
                    ddlSubGate.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading gate dropdowns: {ex.Message}");
            }
        }

        #endregion

        #region Load Data Methods

        private void LoadMissions()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT DISTINCT
                            oc.mission_id,
                            cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers + N' - ' +
                            ISNULL(e.employee_name, N'غير محدد') + N' - ' +
                            CONVERT(nvarchar, oc.date, 23) as mission_display
                        FROM operation_command oc
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        LEFT JOIN employees e ON oc.driver_name = e.employee_id
                        WHERE oc.v_number IS NOT NULL
                        ORDER BY mission_display";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Load mission dropdown for transaction
                    ddlMission.Items.Clear();
                    ddlMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
                    if (dt.Rows.Count > 0)
                    {
                        ddlMission.DataSource = dt;
                        ddlMission.DataTextField = "mission_display";
                        ddlMission.DataValueField = "mission_id";
                        ddlMission.DataBind();
                    }

                    // Load mission dropdown for subscription
                    ddlSubMission.Items.Clear();
                    ddlSubMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
                    if (dt.Rows.Count > 0)
                    {
                        ddlSubMission.DataSource = dt;
                        ddlSubMission.DataTextField = "mission_display";
                        ddlSubMission.DataValueField = "mission_id";
                        ddlSubMission.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading missions: {ex.Message}");
            }
        }

        private void LoadGatesList()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY gate_name) AS RowNum,
                    gate_id,
                    gate_name,
                    ISNULL(location, '') as location,
                    ISNULL(toll_amount, 0) as toll_amount,
                    is_active
                FROM toll_gates
                ORDER BY gate_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    gvGates.DataSource = dt;
                    gvGates.DataBind();
                    gvGates.Visible = true;
                }
                else
                {
                    gvGates.Visible = true;
                    gvGates.DataSource = null;
                    gvGates.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading gates list: {ex.Message}");
                gvGates.Visible = true;
                gvGates.EmptyDataText = $"خطأ في تحميل البيانات: {ex.Message}";
                gvGates.DataSource = null;
                gvGates.DataBind();
            }
        }

        private void LoadStatisticsWithFilter(List<SqlParameter> parameters)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM vehicle_toll_subscriptions) + 
                        (SELECT COUNT(*) FROM toll_payments) as total_transactions,
                        
                        ISNULL((SELECT ISNULL(SUM(subscription_amount), 0) FROM vehicle_toll_subscriptions), 0) + 
                        ISNULL((SELECT ISNULL(SUM(amount_paid), 0) FROM toll_payments), 0) as total_amount,
                        
                        ISNULL((SELECT COUNT(DISTINCT operation_command_id) 
                                FROM vehicle_toll_subscriptions 
                                WHERE is_active = 1), 0) as active_subscribers
                    FROM (SELECT 1 as dummy) as d";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTotalTransactions.Text = reader["total_transactions"].ToString();
                        lblTotalAmount.Text = Convert.ToDecimal(reader["total_amount"]).ToString("N2");
                        lblSubscriptions.Text = reader["active_subscribers"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics with filter: {ex.Message}");
                // Set default values
                lblTotalTransactions.Text = "0";
                lblTotalAmount.Text = "0.00";
                lblSubscriptions.Text = "0";
            }
        }

        #endregion

        #region Gate Management Methods

        protected void btnSaveGate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateGateForm())
                    return;

                int gateId = Convert.ToInt32(hdnGateId.Value);
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    if (gateId > 0)
                    {
                        string query = @"
                            UPDATE toll_gates 
                            SET gate_name = @gateName,
                                location = @location,
                                toll_amount = @amount
                            WHERE gate_id = @gateId";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@gateName", txtGateName.Text.Trim());
                        cmd.Parameters.AddWithValue("@location", string.IsNullOrEmpty(txtGateLocation.Text.Trim()) ? (object)DBNull.Value : txtGateLocation.Text.Trim());
                        cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtGateAmount.Text));
                        cmd.Parameters.AddWithValue("@gateId", gateId);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        ShowSuccess("تم تحديث البوابة بنجاح");
                    }
                    else
                    {
                        string query = @"
                            INSERT INTO toll_gates 
                                (gate_name, location, toll_amount, is_active)
                            VALUES 
                                (@gateName, @location, @amount, 1)";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@gateName", txtGateName.Text.Trim());
                        cmd.Parameters.AddWithValue("@location", string.IsNullOrEmpty(txtGateLocation.Text.Trim()) ? (object)DBNull.Value : txtGateLocation.Text.Trim());
                        cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtGateAmount.Text));

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        ShowSuccess("تم إضافة البوابة بنجاح");
                    }
                }

                ClearGateForm();
                LoadGatesList();
                LoadAllGateDropdowns();
            }
            catch (Exception ex)
            {
                ShowMessage($"خطأ في حفظ البوابة: {ex.Message}");
            }
        }

        protected void btnClearGate_Click(object sender, EventArgs e)
        {
            ClearGateForm();
        }

        private void ClearGateForm()
        {
            txtGateName.Text = "";
            txtGateLocation.Text = "";
            txtGateAmount.Text = "";
            hdnGateId.Value = "0";
            btnSaveGate.Text = "حفظ البوابة";
        }

        private bool ValidateGateForm()
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(txtGateName.Text.Trim()))
                errorMessage += "• الرجاء إدخال اسم البوابة\n";

            if (string.IsNullOrEmpty(txtGateAmount.Text))
            {
                errorMessage += "• الرجاء إدخال قيمة العبور\n";
            }
            else if (!decimal.TryParse(txtGateAmount.Text, out decimal amount) || amount <= 0)
            {
                errorMessage += "• الرجاء إدخال قيمة عبور صحيحة\n";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowMessage(errorMessage);
                return false;
            }

            return true;
        }

        private void LoadGateForEditing(int gateId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT gate_name, location, toll_amount
                        FROM toll_gates 
                        WHERE gate_id = @gateId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@gateId", gateId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtGateName.Text = reader["gate_name"].ToString();
                        txtGateLocation.Text = reader["location"].ToString();
                        txtGateAmount.Text = reader["toll_amount"].ToString();
                        hdnGateId.Value = gateId.ToString();
                        btnSaveGate.Text = "تحديث البوابة";

                        ScriptManager.RegisterStartupScript(this, GetType(), "scrollToForm",
                            "document.getElementById('gateFormSection').scrollIntoView({behavior: 'smooth'});", true);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading gate for editing: {ex.Message}");
            }
        }

        private void ToggleGateStatus(int gateId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        UPDATE toll_gates 
                        SET is_active = CASE WHEN is_active = 1 THEN 0 ELSE 1 END
                        WHERE gate_id = @gateId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@gateId", gateId);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    string checkQuery = "SELECT is_active FROM toll_gates WHERE gate_id = @gateId";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@gateId", gateId);
                    object result = checkCmd.ExecuteScalar();
                    string status = result != null && Convert.ToBoolean(result) ? "تم تفعيل" : "تم تعطيل";

                    ShowSuccess($"{status} البوابة بنجاح");

                    LoadGatesList();
                    LoadAllGateDropdowns();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"خطأ: {ex.Message}");
            }
        }

        protected void gvGates_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditGate")
            {
                int gateId = Convert.ToInt32(e.CommandArgument);
                LoadGateForEditing(gateId);
            }
            else if (e.CommandName == "ToggleStatus")
            {
                int gateId = Convert.ToInt32(e.CommandArgument);
                ToggleGateStatus(gateId);
            }
        }

        protected void gvGates_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGates.PageIndex = e.NewPageIndex;
            LoadGatesList();
        }

        #endregion

        #region Subscription Methods

        private void LoadCarsForSubscription()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            car_number_id,
                            RTRIM(letter1) + ' ' + RTRIM(letter2) + 
                            CASE 
                                WHEN letter3 IS NOT NULL AND RTRIM(letter3) != '' 
                                THEN ' ' + RTRIM(letter3) 
                                ELSE '' 
                            END + ' ' + 
                            RTRIM(plate_numbers) as car_number_display
                        FROM car_numbers
                        WHERE is_active = 1
                        GROUP BY car_number_id, letter1, letter2, letter3, plate_numbers
                        ORDER BY car_number_display";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlSubCar.Items.Clear();
                    ddlSubCar.Items.Add(new ListItem("-- اختر رقم العربية --", "0"));

                    foreach (DataRow row in dt.Rows)
                    {
                        string carId = row["car_number_id"].ToString();
                        string carDisplay = row["car_number_display"].ToString();

                        bool exists = false;
                        foreach (ListItem item in ddlSubCar.Items)
                        {
                            if (item.Text == carDisplay)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            ddlSubCar.Items.Add(new ListItem(carDisplay, carId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cars for subscription: {ex.Message}");
                ddlSubCar.Items.Clear();
                ddlSubCar.Items.Add(new ListItem("-- خطأ في تحميل أرقام السيارات --", "0"));
            }
        }

        private void LoadMissionsForSubscription()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT DISTINCT
                            oc.mission_id,
                            cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers + N' - ' +
                            e.employee_name + N' - ' +
                            CONVERT(nvarchar, oc.date, 23) as mission_display
                        FROM operation_command oc
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        WHERE oc.v_number IS NOT NULL AND oc.driver_name IS NOT NULL
                        ORDER BY mission_display";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlSubMission.Items.Clear();
                    ddlSubMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));

                    if (dt.Rows.Count > 0)
                    {
                        ddlSubMission.DataSource = dt;
                        ddlSubMission.DataTextField = "mission_display";
                        ddlSubMission.DataValueField = "mission_id";
                        ddlSubMission.DataBind();
                    }

                    System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} missions for subscription");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading missions for subscription: {ex.Message}");
                ddlSubMission.Items.Clear();
                ddlSubMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
            }
        }

        protected void ddlSubCar_SelectedIndexChanged(object sender, EventArgs e)
        {
            int carId = Convert.ToInt32(ddlSubCar.SelectedValue);

            if (carId > 0)
            {
                LoadMissionsForCar(carId);
            }
            else
            {
                ddlSubMission.Items.Clear();
                ddlSubMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));
            }
        }

        private void LoadMissionsForCar(int carId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT DISTINCT
                            oc.mission_id,
                            cn.letter1 + cn.letter2 + ISNULL(cn.letter3, '') + cn.plate_numbers + N' - ' +
                            e.employee_name + N' - ' +
                            CONVERT(nvarchar, oc.date, 23) as mission_display
                        FROM operation_command oc
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        WHERE oc.v_number = @carId
                        ORDER BY mission_display";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carId", carId);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlSubMission.Items.Clear();
                    ddlSubMission.Items.Add(new ListItem("-- اختر المهمة --", "0"));

                    if (dt.Rows.Count > 0)
                    {
                        ddlSubMission.DataSource = dt;
                        ddlSubMission.DataTextField = "mission_display";
                        ddlSubMission.DataValueField = "mission_id";
                        ddlSubMission.DataBind();
                    }

                    System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} missions for car {carId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading missions for car: {ex.Message}");
            }
        }

        protected void btnSaveSubscription_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateSubscriptionForm())
                    return;

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        INSERT INTO vehicle_toll_subscriptions 
                            (operation_command_id, gate_id, subscription_start_date, subscription_end_date, 
                             subscription_amount, is_active, created_date, created_by)
                        VALUES 
                            (@missionId, @gateId, @startDate, @endDate, @amount, 1, GETDATE(), @createdBy)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@missionId", Convert.ToInt32(ddlSubMission.SelectedValue));
                    cmd.Parameters.AddWithValue("@gateId", Convert.ToInt32(ddlSubGate.SelectedValue));
                    cmd.Parameters.AddWithValue("@startDate", Convert.ToDateTime(txtSubStartDate.Text));
                    cmd.Parameters.AddWithValue("@endDate", Convert.ToDateTime(txtSubEndDate.Text));
                    cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtSubAmount.Text));
                    cmd.Parameters.AddWithValue("@createdBy", "System");

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    ShowSuccess("تم حفظ الاشتراك بنجاح");

                    ClearSubscriptionForm();
                    SearchTransactions();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"خطأ في حفظ الاشتراك: {ex.Message}");
            }
        }

        protected void btnClearSubscription_Click(object sender, EventArgs e)
        {
            ClearSubscriptionForm();
        }

        private void ClearSubscriptionForm()
        {
            ddlSubCar.SelectedIndex = 0;
            ddlSubGate.SelectedIndex = 0;
            ddlSubMission.SelectedIndex = 0;
            txtSubAmount.Text = "";
            txtSubStartDate.Text = "";
            txtSubEndDate.Text = "";
            txtSubNotes.Text = "";
        }

        private bool ValidateSubscriptionForm()
        {
            string errorMessage = "";

            if (ddlSubCar.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار رقم العربية\n";

            if (ddlSubGate.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار البوابة\n";

            if (ddlSubMission.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار المهمة\n";

            if (string.IsNullOrEmpty(txtSubAmount.Text))
            {
                errorMessage += "• الرجاء إدخال مبلغ الاشتراك\n";
            }
            else if (!decimal.TryParse(txtSubAmount.Text, out decimal amount) || amount <= 0)
            {
                errorMessage += "• الرجاء إدخال مبلغ صحيح\n";
            }

            if (string.IsNullOrEmpty(txtSubStartDate.Text))
                errorMessage += "• الرجاء إدخال تاريخ البداية\n";

            if (string.IsNullOrEmpty(txtSubEndDate.Text))
                errorMessage += "• الرجاء إدخال تاريخ النهاية\n";

            if (!string.IsNullOrEmpty(txtSubStartDate.Text) && !string.IsNullOrEmpty(txtSubEndDate.Text))
            {
                DateTime startDate = Convert.ToDateTime(txtSubStartDate.Text);
                DateTime endDate = Convert.ToDateTime(txtSubEndDate.Text);

                if (endDate <= startDate)
                {
                    errorMessage += "• تاريخ النهاية يجب أن يكون بعد تاريخ البداية\n";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowMessage(errorMessage);
                return false;
            }

            return true;
        }

        #endregion

        #region Event Handlers

        protected void ddlMission_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSubscription();
        }

        protected void ddlGate_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSubscription();
        }

        private void CheckSubscription()
        {
            try
            {
                int missionId = Convert.ToInt32(ddlMission.SelectedValue);
                int gateId = Convert.ToInt32(ddlGate.SelectedValue);

                subscriptionInfo.Visible = false;
                paymentInfo.Visible = false;

                if (missionId > 0 && gateId > 0)
                {
                    string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();

                        string getVehicleQuery = "SELECT v_number FROM operation_command WHERE mission_id = @missionId";
                        SqlCommand getVehicleCmd = new SqlCommand(getVehicleQuery, conn);
                        getVehicleCmd.Parameters.AddWithValue("@missionId", missionId);
                        object vehicleIdObj = getVehicleCmd.ExecuteScalar();

                        if (vehicleIdObj != null && vehicleIdObj != DBNull.Value)
                        {
                            int vehicleId = Convert.ToInt32(vehicleIdObj);

                            string query = @"
                                SELECT TOP 1 
                                    subscription_id,
                                    subscription_amount,
                                    subscription_start_date,
                                    subscription_end_date
                                FROM vehicle_toll_subscriptions vts
                                WHERE vts.operation_command_id IN 
                                    (SELECT mission_id FROM operation_command WHERE v_number = @vehicleId)
                                  AND vts.gate_id = @gateId
                                  AND vts.is_active = 1
                                  AND GETDATE() BETWEEN vts.subscription_start_date AND vts.subscription_end_date
                                ORDER BY vts.subscription_end_date DESC";

                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                            cmd.Parameters.AddWithValue("@gateId", gateId);

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                subscriptionInfo.Visible = true;
                                paymentInfo.Visible = false;

                                lblSubAmount.Text = Convert.ToDecimal(reader["subscription_amount"]).ToString("N2") + " ج.م";
                                lblSubStart.Text = Convert.ToDateTime(reader["subscription_start_date"]).ToString("yyyy-MM-dd");
                                lblSubEnd.Text = Convert.ToDateTime(reader["subscription_end_date"]).ToString("yyyy-MM-dd");
                                lblSubMessage.Text = "✓ المركبة لديها اشتراك ساري لهذه البوابة";
                            }
                            else
                            {
                                reader.Close();
                                subscriptionInfo.Visible = false;
                                paymentInfo.Visible = true;

                                string gateQuery = "SELECT ISNULL(toll_amount, 0) FROM toll_gates WHERE gate_id = @gateId";
                                SqlCommand gateCmd = new SqlCommand(gateQuery, conn);
                                gateCmd.Parameters.AddWithValue("@gateId", gateId);

                                object result = gateCmd.ExecuteScalar();
                                txtAmountPaid.Text = result != null ? Convert.ToDecimal(result).ToString("N2") : "0";
                            }
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking subscription: {ex.Message}");
            }
        }

        #endregion

        #region Save Methods

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                int missionId = Convert.ToInt32(ddlMission.SelectedValue);
                int gateId = Convert.ToInt32(ddlGate.SelectedValue);

                if (subscriptionInfo.Visible)
                {
                    SaveSubscriptionUsage(missionId, gateId);
                }
                else
                {
                    SaveOneTimePayment(missionId, gateId);
                }

                ClearForm();
                SearchTransactions();
                string script = $"activateTab('{hfActiveTab.Value}');";
                ScriptManager.RegisterStartupScript(this, GetType(), "StayOnTab", script, true);
            }
            catch (Exception ex)
            {
                ShowMessage($"خطأ في الحفظ: {ex.Message}");
            }
        }

        private void SaveSubscriptionUsage(int missionId, int gateId)
        {
            ShowSuccess("تم تسجيل استخدام الاشتراك بنجاح");
        }

        private void SaveOneTimePayment(int missionId, int gateId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    INSERT INTO toll_payments 
                        (operation_command_id, gate_id, payment_date, amount_paid, payment_type, notes, created_date)
                    VALUES 
                        (@missionId, @gateId, GETDATE(), @amount, N'نقدي', @notes, GETDATE())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@missionId", missionId);
                cmd.Parameters.AddWithValue("@gateId", gateId);
                cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtAmountPaid.Text));
                cmd.Parameters.AddWithValue("@notes", txtNotes.Text.Trim());

                conn.Open();
                cmd.ExecuteNonQuery();

                ShowSuccess("تم تسجيل الدفع النقدي بنجاح");
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ddlMission.SelectedIndex = 0;
            ddlGate.SelectedIndex = 0;
            txtAmountPaid.Text = "";
            txtNotes.Text = "";
            subscriptionInfo.Visible = false;
            paymentInfo.Visible = false;
            hdnTransactionId.Value = "0";
            hdnTransactionType.Value = "";
        }

        private bool ValidateForm()
        {
            string errorMessage = "";

            if (ddlMission.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار المهمة\n";

            if (ddlGate.SelectedValue == "0")
                errorMessage += "• الرجاء اختيار البوابة\n";

            if (paymentInfo.Visible && string.IsNullOrEmpty(txtAmountPaid.Text))
                errorMessage += "• الرجاء إدخال المبلغ المدفوع\n";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowMessage(errorMessage);
                return false;
            }

            return true;
        }

        #endregion

        #region Search Methods

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTransactions();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchCar.Text = "";
            txtSearchDriver.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            txtSearchSubDate.Text = "";

            if (ddlSearchGate.Items.Count > 0)
                ddlSearchGate.SelectedIndex = 0;

            if (ddlSearchType.Items.Count > 0)
                ddlSearchType.SelectedIndex = 0;

            // Hide statistics and results
            statsContainer.Visible = false;
            searchResults.Visible = false;
            
            // Clear the grid
            gvTransactions.DataSource = null;
            gvTransactions.DataBind();
            
            // Clear any messages
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
        }

        private void SearchTransactions()
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string query = @"
WITH AllTransactions AS (
    SELECT 
        ROW_NUMBER() OVER (ORDER BY vts.created_date DESC) AS SubRowNum,
        'sub' + CAST(vts.subscription_id AS NVARCHAR) as transaction_id,
        ISNULL(cn.letter1, '') + ISNULL(cn.letter2, '') + ISNULL(cn.letter3, '') + ISNULL(cn.plate_numbers, '') as car_number,
        ISNULL(e.employee_name, N'غير محدد') as driver_name,
        ISNULL(tg.gate_name, N'بوابة غير معروفة') as gate_name,
        vts.gate_id,
        vts.subscription_amount as amount,
        vts.created_date as transaction_date,
        'subscription' as transaction_type,
        CONVERT(nvarchar, vts.subscription_start_date, 23) + N' إلى ' + 
        CONVERT(nvarchar, vts.subscription_end_date, 23) as subscription_period,
        vts.subscription_start_date as sub_start,
        vts.subscription_end_date as sub_end
    FROM vehicle_toll_subscriptions vts
    LEFT JOIN operation_command oc ON vts.operation_command_id = oc.mission_id
    LEFT JOIN car_numbers cn ON oc.v_number = cn.car_number_id
    LEFT JOIN employees e ON oc.driver_name = e.employee_id
    LEFT JOIN toll_gates tg ON vts.gate_id = tg.gate_id

    UNION ALL

    SELECT 
        ROW_NUMBER() OVER (ORDER BY tp.payment_date DESC) AS PayRowNum,
        'pay' + CAST(tp.payment_id AS NVARCHAR) as transaction_id,
        ISNULL(cn.letter1, '') + ISNULL(cn.letter2, '') + ISNULL(cn.letter3, '') + ISNULL(cn.plate_numbers, '') as car_number,
        ISNULL(e.employee_name, N'غير محدد') as driver_name,
        ISNULL(tg.gate_name, N'بوابة غير معروفة') as gate_name,
        tp.gate_id,
        tp.amount_paid as amount,
        tp.payment_date as transaction_date,
        'payment' as transaction_type,
        N'' as subscription_period,
        NULL as sub_start,
        NULL as sub_end
    FROM toll_payments tp
    LEFT JOIN operation_command oc ON tp.operation_command_id = oc.mission_id
    LEFT JOIN car_numbers cn ON oc.v_number = cn.car_number_id
    LEFT JOIN employees e ON oc.driver_name = e.employee_id
    LEFT JOIN toll_gates tg ON tp.gate_id = tg.gate_id
)
SELECT 
    ROW_NUMBER() OVER (ORDER BY transaction_date DESC) AS RowNum,
    transaction_id,
    car_number,
    driver_name,
    gate_name,
    gate_id,
    amount,
    transaction_date,
    transaction_type,
    subscription_period,
    sub_start,
    sub_end
FROM AllTransactions
WHERE 1=1";

                    List<string> conditions = new List<string>();
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(txtSearchCar.Text.Trim()))
                    {
                        conditions.Add("car_number LIKE @carNumber");
                        parameters.Add(new SqlParameter("@carNumber", "%" + txtSearchCar.Text.Trim() + "%"));
                    }

                    if (!string.IsNullOrEmpty(txtSearchDriver.Text.Trim()))
                    {
                        conditions.Add("driver_name LIKE @driverName");
                        parameters.Add(new SqlParameter("@driverName", "%" + txtSearchDriver.Text.Trim() + "%"));
                    }

                    if (ddlSearchGate.SelectedValue != "0" && !string.IsNullOrEmpty(ddlSearchGate.SelectedValue))
                    {
                        conditions.Add("gate_id = @gateId");
                        parameters.Add(new SqlParameter("@gateId", ddlSearchGate.SelectedValue));
                    }

                    string selectedType = ddlSearchType.SelectedValue;
                    if (!string.IsNullOrEmpty(selectedType) && selectedType != "0")
                    {
                        conditions.Add("transaction_type = @transType");
                        parameters.Add(new SqlParameter("@transType", selectedType));
                    }

                    if (!string.IsNullOrEmpty(txtFromDate.Text))
                    {
                        DateTime fromDate = DateTime.Parse(txtFromDate.Text).Date;
                        conditions.Add("CAST(transaction_date AS DATE) >= @fromDate");
                        parameters.Add(new SqlParameter("@fromDate", fromDate));
                    }

                    if (!string.IsNullOrEmpty(txtToDate.Text))
                    {
                        DateTime toDate = DateTime.Parse(txtToDate.Text).Date;
                        conditions.Add("CAST(transaction_date AS DATE) <= @toDate");
                        parameters.Add(new SqlParameter("@toDate", toDate));
                    }

                    if (!string.IsNullOrEmpty(txtSearchSubDate.Text))
                    {
                        DateTime subDate = DateTime.Parse(txtSearchSubDate.Text).Date;
                        conditions.Add("transaction_type = 'subscription' AND sub_start <= @subDate AND sub_end >= @subDate");
                        parameters.Add(new SqlParameter("@subDate", subDate));
                    }

                    if (conditions.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", conditions);
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvTransactions.DataSource = dt;
                    gvTransactions.DataBind();
                    
                    // Show results section
                    searchResults.Visible = true;
                    
                    // Show statistics only if there are results
                    if (dt.Rows.Count > 0)
                    {
                        // Load statistics with the filter context
                        LoadStatisticsWithFilter(parameters);
                        statsContainer.Visible = true;
                    }
                    else
                    {
                        statsContainer.Visible = false;
                        string msg = "لا توجد نتائج للبحث";
                        ScriptManager.RegisterStartupScript(this, GetType(), "noResults",
                            $"alert('{msg}');", true);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                    statsContainer.Visible = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "searchError",
                        $"alert('خطأ في البحث: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        protected void gvTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTransactions.PageIndex = e.NewPageIndex;
            SearchTransactions();
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message)
        {
            lblMessage.Text = message.Replace("\n", "<br/>");
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }

        #endregion
    }
}