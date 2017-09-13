<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContributionStatementList.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Finance.ContributionStatementList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title">
                <i class="fa fa-file-text-o"></i> Available Contribution Statements</h1>
            </div>
            <div class="panel-body">
                <asp:PlaceHolder ID ="phStatementButtons" runat="server" />
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
