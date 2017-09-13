using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Security;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.Security;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.SystemGuid;
using Rock.Web.UI;
//namespace RockWeb.Plugins.org_willowcreek.WebUserControls
//{
    public partial class AccountButtonControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnLoad(EventArgs e)
        {
            if (Context.Items.Contains("CurrentPerson"))
            {
                phAccount.Visible = true;
                phLogin.Visible = false;
                var p = Context.Items["CurrentPerson"] as Rock.Model.Person;
                lHello.Text = p.NickName;
                //divProfilePhoto.Attributes.Add("style", String.Format("background-image: url('{0}'); background-size: cover; background-repeat: no-repeat; width:35px; height:35px; float:left;", Rock.Model.Person.GetPersonPhotoUrl(p, 200, 200)));
        }
        else
            {
                phAccount.Visible = false;
                phLogin.Visible = true;
            }
        }

        protected void lbLogin_Click(object sender, EventArgs e)
        {
            var rockPage = (RockPage)this.Page;
            var site = rockPage.Layout.Site;
            if (site.LoginPageId.HasValue)
            {
                site.RedirectToLoginPage(true);
            }
            else
            {
                FormsAuthentication.RedirectToLoginPage();
            }

        }
    }
//}