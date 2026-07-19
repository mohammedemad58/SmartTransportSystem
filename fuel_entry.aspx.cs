using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class fuel_entry : System.Web.UI.Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private bool _hasNaturalGas = false;
        private const decimal GasEnergyEquivalent = 1.25m;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadVehicles();
                LoadFuelTypes();
                LoadCardNumbers();
                LoadTransactionCards();
                LoadVehiclesForTransaction();
                LoadTransactionsGrid();
                SetDefaultDates();

                // If a vehicle is already selected, refresh data
                if (ddlVehicle.SelectedValue != "0")
                {
                    int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                    bool hasNaturalGas = GetVehicleNaturalGasStatus(vehicleId);
                    UpdateUIBasedOnFuelType(hasNaturalGas);
                    RefreshVehicleData(vehicleId);
                }
            }
        }

        private void SetDefaultDates()
        {
            txtRegDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtTransactionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void LoadVehicles()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT mvd.v_id, 
                           cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers as display_number,
                           mvd.current_odometer,
                           ISNULL(mvd.has_natural_gas, 0) as has_natural_gas
                    FROM main_vehicle_details mvd
                    INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                    WHERE mvd.deleted = 0 AND (mvd.hide = 0 OR mvd.hide IS NULL)
                    ORDER BY display_number";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlVehicle.DataSource = dt;
                ddlVehicle.DataTextField = "display_number";
                ddlVehicle.DataValueField = "v_id";
                ddlVehicle.DataBind();
                ddlVehicle.Items.Insert(0, new ListItem("-- اختر السيارة --", "0"));
            }
        }

        private bool GetVehicleNaturalGasStatus(int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT ISNULL(has_natural_gas, 0) FROM main_vehicle_details WHERE v_id = @vehicleId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToBoolean(result);

                return false;
            }
        }

        private void UpdateUIBasedOnFuelType(bool hasNaturalGas)
        {
            _hasNaturalGas = hasNaturalGas;

            if (hasNaturalGas)
            {
                txtHasNaturalGas.Text = "نعم - غاز طبيعي";
                naturalGasFields.Visible = true;              // show gas section
                lblConsumptionUnit.Text = "كم/م³ مكافئ";

                int gasFuelTypeId = GetGasFuelTypeId();
                if (gasFuelTypeId > 0)
                {
                    decimal cubicPrice = GetFuelPricePerCubicMeter(gasFuelTypeId);
                    txtCubicPrice.Text = cubicPrice.ToString("N2");
                }
            }
            else
            {
                txtHasNaturalGas.Text = "لا - بنزين/سولار";
                naturalGasFields.Visible = false;              // FIX: actually hide it
                lblConsumptionUnit.Text = "كم/لتر";

                // Clear gas fields since they don't apply to this vehicle
                txtCubicMeters.Text = "";
                txtCubicPrice.Text = "0.00";
                txtTotalGasCost.Text = "";
            }

            divLiters.Visible = true;
            divLiterPrice.Visible = true;
        }

        private void LoadVehiclesForTransaction()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT mvd.v_id, 
                           cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers as display_number
                    FROM main_vehicle_details mvd
                    INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                    WHERE mvd.deleted = 0 AND (mvd.hide = 0 OR mvd.hide IS NULL)
                    ORDER BY display_number";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTransactionVehicle.DataSource = dt;
                ddlTransactionVehicle.DataTextField = "display_number";
                ddlTransactionVehicle.DataValueField = "v_id";
                ddlTransactionVehicle.DataBind();
                ddlTransactionVehicle.Items.Insert(0, new ListItem("-- اختر السيارة --", "0"));
            }
        }

        private void LoadFuelTypes()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT fuel_id, type FROM fuel_type WHERE ISNULL(is_active, 1) = 1 ORDER BY type";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlFuelType.DataSource = dt;
                ddlFuelType.DataTextField = "type";
                ddlFuelType.DataValueField = "fuel_id";
                ddlFuelType.DataBind();
                ddlFuelType.Items.Insert(0, new ListItem("-- اختر النوع --", "0"));
            }
        }

        private void LoadCardNumbers()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT card_id, card_number FROM fuel_cards ORDER BY card_number";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlCardNumber.DataSource = dt;
                ddlCardNumber.DataTextField = "card_number";
                ddlCardNumber.DataValueField = "card_id";
                ddlCardNumber.DataBind();
                ddlCardNumber.Items.Insert(0, new ListItem("-- بدون كارت --", "0"));
            }
        }

        private void LoadTransactionCards()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT card_id, card_number FROM fuel_cards ORDER BY card_number";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTransactionCard.DataSource = dt;
                ddlTransactionCard.DataTextField = "card_number";
                ddlTransactionCard.DataValueField = "card_id";
                ddlTransactionCard.DataBind();
                ddlTransactionCard.Items.Insert(0, new ListItem("-- اختر الكارت --", "0"));
            }
        }

        private void LoadTransactionsGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT TOP 50 
                    t.transaction_id,
                    t.transaction_number,
                    CONVERT(VARCHAR, t.transaction_date, 23) as transaction_date,
                    c.card_number,
                    ISNULL(cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers, 'بدون سيارة') as vehicle_number,
                    t.transaction_type,
                    t.amount
                FROM fuel_card_transactions t
                INNER JOIN fuel_cards c ON t.card_id = c.card_id
                LEFT JOIN main_vehicle_details v ON t.v_id = v.v_id
                LEFT JOIN car_numbers cn ON v.v_num = cn.car_number_id
                ORDER BY t.transaction_date DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvTransactions.DataSource = dt;
                    gvTransactions.DataBind();

                    CalculateTotals();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadTransactionsGrid: {ex.Message}");
            }
        }

        private void CalculateTotals()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string depositQuery = @"
    SELECT ISNULL(SUM(amount), 0) 
    FROM fuel_card_transactions 
    WHERE transaction_type = N'إيداع'";

                    SqlCommand cmd1 = new SqlCommand(depositQuery, conn);
                    decimal deposits = Convert.ToDecimal(cmd1.ExecuteScalar());
                    lblTotalDeposits.Text = deposits.ToString("N2");

                    string withdrawQuery = @"
    SELECT ISNULL(SUM(amount), 0) 
    FROM fuel_card_transactions 
    WHERE transaction_type = N'سحب' OR transaction_type = N'تعبئة'";

                    SqlCommand cmd2 = new SqlCommand(withdrawQuery, conn);
                    decimal withdrawals = Convert.ToDecimal(cmd2.ExecuteScalar());
                    lblTotalWithdrawals.Text = withdrawals.ToString("N2");

                    decimal balance = deposits - withdrawals;
                    lblCurrentBalance.Text = balance.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CalculateTotals: {ex.Message}");
                lblTotalDeposits.Text = "0.00";
                lblTotalWithdrawals.Text = "0.00";
                lblCurrentBalance.Text = "0.00";
            }
        }

        // FIXED: Get last fuel entry - ORDER BY fuel_id DESC to get the absolute last entry
        private DataRow GetLastFuelEntry(int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT TOP 1 
                        fc.fuel_id,
                        fc.reg_date,
                        ISNULL(fc.liter_num, 0) as liter_num,
                        ISNULL(fc.liter_price, 0) as liter_price,
                        ISNULL(fc.cubic_meters, 0) as cubic_meters,
                        ISNULL(fc.cubic_price, 0) as cubic_price,
                        RTRIM(LTRIM(ISNULL(fc.fule_range, ''))) as odometer_reading
                    FROM fuel_consumption fc
                    WHERE fc.car_details = @vehicleId
                      AND fc.deleted = 0
                    ORDER BY fc.fuel_id DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }

                return null;
            }
        }

        private decimal? GetCurrentOdometer(int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT current_odometer FROM main_vehicle_details WHERE v_id = @vehicleId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToDecimal(result);

                return null;
            }
        }

        private void UpdateVehicleOdometer(int vehicleId, decimal odometer)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    UPDATE main_vehicle_details 
                    SET current_odometer = @odometer,
                        last_updated = GETDATE()
                    WHERE v_id = @vehicleId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@odometer", odometer);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private decimal CalculateDistanceFromOdometer(decimal currentOdometer, decimal? lastOdometer)
        {
            if (lastOdometer.HasValue && currentOdometer > lastOdometer.Value)
                return currentOdometer - lastOdometer.Value;

            return 0;
        }

        private string GenerateTransactionNumber(string prefix, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT COUNT(*) FROM fuel_consumption 
                    WHERE transaction_number LIKE @prefix + '%' AND YEAR(reg_date) = @year";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@prefix", prefix);
                cmd.Parameters.AddWithValue("@year", date.Year);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return $"{prefix}-{date.Year}-{(count + 1):D4}";
            }
        }

        private string GenerateCardTransactionNumber(string prefix, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT COUNT(*) FROM fuel_card_transactions 
                    WHERE transaction_number LIKE @prefix + '%' AND YEAR(transaction_date) = @year";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@prefix", prefix);
                cmd.Parameters.AddWithValue("@year", date.Year);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return $"{prefix}-{date.Year}-{(count + 1):D4}";
            }
        }

        protected void ddlVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlVehicle.SelectedValue != "0")
            {
                int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                bool hasNaturalGas = GetVehicleNaturalGasStatus(vehicleId);
                UpdateUIBasedOnFuelType(hasNaturalGas);
                RefreshVehicleData(vehicleId);
            }
            else
            {
                ClearAllVehicleData();
            }
        }

        // Refresh vehicle data - shows LAST fuel entry's fule_range
        // Refresh vehicle data - shows LAST fuel entry's fule_range
        private void RefreshVehicleData(int vehicleId)
        {
            try
            {
                // Get current odometer from database
                decimal? currentOdometer = GetCurrentOdometer(vehicleId);

                if (currentOdometer.HasValue)
                {
                    txtCurrentOdometer.Text = currentOdometer.Value.ToString("N0");
                }
                else
                {
                    txtCurrentOdometer.Text = "";
                }

                // Get last fuel entry with fresh data
                DataRow lastFuel = GetLastFuelEntry(vehicleId);

                if (lastFuel != null)
                {
                    DateTime lastDate = Convert.ToDateTime(lastFuel["reg_date"]);
                    decimal lastLiters = Convert.ToDecimal(lastFuel["liter_num"]);
                    decimal lastCubicMeters = Convert.ToDecimal(lastFuel["cubic_meters"]);

                    // CRITICAL: Get the odometer from fule_range of the LAST fuel entry
                    decimal lastOdometer = 0;
                    string odometerStr = lastFuel["odometer_reading"].ToString().Trim();
                    if (!string.IsNullOrEmpty(odometerStr))
                    {
                        decimal.TryParse(odometerStr, out lastOdometer);
                    }

                    // Build fuel info string
                    string fuelInfo = "";
                    if (lastLiters > 0)
                        fuelInfo += $"{lastLiters:N2} لتر";
                    if (lastCubicMeters > 0)
                    {
                        if (!string.IsNullOrEmpty(fuelInfo)) fuelInfo += " + ";
                        fuelInfo += $"{lastCubicMeters:N2} م³";
                    }
                    if (string.IsNullOrEmpty(fuelInfo))
                        fuelInfo = "0";

                    // Show the LAST fuel entry's odometer from fule_range
                    lblLastFuelInfo.Text = $"{lastDate:yyyy-MM-dd} - {fuelInfo} - العداد: {lastOdometer:N0} كم";

                    // Calculate distance from current odometer to last odometer
                    if (!string.IsNullOrEmpty(txtCurrentOdometer.Text) && lastOdometer > 0)
                    {
                        decimal currentOdomVal = Convert.ToDecimal(txtCurrentOdometer.Text);
                        decimal distance = CalculateDistanceFromOdometer(currentOdomVal, lastOdometer);
                        txtDistance.Text = distance.ToString("N2");

                        // Calculate consumption using the last fuel entry's quantity
                        decimal totalFuel = lastLiters + (lastCubicMeters * GasEnergyEquivalent);
                        if (distance > 0 && totalFuel > 0)
                        {
                            decimal consumption = distance / totalFuel;
                            lblPreviousConsumption.Text = consumption.ToString("N2");
                            consumptionAlert.Visible = true;
                            lblConsumptionUnit.Text = _hasNaturalGas ? "كم/م³ مكافئ" : "كم/لتر";
                        }
                        else
                        {
                            consumptionAlert.Visible = false;
                        }
                    }
                    else
                    {
                        txtDistance.Text = "0.00";
                        consumptionAlert.Visible = false;
                    }
                }
                else
                {
                    lblLastFuelInfo.Text = "لا توجد تعبئة سابقة - هذه أول تعبئة للسيارة";
                    consumptionAlert.Visible = false;
                    txtDistance.Text = "0.00";
                }

                // ===== NEW: Load gas price if vehicle has natural gas =====
                if (_hasNaturalGas)
                {
                    // Get the gas fuel type ID (find the fuel type that contains "غاز")
                    int gasFuelTypeId = GetGasFuelTypeId();
                    if (gasFuelTypeId > 0)
                    {
                        decimal cubicPrice = GetFuelPricePerCubicMeter(gasFuelTypeId);
                        txtCubicPrice.Text = cubicPrice.ToString("N2");
                        System.Diagnostics.Debug.WriteLine($"Gas price loaded: {cubicPrice} for vehicle {vehicleId}");
                    }
                    else
                    {
                        txtCubicPrice.Text = "0.00";
                        System.Diagnostics.Debug.WriteLine("No gas fuel type found");
                    }
                }
                else
                {
                    // If vehicle doesn't have gas, you can still show the price or set to 0
                    txtCubicPrice.Text = "0.00";
                }
                // =========================================================
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshVehicleData: {ex.Message}");
                lblLastFuelInfo.Text = "خطأ في تحميل البيانات";
            }
        }
        // Get the fuel type ID for gas
        private int GetGasFuelTypeId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT TOP 1 fuel_id FROM fuel_type WHERE type LIKE N'%غاز%' AND is_active = 1 ORDER BY fuel_id";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting gas fuel type ID: {ex.Message}");
            }
            return 0;
        }

        private void ClearAllVehicleData()
        {
            txtCurrentOdometer.Text = "";
            txtDistance.Text = "";
            lblLastFuelInfo.Text = "لا توجد تعبئة سابقة";
            consumptionAlert.Visible = false;
            txtLiters.Text = "";
            txtLiterPrice.Text = "";
            if (txtCubicMeters != null)
                txtCubicMeters.Text = "";
            if (txtCubicPrice != null)
                txtCubicPrice.Text = "";
            if (txtTotalGasCost != null)
                txtTotalGasCost.Text = "";
            txtTotalCost.Text = "";
        }

        protected void txtCurrentOdometer_TextChanged(object sender, EventArgs e)
        {
            // This updates the distance and consumption when the user types a new odometer
            if (ddlVehicle.SelectedValue != "0" && !string.IsNullOrEmpty(txtCurrentOdometer.Text))
            {
                int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                decimal currentOdometer = Convert.ToDecimal(txtCurrentOdometer.Text);

                DataRow lastFuel = GetLastFuelEntry(vehicleId);
                if (lastFuel != null)
                {
                    decimal lastOdometer = 0;
                    string odometerStr = lastFuel["odometer_reading"].ToString().Trim();
                    if (!string.IsNullOrEmpty(odometerStr))
                    {
                        decimal.TryParse(odometerStr, out lastOdometer);
                    }

                    decimal distance = CalculateDistanceFromOdometer(currentOdometer, lastOdometer);
                    txtDistance.Text = distance.ToString("N2");

                    if (distance > 0 && lastOdometer > 0)
                    {
                        decimal lastLiters = Convert.ToDecimal(lastFuel["liter_num"]);
                        decimal lastCubicMeters = Convert.ToDecimal(lastFuel["cubic_meters"]);
                        decimal totalFuel = lastLiters + (lastCubicMeters * GasEnergyEquivalent);

                        if (totalFuel > 0)
                        {
                            decimal consumption = distance / totalFuel;
                            lblPreviousConsumption.Text = consumption.ToString("N2");
                            consumptionAlert.Visible = true;
                            lblConsumptionUnit.Text = _hasNaturalGas ? "كم/م³ مكافئ" : "كم/لتر";
                        }
                    }
                    else
                    {
                        consumptionAlert.Visible = false;
                    }
                }
            }
        }

        protected void btnTabFuel_Click(object sender, EventArgs e)
        {
            MainMultiView.ActiveViewIndex = 0;
            btnTabFuel.CssClass = "btn-tab active";
            btnTabTransactions.CssClass = "btn-tab";

            if (ddlVehicle.SelectedValue != "0")
            {
                int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                RefreshVehicleData(vehicleId);
            }
        }

        protected void btnTabTransactions_Click(object sender, EventArgs e)
        {
            MainMultiView.ActiveViewIndex = 1;
            btnTabFuel.CssClass = "btn-tab";
            btnTabTransactions.CssClass = "btn-tab active";
            LoadTransactionsGrid();
        }

        protected void btnSaveFuel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFuelEntry())
                    return;

                int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                DateTime regDate = DateTime.Parse(txtRegDate.Text);
                decimal currentOdometer = Convert.ToDecimal(txtCurrentOdometer.Text);
                bool hasNaturalGas = GetVehicleNaturalGasStatus(vehicleId);

                decimal totalCost = 0;
                string notes = txtNotes.Text;

                decimal liters = string.IsNullOrEmpty(txtLiters.Text) ? 0 : Convert.ToDecimal(txtLiters.Text);
                decimal literPrice = string.IsNullOrEmpty(txtLiterPrice.Text) ? 0 : Convert.ToDecimal(txtLiterPrice.Text);
                decimal fuelCost = liters * literPrice;
                totalCost += fuelCost;

                decimal gasCost = 0;
                decimal cubicMeters = 0;
                decimal cubicPrice = 0;

                if (hasNaturalGas)
                {
                    cubicMeters = string.IsNullOrEmpty(txtCubicMeters.Text) ? 0 : Convert.ToDecimal(txtCubicMeters.Text);
                    cubicPrice = string.IsNullOrEmpty(txtCubicPrice.Text) ? 0 : Convert.ToDecimal(txtCubicPrice.Text);
                    gasCost = cubicMeters * cubicPrice;
                    totalCost += gasCost;
                }

                // Build notes
                if (liters > 0)
                    notes = $"[وقود: {liters:N2} لتر - {fuelCost:N2} ج.ن] " + notes;
                if (cubicMeters > 0)
                    notes = $"[غاز: {cubicMeters:N2} م³ - {gasCost:N2} ج.ن] " + notes;

                // Get last fuel entry for distance calculation
                DataRow lastFuel = GetLastFuelEntry(vehicleId);
                decimal? lastOdometer = null;
                if (lastFuel != null)
                {
                    string odometerStr = lastFuel["odometer_reading"].ToString().Trim();
                    if (!string.IsNullOrEmpty(odometerStr))
                    {
                        decimal parsedOdometer;
                        if (decimal.TryParse(odometerStr, out parsedOdometer))
                        {
                            lastOdometer = parsedOdometer;
                        }
                    }
                }

                decimal distance = CalculateDistanceFromOdometer(currentOdometer, lastOdometer);
                decimal totalQuantity = liters + cubicMeters;
                decimal consumptionRate = distance > 0 && totalQuantity > 0 ? Math.Round(distance / totalQuantity, 2) : 0;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string transactionNumber = GenerateTransactionNumber("FUEL", regDate);

                    string query = @"
                        INSERT INTO fuel_consumption 
                        (liter_num, fuel_type, petrol_station_location, fule_range, reg_date, deleted, hide, note, car_details, card_id, liter_price, transaction_number, cubic_meters, cubic_price)
                        VALUES 
                        (@liters, @fuelType, @station, @fuleRange, @regDate, 0, 0, @notes, @carId, @cardId, @literPrice, @transactionNumber, @cubicMeters, @cubicPrice)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@liters", liters);
                    cmd.Parameters.AddWithValue("@fuelType", ddlFuelType.SelectedValue);
                    cmd.Parameters.AddWithValue("@station", string.IsNullOrEmpty(txtStation.Text) ? "غير محدد" : txtStation.Text);
                    cmd.Parameters.AddWithValue("@fuleRange", currentOdometer.ToString("F0").Trim());
                    cmd.Parameters.AddWithValue("@regDate", regDate);
                    cmd.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(notes) ? "لا يوجد" : notes);
                    cmd.Parameters.AddWithValue("@carId", vehicleId);
                    cmd.Parameters.AddWithValue("@cardId", ddlCardNumber.SelectedValue);
                    cmd.Parameters.AddWithValue("@literPrice", literPrice);
                    cmd.Parameters.AddWithValue("@transactionNumber", transactionNumber);
                    cmd.Parameters.AddWithValue("@cubicMeters", cubicMeters);
                    cmd.Parameters.AddWithValue("@cubicPrice", cubicPrice);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        UpdateVehicleOdometer(vehicleId, currentOdometer);

                        // In btnSaveFuel_Click — change "تعبئة" to "سحب"
                        if (ddlCardNumber.SelectedValue != "0")
                        {
                            AddCardTransaction(Convert.ToInt32(ddlCardNumber.SelectedValue), vehicleId, totalCost, "سحب", regDate);
                        }

                        // Clear form fields
                        ClearFuelFormFields();

                        // Show success message
                        string message = "تم حفظ بيانات التعبئة بنجاح\n";
                        message += $"رقم المعاملة: {transactionNumber}\n";
                        if (liters > 0)
                            message += $"عدد اللترات: {liters:N2}\n";
                        if (cubicMeters > 0)
                            message += $"الغاز الطبيعي: {cubicMeters:N2} م³ - {gasCost:N2} ج.ن\n";
                        message += $"إجمالي التكلفة: {totalCost:N2} ج.ن\n";
                        if (distance > 0 && totalQuantity > 0)
                        {
                            message += $"\nالمسافة المقطوعة: {distance:N2} كم\n";
                            message += $"معدل الاستهلاك الكلي: {consumptionRate:N2} كم/وحدة";
                        }

                        ShowMessage(message, "success");

                        // Refresh the data to show the updated last fuel entry
                        RefreshVehicleData(vehicleId);

                        // Load transactions grid
                        LoadTransactionsGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في حفظ البيانات: " + ex.Message, "danger");
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            }
        }

        private void ClearFuelFormFields()
        {
            txtLiters.Text = "";
            txtLiterPrice.Text = "";
            txtCubicMeters.Text = "";
            txtCubicPrice.Text = "";
            txtTotalGasCost.Text = "";
            txtTotalCost.Text = "";
            txtStation.Text = "";
            txtNotes.Text = "";
            txtRegDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private bool ValidateFuelEntry()
        {
            if (ddlVehicle.SelectedValue == "0")
            {
                ShowMessage("الرجاء اختيار السيارة", "danger");
                return false;
            }

            if (ddlFuelType.SelectedValue == "0")
            {
                ShowMessage("الرجاء اختيار نوع الوقود", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtCurrentOdometer.Text))
            {
                ShowMessage("الرجاء إدخال قراءة العداد الحالية", "danger");
                return false;
            }

            decimal currentOdometer = Convert.ToDecimal(txtCurrentOdometer.Text);
            if (currentOdometer < 0)
            {
                ShowMessage("قراءة العداد لا يمكن أن تكون سالبة", "danger");
                return false;
            }

            bool hasLiters = !string.IsNullOrEmpty(txtLiters.Text) && Convert.ToDecimal(txtLiters.Text) > 0;
            bool hasCubicMeters = false;

            if (_hasNaturalGas)
            {
                hasCubicMeters = !string.IsNullOrEmpty(txtCubicMeters.Text) && Convert.ToDecimal(txtCubicMeters.Text) > 0;
            }

            if (!hasLiters && !hasCubicMeters)
            {
                ShowMessage("الرجاء إدخال كمية الوقود السائل أو كمية الغاز الطبيعي", "danger");
                return false;
            }

            int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
            decimal? lastOdometer = GetLastOdometer(vehicleId);
            if (lastOdometer.HasValue && currentOdometer < lastOdometer.Value)
            {
                ShowMessage($"قراءة العداد الحالية ({currentOdometer} كم) أقل من آخر قراءة مسجلة ({lastOdometer.Value} كم). يرجى التأكد من القراءة.", "danger");
                return false;
            }

            return true;
        }

        private decimal? GetLastOdometer(int vehicleId)
        {
            DataRow lastFuel = GetLastFuelEntry(vehicleId);
            if (lastFuel != null)
            {
                string odometerStr = lastFuel["odometer_reading"].ToString().Trim();
                if (!string.IsNullOrEmpty(odometerStr))
                {
                    decimal parsedOdometer;
                    if (decimal.TryParse(odometerStr, out parsedOdometer))
                    {
                        return parsedOdometer;
                    }
                }
            }
            return null;
        }

        private void AddCardTransaction(int cardId, int vehicleId, decimal amount, string type, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string transactionNumber = GenerateCardTransactionNumber("CARD", date);

                string query = @"
                    INSERT INTO fuel_card_transactions (card_id, v_id, amount, transaction_type, transaction_date, transaction_number)
                    VALUES (@cardId, @vehicleId, @amount, @type, @date, @transactionNumber)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cardId", cardId);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@transactionNumber", transactionNumber);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnAddTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTransactionCard.SelectedValue == "0")
                {
                    ShowMessage("الرجاء اختيار الكارت", "danger");
                    return;
                }

                if (string.IsNullOrEmpty(txtTransactionAmount.Text))
                {
                    ShowMessage("الرجاء إدخال المبلغ", "danger");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    DateTime transactionDate = DateTime.Parse(txtTransactionDate.Text);
                    string transactionNumber = GenerateCardTransactionNumber("CARD", transactionDate);

                    string query = @"
                        INSERT INTO fuel_card_transactions (card_id, v_id, amount, transaction_type, transaction_date, transaction_number)
                        VALUES (@cardId, @vehicleId, @amount, @type, @date, @transactionNumber)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cardId", ddlTransactionCard.SelectedValue);

                    if (ddlTransactionVehicle.SelectedValue == "0")
                        cmd.Parameters.AddWithValue("@vehicleId", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@vehicleId", ddlTransactionVehicle.SelectedValue);

                    cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtTransactionAmount.Text));
                    cmd.Parameters.AddWithValue("@type", ddlTransactionType.SelectedValue);
                    cmd.Parameters.AddWithValue("@date", transactionDate);
                    cmd.Parameters.AddWithValue("@transactionNumber", transactionNumber);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ShowMessage($"تم إضافة المعاملة بنجاح. رقم المعاملة: {transactionNumber}", "success");

                        ddlTransactionCard.SelectedIndex = 0;
                        ddlTransactionVehicle.SelectedIndex = 0;
                        txtTransactionAmount.Text = "";
                        ddlTransactionType.SelectedIndex = 0;
                        txtTransactionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                        LoadTransactionsGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في إضافة المعاملة: " + ex.Message, "danger");
            }
        }

        protected void gvTransactions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteTransaction")
            {
                try
                {
                    int transId = Convert.ToInt32(e.CommandArgument);

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = "DELETE FROM fuel_card_transactions WHERE transaction_id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", transId);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        ShowMessage("تم حذف المعاملة بنجاح", "success");
                        LoadTransactionsGrid();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("خطأ في حذف المعاملة: " + ex.Message, "danger");
                }
            }
        }

        protected void btnResetFuel_Click(object sender, EventArgs e)
        {
            ClearFuelFormFields();

            if (ddlVehicle.SelectedValue != "0")
            {
                int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                RefreshVehicleData(vehicleId);
            }

            ShowMessage("تم إعادة تعيين النموذج", "info");
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.CssClass = $"alert alert-{type}";
            lblMessage.Text = message;
            pnlMessage.Visible = true;

            string script = @"
                setTimeout(function() {
                    var pnl = document.getElementById('" + pnlMessage.ClientID + @"');
                    if(pnl) pnl.style.display = 'none';
                }, 5000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideMessage", script, true);
        }

        // Add this method to load fuel prices from database
        private void LoadFuelPrices(int fuelTypeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                SELECT 
                    ISNULL(price_per_liter, 0) as price_per_liter,
                    ISNULL(price_per_cubic_meter, 0) as price_per_cubic_meter
                FROM fuel_prices 
                WHERE fuel_type_id = @fuelTypeId AND is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal literPrice = Convert.ToDecimal(reader["price_per_liter"]);
                        decimal cubicPrice = Convert.ToDecimal(reader["price_per_cubic_meter"]);

                        txtLiterPrice.Text = literPrice.ToString("N2");
                        txtCubicPrice.Text = cubicPrice.ToString("N2");

                        System.Diagnostics.Debug.WriteLine($"Loaded prices - Liter: {literPrice}, Cubic: {cubicPrice}");
                    }
                    else
                    {
                        // If no price found, set default values
                        txtLiterPrice.Text = "0.00";
                        txtCubicPrice.Text = "0.00";
                        System.Diagnostics.Debug.WriteLine("No prices found for fuel type ID: " + fuelTypeId);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading fuel prices: {ex.Message}");
                txtLiterPrice.Text = "0.00";
                txtCubicPrice.Text = "0.00";
            }
        }

        // Add this method to get fuel price by fuel type
        private decimal GetFuelPricePerLiter(int fuelTypeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT ISNULL(price_per_liter, 0) FROM fuel_prices WHERE fuel_type_id = @fuelTypeId AND is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting fuel price: {ex.Message}");
            }
            return 0;
        }

        private decimal GetFuelPricePerCubicMeter(int fuelTypeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT ISNULL(price_per_cubic_meter, 0) FROM fuel_prices WHERE fuel_type_id = @fuelTypeId AND is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting fuel price: {ex.Message}");
            }
            return 0;
        }
        // Add this event handler in your code-behind
        protected void ddlFuelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFuelType.SelectedValue != "0")
            {
                int fuelTypeId = Convert.ToInt32(ddlFuelType.SelectedValue);
                bool isGas = IsFuelTypeGas(fuelTypeId);

                // Load liquid fuel price (liter)
                decimal literPrice = GetFuelPricePerLiter(fuelTypeId);
                txtLiterPrice.Text = literPrice.ToString("N2");

                // If it's gas, load gas price as well
                if (isGas)
                {
                    decimal cubicPrice = GetFuelPricePerCubicMeter(fuelTypeId);
                    txtCubicPrice.Text = cubicPrice.ToString("N2");
                    lblConsumptionUnit.Text = "كم/م³ مكافئ";
                    System.Diagnostics.Debug.WriteLine($"Gas selected - Liter Price: {literPrice}, Cubic Price: {cubicPrice}");
                }
                else
                {
                    // If liquid fuel is selected, keep the gas price that was already loaded
                    // from the vehicle selection (or load the default gas price)
                    if (string.IsNullOrEmpty(txtCubicPrice.Text) || txtCubicPrice.Text == "0.00")
                    {
                        int gasFuelTypeId = GetGasFuelTypeId();
                        if (gasFuelTypeId > 0)
                        {
                            decimal cubicPrice = GetFuelPricePerCubicMeter(gasFuelTypeId);
                            txtCubicPrice.Text = cubicPrice.ToString("N2");
                        }
                    }
                    lblConsumptionUnit.Text = "كم/لتر";
                    System.Diagnostics.Debug.WriteLine($"Liquid fuel selected - Liter Price: {literPrice}");
                }

                // Clear totals
                txtTotalCost.Text = "";
                txtTotalGasCost.Text = "";
            }
            else
            {
                // Reset fields
                txtLiterPrice.Text = "0.00";
                txtTotalCost.Text = "";
                txtTotalGasCost.Text = "";
            }
        }
        // Helper method to check if fuel type is gas
        private bool IsFuelTypeGas(int fuelTypeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT type FROM fuel_type WHERE fuel_id = @fuelTypeId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fuelTypeId", fuelTypeId);

                    conn.Open();
                    string fuelTypeName = cmd.ExecuteScalar()?.ToString();

                    // Check if the fuel type name contains "غاز" (gas)
                    return !string.IsNullOrEmpty(fuelTypeName) && fuelTypeName.Contains("غاز");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking fuel type: {ex.Message}");
            }
            return false;
        }
    }
}