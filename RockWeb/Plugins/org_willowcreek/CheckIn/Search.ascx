<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.CheckIn.Search" %>
<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>        
        <Rock:ModalAlert ID="maWarning" runat="server" />
        <div style="visibility: hidden" id=" selectedSearchType">
            <span id="selectedSearchType">
                <asp:Literal ID="searchType" ClientIDMode="Static" runat="server" /></span>
        </div>
        <div class="checkin-body">
            <div class="checkin-scroll-panel">
                <div class="scroller">
                    <div class="checkin-search-body" style="align-content: center;">
                        <table>
                            <tr>
                                <td>
                                    <div>
                                        <asp:Panel ID="barcodeCamera" runat="server" HorizontalAlign="Center" ClientIDMode ="Static" Visible ="false">
                                            <canvas></canvas>
                                        </asp:Panel>                                         
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" valign="middle">
                                        <h1>
                                            <asp:Literal ID="lPageTitle" runat="server" />
                                        </h1>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlSearch" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                        <Rock:RockTextBox ID="search" CssClass="checkin-phone-entry" runat="server" Width="320px" autocomplete="off" autocorrect="off" autocapitalize="off" spellcheck="false" />
                                                </td>                                               
                                            </tr>
                                            <tr>
                                                <td align="center" valign="middle">
                                                        <span class="tapRequired">
                                                            <b>
                                                                <Rock:RockLiteral ID="lClickTheTextbox" runat="server" Text="*Click the textbox to show the keyboard." />
                                                            </b>
                                                        </span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Panel ID="pnlPhoneKeypad" runat="server" HorizontalAlign="Center" ClientIDMode="Static">
                                                        <div class="tenkey checkin-phone-keypad">
                                                            <div>
                                                                <a href="#" class="btn btn-default btn-lg digit">1</a>
                                                                <a href="#" class="btn btn-default btn-lg digit">2</a>
                                                                <a href="#" class="btn btn-default btn-lg digit">3</a>
                                                            </div>
                                                            <div>
                                                                <a href="#" class="btn btn-default btn-lg digit">4</a>
                                                                <a href="#" class="btn btn-default btn-lg digit">5</a>
                                                                <a href="#" class="btn btn-default btn-lg digit">6</a>
                                                            </div>
                                                            <div>
                                                                <a href="#" class="btn btn-default btn-lg digit">7</a>
                                                                <a href="#" class="btn btn-default btn-lg digit">8</a>
                                                                <a href="#" class="btn btn-default btn-lg digit">9</a>
                                                            </div>
                                                            <div>
                                                                <a href="#" class="btn btn-default btn-lg command back">
                                                                    <asp:Literal ID="lKeypadBack" runat="server" Text="Back" />                                                                    
                                                                </a>
                                                                <a href="#" class="btn btn-default btn-lg digit">0</a>
                                                                <a href="#" class="btn btn-default btn-lg command clear">
                                                                    <asp:Literal ID="lKeypadClear" runat="server" Text="Clear" />
                                                                </a>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="checkin-actions" style="width: 50%; margin: 0 auto;">
                                        <Rock:BootstrapButton CssClass="btn btn-primary" ID="lbSearch" ClientIDMode="Static" runat="server" OnClick="lbSearch_Click" Text="Search" DataLoadingText="Search"></Rock:BootstrapButton>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="checkin-footer">
            <div class="checkin-actions">
                <asp:LinkButton CssClass="btn btn-default" ID="lbBack" ClientIDMode="Static" runat="server" OnClick="lbBack_Click" Text="Back" />
            </div>
        </div> 
        <script>        
            $(document).ready(function () {
                var chromeBrowser = /chrom(e|ium)/.test(navigator.userAgent.toLowerCase()) && /google inc/.test(navigator.vendor.toLowerCase());
                if (chromeBrowser) {
                    if ($("#barcodeCamera").length)
                    {
                        //If protocol not https, redirect to https.
                        if (window.location.protocol != "https:" && !(location.hostname === "localhost" || location.hostname === "127.0.0.1"))
                            window.location.href = "https:" + window.location.href.substring(window.location.protocol.length);

                        //Add Code39 and QR barcode scanner javascript and initialization routine to DOM
                        $('head').append('<script src="/scripts/qrcodelib.js"><\/script>');
                        $('head').append('<script src="/scripts/webcodecamjs.js"><\/script>');

                        var arg = {
                            resultFunction: function (result) {
                                console.log("Detected the following " + result.format + " code: " + result.code);
                                $('.checkin-phone-entry').val(result.code);
                                document.getElementById('lbSearch').click();
                            }
                        };
                        new WebCodeCamJS("canvas").init(arg).play();


                        $(document).on("hidden.bs.modal", ".bootbox.modal", function (e) {
                            document.getElementById('lbBack').click();
                        });
                    }
                }

                 $(document).keypress(function (e)
                        {
                            if (e.which == 13)
                            {
                                document.getElementById('lbSearch').click();
                            }
                        });    
            });

            Sys.Application.add_load(function () {
                $('.tenkey a.digit').click(function () {
                    $phoneNumber = $("input[id$='search']");
                    $phoneNumber.val($phoneNumber.val() + $(this).html());
                });
                $('.tenkey a.back').click(function () {
                    $phoneNumber = $("input[id$='search']");
                    $phoneNumber.val($phoneNumber.val().slice(0, -1));
                });
                $('.tenkey a.clear').click(function () {
                    $phoneNumber = $("input[id$='search']");
                    $phoneNumber.val('');
                });

                // set focus to the input unless its an iOS device
                var iOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;
                if (!iOS) {
                    $('.tapRequired').hide();
                    if ($("#barcodeCamera").length)
                    {
                        $('.checkin-phone-entry').focus();
                    }
                }
                else
                {
                    if ($("#barcodeCamera").length)
                    {
                        $('.tapRequired').show();
                    }
                    else
                    {
                        $('.tapRequired').hide();
                    }
                }

                if (!$("#pnlPhoneKeypad").length) {
                    $("#lbSearch").hide();
                }
            });

            /*
            * NoClickDelay disables the .5 second click delay on iOS devices.
            * From: http://cubiq.org/remove-onclick-delay-on-webkit-for-iphone
            */
            function NoClickDelay(el) {
                this.element = typeof el == 'object' ? el : document.getElementById(el);
                if (window.Touch && this.element)
                    this.element.addEventListener('touchstart', this, false);
            }

            NoClickDelay.prototype = {
                handleEvent: function (e) {
                    switch (e.type) {
                        case 'touchstart': this.onTouchStart(e); break;
                        case 'touchmove': this.onTouchMove(e); break;
                        case 'touchend': this.onTouchEnd(e); break;
                    }
                },

                onTouchStart: function (e) {
                    e.preventDefault();
                    this.moved = false;
                    this.startX = e.touches[0].pageX;
                    this.startY = e.touches[0].pageY;

                    this.theTarget = document.elementFromPoint(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
                    if (this.theTarget.nodeType == 3) this.theTarget = theTarget.parentNode;
                    this.theTarget.className += ' pressed';

                    this.element.addEventListener('touchmove', this, false);
                    this.element.addEventListener('touchend', this, false);
                },

                onTouchMove: function (e) {
                    if (Math.abs(this.startX - e.touches[0].pageX) > 40 || Math.abs(this.startY - e.touches[0].pageY) > 40) {
                        this.moved = true;
                        this.theTarget.className = this.theTarget.className.replace(/ ?pressed/gi, '');
                    }
                },

                onTouchEnd: function (e) {
                    this.element.removeEventListener('touchmove', this, false);
                    this.element.removeEventListener('touchend', this, false);

                    if (!this.moved && this.theTarget) {
                        this.theTarget.className = this.theTarget.className.replace(/ ?pressed/gi, '');
                        var theEvent = document.createEvent('MouseEvents');
                        theEvent.initEvent('click', true, true);
                        this.theTarget.dispatchEvent(theEvent);
                    }

                    this.theTarget = undefined;
                }
            };

            /*
             * On page load/postback, set the NoClickDelay.
             */
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, args) {
                $('*[onclick]').each(function (index) {
                    if ($(this).text().length < 100)
                        new NoClickDelay($(this)[0]);
                });
                $('.digit').each(function (index) {
                    new NoClickDelay($(this)[0]);
                });
            });
        </script>      
    </ContentTemplate>
</asp:UpdatePanel>