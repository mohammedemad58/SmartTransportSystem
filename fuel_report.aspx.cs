using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class fuel_report : System.Web.UI.Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private const decimal GasEnergyEquivalent = 1.25m;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadVehicles();
                ResetTotals();
                SetDefaultDates();
            }
        }

        private void SetDefaultDates()
        {
            txtFrom.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            txtTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void ResetTotals()
        {
            lblTotalLiters.Text = "0.00";
            lblTotalCost.Text = "0.00";
            lblCardAmount.Text = "0.00";
            lblDifference.Text = "0.00";
            lblAvgConsumption.Text = "0.00";
            lblBestConsumption.Text = "0.00";
            lblWorstConsumption.Text = "0.00";
            lblCardDeposits.Text = "0.00";
            lblCardWithdrawals.Text = "0.00";
            lblCardBalance.Text = "0.00";
            lblTotalCubicMeters.Text = "0.00";
            lblTotalGasCost.Text = "0.00";
        }

        private void LoadVehicles()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT DISTINCT 
                            mvd.v_id,
                            cn.letter1 + ' ' + cn.letter2 + 
                            ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers as display_number
                        FROM main_vehicle_details mvd
                        INNER JOIN car_numbers cn ON mvd.v_num = cn.car_number_id
                        WHERE (cn.is_active = 1 OR cn.is_active IS NULL)
                          AND mvd.deleted = 0
                        ORDER BY display_number";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlVehicle.Items.Clear();
                    ddlVehicle.Items.Add(new ListItem("-- اختر السيارة --", "0"));

                    while (reader.Read())
                    {
                        string value = reader["v_id"].ToString();
                        string text = reader["display_number"].ToString();
                        ddlVehicle.Items.Add(new ListItem(text, value));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading vehicles: " + ex.Message);
                ShowMessage("خطأ في تحميل قائمة السيارات: " + ex.Message);
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
                return result != null && Convert.ToBoolean(result);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlVehicle.SelectedValue == "0")
                {
                    ShowMessage("الرجاء اختيار سيارة");
                    return;
                }

                int vehicleId = Convert.ToInt32(ddlVehicle.SelectedValue);

                DateTime? fromDate = null;
                DateTime? toDate = null;

                if (!string.IsNullOrEmpty(txtFrom.Text))
                {
                    fromDate = DateTime.Parse(txtFrom.Text);
                }

                if (!string.IsNullOrEmpty(txtTo.Text))
                {
                    toDate = DateTime.Parse(txtTo.Text).Date.AddDays(1).AddSeconds(-1);
                }

                LoadFuelData(vehicleId, fromDate, toDate);
                LoadCardTransactions(vehicleId, fromDate, toDate);
                LoadNaturalGasData(vehicleId, fromDate, toDate);

                btnExportExcel.Visible = true;
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Search error: " + ex.Message);
            }
        }

        private void LoadFuelData(int vehicleId, DateTime? fromDate, DateTime? toDate)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string query = @"
                SELECT 
                    fc.fuel_id,
                    fc.transaction_number,
                    CONVERT(VARCHAR, fc.reg_date, 23) as reg_date_formatted,
                    fc.reg_date,
                    RTRIM(LTRIM(ISNULL(fc.fule_range, ''))) as fule_range,
                    ISNULL(fc.liter_num, 0) as liter_num,
                    ISNULL(fc.liter_price, 0) as liter_price,
                    ISNULL(fc.cubic_meters, 0) as cubic_meters,
                    ISNULL(fc.cubic_price, 0) as cubic_price,
                    ISNULL(ft.type, 'غير محدد') as fuel_type,
                    ISNULL(fc.petrol_station_location, '') as petrol_station_location,
                    ISNULL(fc.note, '') as note
                FROM fuel_consumption fc
                LEFT JOIN fuel_type ft ON fc.fuel_type = ft.fuel_id
                WHERE fc.car_details = @vehicleId
                  AND (fc.deleted = 0 OR fc.deleted IS NULL)
                  AND (fc.hide = 0 OR fc.hide IS NULL)";

                    if (fromDate.HasValue) query += " AND fc.reg_date >= @fromDate";
                    if (toDate.HasValue) query += " AND fc.reg_date <= @toDate";
                    query += " ORDER BY fc.reg_date ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                    if (fromDate.HasValue) cmd.Parameters.AddWithValue("@fromDate", fromDate.Value);
                    if (toDate.HasValue) cmd.Parameters.AddWithValue("@toDate", toDate.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataTable displayDt = new DataTable();
                        displayDt.Columns.Add("رقم_المعاملة");
                        displayDt.Columns.Add("التاريخ");
                        displayDt.Columns.Add("نوع_الوقود");
                        displayDt.Columns.Add("اللترات", typeof(decimal));
                        displayDt.Columns.Add("سعر_اللتر", typeof(decimal));
                        displayDt.Columns.Add("عدد_الأمتار_المكعبة", typeof(decimal));
                        displayDt.Columns.Add("سعر_المتر_المكعب", typeof(decimal));
                        displayDt.Columns.Add("التكلفة", typeof(decimal));
                        displayDt.Columns.Add("المسافة", typeof(decimal));
                        displayDt.Columns.Add("معدل_الاستهلاك", typeof(decimal));
                        displayDt.Columns.Add("المحطة");
                        displayDt.Columns.Add("ملاحظات");

                        bool hasNaturalGas = GetVehicleNaturalGasStatus(vehicleId);
                        decimal totalLiters = 0;
                        decimal totalCost = 0;
                        decimal totalDistance = 0;
                        int validConsumptionCount = 0;
                        decimal sumConsumption = 0;
                        decimal bestConsumption = 0;
                        decimal worstConsumption = 0;

                        int recordCount = dt.Rows.Count;
                        decimal[] odometers = new decimal[recordCount];
                        decimal[] litersArray = new decimal[recordCount];
                        decimal[] cubicArray = new decimal[recordCount];
                        string[] transactionNumbers = new string[recordCount];
                        string[] dates = new string[recordCount];
                        string[] fuelTypes = new string[recordCount];
                        decimal[] literPrices = new decimal[recordCount];
                        decimal[] cubicPrices = new decimal[recordCount];
                        string[] stations = new string[recordCount];
                        string[] notes = new string[recordCount];

                        // Parse all data - handle empty/null fule_range
                        for (int i = 0; i < recordCount; i++)
                        {
                            DataRow row = dt.Rows[i];

                            decimal odometer = 0;
                            string odometerStr = row["fule_range"].ToString().Trim();
                            if (!string.IsNullOrEmpty(odometerStr))
                            {
                                decimal.TryParse(odometerStr, out odometer);
                            }

                            odometers[i] = odometer;
                            litersArray[i] = Convert.ToDecimal(row["liter_num"]);
                            cubicArray[i] = Convert.ToDecimal(row["cubic_meters"]);
                            transactionNumbers[i] = row["transaction_number"].ToString();
                            dates[i] = row["reg_date_formatted"].ToString();
                            fuelTypes[i] = row["fuel_type"].ToString();
                            literPrices[i] = Convert.ToDecimal(row["liter_price"]);
                            cubicPrices[i] = Convert.ToDecimal(row["cubic_price"]);
                            stations[i] = row["petrol_station_location"].ToString();
                            notes[i] = row["note"].ToString();
                        }

                        // Calculate consumption rates for each record
                        for (int i = 0; i < recordCount; i++)
                        {
                            decimal liters = litersArray[i];
                            decimal cubicMeters = cubicArray[i];
                            decimal literPrice = literPrices[i];
                            decimal cubicPrice = cubicPrices[i];

                            // Calculate distance: current odometer - PREVIOUS odometer
                            // (the fuel added at THIS fill-up is what was consumed since the last one)
                            decimal distance = 0;
                            if (i > 0 && odometers[i] > 0 && odometers[i - 1] > 0)
                            {
                                if (odometers[i] > odometers[i - 1])
                                {
                                    distance = odometers[i] - odometers[i - 1];
                                }
                            }

                            // Calculate total fuel units (including gas)
                            decimal totalFuelUnits = liters;
                            if (cubicMeters > 0)
                            {
                                if (hasNaturalGas)
                                {
                                    totalFuelUnits = liters + (cubicMeters * GasEnergyEquivalent);
                                }
                                else
                                {
                                    totalFuelUnits = liters + cubicMeters;
                                }
                            }

                            // Calculate consumption rate
                            decimal consumptionRate = 0;
                            if (distance > 0 && totalFuelUnits > 0)
                            {
                                consumptionRate = Math.Round(distance / totalFuelUnits, 2);
                                totalDistance += distance;
                                sumConsumption += consumptionRate;
                                validConsumptionCount++;

                                if (bestConsumption == 0 || consumptionRate > bestConsumption)
                                    bestConsumption = consumptionRate;

                                if (worstConsumption == 0 || consumptionRate < worstConsumption)
                                    worstConsumption = consumptionRate;
                            }

                       

                            // Calculate total cost for this entry
                            decimal totalCostRow = (liters * literPrice) + (cubicMeters * cubicPrice);
                            totalLiters += liters;
                            totalCost += totalCostRow;

                            // Add row to display table
                            DataRow displayRow = displayDt.NewRow();
                            displayRow["رقم_المعاملة"] = transactionNumbers[i];
                            displayRow["التاريخ"] = dates[i];
                            displayRow["نوع_الوقود"] = fuelTypes[i];
                            displayRow["اللترات"] = liters;
                            displayRow["سعر_اللتر"] = literPrice;
                            displayRow["عدد_الأمتار_المكعبة"] = cubicMeters;
                            displayRow["سعر_المتر_المكعب"] = cubicPrice;
                            displayRow["التكلفة"] = totalCostRow;
                            displayRow["المسافة"] = distance;
                            displayRow["معدل_الاستهلاك"] = consumptionRate;
                            displayRow["المحطة"] = stations[i];
                            displayRow["ملاحظات"] = notes[i];

                            displayDt.Rows.Add(displayRow);
                        }

                        // Bind to GridView (display in descending order - newest first)
                        DataView sortedView = displayDt.DefaultView;
                        sortedView.Sort = "التاريخ DESC";
                        gvFuel.DataSource = sortedView;
                        gvFuel.DataBind();

                        // Update summary labels
                        lblTotalLiters.Text = totalLiters.ToString("N2");
                        lblTotalCost.Text = totalCost.ToString("N2");

                        if (validConsumptionCount > 0)
                        {
                            decimal avgConsumption = sumConsumption / validConsumptionCount;
                            lblAvgConsumption.Text = avgConsumption.ToString("N2");
                            lblBestConsumption.Text = bestConsumption.ToString("N2");
                            lblWorstConsumption.Text = worstConsumption.ToString("N2");
                            lblConsumptionUnit.Text = hasNaturalGas ? "كم/لتر مكافئ" : "كم/لتر";
                        }
                        else
                        {
                            lblAvgConsumption.Text = "0.00";
                            lblBestConsumption.Text = "0.00";
                            lblWorstConsumption.Text = "0.00";
                        }
                    }
                    else
                    {
                        gvFuel.DataSource = null;
                        gvFuel.DataBind();
                        ResetTotals();
                        ShowMessage("لا توجد بيانات للسيارة في الفترة المحددة");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading fuel data: " + ex.Message);
                    ShowMessage("خطأ في تحميل بيانات الوقود: " + ex.Message);
                }
            }
        }

        private void LoadNaturalGasData(int vehicleId, DateTime? fromDate, DateTime? toDate)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ISNULL(SUM(ISNULL(fc.cubic_meters, 0)), 0) as total_cubic_meters,
                            ISNULL(SUM(ISNULL(fc.cubic_meters, 0) * ISNULL(fc.cubic_price, 0)), 0) as total_gas_cost
                        FROM fuel_consumption fc
                        WHERE fc.car_details = @vehicleId
                        AND (fc.deleted = 0 OR fc.deleted IS NULL)
                        AND (fc.hide = 0 OR fc.hide IS NULL)";

                    if (fromDate.HasValue) query += " AND fc.reg_date >= @fromDate";
                    if (toDate.HasValue) query += " AND fc.reg_date <= @toDate";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                    if (fromDate.HasValue) cmd.Parameters.AddWithValue("@fromDate", fromDate.Value);
                    if (toDate.HasValue) cmd.Parameters.AddWithValue("@toDate", toDate.Value);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal totalCubicMeters = reader["total_cubic_meters"] != DBNull.Value ? Convert.ToDecimal(reader["total_cubic_meters"]) : 0;
                        decimal totalGasCost = reader["total_gas_cost"] != DBNull.Value ? Convert.ToDecimal(reader["total_gas_cost"]) : 0;

                        lblTotalCubicMeters.Text = totalCubicMeters.ToString("N2");
                        lblTotalGasCost.Text = totalGasCost.ToString("N2");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading natural gas data: " + ex.Message);
                }
            }
        }

        private void LoadCardTransactions(int vehicleId, DateTime? fromDate, DateTime? toDate)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            ISNULL(t.transaction_number, 'N/A') as 'رقم_المعاملة',
                            CONVERT(VARCHAR, t.transaction_date, 23) as 'التاريخ',
                            ISNULL(c.card_number, 'N/A') as 'رقم_الكارت',
                            t.amount as 'المبلغ',
                            t.transaction_type as 'نوع_المعاملة',
                            ISNULL(cn.letter1 + ' ' + cn.letter2 + ISNULL(' ' + cn.letter3, '') + ' ' + cn.plate_numbers, 'بدون سيارة') as 'السيارة'
                        FROM fuel_card_transactions t
                        INNER JOIN fuel_cards c ON t.card_id = c.card_id
                        LEFT JOIN main_vehicle_details v ON t.v_id = v.v_id
                        LEFT JOIN car_numbers cn ON v.v_num = cn.car_number_id
                        WHERE t.v_id = @vehicleId";

                    if (fromDate.HasValue) query += " AND t.transaction_date >= @fromDate";
                    if (toDate.HasValue) query += " AND t.transaction_date <= @toDate";
                    query += " ORDER BY t.transaction_date DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                    if (fromDate.HasValue) cmd.Parameters.AddWithValue("@fromDate", fromDate.Value);
                    if (toDate.HasValue) cmd.Parameters.AddWithValue("@toDate", toDate.Value);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvCardTransactions.DataSource = dt;
                        gvCardTransactions.DataBind();
                        CalculateCardTotals(dt);
                    }
                    else
                    {
                        gvCardTransactions.DataSource = null;
                        gvCardTransactions.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading card transactions: " + ex.Message);
                }
            }
        }

        private void CalculateCardTotals(DataTable dt)
        {
            try
            {
                decimal totalDeposits = 0;
                decimal totalWithdrawals = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string transactionType = row["نوع_المعاملة"].ToString();
                    decimal amount = Convert.ToDecimal(row["المبلغ"]);

                    if (transactionType == "إيداع")
                        totalDeposits += amount;
                    else if (transactionType == "سحب" || transactionType == "تعبئة")
                        totalWithdrawals += amount;
                }

                lblCardDeposits.Text = totalDeposits.ToString("N2");
                lblCardWithdrawals.Text = totalWithdrawals.ToString("N2");

                decimal cardBalance = totalDeposits - totalWithdrawals;
                lblCardBalance.Text = cardBalance.ToString("N2");

                // FIX: "مصروف الكارت" = actual spending = withdrawals, not deposits
                lblCardAmount.Text = totalWithdrawals.ToString("N2");

                if (decimal.TryParse(lblTotalCost.Text, out decimal totalCost))
                {
                    // FIX: compare fuel cost vs actual card spending (withdrawals), not deposits
                    decimal difference = totalCost - totalWithdrawals;
                    lblDifference.Text = difference.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error calculating card totals: " + ex.Message);
            }
        }

        protected void gvFuel_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.Cells.Count > 9)
            {
                var consumptionCell = e.Row.Cells[9];
                if (consumptionCell.Text != "0.00" && !string.IsNullOrEmpty(consumptionCell.Text))
                {
                    if (decimal.TryParse(consumptionCell.Text, out decimal rate))
                    {
                        if (rate >= 10) e.Row.CssClass = "table-success";
                        else if (rate <= 5 && rate > 0) e.Row.CssClass = "table-warning";
                    }
                }

                if (e.Row.Cells.Count > 5)
                {
                    var cubicMetersCell = e.Row.Cells[5];
                    if (cubicMetersCell.Text != "0.00" && !string.IsNullOrEmpty(cubicMetersCell.Text))
                    {
                        if (decimal.TryParse(cubicMetersCell.Text, out decimal cubicMeters) && cubicMeters > 0)
                            e.Row.CssClass = "table-info";
                    }
                }
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvFuel.Rows.Count == 0)
                {
                    ShowMessage("لا توجد بيانات للتصدير");
                    return;
                }

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=FuelReport.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);

                hw.WriteLine("<table border='1' dir='rtl'>");
                hw.WriteLine("<tr><th colspan='12' style='font-size:20px;text-align:center;background:#a50d25;color:white'>تقرير استهلاك الوقود</th></tr>");
                hw.WriteLine($"<tr><td colspan='12' style='text-align:center'>السيارة: {ddlVehicle.SelectedItem.Text}</td></tr>");
                hw.WriteLine($"<tr><td colspan='12' style='text-align:center'>الفترة: {txtFrom.Text} إلى {txtTo.Text}</td></tr>");
                hw.WriteLine("<tr>");
                hw.WriteLine("<th>رقم المعاملة</th><th>التاريخ</th><th>نوع الوقود</th><th>اللترات</th><th>سعر اللتر</th>");
                hw.WriteLine("<th>م³</th><th>سعر المتر</th><th>التكلفة</th><th>المسافة (كم)</th><th>معدل الاستهلاك</th><th>المحطة</th><th>ملاحظات</th>");
                hw.WriteLine("</tr>");

                foreach (GridViewRow row in gvFuel.Rows)
                {
                    hw.WriteLine("<tr>");
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        hw.WriteLine("<td>" + row.Cells[i].Text + "&nbsp;</td>");
                    }
                    hw.WriteLine("</tr>");
                }

                hw.WriteLine($"<tr><td colspan='6'><b>إجمالي اللترات:</b></td><td colspan='6'><b>{lblTotalLiters.Text}</b></td></tr>");
                hw.WriteLine($"<tr><td colspan='6'><b>إجمالي التكلفة:</b></td><td colspan='6'><b>{lblTotalCost.Text} ج</b></td></tr>");
                hw.WriteLine($"<tr><td colspan='6'><b>متوسط الاستهلاك:</b></td><td colspan='6'><b>{lblAvgConsumption.Text} كم/لتر مكافئ</b></td></tr>");
                hw.WriteLine($"<tr><td colspan='6'><b>أفضل معدل استهلاك:</b></td><td colspan='6'><b>{lblBestConsumption.Text} كم/لتر</b></td></tr>");
                hw.WriteLine($"<tr><td colspan='6'><b>أسوأ معدل استهلاك:</b></td><td colspan='6'><b>{lblWorstConsumption.Text} كم/لتر</b></td></tr>");
                hw.WriteLine("</table>");

                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage("خطأ في تصدير البيانات: " + ex.Message);
            }
        }

        private void ShowMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "showMessage", script, true);
        }
    }
}