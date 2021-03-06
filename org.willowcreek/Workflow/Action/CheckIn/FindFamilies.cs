﻿// <copyright>
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
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;
using Rock;
using org.willowcreek.SystemGuid;

namespace org.willowcreek.Workflow.Action.CheckIn
{
    /// <summary>
    /// Finds families based on a given search critieria (i.e. phone, barcode, etc)
    /// </summary>
    [ActionCategory( "Check-In" )]
    [Description( "Willow Creek Workflow Action that finds families based on a given search critieria (i.e. phone, barcode, etc)" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Willow Creek Find Families" )]
    public class FindFamilies : CheckInActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, Rock.Model.WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            if ( checkInState != null && checkInState.CheckIn.SearchType != null )
            {
                var personService = new PersonService( rockContext );
                var memberService = new GroupMemberService( rockContext );
                GroupService groupService = new GroupService( rockContext );
                PhoneNumberService phoneNumberService = new PhoneNumberService( rockContext );

                int familyGroupTypeId = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() ).Id;
                var dvActive = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() );

                if ( checkInState.CheckIn.SearchType.Guid.Equals( new Guid( Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_PHONE_NUMBER ) ) )
                {
                    string numericPhone = checkInState.CheckIn.SearchValue.AsNumeric();

                    var personRecordTypeId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;

                    // Find the families with any member who has a phone number that contains selected value
                    var familyQry = phoneNumberService.Queryable().AsNoTracking();


                    if ( checkInState.CheckInType == null || checkInState.CheckInType.PhoneSearchType == PhoneSearchType.EndsWith )
                    {
                        char[] charArray = numericPhone.ToCharArray();
                        Array.Reverse( charArray );
                        familyQry = familyQry.Where( o =>
                            o.NumberReversed.StartsWith( new string( charArray ) ) );
                    }
                    else
                    {
                        familyQry = familyQry.Where( o =>
                            o.Number.Contains( numericPhone ) );
                    }

                    var tmpQry = familyQry.Join( personService.Queryable().AsNoTracking(),
                            o => new { PersonId = o.PersonId, IsDeceased = false, RecordTypeValueId = personRecordTypeId },
                            p => new { PersonId = p.Id, IsDeceased = p.IsDeceased, RecordTypeValueId = p.RecordTypeValueId.Value },
                            ( pn, p ) => new { Person = p, PhoneNumber = pn } )
                            .Join( memberService.Queryable().AsNoTracking(),
                            pn => pn.Person.Id,
                            m => m.PersonId,
                            ( o, m ) => new { PersonNumber = o.PhoneNumber, GroupMember = m } );


                    if ( checkInState.CheckInType != null && checkInState.CheckInType.PreventInactivePeopele && dvActive != null )
                    {
                        // Reconstruct the query because doing this in the join condition is the fastest way
                        tmpQry = familyQry.Join( personService.Queryable().AsNoTracking(),
                            o => new { PersonId = o.PersonId, IsDeceased = false, RecordTypeValueId = personRecordTypeId, RecordStatusValueId = dvActive.Id },
                            p => new { PersonId = p.Id, IsDeceased = p.IsDeceased, RecordTypeValueId = p.RecordTypeValueId.Value, RecordStatusValueId = p.RecordStatusValueId.Value },
                            ( pn, p ) => new { Person = p, PhoneNumber = pn } )
                            .Join( memberService.Queryable().AsNoTracking(),
                            pn => pn.Person.Id,
                            m => m.PersonId,
                            ( o, m ) => new { PersonNumber = o.PhoneNumber, GroupMember = m } );
                    }

                    var familyIdQry = groupService.Queryable().Where( g => tmpQry.Any( o => o.GroupMember.GroupId == g.Id ) && g.GroupTypeId == familyGroupTypeId )
                        .Select( g => g.Id )
                        .Distinct();

                    int maxResults = checkInState.CheckInType != null ? checkInState.CheckInType.MaxSearchResults : 100;
                    if ( maxResults > 0 )
                    {
                        familyIdQry = familyIdQry.Take( maxResults );
                    }

                    var familyIds = familyIdQry.ToList();

                    // Load the family members
                    var familyMembers = memberService
                        .Queryable( "Group,GroupRole,Person" ).AsNoTracking()
                        .Where( m => familyIds.Contains( m.GroupId ) )
                        .ToList();

                    // Add each family
                    foreach ( int familyId in familyIds )
                    {
                        // Get each of the members for this family
                        var familyMemberQry = familyMembers
                            .Where( m =>
                                m.GroupId == familyId &&
                                m.Person.NickName != null );

                        if ( checkInState.CheckInType != null && checkInState.CheckInType.PreventInactivePeopele && dvActive != null )
                        {
                            familyMemberQry = familyMemberQry
                                .Where( m =>
                                    m.Person.RecordStatusValueId == dvActive.Id );
                        }

                        var thisFamilyMembers = familyMemberQry.ToList();

                        if ( thisFamilyMembers.Any() )
                        {
                            var group = thisFamilyMembers
                                .Select( m => m.Group )
                                .FirstOrDefault();

                            var firstNames = thisFamilyMembers
                                .OrderBy( m => m.GroupRole.Order )
                                .ThenBy( m => m.Person.BirthYear )
                                .ThenBy( m => m.Person.BirthMonth )
                                .ThenBy( m => m.Person.BirthDay )
                                .ThenBy( m => m.Person.Gender )
                                .Select( m => m.Person.NickName )
                                .ToList();

                            var family = new CheckInFamily();
                            family.Group = group.Clone( false );
                            family.Caption = group.ToString();
                            family.SubCaption = firstNames.AsDelimited( ", " );
                            checkInState.CheckIn.Families.Add( family );
                        }
                    }
                }
                else if ( checkInState.CheckIn.SearchType.Guid.Equals( new Guid( Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_NAME ) ) )
                {
                    var people = personService.GetByFullName( checkInState.CheckIn.SearchValue, false ).AsNoTracking();
                    if ( checkInState.CheckInType != null && checkInState.CheckInType.PreventInactivePeopele && dvActive != null )
                    {
                        people = people.Where( p => p.RecordStatusValueId == dvActive.Id );
                    }

                    foreach ( var person in people )
                    {
                        foreach ( var group in person.Members.Where( m => m.Group.GroupTypeId == familyGroupTypeId ).Select( m => m.Group ).ToList() )
                        {
                            var family = checkInState.CheckIn.Families.Where( f => f.Group.Id == group.Id ).FirstOrDefault();
                            if ( family == null )
                            {
                                family = new CheckInFamily();
                                family.Group = group.Clone( false );
                                family.Group.LoadAttributes( rockContext );
                                family.Caption = group.ToString();
                                family.SubCaption = memberService.GetFirstNames( group.Id ).ToList().AsDelimited( ", " );
                                
                               
                                checkInState.CheckIn.Families.Add( family );
                            }
                        }
                    }
                }
                //Barcode search
                else if ( checkInState.CheckIn.SearchType.Guid.Equals( new Guid( DefinedValueGuids.CHECKIN_SEARCH_TYPE_BARCODE ) ) )
                {
                    string barCode = checkInState.CheckIn.SearchValue.ToString();
                    bool familyIdCard = false;

                    //Old Family Id cards have 4 characters and are not numeric.
                    if ( barCode.Trim().Split( '-' )[0].Length == 4 && ( !IsNumeric( barCode.Trim().Replace( "-", "" ) ) ) )
                    {
                        familyIdCard = true;
                    }
                    else if ( IsNumeric( barCode.Trim().Replace( "-", "" ) ) )
                    {
                        familyIdCard = false;
                    }
                    else
                    {
                        errorMessages.Add( "Invalid Search" );
                        return false;
                    }

                    var personRecordTypeId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
                    var attributeService = new AttributeService( rockContext );
                    var attributeValueService = new AttributeValueService( rockContext );
                    var personAliasService = new PersonAliasService( rockContext );
                    Guid familyIDCardGuid = AttributeGuids.FAMILY_ID_CARD.AsGuid();
                    Guid rockCheckInCardGuid = AttributeGuids.CHECKINCARD.AsGuid();
                    IQueryable<GroupMember> familyQry;

                    if ( familyIdCard )
                    {
                        string familyCardBarcode = barCode.Split( '-' )[0];
                        // Find the families with any member who has the family Id Card
                        familyQry = from m in memberService.Queryable().AsNoTracking()
                                    join pa in personAliasService.Queryable().AsNoTracking() on m.Person.Id equals pa.PersonId
                                    join av in attributeValueService.Queryable().AsNoTracking() on pa.AliasPersonId equals av.EntityId
                                    join a in attributeService.Queryable().AsNoTracking() on av.AttributeId equals a.Id
                                    where m.Group.GroupType.Id == familyGroupTypeId
                                            && m.Person.RecordTypeValueId == personRecordTypeId
                                            && a.Guid == familyIDCardGuid
                                            && av.Value == familyCardBarcode
                                    select m;
                    }
                    else
                    {
                        //Get the first portion of the barcode that ties to the Rock Check-In card number.
                        //The second numeric portion that relates to the person will be used at a later phase.
                        int rockCheckInID = barCode.Split( '-' )[0].AsInteger();


                        // Find the families that match the family group Id
                        familyQry = from m in memberService.Queryable().AsNoTracking()
                                    join av in attributeValueService.Queryable().AsNoTracking() on m.GroupId equals av.EntityId
                                    join a in attributeService.Queryable().AsNoTracking() on av.AttributeId equals a.Id
                                    where a.Guid == rockCheckInCardGuid
                                           && av.Value != "" && av.Value == rockCheckInID.ToString()
                                    select m;
                    }

                    var familyIdQry = familyQry
                        .Select( m => m.GroupId )
                        .Distinct();

                    int maxResults = checkInState.CheckInType != null ? checkInState.CheckInType.MaxSearchResults : 100;
                    if ( maxResults > 0 )
                    {
                        familyIdQry = familyIdQry.Take( maxResults );
                    }

                    var familyIds = familyIdQry.ToList();

                    // Load the family members
                    var familyMembers = memberService
                        .Queryable( "Group,GroupRole,Person" ).AsNoTracking()
                        .Where( m => familyIds.Contains( m.GroupId ) )
                        .ToList();

                    // Add each family
                    foreach ( int familyId in familyIds )
                    {
                        // Get each of the members for this family
                        var familyMemberQry = familyMembers
                            .Where( m =>
                                m.GroupId == familyId &&
                                m.Person.NickName != null );

                        if ( checkInState.CheckInType != null && checkInState.CheckInType.PreventInactivePeopele && dvActive != null )
                        {
                            familyMemberQry = familyMemberQry
                                .Where( m =>
                                    m.Person.RecordStatusValueId == dvActive.Id );
                        }

                        var thisFamilyMembers = familyMemberQry.ToList();

                        if ( thisFamilyMembers.Any() )
                        {
                            var group = thisFamilyMembers
                                .Select( m => m.Group )
                                .FirstOrDefault();

                            var firstNames = thisFamilyMembers
                                .OrderBy( m => m.GroupRole.Order )
                                .ThenBy( m => m.Person.BirthYear )
                                .ThenBy( m => m.Person.BirthMonth )
                                .ThenBy( m => m.Person.BirthDay )
                                .ThenBy( m => m.Person.Gender )
                                .Select( m => m.Person.NickName )
                                .ToList();

                            var family = new CheckInFamily();
                            family.Group = group.Clone( false );
                            family.Caption = group.ToString();
                            family.SubCaption = firstNames.AsDelimited( ", " );
                            checkInState.CheckIn.Families.Add( family );
                        }
                    }
                }
                else
                {
                    errorMessages.Add( "Invalid Search Type" );
                    return false;
                }

                return true;
            }

            errorMessages.Add( "Invalid Check-in State" );
            return false;
        }

        private bool IsNumeric( string input )
        {
            return Regex.IsMatch( input, @"^\d+$" );
        }
    }
}