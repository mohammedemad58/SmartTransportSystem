using System;
using System.Web.UI;

namespace car_WebApplication1
{
    public partial class Site : System.Web.UI.MasterPage
    {
        private const string DriverHome = "~/DriverTripEntry.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.EnsureLoggedIn(this.Page);

            // The page that is really executing, e.g. "~/DriverTripEntry.aspx"
            string currentPage = Request.AppRelativeCurrentExecutionFilePath;

            // Driver only: a page that is not allowed goes to the trip page.
            // Never redirect when we are already on the trip page (this is what caused the loop).
            if (AuthHelper.IsRestrictedUser
                && !AuthHelper.IsPageAllowedForCurrentUser(currentPage)
                && !AuthHelper.IsSamePage(currentPage, DriverHome))
            {
                Response.Redirect(DriverHome, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                lblUserName.Text = AuthHelper.CurrentUserName;
                lblDropdownUserName.Text = AuthHelper.CurrentUserName;
                ApplyRoleBasedMenu();
            }
        }

        private void ApplyRoleBasedMenu()
        {
            if (AuthHelper.IsAdmin) return;

            // ---- Desktop menu ----
            if (navHome != null) navHome.Visible = false;
            if (A1 != null) A1.Visible = false;
            if (A2 != null) A2.Visible = false;
            if (lnkVehicleFullManagement != null) lnkVehicleFullManagement.Visible = false;
            if (lnkLookupManagement != null) lnkLookupManagement.Visible = false;
            if (lnkMaintenanceHistory != null) lnkMaintenanceHistory.Visible = false;
            if (lnkFuelReport != null) lnkFuelReport.Visible = false;
            if (pnlNavEmployees != null) pnlNavEmployees.Visible = false;
            if (lnkWorkOrder != null) lnkWorkOrder.Visible = false;

            // ---- Mobile sidebar ----
            if (lnkMobileDashboard != null) lnkMobileDashboard.Visible = false;
            if (pnlMobileEmployees != null) pnlMobileEmployees.Visible = false;
            if (lnkMobileVehicleFullManagement != null) lnkMobileVehicleFullManagement.Visible = false;
            if (lnkMobileLookupManagement != null) lnkMobileLookupManagement.Visible = false;
            if (lnkMobileMaintenanceHistory != null) lnkMobileMaintenanceHistory.Visible = false;
            if (lnkMobileCarMaintenance != null) lnkMobileCarMaintenance.Visible = false;
            if (lnkMobileFuelReport != null) lnkMobileFuelReport.Visible = false;
            if (lnkMobileWorkOrder != null) lnkMobileWorkOrder.Visible = false;

            // ---- Footer ----
            if (liFooterDashboard != null) liFooterDashboard.Visible = false;
            if (liFooterVehicles != null) liFooterVehicles.Visible = true;
            if (liFooterWorkOrder != null) liFooterWorkOrder.Visible = false;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}