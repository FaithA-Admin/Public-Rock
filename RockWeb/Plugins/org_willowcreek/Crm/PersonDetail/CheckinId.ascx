<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckinId.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Crm.PersonDetail.CheckinId" %>
<asp:UpdatePanel ID="upnlCheckinId" runat="server" class="context-attribute-values">
    <ContentTemplate>
        <section class="panel panel-persondetails">
            <div class="panel-heading rollover-container clearfix">
                <h3 class="panel-title pull-left">
                    <div class='fa fa-star'></div>
                    <asp:Literal ID="lTitle" runat="server" Text="Rock Check-In ID" />
                </h3>
            </div>
            <div class="panel-body">
                <fieldset id="fsCheckinIds" runat="server" class="attribute-values" />
                <div style="float: right;">
                    <asp:LinkButton ID="lbNewCheckinCard" ClientIDMode="Static" runat="server" CssClass="btn btn-primary" OnClick="btnNewCheckinCard_Click" />
                </div>
            </div>
        </section>
        <script>
            Sys.Application.add_load(function () {
                $("#lbNewCheckinCard").on('click', function (e) {
                    // make sure the element that triggered this event isn't disabled
                    if (e.currentTarget && e.currentTarget.disabled) {
                        return false;
                    }

                    e.preventDefault();

                    Rock.dialogs.confirm("Please confirm the assignent of a new Rock Check-In ID for the family.", function (result) {
                        if (result) {
                            window.location = e.target.href ? e.target.href : e.target.parentElement.href;
                        }
                    })
                });
            });
        </script>
    </ContentTemplate>
</asp:UpdatePanel>
