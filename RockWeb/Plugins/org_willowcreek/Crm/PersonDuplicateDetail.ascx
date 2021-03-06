﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonDuplicateDetail.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Crm.PersonDuplicateDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Person Duplicates</h1>
            </div>
            <div class="panel-body">

                <Rock:NotificationBox ID="nbNoDuplicatesMessage" CssClass="alert-grid" runat="server" NotificationBoxType="Success" Text="No duplicates found for this person." />

                <div class="grid grid-panel">
                    <Rock:Grid ID="gList" runat="server" AllowSorting="True" OnRowDataBound="gList_RowDataBound" PersonIdField="PersonId">
                        <Columns>
                            <Rock:SelectField ShowHeader="false" />
                            <Rock:RockTemplateField HeaderText="Confidence" ItemStyle-HorizontalAlign="Right" SortExpression="ConfidenceScore">
                                <ItemTemplate>
                                    <%# GetConfidenceScoreColumnHtml((double?)Eval("ConfidenceScore")) %>
                                </ItemTemplate>
                            </Rock:RockTemplateField>

                            <Rock:RockTemplateField HeaderText="First Name(s)" SortExpression="DuplicatePerson.FirstName, DuplicatePerson.LastName">
                                <ItemTemplate>
                                    <ul class="list-unstyled">
                                        <li class="clearfix"><%# Eval("DuplicatePerson.FirstName") %></li>
                                        <%# ( Eval("DuplicatePerson.FirstName").ToString() != Eval("DuplicatePerson.NickName").ToString() ) 
                                                ? "<li class=\"clearfix\">" + Eval("DuplicatePerson.NickName") + "</li>"
                                                : "" %>
                                    </ul>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:RockBoundField DataField="DuplicatePerson.MiddleName" HeaderText="Middle" SortExpression="DuplicatePerson.MiddleName, DuplicatePerson.LastName" />
                            <%--<Rock:RockBoundField DataField="DuplicatePerson.LastName" HeaderText="Last Name" SortExpression="DuplicatePerson.LastName, DuplicatePerson.FirstName" />--%>
                            <Rock:RockTemplateField HeaderText="Last Name(s)">
                                <ItemTemplate>
                                    <ul class="list-unstyled">
                                        <asp:Repeater ID="rptrLastNames" runat="server" DataSource='<%# GetLastNames(Eval("DuplicatePerson") as Rock.Model.Person) %>'>
                                            <ItemTemplate>
                                                <li class="clearfix"><%# Container.DataItem %></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:RockBoundField DataField="DuplicatePerson.SuffixValue" HeaderText="Suffix" SortExpression="DuplicatePerson.Suffix" />
                            <%--<Rock:RockBoundField DataField="DuplicatePerson.Email" HeaderText="Email" SortExpression="DuplicatePerson.Email" />--%>
                            <Rock:RockTemplateField HeaderText="Email(s)">
                                <ItemTemplate>
                                    <ul class="list-unstyled">
                                        <asp:Repeater ID="rptrEmails" runat="server" DataSource='<%# GetEmails(Eval("DuplicatePerson") as Rock.Model.Person) %>'>
                                            <ItemTemplate>
                                                <li class="clearfix"><%# Container.DataItem %></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:EnumField DataField="DuplicatePerson.Gender" HeaderText="Gender" SortExpression="DuplicatePerson.Gender" />
                            <Rock:RockBoundField DataField="DuplicatePerson.Age" HeaderText="Age" SortExpression="DuplicatePerson.Age" />
                            <Rock:RockBoundField DataField="DuplicatePerson.GradeFormatted" HeaderText="Grade" SortExpression="DuplicatePerson.GraduationYear" />

                            <Rock:RockTemplateField HeaderText="Campus">
                                <ItemTemplate>
                                    <ul class="list-unstyled">
                                        <asp:Repeater ID="rptrCampuses" runat="server" DataSource='<%# GetCampuses(Eval("DuplicatePerson") as Rock.Model.Person) %>'>
                                            <ItemTemplate>
                                                <li class="campus clearfix">
                                                    <p>
                                                        <%# Eval("Name") %>
                                                    </p>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:RockTemplateField HeaderText="Addresses">
                                <ItemTemplate>
                                    <ul class="list-unstyled">
                                        <asp:Repeater ID="rptrAddresses" runat="server" DataSource='<%# GetGroupLocations(Eval("DuplicatePerson") as Rock.Model.Person) %>'>
                                            <ItemTemplate>
                                                <li class="address clearfix">

                                                    <strong><%# Eval("GroupLocationTypeValue.Value") %></strong>
                                                    <p>
                                                        <%# Eval("Location.FormattedHtmlAddress") %>
                                                    </p>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:RockTemplateField HeaderText="Phone Numbers">
                                <ItemTemplate>
                                    <ul class="list-unstyled">
                                        <asp:Repeater ID="rptrPhoneNumbers" runat="server" DataSource='<%# GetPhoneNumbers(Eval("DuplicatePerson") as Rock.Model.Person) %>'>
                                            <ItemTemplate>
                                                <li class="phonenumber clearfix">

                                                    <strong><%# Eval("NumberTypeValue.Value") %></strong>
                                                    <p>
                                                        <%# Eval("NumberFormatted") %>
                                                    </p>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:RockTemplateField ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <p>
                                        <a class="btn btn-default js-view-person" onclick="<%# GetPersonViewOnClick((int)Eval("PersonId")) %>" data-toggle="tooltip" title="View Person"><i class="fa fa-user fa-fw"></i></a>
                                    </p>
                                    <p>
                                        <asp:LinkButton runat="server" ID="btnNotDuplicate" CssClass="btn btn-default js-not-duplicate" data-toggle="tooltip" title="Not Duplicate" OnClick="btnNotDuplicate_Click" CommandName="NotDuplicate" CommandArgument='<%# Eval("PersonDuplicateId") %>'><i class="fa fa-ban fa-fw"></i></asp:LinkButton>
                                    </p>
                                    <p>
                                        <asp:LinkButton runat="server" ID="btnIgnoreDuplicate" CssClass="btn btn-default js-ignore-duplicate" data-toggle="tooltip" title="Ignore" OnClick="btnIgnoreDuplicate_Click" CommandName="IgnoreDuplicate" CommandArgument='<%# Eval("PersonDuplicateId") %>'><i class="fa fa-bell-slash fa-fw"></i></asp:LinkButton>
                                    </p>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                        </Columns>
                    </Rock:Grid>
                </div>
            </div>

            <script>
                Sys.Application.add_load(function () {

                    $('.js-view-person').tooltip();
                    $('.js-not-duplicate').tooltip();
                    $('.js-ignore-duplicate').tooltip();

                    $('.js-not-duplicate').on('click', function (e) {
                        // make sure the element that triggered this event isn't disabled
                        if (e.currentTarget && e.currentTarget.disabled) {
                            return false;
                        }

                        e.preventDefault();

                        Rock.dialogs.confirm("Are you sure this is not a duplicate?", function (result) {
                            if (result) {
                                window.location = e.target.href ? e.target.href : e.target.parentElement.href;
                            }
                        })
                    });


                    $('.js-ignore-duplicate').on('click', function (e) {
                        // make sure the element that triggered this event isn't disabled
                        if (e.currentTarget && e.currentTarget.disabled) {
                            return false;
                        }

                        e.preventDefault();

                        Rock.dialogs.confirm("This will ignore this potential duplicate until the score changes.  Are you sure you want to ignore this as a duplicate?", function (result) {
                            if (result) {
                                window.location = e.target.href ? e.target.href : e.target.parentElement.href;
                            }
                        })
                    });
                });
            </script>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
