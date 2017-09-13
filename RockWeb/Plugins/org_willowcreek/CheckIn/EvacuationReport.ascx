<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EvacuationReport.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.CheckIn.EvacuationReport" %>

<style>
    @media print 
{
    .pg { page-break-before:always; }
    
}

.evacTitle
{
    font-size: 20px;
    font-weight: bold;
    margin-top: 10px;
    margin-bottom: 10px;
    width: 500px;
}

.evacSummary
{
	font-size: 16px;
}

.evacTotal
{
    font-size: 16px;
    font-weight: normal;
}

.evacGrade
{
    font-size: 20px;
    font-weight: bold;
    margin-top: 10px;
    margin-bottom: 10px;
    width: 500px;
}

.evacSmallGroup
{
    font-size: 16px;
    font-weight: bold;
    margin-left: 50px;
    margin-top: 10px;
    margin-bottom: 10px;
    width: 500px;
}

.evacChild
{
    font-size: 14px;
    margin-left: 100px;
    width: 380px;
    border-bottom: 1px solid silver;
}

.evacId
{
    width:80px;
}

.evacName
{
    width:500px;
}
</style>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <%--<div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-check-square-o"></i> Evacuation Report</h1>

            </div>--%>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-2">
                        <Rock:DatePicker ID="dpDate" runat="server" OnTextChanged="dbDate_TextChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-md-3">
                        <Rock:DataDropDownList ID="ddlConfig" runat="server" OnSelectedIndexChanged="ddlConfig_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                </div>
                
                <asp:Literal ID="lHtml" runat="server" />
            </div>
        <%--</div>--%>

    </ContentTemplate>
</asp:UpdatePanel>