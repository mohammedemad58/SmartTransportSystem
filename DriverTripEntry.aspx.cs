using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class DriverTripEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any previous messages
                lblMessage.Visible = false;
                lblSuccess.Visible = false;
                lblWarning.Visible = false;

                // DO NOT auto-load work orders - hide the grid initially
                workOrdersPanel.Visible = false;
            }
        }

        private void LoadTodayWorkOrders(string driverName)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Get today's date (without time)
                    DateTime today = DateTime.Today;

                    string query = @"
                        SELECT 
                            oc.mission_id,
                            e.employee_name as driver_name,
                            cn.letter1 + ' ' + cn.letter2 + ' ' + 
                            ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers as car_number,
                            CASE 
                                WHEN oc.mission_type = 1 THEN 'مأمورية'
                                WHEN oc.mission_type = 2 THEN 'خط'
                                ELSE 'غير معروف'
                            END as mission_type
                        FROM operation_command oc
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        WHERE CAST(oc.date AS DATE) = @today
                          AND oc.deleted = 0
                          AND (oc.kilometer_end IS NULL OR oc.kilometer_end = 0)
                          AND e.employee_name LIKE @driverName
                        ORDER BY oc.mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@today", today);
                    cmd.Parameters.AddWithValue("@driverName", "%" + driverName + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvWorkOrders.DataSource = dt;
                    gvWorkOrders.DataBind();

                    // Show the panel only when there are results
                    workOrdersPanel.Visible = dt.Rows.Count > 0;

                    if (dt.Rows.Count == 0)
                    {
                        ShowMessage($"لا توجد مأموريات جديدة للسائق '{driverName}' تحتاج إدخال بيانات اليوم");
                    }
                    else
                    {
                        ShowMessage($"عُثر على {dt.Rows.Count} مأمورية للسائق '{driverName}'");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading today's work orders: {ex.Message}");
                ShowMessage("خطأ في تحميل البيانات: " + ex.Message);
            }
        }

        private void SearchWorkOrders()
        {
            try
            {
                string driverName = txtDriverNameSearch.Text.Trim();
                string missionNumber = txtMissionNumberSearch.Text.Trim();

                // Validate: Driver name is required
                if (string.IsNullOrEmpty(driverName))
                {
                    ShowMessage("الرجاء إدخال اسم السائق للبحث");
                    return;
                }

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            oc.mission_id,
                            e.employee_name as driver_name,
                            cn.letter1 + ' ' + cn.letter2 + ' ' + 
                            ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers as car_number,
                            CASE 
                                WHEN oc.mission_type = 1 THEN 'مأمورية'
                                WHEN oc.mission_type = 2 THEN 'خط'
                                ELSE 'غير معروف'
                            END as mission_type
                        FROM operation_command oc
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        WHERE oc.deleted = 0
                          AND (oc.kilometer_end IS NULL OR oc.kilometer_end = 0)
                          AND e.employee_name LIKE @driverName";

                    // Add mission number condition if provided
                    if (!string.IsNullOrEmpty(missionNumber))
                    {
                        query += " AND oc.mission_id = @missionNumber";
                    }
                    else
                    {
                        // If no mission number, only show today's work orders
                        DateTime today = DateTime.Today;
                        query += " AND CAST(oc.date AS DATE) = @today";
                    }

                    query += " ORDER BY oc.mission_id DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@driverName", "%" + driverName + "%");

                    if (!string.IsNullOrEmpty(missionNumber))
                    {
                        cmd.Parameters.AddWithValue("@missionNumber", Convert.ToInt32(missionNumber));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@today", DateTime.Today);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvWorkOrders.DataSource = dt;
                    gvWorkOrders.DataBind();

                    workOrdersPanel.Visible = dt.Rows.Count > 0;

                    if (dt.Rows.Count == 0)
                    {
                        if (!string.IsNullOrEmpty(missionNumber))
                        {
                            ShowMessage($"لا توجد مأمورية برقم {missionNumber} للسائق '{driverName}' تحتاج إدخال بيانات");
                        }
                        else
                        {
                            ShowMessage($"لا توجد مأموريات للسائق '{driverName}' تحتاج إدخال بيانات اليوم");
                        }
                    }
                    else
                    {
                        ShowMessage($"عُثر على {dt.Rows.Count} مأمورية للسائق '{driverName}'");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching work orders: {ex.Message}");
                ShowMessage("خطأ في البحث: " + ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Validate the page (checks RequiredFieldValidator)
            if (!Page.IsValid)
            {
                return;
            }

            SearchWorkOrders();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtDriverNameSearch.Text = "";
            txtMissionNumberSearch.Text = "";
            workOrdersPanel.Visible = false;
            missionInfo.Visible = false;
            ClearForm();
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
            lblWarning.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Clear the form and hide mission info
            missionInfo.Visible = false;
            ClearForm();
        }

        protected void gvWorkOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectMission")
            {
                int missionId = Convert.ToInt32(e.CommandArgument);
                LoadMissionData(missionId);
            }
        }

        private void LoadMissionData(int missionId)
        {
            string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
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
                            cn.letter1 + ' ' + cn.letter2 + ' ' + 
                            ISNULL(cn.letter3 + ' ', '') + cn.plate_numbers as car_number,
                            e.employee_name as driver_name,
                            r.route_name
                        FROM operation_command oc
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        INNER JOIN employees e ON oc.driver_name = e.employee_id
                        LEFT JOIN routes r ON oc.location_point = r.route_id AND oc.mission_type = 2
                        WHERE oc.mission_id = @missionId
                          AND oc.deleted = 0";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@missionId", missionId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Check if work order is from today
                        DateTime workOrderDate = Convert.ToDateTime(reader["work_order_date"]);
                        bool isToday = workOrderDate.Date == DateTime.Today;

                        // Check if trip has been completed (end KM has value)
                        bool hasData = reader["kilometer_end"] != DBNull.Value && Convert.ToDecimal(reader["kilometer_end"]) > 0;

                        // Debug output
                        System.Diagnostics.Debug.WriteLine($"=== Mission {missionId} ===");
                        System.Diagnostics.Debug.WriteLine($"Work Order Date: {workOrderDate:yyyy-MM-dd}");
                        System.Diagnostics.Debug.WriteLine($"Today's Date: {DateTime.Today:yyyy-MM-dd}");
                        System.Diagnostics.Debug.WriteLine($"Is Today: {isToday}");
                        System.Diagnostics.Debug.WriteLine($"Has End KM Data: {hasData}");
                        System.Diagnostics.Debug.WriteLine($"Start KM: {reader["kilometer_start"]}");
                        System.Diagnostics.Debug.WriteLine($"End KM: {reader["kilometer_end"]}");

                        // Display mission info
                        txtCarNumber.Text = reader["car_number"].ToString();
                        lblDriverName.Text = reader["driver_name"].ToString();

                        string missionType = reader["mission_type"].ToString();
                        string missionTypeName = missionType == "1" ? "مأمورية" : "خط";
                        lblMissionType.Text = missionTypeName;

                        string missionDetails = "";
                        if (missionType == "1")
                        {
                            missionDetails = reader["mission_description"]?.ToString() ?? "";
                        }
                        else
                        {
                            missionDetails = reader["route_name"]?.ToString() ?? "";
                        }
                        lblMissionDetails.Text = missionDetails;

                        // Show work order date
                        lblWorkOrderDate.Text = workOrderDate.ToString("yyyy-MM-dd");

                        // Load start KM (read-only)
                        if (reader["kilometer_start"] != DBNull.Value && Convert.ToDecimal(reader["kilometer_start"]) > 0)
                        {
                            txtStartKM.Text = reader["kilometer_start"].ToString();
                        }
                        else
                        {
                            txtStartKM.Text = "";
                        }

                        // Load end KM if exists
                        if (reader["kilometer_end"] != DBNull.Value && Convert.ToDecimal(reader["kilometer_end"]) > 0)
                        {
                            txtEndKM.Text = reader["kilometer_end"].ToString();
                        }
                        else
                        {
                            txtEndKM.Text = "";
                        }

                        // Load start time if exists
                        if (reader["mission_starting_time"] != DBNull.Value)
                        {
                            txtStartTime.Text = Convert.ToDateTime(reader["mission_starting_time"]).ToString("yyyy-MM-ddTHH:mm");
                        }
                        else
                        {
                            txtStartTime.Text = "";
                        }

                        // Load return time if exists
                        if (reader["mission_ending_time"] != DBNull.Value)
                        {
                            txtReturnTime.Text = Convert.ToDateTime(reader["mission_ending_time"]).ToString("yyyy-MM-ddTHH:mm");
                        }
                        else
                        {
                            txtReturnTime.Text = "";
                        }

                        // Make Start KM read-only (display only)
                        txtStartKM.ReadOnly = true;
                        txtStartKM.BackColor = Color.FromName("#F8F9FA");

                        // Enable editing if work order is from today AND end KM is not filled yet
                        bool canEdit = isToday && !hasData;

                        txtStartTime.Enabled = canEdit;
                        txtReturnTime.Enabled = canEdit;
                        txtEndKM.Enabled = canEdit;
                        btnSave.Enabled = canEdit;

                        if (!canEdit)
                        {
                            if (!isToday)
                            {
                                ShowWarning($"لا يمكن تعديل هذه المأمورية لأن تاريخها ({workOrderDate:yyyy-MM-dd}) ليس اليوم");
                            }
                            else if (hasData)
                            {
                                ShowWarning("تم إدخال بيانات العودة لهذه المأمورية مسبقاً");
                            }
                        }
                        else
                        {
                            // Clear any warning and show that editing is allowed
                            lblWarning.Visible = false;
                            ShowMessage("يمكنك إدخال بيانات الرحلة (وقت العودة وعداد العودة)");
                        }

                        // Store mission ID in ViewState
                        ViewState["CurrentMissionId"] = missionId;

                        missionInfo.Visible = true;
                        workOrdersPanel.Visible = false;
                    }
                    else
                    {
                        ShowMessage("لم يتم العثور على المأمورية رقم " + missionId);
                        missionInfo.Visible = false;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine($"Error in LoadMissionData: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["CurrentMissionId"] == null)
                {
                    ShowMessage("الرجاء اختيار المأمورية أولاً");
                    return;
                }

                int missionId = Convert.ToInt32(ViewState["CurrentMissionId"]);

                // Verify again that editing is allowed
                if (!CanEditMission(missionId))
                {
                    ShowMessage("لا يمكن تعديل هذه المأمورية. تأكد من أنها من اليوم ولم يتم إدخال بيانات العودة مسبقاً");
                    return;
                }

                // Validate inputs
                if (!ValidateInputs())
                    return;

                DateTime startTime = DateTime.Parse(txtStartTime.Text);
                DateTime returnTime = DateTime.Parse(txtReturnTime.Text);
                decimal endKM = decimal.Parse(txtEndKM.Text);

                // Get the existing start KM (read-only, so we keep the existing value)
                decimal startKM = 0;
                if (!string.IsNullOrEmpty(txtStartKM.Text))
                {
                    decimal.TryParse(txtStartKM.Text, out startKM);
                }

                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE operation_command 
                        SET mission_starting_time = @startTime,
                            mission_ending_time = @endTime,
                            kilometer_end = @endKM
                        WHERE mission_id = @missionId";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@missionId", missionId);
                    cmd.Parameters.AddWithValue("@startTime", startTime);
                    cmd.Parameters.AddWithValue("@endTime", returnTime);
                    cmd.Parameters.AddWithValue("@endKM", endKM);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowSuccessMessage("تم حفظ بيانات الرحلة بنجاح");

                        // Clear the form
                        missionInfo.Visible = false;
                        ClearForm();

                        // Refresh the search results
                        SearchWorkOrders();
                    }
                    else
                    {
                        ShowMessage("فشل في حفظ البيانات");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في حفظ البيانات: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error in btnSave_Click: {ex.Message}");
            }
        }

        private bool CanEditMission(int missionId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            CAST(date AS DATE) as work_order_date,
                            CASE WHEN kilometer_end IS NOT NULL AND kilometer_end > 0 THEN 1 ELSE 0 END as has_data
                        FROM operation_command 
                        WHERE mission_id = @missionId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@missionId", missionId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        DateTime workOrderDate = Convert.ToDateTime(reader["work_order_date"]);
                        bool isToday = workOrderDate.Date == DateTime.Today;
                        bool hasData = Convert.ToInt32(reader["has_data"]) == 1;

                        return isToday && !hasData;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking edit permission: {ex.Message}");
                return false;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtStartTime.Text))
            {
                ShowMessage("الرجاء إدخال بداية المأمورية");
                return false;
            }

            if (string.IsNullOrEmpty(txtReturnTime.Text))
            {
                ShowMessage("الرجاء إدخال وقت العودة");
                return false;
            }

            if (string.IsNullOrEmpty(txtEndKM.Text))
            {
                ShowMessage("الرجاء إدخال عداد العودة");
                return false;
            }

            DateTime startTime = DateTime.Parse(txtStartTime.Text);
            DateTime returnTime = DateTime.Parse(txtReturnTime.Text);

            if (returnTime <= startTime)
            {
                ShowMessage("وقت العودة يجب أن يكون بعد وقت البداية");
                return false;
            }

            decimal startKM = 0;
            decimal endKM = 0;

            if (!string.IsNullOrEmpty(txtStartKM.Text))
            {
                decimal.TryParse(txtStartKM.Text, out startKM);
            }
            decimal.TryParse(txtEndKM.Text, out endKM);

            if (endKM <= startKM)
            {
                ShowMessage("عداد العودة يجب أن يكون أكبر من عداد البداية");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtCarNumber.Text = "";
            lblDriverName.Text = "";
            lblMissionType.Text = "";
            lblMissionDetails.Text = "";
            lblWorkOrderDate.Text = "";
            txtStartTime.Text = "";
            txtReturnTime.Text = "";
            txtStartKM.Text = "";
            txtEndKM.Text = "";
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
            lblWarning.Visible = false;
        }

        private void ShowSuccessMessage(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
            lblWarning.Visible = false;
        }

        private void ShowWarning(string message)
        {
            lblWarning.Text = message;
            lblWarning.Visible = true;
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
        }
    }
}