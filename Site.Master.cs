using System;


using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace car_WebApplication1

{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUserName.Text = Session["UserName"].ToString();
                BuildMenu();
            }
        }
        private void BuildMenu()
        {
            // Dashboard

        }
        private HtmlGenericControl CreateMenuItem(string text, string url, string icon)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes["href"] = ResolveUrl(url); ;

            HtmlGenericControl i = new HtmlGenericControl("i");
            i.Attributes["class"] = icon;

            a.Controls.Add(i);
            a.Controls.Add(new LiteralControl(text));

            li.Controls.Add(a);
            return li;
        }





        private void BuildStaticMenu()
        {


            // Management
            HtmlGenericControl management = CreateMenuItem(
                "Management",
                "#",
                "fas fa-cogs",
                true
            );

            HtmlGenericControl subMenu = new HtmlGenericControl("ul");
            subMenu.Attributes["class"] = "sub-menu";

            subMenu.Controls.Add(CreateSubMenuItem(
                "Users",
                "~/Users.aspx",
                "fas fa-users"
            ));

            subMenu.Controls.Add(CreateSubMenuItem(
                "Roles",
                "~/Roles.aspx",
                "fas fa-user-shield"
            ));


        }
        private HtmlGenericControl CreateMenuItem(string text, string url, string icon, bool hasSub = false)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            if (hasSub) li.Attributes["class"] = "has-submenu";

            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes["href"] = url;

            HtmlGenericControl i = new HtmlGenericControl("i");
            i.Attributes["class"] = icon;

            a.Controls.Add(i);
            a.Controls.Add(new LiteralControl(text));
            li.Controls.Add(a);

            return li;
        }

        private HtmlGenericControl CreateSubMenuItem(string text, string url, string icon)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            HtmlGenericControl a = new HtmlGenericControl("a");
            a.Attributes["href"] = url;

            HtmlGenericControl i = new HtmlGenericControl("i");
            i.Attributes["class"] = icon;

            a.Controls.Add(i);
            a.Controls.Add(new LiteralControl(text));
            li.Controls.Add(a);

            return li;
        }


        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}