<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="Rock.Web.UI.RockPage" %>

<asp:Content ID="ctMain" ContentPlaceHolderID="main" runat="server">
    <div class="line">
        <main class="content--global" role="main">

            <!-- Start Content Area -->
            <section class="row mw-md row-eq-height" data-equalizer="hxqx77-equalizer" data-equalize-on="medium" data-resize="44br6a-eq" data-events="resize">
                <div class="intro columns medium-6" data-equalizer-watch="">
                    <!-- Page Title -->
                    <Rock:PageIcon ID="PageIcon" runat="server" />
                    <h1 class="intro--header">
                        <Rock:PageTitle ID="PageTitle" runat="server" />
                    </h1>
                </div>
                <div class="columns medium-6 align-right-bottom" style="margin-bottom: 2em;" data-equalizer-watch="">
                    <div> 
                        <!-- Breadcrumbs -->
                        <nav aria-label="You are here:" role="navigation">
                            <Rock:PageBreadCrumbs ID="PageBreadCrumbs" runat="server" />
                        </nav>
                    </div>
                </div>
            </section>

            <!-- Ajax Error -->
            <div class="alert alert-danger ajax-error" style="display: none">
                <p><strong>Error</strong></p>
                <span class="ajax-error-message"></span>
            </div>


            <div class="row">
                <div class="col-md-12">
                    <Rock:Zone Name="Feature" runat="server" />
                </div>
            </div>
            <section class="row block-white mw-md">
                <div class="columns">
                    <Rock:Zone Name="Main" runat="server" />
                </div>
            </section>

            <div class="row">
                <div class="col-md-12">
                    <Rock:Zone Name="Section A" runat="server" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-4">
                    <Rock:Zone Name="Section B" runat="server" />
                </div>
                <div class="col-md-4">
                    <Rock:Zone Name="Section C" runat="server" />
                </div>
                <div class="col-md-4">
                    <Rock:Zone Name="Section D" runat="server" />
                </div>
            </div>

            <!-- End Content Area -->

        </main>
    </div>
</asp:Content>

