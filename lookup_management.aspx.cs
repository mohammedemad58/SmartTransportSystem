using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class lookup_management : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAllData();

                // Check for tab parameter in URL
                string tab = Request.QueryString["tab"];
                if (!string.IsNullOrEmpty(tab))
                {
                    hdnCurrentTab.Value = tab;
                }
                else
                {
                    hdnCurrentTab.Value = "carTypes";
                }
            }
        }

        private void LoadAllData()
        {
            LoadCarTypes();
            LoadModels();
            LoadStatuses();
            LoadTrafficUnits();
            LoadFuelTypes();
            LoadLicenseTypes();
            LoadInsuranceCompanies();
            LoadEmployeePositions();
            LoadViolationTypes();
            LoadCardsRepeater();
            LoadFuelLiquidPrices();
            LoadGasPrices();
        }

        #region Violation Types

        private void LoadViolationTypes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY violation_type_name) AS RowNum,
                            violation_type_id as id, 
                            violation_type_name as type_name, 
                            ISNULL(is_active, 1) as is_active 
                        FROM violation_types";

                    string search = txtSearchViolationType.Text.Trim();
                    if (!string.IsNullOrEmpty(search))
                        query = query.Replace("FROM violation_types", "FROM violation_types WHERE violation_type_name LIKE @Search");
                    query += " ORDER BY violation_type_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(search))
                        da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvViolationTypes.DataSource = dt;
                    gvViolationTypes.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message, false);
                }
            }
        }

        protected void BtnAddViolationType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtViolationTypeName.Text))
            {
                ShowMessage("الرجاء إدخال نوع المخالفة", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM violation_types WHERE violation_type_name = @Name";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", txtViolationTypeName.Text.Trim());
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("نوع المخالفة موجود بالفعل", false);
                        return;
                    }

                    string query = @"
                        INSERT INTO violation_types (violation_type_name, is_active, created_date) 
                        VALUES (@Name, 1, GETDATE())";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", txtViolationTypeName.Text.Trim());
                    cmd.ExecuteNonQuery();

                    txtViolationTypeName.Text = "";
                    LoadViolationTypes();
                    ShowMessage("تم إضافة نوع المخالفة بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void TxtSearchViolationType_TextChanged(object sender, EventArgs e) => LoadViolationTypes();

        protected void GvViolationTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE violation_types SET is_active = ISNULL(is_active, 1) ^ 1 WHERE violation_type_id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                        LoadViolationTypes();
                        ShowMessage("تم تغيير حالة النوع بنجاح", true);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("خطأ: " + ex.Message, false);
                    }
                }
            }
            else if (e.CommandName == "DeleteViolationType")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();

                        string getNameQuery = "SELECT violation_type_name FROM violation_types WHERE violation_type_id = @Id";
                        SqlCommand getNameCmd = new SqlCommand(getNameQuery, conn);
                        getNameCmd.Parameters.AddWithValue("@Id", id);
                        string violationName = getNameCmd.ExecuteScalar()?.ToString();

                        string checkQuery = "SELECT COUNT(*) FROM traffic_violations WHERE violation_type = @Name";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Name", violationName);
                        int inUse = (int)checkCmd.ExecuteScalar();

                        if (inUse > 0)
                        {
                            ShowMessage("لا يمكن حذف هذا النوع لأنه مستخدم في مخالفات مسجلة", false);
                            return;
                        }

                        string query = "DELETE FROM violation_types WHERE violation_type_id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();

                        LoadViolationTypes();
                        ShowMessage("تم حذف النوع بنجاح", true);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("خطأ: " + ex.Message, false);
                    }
                }
            }
        }

        protected void GvViolationTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvViolationTypes.EditIndex = e.NewEditIndex;
            LoadViolationTypes();
        }

        protected void GvViolationTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvViolationTypes.EditIndex = -1;
            LoadViolationTypes();
        }

        protected void GvViolationTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvViolationTypes.DataKeys[e.RowIndex].Value);
            string typeName = ((TextBox)gvViolationTypes.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM violation_types WHERE violation_type_name = @Name AND violation_type_id != @Id";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", typeName);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("نوع المخالفة موجود بالفعل", false);
                        gvViolationTypes.EditIndex = -1;
                        LoadViolationTypes();
                        return;
                    }

                    string query = "UPDATE violation_types SET violation_type_name = @Name WHERE violation_type_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", typeName);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    gvViolationTypes.EditIndex = -1;
                    LoadViolationTypes();
                    ShowMessage("تم تحديث النوع بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void GvViolationTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvViolationTypes.PageIndex = e.NewPageIndex;
            LoadViolationTypes();
        }

        #endregion

        #region Car Types

        private void LoadCarTypes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY car_brand, car_type_name) AS RowNum,
                        car_type_id, 
                        car_brand, 
                        car_type_name, 
                        CONCAT(car_brand, ' - ', car_type_name) as full_name,
                        ISNULL(is_active, 1) as is_active 
                    FROM car_types";
                
                string search = txtSearchCarType.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                    query = query.Replace("FROM car_types", "FROM car_types WHERE car_brand LIKE @Search OR car_type_name LIKE @Search");
                query += " ORDER BY car_brand, car_type_name";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvCarTypes.DataSource = dt;
                gvCarTypes.DataBind();
            }
        }

        protected void BtnAddCarType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCarBrand.Text) || string.IsNullOrEmpty(txtCarTypeName.Text))
            {
                ShowMessage("الرجاء إدخال جميع البيانات", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO car_types (car_brand, car_type_name, is_active) VALUES (@Brand, @Name, 1)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Brand", txtCarBrand.Text.Trim());
                cmd.Parameters.AddWithValue("@Name", txtCarTypeName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtCarBrand.Text = "";
                txtCarTypeName.Text = "";
                LoadCarTypes();
                ShowMessage("تم الإضافة بنجاح", true);
            }
        }

        protected void TxtSearchCarType_TextChanged(object sender, EventArgs e) => LoadCarTypes();

        protected void GvCarTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE car_types SET is_active = ISNULL(is_active, 1) ^ 1 WHERE car_type_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadCarTypes();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvCarTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCarTypes.EditIndex = e.NewEditIndex;
            LoadCarTypes();
        }

        protected void GvCarTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCarTypes.EditIndex = -1;
            LoadCarTypes();
        }

        protected void GvCarTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvCarTypes.DataKeys[e.RowIndex].Value);
            string brand = ((TextBox)gvCarTypes.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
            string name = ((TextBox)gvCarTypes.Rows[e.RowIndex].Cells[2].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE car_types SET car_brand = @Brand, car_type_name = @Name WHERE car_type_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Brand", brand);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvCarTypes.EditIndex = -1;
            LoadCarTypes();
            ShowMessage("تم التحديث بنجاح", true);
        }

        protected void GvCarTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCarTypes.PageIndex = e.NewPageIndex;
            LoadCarTypes();
        }

        #endregion

        #region Models

        private void LoadModels()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY model) AS RowNum,
                        m_id, 
                        model, 
                        ISNULL(is_active, 1) as is_active 
                    FROM car_model";
                
                string search = txtSearchModel.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                    query = query.Replace("FROM car_model", "FROM car_model WHERE model LIKE @Search");
                query += " ORDER BY model";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvModels.DataSource = dt;
                gvModels.DataBind();
            }
        }

        protected void BtnAddModel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtModelName.Text))
            {
                ShowMessage("الرجاء إدخال الموديل", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO car_model (model, is_active) VALUES (@Model, 1)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Model", txtModelName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtModelName.Text = "";
                LoadModels();
                ShowMessage("تم الإضافة بنجاح", true);
            }
        }

        protected void TxtSearchModel_TextChanged(object sender, EventArgs e) => LoadModels();

        protected void GvModels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE car_model SET is_active = ISNULL(is_active, 1) ^ 1 WHERE m_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadModels();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvModels_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvModels.EditIndex = e.NewEditIndex;
            LoadModels();
        }

        protected void GvModels_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvModels.EditIndex = -1;
            LoadModels();
        }

        protected void GvModels_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvModels.DataKeys[e.RowIndex].Value);
            string model = ((TextBox)gvModels.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE car_model SET model = @Model WHERE m_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Model", model);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvModels.EditIndex = -1;
            LoadModels();
            ShowMessage("تم التحديث بنجاح", true);
        }

        protected void GvModels_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvModels.PageIndex = e.NewPageIndex;
            LoadModels();
        }

        #endregion

        #region Status

        private void LoadStatuses()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY status) AS RowNum,
                        s_id, 
                        status, 
                        ISNULL(is_active, 1) as is_active 
                    FROM car_status";
                
                string search = txtSearchStatus.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                    query = query.Replace("FROM car_status", "FROM car_status WHERE status LIKE @Search");
                query += " ORDER BY status";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvStatus.DataSource = dt;
                gvStatus.DataBind();
            }
        }

        protected void BtnAddStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStatusName.Text))
            {
                ShowMessage("الرجاء إدخال حالة السيارة", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO car_status (status, is_active) VALUES (@Status, 1)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", txtStatusName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtStatusName.Text = "";
                LoadStatuses();
                ShowMessage("تم الإضافة بنجاح", true);
            }
        }

        protected void TxtSearchStatus_TextChanged(object sender, EventArgs e) => LoadStatuses();

        protected void GvStatus_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE car_status SET is_active = ISNULL(is_active, 1) ^ 1 WHERE s_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadStatuses();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvStatus_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvStatus.EditIndex = e.NewEditIndex;
            LoadStatuses();
        }

        protected void GvStatus_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvStatus.EditIndex = -1;
            LoadStatuses();
        }

        protected void GvStatus_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvStatus.DataKeys[e.RowIndex].Value);
            string status = ((TextBox)gvStatus.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE car_status SET status = @Status WHERE s_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvStatus.EditIndex = -1;
            LoadStatuses();
            ShowMessage("تم التحديث بنجاح", true);
        }

        protected void GvStatus_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStatus.PageIndex = e.NewPageIndex;
            LoadStatuses();
        }

        #endregion

        #region Traffic Units

        private void LoadTrafficUnits()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY name) AS RowNum,
                        id_TF, 
                        name, 
                        ISNULL(is_active, 1) as is_active 
                    FROM traffic_unit";
                
                string search = txtSearchTrafficUnit.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                    query = query.Replace("FROM traffic_unit", "FROM traffic_unit WHERE name LIKE @Search");
                query += " ORDER BY name";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvTrafficUnits.DataSource = dt;
                gvTrafficUnits.DataBind();
            }
        }

        protected void BtnAddTrafficUnit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTrafficUnitName.Text))
            {
                ShowMessage("الرجاء إدخال اسم وحدة المرور", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO traffic_unit (name, is_active) VALUES (@Name, 1)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", txtTrafficUnitName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtTrafficUnitName.Text = "";
                LoadTrafficUnits();
                ShowMessage("تم الإضافة بنجاح", true);
            }
        }

        protected void TxtSearchTrafficUnit_TextChanged(object sender, EventArgs e) => LoadTrafficUnits();

        protected void GvTrafficUnits_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE traffic_unit SET is_active = ISNULL(is_active, 1) ^ 1 WHERE id_TF = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadTrafficUnits();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvTrafficUnits_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvTrafficUnits.EditIndex = e.NewEditIndex;
            LoadTrafficUnits();
        }

        protected void GvTrafficUnits_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvTrafficUnits.EditIndex = -1;
            LoadTrafficUnits();
        }

        protected void GvTrafficUnits_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvTrafficUnits.DataKeys[e.RowIndex].Value);
            string name = ((TextBox)gvTrafficUnits.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE traffic_unit SET name = @Name WHERE id_TF = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvTrafficUnits.EditIndex = -1;
            LoadTrafficUnits();
            ShowMessage("تم التحديث بنجاح", true);
        }

        protected void GvTrafficUnits_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrafficUnits.PageIndex = e.NewPageIndex;
            LoadTrafficUnits();
        }

        #endregion

        #region Fuel Types

        private void LoadFuelTypes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY type) AS RowNum,
                        fuel_id, 
                        type, 
                        ISNULL(is_active, 1) as is_active 
                    FROM fuel_type";
                
                string search = txtSearchFuelType.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                    query = query.Replace("FROM fuel_type", "FROM fuel_type WHERE type LIKE @Search");
                query += " ORDER BY type";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvFuelTypes.DataSource = dt;
                gvFuelTypes.DataBind();
            }
        }

        protected void BtnAddFuelType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFuelTypeName.Text))
            {
                ShowMessage("الرجاء إدخال نوع الوقود", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO fuel_type (type, is_active) VALUES (@Type, 1)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Type", txtFuelTypeName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtFuelTypeName.Text = "";
                LoadFuelTypes();
                ShowMessage("تم الإضافة بنجاح", true);
            }
        }

        protected void TxtSearchFuelType_TextChanged(object sender, EventArgs e) => LoadFuelTypes();

        protected void GvFuelTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE fuel_type SET is_active = ISNULL(is_active, 1) ^ 1 WHERE fuel_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadFuelTypes();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvFuelTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvFuelTypes.EditIndex = e.NewEditIndex;
            LoadFuelTypes();
        }

        protected void GvFuelTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFuelTypes.EditIndex = -1;
            LoadFuelTypes();
        }

        protected void GvFuelTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvFuelTypes.DataKeys[e.RowIndex].Value);
            string type = ((TextBox)gvFuelTypes.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE fuel_type SET type = @Type WHERE fuel_id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvFuelTypes.EditIndex = -1;
            LoadFuelTypes();
            ShowMessage("تم التحديث بنجاح", true);
        }

        protected void GvFuelTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFuelTypes.PageIndex = e.NewPageIndex;
            LoadFuelTypes();
        }

        #endregion

        #region Fuel Prices Management

        private void LoadFuelLiquidPrices()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY ft.type) AS RowNum,
                            ft.fuel_id as fuel_type_id,
                            ft.type as fuel_type_name,
                            ISNULL(fp.price_per_liter, 0) as price_per_liter,
                            CASE WHEN fp.price_id IS NOT NULL THEN 1 ELSE 0 END as has_price
                        FROM fuel_type ft
                        LEFT JOIN fuel_prices fp ON ft.fuel_id = fp.fuel_type_id AND fp.is_active = 1
                        WHERE ft.is_active = 1
                        AND ft.type NOT LIKE N'%غاز%'
                        ORDER BY ft.type";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvFuelLiquidPrices.DataSource = dt;
                    gvFuelLiquidPrices.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل أسعار الوقود السائل: " + ex.Message, false);
                    System.Diagnostics.Debug.WriteLine($"Error loading fuel liquid prices: {ex.Message}");
                }
            }
        }

        private void LoadGasPrices()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY ft.type) AS RowNum,
                            ft.fuel_id as fuel_type_id,
                            ft.type as fuel_type_name,
                            ISNULL(fp.price_per_cubic_meter, 0) as price_per_cubic_meter,
                            CASE WHEN fp.price_id IS NOT NULL THEN 1 ELSE 0 END as has_price
                        FROM fuel_type ft
                        LEFT JOIN fuel_prices fp ON ft.fuel_id = fp.fuel_type_id AND fp.is_active = 1
                        WHERE ft.is_active = 1
                        AND ft.type LIKE N'%غاز%'
                        ORDER BY ft.type";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvGasPrices.DataSource = dt;
                    gvGasPrices.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل أسعار الغاز الطبيعي: " + ex.Message, false);
                    System.Diagnostics.Debug.WriteLine($"Error loading gas prices: {ex.Message}");
                }
            }
        }

        protected void GvFuelLiquidPrices_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvFuelLiquidPrices.EditIndex = e.NewEditIndex;
            LoadFuelLiquidPrices();
        }

        protected void GvFuelLiquidPrices_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFuelLiquidPrices.EditIndex = -1;
            LoadFuelLiquidPrices();
        }

        protected void GvFuelLiquidPrices_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int fuelTypeId = Convert.ToInt32(gvFuelLiquidPrices.DataKeys[e.RowIndex].Value);
                decimal pricePerLiter = 0;

                GridViewRow row = gvFuelLiquidPrices.Rows[e.RowIndex];
                TextBox txtPricePerLiter = (TextBox)row.FindControl("txtEditPricePerLiter");

                if (txtPricePerLiter != null && !string.IsNullOrEmpty(txtPricePerLiter.Text))
                    decimal.TryParse(txtPricePerLiter.Text, out pricePerLiter);

                UpdateFuelPrice(fuelTypeId, pricePerLiter, null);

                gvFuelLiquidPrices.EditIndex = -1;
                LoadFuelLiquidPrices();
                ShowMessage("تم تحديث سعر الوقود السائل بنجاح", true);
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحديث السعر: " + ex.Message, false);
                System.Diagnostics.Debug.WriteLine($"Error updating fuel liquid price: {ex.Message}");
            }
        }

        protected void GvFuelLiquidPrices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFuelLiquidPrices.PageIndex = e.NewPageIndex;
            LoadFuelLiquidPrices();
        }

        protected void GvFuelLiquidPrices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Handle any additional commands if needed
        }

        protected void GvGasPrices_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvGasPrices.EditIndex = e.NewEditIndex;
            LoadGasPrices();
        }

        protected void GvGasPrices_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvGasPrices.EditIndex = -1;
            LoadGasPrices();
        }

        protected void GvGasPrices_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int fuelTypeId = Convert.ToInt32(gvGasPrices.DataKeys[e.RowIndex].Value);
                decimal pricePerCubic = 0;

                GridViewRow row = gvGasPrices.Rows[e.RowIndex];
                TextBox txtPricePerCubic = (TextBox)row.FindControl("txtEditPricePerCubic");

                if (txtPricePerCubic != null && !string.IsNullOrEmpty(txtPricePerCubic.Text))
                    decimal.TryParse(txtPricePerCubic.Text, out pricePerCubic);

                UpdateFuelPrice(fuelTypeId, null, pricePerCubic);

                gvGasPrices.EditIndex = -1;
                LoadGasPrices();
                ShowMessage("تم تحديث سعر الغاز الطبيعي بنجاح", true);
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحديث السعر: " + ex.Message, false);
                System.Diagnostics.Debug.WriteLine($"Error updating gas price: {ex.Message}");
            }
        }

        protected void GvGasPrices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGasPrices.PageIndex = e.NewPageIndex;
            LoadGasPrices();
        }

        protected void GvGasPrices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Handle any additional commands if needed
        }

        private void UpdateFuelPrice(int fuelTypeId, decimal? pricePerLiter, decimal? pricePerCubic)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM fuel_prices WHERE fuel_type_id = @fuelTypeId AND is_active = 1";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    string updateQuery = "UPDATE fuel_prices SET updated_date = GETDATE()";
                    if (pricePerLiter.HasValue)
                        updateQuery += ", price_per_liter = @pricePerLiter";
                    if (pricePerCubic.HasValue)
                        updateQuery += ", price_per_cubic_meter = @pricePerCubic";
                    updateQuery += " WHERE fuel_type_id = @fuelTypeId AND is_active = 1";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    if (pricePerLiter.HasValue)
                        updateCmd.Parameters.AddWithValue("@pricePerLiter", pricePerLiter.Value);
                    if (pricePerCubic.HasValue)
                        updateCmd.Parameters.AddWithValue("@pricePerCubic", pricePerCubic.Value);
                    updateCmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    string insertQuery = @"
                        INSERT INTO fuel_prices (fuel_type_id, price_per_liter, price_per_cubic_meter, is_active, created_date)
                        VALUES (@fuelTypeId, @pricePerLiter, @pricePerCubic, 1, GETDATE())";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);
                    insertCmd.Parameters.AddWithValue("@pricePerLiter", pricePerLiter ?? 0);
                    insertCmd.Parameters.AddWithValue("@pricePerCubic", pricePerCubic ?? 0);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region License Types

        private void LoadLicenseTypes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY type) AS RowNum,
                        id_LT, 
                        type, 
                        ISNULL(is_active, 1) as is_active 
                    FROM v_license_type";
                
                string search = txtSearchLicenseType.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                    query = query.Replace("FROM v_license_type", "FROM v_license_type WHERE type LIKE @Search");
                query += " ORDER BY type";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvLicenseTypes.DataSource = dt;
                gvLicenseTypes.DataBind();
            }
        }

        protected void BtnAddLicenseType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLicenseTypeName.Text))
            {
                ShowMessage("الرجاء إدخال نوع الترخيص", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO v_license_type (type, is_active) VALUES (@Type, 1)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Type", txtLicenseTypeName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtLicenseTypeName.Text = "";
                LoadLicenseTypes();
                ShowMessage("تم الإضافة بنجاح", true);
            }
        }

        protected void TxtSearchLicenseType_TextChanged(object sender, EventArgs e) => LoadLicenseTypes();

        protected void GvLicenseTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE v_license_type SET is_active = ISNULL(is_active, 1) ^ 1 WHERE id_LT = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadLicenseTypes();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvLicenseTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLicenseTypes.EditIndex = e.NewEditIndex;
            LoadLicenseTypes();
        }

        protected void GvLicenseTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvLicenseTypes.EditIndex = -1;
            LoadLicenseTypes();
        }

        protected void GvLicenseTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvLicenseTypes.DataKeys[e.RowIndex].Value);
            string type = ((TextBox)gvLicenseTypes.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE v_license_type SET type = @Type WHERE id_LT = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvLicenseTypes.EditIndex = -1;
            LoadLicenseTypes();
            ShowMessage("تم التحديث بنجاح", true);
        }

        protected void GvLicenseTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLicenseTypes.PageIndex = e.NewPageIndex;
            LoadLicenseTypes();
        }

        #endregion

        #region Insurance Companies

        private void LoadInsuranceCompanies()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY company_name) AS RowNum,
                            id, 
                            company_name, 
                            ISNULL(is_active, 1) as is_active 
                        FROM insurance_companies";
                    
                    string search = txtSearchInsuranceCompany.Text.Trim();
                    if (!string.IsNullOrEmpty(search))
                        query = query.Replace("FROM insurance_companies", "FROM insurance_companies WHERE company_name LIKE @Search");
                    query += " ORDER BY company_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(search))
                        da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvInsuranceCompanies.DataSource = dt;
                    gvInsuranceCompanies.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message, false);
                }
            }
        }

        protected void BtnAddInsuranceCompany_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInsuranceCompanyName.Text))
            {
                ShowMessage("الرجاء إدخال اسم شركة التأمين", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM insurance_companies WHERE company_name = @Name";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", txtInsuranceCompanyName.Text.Trim());
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("اسم الشركة موجود بالفعل", false);
                        return;
                    }

                    string query = "INSERT INTO insurance_companies (company_name, is_active, created_date) VALUES (@Name, 1, GETDATE())";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", txtInsuranceCompanyName.Text.Trim());
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        txtInsuranceCompanyName.Text = "";
                        LoadInsuranceCompanies();
                        ShowMessage("تم إضافة شركة التأمين بنجاح", true);
                    }
                    else
                    {
                        ShowMessage("حدث خطأ أثناء إضافة الشركة", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void TxtSearchInsuranceCompany_TextChanged(object sender, EventArgs e) => LoadInsuranceCompanies();

        protected void GvInsuranceCompanies_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE insurance_companies SET is_active = ISNULL(is_active, 1) ^ 1, updated_date = GETDATE() WHERE id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                        LoadInsuranceCompanies();
                        ShowMessage("تم تغيير حالة الشركة بنجاح", true);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("خطأ: " + ex.Message, false);
                    }
                }
            }
        }

        protected void GvInsuranceCompanies_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvInsuranceCompanies.EditIndex = e.NewEditIndex;
            LoadInsuranceCompanies();
        }

        protected void GvInsuranceCompanies_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvInsuranceCompanies.EditIndex = -1;
            LoadInsuranceCompanies();
        }

        protected void GvInsuranceCompanies_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvInsuranceCompanies.DataKeys[e.RowIndex].Value);
            string companyName = ((TextBox)gvInsuranceCompanies.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM insurance_companies WHERE company_name = @Name AND id != @Id";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", companyName);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("اسم الشركة موجود بالفعل", false);
                        gvInsuranceCompanies.EditIndex = -1;
                        LoadInsuranceCompanies();
                        return;
                    }

                    string query = "UPDATE insurance_companies SET company_name = @Name, updated_date = GETDATE() WHERE id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", companyName);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    gvInsuranceCompanies.EditIndex = -1;
                    LoadInsuranceCompanies();
                    ShowMessage("تم تحديث الشركة بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void GvInsuranceCompanies_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInsuranceCompanies.PageIndex = e.NewPageIndex;
            LoadInsuranceCompanies();
        }

        #endregion

        #region Employee Positions

        private void LoadEmployeePositions()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY position_name) AS RowNum,
                            id, 
                            position_name, 
                            ISNULL(is_active, 1) as is_active 
                        FROM employee_positions";
                    
                    string search = txtSearchPosition.Text.Trim();
                    if (!string.IsNullOrEmpty(search))
                        query = query.Replace("FROM employee_positions", "FROM employee_positions WHERE position_name LIKE @Search");
                    query += " ORDER BY position_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(search))
                        da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvPositions.DataSource = dt;
                    gvPositions.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message, false);
                }
            }
        }

        protected void BtnAddPosition_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPositionName.Text))
            {
                ShowMessage("الرجاء إدخال اسم الوظيفة", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM employee_positions WHERE position_name = @Name";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", txtPositionName.Text.Trim());
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("اسم الوظيفة موجود بالفعل", false);
                        return;
                    }

                    string query = "INSERT INTO employee_positions (position_name, is_active, created_date) VALUES (@Name, 1, GETDATE())";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", txtPositionName.Text.Trim());
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        txtPositionName.Text = "";
                        LoadEmployeePositions();
                        ShowMessage("تم إضافة الوظيفة بنجاح", true);
                    }
                    else
                    {
                        ShowMessage("حدث خطأ أثناء إضافة الوظيفة", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void TxtSearchPosition_TextChanged(object sender, EventArgs e) => LoadEmployeePositions();

        protected void GvPositions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE employee_positions SET is_active = ISNULL(is_active, 1) ^ 1, updated_date = GETDATE() WHERE id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                        LoadEmployeePositions();
                        ShowMessage("تم تغيير حالة الوظيفة بنجاح", true);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("خطأ: " + ex.Message, false);
                    }
                }
            }
            else if (e.CommandName == "DeletePosition")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();

                        string checkQuery = "SELECT COUNT(*) FROM employees WHERE employee_position = (SELECT position_name FROM employee_positions WHERE id = @Id)";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Id", id);
                        int inUse = (int)checkCmd.ExecuteScalar();

                        if (inUse > 0)
                        {
                            ShowMessage("لا يمكن حذف هذه الوظيفة لأنها مستخدمة لدى موظفين", false);
                            return;
                        }

                        string query = "DELETE FROM employee_positions WHERE id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();

                        LoadEmployeePositions();
                        ShowMessage("تم حذف الوظيفة بنجاح", true);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("خطأ: " + ex.Message, false);
                    }
                }
            }
        }

        protected void GvPositions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPositions.EditIndex = e.NewEditIndex;
            LoadEmployeePositions();
        }

        protected void GvPositions_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPositions.EditIndex = -1;
            LoadEmployeePositions();
        }

        protected void GvPositions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvPositions.DataKeys[e.RowIndex].Value);
            string positionName = ((TextBox)gvPositions.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM employee_positions WHERE position_name = @Name AND id != @Id";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", positionName);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("اسم الوظيفة موجود بالفعل", false);
                        gvPositions.EditIndex = -1;
                        LoadEmployeePositions();
                        return;
                    }

                    string query = "UPDATE employee_positions SET position_name = @Name, updated_date = GETDATE() WHERE id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", positionName);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    gvPositions.EditIndex = -1;
                    LoadEmployeePositions();
                    ShowMessage("تم تحديث الوظيفة بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void GvPositions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPositions.PageIndex = e.NewPageIndex;
            LoadEmployeePositions();
        }

        #endregion

        #region Cards Management

        private void LoadCardsRepeater()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT card_id, card_number FROM fuel_cards ORDER BY card_id DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    rptCards.DataSource = dt;
                    rptCards.DataBind();
                    pnlNoCards.Visible = false;
                }
                else
                {
                    rptCards.DataSource = null;
                    rptCards.DataBind();
                    pnlNoCards.Visible = true;
                }
            }
        }

        protected void BtnAddCard_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtNewCardNumber.Text))
                {
                    ShowMessage("الرجاء إدخال رقم الكارت", false);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM fuel_cards WHERE card_number = @number";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@number", txtNewCardNumber.Text.Trim());
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        ShowMessage("رقم الكارت موجود بالفعل", false);
                        return;
                    }

                    string insertQuery = "INSERT INTO fuel_cards (card_number) VALUES (@number)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@number", txtNewCardNumber.Text.Trim());
                    insertCmd.ExecuteNonQuery();

                    ShowMessage("تم إضافة الكارت بنجاح", true);
                    txtNewCardNumber.Text = "";

                    LoadCardsRepeater();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في إضافة الكارت: " + ex.Message, false);
            }
        }

        protected void RptCards_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCard")
            {
                try
                {
                    int cardId = Convert.ToInt32(e.CommandArgument);

                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();

                        string checkQuery = "SELECT COUNT(*) FROM fuel_card_transactions WHERE card_id = @id";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@id", cardId);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ShowMessage("لا يمكن حذف الكارت لأنه مرتبط بمعاملات", false);
                            return;
                        }

                        string deleteQuery = "DELETE FROM fuel_cards WHERE card_id = @id";
                        SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                        deleteCmd.Parameters.AddWithValue("@id", cardId);
                        deleteCmd.ExecuteNonQuery();

                        ShowMessage("تم حذف الكارت بنجاح", true);
                        LoadCardsRepeater();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في حذف الكارت: " + ex.Message, false);
                }
            }
        }

        #endregion

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Visible = true;
        }
    }
}