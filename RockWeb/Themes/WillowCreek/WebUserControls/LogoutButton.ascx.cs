using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Rock;
using Rock.Web.UI;
using Rock.Security;

public partial class LogoutButtonControl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbLogout_Click(object sender, EventArgs e)
    {
        var rockPage = (RockPage)this.Page;
        //string action = hfActionType.Value;
        //if (action == "Login")
        //{
        //    var site = RockPage.Layout.Site;
        //    if (site.LoginPageId.HasValue)
        //    {
        //        site.RedirectToLoginPage(true);
        //    }
        //    else
        //    {
        //        FormsAuthentication.RedirectToLoginPage();
        //    }
        //}
        //else
        //{
        if (rockPage.CurrentUser != null)
        {
            var transaction = new Rock.Transactions.UserLastActivityTransaction();
            transaction.UserId = rockPage.CurrentUser.Id;
            transaction.LastActivityDate = RockDateTime.Now;
            transaction.IsOnLine = false;
            Rock.Transactions.RockQueue.TransactionQueue.Enqueue(transaction);
        }

        FormsAuthentication.SignOut();

        // After logging out check to see if an anonymous user is allowed to view the current page.  If so
        // redirect back to the current page, otherwise redirect to the site's default page
        var currentPage = Rock.Web.Cache.PageCache.Read(rockPage.PageId);
        if (currentPage != null && currentPage.IsAuthorized(Authorization.VIEW, null))
        {
            Response.Redirect(rockPage.PageReference.BuildUrl());
            Context.ApplicationInstance.CompleteRequest();
        }
        else
        {
            rockPage.Layout.Site.RedirectToDefaultPage();
        }
        //}
    }
}