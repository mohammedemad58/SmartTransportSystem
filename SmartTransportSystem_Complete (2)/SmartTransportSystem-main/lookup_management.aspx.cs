using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
            LoadYears();
            LoadCarBodyTypes();
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
            LoadRoutes();
        }

        #region Years Management

        private void LoadYears()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY car_year DESC) AS RowNum,
                            car_year AS year_value,
                            MAX(m_id) AS year_id,
                            CAST(1 AS BIT) AS is_active
                        FROM car_model
                        WHERE car_year IS NOT NULL
                        GROUP BY car_year";

                    string search = txtSearchModel.Text.Trim();
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (int.TryParse(search, out int searchYear))
                        {
                            query = "SELECT * FROM (" + query + ") AS Years WHERE year_value = @Search";
                        }
                        else
                        {
                            query = "SELECT * FROM (" + query + ") AS Years WHERE CAST(year_value AS NVARCHAR) LIKE @Search";
                        }
                    }

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (int.TryParse(search, out _))
                            da.SelectCommand.Parameters.AddWithValue("@Search", search);
                        else
                            da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvModels.DataSource = dt;
                    gvModels.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل سنوات الصنع: " + ex.Message, false);
                }
            }
        }

        protected void BtnAddModel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtModelYear.Text) || !int.TryParse(txtModelYear.Text.Trim(), out int yearValue))
            {
                ShowMessage("الرجاء إدخال سنة صنع صحيحة", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM car_model WHERE car_year = @Year";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Year", yearValue);

                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        ShowMessage("هذه السنة موجودة بالفعل", false);
                        return;
                    }

                    string query = @"INSERT INTO car_model (model, brand, car_year, is_active) 
                                    VALUES (@Model, @Brand, @Year, 1)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Model", "سنة " + yearValue);
                    cmd.Parameters.AddWithValue("@Brand", "عام");
                    cmd.Parameters.AddWithValue("@Year", yearValue);
                    cmd.ExecuteNonQuery();

                    txtModelYear.Text = "";
                    LoadYears();
                    ShowMessage("تم إضافة سنة الصنع بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void TxtSearchModel_TextChanged(object sender, EventArgs e) => LoadYears();

        protected void GvModels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int yearValue = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE car_model 
                                    SET is_active = CASE 
                                        WHEN EXISTS (SELECT 1 FROM car_model WHERE car_year = @Year AND is_active = 0) THEN 1 
                                        ELSE 0 
                                    END 
                                    WHERE car_year = @Year";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Year", yearValue);
                    cmd.ExecuteNonQuery();
                }
                LoadYears();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvModels_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvModels.EditIndex = e.NewEditIndex;
            LoadYears();
        }

        protected void GvModels_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvModels.EditIndex = -1;
            LoadYears();
        }

        protected void GvModels_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int oldYear = Convert.ToInt32(gvModels.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvModels.Rows[e.RowIndex];

            TextBox txtEditYear = (TextBox)row.FindControl("txtEditYear");

            if (txtEditYear == null)
            {
                ShowMessage("حدث خطأ في تحديث البيانات", false);
                gvModels.EditIndex = -1;
                LoadYears();
                return;
            }

            if (!int.TryParse(txtEditYear.Text.Trim(), out int newYear))
            {
                ShowMessage("الرجاء إدخال سنة صنع صحيحة", false);
                gvModels.EditIndex = -1;
                LoadYears();
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM car_model WHERE car_year = @Year AND car_year != @OldYear";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Year", newYear);
                    checkCmd.Parameters.AddWithValue("@OldYear", oldYear);

                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        ShowMessage("هذه السنة موجودة بالفعل", false);
                        gvModels.EditIndex = -1;
                        LoadYears();
                        return;
                    }

                    string query = "UPDATE car_model SET car_year = @NewYear WHERE car_year = @OldYear";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NewYear", newYear);
                    cmd.Parameters.AddWithValue("@OldYear", oldYear);
                    cmd.ExecuteNonQuery();

                    gvModels.EditIndex = -1;
                    LoadYears();
                    ShowMessage("تم تحديث سنة الصنع بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void GvModels_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvModels.PageIndex = e.NewPageIndex;
            LoadYears();
        }

        #endregion

        #region Car Body Types

        private void LoadCarBodyTypes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY body_name) AS RowNum,
                            body_id,
                            body_name,
                            ISNULL(is_active, 1) as is_active 
                        FROM car_body_type";

                    string search = txtSearchCarBodyType.Text.Trim();
                    if (!string.IsNullOrEmpty(search))
                    {
                        query = query.Replace("FROM car_body_type", "FROM car_body_type WHERE body_name LIKE @Search");
                    }
                    query += " ORDER BY body_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(search))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Columns.Contains("body_name"))
                    {
                        dt.Columns["body_name"].ColumnName = "body_type_name";
                    }

                    gvCarBodyTypes.DataSource = dt;
                    gvCarBodyTypes.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل هياكل السيارات: " + ex.Message, false);
                }
            }
        }

        protected void BtnAddCarBodyType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCarBodyTypeName.Text))
            {
                ShowMessage("الرجاء إدخال اسم هيكل السيارة", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM car_body_type WHERE body_name = @Name";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", txtCarBodyTypeName.Text.Trim());

                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        ShowMessage("هذا الهيكل موجود بالفعل", false);
                        return;
                    }

                    string query = "INSERT INTO car_body_type (body_name, is_active) VALUES (@Name, 1)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", txtCarBodyTypeName.Text.Trim());
                    cmd.ExecuteNonQuery();

                    txtCarBodyTypeName.Text = "";
                    LoadCarBodyTypes();
                    ShowMessage("تم إضافة هيكل السيارة بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void TxtSearchCarBodyType_TextChanged(object sender, EventArgs e) => LoadCarBodyTypes();

        protected void GvCarBodyTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE car_body_type SET is_active = ISNULL(is_active, 1) ^ 1 WHERE body_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadCarBodyTypes();
                ShowMessage("تم تغيير الحالة", true);
            }
        }

        protected void GvCarBodyTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCarBodyTypes.EditIndex = e.NewEditIndex;
            LoadCarBodyTypes();
        }

        protected void GvCarBodyTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCarBodyTypes.EditIndex = -1;
            LoadCarBodyTypes();
        }

        protected void GvCarBodyTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvCarBodyTypes.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvCarBodyTypes.Rows[e.RowIndex];

            TextBox txtEditCarBodyType = (TextBox)row.FindControl("txtEditCarBodyType");

            if (txtEditCarBodyType == null)
            {
                ShowMessage("حدث خطأ في تحديث البيانات", false);
                gvCarBodyTypes.EditIndex = -1;
                LoadCarBodyTypes();
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM car_body_type WHERE body_name = @Name AND body_id != @Id";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", txtEditCarBodyType.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Id", id);

                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        ShowMessage("هذا الهيكل موجود بالفعل", false);
                        gvCarBodyTypes.EditIndex = -1;
                        LoadCarBodyTypes();
                        return;
                    }

                    string query = "UPDATE car_body_type SET body_name = @Name WHERE body_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", txtEditCarBodyType.Text.Trim());
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    gvCarBodyTypes.EditIndex = -1;
                    LoadCarBodyTypes();
                    ShowMessage("تم تحديث هيكل السيارة بنجاح", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ: " + ex.Message, false);
                }
            }
        }

        protected void GvCarBodyTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCarBodyTypes.PageIndex = e.NewPageIndex;
            LoadCarBodyTypes();
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
            }
        }

        protected void GvFuelLiquidPrices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFuelLiquidPrices.PageIndex = e.NewPageIndex;
            LoadFuelLiquidPrices();
        }

        protected void GvFuelLiquidPrices_RowCommand(object sender, GridViewCommandEventArgs e) { }

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
            }
        }

        protected void GvGasPrices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGasPrices.PageIndex = e.NewPageIndex;
            LoadGasPrices();
        }

        protected void GvGasPrices_RowCommand(object sender, GridViewCommandEventArgs e) { }

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
                    cmd.ExecuteNonQuery();

                    txtInsuranceCompanyName.Text = "";
                    LoadInsuranceCompanies();
                    ShowMessage("تم إضافة شركة التأمين بنجاح", true);
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
                    conn.Open();
                    string query = "UPDATE insurance_companies SET is_active = ISNULL(is_active, 1) ^ 1, updated_date = GETDATE() WHERE id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadInsuranceCompanies();
                ShowMessage("تم تغيير الحالة", true);
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
                conn.Open();
                string query = "UPDATE insurance_companies SET company_name = @Name, updated_date = GETDATE() WHERE id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", companyName);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            gvInsuranceCompanies.EditIndex = -1;
            LoadInsuranceCompanies();
            ShowMessage("تم تحديث الشركة بنجاح", true);
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
                cmd.ExecuteNonQuery();

                txtPositionName.Text = "";
                LoadEmployeePositions();
                ShowMessage("تم إضافة الوظيفة بنجاح", true);
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
                    conn.Open();
                    string query = "UPDATE employee_positions SET is_active = ISNULL(is_active, 1) ^ 1, updated_date = GETDATE() WHERE id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadEmployeePositions();
                ShowMessage("تم تغيير الحالة", true);
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
        }

        protected void GvPositions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPositions.PageIndex = e.NewPageIndex;
            LoadEmployeePositions();
        }

        #endregion

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

                string query = "INSERT INTO violation_types (violation_type_name, is_active, created_date) VALUES (@Name, 1, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", txtViolationTypeName.Text.Trim());
                cmd.ExecuteNonQuery();

                txtViolationTypeName.Text = "";
                LoadViolationTypes();
                ShowMessage("تم إضافة نوع المخالفة بنجاح", true);
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
                    conn.Open();
                    string query = "UPDATE violation_types SET is_active = ISNULL(is_active, 1) ^ 1 WHERE violation_type_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadViolationTypes();
                ShowMessage("تم تغيير الحالة", true);
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
        }

        protected void GvViolationTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvViolationTypes.PageIndex = e.NewPageIndex;
            LoadViolationTypes();
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


        #region Routes Management

        private void LoadRoutes()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY r.route_name) AS RowNum,
                            r.route_id                      AS route_id,
                            r.route_name                    AS route_name,
                            r.[description]                 AS [description],
                            r.start_loc                     AS start_loc,
                            r.end_loc                       AS end_loc,
                            ISNULL(r.KM, 0)                 AS KM,
                            ISNULL(r.is_active, 1)          AS is_active
                        FROM dbo.routes r";

                    string search = txtSearchLookupRoute.Text.Trim();
                    if (!string.IsNullOrEmpty(search))
                    {
                        query += @"
                        WHERE r.route_name LIKE @Search 
                           OR r.start_loc LIKE @Search 
                           OR r.end_loc LIKE @Search 
                           OR r.[description] LIKE @Search";
                    }
                    query += " ORDER BY r.route_name";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(search))
                        da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Safety net: guarantee every column the grid uses exists
                    EnsureColumn(dt, "RowNum", typeof(long));
                    EnsureColumn(dt, "route_id", typeof(int));
                    EnsureColumn(dt, "route_name", typeof(string));
                    EnsureColumn(dt, "description", typeof(string));
                    EnsureColumn(dt, "start_loc", typeof(string));
                    EnsureColumn(dt, "end_loc", typeof(string));
                    EnsureColumn(dt, "KM", typeof(decimal));
                    EnsureColumn(dt, "is_active", typeof(bool));

                    gvLookupRoutes.DataSource = dt;
                    gvLookupRoutes.DataBind();
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل الخطوط: " + ex.Message, false);
                }
            }
        }

        private static void EnsureColumn(DataTable dt, string name, Type type)
        {
            if (!dt.Columns.Contains(name))
                dt.Columns.Add(name, type);
        }

        private void Notify(string message, bool ok)
        {
            ShowMessage(message, ok);
            string js = "alert('" + System.Web.HttpUtility.JavaScriptStringEncode(message) + "');";
            ClientScript.RegisterStartupScript(GetType(), "routeNotify" + Guid.NewGuid().ToString("N"), js, true);
        }

        protected void BtnAddLookupRoute_Click(object sender, EventArgs e)
        {
            hdnCurrentTab.Value = "routesManagement";

            string name = txtLookupRouteName.Text.Trim();
            string startLoc = txtLookupStartLoc.Text.Trim();
            string endLoc = txtLookupEndLoc.Text.Trim();
            string desc = txtLookupDescription.Text.Trim();

            if (name == "" || startLoc == "" || endLoc == "")
            {
                Notify("الرجاء إدخال اسم الخط ونقطة البداية والنهاية", false);
                return;
            }

            decimal km = 0;
            if (txtLookupKM.Text.Trim() != "")
                decimal.TryParse(txtLookupKM.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out km);

            try
            {
                int newId;

                // 1) Save inside a transaction
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        SqlCommand check = new SqlCommand(
                            "SELECT COUNT(*) FROM dbo.routes WHERE route_name = @Name", conn, tx);
                        check.Parameters.AddWithValue("@Name", name);
                        if ((int)check.ExecuteScalar() > 0)
                        {
                            tx.Rollback();
                            Notify("اسم الخط موجود بالفعل", false);
                            return;
                        }

                        SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO dbo.routes
                        (route_name, [description], start_loc, end_loc, KM, is_active, created_by_page)
                    VALUES
                        (@Name, @Description, @StartLoc, @EndLoc, @KM, 1, 'lookup_management');
                    SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx);
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = name;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
                            desc == "" ? (object)DBNull.Value : desc;
                        cmd.Parameters.Add("@StartLoc", SqlDbType.NVarChar, 200).Value = startLoc;
                        cmd.Parameters.Add("@EndLoc", SqlDbType.NVarChar, 200).Value = endLoc;
                        cmd.Parameters.Add("@KM", SqlDbType.Decimal).Value = km;
                        newId = Convert.ToInt32(cmd.ExecuteScalar());

                        tx.Commit();
                    }
                }

                // 2) Read it back on a NEW connection to prove it is in the table
                string proof;
                using (SqlConnection conn2 = new SqlConnection(_connectionString))
                {
                    conn2.Open();
                    SqlCommand verify = new SqlCommand(
                        "SELECT COUNT(*) FROM dbo.routes WHERE route_id = @Id", conn2);
                    verify.Parameters.AddWithValue("@Id", newId);
                    int found = (int)verify.ExecuteScalar();

                    SqlCommand total = new SqlCommand("SELECT COUNT(*) FROM dbo.routes", conn2);
                    int totalRows = (int)total.ExecuteScalar();

                    proof = "route_id=" + newId + " | found=" + found + " | total rows in routes=" + totalRows +
                            " | DB=" + conn2.Database + " | Server=" + conn2.DataSource;
                }

                txtLookupRouteName.Text = "";
                txtLookupStartLoc.Text = "";
                txtLookupEndLoc.Text = "";
                txtLookupDescription.Text = "";
                txtLookupKM.Text = "";

                gvLookupRoutes.PageIndex = 0;
                LoadRoutes();
                Notify("تم الحفظ في جدول routes - " + proof, true);
            }
            catch (Exception ex)
            {
                Notify("فشل الحفظ: " + ex.Message, false);
            }
        }

        protected void TxtSearchLookupRoute_TextChanged(object sender, EventArgs e)
        {
            gvLookupRoutes.PageIndex = 0;
            LoadRoutes();
        }

        protected void GvLookupRoutes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int id;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(
                            "UPDATE dbo.routes SET is_active = ISNULL(is_active, 1) ^ 1 WHERE route_id = @Id", conn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();

                        LoadRoutes();
                        ShowMessage("تم تغيير الحالة", true);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("خطأ: " + ex.Message, false);
                    }
                }
            }
        }

        protected void GvLookupRoutes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLookupRoutes.EditIndex = e.NewEditIndex;
            LoadRoutes();
        }

        protected void GvLookupRoutes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvLookupRoutes.EditIndex = -1;
            LoadRoutes();
        }

        protected void GvLookupRoutes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvLookupRoutes.DataKeys[e.RowIndex].Value);
                GridViewRow row = gvLookupRoutes.Rows[e.RowIndex];

                TextBox txtEditRouteName = (TextBox)row.FindControl("txtEditRouteName");
                TextBox txtEditStartLoc = (TextBox)row.FindControl("txtEditStartLoc");
                TextBox txtEditEndLoc = (TextBox)row.FindControl("txtEditEndLoc");
                TextBox txtEditDescription = (TextBox)row.FindControl("txtEditDescription");
                TextBox txtEditKM = (TextBox)row.FindControl("txtEditKM");

                string name = txtEditRouteName != null ? txtEditRouteName.Text.Trim() : "";
                string startLoc = txtEditStartLoc != null ? txtEditStartLoc.Text.Trim() : "";
                string endLoc = txtEditEndLoc != null ? txtEditEndLoc.Text.Trim() : "";
                string desc = txtEditDescription != null ? txtEditDescription.Text.Trim() : "";

                if (string.IsNullOrEmpty(name))
                {
                    ShowMessage("اسم الخط مطلوب", false);
                    gvLookupRoutes.EditIndex = -1;
                    LoadRoutes();
                    return;
                }

                decimal km = 0;
                if (txtEditKM != null && !string.IsNullOrEmpty(txtEditKM.Text.Trim()))
                    decimal.TryParse(txtEditKM.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out km);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM dbo.routes WHERE route_name = @Name AND route_id <> @Id", conn);
                    checkCmd.Parameters.AddWithValue("@Name", name);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("اسم الخط موجود بالفعل", false);
                        gvLookupRoutes.EditIndex = -1;
                        LoadRoutes();
                        return;
                    }

                    string query = @"
                        UPDATE dbo.routes SET
                            route_name  = @Name,
                            [description] = @Description,
                            start_loc   = @StartLoc,
                            end_loc     = @EndLoc,
                            KM          = @KM
                        WHERE route_id = @Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(desc) ? (object)DBNull.Value : desc);
                    cmd.Parameters.AddWithValue("@StartLoc", string.IsNullOrEmpty(startLoc) ? (object)DBNull.Value : startLoc);
                    cmd.Parameters.AddWithValue("@EndLoc", string.IsNullOrEmpty(endLoc) ? (object)DBNull.Value : endLoc);
                    cmd.Parameters.AddWithValue("@KM", km);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                gvLookupRoutes.EditIndex = -1;
                LoadRoutes();
                ShowMessage("تم تحديث الخط بنجاح", true);
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تحديث الخط: " + ex.Message, false);
                gvLookupRoutes.EditIndex = -1;
                LoadRoutes();
            }
        }

        protected void GvLookupRoutes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLookupRoutes.PageIndex = e.NewPageIndex;
            LoadRoutes();
        }

        #endregion

        #region Helper Method

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Visible = true;
        }

        #endregion

        #region Safe field accessors for gvLookupRoutes

        protected object SafeRouteValue(object dataItem, string columnName)
        {
            DataRowView drv = dataItem as DataRowView;
            if (drv == null) return null;
            if (!drv.DataView.Table.Columns.Contains(columnName)) return null;
            return drv[columnName];
        }

        protected string GetRouteField(object dataItem, string columnName)
        {
            object v = SafeRouteValue(dataItem, columnName);
            return (v == null || v == DBNull.Value) ? "" : Convert.ToString(v, CultureInfo.InvariantCulture);
        }

        protected string GetRouteFieldOrDash(object dataItem, string columnName)
        {
            string s = GetRouteField(dataItem, columnName);
            return string.IsNullOrEmpty(s) ? "—" : s;
        }

        protected string GetRouteKM(object dataItem)
        {
            object v = SafeRouteValue(dataItem, "KM");
            if (v == null || v == DBNull.Value) return "—";
            decimal d;
            return decimal.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture),
                       NumberStyles.Any, CultureInfo.InvariantCulture, out d)
                ? d.ToString("N2", CultureInfo.InvariantCulture)
                : "—";
        }

        protected string GetRouteKMForInput(object dataItem)
        {
            object v = SafeRouteValue(dataItem, "KM");
            if (v == null || v == DBNull.Value) return "";
            decimal d;
            return decimal.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture),
                       NumberStyles.Any, CultureInfo.InvariantCulture, out d)
                ? d.ToString("0.##", CultureInfo.InvariantCulture)
                : "";
        }

        protected bool IsRouteActive(object dataItem)
        {
            object v = SafeRouteValue(dataItem, "is_active");
            return v != null && v != DBNull.Value && Convert.ToBoolean(v);
        }

        protected string GetRouteStatusBadge(object dataItem)
        {
            bool active = IsRouteActive(dataItem);
            string cls = active ? "status-active" : "status-inactive";
            string text = active ? "نشط" : "غير نشط";
            return "<span class='status-badge " + cls + "'>" + text + "</span>";
        }

        #endregion
    }
}


