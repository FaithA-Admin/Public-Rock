<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocationList.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.CheckIn.LocationList" %>
<link rel="stylesheet" href="/Styles/IonIcons/ionicons.css"/>
<meta name="viewport" content="width=device-width, initial-scale=1">

<style>
    td.text {
        padding-top:16px !important; 
        padding-bottom:16px !important;
    }
    tr:active {
        background-color:rgba(0, 0, 0, 0.05);
    }
</style>

<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>
        <Rock:Grid ID="gLocations" runat="server" DisplayType="Light" AllowSorting="false" OnRowSelected="gLocations_RowSelected" DataKeyNames="LocationId" Font-Size="Large">
            <Columns>
                <Rock:RockBoundField DataField="Area" HeaderText="Grade" ItemStyle-CssClass="text" />
                <Rock:RockBoundField DataField="Location" HeaderText="Room" ItemStyle-CssClass="text" />
                <Rock:RockBoundField DataField="EnRoute" HeaderStyle-CssClass="icon ion-android-walk" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-CssClass="text" />
                <Rock:RockBoundField DataField="InRoom" HeaderStyle-CssClass="fa fa-sign-in" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-CssClass="text" />
                <Rock:RockBoundField DataField="CheckedOut" HeaderStyle-CssClass="fa fa-sign-out" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-CssClass="text" />
                <Rock:RockBoundField DataField="Total" HeaderText="=" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-CssClass="text" />
            </Columns>
        </Rock:Grid>
        <asp:LinkButton ID="btnEvac" runat="server" Text="Evac Report" CssClass="btn btn-primary btn-lg " />
    </ContentTemplate>
</asp:UpdatePanel>