<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckinTypes.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.CheckIn.CheckinTypes" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-check-square-o"></i> Check-in Configurations</h1>

            </div>
            <div class="panel-body">

                <div class="list-as-blocks clearfix">
                    <ul>
                        <asp:Repeater ID="rptCheckinTypes" runat="server">
                            <ItemTemplate>
                                <li class='<%# Eval("ActiveCssClass") %>'>
                                    <asp:LinkButton ID="lbCheckinType" runat="server" CommandArgument='<%# Eval("Id") %>' CommandName="Display">
                                        <i class='<%# Eval("IconCssClass") %>'></i>
                                        <h3><%# Eval("Name") %> </h3>
                                    </asp:LinkButton>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>