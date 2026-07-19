using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class vehicle_management : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private int _currentVehicleId = 0;
        private int _currentCarNumberId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadYears();
                MainMultiView.ActiveViewIndex = 0;
                btnTabEntry.CssClass = "btn-tab active";
                btnTabSearch.CssClass = "btn-tab";
            }
        }

        private void LoadDropdowns()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string carTypesQuery = "SELECT car_type_id, CONCAT(car_brand, ' - ', car_type_name) AS CarType FROM car_types WHERE is_active = 1 ORDER BY car_brand";
                    SqlDataAdapter da = new SqlDataAdapter(carTypesQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCarType.DataSource = dt;
                    ddlCarType.DataTextField = "CarType";
                    ddlCarType.DataValueField = "car_type_id";
                    ddlCarType.DataBind();
                    ddlCarType.Items.Insert(0, new ListItem("-- اختر نوع السيارة --", ""));

                    string modelsQuery = "SELECT m_id, model FROM car_model WHERE is_active = 1 ORDER BY model";
                    da = new SqlDataAdapter(modelsQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);
                    ddlCarModel.DataSource = dt;
                    ddlCarModel.DataTextField = "model";
                    ddlCarModel.DataValueField = "m_id";
                    ddlCarModel.DataBind();
                    ddlCarModel.Items.Insert(0, new ListItem("-- اختر الموديل --", ""));

                    string statusQuery = "SELECT s_id, status FROM car_status WHERE is_active = 1 ORDER BY status";
                    da = new SqlDataAdapter(statusQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);
                    ddlStatus.DataSource = dt;
                    ddlStatus.DataTextField = "status";
                    ddlStatus.DataValueField = "s_id";
                    ddlStatus.DataBind();
                    ddlStatus.Items.Insert(0, new ListItem("-- اختر الحالة --", ""));

                    string fuelQuery = "SELECT fuel_id, type FROM fuel_type WHERE is_active = 1 ORDER BY type";
                    da = new SqlDataAdapter(fuelQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);
                    ddlFuelType.DataSource = dt;
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

        private void LoadYears()
        {
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 20; year <= currentYear + 1; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
            ddlYear.Items.Insert(0, new ListItem("-- اختر السنة --", ""));
        }

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
            if (ddlCarType.SelectedValue == "") { ShowMessage("الرجاء اختيار نوع السيارة"); return false; }
            if (ddlCarModel.SelectedValue == "") { ShowMessage("الرجاء اختيار الموديل"); return false; }
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

        protected void BtnSave_Click(object sender, EventArgs e)
        {
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
                cmd.Parameters.AddWithValue("@VType", ddlCarType.SelectedValue);
                cmd.Parameters.AddWithValue("@VModel", ddlCarModel.SelectedValue);
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
            ddlYear.SelectedValue = ""; ddlStatus.SelectedValue = ""; ddlFuelType.SelectedValue = "";
            ddlNaturalGas.SelectedValue = "";
            txtPassengers.Text = ""; txtChassis.Text = ""; txtMotor.Text = "";
            txtInitialKM.Text = "";
            lblMessage.Visible = false; lblSuccess.Visible = false;
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
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
                    upInsurance.Update(); upMaintenance.Update(); upSubscription.Update(); upRoutes.Update(); upLicense.Update();
                }
                catch (Exception ex) { ShowMessage("خطأ: " + ex.Message); }
            }
        }

        private decimal GetLastOdometerFromWorkOrders(int carNumberId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // First check if any work orders exist
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

                    // Get the last odometer reading
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
                                lblEngineOverhaulCost.Text = overhaulCost.ToString("N2"); // Display cost
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

        private void LoadVehicleData(SqlConnection conn)
        {
            string query = @"SELECT mvd.v_type, mvd.v_model, mvd.v_year, mvd.v_statues, mvd.chassis, 
                            mvd.motor_num, mvd.passenger, mvd.gas_type, mvd.note, mvd.has_natural_gas,
                            mvd.initial_km,
                            ct.car_type_name, ct.car_brand, cm.model AS model_name, 
                            cs.status AS status_name, ft.type AS fuel_type_name
                    FROM main_vehicle_details mvd
                    LEFT JOIN car_types ct ON mvd.v_type = ct.car_type_id
                    LEFT JOIN car_model cm ON mvd.v_model = cm.m_id
                    LEFT JOIN car_status cs ON mvd.v_statues = cs.s_id
                    LEFT JOIN fuel_type ft ON mvd.gas_type = ft.fuel_id
                    WHERE mvd.v_id = @VehicleId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string carBrand = reader["car_brand"]?.ToString() ?? "";
                        string carType = reader["car_type_name"]?.ToString() ?? "";
                        txtType.Text = !string.IsNullOrEmpty(carBrand) ? carBrand + " - " + carType : carType;
                        txtModel.Text = reader["model_name"]?.ToString() ?? "";
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

                        decimal lastOdometer = GetLastOdometerFromWorkOrders(_currentCarNumberId);
                        lblCurrentKM.Text = lastOdometer.ToString("N0");
                    }
                }
            }

            LoadCurrentLicense(conn);
            LoadMandatoryInsuranceForCurrentLicense(conn);
            LoadEngineOverhaulInfo(conn, _currentVehicleId);

            string displayNumber = txtSearchLetter1.Text.Trim();
            if (!string.IsNullOrEmpty(txtSearchLetter2.Text)) displayNumber += " " + txtSearchLetter2.Text.Trim();
            if (!string.IsNullOrEmpty(txtSearchLetter3.Text)) displayNumber += " " + txtSearchLetter3.Text.Trim();
            displayNumber += " " + txtSearchNumbers.Text.Trim();
            lblCarNumber.Text = displayNumber;

            LoadInsuranceData(conn);
            LoadMaintenanceData(conn);
            LoadSubscriptionData(conn);
            LoadRoutesData(conn);
            LoadLicenseHistoryData(conn);
        }


        private bool HasWorkOrders(int carNumberId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT COUNT(1) FROM operation_command 
                            WHERE v_number = @CarNumberId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarNumberId", carNumberId);
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
                            string fallbackQuery = @"
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
                                ORDER BY vl.issue_date DESC";

                            using (SqlCommand fallbackCmd = new SqlCommand(fallbackQuery, conn))
                            {
                                fallbackCmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                                using (SqlDataReader fallbackReader = fallbackCmd.ExecuteReader())
                                {
                                    if (fallbackReader.Read())
                                    {
                                        txtCurrentLicenseNumber.Text = fallbackReader["license_number"]?.ToString() ?? "غير محدد";
                                        txtLicenseStartView.Text = fallbackReader["issue_date"] != DBNull.Value ?
                                            Convert.ToDateTime(fallbackReader["issue_date"]).ToString("dd/MM/yyyy") : "غير محدد";
                                        txtLicenseEndView.Text = fallbackReader["expiry_date"] != DBNull.Value ?
                                            Convert.ToDateTime(fallbackReader["expiry_date"]).ToString("dd/MM/yyyy") : "غير محدد";
                                        txtTrafficUnitView.Text = fallbackReader["traffic_unit"]?.ToString() ?? "غير محدد";
                                        txtLicenseTypeView.Text = fallbackReader["license_type"]?.ToString() ?? "غير محدد";

                                        if (fallbackReader["expiry_date"] != DBNull.Value)
                                        {
                                            CheckLicenseExpiration(fallbackReader["expiry_date"]);
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

                // Query to get maintenance data with parts
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

                // Add PartsNames column
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

                // Remove HistoryID column (not needed for display)
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
        protected void ViewMaintenanceDetails_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int maintenanceId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Get complete maintenance details
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

                            // Basic information
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

                            // Calculate kilometers since last maintenance using current odometer
                            int currentOdometer = GetLastWorkOrderKilometer(_currentCarNumberId);
                            int maintenanceOdometer = reader["OdometerReading"] != DBNull.Value ? Convert.ToInt32(reader["OdometerReading"]) : 0;
                            int kmSinceMaintenance = currentOdometer - maintenanceOdometer;

                            if (kmSinceMaintenance < 0) kmSinceMaintenance = 0;

                            data["المسافة منذ آخر صيانة (كم)"] = string.Format("{0:N0}", kmSinceMaintenance);

                            // Status based on kilometers since maintenance
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
                        else
                        {
                            string script = "alert('❌ لم يتم العثور على تفاصيل الصيانة');";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showError", script, true);
                        }
                    }
                }
            }
        }

        private void LoadSubscriptionData(SqlConnection conn)
        {
            try
            {
                DataTable dt = new DataTable();

                // Try direct query from subscription_details without vehicle_subscription join first
                string query = @"
            SELECT 
                sd.n_id,
                sd.name,
                sd.phone,
                sd.department,
                sd.position,
                sd.start_date,
                sd.end_date,
                sd.cost,
                ISNULL(sd.sub_count, 1) as sub_count
            FROM subscription_details sd 
            WHERE sd.n_id IN (SELECT subscription_id FROM vehicle_subscription WHERE vehicle_id = @VehicleId)
              AND (sd.hide IS NULL OR sd.hide = 0)
            ORDER BY sd.start_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                // If no data, try alternative with vehicle_subscriptions table
                if (dt.Rows.Count == 0)
                {
                    string altQuery = @"
                SELECT 
                    sd.n_id,
                    sd.name,
                    sd.phone,
                    sd.department,
                    sd.position,
                    sd.start_date,
                    sd.end_date,
                    sd.cost,
                    ISNULL(sd.sub_count, 1) as sub_count
                FROM subscription_details sd 
                WHERE sd.n_id IN (SELECT subscriber_id FROM vehicle_subscriptions WHERE vehicle_id = @VehicleId)
                ORDER BY sd.start_date DESC";

                    using (SqlCommand cmd = new SqlCommand(altQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }
                    }
                }

                // If still no data, try without any relationship (for testing)
                if (dt.Rows.Count == 0)
                {
                    // Show message that no subscriptions found
                    dt = new DataTable();
                }

                gvSubscription.DataSource = dt;
                gvSubscription.DataBind();
                int count = dt.Rows.Count;
                string script = "document.getElementById('subscriptionCount').textContent = '" + count + "';";
                ScriptManager.RegisterStartupScript(this, GetType(), "updateSubscriptionCount", script, true);

                if (count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"No subscriptions found for vehicle ID: {_currentVehicleId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subscriptions: {ex.Message}");
                gvSubscription.DataSource = null;
                gvSubscription.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "updateSubscriptionCount", "document.getElementById('subscriptionCount').textContent = '0';", true);
            }
        }

        private void LoadRoutesData(SqlConnection conn)
        {
            try
            {
                DataTable dt = new DataTable();

                // Try direct query from routes through subscription_details
                string query = @"
            SELECT DISTINCT 
                r.route_id, 
                r.route_name, 
                r.description, 
                r.start_loc, 
                r.end_loc, 
                r.KM, 
                r.direction, 
                ISNULL(r.subscription_count, 0) as subscription_count, 
                r.is_active,
                r.start_date,
                r.end_date,
                r.driver_name,
                r.driver_phone
            FROM routes r 
            INNER JOIN subscription_details sd ON r.route_id = sd.route_id 
            WHERE sd.n_id IN (SELECT subscription_id FROM vehicle_subscription WHERE vehicle_id = @VehicleId)
            ORDER BY r.route_name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                // If no data, try alternative with vehicle_subscriptions table
                if (dt.Rows.Count == 0)
                {
                    string altQuery = @"
                SELECT DISTINCT 
                    r.route_id, 
                    r.route_name, 
                    r.description, 
                    r.start_loc, 
                    r.end_loc, 
                    r.KM, 
                    r.direction, 
                    ISNULL(r.subscription_count, 0) as subscription_count, 
                    r.is_active,
                    r.start_date,
                    r.end_date,
                    r.driver_name,
                    r.driver_phone
                FROM routes r 
                INNER JOIN subscription_details sd ON r.route_id = sd.route_id 
                WHERE sd.n_id IN (SELECT subscriber_id FROM vehicle_subscriptions WHERE vehicle_id = @VehicleId)
                ORDER BY r.route_name";

                    using (SqlCommand cmd = new SqlCommand(altQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@VehicleId", _currentVehicleId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }
                    }
                }

                gvRoutes.DataSource = dt;
                gvRoutes.DataBind();
                int count = dt.Rows.Count;
                string script = "document.getElementById('routesCount').textContent = '" + count + "';";
                ScriptManager.RegisterStartupScript(this, GetType(), "updateRoutesCount", script, true);

                if (count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"No routes found for vehicle ID: {_currentVehicleId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading routes: {ex.Message}");
                gvRoutes.DataSource = null;
                gvRoutes.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "updateRoutesCount", "document.getElementById('routesCount').textContent = '0';", true);
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

        protected void ViewInsurance_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int insuranceId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Get complete insurance details
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
                            // Create the data object
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

                            // Add days remaining warning if applicable
                            string status = reader["status"]?.ToString() ?? "";
                            int daysRemaining = reader["days_remaining"] != DBNull.Value ? Convert.ToInt32(reader["days_remaining"]) : 0;

                            if (status == "ساري" && daysRemaining <= 30 && daysRemaining > 0)
                            {
                                insuranceData["تنبيه"] = $"⚠️ ينتهي هذا التأمين بعد {daysRemaining} يوم";
                            }
                            else if (status == "منتهي")
                            {
                                insuranceData["تنبيه"] = $"❌ هذا التأمين منتهي الصلاحية منذ {Math.Abs(daysRemaining)} يوم";
                            }

                            // Serialize and show modal
                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string json = serializer.Serialize(insuranceData);
                            string script = "showDetailsModal('تفاصيل التأمين', " + json + ");";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
                        }
                        else
                        {
                            string script = "alert('❌ لم يتم العثور على تفاصيل التأمين');";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showError", script, true);
                        }
                    }
                }
            }
        }

        protected void ViewMaintenance_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int maintenanceId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Get complete maintenance details
                string query = @"
            SELECT 
                vm.MaintenanceID,
                CONVERT(DATE, vm.MaintenanceDate) as MaintenanceDate,
                vm.MaintenanceType,
                ISNULL(vm.OdometerReading, 0) as OdometerReading,
                ISNULL(vm.TotalCost, 0) as TotalCost,
                ISNULL(vm.Notes, 'لا توجد ملاحظات') as Notes,
                CONVERT(DATE, vm.MaintenanceEntryDate) as MaintenanceEntryDate,
                CONVERT(DATE, vm.MaintenanceExitDate) as MaintenanceExitDate,
                ISNULL(vm.NextMaintenanceKM, 0) as NextMaintenanceKM,
                ISNULL(vm.CreatedBy, 'غير محدد') as CreatedBy,
                vm.service_type,
                vm.maintenance_part,
                vm.maintenance_conf,
                vm.history,
                CONVERT(DATE, vm.CreatedDate) as CreatedDate
            FROM VehicleMaintenance vm
            WHERE vm.MaintenanceID = @MaintenanceId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaintenanceId", maintenanceId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = new Dictionary<string, object>();

                            // Basic information
                            data["تاريخ الصيانة"] = reader["MaintenanceDate"] != DBNull.Value ?
                                Convert.ToDateTime(reader["MaintenanceDate"]).ToString("yyyy-MM-dd") : "غير محدد";

                            data["نوع الصيانة"] = reader["MaintenanceType"]?.ToString() ?? "غير محدد";

                            data["قراءة العداد (كم)"] = reader["OdometerReading"] != DBNull.Value ?
                                string.Format("{0:N0}", reader["OdometerReading"]) : "0";

                            data["التكلفة الإجمالية (ج.م)"] = reader["TotalCost"] != DBNull.Value ?
                                string.Format("{0:N2}", reader["TotalCost"]) : "0";

                            // Entry and exit dates
                            if (reader["MaintenanceEntryDate"] != DBNull.Value)
                            {
                                data["تاريخ دخول الصيانة"] = Convert.ToDateTime(reader["MaintenanceEntryDate"]).ToString("yyyy-MM-dd");
                            }

                            if (reader["MaintenanceExitDate"] != DBNull.Value)
                            {
                                data["تاريخ خروج الصيانة"] = Convert.ToDateTime(reader["MaintenanceExitDate"]).ToString("yyyy-MM-dd");

                                // Calculate duration if both dates exist
                                if (reader["MaintenanceEntryDate"] != DBNull.Value)
                                {
                                    DateTime entryDate = Convert.ToDateTime(reader["MaintenanceEntryDate"]);
                                    DateTime exitDate = Convert.ToDateTime(reader["MaintenanceExitDate"]);
                                    int daysInMaintenance = (exitDate - entryDate).Days;
                                    data["مدة الصيانة (أيام)"] = daysInMaintenance.ToString();
                                }
                            }

                            // Next maintenance information
                            data["قراءة العداد للصيانة القادمة (كم)"] = reader["NextMaintenanceKM"] != DBNull.Value ?
                                string.Format("{0:N0}", reader["NextMaintenanceKM"]) : "غير محدد";

                            data["تم الإنشاء بواسطة"] = reader["CreatedBy"]?.ToString() ?? "غير محدد";

                            data["تاريخ التسجيل"] = reader["CreatedDate"] != DBNull.Value ?
                                Convert.ToDateTime(reader["CreatedDate"]).ToString("yyyy-MM-dd") : "غير محدد";

                            data["ملاحظات"] = reader["Notes"]?.ToString() ?? "لا توجد ملاحظات";

                            // Calculate kilometers since last maintenance using current odometer
                            int currentOdometer = GetLastWorkOrderKilometer(_currentCarNumberId);
                            int maintenanceOdometer = reader["OdometerReading"] != DBNull.Value ? Convert.ToInt32(reader["OdometerReading"]) : 0;
                            int kmSinceMaintenance = currentOdometer - maintenanceOdometer;

                            if (kmSinceMaintenance < 0) kmSinceMaintenance = 0;

                            data["المسافة منذ آخر صيانة (كم)"] = string.Format("{0:N0}", kmSinceMaintenance);

                            // Status based on kilometers since maintenance
                            if (kmSinceMaintenance <= 5000)
                                data["حالة الصيانة"] = "🟢 ممتازة";
                            else if (kmSinceMaintenance <= 10000)
                                data["حالة الصيانة"] = "🟡 جيدة - تحتاج متابعة";
                            else if (kmSinceMaintenance <= 15000)
                                data["حالة الصيانة"] = "🟠 تحتاج صيانة قريباً";
                            else
                                data["حالة الصيانة"] = "🔴 تحتاج صيانة فورية";

                            // Check if next maintenance is approaching
                            int nextMaintenanceKM = reader["NextMaintenanceKM"] != DBNull.Value ? Convert.ToInt32(reader["NextMaintenanceKM"]) : 0;
                            if (nextMaintenanceKM > 0)
                            {
                                int kmUntilNextMaintenance = nextMaintenanceKM - currentOdometer;
                                if (kmUntilNextMaintenance <= 0)
                                    data["تنبيه الصيانة القادمة"] = "⚠️ تجاوزت قراءة العداد موعد الصيانة المحدد!";
                                else if (kmUntilNextMaintenance <= 1000)
                                    data["تنبيه الصيانة القادمة"] = $"⚠️ متبقي {kmUntilNextMaintenance:N0} كم فقط حتى موعد الصيانة التالية!";
                                else if (kmUntilNextMaintenance <= 3000)
                                    data["تنبيه الصيانة القادمة"] = $"⚡ متبقي {kmUntilNextMaintenance:N0} كم حتى الصيانة التالية";
                            }

                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string json = serializer.Serialize(data);
                            string script = "showDetailsModal('تفاصيل الصيانة', " + json + ");";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
                        }
                        else
                        {
                            string script = "alert('❌ لم يتم العثور على تفاصيل الصيانة');";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showError", script, true);
                        }
                    }
                }
            }
        }
        protected void ViewSubscription_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int subscriptionId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                DataTable dt = new DataTable();

                // Try to get from subscription_details table first
                string query = @"
            SELECT 
                sd.name, 
                sd.phone, 
                sd.department, 
                sd.position, 
                CONVERT(DATE, sd.start_date) as start_date, 
                CONVERT(DATE, sd.end_date) as end_date, 
                ISNULL(sd.cost, 0) as cost,
                ISNULL(sd.sub_count, 1) as sub_count,
                ISNULL(r.route_name, 'غير محدد') as route_name,
                CASE 
                    WHEN GETDATE() BETWEEN sd.start_date AND sd.end_date THEN 'نشط'
                    WHEN GETDATE() > sd.end_date THEN 'منتهي'
                    ELSE 'لم يبدأ'
                END as status
            FROM subscription_details sd
            LEFT JOIN routes r ON sd.route_id = r.route_id
            WHERE sd.n_id = @SubscriptionId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SubscriptionId", subscriptionId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                // If no data found, try vehicle_subscriptions table
                if (dt.Rows.Count == 0)
                {
                    string altQuery = @"
                SELECT 
                    sd.name, 
                    sd.phone, 
                    sd.department, 
                    sd.position, 
                    CONVERT(DATE, vs.start_date) as start_date, 
                    CONVERT(DATE, vs.end_date) as end_date, 
                    ISNULL(vs.cost, 0) as cost,
                    ISNULL(sd.sub_count, 1) as sub_count,
                    ISNULL(r.route_name, 'غير محدد') as route_name,
                    CASE 
                        WHEN GETDATE() BETWEEN vs.start_date AND vs.end_date THEN 'نشط'
                        WHEN GETDATE() > vs.end_date THEN 'منتهي'
                        ELSE 'لم يبدأ'
                    END as status
                FROM vehicle_subscriptions vs
                INNER JOIN subscription_details sd ON vs.subscriber_id = sd.n_id
                LEFT JOIN routes r ON sd.route_id = r.route_id
                WHERE vs.subscription_id = @SubscriptionId";

                    using (SqlCommand cmd = new SqlCommand(altQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubscriptionId", subscriptionId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    DataRow reader = dt.Rows[0];
                    var data = new Dictionary<string, object>
                    {
                        ["اسم المشترك"] = reader["name"]?.ToString() ?? "-",
                        ["رقم الهاتف"] = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "-",
                        ["القسم"] = reader["department"]?.ToString() ?? "-",
                        ["الوظيفة"] = reader["position"]?.ToString() ?? "-",
                        ["المسار"] = reader["route_name"]?.ToString() ?? "غير محدد",
                        ["تاريخ البداية"] = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]).ToString("yyyy-MM-dd") : "-",
                        ["تاريخ النهاية"] = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]).ToString("yyyy-MM-dd") : "-",
                        ["التكلفة (ج.م)"] = reader["cost"] != DBNull.Value ? string.Format("{0:N2}", reader["cost"]) : "0",
                        ["عدد المشتركين"] = reader["sub_count"]?.ToString() ?? "1",
                        ["الحالة"] = reader["status"]?.ToString() ?? "-"
                    };

                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string json = serializer.Serialize(data);
                    string script = "showDetailsModal('تفاصيل المشترك', " + json + ");";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
                }
                else
                {
                    string script = "alert('❌ لم يتم العثور على بيانات المشترك');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showError", script, true);
                }
            }
        }

        protected void ViewRoute_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int routeId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT 
                            route_name, 
                            ISNULL(description, '-') as description, 
                            ISNULL(start_loc, '-') as start_loc, 
                            ISNULL(end_loc, '-') as end_loc, 
                            ISNULL(KM, 0) as KM, 
                            ISNULL(direction, '-') as direction, 
                            ISNULL(subscription_count, 0) as subscription_count,
                            ISNULL(CONVERT(DATE, start_date), GETDATE()) as start_date,
                            ISNULL(CONVERT(DATE, end_date), '2099-12-31') as end_date,
                            ISNULL(driver_name, '-') as driver_name,
                            ISNULL(driver_phone, '-') as driver_phone,
                            CASE WHEN is_active = 1 THEN 'نشط' ELSE 'غير نشط' END as status
                         FROM routes 
                         WHERE route_id = @RouteId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RouteId", routeId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = new Dictionary<string, object>
                            {
                                ["اسم المسار"] = reader["route_name"]?.ToString() ?? "-",
                                ["الوصف"] = reader["description"]?.ToString() ?? "-",
                                ["نقطة البداية"] = reader["start_loc"]?.ToString() ?? "-",
                                ["نقطة النهاية"] = reader["end_loc"]?.ToString() ?? "-",
                                ["المسافة (كم)"] = reader["KM"] != DBNull.Value ? string.Format("{0:N0}", reader["KM"]) : "0",
                                ["الاتجاه"] = reader["direction"]?.ToString() ?? "-",
                                ["عدد المشتركين الحالي"] = reader["subscription_count"]?.ToString() ?? "0",
                                ["تاريخ البدء"] = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]).ToString("yyyy-MM-dd") : "-",
                                ["تاريخ الانتهاء"] = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]).ToString("yyyy-MM-dd") : "-",
                                ["اسم السائق"] = reader["driver_name"]?.ToString() ?? "-",
                                ["هاتف السائق"] = reader["driver_phone"]?.ToString() ?? "-",
                                ["الحالة"] = reader["status"]?.ToString() ?? "-"
                            };

                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string json = serializer.Serialize(data);
                            string script = "showDetailsModal('تفاصيل المسار', " + json + ");";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
                        }
                        else
                        {
                            string script = "alert('❌ لم يتم العثور على تفاصيل المسار');";
                            ScriptManager.RegisterStartupScript(this, GetType(), "showError", script, true);
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

                // Get license details
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

                // Get mandatory insurance for this license
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
                    mandatory_insurance = mandatoryInsurance
                };

                string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
                string script = "showLicenseDetailsWithInsurance(" + json + ");";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", script, true);
            }
        }

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

        protected void GvSubscription_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscription.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadSubscriptionData(conn);
            }
        }

        protected void GvRoutes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRoutes.PageIndex = e.NewPageIndex;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                LoadRoutesData(conn);
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

        protected void BtnTabEntry_Click(object sender, EventArgs e)
        {
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

        private void ClearAllData()
        {
            txtType.Text = ""; txtModel.Text = ""; txtYear.Text = ""; txtStatus.Text = "";
            txtChassisView.Text = ""; txtMotorView.Text = ""; txtGasView.Text = "";
            txtPassengersView.Text = ""; txtLicenseTypeView.Text = ""; txtLicenseStartView.Text = "";
            txtLicenseEndView.Text = ""; txtTrafficUnitView.Text = ""; txtCurrentLicenseNumber.Text = "";
            txtNoteView.Text = ""; txtNaturalGasView.Text = "";
            lblInitialKM.Text = "0";
            lblCurrentKM.Text = "0";
            gvInsurance.DataSource = null; gvInsurance.DataBind();
            gvMaintenance.DataSource = null; gvMaintenance.DataBind();
            gvSubscription.DataSource = null; gvSubscription.DataBind();
            gvRoutes.DataSource = null; gvRoutes.DataBind();
            gvLicense.DataSource = null; gvLicense.DataBind();
            licenseWarning.Visible = false;
            engineOverhaulRow.Visible = false;

            ScriptManager.RegisterStartupScript(this, GetType(), "resetCounts",
                "document.getElementById('insuranceCount').textContent = '0'; " +
                "document.getElementById('maintenanceCount').textContent = '0'; " +
                "document.getElementById('subscriptionCount').textContent = '0'; " +
                "document.getElementById('routesCount').textContent = '0'; " +
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

        // Helper method to get maintenance status CSS class
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

        // Helper method to get maintenance status text
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
    }
}










