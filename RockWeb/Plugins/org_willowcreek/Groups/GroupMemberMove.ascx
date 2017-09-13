<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupMemberMove.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Groups.GroupMemberMove" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <Triggers>
        <asp:PostBackTrigger ControlID="btnMove" />
    </Triggers>
    <ContentTemplate>
        <asp:Panel ID="pnlEntry" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-files-o"></i>
                    <asp:Literal ID="lActionTitle" runat="server" Text="Move Group Members" /></h1>
            </div>
            <div class="panel-body">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                <Rock:NotificationBox ID="nbSuccess" runat="server" NotificationBoxType="Success" Text="" Visible="false" />
                <div class="row">
                    <div class="col-md-6">
                        <Rock:Toggle ID="tglPeople" runat="server" OnText="Simple" OffText="Advanced" CssClass="pull-right" Checked="true" OnCheckedChanged="tglPeople_CheckedChanged" />
                        <Rock:GroupPicker ID="gpFrom" Label="Move From" AllowMultiSelect="false" Required="true" runat="server" OnSelectItem="gpFrom_SelectItem" />
                        <asp:Panel ID="pnlSimple" runat="server">
                            <Rock:RockCheckBoxList ID="cblRoles" Label="People To Move" runat="server" Required="true" />
                        </asp:Panel>
                        <asp:Panel ID="pnlAdvanced" runat="server">
                            <Rock:RockDropDownList ID="ddlAttendanceGroup" Label="Attendance Group" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlAttendanceGroup_SelectedIndexChanged"
                                 Help="Show attendance numbers for the past 8 weeks in this group and all beneath it." />
                            <Rock:Grid ID="gMembers" runat="server" DisplayType="Light" AllowSorting="true" OnGridRebind="gMembers_GridRebind" DataKeyNames="Id">
                                <Columns>
                                    <Rock:SelectField />
                                    <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" HtmlEncode="false" />
                                    <Rock:RockBoundField DataField="GroupRole" HeaderText="Role" SortExpression="GroupRole" />
                                    <Rock:RockBoundField DataField="Gender" HeaderText="Gender" SortExpression="Gender" />
                                    <Rock:RockBoundField DataField="BirthDate" HeaderText="Birth Date" SortExpression="BirthDate" DataFormatString="{0:M/dd/yyyy}" NullDisplayText =""/>
                                    <Rock:RockBoundField DataField="Attendance" HeaderText="Attendance" SortExpression="Attendance" />
                                </Columns>
                            </Rock:Grid>
                        </asp:Panel>
                    </div>
                    <div class="col-md-6">
                        <Rock:GroupPicker ID="gpTo" Label="Move To" AllowMultiSelect="false" Required="true" runat="server" OnSelectItem="gpTo_SelectItem" />
                        <Rock:RockRadioButtonList ID="rblRole" Label="New Role" runat="server" Visible="false" Required="true" />
                        <%--<div class="actions">--%>
                            <asp:LinkButton ID="btnMove" runat="server" Text="Move Members" CssClass="btn btn-primary" OnClick="btnMove_Click" />
                        <%--</div>--%>
                    </div>
                </div>

            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>