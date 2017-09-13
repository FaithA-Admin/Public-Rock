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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using org.willowcreek.Cars.Model;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_willowcreek.Cars
{
    [DisplayName( "Vehicle Donation Add" )]
    [Category( "org_willowcreek > Cars" )]
    [Description( "Add the details of a Donated Vehicle." )]

    [BooleanField( "Allow Businesses", "Should user be able to donate as a business?", true, order: 0 )]
    [AttributeField( Rock.SystemGuid.EntityType.PERSON, "SSN Attribute", "The Person attribute for storing SSN Number.", true, false, org.willowcreek.Cars.SystemGuid.Attribute.PERSON_SSN, "", 1 )]
    [BooleanField( "Show Photo", "Should user be able to attach a photo?", true, order: 2 )]
    [BooleanField( "Photo Required", "Should a photo be required?", false, order: 3 )]
    [BinaryFileTypeField( "Photo File Type", "The file type to use for the photo.", true, "C1142570-8CD6-4A20-83B1-ACB47C1CD377", order: 4 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS, "Record Status", "The record status to use for new individuals (default: 'Pending'.)", true, false, Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING, "", order: 5 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS, "Connection Status", "The connection status to use for new individuals (default: 'Web Prospect'.)", true, false, Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_WEB_PROSPECT, "", order: 6 )]
    [CodeEditorField( "Result Message", "Message to display after successful entry.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 300, true, @"
<div class='alert alert-success'>
Thank you for your vehicle donation! 
{% if Vehicle.IsDropOff %}
You can drop your vehicle off at [address here].
{% else %}
Someone will be in contact with you shortly regarding your donation.
{% endif %}
</div>", order: 7 )]
    public partial class VehicleDonationAdd : Rock.Web.UI.RockBlock
    {

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upAddVehicle );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( System.EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                BindDropdowns();
                SetDefaults();
                ShowForm();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the Block control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            ShowForm();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the rblDonationType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void rblDonationType_SelectedIndexChanged( object sender, EventArgs e )
        {
            ShowBusinessPanel();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlMake control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ddlMake_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindModel();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the rblPickupSameAsDonar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void rblPickupSameAsDonor_SelectedIndexChanged( object sender, EventArgs e )
        {
            acPickupLocation.Visible = rblPickup.SelectedValue.AsInteger() == 1;
        }

        /// <summary>
        /// Handles the Click event of the btnFinish control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnFinish_Click( object sender, EventArgs e )
        {
            using ( RockContext rockContext = new RockContext() )
            {
                var personService = new PersonService( rockContext );
                var vehicleService = new VehicleService( rockContext );
                Guid addressTypeGuid = new Guid( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME );

                // Get (and possibly create) the person
                var person = CreateOrUpdatePerson( rockContext, personService );
                if ( person != null )
                {
                    // Get the donor (person or business)
                    Person donor = null;
                    if ( rblDonationType.SelectedValueAsEnum<DonorType>() == DonorType.Person )
                    {
                        donor = person;
                    }
                    else
                    {
                        donor = CreateOrUpdateBusiness( rockContext, person );
                        addressTypeGuid = Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_WORK.AsGuid();
                    }

                    if ( donor != null )
                    {
                        try
                        {
                            Vehicle vehicle = new Vehicle();
                            vehicleService.Add( vehicle );

                            vehicle.DateEntered = RockDateTime.Now;
                            vehicle.DonorType = rblDonationType.SelectedValueAsEnum<DonorType>();
                            vehicle.DonorPersonAliasId = donor.PrimaryAliasId;
                            vehicle.Year = ypYear.SelectedYear;
                            vehicle.MakeValueId = ddlMake.SelectedValueAsInt();
                            vehicle.ModelValueId = ddlModel.SelectedValueAsInt();
                            vehicle.ColorValueId = ddlColor.SelectedValueAsInt();
                            vehicle.BodyStyleValueId = ddlBodyStyle.SelectedValueAsInt();
                            vehicle.Vin = dtvin.Text;
                            vehicle.Note = tbNote.Text;
                            vehicle.Mileage = tbMileage.Text.AsNumeric().AsIntegerOrNull();
                            vehicle.Status = StatusType.Pending;
                            if ( fuPhoto.Visible == true && fuPhoto.BinaryFileId.HasValue )
                            {
                                vehicle.Photo1Id = fuPhoto.BinaryFileId;
                            }

                            switch ( rblPickup.SelectedValue.AsInteger() )
                            {
                                case 0:
                                    GroupLocation groupLocation = personService.GetFirstLocation( donor.Id, DefinedValueCache.Read( addressTypeGuid ).Id );
                                    if ( groupLocation != null )
                                    {
                                        vehicle.PickUpLocationId = groupLocation.LocationId;
                                    }
                                    vehicle.IsDropOff = false;
                                    break;

                                case 1:
                                    Location location = new LocationService( rockContext ).Get(
                                        acPickupLocation.Street1,
                                        acPickupLocation.Street2,
                                        acPickupLocation.City,
                                        acPickupLocation.State,
                                        acPickupLocation.PostalCode,
                                        acPickupLocation.Country );
                                    vehicle.PickUpLocationId = location.Id;
                                    vehicle.IsDropOff = false;
                                    break;
                                case 2:
                                    vehicle.PickUpLocationId = null;
                                    vehicle.IsDropOff = true;
                                    break;
                            }

                            rockContext.SaveChanges();

                            pnlResult.Visible = true;
                            pnlVehicle.Visible = false;

                            vehicle = vehicleService.Get( vehicle.Id );
                            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( RockPage, CurrentPerson );
                            mergeFields.Add( "Vehicle", vehicle );

                            lResultMessage.Text = GetAttributeValue( "ResultMessage" ).ResolveMergeFields( mergeFields );
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogService.LogException( ex, Context, this.RockPage.PageId, this.RockPage.Site.Id, CurrentPersonAlias );
                            ShowError( string.Format( "We could not save your donation information ({0}).", ex.Message ) );
                        }
                    }
                    else
                    {
                        ShowError( "We could not save your donor information." );
                    }
                }
                else
                {
                    ShowError( "We could not save your personal information." );
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the dropdowns.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        private void BindDropdowns()
        {
            using ( RockContext rockContext = new RockContext() )
            {
                rblDonationType.BindToEnum<DonorType>( false );
                rblDonationType.SetValue( DonorType.Person.ConvertToInt() );

                BindMake();
                BindModel();

                ddlBodyStyle.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_BODY_STYLE.AsGuid(), rockContext ), true );
                ddlColor.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_COLOR.AsGuid(), rockContext ), true );
                ddlBodyStyle.BindToDefinedType( DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_BODY_STYLE.AsGuid(), rockContext ), true );
            }
        }

        private void SetDefaults()
        {
            if ( CurrentPerson != null )
            {
                tbFirstName.Text = CurrentPerson.FirstName;
                tbLastName.Text = CurrentPerson.LastName;
                tbPersonalEmail.Text = CurrentPerson.Email;

                PhoneNumber phoneNumber = null;
                var phoneType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid() );
                if ( phoneType != null )
                {
                    phoneNumber = CurrentPerson.PhoneNumbers.FirstOrDefault( n => n.NumberTypeValueId == phoneType.Id );
                }
                pnHome.Text = phoneNumber != null ? phoneNumber.NumberFormatted : string.Empty;
                dpBirthdate.SelectedDate = CurrentPerson.BirthDate;

                var mailingLocation = CurrentPerson.GetMailingLocation();
                acPersonalAddress.SetValues( mailingLocation != null ? mailingLocation : null );
                tbSSN.TextEncrypted = CurrentPerson.GetAttributeValue( "core_SSN" );
            }
        }

        /// <summary>
        /// Binds the make.
        /// </summary>
        private void BindMake()
        {
            ddlMake.Items.Clear();
            ddlMake.Items.Add( new ListItem() );

			var dtMake = DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MAKE.AsGuid() );
			if ( dtMake != null )
			{
				foreach ( var dvMake in dtMake.DefinedValues )
				{
					if ( !dvMake.Value.ToLower().Contains( "inactive" ) )
					{
						ddlMake.Items.Add( new ListItem( dvMake.Value, dvMake.Id.ToString() ) );
					}
				}
			}
        }

        /// <summary>
        /// Binds the model.
        /// </summary>
        private void BindModel()
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
                        if ( dvModel.GetAttributeValue( "VehicleMake" ).AsGuid().Equals( dvMake.Guid ) && 
                            ( !dvModel.Value.ToLower().Contains( "inactive" ) ) )
                        {
                            ddlModel.Items.Add( new ListItem( dvModel.Value, dvModel.Id.ToString() ) );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setting based on block configuration.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        private void ShowForm()
        {
            // Show/Hide Business Options
            ShowBusinessPanel();

            // Show/Hide Photo 
            fuPhoto.Visible = GetAttributeValue( "ShowPhoto" ).AsBoolean();
            fuPhoto.Required = GetAttributeValue( "PhotoRequired" ).AsBoolean();
            fuPhoto.BinaryFileTypeGuid = GetAttributeValue( "PhotoFileType" ).AsGuid();
        }

        /// <summary>
        /// Shows the business panel.
        /// </summary>
        private void ShowBusinessPanel()
        {
            bool allowBusinesses = GetAttributeValue( "AllowBusinesses" ).AsBoolean();
            rblDonationType.Visible = allowBusinesses;
            var showBusiness = allowBusinesses && rblDonationType.SelectedValueAsEnum<DonorType>() == DonorType.Business;
            pnlBusinessData.Visible = showBusiness;
            tbSSN.Visible = !showBusiness;

            BindPickup( showBusiness );
        }

        private void BindPickup( bool showBusiness )
        {
            var options = new Dictionary<int, string>();
            options.Add( 0, showBusiness ? "Same as Business Address" : "Same as Mailing Address" );
            options.Add( 1, "Different Location" );
            options.Add( 2, "I'll Drop Vehicle Off" );
            rblPickup.Items.Clear();
            rblPickup.DataSource = options;
            rblPickup.DataBind();

        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ShowError( string message )
        {
            nbError.Text = string.Format( "<p>{0}</p><p>Please try again. If this error continues to happen, please give us a call and we will get things worked out.</p>", message );
            nbError.Visible = true;
        }

        /// <summary>
        /// Creates or update the person.
        /// </summary>
        /// <returns></returns>
        private Person CreateOrUpdatePerson( RockContext rockContext, PersonService personService )
        {
            Person person = null;
            Group family = null;

            // Check to see if the name is the same as the currently logged in person, and if so, use that person
            if ( CurrentPerson != null &&
                ( CurrentPerson.FirstName == tbFirstName.Text.Trim() || CurrentPerson.NickName == tbFirstName.Text.Trim() ) &&
                CurrentPerson.LastName == tbLastName.Text.Trim() )
            {
                person = CurrentPerson;
                family = person.GetFamily( rockContext );
                person.Email = tbPersonalEmail.Text;

                rockContext.SaveChanges();
            }
            else
            {
                // otherwise see if there's only one match
                var personMatches = personService.GetByMatch( tbFirstName.Text.Trim(), tbLastName.Text.Trim(), tbPersonalEmail.Text );
                if ( person == null && personMatches.Count() == 1 )
                {
                    // If only one was found, use that person
                    person = personMatches.First();
                    family = person.GetFamily( rockContext );
                }
                else
                {
                    // Otherwise, create a new person 
                    person = new Person();
                    person.FirstName = tbFirstName.Text;
                    person.LastName = tbLastName.Text;
                    person.Email = tbPersonalEmail.Text;
                    person.IsEmailActive = true;
                    person.EmailPreference = EmailPreference.EmailAllowed;
                    person.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;

                    DefinedValueCache dvcConnectionStatus = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );
                    if ( dvcConnectionStatus != null )
                    {
                        person.ConnectionStatusValueId = dvcConnectionStatus.Id;
                    }

                    DefinedValueCache dvcRecordStatus = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() );
                    if ( dvcRecordStatus != null )
                    {
                        person.RecordStatusValueId = dvcRecordStatus.Id;
                    }

                    // Don't know the gender
                    person.Gender = Gender.Unknown;

                    family = PersonService.SaveNewPerson( person, rockContext, null, false );
                }
            }

            person = personService.Get( person.Id );

            // Add/Update home phone number
            if ( !string.IsNullOrWhiteSpace( PhoneNumber.CleanNumber( pnHome.Number ) ) )
            {
                int homePhoneTypeId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid() ).Id;
                var homePhoneNumber = person.PhoneNumbers.FirstOrDefault( p => p.NumberTypeValueId == homePhoneTypeId );
                if ( homePhoneNumber == null )
                {
                    homePhoneNumber = new PhoneNumber();
                    person.PhoneNumbers.Add( homePhoneNumber );
                    homePhoneNumber.NumberTypeValueId = homePhoneTypeId;
                }
                homePhoneNumber.CountryCode = PhoneNumber.CleanNumber( pnHome.CountryCode );
                homePhoneNumber.Number = PhoneNumber.CleanNumber( pnHome.Number );
            }

            // Add/Update address
            GroupService.AddNewGroupAddress( rockContext, family, Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME,
                acPersonalAddress.Street1, acPersonalAddress.Street2, acPersonalAddress.City, acPersonalAddress.State, acPersonalAddress.PostalCode, acPersonalAddress.Country, true, "", true, true );

            // Set the new person's birthdate
            var birthday = dpBirthdate.SelectedDate;
            if ( birthday.HasValue )
            {
                person.BirthMonth = birthday.Value.Month;
                person.BirthDay = birthday.Value.Day;
                if ( birthday.Value.Year != DateTime.MinValue.Year )
                {
                    person.BirthYear = birthday.Value.Year;
                }
            }

            // Update SSN
            if ( tbSSN.Text.IsNotNullOrWhitespace() )
            {
                var ssnAttribute = AttributeCache.Read( org.willowcreek.Cars.SystemGuid.Attribute.PERSON_SSN.AsGuid() );
                if ( ssnAttribute != null )
                {
                    person.LoadAttributes();
                    person.SetAttributeValue( ssnAttribute.Key, tbSSN.TextEncrypted );
                }
            }

            rockContext.SaveChanges();
            person.SaveAttributeValues( rockContext );

            return person;
        }

        /// <summary>
        /// Creates the Business.
        /// </summary>
        /// <returns></returns>
        private Person CreateOrUpdateBusiness( RockContext rockContext, Person person )
        {
            var personService = new PersonService( rockContext );

            Person business = null;
            Group family = null;

            // Check for Duplicate
            var businessMatches = personService.GetBusinessByMatch( tbBusinessName.Text.Trim(), tbEmail.Text.Trim() );
            if ( businessMatches.Count() > 0 )
            {
                business = businessMatches.First();
                family = business.GetFamily( rockContext );
            }

            if ( business == null )
            {
                // Create Business
                business = new Person();
                business.LastName = tbBusinessName.Text;
                business.Email = tbEmail.Text;
                business.IsEmailActive = true;
                business.EmailPreference = EmailPreference.EmailAllowed;
                business.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS.AsGuid() ).Id;

                DefinedValueCache dvcRecordStatus = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() );
                if ( dvcRecordStatus != null )
                {
                    business.RecordStatusValueId = dvcRecordStatus.Id;
                }

                DefinedValueCache dvcConnectionStatus = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );
                if ( dvcConnectionStatus != null )
                {
                    business.ConnectionStatusValueId = dvcConnectionStatus.Id;
                }

                // Create Person/Family
                family = PersonService.SaveNewPerson( business, rockContext, null, false );
            }

            UpdateRelationshipWithBusiness( rockContext, person, business );

            // Add/Update home phone number
            if ( !string.IsNullOrWhiteSpace( PhoneNumber.CleanNumber( pnWork.Number ) ) )
            {
                int workPhoneTypeId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK.AsGuid() ).Id;
                var workPhoneNumber = business.PhoneNumbers.FirstOrDefault( p => p.NumberTypeValueId == workPhoneTypeId );
                if ( workPhoneNumber == null )
                {
                    workPhoneNumber = new PhoneNumber();
                    business.PhoneNumbers.Add( workPhoneNumber );
                    workPhoneNumber.NumberTypeValueId = workPhoneTypeId;
                }
                workPhoneNumber.CountryCode = PhoneNumber.CleanNumber( pnHome.CountryCode );
                workPhoneNumber.Number = PhoneNumber.CleanNumber( pnHome.Number );
            }

            // Add/Update address
            GroupService.AddNewGroupAddress( rockContext, family, Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_WORK,
                acAddress.Street1, acAddress.Street2, acAddress.City, acAddress.State, acAddress.PostalCode, acAddress.Country, true, "", true, true );

            // Update EIN
            if ( tbEIN.Text.IsNotNullOrWhitespace() )
            {
                var einAttribute = AttributeCache.Read( org.willowcreek.Cars.SystemGuid.Attribute.BUSINESS_EIN.AsGuid() );
                if ( einAttribute != null )
                {
                    business.LoadAttributes();
                    business.SetAttributeValue( einAttribute.Key, tbEIN.Text );
                }
            }

            rockContext.SaveChanges();
            business.SaveAttributeValues( rockContext );

            return business;
        }

        /// <summary>
        /// Creates the Known Relationshio for Business.
        /// </summary>
        /// <returns></returns>
        private static void UpdateRelationshipWithBusiness( RockContext rockContext, Person person, Person business )
        {
            var groupService = new GroupService( rockContext );
            var groupMemberService = new GroupMemberService( rockContext );

            // Get the relationship roles to use
            var knownRelationshipGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS.AsGuid() );
            int businessContactRoleId = knownRelationshipGroupType.Roles
                .Where( r =>
                    r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS_CONTACT.AsGuid() ) )
                .Select( r => r.Id )
                .FirstOrDefault();
            int businessRoleId = knownRelationshipGroupType.Roles
                .Where( r =>
                    r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS.AsGuid() ) )
                .Select( r => r.Id )
                .FirstOrDefault();
            int ownerRoleId = knownRelationshipGroupType.Roles
                .Where( r =>
                    r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER.AsGuid() ) )
                .Select( r => r.Id )
                .FirstOrDefault();

            if ( ownerRoleId > 0 && businessContactRoleId > 0 && businessRoleId > 0 )
            {
                // get the known relationship group of the business contact
                // add the business as a group member of that group using the group role of GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS
                var contactKnownRelationshipGroup = groupMemberService.Queryable()
                    .Where( g =>
                        g.GroupRoleId == ownerRoleId &&
                        g.PersonId == person.Id )
                    .Select( g => g.Group )
                    .FirstOrDefault();
                if ( contactKnownRelationshipGroup == null )
                {
                    // In some cases person may not yet have a know relationship group type
                    contactKnownRelationshipGroup = new Group();
                    groupService.Add( contactKnownRelationshipGroup );
                    contactKnownRelationshipGroup.Name = "Known Relationship";
                    contactKnownRelationshipGroup.GroupTypeId = knownRelationshipGroupType.Id;

                    var ownerMember = new GroupMember();
                    ownerMember.PersonId = person.Id;
                    ownerMember.GroupRoleId = ownerRoleId;
                    contactKnownRelationshipGroup.Members.Add( ownerMember );
                }
                if ( !contactKnownRelationshipGroup.Members.Any( acc => acc.PersonId == business.Id && acc.GroupRoleId == businessRoleId ) )
                {
                    var groupMember = new GroupMember();
                    groupMember.PersonId = business.Id;
                    groupMember.GroupRoleId = businessRoleId;
                    contactKnownRelationshipGroup.Members.Add( groupMember );
                }
                // get the known relationship group of the business
                // add the business contact as a group member of that group using the group role of GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS_CONTACT
                var businessKnownRelationshipGroup = groupMemberService.Queryable()
                    .Where( g =>
                        g.GroupRole.Guid.Equals( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER ) ) &&
                        g.PersonId == business.Id )
                    .Select( g => g.Group )
                    .FirstOrDefault();
                if ( businessKnownRelationshipGroup == null )
                {
                    // In some cases business may not yet have a know relationship group type
                    businessKnownRelationshipGroup = new Group();
                    groupService.Add( businessKnownRelationshipGroup );
                    businessKnownRelationshipGroup.Name = "Known Relationship";
                    businessKnownRelationshipGroup.GroupTypeId = knownRelationshipGroupType.Id;

                    var ownerMember = new GroupMember();
                    ownerMember.PersonId = business.Id;
                    ownerMember.GroupRoleId = ownerRoleId;
                    businessKnownRelationshipGroup.Members.Add( ownerMember );
                }
                if ( !businessKnownRelationshipGroup.Members.Any( acc => acc.PersonId == person.Id && acc.GroupRoleId == businessContactRoleId ) )
                {
                    var businessGroupMember = new GroupMember();
                    businessGroupMember.PersonId = person.Id;
                    businessGroupMember.GroupRoleId = businessContactRoleId;
                    businessKnownRelationshipGroup.Members.Add( businessGroupMember );
                }
                rockContext.SaveChanges();
            }
        }

        #endregion

    }
}