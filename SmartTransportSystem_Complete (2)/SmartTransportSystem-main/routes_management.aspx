<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="routes_management.aspx.cs"
    Inherits="car_WebApplication1.RoutesManagement"
    MasterPageFile="~/Site.master"
    Title="تسكين السيارة/الخط" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<script runat="server">

    private readonly string _connectionString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
    private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

    private const string PageTag = "route_management";
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

    // --- Car data expressions ---
    // aliases: v = main_vehicle_details, ct = car_types, cbt = car_body_type
    // (same source as vehicle_management: v_type -> car_types, v_model -> car_body_type)

    // الموديل = نوع السيارة (العلامة التجارية) + الموديل
    private const string CarModelExpr =
        "LTRIM(RTRIM(ISNULL(ct.car_brand, '') + ' ' + ISNULL(ct.car_type_name, '')))";
    // هيكل السيارة
    private const string CarBodyExpr = "cbt.body_name";
    private const string CarYearExpr = "v.v_year";
    private const string CarPassengersExpr = "v.passenger";

    private const string CarDisplaySql =
        "(ISNULL(cn.letter1 + ' ', '') + ISNULL(cn.letter2 + ' ', '') + ISNULL(cn.letter3 + ' ', '') + ISNULL(CAST(cn.plate_numbers AS NVARCHAR(50)), ''))";

    private static string ActiveWorkOrderCondition()
    {
        return "w.deleted = 0 AND w.hide = 0 AND w.trip_status = 0";
    }

    // ViewState-backed: the route row being edited (0 = adding new)
    private int EditId
    {
        get { return ViewState["EditId"] == null ? 0 : (int)ViewState["EditId"]; }
        set { ViewState["EditId"] = value; }
    }

    // ViewState-backed: list filters (each one has its own dropdown)
    private string FilterRoute
    {
        get { return ViewState["FilterRoute"] as string ?? ""; }
        set { ViewState["FilterRoute"] = value ?? ""; }
    }
    private string FilterCar
    {
        get { return ViewState["FilterCar"] as string ?? ""; }
        set { ViewState["FilterCar"] = value ?? ""; }
    }
    private string FilterDriver
    {
        get { return ViewState["FilterDriver"] as string ?? ""; }
        set { ViewState["FilterDriver"] = value ?? ""; }
    }

    // ViewState-backed: current page index of the list (0-based)
    private int CurrentPage
    {
        get { return ViewState["CurrentPage"] == null ? 0 : (int)ViewState["CurrentPage"]; }
        set { ViewState["CurrentPage"] = value; }
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

    private static string RouteText(string name, string start, string end)
    {
        string text = name;
        if (start != "" || end != "") text += "  (" + start + " ← " + end + ")";
        return text;
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

    // car_id / driver_id of the row being edited (so they stay selectable while editing)
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
    private DataTable GetRouteMasterTable()
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT DISTINCT route_name, start_location, end_location
            FROM route_master
            WHERE is_active = 1
              AND route_name IS NOT NULL AND route_name != ''
            ORDER BY route_name, start_location, end_location", conn))
        {
            DataTable dt = new DataTable();
            new SqlDataAdapter(cmd).Fill(dt);
            return dt;
        }
    }

    // السيارات المتاحة: ليست في مأمورية سارية الآن
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
    //  BIND DROPDOWNS (form)
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
                ddlRoute.Items.Add(new ListItem(RouteText(name, start, end), RouteKey(name, start, end)));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Error loading routes: " + ex.Message);
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

    // Car data for the JS (shown when a car is chosen). Registered on EVERY request.
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
    //  LIST (3 filters + paging, max 6 per page)
    // =====================================================================

    // Fills one filter dropdown with the distinct values found in the list.
    // Returns the selected value (falls back to "" = الكل if it no longer exists).
    private string FillFilter(DropDownList ddl, IEnumerable<string> values, string selected, string allText)
    {
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem(allText, ""));

        foreach (string v in values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .OrderBy(x => x, StringComparer.CurrentCulture))
        {
            ddl.Items.Add(new ListItem(v, v));
        }

        ddl.ClearSelection();
        ListItem found = ddl.Items.FindByValue(selected ?? "");
        if (found != null) { found.Selected = true; return found.Value; }

        ddl.Items[0].Selected = true;
        return "";
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

            // text used by the الخط filter: "name (start ← end)"
            all.Columns.Add("route_display", typeof(string));
            foreach (DataRow r in all.Rows)
                r["route_display"] = RouteText(Str(r["route_name"]), Str(r["start_loc"]), Str(r["end_loc"]));

            // ---- fill the 3 filter dropdowns from the data ----
            FilterRoute = FillFilter(ddlFilterRoute,
                all.AsEnumerable().Select(r => Str(r["route_display"])), FilterRoute, "الكل");
            FilterCar = FillFilter(ddlFilterCar,
                all.AsEnumerable().Select(r => Str(r["car_display"])), FilterCar, "الكل");
            FilterDriver = FillFilter(ddlFilterDriver,
                all.AsEnumerable().Select(r => Str(r["driver_display"])), FilterDriver, "الكل");

            // ---- apply the filters (all selected filters must match) ----
            string fr = FilterRoute, fc = FilterCar, fd = FilterDriver;
            List<DataRow> filtered = all.AsEnumerable()
                .Where(r => (fr == "" || Str(r["route_display"]) == fr)
                         && (fc == "" || Str(r["car_display"]) == fc)
                         && (fd == "" || Str(r["driver_display"]) == fd))
                .ToList();

            int total = filtered.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));

            if (CurrentPage >= totalPages) CurrentPage = totalPages - 1;
            if (CurrentPage < 0) CurrentPage = 0;

            // ---- paging ----
            List<DataRow> pageRows = filtered.Skip(CurrentPage * PageSize).Take(PageSize).ToList();
            DataTable pageTable = pageRows.Count > 0 ? pageRows.CopyToDataTable() : all.Clone();

            rptAllRoutes.DataSource = pageTable;
            rptAllRoutes.DataBind();

            lblTotalCount.Text = total.ToString(CultureInfo.InvariantCulture);

            bool hasFilter = fr != "" || fc != "" || fd != "";
            lblNoRoutes.Visible = total == 0;
            lblNoRoutes.Text = hasFilter ? "لا توجد نتائج مطابقة للبحث" : "لا توجد تسكينات";

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
    //  FILTER / PAGER EVENTS
    // =====================================================================
    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        FilterRoute = ddlFilterRoute.SelectedValue;
        FilterCar = ddlFilterCar.SelectedValue;
        FilterDriver = ddlFilterDriver.SelectedValue;
        CurrentPage = 0;
        LoadAllPlacements();
    }

    protected void BtnResetSearch_Click(object sender, EventArgs e)
    {
        FilterRoute = "";
        FilterCar = "";
        FilterDriver = "";
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
        CurrentPage++;               // clamped inside LoadAllPlacements
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
                ddlRoute.Items.Add(new ListItem(RouteText(routeName, startLoc, endLoc), key));
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

</script>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .page-title { font-weight: bold; color: #000; text-align: center; margin-bottom: 30px; font-size: 32px; }
    .main-box { border: 2px solid #000; border-radius: 14px; padding: 30px; background-color: #fff; margin-bottom: 20px; }
    .section-header { background: linear-gradient(90deg, #ce1126, #ffffff); padding: 12px 18px; border-radius: 8px; margin-bottom: 20px; font-weight: bold; color: #000; overflow: auto; font-size: 18px; }
    .form-label { font-weight: bold; font-size: 15px; margin-bottom: 6px; }
    .required-field::after { content: " *"; color: #ce1126; font-weight: bold; }

    .btn-save { background-color: #a50d25; border-color: #c8102e; color: white; font-weight: bold; height: 52px; width: 100%; font-size: 17px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-save:hover { background-color: #8a0a1e; }
    .btn-clear { background-color: #6c757d; color: #fff; font-weight: bold; height: 52px; width: 100%; font-size: 17px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-clear:hover { background-color: #5a6268; }

    .btn-edit-route { background-color: #ffc107; color: #212529; border: none; padding: 6px 12px; border-radius: 5px; font-size: 13px; cursor: pointer; text-decoration: none; display: inline-block; }
    .btn-edit-route:hover { background-color: #e0a800; color: #212529; }
    .btn-delete-route { background-color: #dc3545; color: white; border: none; padding: 6px 12px; border-radius: 5px; font-size: 13px; cursor: pointer; text-decoration: none; display: inline-block; }
    .btn-delete-route:hover { background-color: #c82333; color: white; }

    .route-item { background: #f8f9fa; border-right: 4px solid #a50d25; padding: 12px 18px; margin-bottom: 10px; border-radius: 5px; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; }
    .route-item:hover { background: #e9ecef; }
    .route-info { flex: 1; }
    .route-name { font-weight: bold; color: #a50d25; font-size: 18px; }
    .route-stats { margin-top: 8px; display: flex; gap: 12px; flex-wrap: wrap; }
    .stat-badge { background-color: #e3f2fd; padding: 3px 10px; border-radius: 12px; font-size: 12px; color: #0d6efd; }
    .route-actions { margin-right: 10px; white-space: nowrap; }

    .form-control { font-size: 15px; padding: 10px 14px; height: 42px; }
    select.form-control { height: 42px; padding: 8px 12px; }
    input[readonly].form-control { background-color: #f1f3f5; }

    .alert { font-size: 16px; padding: 15px 20px; }
    .badge-secondary { background-color: #6c757d; color: white; padding: 5px 10px; border-radius: 20px; font-size: 12px; }

    .car-info-box { background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 8px; padding: 15px; margin-top: 15px; }
    .car-info-title { font-weight: bold; color: #a50d25; margin-bottom: 10px; }
    .edit-banner { background: #fff3cd; color: #856404; border: 1px solid #ffeeba; border-radius: 6px; padding: 10px 15px; margin-bottom: 15px; font-weight: bold; }

    /* ===== Filter bar above the list (3 dropdowns, each with its own search) ===== */
    .filter-box { background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 8px; padding: 15px; margin-bottom: 15px; }
    .filter-box .form-label { color: #a50d25; }
    .filter-buttons { display: flex; gap: 8px; margin-top: 12px; }
    .btn-search { background-color: #a50d25; color: #fff; font-weight: bold; border: none; border-radius: 6px; height: 40px; padding: 0 22px; cursor: pointer; }
    .btn-search:hover { background-color: #8a0a1e; }
    .btn-search-reset { background-color: #6c757d; color: #fff; font-weight: bold; border: none; border-radius: 6px; height: 40px; padding: 0 18px; cursor: pointer; }
    .btn-search-reset:hover { background-color: #5a6268; }

    /* ===== Pager ===== */
    .pager-bar { display: flex; justify-content: center; align-items: center; gap: 6px; flex-wrap: wrap; margin-top: 15px; }
    .pager-bar a { display: inline-block; min-width: 38px; text-align: center; padding: 7px 12px; border: 1px solid #ced4da; border-radius: 6px; background: #fff; color: #a50d25; text-decoration: none; font-weight: bold; font-size: 14px; }
    .pager-bar a:hover { background: #f1f3f5; }
    .pager-bar a.pager-active { background: #a50d25; color: #fff; border-color: #a50d25; cursor: default; }
    .pager-bar a.aspNetDisabled, .pager-bar a.pager-disabled { color: #adb5bd; background: #f8f9fa; cursor: not-allowed; pointer-events: none; }
    .pager-info { text-align: center; color: #6c757d; font-size: 13px; margin-top: 8px; }

    /* ===== Select2 (searchable dropdown) ===== */
    .select2-container { direction: rtl; text-align: right; }
    .select2-container--default .select2-selection--single { height: 42px; border: 1px solid #ced4da; border-radius: 6px; }
    .select2-container--default .select2-selection--single .select2-selection__rendered { line-height: 40px; padding-right: 14px; padding-left: 28px; font-size: 15px; }
    .select2-container--default .select2-selection--single .select2-selection__arrow { height: 40px; right: auto; left: 6px; }
    .select2-container--default .select2-selection--single .select2-selection__clear { float: left; margin-left: 10px; }
    .select2-dropdown { direction: rtl; text-align: right; }
    .select2-search--dropdown .select2-search__field { direction: rtl; }

    @media (max-width: 768px) {
        .page-title { font-size: 24px; }
        .main-box { padding: 15px !important; }
        .route-item { flex-direction: column; align-items: flex-start; }
        .route-actions { margin-top: 10px; margin-right: 0; }
        .btn-save, .btn-clear { height: 46px !important; font-size: 15px !important; }
        .section-header { font-size: 15px !important; padding: 10px 14px !important; }
    }
</style>

<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.13/css/select2.min.css" />

<div class="container mt-4" dir="rtl">
    <div class="text-center mb-4">
        <h2 class="page-title">تسكين السيارة/الخط</h2>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger text-center d-block" Visible="false"></asp:Label>
    <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success text-center d-block" Visible="false"></asp:Label>

    <div class="main-box">
        <div class="section-header">
            <i class="fas fa-plus-circle"></i> <asp:Label ID="lblFormTitle" runat="server" Text="تسكين سيارة على خط"></asp:Label>
        </div>

        <asp:Panel ID="pnlEditMode" runat="server" Visible="false" CssClass="edit-banner">
            <i class="fas fa-edit"></i> أنت الآن في وضع تعديل تسكين موجود — اضغط "مسح الحقول" للخروج من وضع التعديل
        </asp:Panel>

        <div class="row g-3">
            <!-- الخط -->
            <div class="col-md-12">
                <label class="form-label required-field">الخط</label>
                <asp:DropDownList ID="ddlRoute" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>

            <!-- السيارة -->
            <div class="col-md-6">
                <label class="form-label required-field">السيارة</label>
                <asp:DropDownList ID="ddlCar" runat="server" CssClass="form-control"></asp:DropDownList>
                <small class="text-muted">تظهر فقط السيارات غير الموجودة في مأمورية حالياً</small>
            </div>

            <!-- السائق -->
            <div class="col-md-6">
                <label class="form-label required-field">السائق</label>
                <asp:DropDownList ID="ddlDriver" runat="server" CssClass="form-control"></asp:DropDownList>
                <small class="text-muted">يظهر فقط السائقون غير الموجودين في مأمورية أو سيارة حالياً</small>
            </div>
        </div>

        <!-- بيانات السيارة (تظهر عند اختيار السيارة) -->
        <div class="car-info-box" id="carInfoBox" style="display:none;">
            <div class="car-info-title"><i class="fas fa-car"></i> بيانات السيارة</div>
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label">الموديل</label>
                    <input type="text" id="infoModel" class="form-control" readonly="readonly" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">هيكل السيارة</label>
                    <input type="text" id="infoBody" class="form-control" readonly="readonly" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">سنة الصنع</label>
                    <input type="text" id="infoYear" class="form-control" readonly="readonly" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">عدد الركاب</label>
                    <input type="text" id="infoPassengers" class="form-control" readonly="readonly" />
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-md-6">
                <asp:Button ID="btnSaveRoute" runat="server" Text="حفظ التسكين" CssClass="btn btn-save" OnClick="BtnSaveRoute_Click" />
            </div>
            <div class="col-md-6">
                <asp:Button ID="btnClearRoute" runat="server" Text="مسح الحقول" CssClass="btn btn-clear" OnClick="BtnClearRoute_Click" />
            </div>
        </div>

        <div class="mt-4">
            <div class="section-header">
                <i class="fas fa-list"></i> جميع التسكينات
                <span class="badge badge-secondary"><asp:Label ID="lblTotalCount" runat="server" Text="0"></asp:Label></span>
            </div>

            <!-- ===== Filters: 3 fields, each is a dropdown with its own search ===== -->
            <div class="filter-box">
                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label"><i class="fas fa-route"></i> بحث بالخط</label>
                        <asp:DropDownList ID="ddlFilterRoute" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label"><i class="fas fa-car"></i> بحث بالسيارة</label>
                        <asp:DropDownList ID="ddlFilterCar" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label"><i class="fas fa-user"></i> بحث بالسائق</label>
                        <asp:DropDownList ID="ddlFilterDriver" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
                <div class="filter-buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="بحث" CssClass="btn-search" CausesValidation="false" OnClick="BtnSearch_Click" />
                    <asp:Button ID="btnResetSearch" runat="server" Text="مسح الفلاتر" CssClass="btn-search-reset" CausesValidation="false" OnClick="BtnResetSearch_Click" />
                </div>
            </div>

            <asp:Repeater ID="rptAllRoutes" runat="server" OnItemCommand="rptAllRoutes_ItemCommand">
                <ItemTemplate>
                    <div class="route-item">
                        <div class="route-info">
                            <div class="route-name">
                                <i class="fas fa-route"></i> <%# Server.HtmlEncode(Convert.ToString(Eval("route_name"))) %>
                            </div>
                            <div class="route-stats">
                                <span class="stat-badge">
                                    <i class="fas fa-map-marker-alt"></i>
                                    <%# Server.HtmlEncode(Convert.ToString(Eval("start_loc"))) %>
                                    <i class="fas fa-arrow-left"></i>
                                    <%# Server.HtmlEncode(Convert.ToString(Eval("end_loc"))) %>
                                </span>
                                <span class="stat-badge">
                                    <i class="fas fa-car"></i>
                                    السيارة: <%# Server.HtmlEncode(Convert.ToString(Eval("car_display"))) %>
                                </span>
                                <span class="stat-badge">
                                    <i class="fas fa-user"></i>
                                    السائق: <%# Server.HtmlEncode(Convert.ToString(Eval("driver_display"))) %>
                                </span>
                            </div>
                        </div>
                        <div class="route-actions">
                            <asp:LinkButton ID="btnEditRoute" runat="server"
                                CommandName="EditRoute"
                                CommandArgument='<%# Eval("route_id") %>'
                                CssClass="btn-edit-route"
                                CausesValidation="false">
                                <i class="fas fa-edit"></i> تعديل
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnDeleteRoute" runat="server"
                                CommandName="DeleteRoute"
                                CommandArgument='<%# Eval("route_id") %>'
                                CssClass="btn-delete-route"
                                CausesValidation="false"
                                OnClientClick="return confirm('هل أنت متأكد من حذف هذا التسكين؟');">
                                <i class="fas fa-trash"></i> حذف
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoRoutes" runat="server"
                Text="لا توجد تسكينات"
                CssClass="text-center d-block p-3 text-muted"
                Visible="false">
            </asp:Label>

            <!-- ===== Pager (max 6 items per page) ===== -->
            <asp:Panel ID="pnlPager" runat="server" Visible="false">
                <div class="pager-bar">
                    <asp:LinkButton ID="btnNextPage" runat="server" CausesValidation="false" OnClick="BtnNextPage_Click">
                        <i class="fas fa-chevron-right"></i> التالي
                    </asp:LinkButton>

                    <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnPageNum" runat="server"
                                CommandName="GoPage"
                                CommandArgument='<%# Eval("PageIndex") %>'
                                CssClass='<%# Eval("Css") %>'
                                CausesValidation="false"><%# Eval("Label") %></asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:LinkButton ID="btnPrevPage" runat="server" CausesValidation="false" OnClick="BtnPrevPage_Click">
                        السابق <i class="fas fa-chevron-left"></i>
                    </asp:LinkButton>
                </div>
                <div class="pager-info">
                    <asp:Label ID="lblPagerInfo" runat="server"></asp:Label>
                </div>
            </asp:Panel>
        </div>
    </div>
</div>

<!-- jQuery + Select2 (jQuery is isolated with noConflict so it never clashes with the master page's copy) -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.13/js/select2.min.js"></script>
<script type="text/javascript">
    var $j = jQuery.noConflict(true);

    // Filled by the server on EVERY request (see RegisterCarData)
    var carsData = [];   // [{id, text, model, body, year, passengers}]

    var select2Arabic = {
        noResults: function () { return 'لا توجد نتائج'; },
        searching: function () { return 'جاري البحث...'; }
    };

    function updateCarInfo() {
        var id = $j('#<%= ddlCar.ClientID %>').val();
        var box = document.getElementById('carInfoBox');
        if (!id) { box.style.display = 'none'; return; }

        var car = null;
        for (var i = 0; i < carsData.length; i++) {
            if (carsData[i].id === id) { car = carsData[i]; break; }
        }
        if (!car) { box.style.display = 'none'; return; }

        document.getElementById('infoModel').value = car.model || '';
        document.getElementById('infoBody').value = car.body || '';
        document.getElementById('infoYear').value = car.year || '';
        document.getElementById('infoPassengers').value = car.passengers || '';
        box.style.display = 'block';
    }

    $j(function () {
        // ----- form dropdowns -----
        $j('#<%= ddlRoute.ClientID %>').select2({ dir: 'rtl', width: '100%', language: select2Arabic, placeholder: '-- اختر الخط --' });
        $j('#<%= ddlCar.ClientID %>').select2({ dir: 'rtl', width: '100%', language: select2Arabic, placeholder: '-- اختر السيارة --' });
        $j('#<%= ddlDriver.ClientID %>').select2({ dir: 'rtl', width: '100%', language: select2Arabic, placeholder: '-- اختر السائق --' });

        $j('#<%= ddlCar.ClientID %>').on('change', updateCarInfo);
        updateCarInfo();

        // ----- list filters: each dropdown has its own search box; picking a value searches immediately -----
        var filterIds = [
            '#<%= ddlFilterRoute.ClientID %>',
            '#<%= ddlFilterCar.ClientID %>',
            '#<%= ddlFilterDriver.ClientID %>'
        ];
        for (var i = 0; i < filterIds.length; i++) {
            $j(filterIds[i]).select2({ dir: 'rtl', width: '100%', language: select2Arabic });
            $j(filterIds[i]).on('change', function () {
                $j('#<%= btnSearch.ClientID %>').click();
            });
        }
    });
</script>

</asp:Content>