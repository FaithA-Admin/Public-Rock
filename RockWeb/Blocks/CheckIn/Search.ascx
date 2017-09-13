<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="RockWeb.Blocks.CheckIn.Search" %>
<asp:UpdatePanel ID="upContent" runat="server">
<ContentTemplate>

    <script>
        
        Sys.Application.add_load(function () {
            $('.tenkey a.digit').click(function () {
                $phoneNumber = $("input[id$='tbPhone']");
                $phoneNumber.val($phoneNumber.val() + $(this).html());
            });
            $('.tenkey a.back').click(function () {
                $phoneNumber = $("input[id$='tbPhone']");
                $phoneNumber.val($phoneNumber.val().slice(0,-1));
            });
            $('.tenkey a.clear').click(function () {
                $phoneNumber = $("input[id$='tbPhone']");
                $phoneNumber.val('');
            });

            // set focus to the input unless on a touch device
            var isTouchDevice = 'ontouchstart' in document.documentElement;
            if (!isTouchDevice) {
                $('.checkin-phone-entry').focus();
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

    <Rock:ModalAlert ID="maWarning" runat="server" />

    <div class="checkin-header">
        <h1><asp:Literal ID="lPageTitle" runat="server" /></h1>
    </div>
                
    <div class="checkin-body">
        
        <div class="checkin-scroll-panel">
            <div class="scroller">

                <div class="checkin-search-body">

                <asp:Panel ID="pnlSearchPhone" runat="server">
                    <Rock:RockTextBox ID="tbPhone" MaxLength="10" CssClass="checkin-phone-entry" runat="server" Label="Phone Number" />

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
                            <a href="#" class="btn btn-default btn-lg command back">Back</a>
                            <a href="#" class="btn btn-default btn-lg digit">0</a>
                            <a href="#" class="btn btn-default btn-lg command clear">Clear</a>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlSearchName" runat="server">
                    <Rock:RockTextBox ID="txtName" runat="server" Label="Name" CssClass="namesearch" />
                </asp:Panel>

                <div class="checkin-actions">
                    <Rock:BootstrapButton CssClass="btn btn-primary" ID="lbSearch" runat="server" OnClick="lbSearch_Click" Text="Search" DataLoadingText="Searching..." ></Rock:BootstrapButton>
                </div>

            </div>
            
            </div>
        </div>

    </div>


    <div class="checkin-footer">   
        <div class="checkin-actions">
            <asp:LinkButton CssClass="btn btn-default" ID="lbBack" runat="server" OnClick="lbBack_Click" Text="Back" />
        </div>
    </div>

</ContentTemplate>
</asp:UpdatePanel>
