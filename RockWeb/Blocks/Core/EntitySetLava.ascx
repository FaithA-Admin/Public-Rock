<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EntitySetLava.ascx.cs" Inherits="RockWeb.Blocks.Core.EntitySetLava" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Literal ID="lResults" runat="server" />
        <asp:Literal ID="lDebug" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
