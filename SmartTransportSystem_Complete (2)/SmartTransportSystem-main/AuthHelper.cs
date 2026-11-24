using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace car_WebApplication1
{
    public static class AuthHelper
    {
        public const string RoleAdmin = "admin";
        public const string RoleDriver = "driver";

        /// <summary>Pages a driver (any non-admin account) may open. Admin is never restricted.</summary>
        public static readonly string[] DriverAllowedPages = new string[]
        {
            "fuel_entry.aspx",
            "vehicle_management.aspx",
            "DriverTripEntry.aspx"
        };

        public const int DriverCarHistoryDays = 2;

        /// <summary>
        /// SQL returning the car_numbers.car_number_id values the current driver may use. Needs @EmpId.
        /// </summary>
        public static string DriverCarNumberIdsSql
        {
            get
            {
                return @"SELECT DISTINCT oc.v_number
                         FROM operation_command oc
                         WHERE oc.driver_name = @EmpId
                           AND oc.deleted = 0
                           AND ISNULL(oc.hide, 0) = 0
                           AND (ISNULL(oc.trip_status, 0) <> 2
                                OR oc.date >= DATEADD(DAY, -" + DriverCarHistoryDays + @", CAST(GETDATE() AS DATE)))";
            }
        }

        public static bool IsLoggedIn
        {
            get { return System.Web.HttpContext.Current.Session["UserName"] != null; }
        }

        public static string CurrentUserName
        {
            get
            {
                object v = System.Web.HttpContext.Current.Session["UserName"];
                return v == null ? null : v.ToString();
            }
        }

        public static string CurrentRole
        {
            get
            {
                object v = System.Web.HttpContext.Current.Session["role"];
                return v == null ? null : v.ToString();
            }
        }

        public static int? CurrentEmployeeId
        {
            get
            {
                object v = System.Web.HttpContext.Current.Session["employee_id"];
                if (v == null || v == DBNull.Value) return null;
                int result;
                return int.TryParse(v.ToString(), out result) ? (int?)result : null;
            }
        }

        public static bool IsAdmin
        {
            get { return string.Equals(CurrentRole, RoleAdmin, StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsDriver
        {
            get { return string.Equals(CurrentRole, RoleDriver, StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>True for every logged-in account that is NOT an admin (drivers).</summary>
        public static bool IsRestrictedUser
        {
            get { return IsLoggedIn && !IsAdmin; }
        }

        public static void EnsureLoggedIn(Page page)
        {
            if (!IsLoggedIn)
            {
                page.Response.Redirect("~/Login.aspx", true);
            }
        }

        /// <summary>Admin-only pages. A non-admin is sent to his own page.</summary>
      

        /// <summary>Admin: every page. Driver: only DriverAllowedPages.</summary>


        /// <summary>Admin: any car. Driver: only the cars of his own work orders.</summary>
        public static bool IsCarAllowedForCurrentDriver(int carNumberId)
        {
            if (IsAdmin) return true;

            int? empId = CurrentEmployeeId;
            if (!empId.HasValue) return false;

            try
            {
                string connString = ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string sql = "SELECT COUNT(*) FROM (" + DriverCarNumberIdsSql + ") d WHERE d.v_number = @CarNumberId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId.Value;
                        cmd.Parameters.Add("@CarNumberId", SqlDbType.Int).Value = carNumberId;
                        conn.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Page to open right after login. Admin keeps the dashboard.</summary>
        public static string GetHomePageForRole(string role)
        {
            if (string.Equals(role, RoleDriver, StringComparison.OrdinalIgnoreCase))
                return "~/DriverTripEntry.aspx";

            return "~/dashboard.aspx";
        }
        /// <summary>"~/DriverTripEntry.aspx", "/DriverTripEntry" and "DriverTripEntry.aspx" all become "drivertripentry".</summary>
        private static string NormalizePage(string path)
        {
            string file = System.IO.Path.GetFileName((path ?? "").TrimEnd('/'));
            if (file.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                file = file.Substring(0, file.Length - 5);
            return file.ToLowerInvariant();
        }

        /// <summary>True when both paths point to the same page.</summary>
        public static bool IsSamePage(string currentPath, string targetPath)
        {
            return NormalizePage(currentPath) == NormalizePage(targetPath);
        }

        /// <summary>Admin-only pages. A non-admin is sent to his own page (never to the page he is already on).</summary>
        public static void RequireAdmin(Page page)
        {
            EnsureLoggedIn(page);
            if (!IsAdmin)
            {
                string current = page.Request.AppRelativeCurrentExecutionFilePath;
                if (!IsSamePage(current, "~/DriverTripEntry.aspx"))
                    page.Response.Redirect("~/DriverTripEntry.aspx", true);
            }
        }

        /// <summary>Admin: every page. Driver: only DriverAllowedPages.</summary>
        public static bool IsPageAllowedForCurrentUser(string requestPath)
        {
            if (IsAdmin) return true;

            string current = NormalizePage(requestPath);
            foreach (string allowed in DriverAllowedPages)
            {
                if (current == NormalizePage(allowed))
                    return true;
            }
            return false;
        }
    }
}