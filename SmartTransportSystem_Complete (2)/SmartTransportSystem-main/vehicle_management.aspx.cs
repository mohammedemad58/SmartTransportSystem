using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class vehicle_management : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int _currentVehicleId = 0;
        private int _currentCarNumberId = 0;

        // SQL expression: the plate shown in the driver's car dropdown
        private const string CarDisplaySql =
            "RTRIM(cn.letter1) + ' ' + ISNULL(RTRIM(cn.letter2), '') + " +
            "CASE WHEN cn.letter3 IS NOT NULL AND RTRIM(cn.letter3) <> '' THEN ' ' + RTRIM(cn.letter3) ELSE '' END + " +
            "' ' + RTRIM(cn.plate_numbers)";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadAllYears();
                MainMultiView.ActiveViewIndex = 0;
                btnTabEntry.CssClass = "btn-tab active";
                btnTabSearch.CssClass = "btn-tab";
            }

            // DRIVER: search only - no adding new cars
            if (AuthHelper.IsRestrictedUser)
            {
                btnTabEntry.Visible = false;
                MainMultiView.ActiveViewIndex = 1;
                btnTabSearch.CssClass = "btn-tab active";

                // DRIVER ONLY: choose the car from a dropdown instead of typing the plate
                colPlateSearch.Visible = false;
                colDriverCar.Visible = true;
                if (!IsPostBack)
                {
                    LoadDriverCars();
                }
            }
        }

        #region Driver cars (dropdown for a driver login only)

        /// <summary>
        /// Fills the dropdown with ONLY the cars of the logged-in driver:
        ///  - cars linked to him on the lines (routes.driver_id -> routes.car_id)
        ///  - cars on his own unfinished work orders
        /// Several cars: "-- اختر السيارة --" + the cars. One car: only that car.
        /// </summary>
        private void LoadDriverCars()
        {
            ddlDriverCar.Items.Clear();

            int? empId = AuthHelper.CurrentEmployeeId;
            if (!empId.HasValue)
            {
                ddlDriverCar.Items.Add(new ListItem("-- لا توجد سيارات --", ""));
                return;
            }

            try
            {
                string query = @"
                    SELECT DISTINCT x.car_number_id, x.car_display
                    FROM (
                        SELECT cn.car_number_id, " + CarDisplaySql + @" AS car_display
                        FROM routes r
                        INNER JOIN main_vehicle_details mvd ON r.car_id = mvd.v_id
                        INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                        WHERE r.driver_id = @EmpId
                          AND r.car_id IS NOT NULL
                          AND cn.is_active = 1

                        UNION ALL

                        SELECT cn.car_number_id, " + CarDisplaySql + @" AS car_display
                        FROM operation_command oc
                        INNER JOIN car_numbers cn ON oc.v_number = cn.car_number_id
                        WHERE oc.driver_name = @EmpId
                          AND oc.deleted = 0
                          AND ISNULL(oc.trip_status, 0) <> 2
                    ) x
                    ORDER BY x.car_display";

                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId.Value;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    ddlDriverCar.Items.Add(new ListItem("-- لا توجد سيارات لك --", ""));
                    return;
                }

                if (dt.Rows.Count > 1)
                    ddlDriverCar.Items.Add(new ListItem("-- اختر السيارة --", ""));

                foreach (DataRow row in dt.Rows)
                {
                    ddlDriverCar.Items.Add(new ListItem(row["car_display"].ToString().Trim(), row["car_number_id"].ToString()));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading driver cars: {ex.Message}");
                ddlDriverCar.Items.Clear();
                ddlDriverCar.Items.Add(new ListItem("-- لا توجد سيارات --", ""));
            }
        }

        // True when the car is linked to the logged-in driver on a line (routes)
        private bool DriverHasCarOnRoute(int carNumberId)
        {
            int? empId = AuthHelper.CurrentEmployeeId;
            if (!empId.HasValue) return false;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM routes r
                    INNER JOIN main_vehicle_details mvd ON r.car_id = mvd.v_id
                    WHERE r.driver_id = @EmpId AND mvd.v_num = @CarId", conn))
                {
                    cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId.Value;
                    cmd.Parameters.Add("@CarId", SqlDbType.Int).Value = carNumberId;
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // Driver: puts the letters / numbers of the chosen car into the (hidden) plate boxes,
        // so the existing search code works unchanged.
        private bool FillSearchBoxesFromChosenCar()
        {
            int carId;
            if (!int.TryParse(ddlDriverCar.SelectedValue, out carId) || carId <= 0)
                return false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT letter1, letter2, letter3, plate_numbers FROM car_numbers WHERE car_number_id = @Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = carId;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return false;

                    txtSearchLetter1.Text = reader["letter1"] == DBNull.Value ? "" : reader["letter1"].ToString().Trim();
                    txtSearchLetter2.Text = reader["letter2"] == DBNull.Value ? "" : reader["letter2"].ToString().Trim();
                    txtSearchLetter3.Text = reader["letter3"] == DBNull.Value ? "" : reader["letter3"].ToString().Trim();
                    txtSearchNumbers.Text = reader["plate_numbers"] == DBNull.Value ? "" : reader["plate_numbers"].ToString().Trim();
                    return true;
                }
            }
        }

        #endregion

        #region Dropdown Loading Methods

        private void LoadDropdowns()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string carBrandsQuery = "SELECT DISTINCT car_type_id, car_brand FROM car_types WHERE is_active = 1 ORDER BY car_brand";
                    SqlDataAdapter da = new SqlDataAdapter(carBrandsQuery, conn);
                    DataTable dtBrands = new DataTable();
                    da.Fill(dtBrands);
                    ddlCarType.DataSource = dtBrands;
                    ddlCarType.DataTextField = "car_brand";
                    ddlCarType.DataValueField = "car_brand";
                    ddlCarType.DataBind();
                    ddlCarType.Items.Insert(0, new ListItem("-- اختر نوع السيارة --", ""));

                    string carModelQuery = "SELECT car_type_id, car_type_name FROM car_types WHERE is_active = 1 ORDER BY car_type_name";
                    da = new SqlDataAdapter(carModelQuery, conn);
                    DataTable dtModels = new DataTable();
                    da.Fill(dtModels);
                    ddlCarModel.DataSource = dtModels;
                    ddlCarModel.DataTextField = "car_type_name";
                    ddlCarModel.DataValueField = "car_type_id";
                    ddlCarModel.DataBind();
                    ddlCarModel.Items.Insert(0, new ListItem("-- اختر الموديل --", ""));

                    string carBodyQuery = "SELECT body_id, body_name FROM car_body_type WHERE is_active = 1 ORDER BY body_name";
                    da = new SqlDataAdapter(carBodyQuery, conn);
                    DataTable dtBody = new DataTable();
                    da.Fill(dtBody);
                    ddlCarBody.DataSource = dtBody;
                    ddlCarBody.DataTextField = "body_name";
                    ddlCarBody.DataValueField = "body_id";
                    ddlCarBody.DataBind();
                    ddlCarBody.Items.Insert(0, new ListItem("-- اختر هيكل السيارة --", ""));

                    string statusQuery = "SELECT s_id, status FROM car_status WHERE is_active = 1 ORDER BY status";
                    da = new SqlDataAdapter(statusQuery, conn);
                    DataTable dtStatus = new DataTable();
                    da.Fill(dtStatus);
                    ddlStatus.DataSource = dtStatus;
                    ddlStatus.DataTextField = "status";
                    ddlStatus.DataValueField = "s_id";
                    ddlStatus.DataBind();
                    ddlStatus.Items.Insert(0, new ListItem("-- اختر الحالة --", ""));

                    string fuelQuery = "SELECT fuel_id, type FROM fuel_type WHERE is_active = 1 ORDER BY type";
                    da = new SqlDataAdapter(fuelQuery, conn);
                    DataTable dtFuel = new DataTable();
                    da.Fill(dtFuel);
                    ddlFuelType.DataSource = dtFuel;
                    ddlFuelType.DataTextField = "type";
                    ddlFuelType.DataValueField = "fuel_id";
                    ddlFuelType.DataBind();
                    ddlFuelType.Items.Insert(0, new ListItem("-- اختر نوع الوقود --", ""));
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل البيانات: " + ex.Message);
                }
            }
        }

        private void LoadAllYears()
        {
            ddlYear.Items.Clear();
            ddlYear.Items.Insert(0, new ListItem("-- اختر سنة الصنع --", ""));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"SELECT DISTINCT car_year 
                                    FROM car_model 
                                    WHERE car_year IS NOT NULL 
                                    AND car_year > 0
                                    ORDER BY car_year DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["car_year"] != DBNull.Value)
                            {
                                int year = Convert.ToInt32(reader["car_year"]);
                                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل سنوات الصنع: " + ex.Message);
                }
            }
        }

        protected void DdlCarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBrand = ddlCarType.SelectedValue;
            LoadModelsByBrand(selectedBrand);
        }

        private void LoadModelsByBrand(string brand)
        {
            ddlCarModel.Items.Clear();
            ddlCarModel.Items.Insert(0, new ListItem("-- اختر الموديل --", ""));

            if (string.IsNullOrEmpty(brand))
                return;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"SELECT car_type_id, car_type_name 
                                    FROM car_types 
                                    WHERE is_active = 1 
                                    AND car_brand = @Brand 
                                    ORDER BY car_type_name";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Brand", brand);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ddlCarModel.Items.Add(new ListItem(
                                    reader["car_type_name"].ToString(),
                                    reader["car_type_id"].ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في تحميل الموديلات: " + ex.Message);
                }
            }
        }

        protected void DdlCarBody_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Years remain independent
        }

        #endregion

        #region Validation Methods

        private bool ValidateChassisNumber(string chassisNumber)
        {
            if (string.IsNullOrEmpty(chassisNumber))
            {
                ShowMessage("رقم الشاسيه مطلوب");
                return false;
            }
            chassisNumber = chassisNumber.Trim().ToUpper();
            if (chassisNumber.Length != 17)
            {
                ShowMessage("رقم الشاسيه يجب أن يكون 17 رمزاً بالضبط");
                return false;
            }
            if (chassisNumber.Contains("I") || chassisNumber.Contains("O"))
            {
                ShowMessage("رقم الشاسيه لا يمكن أن يحتوي على الحرفين I أو O");
                return false;
            }
            if (!Regex.IsMatch(chassisNumber, @"^[A-HJ-NPR-Z0-9]{17}$"))
            {
                ShowMessage("رقم الشاسيه يجب أن يحتوي فقط على أرقام (0-9) وحروف أبجدية (ما عدا I, O)");
                return false;
            }
            return true;
        }

        private bool IsChassisNumberExists(string chassisNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT COUNT(*) FROM main_vehicle_details WHERE chassis = @ChassisNumber AND deleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ChassisNumber", chassisNumber.Trim().ToUpper());
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsCarNumberExists(string letter1, string letter2, string letter3, string numbers)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT COUNT(*) FROM car_numbers 
                                    WHERE letter1 = @Letter1 
                                    AND (letter2 = @Letter2 OR (letter2 IS NULL AND @Letter2 IS NULL))
                                    AND (letter3 = @Letter3 OR (letter3 IS NULL AND @Letter3 IS NULL))
                                    AND plate_numbers = @Numbers
                                    AND is_active = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Letter1", letter1.Trim());
                        cmd.Parameters.AddWithValue("@Letter2", string.IsNullOrEmpty(letter2) ? DBNull.Value : (object)letter2.Trim());
                        cmd.Parameters.AddWithValue("@Letter3", string.IsNullOrEmpty(letter3) ? DBNull.Value : (object)letter3.Trim());
                        cmd.Parameters.AddWithValue("@Numbers", numbers.Trim());
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtCarLetter1.Text) || string.IsNullOrEmpty(txtCarNumbers.Text))
            {
                ShowMessage("الرجاء إدخال رقم السيارة (حرف واحد على الأقل والأرقام)");
                return false;
            }
            if (ddlCarType.SelectedValue == "") { ShowMessage("الرجاء اختيار نوع السيارة (العلامة التجارية)"); return false; }
            if (ddlCarModel.SelectedValue == "") { ShowMessage("الرجاء اختيار الموديل"); return false; }
            if (ddlCarBody.SelectedValue == "") { ShowMessage("الرجاء اختيار هيكل السيارة"); return false; }
            if (ddlYear.SelectedValue == "") { ShowMessage("الرجاء اختيار سنة الصنع"); return false; }
            if (ddlStatus.SelectedValue == "") { ShowMessage("الرجاء اختيار حالة السيارة"); return false; }
            if (ddlFuelType.SelectedValue == "") { ShowMessage("الرجاء اختيار نوع الوقود"); return false; }
            if (string.IsNullOrEmpty(ddlNaturalGas.SelectedValue)) { ShowMessage("الرجاء اختيار هل السيارة مزودة بغاز طبيعي"); return false; }
            if (string.IsNullOrEmpty(txtPassengers.Text)) { ShowMessage("الرجاء إدخال عدد الركاب"); return false; }
            if (string.IsNullOrEmpty(txtChassis.Text)) { ShowMessage("الرجاء إدخال رقم الشاسيه"); return false; }
            if (!ValidateChassisNumber(txtChassis.Text)) return false;
            if (string.IsNullOrEmpty(txtMotor.Text)) { ShowMessage("الرجاء إدخال رقم الموتور"); return false; }

            if (IsChassisNumberExists(txtChassis.Text))
            {
                ShowMessage("رقم الشاسيه موجود بالفعل في قاعدة البيانات! لا يمكن إدخال نفس رقم الشاسيه مرتين.");
                return false;
            }

            if (IsCarNumberExists(txtCarLetter1.Text, txtCarLetter2.Text, txtCarLetter3.Text, txtCarNumbers.Text))
            {
                ShowMessage("رقم السيارة موجود بالفعل في قاعدة البيانات! لا يمكن إدخال نفس رقم السيارة مرتين.");
                return false;
            }

            return true;
        }

        #endregion

        #region Save Methods

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            // DRIVER: cannot add cars
            if (AuthHelper.IsRestrictedUser) return;

            try
            {
                if (!ValidateInputs()) return;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        int carNumberId = InsertCarNumber(conn, transaction);
                        InsertVehicleDetails(conn, transaction, carNumberId);
                        transaction.Commit();
                        ShowSuccess("تم حفظ بيانات السيارة بنجاح!");
                        ClearForm();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء حفظ البيانات: " + ex.Message);
            }
        }

        private int InsertCarNumber(SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"INSERT INTO car_numbers (letter1, letter2, letter3, plate_numbers, created_date, is_active)
                            VALUES (@Letter1, @Letter2, @Letter3, @Numbers, @CreatedDate, 1);
                            SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Letter1", txtCarLetter1.Text.Trim());
                cmd.Parameters.AddWithValue("@Letter2", string.IsNullOrEmpty(txtCarLetter2.Text) ? DBNull.Value : (object)txtCarLetter2.Text.Trim());
                cmd.Parameters.AddWithValue("@Letter3", string.IsNullOrEmpty(txtCarLetter3.Text) ? DBNull.Value : (object)txtCarLetter3.Text.Trim());
                cmd.Parameters.AddWithValue("@Numbers", txtCarNumbers.Text.Trim());
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void InsertVehicleDetails(SqlConnection conn, SqlTransaction transaction, int carNumberId)
        {
            string query = @"INSERT INTO main_vehicle_details 
        (v_num, v_type, v_model, v_year, v_statues, gas_type, passenger, chassis, motor_num, 
         reg_date, deleted, hide, has_natural_gas, initial_km, current_odometer)
        VALUES 
        (@VNum, @VType, @VModel, @VYear, @VStatus, @GasType, @Passenger, @Chassis, @Motor, 
         @RegDate, @Deleted, @Hide, @HasNaturalGas, @InitialKM, @InitialKM)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@VNum", carNumberId);
                cmd.Parameters.AddWithValue("@VType", ddlCarModel.SelectedValue);
                cmd.Parameters.AddWithValue("@VModel", ddlCarBody.SelectedValue);
                cmd.Parameters.AddWithValue("@VYear", ddlYear.SelectedValue);
                cmd.Parameters.AddWithValue("@VStatus", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@GasType", ddlFuelType.SelectedValue);
                cmd.Parameters.AddWithValue("@Passenger", txtPassengers.Text.Trim());
                cmd.Parameters.AddWithValue("@Chassis", txtChassis.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@Motor", txtMotor.Text.Trim());
                cmd.Parameters.AddWithValue("@RegDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Deleted", 0);
                cmd.Parameters.AddWithValue("@Hide", 0);

                bool hasNaturalGas = ddlNaturalGas.SelectedValue == "1";
                cmd.Parameters.AddWithValue("@HasNaturalGas", hasNaturalGas);

                decimal initialKMValue = 0;
                if (!string.IsNullOrEmpty(txtInitialKM.Text))
                {
                    decimal.TryParse(txtInitialKM.Text, out initialKMValue);
                }
                cmd.Parameters.AddWithValue("@InitialKM", initialKMValue);

                cmd.ExecuteNonQuery();
            }
        }

        protected void BtnClear_Click(object sender, EventArgs e) { ClearForm(); }

        private void ClearForm()
        {
            txtCarLetter1.Text = ""; txtCarLetter2.Text = ""; txtCarLetter3.Text = ""; txtCarNumbers.Text = "";
            ddlCarType.SelectedValue = "";
            ddlCarModel.Items.Clear();
            ddlCarModel.Items.Insert(0, new ListItem("-- اختر الموديل --", ""));
            ddlCarBody.SelectedValue = "";
            ddlYear.Items.Clear();
            LoadAllYears();
            ddlStatus.SelectedValue = ""; ddlFuelType.SelectedValue = "";
            ddlNaturalGas.SelectedValue = "";
            txtPassengers.Text = ""; txtChassis.Text = ""; txtMotor.Text = "";
            txtInitialKM.Text = "";
            lblMessage.Visible = false; lblSuccess.Visible = false;
        }

        #endregion

        #region Search Methods

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // DRIVER ONLY: the car comes from the dropdown
                if (AuthHelper.IsRestrictedUser)
                {
                    if (!FillSearchBoxesFromChosenCar())
                    { ShowMessage("الرجاء اختيار السيارة"); resultBox.Visible = false; return; }
                }

                if (string.IsNullOrEmpty(txtSearchLetter1.Text) || string.IsNullOrEmpty(txtSearchNumbers.Text))
                { ShowMessage("الرجاء إدخال رقم السيارة كاملاً"); return; }
                SearchVehicleByNumber();
            }
            catch (Exception ex) { ShowMessage("حدث خطأ أثناء البحث: " + ex.Message); }
        }

        private void SearchVehicleByNumber()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string getCarNumberIdQuery = @"SELECT car_number_id FROM car_numbers WHERE letter1 = @Letter1 AND (letter2 = @Letter2 OR (letter2 IS NULL AND @Letter2 IS NULL)) AND (letter3 = @Letter3 OR (letter3 IS NULL AND @Letter3 IS NULL)) AND plate_numbers = @Numbers";
                    using (SqlCommand cmd = new SqlCommand(getCarNumberIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Letter1", txtSearchLetter1.Text.Trim());
                        cmd.Parameters.AddWithValue("@Letter2", string.IsNullOrEmpty(txtSearchLetter2.Text) ? DBNull.Value : (object)txtSearchLetter2.Text.Trim());
                        cmd.Parameters.AddWithValue("@Letter3", string.IsNullOrEmpty(txtSearchLetter3.Text) ? DBNull.Value : (object)txtSearchLetter3.Text.Trim());
                        cmd.Parameters.AddWithValue("@Numbers", txtSearchNumbers.Text.Trim());
                        object carNumberIdObj = cmd.ExecuteScalar();
                        if (carNumberIdObj == null) { ShowMessage("رقم السيارة غير موجود"); resultBox.Visible = false; return; }
                        _currentCarNumberId = Convert.ToInt32(carNumberIdObj);

                        // DRIVER: only his own cars (his work orders, or the cars linked to him on the lines)
                        if (AuthHelper.IsRestrictedUser
                            && !AuthHelper.IsCarAllowedForCurrentDriver(_currentCarNumberId)
                            && !DriverHasCarOnRoute(_currentCarNumberId))
                        {
                            ShowMessage("هذه السيارة ليست من سياراتك");
                            resultBox.Visible = false;
                            return;
                        }

                        hdnCarNumberId.Value = _currentCarNumberId.ToString();
                    }
                    string getVehicleIdQuery = "SELECT TOP 1 v_id FROM main_vehicle_details WHERE v_num = @CarNumberId ORDER BY v_id DESC";
                    using (SqlCommand cmd = new SqlCommand(getVehicleIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarNumberId", _currentCarNumberId);
                        object vehicleIdObj = cmd.ExecuteScalar();
                        if (vehicleIdObj == null) { ShowMessage("لا توجد بيانات للسيارة"); resultBox.Visible = false; return; }
                        _currentVehicleId = Convert.ToInt32(vehicleIdObj);
                        hdnVehicleID.Value = _currentVehicleId.ToString();
                    }
                    LoadVehicleData(conn);
                    resultBox.Visible = true;
                    ShowSuccess("تم العثور على السيارة");
                    upInsurance.Update(); upMaintenance.Update(); upLicense.Update();
                }
                catch (Exception ex) { ShowMessage("خطأ: " + ex.Message); }
            }
        }

        #endregion

        #region Odometer Methods

        private decimal GetLastOdometerFromWorkOrders(int carNumberId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string checkQuery = @"SELECT COUNT(1) FROM operation_command 
                                 WHERE v_number = @CarNumberId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                        conn.Open();
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                            return 0;
                    }

                    string query = @"
                SELECT TOP 1 oc.kilometer_end
                FROM operation_command oc
                WHERE oc.v_number = @CarNumberId
                  AND oc.kilometer_end IS NOT NULL
                  AND oc.kilometer_end > 0
                ORDER BY oc.mission_ending_time DESC, oc.mission_id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch
            {
                return 0;
            }
            return 0;
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
                ORDER BY mission_ending_time DESC, mission_id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch
            {
                return 0;
            }
            return 0;
        }

        #endregion

        #region Vehicle Data Loading

        private void LoadVehicleData(SqlConnection conn)
        {
            string query = @"SELECT mvd.v_type, mvd.v_model, mvd.v_year, mvd.v_statues, mvd.chassis, 
                            mvd.motor_num, mvd.passenger, mvd.gas_type, mvd.note, mvd.has_natural_gas,
                            mvd.initial_km,
                            ct.car_brand,
                            ct.car_type_name AS model_name,
                            cbt.body_name AS body_name,
                            cs.status AS status_name, ft.type AS fuel_type_name
                    FROM main_vehicle_details mvd
                    LEFT JOIN car_types ct ON mvd.v_type = ct.car_type_id
                    LEFT JOIN car_body_type cbt ON mvd.v_model = cbt.body_id
                    LEFT JOIN car_status cs ON mvd.v_statues = cs.s_id
                    LEFT JOIN fuel_type ft ON mvd.gas_type = ft.fuel_id
                    WHERE mvd.v_id = @VehicleId";

            decimal lastOdometer = 0;

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string carBrand = reader["car_brand"]?.ToString() ?? "";
                        string modelName = reader["model_name"]?.ToString() ?? "";
                        string bodyName = reader["body_name"] != DBNull.Value ? reader["body_name"].ToString() : "";

                        txtType.Text = carBrand;
                        txtModel.Text = modelName;
                        txtCarBody.Text = bodyName;
                        txtYear.Text = reader["v_year"]?.ToString() ?? "";
                        txtStatus.Text = reader["status_name"]?.ToString() ?? "";
                        txtChassisView.Text = reader["chassis"]?.ToString() ?? "";
                        txtMotorView.Text = reader["motor_num"]?.ToString() ?? "";
                        txtGasView.Text = reader["fuel_type_name"]?.ToString() ?? "";
                        txtPassengersView.Text = reader["passenger"]?.ToString() ?? "";
                        txtNoteView.Text = reader["note"]?.ToString() ?? "";

                        object hasNaturalGas = reader["has_natural_gas"];
                        txtNaturalGasView.Text = (hasNaturalGas != DBNull.Value && Convert.ToBoolean(hasNaturalGas)) ? "نعم" : "لا";

                        decimal initialKM = reader["initial_km"] != DBNull.Value ? Convert.ToDecimal(reader["initial_km"]) : 0;
                        lblInitialKM.Text = initialKM.ToString("N0");

                        lastOdometer = GetLastOdometerFromWorkOrders(_currentCarNumberId);
                        lblCurrentKM.Text = lastOdometer.ToString("N0");
                    }
                }
            }

            LoadCurrentLicense(conn);
            LoadMandatoryInsuranceForCurrentLicense(conn);
            LoadEngineOverhaulInfo(conn, _currentVehicleId);
            CheckMaintenanceAlerts(conn, _currentVehicleId, (int)lastOdometer);

            string displayNumber = txtSearchLetter1.Text.Trim();
            if (!string.IsNullOrEmpty(txtSearchLetter2.Text)) displayNumber += " " + txtSearchLetter2.Text.Trim();
            if (!string.IsNullOrEmpty(txtSearchLetter3.Text)) displayNumber += " " + txtSearchLetter3.Text.Trim();
            displayNumber += " " + txtSearchNumbers.Text.Trim();
            lblCarNumber.Text = displayNumber;

            LoadInsuranceData(conn);
            LoadMaintenanceData(conn);
            LoadLicenseHistoryData(conn);
        }

        #endregion

        #region Engine Overhaul

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
                            object overhaulDateObj = reader["EngineOverhaulDate"];

                            if (overhaulDateObj != DBNull.Value && overhaulDateObj != null)
                            {
                                DateTime overhaulDate = Convert.ToDateTime(overhaulDateObj);
                                int overhaulOdometer = reader["EngineOverhaulOdometer"] != DBNull.Value ? Convert.ToInt32(reader["EngineOverhaulOdometer"]) : 0;
                                decimal overhaulCost = reader["EngineOverhaulCost"] != DBNull.Value ? Convert.ToDecimal(reader["EngineOverhaulCost"]) : 0;
                                string overhaulNotes = reader["EngineOverhaulNotes"] != DBNull.Value ? reader["EngineOverhaulNotes"].ToString() : "";

                                int currentOdometer = GetLastWorkOrderKilometer(_currentCarNumberId);
                                int kmSinceOverhaul = currentOdometer - overhaulOdometer;
                                if (kmSinceOverhaul < 0) kmSinceOverhaul = 0;

                                lblEngineOverhaulDate.Text = overhaulDate.ToString("yyyy-MM-dd");
                                lblEngineOverhaulOdometer.Text = overhaulOdometer.ToString("N0");
                                lblEngineOverhaulDistance.Text = kmSinceOverhaul.ToString("N0");
                                lblEngineOverhaulCost.Text = overhaulCost.ToString("N2");
                                lblEngineOverhaulNotes.Text = string.IsNullOrEmpty(overhaulNotes) ? "—" : overhaulNotes;

                                string statusMessage = "";
                                string statusClass = "";

                                if (kmSinceOverhaul <= 50000)
                                {
                                    statusMessage = $"🟢 حالة الموتور جيدة - تم قطع {kmSinceOverhaul:N0} كم منذ العمرة";
                                    statusClass = "alert-success";
                                }
                                else if (kmSinceOverhaul <= 100000)
                                {
                                    statusMessage = $"🟡 يحتاج إلى متابعة - تم قطع {kmSinceOverhaul:N0} كم منذ العمرة";
                                    statusClass = "alert-warning";
                                }
                                else
                                {
                                    statusMessage = $"🔴 يحتاج إلى عمرة جديدة - تم قطع {kmSinceOverhaul:N0} كم منذ العمرة";
                                    statusClass = "alert-danger";
                                }

                                lblOverhaulStatus.Text = statusMessage;
                                overhaulStatusDiv.Attributes.Add("class", $"alert {statusClass}");
                                engineOverhaulRow.Visible = true;
                            }
                            else
                            {
                                engineOverhaulRow.Visible = false;
                            }
                        }
                        else
                        {
                            engineOverhaulRow.Visible = false;
                        }
                    }
                }
            }
            catch
            {
                engineOverhaulRow.Visible = false;
            }
        }

        #endregion

        #region License Methods

        private void LoadCurrentLicense(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT TOP 1 
                        vl.license_number,
                        CONVERT(DATE, vl.issue_date) AS issue_date,
                        CONVERT(DATE, vl.expiry_date) AS expiry_date,
                        tu.name AS traffic_unit,
                        lt.type AS license_type
                    FROM vehicle_licenses vl
                    LEFT JOIN v_license_type lt ON vl.license_type_id = lt.id_LT
                    LEFT JOIN traffic_unit tu ON vl.traffic_unit_id = tu.id_TF
                    WHERE vl.vehicle_id = @VehicleId 
                      AND (vl.deleted = 0 OR vl.deleted IS NULL)
                      AND vl.is_active = 1
                    ORDER BY vl.issue_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtCurrentLicenseNumber.Text = reader["license_number"]?.ToString() ?? "غير محدد";
                            txtLicenseStartView.Text = reader["issue_date"] != DBNull.Value ?
                                Convert.ToDateTime(reader["issue_date"]).ToString("dd/MM/yyyy") : "غير محدد";
                            txtLicenseEndView.Text = reader["expiry_date"] != DBNull.Value ?
                                Convert.ToDateTime(reader["expiry_date"]).ToString("dd/MM/yyyy") : "غير محدد";
                            txtTrafficUnitView.Text = reader["traffic_unit"]?.ToString() ?? "غير محدد";
                            txtLicenseTypeView.Text = reader["license_type"]?.ToString() ?? "غير محدد";

                            if (reader["expiry_date"] != DBNull.Value)
                            {
                                CheckLicenseExpiration(reader["expiry_date"]);
                            }
                        }
                        else
                        {
                            SetDefaultLicenseValues();
                            licenseWarning.Visible = true;
                            lblLicenseWarning.Text = "لا توجد بيانات ترخيص مسجلة لهذه السيارة.";
                        }
                    }
                }
            }
            catch
            {
                SetDefaultLicenseValues();
            }
        }

        private void SetDefaultLicenseValues()
        {
            txtCurrentLicenseNumber.Text = "غير محدد";
            txtLicenseStartView.Text = "غير محدد";
            txtLicenseEndView.Text = "غير محدد";
            txtTrafficUnitView.Text = "غير محدد";
            txtLicenseTypeView.Text = "غير محدد";
        }

        private void CheckLicenseExpiration(object expiryDateObj)
        {
            if (expiryDateObj != DBNull.Value && expiryDateObj != null)
            {
                DateTime expiryDate = Convert.ToDateTime(expiryDateObj);
                int daysRemaining = (expiryDate - DateTime.Now).Days;

                if (daysRemaining < 0)
                {
                    licenseWarning.Visible = true;
                    lblLicenseWarning.Text = "انتهت صلاحية الترخيص! يرجى تجديد الترخيص فوراً.";
                }
                else if (daysRemaining <= 30)
                {
                    licenseWarning.Visible = true;
                    lblLicenseWarning.Text = $"تنبيه: ينتهي الترخيص بعد {daysRemaining} يوم (تاريخ الانتهاء: {expiryDate:dd/MM/yyyy}).";
                }
                else if (daysRemaining <= 60)
                {
                    licenseWarning.Visible = true;
                    lblLicenseWarning.Text = $"ملاحظة: ينتهي الترخيص بعد {daysRemaining} يوم (تاريخ الانتهاء: {expiryDate:dd/MM/yyyy}).";
                }
                else
                {
                    licenseWarning.Visible = false;
                }
            }
        }

        private void LoadMandatoryInsuranceForCurrentLicense(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT TOP 1 ci.company_name, ci.cost, ci.notes, ci.start_date, ci.end_date
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
                            string companyName = reader["company_name"]?.ToString() ?? "-";
                            decimal cost = reader["cost"] != DBNull.Value ? Convert.ToDecimal(reader["cost"]) : 0;
                            string insuranceNotes = reader["notes"]?.ToString() ?? "-";
                            DateTime startDate = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]) : DateTime.MinValue;
                            DateTime endDate = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]) : DateTime.MinValue;

                            string insuranceHtml = "<div class='alert alert-info mt-3' style='background-color: #e8f4f8; border-right: 4px solid #17a2b8; border-radius: 8px; padding: 15px;'>" +
                                "<strong><i class='fas fa-shield-alt'></i> التأمين الإجباري المرتبط بالترخيص:</strong><br/>" +
                                "<div class='row mt-2'>" +
                                "<div class='col-md-4'><strong>شركة التأمين:</strong> " + System.Web.HttpUtility.HtmlEncode(companyName) + "</div>" +
                                "<div class='col-md-4'><strong>التكلفة:</strong> " + cost.ToString("N2") + " ج.م</div>" +
                                "<div class='col-md-4'><strong>المدة:</strong> " + startDate.ToString("yyyy-MM-dd") + " إلى " + endDate.ToString("yyyy-MM-dd") + "</div>" +
                                (string.IsNullOrEmpty(insuranceNotes) ? "" : "<div class='col-md-12 mt-2'><strong>ملاحظات:</strong> " + System.Web.HttpUtility.HtmlEncode(insuranceNotes) + "</div>") +
                                "</div></div>";

                            string script = "$('#mandatoryInsuranceContainer').html('" + insuranceHtml.Replace("'", "\\'") + "').show();";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showMandatoryInsurance", script, true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "hideMandatoryInsurance", "$('#mandatoryInsuranceContainer').hide();", true);
                        }
                    }
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hideMandatoryInsurance", "$('#mandatoryInsuranceContainer').hide();", true);
            }
        }

        private void LoadLicenseHistoryData(SqlConnection conn)
        {
            try
            {
                string query = @"
            SELECT 
                vl.license_id, 
                vl.license_number, 
                lt.type AS license_type,
                CONVERT(DATE, vl.issue_date) AS issue_date,
                CONVERT(DATE, vl.expiry_date) AS expiry_date,
                tu.name AS traffic_unit,
                vl.renewal_cost,
                vl.registration_number
            FROM vehicle_licenses vl
            LEFT JOIN v_license_type lt ON vl.license_type_id = lt.id_LT
            LEFT JOIN traffic_unit tu ON vl.traffic_unit_id = tu.id_TF
            WHERE vl.vehicle_id = @VehicleId 
              AND (vl.deleted = 0 OR vl.deleted IS NULL)
            ORDER BY vl.issue_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvLicense.DataSource = dt;
                    gvLicense.DataBind();
                    int count = dt.Rows.Count;
                    string script = "document.getElementById('licenseCount').textContent = '" + count + "';";
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateLicenseCount", script, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading license history: {ex.Message}");
                gvLicense.DataSource = null;
                gvLicense.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "updateLicenseCount", "document.getElementById('licenseCount').textContent = '0';", true);
            }
        }

        #endregion

        #region Data Loading Methods (Insurance, Maintenance)

        private void LoadInsuranceData(SqlConnection conn)
        {
            try
            {
                string query = @"SELECT ci.insurance_id, ci.company_name, ci.insurance_name, ci.start_date, ci.end_date, ci.cost, ci.notes 
                                FROM car_insurance ci 
                                INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id 
                                WHERE vi.vehicle_id = @VehicleId AND (ci.deleted IS NULL OR ci.deleted = 0) 
                                ORDER BY ci.end_date DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvInsurance.DataSource = dt;
                    gvInsurance.DataBind();
                    int count = dt.Rows.Count;
                    string script = "document.getElementById('insuranceCount').textContent = '" + count + "';";
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateInsuranceCount", script, true);
                }
            }
            catch
            {
                gvInsurance.DataSource = null;
                gvInsurance.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "updateInsuranceCount", "document.getElementById('insuranceCount').textContent = '0';", true);
            }
        }

        private void LoadMaintenanceData(SqlConnection conn)
        {
            try
            {
                DataTable dt = new DataTable();

                string query = @"
            SELECT 
                vm.MaintenanceID,
                CONVERT(DATE, vm.MaintenanceDate) as MaintenanceDate,
                vm.MaintenanceType,
                ISNULL(mh.OdometerReading, 0) as OdometerReading,
                ISNULL(vm.TotalCost, 0) as TotalCost,
                mh.HistoryID
            FROM VehicleMaintenance vm
            INNER JOIN vehicle_maintenance vmc ON vm.MaintenanceID = vmc.maintenance_id
            LEFT JOIN MaintenanceHistory mh ON vm.history = mh.HistoryID
            WHERE vmc.vehicle_id = @VehicleId
            ORDER BY vm.MaintenanceDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                dt.Columns.Add("PartsNames", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    int historyId = row["HistoryID"] != DBNull.Value ? Convert.ToInt32(row["HistoryID"]) : 0;
                    if (historyId > 0)
                    {
                        string partsNames = GetMaintenanceParts(historyId);
                        row["PartsNames"] = partsNames;
                    }
                    else
                    {
                        row["PartsNames"] = "لا توجد قطع غيار";
                    }
                }

                if (dt.Columns.Contains("HistoryID"))
                {
                    dt.Columns.Remove("HistoryID");
                }

                gvMaintenance.DataSource = dt;
                gvMaintenance.DataBind();
                int count = dt.Rows.Count;
                string script = "document.getElementById('maintenanceCount').textContent = '" + count + "';";
                ScriptManager.RegisterStartupScript(this, GetType(), "updateMaintenanceCount", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading maintenance: {ex.Message}");
                gvMaintenance.DataSource = null;
                gvMaintenance.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "updateMaintenanceCount", "document.getElementById('maintenanceCount').textContent = '0';", true);
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
            catch
            {
                return "لا توجد قطع غيار";
            }
        }

        #endregion

        #region View Detail Methods

        protected void ViewInsurance_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int insuranceId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                ci.insurance_id,
                ci.company_name, 
                ci.insurance_name, 
                CONVERT(DATE, ci.start_date) as start_date, 
                CONVERT(DATE, ci.end_date) as end_date, 
                ci.cost, 
                ci.notes,
                ci.reg_date,
                CASE 
                    WHEN GETDATE() BETWEEN ci.start_date AND ci.end_date THEN N'ساري'
                    WHEN GETDATE() > ci.end_date THEN N'منتهي'
                    ELSE N'لم يبدأ'
                END as status,
                DATEDIFF(DAY, GETDATE(), ci.end_date) as days_remaining
            FROM car_insurance ci 
            WHERE ci.insurance_id = @InsuranceId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InsuranceId", insuranceId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var insuranceData = new Dictionary<string, object>();

                            insuranceData["شركة التأمين"] = reader["company_name"]?.ToString() ?? "-";
                            insuranceData["نوع التأمين"] = reader["insurance_name"]?.ToString() ?? "تأمين شامل";
                            insuranceData["تاريخ البداية"] = reader["start_date"] != DBNull.Value ?
                                Convert.ToDateTime(reader["start_date"]).ToString("yyyy-MM-dd") : "-";
                            insuranceData["تاريخ النهاية"] = reader["end_date"] != DBNull.Value ?
                                Convert.ToDateTime(reader["end_date"]).ToString("yyyy-MM-dd") : "-";
                            insuranceData["التكلفة"] = reader["cost"] != DBNull.Value ?
                                string.Format("{0:N2}", reader["cost"]) + " ج.م" : "0 ج.م";
                            insuranceData["الحالة"] = reader["status"]?.ToString() ?? "-";
                            insuranceData["ملاحظات"] = reader["notes"]?.ToString() ?? "لا توجد ملاحظات";

                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string json = serializer.Serialize(insuranceData);
                            string script = "showDetailsModal('تفاصيل التأمين', " + json + ");";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
                        }
                    }
                }
            }
        }

        protected void ViewMaintenanceDetails_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int maintenanceId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                vm.MaintenanceID,
                CONVERT(DATE, vm.MaintenanceDate) as MaintenanceDate,
                vm.MaintenanceType,
                ISNULL(mh.OdometerReading, 0) as OdometerReading,
                ISNULL(vm.TotalCost, 0) as TotalCost,
                ISNULL(mh.Notes, 'لا توجد ملاحظات') as Notes,
                CONVERT(DATE, vm.CreatedDate) as CreatedDate,
                mh.HistoryID
            FROM VehicleMaintenance vm
            LEFT JOIN MaintenanceHistory mh ON vm.history = mh.HistoryID
            WHERE vm.MaintenanceID = @MaintenanceId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaintenanceId", maintenanceId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int historyId = reader["HistoryID"] != DBNull.Value ? Convert.ToInt32(reader["HistoryID"]) : 0;
                            string partsNames = "";

                            if (historyId > 0)
                            {
                                partsNames = GetMaintenanceParts(historyId);
                            }

                            var data = new Dictionary<string, object>();

                            data["تاريخ الصيانة"] = reader["MaintenanceDate"] != DBNull.Value ?
                                Convert.ToDateTime(reader["MaintenanceDate"]).ToString("yyyy-MM-dd") : "غير محدد";

                            data["نوع الصيانة"] = reader["MaintenanceType"]?.ToString() ?? "غير محدد";

                            data["قراءة العداد (كم)"] = reader["OdometerReading"] != DBNull.Value ?
                                string.Format("{0:N0}", reader["OdometerReading"]) : "0";

                            data["التكلفة الإجمالية (ج.م)"] = reader["TotalCost"] != DBNull.Value ?
                                string.Format("{0:N2}", reader["TotalCost"]) : "0";

                            data["قطع الغيار المستخدمة"] = string.IsNullOrEmpty(partsNames) ? "لا توجد قطع غيار" : partsNames;

                            data["ملاحظات"] = reader["Notes"]?.ToString() ?? "لا توجد ملاحظات";

                            data["تاريخ التسجيل"] = reader["CreatedDate"] != DBNull.Value ?
                                Convert.ToDateTime(reader["CreatedDate"]).ToString("yyyy-MM-dd") : "غير محدد";

                            int currentOdometer = GetLastWorkOrderKilometer(_currentCarNumberId);
                            int maintenanceOdometer = reader["OdometerReading"] != DBNull.Value ? Convert.ToInt32(reader["OdometerReading"]) : 0;
                            int kmSinceMaintenance = currentOdometer - maintenanceOdometer;

                            if (kmSinceMaintenance < 0) kmSinceMaintenance = 0;

                            data["المسافة منذ آخر صيانة (كم)"] = string.Format("{0:N0}", kmSinceMaintenance);

                            if (kmSinceMaintenance <= 5000)
                                data["حالة الصيانة"] = "🟢 ممتازة";
                            else if (kmSinceMaintenance <= 10000)
                                data["حالة الصيانة"] = "🟡 جيدة - تحتاج متابعة";
                            else if (kmSinceMaintenance <= 15000)
                                data["حالة الصيانة"] = "🟠 تحتاج صيانة قريباً";
                            else
                                data["حالة الصيانة"] = "🔴 تحتاج صيانة فورية";

                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string json = serializer.Serialize(data);
                            string script = "showDetailsModal('تفاصيل الصيانة', " + json + ");";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
                        }
                    }
                }
            }
        }

        protected void ViewLicenseDetails_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int licenseId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string licenseQuery = @"
                    SELECT 
                        vl.license_number, 
                        lt.type AS license_type,
                        CONVERT(DATE, vl.issue_date) as issue_date,
                        CONVERT(DATE, vl.expiry_date) as expiry_date,
                        tu.name AS traffic_unit, 
                        vl.renewal_cost, 
                        vl.registration_number,
                        vl.notes
                    FROM vehicle_licenses vl
                    LEFT JOIN v_license_type lt ON vl.license_type_id = lt.id_LT
                    LEFT JOIN traffic_unit tu ON vl.traffic_unit_id = tu.id_TF
                    WHERE vl.license_id = @LicenseId";

                string licenseNumber = "", licenseType = "", issueDate = "", expiryDate = "", trafficUnit = "", renewalCost = "0", registrationNumber = "", notes = "";

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
                            notes = reader["notes"]?.ToString() ?? "-";
                        }
                    }
                }

                string insuranceQuery = @"
                    SELECT TOP 1 
                        ci.company_name, 
                        ci.cost, 
                        ci.notes, 
                        CONVERT(DATE, ci.start_date) as start_date, 
                        CONVERT(DATE, ci.end_date) as end_date
                    FROM car_insurance ci
                    INNER JOIN vehicle_insurance vi ON ci.insurance_id = vi.insurance_id
                    WHERE vi.vehicle_id = @VehicleId 
                      AND ci.insurance_name = N'تأمين إجباري'
                      AND (ci.deleted = 0 OR ci.deleted IS NULL)
                    ORDER BY ci.reg_date DESC";

                var mandatoryInsurance = new { company = "", cost = "0", notes = "", start_date = "", end_date = "" };

                using (SqlCommand cmd = new SqlCommand(insuranceQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            mandatoryInsurance = new
                            {
                                company = reader["company_name"]?.ToString() ?? "-",
                                cost = reader["cost"] != DBNull.Value ? Convert.ToDecimal(reader["cost"]).ToString("N2") : "0",
                                notes = reader["notes"]?.ToString() ?? "-",
                                start_date = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]).ToString("yyyy-MM-dd") : "-",
                                end_date = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]).ToString("yyyy-MM-dd") : "-"
                            };
                        }
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
                    notes = notes,
                    mandatory_insurance_company = mandatoryInsurance.company,
                    mandatory_insurance_cost = mandatoryInsurance.cost,
                    mandatory_insurance_start_date = mandatoryInsurance.start_date,
                    mandatory_insurance_end_date = mandatoryInsurance.end_date
                };

                string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
                string script = "showLicenseDetailsWithInsurance(" + json + ");";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
            }
        }

        #endregion

        #region Page Index Changing Events

        protected void GvInsurance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInsurance.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadInsuranceData(conn);
            }
        }

        protected void GvMaintenance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMaintenance.PageIndex = e.NewPageIndex;
            if (_currentVehicleId > 0)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    LoadMaintenanceData(conn);
                }
            }
        }

        protected void GvLicense_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLicense.PageIndex = e.NewPageIndex;
            if (_currentVehicleId > 0)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    LoadLicenseHistoryData(conn);
                }
            }
        }

        #endregion

        #region Tab Methods

        protected void BtnTabEntry_Click(object sender, EventArgs e)
        {
            // DRIVER: the "new car" tab is not available
            if (AuthHelper.IsRestrictedUser) return;

            MainMultiView.ActiveViewIndex = 0;
            btnTabEntry.CssClass = "btn-tab active";
            btnTabSearch.CssClass = "btn-tab";
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
            resultBox.Visible = false;
        }

        protected void BtnTabSearch_Click(object sender, EventArgs e)
        {
            MainMultiView.ActiveViewIndex = 1;
            btnTabSearch.CssClass = "btn-tab active";
            btnTabEntry.CssClass = "btn-tab";
            lblMessage.Visible = false;
            lblSuccess.Visible = false;
            resultBox.Visible = false;
            txtSearchLetter1.Text = "";
            txtSearchLetter2.Text = "";
            txtSearchLetter3.Text = "";
            txtSearchNumbers.Text = "";
            ClearAllData();
        }

        #endregion

        #region Helper Methods

        private void ClearAllData()
        {
            txtType.Text = ""; txtModel.Text = ""; txtYear.Text = ""; txtStatus.Text = "";
            txtChassisView.Text = ""; txtMotorView.Text = ""; txtGasView.Text = "";
            txtPassengersView.Text = ""; txtLicenseTypeView.Text = ""; txtLicenseStartView.Text = "";
            txtLicenseEndView.Text = ""; txtTrafficUnitView.Text = ""; txtCurrentLicenseNumber.Text = "";
            txtNoteView.Text = ""; txtNaturalGasView.Text = ""; txtCarBody.Text = "";
            lblInitialKM.Text = "0";
            lblCurrentKM.Text = "0";
            gvInsurance.DataSource = null; gvInsurance.DataBind();
            gvMaintenance.DataSource = null; gvMaintenance.DataBind();
            gvLicense.DataSource = null; gvLicense.DataBind();
            licenseWarning.Visible = false;
            engineOverhaulRow.Visible = false;
            pnlAlerts.Visible = false;
            rptAlerts.DataSource = null;
            rptAlerts.DataBind();

            ScriptManager.RegisterStartupScript(this, GetType(), "resetCounts",
                "document.getElementById('insuranceCount').textContent = '0'; " +
                "document.getElementById('maintenanceCount').textContent = '0'; " +
                "document.getElementById('licenseCount').textContent = '0';", true);
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

        #endregion

        #region Maintenance Alerts

        private void CheckMaintenanceAlerts(SqlConnection conn, int vehicleId, int currentOdometer)
        {
            try
            {
                DateTime currentDate = DateTime.Now;

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
                        }

                        if (timeMonths > 0 && lastServiceDate.HasValue)
                        {
                            nextDueDate = lastServiceDate.Value.AddMonths(timeMonths);
                            remainingDays = (int)(nextDueDate.Value - currentDate).TotalDays;
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في فحص التنبيهات: {ex.Message}");
                pnlAlerts.Visible = true;
                lblNoAlerts.Visible = true;
                rptAlerts.Visible = false;
                lblNoAlerts.Text = "حدث خطأ في فحص التنبيهات";
            }
        }

        protected void RptAlerts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DataRowView rowView = (DataRowView)e.Item.DataItem;
            string alertLevel = rowView["AlertLevel"]?.ToString() ?? "success";

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

            if (lblMaintenanceType != null)
            {
                lblMaintenanceType.Visible = true;
                lblMaintenanceType.Text = rowView["PartType"]?.ToString() ?? "";
            }

            if (lblAlertMessageText != null && rowView["AlertMessage"] != null)
            {
                lblAlertMessageText.Text = rowView["AlertMessage"].ToString();
            }

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

            if (lblKMInterval != null && rowView["KMInterval"] is int kmInterval)
            {
                lblKMInterval.Text = kmInterval > 0 ? $"{kmInterval:N0} كم" : "غير محدد";
            }

            if (lblTimeInterval != null && rowView["TimeIntervalMonths"] is int timeInterval)
            {
                lblTimeInterval.Text = timeInterval > 0 ? $"{timeInterval} شهر" : "غير محدد";
            }

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

            if (lblNextDue != null)
            {
                string nextDueInfo = "";
                int kmIntervalVal2 = rowView["KMInterval"] as int? ?? 0;
                int nextDueKMVal = rowView["NextDueKM"] as int? ?? 0;

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

        protected string GetMaintenanceStatus(object nextDateObj, object nextOdometerObj)
        {
            if (nextDateObj != DBNull.Value && nextDateObj != null)
            {
                DateTime nextDate = Convert.ToDateTime(nextDateObj);
                int daysUntil = (nextDate - DateTime.Now).Days;

                if (daysUntil < 0)
                    return "status-expired";
                else if (daysUntil <= 7)
                    return "status-warning";
                else
                    return "status-active";
            }
            return "status-active";
        }

        protected string GetMaintenanceStatusText(object nextDateObj, object nextOdometerObj)
        {
            if (nextDateObj != DBNull.Value && nextDateObj != null)
            {
                DateTime nextDate = Convert.ToDateTime(nextDateObj);
                int daysUntil = (nextDate - DateTime.Now).Days;

                if (daysUntil < 0)
                    return "منتهية";
                else if (daysUntil <= 7)
                    return "قريبة";
                else
                    return "جيدة";
            }
            return "غير محدد";
        }

        #endregion
    }
}