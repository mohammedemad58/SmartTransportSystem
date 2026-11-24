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
                LoadTransactionsGrid();
                SetDefaultDates();

                // If a vehicle is already selected, refresh data
                if (ddlVehicle.SelectedValue != "0")
                {
                    int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);
                    bool hasNaturalGas = GetVehicleNaturalGasStatus(vehicleId);
                    UpdateUIBasedOnFuelType(hasNaturalGas);
                    RefreshVehicleData(vehicleId);
                    SelectLastCardForVehicle(vehicleId);
                    ShowCardBalance();
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

        /// <summary>
        /// Returns car_numbers.car_number_id for the vehicle (main_vehicle_details.v_num).
        /// fuel_card_transactions.v_id has a FK to car_numbers.car_number_id, so THIS is the value stored there.
        /// </summary>
        private int? GetVehicleCarNumberId(int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT v_num FROM main_vehicle_details WHERE v_id = @vehicleId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);

                return null;
            }
        }

        private void UpdateUIBasedOnFuelType(bool hasNaturalGas)
        {
            _hasNaturalGas = hasNaturalGas;

            if (hasNaturalGas)
            {
                txtHasNaturalGas.Text = "نعم - غاز طبيعي";
                naturalGasFields.Visible = true;
                lblConsumptionUnit.Text = "كم/م³ مكافئ";
            }
            else
            {
                txtHasNaturalGas.Text = "لا - بنزين/سولار";
                naturalGasFields.Visible = false;
                lblConsumptionUnit.Text = "كم/لتر";

                // Clear gas field since it doesn't apply to this vehicle
                if (txtCubicMeters != null)
                    txtCubicMeters.Text = "";
            }

            divLiters.Visible = true;
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
                string query = "SELECT card_id, card_number FROM fuel_cards WHERE is_active = 1 ORDER BY card_number";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlCardNumber.DataSource = dt;
                ddlCardNumber.DataTextField = "card_number";
                ddlCardNumber.DataValueField = "card_id";
                ddlCardNumber.DataBind();
                // Card is mandatory - the placeholder item has value 0 and is rejected by the validator
                ddlCardNumber.Items.Insert(0, new ListItem("-- اختر الكارت --", "0"));
            }
        }

        private void LoadTransactionCards()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT card_id, card_number FROM fuel_cards WHERE is_active = 1 ORDER BY card_number";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTransactionCard.DataSource = dt;
                ddlTransactionCard.DataTextField = "card_number";
                ddlTransactionCard.DataValueField = "card_id";
                ddlTransactionCard.DataBind();
                ddlTransactionCard.Items.Insert(0, new ListItem("-- اختر الكارت --", "0"));

                ddlTransferToCard.DataSource = dt;
                ddlTransferToCard.DataTextField = "card_number";
                ddlTransferToCard.DataValueField = "card_id";
                ddlTransferToCard.DataBind();
                ddlTransferToCard.Items.Insert(0, new ListItem("-- اختر الكارت --", "0"));
            }
        }

        // Pre-selects the card this vehicle used on its last fuel entry (user can still change it)
        private void SelectLastCardForVehicle(int vehicleId)
        {
            ddlCardNumber.SelectedIndex = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT TOP 1 card_id
                    FROM fuel_consumption
                    WHERE car_details = @vehicleId
                      AND (deleted = 0 OR deleted IS NULL)
                      AND card_id IS NOT NULL
                    ORDER BY fuel_id DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    ListItem item = ddlCardNumber.Items.FindByValue(result.ToString());
                    if (item != null)
                        ddlCardNumber.SelectedValue = item.Value;
                }
            }
        }

        // ===================== CARD BALANCE HELPERS =====================

        private (decimal literBalance, decimal cubicBalance) GetCardBalance(int cardId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT ISNULL(liter_balance,0), ISNULL(cubic_balance,0) FROM fuel_cards WHERE card_id = @cardId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cardId", cardId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    decimal l = Convert.ToDecimal(reader[0]);
                    decimal c = Convert.ToDecimal(reader[1]);
                    reader.Close();
                    return (l, c);
                }
                reader.Close();
                return (0, 0);
            }
        }

        private void AdjustCardBalance(int cardId, decimal literDelta, decimal cubicDelta)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    UPDATE fuel_cards
                    SET liter_balance = liter_balance + @literDelta,
                        cubic_balance = cubic_balance + @cubicDelta
                    WHERE card_id = @cardId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@literDelta", literDelta);
                cmd.Parameters.AddWithValue("@cubicDelta", cubicDelta);
                cmd.Parameters.AddWithValue("@cardId", cardId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // carNumberId = car_numbers.car_number_id (saved into fuel_card_transactions.v_id). NULL for charge/transfer rows.
        private void AddCardTransaction(int cardId, int? toCardId, int? carNumberId, string type, string unitType, decimal quantity, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string transactionNumber = GenerateCardTransactionNumber("CARD", date);

                string query = @"
                    INSERT INTO fuel_card_transactions (card_id, to_card_id, v_id, amount, unit_type, transaction_type, transaction_date, transaction_number)
                    VALUES (@cardId, @toCardId, @carNumberId, @quantity, @unitType, @type, @date, @transactionNumber)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cardId", cardId);
                cmd.Parameters.AddWithValue("@toCardId", (object)toCardId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@carNumberId", (object)carNumberId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@unitType", unitType);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@transactionNumber", transactionNumber);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ShowCardBalance()
        {
            if (ddlCardNumber.SelectedValue != "0")
            {
                int cardId = Convert.ToInt32(ddlCardNumber.SelectedValue);
                var bal = GetCardBalance(cardId);
                lblCardBalance.Text = $"الرصيد الحالي: {bal.literBalance:N2} لتر / {bal.cubicBalance:N2} م³";
            }
            else
            {
                lblCardBalance.Text = "";
            }
        }

        protected void ddlCardNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCardBalance();
        }

        protected void ddlTransactionCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTransactionCard.SelectedValue != "0")
            {
                var bal = GetCardBalance(Convert.ToInt32(ddlTransactionCard.SelectedValue));
                lblTransactionCardBalance.Text = $"الرصيد: {bal.literBalance:N2} لتر / {bal.cubicBalance:N2} م³";
            }
            else
            {
                lblTransactionCardBalance.Text = "";
            }
        }

        protected void ddlTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            divToCard.Visible = (ddlTransactionType.SelectedValue == "تحويل");
        }

        // ===================== TRANSACTIONS GRID =====================

        protected void gvTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTransactions.PageIndex = e.NewPageIndex;
            LoadTransactionsGrid();
        }

        private void LoadTransactionsGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // vehicle_number = the car the card was used for (NULL for charge/transfer rows -> shown as "—")
                    string query = @"
                SELECT TOP 200 
                    t.transaction_id,
                    t.transaction_number,
                    CONVERT(VARCHAR, t.transaction_date, 23) as transaction_date,
                    c.card_number,
                    ISNULL(cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers, N'—') as vehicle_number,
                    t.transaction_type,
                    ISNULL(t.amount, 0) as quantity,
                    CASE t.unit_type WHEN 'liter' THEN N'لتر' WHEN 'cubic' THEN N'م³' ELSE '' END as unit_display,
                    tc.card_number as to_card_number
                FROM fuel_card_transactions t
                INNER JOIN fuel_cards c ON t.card_id = c.card_id
                LEFT JOIN fuel_cards tc ON t.to_card_id = tc.card_id
                LEFT JOIN car_numbers cn ON t.v_id = cn.car_number_id
                ORDER BY t.transaction_date DESC, t.transaction_id DESC";

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

                    string chargeLitersQuery = "SELECT ISNULL(SUM(amount),0) FROM fuel_card_transactions WHERE transaction_type = N'شحن' AND unit_type = 'liter'";
                    SqlCommand cmd1 = new SqlCommand(chargeLitersQuery, conn);
                    lblTotalChargeLiters.Text = Convert.ToDecimal(cmd1.ExecuteScalar()).ToString("N2");

                    string chargeCubicQuery = "SELECT ISNULL(SUM(amount),0) FROM fuel_card_transactions WHERE transaction_type = N'شحن' AND unit_type = 'cubic'";
                    SqlCommand cmd2 = new SqlCommand(chargeCubicQuery, conn);
                    lblTotalChargeCubic.Text = Convert.ToDecimal(cmd2.ExecuteScalar()).ToString("N2");

                    string deductionQuery = "SELECT ISNULL(SUM(amount),0) FROM fuel_card_transactions WHERE transaction_type = N'خصم'";
                    SqlCommand cmd3 = new SqlCommand(deductionQuery, conn);
                    lblTotalDeduction.Text = Convert.ToDecimal(cmd3.ExecuteScalar()).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CalculateTotals: {ex.Message}");
                lblTotalChargeLiters.Text = "0.00";
                lblTotalChargeCubic.Text = "0.00";
                lblTotalDeduction.Text = "0.00";
            }
        }

        // Get last fuel entry - ORDER BY fuel_id DESC to get the absolute last entry
        private DataRow GetLastFuelEntry(int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT TOP 1 
                        fc.fuel_id,
                        fc.reg_date,
                        ISNULL(fc.liter_num, 0) as liter_num,
                        ISNULL(fc.cubic_meters, 0) as cubic_meters,
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

                // Bring up the card this car used last time (user may change it)
                SelectLastCardForVehicle(vehicleId);
                ShowCardBalance();
            }
            else
            {
                ClearAllVehicleData();
            }
        }

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

                    // Get the odometer from fule_range of the LAST fuel entry
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshVehicleData: {ex.Message}");
                lblLastFuelInfo.Text = "خطأ في تحميل البيانات";
            }
        }

        private void ClearAllVehicleData()
        {
            txtCurrentOdometer.Text = "";
            txtDistance.Text = "";
            lblLastFuelInfo.Text = "لا توجد تعبئة سابقة";
            consumptionAlert.Visible = false;
            txtLiters.Text = "";
            if (txtCubicMeters != null)
                txtCubicMeters.Text = "";
            ddlCardNumber.SelectedIndex = 0;
            lblCardBalance.Text = "";
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
                            lblConsumptionUnit.Text = GetVehicleNaturalGasStatus(vehicleId) ? "كم/م³ مكافئ" : "كم/لتر";
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

                decimal liters = string.IsNullOrEmpty(txtLiters.Text) ? 0 : Convert.ToDecimal(txtLiters.Text);
                decimal cubicMeters = (hasNaturalGas && !string.IsNullOrEmpty(txtCubicMeters.Text))
                    ? Convert.ToDecimal(txtCubicMeters.Text) : 0;

                // The car number id that will be stored with the card transaction (validated above)
                int? carNumberId = GetVehicleCarNumberId(vehicleId);

                // Card is mandatory (validated above) - check its balance BEFORE saving
                int cardId = Convert.ToInt32(ddlCardNumber.SelectedValue);
                var balance = GetCardBalance(cardId);

                if (liters > 0 && balance.literBalance < liters)
                {
                    ShowMessage($"رصيد الكارت من اللترات غير كافٍ. الرصيد الحالي: {balance.literBalance:N2} لتر", "danger");
                    return;
                }
                if (cubicMeters > 0 && balance.cubicBalance < cubicMeters)
                {
                    ShowMessage($"رصيد الكارت من الغاز غير كافٍ. الرصيد الحالي: {balance.cubicBalance:N2} م³", "danger");
                    return;
                }

                string notes = txtNotes.Text;
                if (liters > 0)
                    notes = $"[وقود: {liters:N2} لتر] " + notes;
                if (cubicMeters > 0)
                    notes = $"[غاز: {cubicMeters:N2} م³] " + notes;

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
                        (@liters, @fuelType, @station, @fuleRange, @regDate, 0, 0, @notes, @carId, @cardId, 0, @transactionNumber, @cubicMeters, 0)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@liters", liters);
                    cmd.Parameters.AddWithValue("@fuelType", ddlFuelType.SelectedValue);
                    cmd.Parameters.AddWithValue("@station", string.IsNullOrEmpty(txtStation.Text) ? "غير محدد" : txtStation.Text);
                    cmd.Parameters.AddWithValue("@fuleRange", currentOdometer.ToString("F0").Trim());
                    cmd.Parameters.AddWithValue("@regDate", regDate);
                    cmd.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(notes) ? "لا يوجد" : notes);
                    cmd.Parameters.AddWithValue("@carId", vehicleId);
                    cmd.Parameters.AddWithValue("@cardId", cardId);
                    cmd.Parameters.AddWithValue("@transactionNumber", transactionNumber);
                    cmd.Parameters.AddWithValue("@cubicMeters", cubicMeters);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        UpdateVehicleOdometer(vehicleId, currentOdometer);

                        // Deduct consumed quantity from the card's balance and log it WITH the car number
                        if (liters > 0)
                        {
                            AdjustCardBalance(cardId, -liters, 0);
                            AddCardTransaction(cardId, null, carNumberId, "خصم", "liter", liters, regDate);
                        }
                        if (cubicMeters > 0)
                        {
                            AdjustCardBalance(cardId, 0, -cubicMeters);
                            AddCardTransaction(cardId, null, carNumberId, "خصم", "cubic", cubicMeters, regDate);
                        }

                        // Clear form fields (the card selection is kept for the next entry of the same car)
                        ClearFuelFormFields();

                        // Show success message
                        string message = "تم حفظ بيانات التعبئة بنجاح\n";
                        message += $"رقم المعاملة: {transactionNumber}\n";
                        if (liters > 0)
                            message += $"عدد اللترات: {liters:N2}\n";
                        if (cubicMeters > 0)
                            message += $"الغاز الطبيعي: {cubicMeters:N2} م³\n";
                        if (distance > 0 && totalQuantity > 0)
                        {
                            message += $"\nالمسافة المقطوعة: {distance:N2} كم\n";
                            message += $"معدل الاستهلاك الكلي: {consumptionRate:N2} كم/وحدة";
                        }

                        ShowMessage(message, "success");

                        // Refresh the data to show the updated last fuel entry
                        RefreshVehicleData(vehicleId);

                        // Load transactions grid and card balance
                        LoadTransactionsGrid();
                        ShowCardBalance();
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
            txtCubicMeters.Text = "";
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

            int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);

            if (!GetVehicleCarNumberId(vehicleId).HasValue)
            {
                ShowMessage("تعذر تحديد رقم لوحة السيارة المختارة", "danger");
                return false;
            }

            if (ddlFuelType.SelectedValue == "0")
            {
                ShowMessage("الرجاء اختيار نوع الوقود", "danger");
                return false;
            }

            // Card is MANDATORY
            if (ddlCardNumber.SelectedValue == "0")
            {
                ShowMessage("الرجاء اختيار رقم الكارت (إجباري)", "danger");
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

            // Read the gas status from the DB (the _hasNaturalGas field is reset on every postback)
            if (GetVehicleNaturalGasStatus(vehicleId))
            {
                hasCubicMeters = !string.IsNullOrEmpty(txtCubicMeters.Text) && Convert.ToDecimal(txtCubicMeters.Text) > 0;
            }

            if (!hasLiters && !hasCubicMeters)
            {
                ShowMessage("الرجاء إدخال كمية الوقود السائل أو كمية الغاز الطبيعي", "danger");
                return false;
            }

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

        protected void btnAddTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTransactionCard.SelectedValue == "0")
                {
                    ShowMessage("الرجاء اختيار الكارت", "danger");
                    return;
                }

                if (string.IsNullOrEmpty(txtTransactionAmount.Text) || Convert.ToDecimal(txtTransactionAmount.Text) <= 0)
                {
                    ShowMessage("الرجاء إدخال كمية صحيحة", "danger");
                    return;
                }

                int cardId = Convert.ToInt32(ddlTransactionCard.SelectedValue);
                string unitType = ddlTransactionUnit.SelectedValue; // "liter" or "cubic"
                decimal quantity = Convert.ToDecimal(txtTransactionAmount.Text);
                DateTime transactionDate = DateTime.Parse(txtTransactionDate.Text);
                string type = ddlTransactionType.SelectedValue;

                if (type == "شحن")
                {
                    AdjustCardBalance(cardId, unitType == "liter" ? quantity : 0, unitType == "cubic" ? quantity : 0);
                    AddCardTransaction(cardId, null, null, "شحن", unitType, quantity, transactionDate);
                    ShowMessage("تم شحن الكارت بنجاح", "success");
                }
                else if (type == "تحويل")
                {
                    if (ddlTransferToCard.SelectedValue == "0")
                    {
                        ShowMessage("الرجاء اختيار الكارت المحول إليه", "danger");
                        return;
                    }

                    int toCardId = Convert.ToInt32(ddlTransferToCard.SelectedValue);
                    if (toCardId == cardId)
                    {
                        ShowMessage("لا يمكن التحويل لنفس الكارت", "danger");
                        return;
                    }

                    var sourceBalance = GetCardBalance(cardId);
                    decimal available = unitType == "liter" ? sourceBalance.literBalance : sourceBalance.cubicBalance;

                    if (available < quantity)
                    {
                        ShowMessage($"رصيد الكارت غير كافٍ للتحويل. الرصيد الحالي: {available:N2}", "danger");
                        return;
                    }

                    AdjustCardBalance(cardId, unitType == "liter" ? -quantity : 0, unitType == "cubic" ? -quantity : 0);
                    AdjustCardBalance(toCardId, unitType == "liter" ? quantity : 0, unitType == "cubic" ? quantity : 0);
                    AddCardTransaction(cardId, toCardId, null, "تحويل", unitType, quantity, transactionDate);
                    ShowMessage("تم التحويل بنجاح", "success");
                }

                ddlTransactionCard.SelectedIndex = 0;
                ddlTransferToCard.SelectedIndex = 0;
                ddlTransactionType.SelectedIndex = 0;
                divToCard.Visible = false;
                txtTransactionAmount.Text = "";
                txtTransactionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                lblTransactionCardBalance.Text = "";

                LoadTransactionsGrid();
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تنفيذ العملية: " + ex.Message, "danger");
            }
        }

        protected void gvTransactions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteTransaction")
            {
                try
                {
                    int transId = Convert.ToInt32(e.CommandArgument);

                    int cardId = 0;
                    int? toCardId = null;
                    decimal amount = 0;
                    string unitType = "";
                    string type = "";

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string selectQuery = "SELECT card_id, to_card_id, amount, unit_type, transaction_type FROM fuel_card_transactions WHERE transaction_id = @id";
                        SqlCommand selectCmd = new SqlCommand(selectQuery, conn);
                        selectCmd.Parameters.AddWithValue("@id", transId);

                        conn.Open();
                        SqlDataReader reader = selectCmd.ExecuteReader();
                        if (reader.Read())
                        {
                            cardId = Convert.ToInt32(reader["card_id"]);
                            toCardId = reader["to_card_id"] != DBNull.Value ? Convert.ToInt32(reader["to_card_id"]) : (int?)null;
                            amount = Convert.ToDecimal(reader["amount"]);
                            unitType = reader["unit_type"] != DBNull.Value ? reader["unit_type"].ToString() : "";
                            type = reader["transaction_type"].ToString();
                        }
                        reader.Close();
                    }

                    // Reverse the balance effect of this transaction before deleting it
                    if (cardId > 0)
                    {
                        if (type == "شحن")
                        {
                            AdjustCardBalance(cardId, unitType == "liter" ? -amount : 0, unitType == "cubic" ? -amount : 0);
                        }
                        else if (type == "خصم")
                        {
                            AdjustCardBalance(cardId, unitType == "liter" ? amount : 0, unitType == "cubic" ? amount : 0);
                        }
                        else if (type == "تحويل" && toCardId.HasValue)
                        {
                            AdjustCardBalance(cardId, unitType == "liter" ? amount : 0, unitType == "cubic" ? amount : 0);
                            AdjustCardBalance(toCardId.Value, unitType == "liter" ? -amount : 0, unitType == "cubic" ? -amount : 0);
                        }
                    }

                    using (SqlConnection conn2 = new SqlConnection(connString))
                    {
                        string deleteQuery = "DELETE FROM fuel_card_transactions WHERE transaction_id = @id";
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn2);
                        cmd.Parameters.AddWithValue("@id", transId);

                        conn2.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("تم حذف المعاملة وإعادة الرصيد بنجاح", "success");
                    LoadTransactionsGrid();
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
                SelectLastCardForVehicle(vehicleId);
                ShowCardBalance();
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
    }
}
