<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VehicleDashboard.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Cars.VehicleDashboard" %>

<script type="text/javascript">
    Sys.Application.add_load(function ()
    {
        $('.js-follow-status').tooltip();
    });
</script>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlVehicle" runat="server">
            
            <asp:Panel ID="pnlVehicleDashboard" runat="server" CssClass="panel panel-block">
                <div class="panel-heading">
                    <h1 class="panel-title"><i class="fa fa-car"></i> Donations</h1>
                </div>
                <div class="panel-body">
                    
                    <Rock:ModalAlert ID="mdRegistrationsGridWarning" runat="server" />
                    <Rock:ModalAlert ID="mdDeleteWarning" runat="server" />

                    <ul class="nav nav-pills margin-b-lg">
                        <li id="liPending" runat="server" class="active">
                            <asp:LinkButton ID="lbPending" runat="server" Text="Pending" OnClick="lbTab_Click" />
                        </li>
                        <li id="liInvenory" runat="server">
                            <asp:LinkButton ID="lbInventory" runat="server" Text="Inventory" OnClick="lbTab_Click" />
                        </li>
                        <li id="liComplete" runat="server">
                            <asp:LinkButton ID="lbComplete" runat="server" Text="Complete" OnClick="lbTab_Click" />
                        </li>
                    </ul>
                    
                    <div class="grid grid-panel">
                        <Rock:GridFilter ID="fVehicleDashboard" runat="server" OnDisplayFilterValue="fVehicleDashboard_DisplayFilterValue">
                            <Rock:DateRangePicker ID="drpVehicleEntered" runat="server" Label="Date Entered" />
                            <Rock:DateRangePicker ID="drpVehicleInventory" runat="server" Label="Date In Inventory" />
                            <Rock:DateRangePicker ID="drpVehicleCompleted" runat="server" Label="Date Completed" />
                            <Rock:RockDropDownList ID="ddlDispositionType" runat="server" Label="Disposition Type" />
                            <Rock:PersonPicker ID="ppDonor" runat="server" Label="Donor" IncludeBusinesses="true" />
                            <Rock:YearPicker ID="ypYear" runat="server" Label="Vehicle Year" />
                            <Rock:RockDropDownList ID="ddlMake" runat="server" Label="Vehicle Make" AutoPostBack="true" OnSelectedIndexChanged="ddlMake_SelectedIndexChanged" />
                            <Rock:RockDropDownList ID="ddlModel" runat="server" Label="Vehicle Model" />
                            <Rock:RockDropDownList ID="ddlSubStatus" runat="server" Label="Sub-Status" />
                        </Rock:GridFilter>
                        <Rock:Grid ID="gVehicleDashboard" runat="server" DisplayType="Full" AllowSorting="true" OnRowSelected="gVehicleDashboard_RowSelected">
                            <Columns>
                                <Rock:DateField DataField="DateEntered" HeaderText="Date Entered" SortExpression="DateEntered" />
                                <Rock:RockBoundField DataField="StockNumber" HeaderText="Stock Number" SortExpression="StockNumber" />
                                <Rock:RockBoundField DataField="DonorPersonAlias.Person.FullNameReversed" HeaderText="Donor"
                                    SortExpression="DonorPersonAlias.Person.LastName,DonorPersonAlias.Person.NickName" />
                                <Rock:RockBoundField DataField="Year" HeaderText="Year" SortExpression="Year" />
                                <Rock:RockTemplateField HeaderText="Make/Model">
                                    <ItemTemplate>
                                        <asp:Literal ID="lMakeModel" runat="server"></asp:Literal>
                                    </ItemTemplate>
                                </Rock:RockTemplateField>
                                <Rock:RockBoundField DataField="SubStatusValue.Value" HeaderText="Sub-Status" SortExpression="SubStatusVaue.Value" />
                                <Rock:RockTemplateField HeaderText="Pickup Address">
                                    <ItemTemplate>
                                        <%# ( (bool?)Eval("IsDropOff") ?? false ) ? "Drop Off" : Eval("PickUpLocation") %>
                                    </ItemTemplate>
                                </Rock:RockTemplateField>
                                <Rock:DateField DataField="LastDonarLetterSendDate" HeaderText="Donor Leter Sent" SortExpression="LastDonarLetterSendDate" />
                                <Rock:DeleteField OnClick="gVehicleDashboard_Delete" />
                            </Columns>
                        </Rock:Grid>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
