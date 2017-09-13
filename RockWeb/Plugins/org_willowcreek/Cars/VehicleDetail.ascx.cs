// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using org.willowcreek.Cars.Model;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;


using Rock.Attribute;
using Rock.Constants;
using Rock.Web;
using System.Text;
using System.Web;
using System.Data.Entity;

namespace RockWeb.Plugins.org_willowcreek.Cars
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Vehicle Donation Detail" )]
    [Category( "org_willowcreek > Cars" )]
    [Description( "Block for managing detials about a donated vehicle." )]

    [LinkedPage( "Person Detail Page", "Page to display person", false, "", "", 0)]
    [LinkedPage( "Business Detail Page", "Page to display a business", false, "", "", 1 )]
    [LinkedPage( "Print Communication Page", "Page used to print a communication message", false, "", "", 2 )]
    [TextField( "Communication From Name", "Default Communication From Name", false, "", "", 3)]
    [TextField( "Communication From Address", "Default Communication From Email Address", false, "", "", 4 )]
    [TextField( "Donor Letter Subject", "Default Donor Letter Subject", false, "", "", 5 )]
    [CodeEditorField( "Donor Letter Message", "The Lava template to use for the donor letter message.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 500, true, @"
<p>Dear {{ Vehicle.DonorPersonAlias.Person.NickName }},</p>

<p>On behalf of the C.A.R.S. Ministry please accept our grateful thanks for the {{ Vehicle.Year }} {{ Vehicle.MakeValue.Value }} {{ Vehicle.ModelValue.Value }}, 
VIN # {{ Vehicle.Vin }}, mileage {{ Vehicle.Mileage }}, you donated to Willow Creek Community Church. Our ministry is committed to meeeting the transportation 
needs of the under-resourced. Your donation will greatly help us serve a person in need.</p>

{% assign ssn = Vehicle.PersonAlias.Person | Attribute:'SSN' %}
{% if ssn == '' %}
<p>We need your Social Security number to file Form 1098-C with the IRS. If we do not have your Social Security number, you may not claim more than $500 for 
your donation regardless of the vehicles book value or sale proceeds. If you would like to provide it, please call me at 224-512-1775.</p>
{% endif %}

<p>This contemporaneous written acknowledgement is in lieu of IRS tax form 1098-C. We acknowledge that no goods or services were received in exchange for this 
contribution. We provide this writtedn acknowledgement to expeditiously provide you the manner in which your vehicle will be utilizied. Please retain this 
letter with your other tax papers. If you would like more information in IRS reporting guidelines and instructions, please visit www.IRS.gov. Our Federal 
identification number is 51-0164942.</p>
", "", 6 )]
    [TextField( "Sold Letter Subject", "Default Sold Letter Subject", false, "", "", 7 )]
    [CodeEditorField( "Sold Letter Message", "The Lava template to use for the sold letter message.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 500, true, @"
", "", 8 )]

    public partial class VehicleDetail : Rock.Web.UI.RockBlock
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            nbMessage.Visible = false;

            if ( !Page.IsPostBack )
            {
                ShowView( PageParameter( "VehicleId" ).AsIntegerOrNull() );
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            ShowView( hfVehicleId.Value.AsIntegerOrNull() );
        }

        protected void lbMoveToInventory_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
                if ( vehicleId.HasValue )
                {
                    var vehicle = new VehicleService( rockContext ).Get( vehicleId.Value );
                    if ( vehicle != null )
                    {
                        vehicle.Status = StatusType.Inventory;
                        vehicle.DateInInventory = RockDateTime.Now;
                        rockContext.SaveChanges();
                    }

                    ShowView( vehicleId );
                }
            }
        }

        protected void lbMarkComplete_Click( object sender, EventArgs e )
        {
            ddlDispositionType.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.DISPOSITION_TYPE.AsGuid() ), true );
            ddlPaymentType.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.DISPOSITION_PAYMENT_TYPE.AsGuid() ), true );

            using ( var rockContext = new RockContext() )
            {
                Vehicle vehicle = null;

                int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
                if ( vehicleId.HasValue )
                {
                    vehicle = new VehicleService( rockContext ).Get( vehicleId.Value );
                }

                ddlDispositionType.SetValue( vehicle != null ? vehicle.DispositionTypeId : (int?)null );
                ShowHideDispositionAmount();

                ppRecipient.SetValue( vehicle != null && vehicle.RecipientPersonAlias != null && vehicle.RecipientPersonAlias.Person != null ? vehicle.RecipientPersonAlias.Person : null );
                dpCompletedDate.SelectedDate = vehicle != null && vehicle.DateCompleted.HasValue ? vehicle.DateCompleted.Value : RockDateTime.Today;
                cbAmountCollected.Text = vehicle != null && vehicle.DispositionAmount.HasValue ? vehicle.DispositionAmount.Value.ToString() : string.Empty;
                ddlPaymentType.SetValue( vehicle != null ? vehicle.DispositionPaymentTypeValueId : (int?)null );
                tbPaymentNote.Text = vehicle != null ? vehicle.DispositionNote : string.Empty;
                tb1098Summary.Text = vehicle != null ? vehicle.Tax1098Summary : string.Empty;
            }

            pnlDisposition.Visible = true; pnlDisposition.Visible = true;
        }

        protected void ddlDispositionType_SelectedIndexChanged( object sender, EventArgs e )
        {
            ShowHideDispositionAmount();
        }

        protected void lbSaveDisposition_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
                if ( vehicleId.HasValue )
                {
                    var vehicleService = new VehicleService( rockContext );
                    var vehicle = vehicleService.Get( vehicleId.Value );
                    if ( vehicle != null )
                    {
                        vehicle.Status = StatusType.Complete;
                        vehicle.DispositionTypeId = ddlDispositionType.SelectedValueAsInt();
                        vehicle.RecipientPersonAliasId = ppRecipient.PersonAliasId;
                        vehicle.DateCompleted = dpCompletedDate.SelectedDate;
                        vehicle.DispositionAmount = cbAmountCollected.Text.AsDecimalOrNull();
                        vehicle.DispositionPaymentTypeValueId = ddlPaymentType.SelectedValueAsInt();
                        vehicle.DispositionNote = tbPaymentNote.Text;
                        vehicle.Tax1098Summary = tb1098Summary.Text;

                        rockContext.SaveChanges();

                        pnlDisposition.Visible = false;
                        ShowView( vehicle.Id );
                    }
                }
            }
        }

        protected void lbCancelDisposition_Click( object sender, EventArgs e )
        {
            pnlDisposition.Visible = false;
        }

        protected void lbEdit_Click( object sender, EventArgs e )
        {
            ShowEdit( hfVehicleId.Value.AsIntegerOrNull() );
        }

        protected void cbIsDropOff_CheckedChanged( object sender, EventArgs e )
        {
            ShowHidePickupAddress();
        }

        protected void lbEmailDonorLetter_Click( object sender, EventArgs e )
        {
            ShowCommunication( CommunicationType.Donor );
        }

        protected void lbEmailSoldLetter_Click( object sender, EventArgs e )
        {
            ShowCommunication( CommunicationType.Sold );
        }

        protected void ddlMake_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindModel( null );
        }

        protected void lbSave_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                var vehicleService = new VehicleService( rockContext );
                Vehicle vehicle = null;

                int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
                if ( vehicleId.HasValue )
                {
                    vehicle = vehicleService.Get( vehicleId.Value );
                }

                if ( vehicle == null )
                {
                    vehicle = new Vehicle();
                    vehicleService.Add( vehicle );
                }

                if ( ppDonor.PersonId.HasValue )
                {
                    var person = new PersonService( rockContext ).Get( ppDonor.PersonId.Value );
                    if ( person != null )
                    {
                        vehicle.DonorType = person.RecordTypeValue.Guid.Equals( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS.AsGuid() ) ? DonorType.Business : DonorType.Person;
                        vehicle.DonorPersonAliasId = person.PrimaryAliasId;
                    }
                }

                vehicle.Status = rblStatus.SelectedValueAsEnum<StatusType>( StatusType.Pending );
                if ( vehicle.Status == StatusType.Pending )
                {
                    vehicle.DateInInventory = null;
                    vehicle.DateCompleted = null;
                }
                else if ( vehicle.Status == StatusType.Inventory )
                {
                    vehicle.DateInInventory = vehicle.DateInInventory ?? RockDateTime.Now;
                    vehicle.DateCompleted = null;
                }
                else if ( vehicle.Status == StatusType.Complete )
                {
                    vehicle.DateInInventory = vehicle.DateInInventory ?? RockDateTime.Now;
                    vehicle.DateCompleted = vehicle.DateCompleted ?? RockDateTime.Now;
                }

                vehicle.AssessedValueType = rblAssessedValue.SelectedValueAsEnumOrNull<AssessedValueType>();
                vehicle.EstimatedValue = cbEstimatedValue.Text.AsDecimalOrNull();

                vehicle.SubStatusValueId = ddlSubStatus.SelectedValueAsInt();
                vehicle.DateEntered = dpDateEntered.SelectedDate ?? RockDateTime.Now;
                vehicle.IsDropOff = cbIsDropOff.Checked;
                if ( vehicle.IsDropOff ?? false )
                {
                    vehicle.PickUpLocationId = null;
                }
                else
                {
                    Location location = new LocationService( rockContext ).Get(
                        acPickupLocation.Street1,
                        acPickupLocation.Street2,
                        acPickupLocation.City,
                        acPickupLocation.State,
                        acPickupLocation.PostalCode,
                        acPickupLocation.Country );
                    vehicle.PickUpLocationId = location.Id;
                }
                vehicle.Year = ypYear.SelectedYear;
                vehicle.MakeValueId = ddlMake.SelectedValueAsInt();
                vehicle.ModelValueId = ddlModel.SelectedValueAsInt();
                vehicle.ColorValueId = ddlColor.SelectedValueAsInt();
                vehicle.BodyStyleValueId = ddlBodyStyle.SelectedValueAsInt();
                vehicle.Vin = dtvin.Text;
                vehicle.Note = tbNote.Text;
                vehicle.Mileage = tbMileage.Text.AsNumeric().AsIntegerOrNull();
                vehicle.Condition = rblCondition.SelectedValueAsEnum<ConditionType>( ConditionType.Running );

                vehicle.Photo1Id = iuPhoto1.BinaryFileId;
                vehicle.Photo2Id = iuPhoto2.BinaryFileId;
                vehicle.Photo3Id = iuPhoto3.BinaryFileId;
                vehicle.Photo4Id = iuPhoto4.BinaryFileId;
                vehicle.TitleFileId = fuTitle.BinaryFileId;

                if ( vehicle.TitleFileId.HasValue )
                {
                    var binaryFile = new BinaryFileService( rockContext ).Get( vehicle.TitleFileId.Value );
                    if ( binaryFile != null )
                    {
                        binaryFile.Description = "Title";
                    }
                }

                if ( Page.IsValid && vehicle.IsValid )
                {
                    rockContext.SaveChanges();
                    ShowView( vehicle.Id );
                }
            }
        }

        protected void lbCancel_Click( object sender, EventArgs e )
        {
            int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
            if ( vehicleId.HasValue )
            {
                ShowView( vehicleId.Value );
            }
            else
            {
                NavigateToParentPage();
            }
        }

        protected void rptrAttachments_ItemCommand( object source, RepeaterCommandEventArgs e )
        {
            if ( e.CommandName == "Remove" )
            {
                int? binaryFileId = e.CommandArgument.ToString().AsIntegerOrNull();
                if ( binaryFileId.HasValue )
                {
                    var attachmentList = hfAttachments.Value.SplitDelimitedValues()
                        .ToList().AsIntegerList()
                        .Where( i => i != binaryFileId )
                        .ToList();
                    hfAttachments.Value = attachmentList.AsDelimited( "," );
                    fuAttachments.BinaryFileId = null;

                    BindCommunicationAttachments();
                }
            }
        }

        protected void lbSend_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
                if ( vehicleId.HasValue )
                {
                    var vehicleService = new VehicleService( rockContext );
                    var vehicle = vehicleService.Get( vehicleId.Value );
                    if ( vehicle != null && vehicle.DonorPersonAlias != null && vehicle.DonorPersonAlias.Person != null && vehicle.DonorPersonAlias.Person.Email.IsNotNullOrWhitespace() )
                    {
                        var recipients = new List<string>();
                        recipients.Add( vehicle.DonorPersonAlias.Person.Email );

                        var attachments = new List<System.Net.Mail.Attachment>();
                        var attachmentIds = hfAttachments.Value.SplitDelimitedValues().ToList().AsIntegerList();
                        foreach ( var binaryFile in new BinaryFileService( rockContext )
                            .Queryable().AsNoTracking()
                            .Where( f => attachmentIds.Contains( f.Id ))
                            .ToList() )
                            
                        {
                            attachments.Add( new System.Net.Mail.Attachment( binaryFile.ContentStream, binaryFile.FileName ) );
                        }

                        Rock.Communication.Email.Send( ebFromAddress.Text, tbFromName.Text, tbSubject.Text, recipients, heCommMessage.Text, ResolveRockUrl( "~/" ), ResolveRockUrl( "~~/" ), attachments, true );

                        if ( hfCommType.Value.ConvertToEnum<CommunicationType>() == CommunicationType.Donor )
                        {
                            vehicle.LastDonarLetterSendDate = RockDateTime.Now;
                        }
                        else
                        {
                            vehicle.LastSoldLetterSendDate = RockDateTime.Now;
                        }
                        rockContext.SaveChanges();

                        pnlCommunication.Visible = false;
                        pnlMain.Visible = true;
                        ShowView( vehicle.Id );
                    }
                }
            }
        }

        protected void lbCancelSend_Click( object sender, EventArgs e )
        {
            pnlCommunication.Visible = false;
            pnlMain.Visible = true;
            ShowView( hfVehicleId.Value.AsIntegerOrNull() );
        }

        protected void fuAttachments_FileUploaded( object sender, EventArgs e )
        {
            var attachmentList = hfAttachments.Value.SplitDelimitedValues().ToList();
            attachmentList.Add( fuAttachments.BinaryFileId.ToString() );
            hfAttachments.Value = attachmentList.AsDelimited( "," );
            fuAttachments.BinaryFileId = null;

            BindCommunicationAttachments();
        }

        protected void lbPrint_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                var entitySetService = new EntitySetService( rockContext );
                var entitySet = new EntitySet();
                entitySetService.Add( entitySet );
                entitySet.Name = tbSubject.Text;
                entitySet.EntityTypeId = EntityTypeCache.Read( typeof( Vehicle ) ).Id;
                entitySet.ExpireDateTime = RockDateTime.Now.AddHours( 1 );
                entitySet.Note = heCommMessage.Text;
                rockContext.SaveChanges();

                NavigateToLinkedPage( "PrintCommunicationPage", new Dictionary<string, string> { { "EntitySetID", entitySet.Id.ToString() } } );
            }
        }

        #endregion

        #region Methods

        private void ShowView( int? vehicleId )
        {
            using ( var rockContext = new RockContext() )
            {
                if ( vehicleId.HasValue )
                {
                    ShowView( rockContext, new VehicleService( rockContext ).Get( vehicleId.Value ) );
                }
                else
                {
                    ShowView( rockContext, null );
                }
            }
        }

        private void ShowView( RockContext rockContext, Vehicle vehicle )
        {
            SetHeading( vehicle );

            if ( vehicle == null )
            {
                ShowEdit( rockContext, null );
            }
            else
            {
                hfVehicleId.Value = vehicle != null ? vehicle.Id.ToString() : string.Empty;
                pnlView.Visible = true;
                pnlEdit.Visible = false;

                lbMoveToInventory.Enabled = UserCanEdit;
                lbMarkComplete.Enabled = UserCanEdit;
                lbEditDisposition.Enabled = UserCanEdit;
                lbEdit.Enabled = UserCanEdit;
                lbEmailDonorLetter.Enabled = UserCanEdit;
                lbEmailSoldLetter.Enabled = UserCanEdit;

                lbMoveToInventory.Visible = vehicle.Status == StatusType.Pending;
                lbMarkComplete.Visible = vehicle.Status == StatusType.Inventory;
                lbEditDisposition.Visible = vehicle.Status == StatusType.Complete;
                lbEmailSoldLetter.Visible = vehicle.Status == StatusType.Complete;

                lbEmailDonorLetter.Text = vehicle.LastDonarLetterSendDate.HasValue ? "Resend Donor Letter" : "Send Donor Letter";
                lbEmailSoldLetter.Text = vehicle.LastSoldLetterSendDate.HasValue ? "Resend Sold Letter" : "Send Sold Letter";

                if ( vehicle.DonorPersonAlias != null && vehicle.DonorPersonAlias.Person != null )
                {
                    lDonor.Text = GetPersonHtml( vehicle.DonorPersonAlias );

                    var donor = vehicle.DonorPersonAlias.Person;
                    if ( donor.Attributes == null )
                    {
                        donor.LoadAttributes( rockContext );
                    }

                    DefinedValueCache phoneType = null;

                    var businessType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS.AsGuid() );
                    if ( businessType != null && donor.RecordTypeValueId.HasValue && donor.RecordTypeValueId.Value == businessType.Id )
                    {
                        phoneType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK.AsGuid() );
                        lPhoneNumber.Label = "Work Phone Number";

                        lFedId.Label = "EIN";
                        lFedId.Text = donor.GetAttributeValue( "core_EIN" );
                    }
                    else
                    {
                        phoneType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid() );
                        lPhoneNumber.Label = "Home Phone Number";

                        lFedId.Label = "SSN";
                        var attr = donor.Attributes["core_SSN"];
                        if ( attr != null )
                        {
                            lFedId.Text = attr.FieldType.Field.FormatValue( null, donor.GetAttributeValue( "core_SSN" ), attr.QualifierValues, false );
                        }
                    }

                    if ( phoneType != null )
                    {
                        var phoneNumber = donor.PhoneNumbers.FirstOrDefault( n => n.NumberTypeValueId == phoneType.Id );
                        lPhoneNumber.Text = phoneNumber != null ? phoneNumber.NumberFormatted : string.Empty;
                    }
                    else
                    {
                        lPhoneNumber.Text = string.Empty;
                    }

                    lEmail.Text = donor.Email;

                    var mailingLocation = donor.GetMailingLocation();
                    lMailingAddr.Text = mailingLocation != null ? mailingLocation.ToString().ConvertCrLfToHtmlBr() : string.Empty;
                }
                else
                {
                    lDonor.Text = string.Empty;
                    lFedId.Text = string.Empty;
                    lEmail.Text = string.Empty;
                    lPhoneNumber.Text = string.Empty;
                    lMailingAddr.Text = string.Empty;
                }

                lPickupAddr.Text = ( vehicle.IsDropOff ?? false ) ? "Drop Off" : ( vehicle.PickUpLocation != null ? vehicle.PickUpLocation.ToString().ConvertCrLfToHtmlBr() : string.Empty );
                lDateEntered.Text = vehicle.DateEntered.ToShortDateString();

                divSubStatus.Visible = vehicle.SubStatusValue != null;
                lSubStatus.Text = vehicle.SubStatusValue != null ? vehicle.SubStatusValue.Value : string.Empty;

                divDateInInventory.Visible = vehicle.DateInInventory.HasValue;
                lDateInInventory.Text = vehicle.DateInInventory.HasValue ? vehicle.DateInInventory.Value.ToShortDateString() : string.Empty;

                divDateDonorLetterSent.Visible = vehicle.LastDonarLetterSendDate.HasValue;
                lDateDonorLetterSent.Text = vehicle.LastDonarLetterSendDate.HasValue ? vehicle.LastDonarLetterSendDate.Value.ToShortDateString() : string.Empty;

                divDateSoldLetterSent.Visible = vehicle.LastSoldLetterSendDate.HasValue;
                lDateSoldLetterSent.Text = vehicle.LastSoldLetterSendDate.HasValue ? vehicle.LastSoldLetterSendDate.Value.ToShortDateString() : string.Empty;

                divDateCompleted.Visible = vehicle.DateCompleted.HasValue;
                lDateCompleted.Text = vehicle.DateCompleted.HasValue ? vehicle.DateCompleted.Value.ToShortDateString() : string.Empty;

                hlblCondition.Text = vehicle.Condition.HasValue ? vehicle.Condition.ConvertToString() : "Unknown Condition";
                hlblCondition.LabelType = vehicle.Condition == ConditionType.Running ? LabelType.Success : LabelType.Danger;

                lPhoto1.Text = GetPhotoHtml( vehicle.Photo1, 180, 180 );
                lPhoto2.Text = GetPhotoHtml( vehicle.Photo2, 180, 180 );
                lPhoto3.Text = GetPhotoHtml( vehicle.Photo3, 180, 180 );
                lPhoto4.Text = GetPhotoHtml( vehicle.Photo4, 180, 180 );
                lTitleLink.Text = GetTitleHtml( vehicle.TitleFile );

                var vehicleDetailsLeft = new DescriptionList();
                vehicleDetailsLeft.Add( "Year", vehicle.Year.HasValue ? vehicle.Year.Value.ToString() : string.Empty );
                vehicleDetailsLeft.Add( "Make", vehicle.MakeValue != null ? vehicle.MakeValue.Value : string.Empty );
                vehicleDetailsLeft.Add( "Model", vehicle.ModelValue != null ? vehicle.ModelValue.Value : string.Empty );
                vehicleDetailsLeft.Add( "VIN", vehicle.Vin );
                lVehicleDetailsLeft.Text = vehicleDetailsLeft.Html;

                var vehicleDetailsRight = new DescriptionList();
                vehicleDetailsRight.Add( "Mileage", vehicle.Mileage.HasValue ? vehicle.Mileage.Value.ToString( "N0" ) : string.Empty );
                vehicleDetailsRight.Add( "Body Style", vehicle.BodyStyleValue != null ? vehicle.BodyStyleValue.Value : string.Empty );
                vehicleDetailsRight.Add( "Color", vehicle.ColorValue != null ? vehicle.ColorValue.Value : string.Empty );
                vehicleDetailsRight.Add( "Condition", vehicle.Condition != null ? vehicle.Condition.ConvertToString() : string.Empty );
                vehicleDetailsRight.Add( "Estimated Value", vehicle.EstimatedValue.HasValue ? vehicle.EstimatedValue.Value.FormatAsCurrency() : string.Empty );
                vehicleDetailsRight.Add( "Assessed Value", vehicle.AssessedValueType.HasValue ? vehicle.AssessedValueType.Value.ConvertToString() : string.Empty );
                lVehicleDetailsRight.Text = vehicleDetailsRight.Html;

                SetValueAndVisibility( lNotes, vehicle.Note );

                var dispositionDetails = new DescriptionList();
                dispositionDetails.Add( "Disposition Type", vehicle.DispositionType != null ? vehicle.DispositionType.Value : string.Empty );
                dispositionDetails.Add( "Recipient", GetPersonHtml( vehicle.RecipientPersonAlias ) );
                dispositionDetails.Add( "Amount Collected", vehicle.DispositionAmount.HasValue ? vehicle.DispositionAmount.Value.FormatAsCurrency() : string.Empty );
                dispositionDetails.Add( "Payment Type", vehicle.DispositionPaymentTypeValue != null ? vehicle.DispositionPaymentTypeValue.Value : string.Empty );
                dispositionDetails.Add( "Payment Note", vehicle.DispositionNote );
                dispositionDetails.Add( "1098 Summary", vehicle.Tax1098Summary );
                lDispositionDetails.Text = dispositionDetails.Html;
            }
        }

        private string GetPersonHtml( PersonAlias personAlias )
        {
            if ( personAlias != null && personAlias.Person != null )
            {
                var businessRecordType = DefinedTypeCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS.AsGuid() );
                bool isBusiness = businessRecordType != null && personAlias.Person.RecordTypeValueId.HasValue && personAlias.Person.RecordTypeValueId.Value == businessRecordType.Id;

                var parms = new Dictionary<string, string> { { isBusiness ? "BusinessId" : "PersonId", personAlias.Person.Id.ToString() } };
                string url = LinkedPageUrl( isBusiness ? "BusinessDetailPage" : "PersonDetailPage", parms );
                if ( url.IsNotNullOrWhitespace() )
                {
                    return string.Format( "<a href='{0}' target='_blank'>{1}</a>", url, personAlias.Person.FullName );
                }
                return personAlias.Person.FullName;
            }
            return string.Empty;
        }

        private string GetPhotoHtml( BinaryFile binaryFile, int? maxWidth, int? maxHeight )
        {
            if ( binaryFile != null )
            {
                string photoUrl = string.Format( "{0}GetImage.ashx?id={1}", VirtualPathUtility.ToAbsolute( "~/" ), binaryFile.Id );
                string width = maxWidth.HasValue ? string.Format( "&maxwidth={0}", maxWidth.Value ) : string.Empty;
                string height = maxHeight.HasValue ? string.Format( "&maxHeight={0}", maxHeight.Value ) : string.Empty;
                string tag = string.Format( "<img src='{0}{1}{2}'/>", photoUrl, width, height );
                return string.Format( "<a href='{0}' target='vehicle'>{1}</a>", photoUrl, tag );
            }

            return string.Empty;
        }

        private string GetTitleHtml( BinaryFile binaryFile )
        {
            if ( binaryFile != null )
            {
                string titleUrl = string.Format( "{0}GetImage.ashx?id={1}", VirtualPathUtility.ToAbsolute( "~/" ), binaryFile.Id );
                return string.Format( "<a href='{0}' target='vehicle'><i class='fa fa-file-text-o'></i> Title</a>", titleUrl );
            }

            return string.Empty;
        }

        private void SetValueAndVisibility( RockLiteral literal, string value )
        {
            literal.Text = value;
            literal.Visible = value.IsNotNullOrWhitespace();
        }

        private void ShowEdit( int? vehicleId )
        {
            using ( var rockContext = new RockContext() )
            {
                if ( vehicleId.HasValue )
                {
                    ShowEdit( rockContext, new VehicleService( rockContext ).Get( vehicleId.Value ) );
                }
                else
                {
                    ShowEdit( rockContext, null );
                }
            }
        }

        private void ShowEdit( RockContext rockContext, Vehicle vehicle )
        {
            hfVehicleId.Value = vehicle != null ? vehicle.Id.ToString() : string.Empty;
            pnlView.Visible = false;
            pnlEdit.Visible = true;

            lbSave.Visible = UserCanEdit;

            hfVehicleId.Value = vehicle != null ? vehicle.Id.ToString() : string.Empty;

            SetHeading( vehicle );

            if ( vehicle == null )
            {
                vehicle = new Vehicle();
                vehicle.DateEntered = RockDateTime.Now;
                vehicle.Status = StatusType.Pending;
                vehicle.Condition = ConditionType.Running;
            }

            ppDonor.SetValue( vehicle.DonorPersonAlias != null ? vehicle.DonorPersonAlias.Person : null );
            cbIsDropOff.Checked = vehicle.IsDropOff ?? false;
            ShowHidePickupAddress();
            acPickupLocation.SetValues( vehicle.PickUpLocation );

            rblStatus.BindToEnum<StatusType>();
            rblStatus.SetValue( vehicle.Status.HasValue ? vehicle.Status.Value.ConvertToInt().ToString() : StatusType.Pending.ConvertToInt().ToString() );

            rblAssessedValue.BindToEnum<AssessedValueType>();
            rblAssessedValue.Items.Add( new ListItem( "n/a", "" ) );
            rblAssessedValue.SetValue( vehicle != null && vehicle.AssessedValueType.HasValue ? vehicle.AssessedValueType.Value.ConvertToInt().ToString() : string.Empty );

            cbEstimatedValue.Text = vehicle.EstimatedValue.HasValue ? vehicle.EstimatedValue.Value.ToString() : string.Empty;

            ddlSubStatus.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_SUB_STATUS.AsGuid() ), true );
            ddlSubStatus.SetValue( vehicle.SubStatusValueId );

            dpDateEntered.SelectedDate = vehicle.DateEntered;

            ypYear.SelectedYear = vehicle.Year;

            BindMake( vehicle.MakeValueId );
            ddlMake.SetValue( vehicle.MakeValueId );

            BindModel( vehicle.ModelValueId );
            ddlModel.SetValue( vehicle.ModelValueId );

            dtvin.Text = vehicle.Vin;
            tbMileage.Text = vehicle.Mileage.HasValue ? vehicle.Mileage.Value.ToString( "N0" ) : string.Empty;

            ddlBodyStyle.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_BODY_STYLE.AsGuid() ), true );
            ddlBodyStyle.SetValue( vehicle.BodyStyleValueId );

            ddlColor.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_COLOR.AsGuid() ), true );
            ddlColor.SetValue( vehicle.ColorValueId );

            rblCondition.SetValue( ( vehicle.Condition.HasValue ? vehicle.Condition.Value : ConditionType.Running ).ConvertToInt().ToString() );

            iuPhoto1.BinaryFileId = vehicle.Photo1Id;
            iuPhoto2.BinaryFileId = vehicle.Photo2Id;
            iuPhoto3.BinaryFileId = vehicle.Photo3Id;
            iuPhoto4.BinaryFileId = vehicle.Photo4Id;
            fuTitle.BinaryFileId = vehicle.TitleFileId;
            tbNote.Text = vehicle.Note;
        }

        private void ShowHidePickupAddress()
        {
            acPickupLocation.Visible = !cbIsDropOff.Checked;
        }

        private void ShowHideDispositionAmount()
        {
            int? dvId = ddlDispositionType.SelectedValueAsInt();
            if ( dvId.HasValue )
            {
                var dv = DefinedValueCache.Read( dvId.Value );
                if ( dv != null )
                {
                    pnlDispositionAmount.Visible = dv.GetAttributeValue( "HasAmountCollected" ).AsBoolean();
                }
            }
        }

        private void ShowCommunication( CommunicationType commType )
        {
            using ( var rockContext = new RockContext() )
            {
                int? vehicleId = hfVehicleId.Value.AsIntegerOrNull();
                if ( vehicleId.HasValue )
                {
                    var vehicle = new VehicleService( rockContext ).Get( vehicleId.Value );
                    if ( vehicle != null )
                    {
                        hfCommType.Value = commType.ConvertToInt().ToString();

                        if ( vehicle.DonorPersonAlias == null || vehicle.DonorPersonAlias.Person == null || vehicle.DonorPersonAlias.Person.Email.IsNullOrWhiteSpace() )
                        {
                            nbCommunication.NotificationBoxType = NotificationBoxType.Warning;
                            nbCommunication.Text = "NOTE: The donor does not have a valid email address. Make sure to print the message instead of sending an email";
                            nbCommunication.Visible = true;

                            lbSend.Enabled = false;
                        }
                        else if ( commType == CommunicationType.Donor && vehicle.LastDonarLetterSendDate.HasValue )
                        {
                            nbCommunication.NotificationBoxType = NotificationBoxType.Info;
                            nbCommunication.Text = string.Format( "A donor email was sent for this vehicle on {0}.", vehicle.LastDonarLetterSendDate.Value.ToShortDateString() );
                            nbCommunication.Visible = true;
                        }
                        else if ( commType == CommunicationType.Sold && vehicle.LastSoldLetterSendDate.HasValue )
                        {
                            nbCommunication.NotificationBoxType = NotificationBoxType.Info;
                            nbCommunication.Text = string.Format( "A sold email was sent for this vehicle on {0}.", vehicle.LastSoldLetterSendDate.Value.ToShortDateString() );
                            nbCommunication.Visible = true;
                        }
                        else
                        {
                            nbCommunication.Visible = false;
                        }

                        lCommDonor.Text = GetPersonHtml( vehicle.DonorPersonAlias );
                        lCommEmail.Text = vehicle.DonorPersonAlias != null && vehicle.DonorPersonAlias.Person != null ? vehicle.DonorPersonAlias.Person.Email : string.Empty;
                        lCommPhoto.Text = GetPhotoHtml( vehicle.Photo1, 100, 100 );
                        lComVehicleSummary.Text = string.Format( "<strong>{0} {1} {2}</strong><br/>VIN: {3}<br/>Mileage: {4}<br/>Color: {5}",
                            vehicle.Year,
                            vehicle.MakeValue != null ? vehicle.MakeValue.Value : string.Empty,
                            vehicle.ModelValue != null ? vehicle.ModelValue.Value : string.Empty,
                            vehicle.Vin,
                            vehicle.Mileage.HasValue ? vehicle.Mileage.Value.ToString( "N0" ) : string.Empty,
                            vehicle.ColorValue != null ? vehicle.ColorValue.Value : string.Empty );

                        pnlMain.Visible = false;
                        pnlCommunication.Visible = true;

                        tbFromName.Text = GetAttributeValue( "CommunicationFromName" );
                        ebFromAddress.Text = GetAttributeValue( "CommunicationFromAddress" );

                        if ( vehicle.TitleFile != null )
                        {
                            hfAttachments.Value = vehicle.TitleFile.Id.ToString();
                            var attachments = new Dictionary<int, string> { { vehicle.TitleFile.Id, vehicle.TitleFile.Description.IsNotNullOrWhitespace() ? vehicle.TitleFile.Description : vehicle.TitleFile.FileName } };
                            rptrAttachments.DataSource = attachments;
                            rptrAttachments.DataBind();
                        }

                        string message = string.Empty;
                        if ( commType == CommunicationType.Donor )
                        {
                            lCommunicationHeading.Text = "Send Donor Letter";
                            tbSubject.Text = GetAttributeValue( "DonorLetterSubject" );
                            message = GetAttributeValue( "DonorLetterMessage" );
                        }
                        else
                        {
                            lCommunicationHeading.Text = "Send Sold Letter";
                            tbSubject.Text = GetAttributeValue( "SoldLetterSubject" );
                            message = GetAttributeValue( "SoldLetterMessage" );
                        }

                        var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
                        mergeFields.Add( "Vehicle", vehicle );
                        heCommMessage.Text = message.ResolveMergeFields( mergeFields );
                    }
                }
            }
        }

        private void BindCommunicationAttachments()
        {
            var fileIds = hfAttachments.Value.SplitDelimitedValues().ToList().AsIntegerList();
            var attachments = new Dictionary<int, string>();
            foreach( var binaryFile in new BinaryFileService( new RockContext() )
                .Queryable().AsNoTracking()
                .Where( f => fileIds.Contains( f.Id ) )
                .Select( f => new
                {
                    f.Id,
                    f.FileName,
                    f.Description
                } )
                .ToList() )
            {
                attachments.Add( binaryFile.Id, binaryFile.Description.IsNotNullOrWhitespace() ? binaryFile.Description : binaryFile.FileName );
            }
                
            rptrAttachments.DataSource = attachments;
            rptrAttachments.DataBind();
        }

        protected string GetAttachmentUrl( int binaryFileId )
        {
            return string.Format( "{0}GetFile.ashx?id={1}", System.Web.VirtualPathUtility.ToAbsolute( "~" ), binaryFileId );
        }

        private void SetHeading( Vehicle vehicle )
        {
            hlblStatus.Text = StatusType.Pending.ConvertToString();
            hlblStatus.LabelType = LabelType.Info;

            if ( vehicle != null )
            {
                lTitle.Text = string.Format( "{0} {1} {2} - {3}",
                    vehicle.Year.HasValue ? vehicle.Year.ToString() : string.Empty,
                    vehicle.MakeValue != null ? vehicle.MakeValue.Value : string.Empty,
                    vehicle.ModelValue != null ? vehicle.ModelValue.Value : string.Empty,
                    ( vehicle.DonorPersonAlias != null && vehicle.DonorPersonAlias.Person != null ) ? vehicle.DonorPersonAlias.Person.FullName : string.Empty );

                if ( vehicle.Status.HasValue )
                {
                    hlblStatus.Text = vehicle.Status.Value.ConvertToString();
                    switch( vehicle.Status.Value )
                    {
                        case StatusType.Pending: hlblStatus.LabelType = LabelType.Info; break;
                        case StatusType.Inventory: hlblStatus.LabelType = LabelType.Primary; break;
                        case StatusType.Complete: hlblStatus.LabelType = LabelType.Default; break;
                    }
                }

                hlblStockNumber.Visible = true;
                hlblStockNumber.Text = vehicle.StockNumber;
            }
            else
            {
                lTitle.Text = "New Vehicle";
                hlblStockNumber.Visible = false;
            }

            pdAuditDetails.SetEntity( vehicle, ResolveRockUrl( "~" ) );
        }

        private void BindMake( int? currentValue )
        {
            ddlMake.Items.Clear();
            ddlMake.Items.Add( new ListItem() );
            var dtMake = DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MAKE.AsGuid() );
            if ( dtMake != null )
            {
                foreach( var dvMake in dtMake.DefinedValues )
                {
                    if ( !dvMake.Value.ToLower().Contains( "inactive" ) || ( currentValue.HasValue && dvMake.Id == currentValue.Value ) )
                    {
                        ddlMake.Items.Add( new ListItem( dvMake.Value, dvMake.Id.ToString() ) );
                    }
                }
            }
        }
        private void BindModel( int? currentValue )
        {
            ddlModel.Items.Clear();
            ddlModel.Items.Add( new ListItem() );

            int? makeId = ddlMake.SelectedValueAsInt();
            if ( makeId.HasValue )
            {
                var dvMake = DefinedValueCache.Read( makeId.Value );
                var dtModel = DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MODEL.AsGuid() );
                if ( dvMake != null && dtModel != null )
                {
                    foreach ( var dvModel in dtModel.DefinedValues )
                    {
                        if ( 
                            dvModel.GetAttributeValue( "VehicleMake" ).AsGuid().Equals( dvMake.Guid ) &&
                            ( 
                                !dvModel.Value.ToLower().Contains("inactive") || 
                                ( currentValue.HasValue && dvModel.Id == currentValue.Value )
                            )
                        )
                        {
                            ddlModel.Items.Add( new ListItem( dvModel.Value, dvModel.Id.ToString() ) );
                        }
                    }
                }
            }
        }

        #endregion

        #region Enums

        public enum CommunicationType
        {
            Donor = 0,
            Sold = 1
        }

        #endregion

    }
}