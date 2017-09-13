<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RedirectAllRoles.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Cms.RedirectAllRoles" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox ID="nbAlert" runat="server" NotificationBoxType="Danger" />
    </ContentTemplate>
</asp:UpdatePanel>
