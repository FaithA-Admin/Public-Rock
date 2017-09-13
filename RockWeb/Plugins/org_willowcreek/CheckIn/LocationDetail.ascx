<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocationDetail.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.CheckIn.LocationDetail" %>

<link rel="stylesheet" href="/Styles/IonIcons/ionicons.css"/>
<meta name="viewport" content="width=device-width, initial-scale=1">

<script type="text/javascript">
    function clearActiveDialog() {
        $('#<%=hfActiveDialog.ClientID %>').val('');
    }
</script>
<style>
    .center-pills {
        display: flex;
        justify-content: center;
        margin-bottom:10px;
    }
    td.text {
        padding-top:16px !important; 
        padding-bottom:16px !important;
    }
    tr:active {
        background-color:rgba(0, 0, 0, 0.05);
    }
    /*div#header-fixed {
        position:fixed; 
        top:0px; 
        margin:auto; 
        z-index:100000; 
        width:100%;
    }
    div.tab-content {
        margin-top:150px;
    }*/
</style>

<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>
        <asp:Timer ID="timer" runat="server" OnTick="timer_Tick" />
        <asp:HiddenField ID="hfActiveTab" runat="server" />
        <asp:HiddenField ID="hfActiveDialog" runat="server" />
        <asp:HiddenField ID="hfAttendanceId" runat="server" />
        <asp:HiddenField ID="hfPersonId" runat="server" />
        <asp:HiddenField ID="hfGroupId" runat="server" />
        <asp:HiddenField ID="hfGroupTypeId" runat="server" />
        
        
        <div id="header-fixed">
            <h1 class="text-center"><asp:Literal ID="lLocation" runat="server" /></h1>
            <%--<ul class="nav nav-pills center-pills margin-b-md" style="transform:scale(1.2);">--%>
            <ul class="nav nav-pills center-pills">
                <li id="liEnRoute" runat="server" class="active">
                    <asp:LinkButton ID="pillEnRoute" OnClick="pillEnRoute_Click" runat="server"><i class="icon ion-android-walk"></i> En Route</asp:LinkButton>
                </li>
                <li id="liInRoom" runat="server">
                    <asp:LinkButton ID="pillInRoom" OnClick="pillInRoom_Click" runat="server"><i class="fa fa-sign-in" aria-hidden="true"></i> In Room</asp:LinkButton>
                </li>
                <li id="liHealthNotes" runat="server">
                    <asp:LinkButton ID="pillHealthNotes" OnClick="pillHealthNotes_Click" runat="server"><i class="fa fa-medkit" aria-hidden="true"></i> Health Notes</asp:LinkButton>
                </li>
                <li id="liCheckedOut" runat="server">
                    <asp:LinkButton ID="pillCheckedOut" OnClick="pillCheckedOut_Click" runat="server"><i class="fa fa-sign-out" aria-hidden="true"></i> Checked Out</asp:LinkButton>
                </li>
                <li id="liRoomList" runat="server">
                    <a href='/RoomList/<%=PageParameter( "CheckinTypeId" ) %>'><i class="fa fa-list" aria-hidden="true"></i> Room List</a>
                </li>
            </ul>
        </div>

        <div class="tab-content">
            <div id="divEnRoute" runat="server" class="tab-pane active margin-b-lg">
                <Rock:Grid ID="gEnRoute" OnGridRebind="gEnRoute_GridRebind" runat="server" DataKeyNames="Id,PersonId,GroupId,GroupTypeId" DisplayType="Light" AllowSorting="false" 
                    EmptyDataText="Waiting for Check-Ins..." Font-Size="Large" ShowHeader="false" AllowPaging="false" OnRowSelected="gEnRoute_Edit">
                    <Columns>
                        <Rock:LinkButtonField CssClass="btn btn-lg btn-primary fa fa-sign-in" Text=" Check In" OnClick="gEnRoute_CheckIn" HeaderStyle-HorizontalAlign="Center" />
                        <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Code" HeaderText="Code" SortExpression="Code" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Group" HeaderText="Group" SortExpression="Group" ItemStyle-CssClass="text" />
                        <Rock:LinkButtonField CssClass="btn btn-lg btn-default fa fa-sign-out" Text=" Check Out" OnClick="gEnRoute_CheckOut" HeaderStyle-HorizontalAlign="Center" />
                    </Columns>
                </Rock:Grid>    
            </div>
            <div id="divInRoom" runat="server" class="tab-pane">
                <Rock:Grid ID="gInRoom" OnGridRebind="gInRoom_GridRebind" runat="server" DataKeyNames="Id,PersonId,GroupId,GroupTypeId" DisplayType="Light" AllowSorting="false" 
                    EmptyDataText="This room is empty" Font-Size="Large" ShowHeader="false" AllowPaging="false" OnRowSelected="gInRoom_Edit">
                    <Columns>
                        <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Code" HeaderText="Code" SortExpression="Code" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Group" HeaderText="Group" SortExpression="Group" ItemStyle-CssClass="text" />
                        <Rock:LinkButtonField CssClass="btn btn-lg btn-default fa fa-building-o" Text=" En Route" OnClick="gInRoom_EnRoute" HeaderStyle-HorizontalAlign="Center" />
                        <Rock:LinkButtonField CssClass="btn btn-lg btn-default fa fa-sign-out" Text=" Check Out" OnClick="gInRoom_CheckOut" HeaderStyle-HorizontalAlign="Center" />
                    </Columns>
                </Rock:Grid>    
            </div>
            <div id="divHealthNotes" runat="server" class="tab-pane">
                <Rock:Grid ID="gHealthNotes" OnGridRebind="gHealthNotes_GridRebind" runat="server" DataKeyNames="Id,PersonId,GroupId,GroupTypeId" DisplayType="Light" AllowSorting="false" 
                    EmptyDataText="None" Font-Size="Large" ShowHeader="false" AllowPaging="false" OnRowSelected="gHealthNotes_Edit">
                    <Columns>
                        <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Code" HeaderText="Code" SortExpression="Code" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Group" HeaderText="Group" SortExpression="Group" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Note" HeaderText="Allergy / Health Note" ItemStyle-CssClass="text" />
                    </Columns>
                </Rock:Grid>
            </div>
            <div id="divCheckedOut" runat="server" class="tab-pane">
                <Rock:Grid ID="gCheckedOut" OnGridRebind="gCheckedOut_GridRebind" runat="server" DataKeyNames="Id,PersonId,GroupId,GroupTypeId" DisplayType="Light" AllowSorting="false" 
                    EmptyDataText="None" Font-Size="Large" ShowHeader="false" AllowPaging="false" OnRowSelected="gCheckedOut_Edit">
                    <Columns>
                        <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Code" HeaderText="Code" SortExpression="Code" ItemStyle-CssClass="text" />
                        <Rock:RockBoundField DataField="Group" HeaderText="Group" SortExpression="Group" ItemStyle-CssClass="text" />
                        <Rock:LinkButtonField CssClass="btn btn-lg btn-default fa fa-building-o" Text=" En Route" OnClick="gCheckedOut_EnRoute" HeaderStyle-HorizontalAlign="Center" />
                        <Rock:LinkButtonField CssClass="btn btn-lg btn-default fa fa-sign-in" Text=" Check In" OnClick="gCheckedOut_CheckIn" HeaderStyle-HorizontalAlign="Center" />
                    </Columns>
                </Rock:Grid>    
            </div>
        </div>

        <Rock:ModalDialog id="dlgEdit" runat="server" Title="Edit Person" OnSaveClick="dlgEdit_SaveClick" OnCancelScript="clearActiveDialog();" ValidationGroup="EditPerson">
            <Content>
                
                <Rock:RockTextBox id="tbNickName" runat="server" Label="First Name" Required="true" />
                <Rock:RockTextBox id="tbHealthNote" runat="server" Label="Allergy / Health Note" Required="false" />
                <Rock:RockTextBox id="tbNotes" runat="server" Label="Attendance Note" Required="false" />
                <Rock:RockDropDownList ID="ddlArea" runat="server" Label="Grade" Required="true" DataValueField="id" DataTextField="Name" AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" Help="If the desired grade is not listed, move the person to En Route first." />
                <Rock:RockDropDownList ID="ddlSmallGroup" runat="server" Label="Small Group" Required="true" DataValueField="id" DataTextField="Name" />
                <label class="control-label" for="ddlKiosk">Label Printer<Rock:HelpBlock Text="Choose where to reprint the tag" runat="server" /></label>
                <div class="input-group">
                    <span class="input-group-addon"><i class="fa fa-id-card-o" aria-hidden="true"></i></span>
                    <Rock:RockDropDownList ID="ddlKiosk" runat="server" CssClass="input-xlarge" OnSelectedIndexChanged="ddlKiosk_SelectedIndexChanged" AutoPostBack="true" DataTextField="Name" DataValueField="Id" />
                    <span class="input-group-btn">
                        <asp:LinkButton CssClass="btn btn-primary" ID="btnPrint" OnClick="btnPrint_Click" runat="server" Text="Reprint Tag" />
                    </span>
                </div>
                <br />
                <label class="control-label" for="lAdults">Checked In By</label>
                <div class="input-group">
                <asp:Literal ID="lAdults" runat="server" Text="None" />
                    </div>
            </Content>
        </Rock:ModalDialog>

    </ContentTemplate>
</asp:UpdatePanel>
