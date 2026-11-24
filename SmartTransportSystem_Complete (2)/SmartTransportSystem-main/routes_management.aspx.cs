using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace car_WebApplication1
{
    public partial class RoutesManagement : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        private const string PageTag = "route_management";
        private const string CatalogTag = "lookup_management";
        private const string DriverPosition = "سائق";
        private const string KeySep = "||";

        // Max items shown in the list per page
        private const int PageSize = 6;

        // =====================================================================
        //  SCHEMA SETTINGS - verify these against your real database
        // =====================================================================
        private const string WO_Table = "operation_command";
        private const string WO_CarCol = "v_number";
        private const string WO_DriverCol = "driver_name";
        private const string WO_ExtraFilter = "";

        private const string CarModelExpr = "LTRIM(RTRIM(ISNULL(ct.car_brand, '') + ' ' + ISNULL(ct.car_type_name, '')))";
        private const string CarBodyExpr = "cbt.body_name";
        private const string CarYearExpr = "v.v_year";
        private const string CarPassengersExpr = "v.passenger";

        private const string CarDisplaySql =
            "(ISNULL(cn.letter1 + ' ', '') + ISNULL(cn.letter2 + ' ', '') + ISNULL(cn.letter3 + ' ', '') + ISNULL(CAST(cn.plate_numbers AS NVARCHAR(50)), ''))";

        private static string ActiveWorkOrderCondition()
        {
            return "w.deleted = 0 AND w.hide = 0 AND w.trip_status = 0";
        }

        private int EditId
        {
            get { return ViewState["EditId"] == null ? 0 : (int)ViewState["EditId"]; }
            set { ViewState["EditId"] = value; }
        }

        private string SearchText
        {
            get { return ViewState["SearchText"] as string ?? ""; }
            set { ViewState["SearchText"] = value ?? ""; }
        }

        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] == null ? 0 : (int)ViewState["CurrentPage"]; }
            set { ViewState["CurrentPage"] = value; }
        }

        // =====================================================================
        //  SEARCH BOX ACCESS (works even if the designer file has no txtSearch)
        // =====================================================================
        private static Control FindControlRecursive(Control root, string id)
        {
            if (root == null) return null;
            if (root.ID == id) return root;
            foreach (Control child in root.Controls)
            {
                Control found = FindControlRecursive(child, id);
                if (found != null) return found;
            }
            return null;
        }

        private string ReadSearchBox()
        {
            TextBox tb = FindControlRecursive(this, "txtSearch") as TextBox;
            if (tb != null) return (tb.Text ?? "").Trim();

            foreach (string k in Request.Form.AllKeys)
            {
                if (k != null && k.EndsWith("txtSearch", StringComparison.Ordinal))
                    return (Request.Form[k] ?? "").Trim();
            }
            return "";
        }

        private void ClearSearchBox()
        {
            TextBox tb = FindControlRecursive(this, "txtSearch") as TextBox;
            if (tb != null) tb.Text = "";
        }

        // =====================================================================
        //  PAGE EVENTS
        // =====================================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRouteList();
                BindCarsAndDrivers();
                LoadAllPlacements();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterCarData();
        }

        // =====================================================================
        //  HELPERS
        // =====================================================================
        private static string ToJs(object value)
        {
            return _json.Serialize(value).Replace("<", "\\u003c");
        }

        private static string RouteKey(string name, string start, string end)
        {
            return name + KeySep + start + KeySep + end;
        }

        private static string Str(object o)
        {
            return o == null || o == DBNull.Value ? "" : o.ToString();
        }

        private static object DbOrNull(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim();
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

        private void GetEditCurrent(out int? carId, out int? driverId)
        {
            carId = null; driverId = null;
            if (EditId <= 0) return;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT car_id, driver_id FROM routes WHERE route_id = @Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = EditId;
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        if (r["car_id"] != DBNull.Value) carId = Convert.ToInt32(r["car_id"]);
                        if (r["driver_id"] != DBNull.Value) driverId = Convert.ToInt32(r["driver_id"]);
                    }
                }
            }
        }

        // =====================================================================
        //  DATA SOURCES
        // =====================================================================

        // Route catalog: routes added in lookup_management + old route_master rows
        private DataTable GetRouteMasterTable()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT route_name, start_location, end_location
                FROM (
                    SELECT route_name,
                           ISNULL(start_loc, '') AS start_location,
                           ISNULL(end_loc, '')   AS end_location
                    FROM routes
                    WHERE is_active = 1
                      AND created_by_page = @CatalogTag
                      AND route_name IS NOT NULL AND route_name <> ''
                    UNION
                    SELECT route_name,
                           ISNULL(start_location, ''),
                           ISNULL(end_location, '')
                    FROM route_master
                    WHERE is_active = 1
                      AND route_name IS NOT NULL AND route_name <> ''
                ) x
                ORDER BY route_name, start_location, end_location", conn))
            {
                cmd.Parameters.AddWithValue("@CatalogTag", CatalogTag);
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        // Available cars: not in a running work order now
        private DataTable GetAvailableCars()
        {
            int? curCar, curDriver;
            GetEditCurrent(out curCar, out curDriver);

            string sql = @"
        SELECT v.v_id,
               " + CarDisplaySql + @" AS car_display,
               " + CarModelExpr + @" AS car_model,
               " + CarBodyExpr + @" AS car_body,
               " + CarYearExpr + @" AS car_year,
               " + CarPassengersExpr + @" AS car_passengers
        FROM main_vehicle_details v
        INNER JOIN car_numbers cn ON v.v_num = cn.car_number_id
        LEFT JOIN car_types ct ON v.v_type = ct.car_type_id
        LEFT JOIN car_body_type cbt ON v.v_model = cbt.body_id
        WHERE v.deleted = 0 AND v.hide = 0
          AND ( v.v_id = @CurCar
                OR NOT EXISTS (SELECT 1 FROM " + WO_Table + @" w
                               WHERE w." + WO_CarCol + @" = v.v_num
                                 AND " + ActiveWorkOrderCondition() + @") )
        ORDER BY cn.plate_numbers";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                SqlParameter p = cmd.Parameters.Add("@CurCar", SqlDbType.Int);
                p.Value = curCar.HasValue ? (object)curCar.Value : DBNull.Value;

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        // Available drivers: not in a running work order and not assigned to an active route
        private DataTable GetAvailableDrivers()
        {
            int? curCar, curDriver;
            GetEditCurrent(out curCar, out curDriver);

            string sql = @"
                SELECT e.employee_id   AS driver_id,
                       e.employee_name AS driver_name,
                       e.phone_number  AS phone
                FROM employees e
                WHERE e.is_active = 1
                  AND LTRIM(RTRIM(e.employee_position)) = @Position
                  AND ( e.employee_id = @CurDriver
                        OR ( NOT EXISTS (SELECT 1 FROM " + WO_Table + @" w
                                         WHERE w." + WO_DriverCol + @" = e.employee_id
                                           AND " + ActiveWorkOrderCondition() + @")
                             AND NOT EXISTS (SELECT 1 FROM routes r
                                             WHERE r.driver_id = e.employee_id
                                               AND r.is_active = 1) ) )
                ORDER BY e.employee_name";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Position", SqlDbType.NVarChar, 150).Value = DriverPosition;
                SqlParameter p = cmd.Parameters.Add("@CurDriver", SqlDbType.Int);
                p.Value = curDriver.HasValue ? (object)curDriver.Value : DBNull.Value;

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        private static string CarText(DataRow row)
        {
            string text = Str(row["car_display"]).Trim();
            return string.IsNullOrEmpty(text) ? "سيارة رقم " + row["v_id"] : text;
        }

        private static string DriverText(DataRow row)
        {
            string text = Str(row["driver_name"]);
            string phone = Str(row["phone"]);
            if (!string.IsNullOrEmpty(phone)) text += " (" + phone + ")";
            return text;
        }

        // =====================================================================
        //  BIND DROPDOWNS
        // =====================================================================
        private void LoadRouteList()
        {
            try
            {
                ddlRoute.Items.Clear();
                ddlRoute.Items.Add(new ListItem("-- اختر الخط --", ""));
                foreach (DataRow row in GetRouteMasterTable().Rows)
                {
                    string name = Str(row["route_name"]);
                    string start = Str(row["start_location"]);
                    string end = Str(row["end_location"]);

                    string text = name;
                    if (start != "" || end != "") text += "  (" + start + " ← " + end + ")";
                    ddlRoute.Items.Add(new ListItem(text, RouteKey(name, start, end)));
                }
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحميل الخطوط: " + ex.Message);
            }
        }

        private void BindCarsAndDrivers()
        {
            try
            {
                ddlCar.Items.Clear();
                ddlCar.Items.Add(new ListItem("-- اختر السيارة --", ""));
                foreach (DataRow row in GetAvailableCars().Rows)
                    ddlCar.Items.Add(new ListItem(CarText(row), row["v_id"].ToString()));

                ddlDriver.Items.Clear();
                ddlDriver.Items.Add(new ListItem("-- اختر السائق --", ""));
                foreach (DataRow row in GetAvailableDrivers().Rows)
                    ddlDriver.Items.Add(new ListItem(DriverText(row), row["driver_id"].ToString()));
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحميل السيارات/السائقين: " + ex.Message);
            }
        }

        private void RegisterCarData()
        {
            try
            {
                var cars = GetAvailableCars().AsEnumerable().Select(r => new
                {
                    id = r["v_id"].ToString(),
                    text = CarText(r),
                    model = Str(r["car_model"]),
                    body = Str(r["car_body"]),
                    year = Str(r["car_year"]),
                    passengers = Str(r["car_passengers"])
                }).ToList();

                ScriptManager.RegisterStartupScript(this, GetType(), "CarData",
                    "carsData = " + ToJs(cars) + ";", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error registering car data: " + ex.Message);
            }
        }

        // =====================================================================
        //  LIST (search + paging)
        // =====================================================================
        private static bool RowMatches(DataRow row, string term)
        {
            string[] cols = { "route_name", "start_loc", "end_loc", "car_display", "driver_display" };
            foreach (string c in cols)
            {
                if (Str(row[c]).IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private void LoadAllPlacements()
        {
            try
            {
                string sql = @"
                    SELECT r.route_id, r.route_name, r.start_loc, r.end_loc,
                           ISNULL(NULLIF(LTRIM(RTRIM(" + CarDisplaySql + @")), ''), N'غير محدد') AS car_display,
                           ISNULL(e.employee_name, N'غير محدد') AS driver_display
                    FROM routes r
                    LEFT JOIN main_vehicle_details v ON v.v_id = r.car_id
                    LEFT JOIN car_numbers cn ON v.v_num = cn.car_number_id
                    LEFT JOIN employees e ON e.employee_id = r.driver_id
                    WHERE r.created_by_page = @Tag
                    ORDER BY r.route_name, r.start_loc, r.route_id";

                DataTable all = new DataTable();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Tag", PageTag);
                    new SqlDataAdapter(cmd).Fill(all);
                }

                string term = (SearchText ?? "").Trim();
                List<DataRow> filtered = all.AsEnumerable()
                    .Where(r => term.Length == 0 || RowMatches(r, term))
                    .ToList();

                int total = filtered.Count;
                int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));

                if (CurrentPage >= totalPages) CurrentPage = totalPages - 1;
                if (CurrentPage < 0) CurrentPage = 0;

                List<DataRow> pageRows = filtered.Skip(CurrentPage * PageSize).Take(PageSize).ToList();
                DataTable pageTable = pageRows.Count > 0 ? pageRows.CopyToDataTable() : all.Clone();

                rptAllRoutes.DataSource = pageTable;
                rptAllRoutes.DataBind();

                lblTotalCount.Text = total.ToString(CultureInfo.InvariantCulture);

                lblNoRoutes.Visible = total == 0;
                lblNoRoutes.Text = term.Length > 0 ? "لا توجد نتائج مطابقة للبحث" : "لا توجد تسكينات";

                BindPager(total, totalPages);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading placements: " + ex.Message);
                lblNoRoutes.Visible = true;
                lblNoRoutes.Text = "حدث خطأ في تحميل التسكينات";
                pnlPager.Visible = false;
            }
        }

        private void BindPager(int total, int totalPages)
        {
            pnlPager.Visible = totalPages > 1;
            if (totalPages <= 1) return;

            int start = Math.Max(0, CurrentPage - 2);
            int end = Math.Min(totalPages - 1, start + 4);
            start = Math.Max(0, end - 4);

            DataTable pages = new DataTable();
            pages.Columns.Add("PageIndex", typeof(int));
            pages.Columns.Add("Label", typeof(string));
            pages.Columns.Add("Css", typeof(string));

            for (int i = start; i <= end; i++)
            {
                pages.Rows.Add(i,
                    (i + 1).ToString(CultureInfo.InvariantCulture),
                    i == CurrentPage ? "pager-active" : "");
            }

            rptPager.DataSource = pages;
            rptPager.DataBind();

            btnPrevPage.Enabled = CurrentPage > 0;
            btnNextPage.Enabled = CurrentPage < totalPages - 1;
            btnPrevPage.CssClass = btnPrevPage.Enabled ? "" : "pager-disabled";
            btnNextPage.CssClass = btnNextPage.Enabled ? "" : "pager-disabled";

            int from = CurrentPage * PageSize + 1;
            int to = Math.Min(total, (CurrentPage + 1) * PageSize);
            lblPagerInfo.Text = string.Format(CultureInfo.InvariantCulture,
                "عرض {0} - {1} من {2} | صفحة {3} من {4}", from, to, total, CurrentPage + 1, totalPages);
        }

        // =====================================================================
        //  SEARCH / PAGER EVENTS
        // =====================================================================
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchText = ReadSearchBox();
            CurrentPage = 0;
            LoadAllPlacements();
        }

        protected void BtnResetSearch_Click(object sender, EventArgs e)
        {
            ClearSearchBox();
            SearchText = "";
            CurrentPage = 0;
            LoadAllPlacements();
        }

        protected void BtnPrevPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 0) CurrentPage--;
            LoadAllPlacements();
        }

        protected void BtnNextPage_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            LoadAllPlacements();
        }

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "GoPage") return;

            int page;
            if (int.TryParse(Convert.ToString(e.CommandArgument), out page))
            {
                CurrentPage = page;
                LoadAllPlacements();
            }
        }

        // =====================================================================
        //  SAVE (add / update)
        // =====================================================================
        protected void BtnSaveRoute_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlRoute.SelectedValue)) { ShowMessage("الرجاء اختيار الخط"); return; }
                if (string.IsNullOrEmpty(ddlCar.SelectedValue)) { ShowMessage("الرجاء اختيار السيارة"); return; }
                if (string.IsNullOrEmpty(ddlDriver.SelectedValue)) { ShowMessage("الرجاء اختيار السائق"); return; }

                string[] key = ddlRoute.SelectedValue.Split(new[] { KeySep }, StringSplitOptions.None);
                if (key.Length != 3) { ShowMessage("الخط المختار غير صحيح"); return; }
                string routeName = key[0], startLoc = key[1], endLoc = key[2];

                int carId = int.Parse(ddlCar.SelectedValue, CultureInfo.InvariantCulture);
                int driverId = int.Parse(ddlDriver.SelectedValue, CultureInfo.InvariantCulture);

                DataRow[] carRows = GetAvailableCars().Select("v_id = " + carId);
                if (carRows.Length == 0) { ShowMessage("هذه السيارة غير متاحة (موجودة في مأمورية حالياً)"); BindCarsAndDrivers(); return; }

                DataRow[] driverRows = GetAvailableDrivers().Select("driver_id = " + driverId);
                if (driverRows.Length == 0) { ShowMessage("هذا السائق غير متاح (موجود في مأمورية أو مسكّن على سيارة)"); BindCarsAndDrivers(); return; }

                string driverName = Str(driverRows[0]["driver_name"]);
                string driverPhone = Str(driverRows[0]["phone"]);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // KM: reuse the KM already stored for the same route (if any)
                    decimal? km = null;
                    using (SqlCommand kmCmd = new SqlCommand(@"
                        SELECT MAX(KM) FROM routes
                        WHERE route_name = @N AND ISNULL(start_loc, '') = @S AND ISNULL(end_loc, '') = @E", conn))
                    {
                        kmCmd.Parameters.AddWithValue("@N", routeName);
                        kmCmd.Parameters.AddWithValue("@S", startLoc);
                        kmCmd.Parameters.AddWithValue("@E", endLoc);
                        object o = kmCmd.ExecuteScalar();
                        if (o != null && o != DBNull.Value) km = Convert.ToDecimal(o);
                    }

                    string sql;
                    if (EditId > 0)
                    {
                        sql = @"
                            UPDATE routes
                            SET route_name = @RouteName, start_loc = @StartLoc, end_loc = @EndLoc,
                                KM = ISNULL(@KM, KM),
                                car_id = @CarId, driver_id = @DriverId,
                                driver_name = @DriverName, driver_phone = @DriverPhone
                            WHERE route_id = @RouteId";
                    }
                    else
                    {
                        sql = @"
                            INSERT INTO routes
                            (route_name, start_loc, end_loc, KM, is_active, start_date, end_date,
                             driver_id, car_id, driver_name, driver_phone, created_by_page)
                            VALUES
                            (@RouteName, @StartLoc, @EndLoc, ISNULL(@KM, 0), 1, @StartDate, @EndDate,
                             @DriverId, @CarId, @DriverName, @DriverPhone, @Tag)";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RouteName", routeName);
                        cmd.Parameters.AddWithValue("@StartLoc", DbOrNull(startLoc));
                        cmd.Parameters.AddWithValue("@EndLoc", DbOrNull(endLoc));

                        SqlParameter pKm = cmd.Parameters.Add("@KM", SqlDbType.Decimal);
                        pKm.Precision = 18; pKm.Scale = 2;
                        pKm.Value = km.HasValue ? (object)km.Value : DBNull.Value;

                        cmd.Parameters.AddWithValue("@CarId", carId);
                        cmd.Parameters.AddWithValue("@DriverId", driverId);
                        cmd.Parameters.AddWithValue("@DriverName", DbOrNull(driverName));
                        cmd.Parameters.AddWithValue("@DriverPhone", DbOrNull(driverPhone));

                        if (EditId > 0)
                        {
                            cmd.Parameters.AddWithValue("@RouteId", EditId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@StartDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EndDate", DateTime.Now.AddYears(1));
                            cmd.Parameters.AddWithValue("@Tag", PageTag);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }

                bool wasEdit = EditId > 0;
                ResetForm();
                LoadAllPlacements();
                ShowSuccess(wasEdit ? "تم تعديل التسكين بنجاح" : "تم حفظ التسكين بنجاح");
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء حفظ البيانات: " + ex.Message);
            }
        }

        protected void BtnClearRoute_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            EditId = 0;
            pnlEditMode.Visible = false;
            lblFormTitle.Text = "تسكين سيارة على خط";
            btnSaveRoute.Text = "حفظ التسكين";

            ddlRoute.ClearSelection();
            BindCarsAndDrivers();
        }

        // =====================================================================
        //  EDIT / DELETE (from the list)
        // =====================================================================
        protected void rptAllRoutes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int routeId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out routeId)) return;

            if (e.CommandName == "EditRoute") StartEdit(routeId);
            else if (e.CommandName == "DeleteRoute") DeletePlacement(routeId);
        }

        private void StartEdit(int routeId)
        {
            try
            {
                string routeName = "", startLoc = "", endLoc = "";
                int? carId = null, driverId = null;
                bool found = false;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT route_name, start_loc, end_loc, car_id, driver_id FROM routes WHERE route_id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", routeId);
                    conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            found = true;
                            routeName = Str(r["route_name"]);
                            startLoc = Str(r["start_loc"]);
                            endLoc = Str(r["end_loc"]);
                            if (r["car_id"] != DBNull.Value) carId = Convert.ToInt32(r["car_id"]);
                            if (r["driver_id"] != DBNull.Value) driverId = Convert.ToInt32(r["driver_id"]);
                        }
                    }
                }

                if (!found) { ShowMessage("التسكين غير موجود"); return; }

                EditId = routeId;
                BindCarsAndDrivers();

                string key = RouteKey(routeName, startLoc, endLoc);
                if (ddlRoute.Items.FindByValue(key) == null)
                {
                    string text = routeName;
                    if (startLoc != "" || endLoc != "") text += "  (" + startLoc + " ← " + endLoc + ")";
                    ddlRoute.Items.Add(new ListItem(text, key));
                }
                ddlRoute.ClearSelection();
                ddlRoute.SelectedValue = key;

                ddlCar.ClearSelection();
                if (carId.HasValue && ddlCar.Items.FindByValue(carId.Value.ToString()) != null)
                    ddlCar.SelectedValue = carId.Value.ToString();

                ddlDriver.ClearSelection();
                if (driverId.HasValue && ddlDriver.Items.FindByValue(driverId.Value.ToString()) != null)
                    ddlDriver.SelectedValue = driverId.Value.ToString();

                pnlEditMode.Visible = true;
                lblFormTitle.Text = "تعديل التسكين";
                btnSaveRoute.Text = "حفظ التعديلات";
                lblMessage.Visible = false;
                lblSuccess.Visible = false;

                ScriptManager.RegisterStartupScript(this, GetType(), "scrollTop",
                    "window.scrollTo(0, 0);", true);
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء تحميل بيانات التسكين: " + ex.Message);
            }
        }

        private void DeletePlacement(int routeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            string[] tables = { "vehicle_subscriptions", "subscription_details", "vehicle_route_assignment" };
                            foreach (string table in tables)
                            {
                                if (table == "vehicle_route_assignment" && !TableExists(conn, tx, table)) continue;

                                using (SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM " + table + " WHERE route_id = @Id", conn, tx))
                                {
                                    check.Parameters.AddWithValue("@Id", routeId);
                                    int count = Convert.ToInt32(check.ExecuteScalar());
                                    if (count > 0)
                                    {
                                        tx.Rollback();
                                        ShowMessage("لا يمكن حذف هذا التسكين لأنه مرتبط بـ " + count + " سجل في " + table + ". يرجى حذف السجلات المرتبطة أولاً.");
                                        return;
                                    }
                                }
                            }

                            using (SqlCommand del = new SqlCommand("DELETE FROM routes WHERE route_id = @Id AND created_by_page = @Tag", conn, tx))
                            {
                                del.Parameters.AddWithValue("@Id", routeId);
                                del.Parameters.AddWithValue("@Tag", PageTag);
                                del.ExecuteNonQuery();
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                if (EditId == routeId) EditId = 0;
                ResetForm();
                LoadAllPlacements();
                ShowSuccess("تم حذف التسكين بنجاح");
            }
            catch (Exception ex)
            {
                ShowMessage("حدث خطأ أثناء حذف التسكين: " + ex.Message);
            }
        }

        private bool TableExists(SqlConnection conn, SqlTransaction tx, string tableName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM sys.tables WHERE name = @TableName", conn, tx))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}