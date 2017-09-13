<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountButton.ascx.cs" Inherits="AccountButtonControl" %>

<asp:PlaceHolder ID="phAccount" runat="server">
<%--    <li class="header--nav--item first has_photo"><div id="divProfilePhoto" runat="server" class="profile-photo photo-icon photo-round photo-round-xs"></div></li>
    <li class="header--nav--item has_more has_picture" data-id="account" />
        <a href="#"><asp:Literal ID="lHello" runat="server" /></a>
    </li>--%>
    <li class="header--nav--item has_icon login first has_more" data-id="account" />
        <a href="#"><asp:Literal ID="lHello" runat="server" /></a>
    </li>
</asp:PlaceHolder>

<asp:PlaceHolder ID="phLogin" runat="server">
    <li class="header--nav--item has_icon login first" />
        <asp:LinkButton ID="lbLogin" runat="server" OnClick="lbLogin_Click" CausesValidation="false" Text="Login"></asp:LinkButton>
    </li>
</asp:PlaceHolder>