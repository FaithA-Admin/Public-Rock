﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.Workflow;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using Rock.Workflow.Action.CheckIn;

namespace org.secc.FamilyCheckin
{
    /// <summary>
    /// Finds family members in a given family
    /// </summary>
    [ActionCategory( "SECC > Check-In" )]
    [Description( "Searches and loads person by PIN (does not load family)." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Load Person By PIN" )]
    [BooleanField( "Search By Phone", "Should we also allow searching by phone number? This will only return true if a person is found by PIN." )]
    [IntegerField( "Minimum Phone Length", "The minimum number of digits for a phone number.", false, 7 )]
    [BooleanField("Search By Pin", "Should we search by PIN?")]
    public class LoadPersonByPIN : CheckInActionComponent
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
        public override bool Execute( RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            if ( checkInState != null )
            {
                var searchValue = checkInState.CheckIn.SearchValue.Trim();

                long n;
                bool isNumeric = long.TryParse( searchValue, out n );

                if ( isNumeric )
                {
                    UserLoginService userLogin = new UserLoginService( rockContext );
                    var user = userLogin.GetByUserName( searchValue );
                    if ( user != null && GetAttributeValue(action, "SearchByPin").AsBoolean() )
                    {
                        var memberService = new GroupMemberService( rockContext );
                        var families = user.Person.GetFamilies();
                        foreach ( var group in families )
                        {
                            var family = checkInState.CheckIn.Families.Where( f => f.Group.Id == group.Id ).FirstOrDefault();
                            if ( family == null )
                            {
                                family = new CheckInFamily();
                                family.Group = group.Clone( false );
                                family.Group.LoadAttributes( rockContext );
                                family.Caption = group.ToString();
                                family.SubCaption = "";
                                checkInState.CheckIn.Families.Add( family );
                            }
                            var person = new CheckInPerson();
                            person.Person = user.Person;
                            person.Selected = true;
                            family.People = new List<CheckInPerson>() { person };
                            family.Selected = true;
                        }

                    }
                    else if ( GetAttributeValue( action, "SearchByPhone" ).AsBoolean() )
                    {
                        //Look for person by phone number
                        if ( searchValue.Length < ( GetAttributeValue( action, "MinimumPhoneLength" ).AsIntegerOrNull() ?? 7 ) )
                        {
                            return false;
                        }
                        PhoneNumberService phoneNumberService = new PhoneNumberService( rockContext );
                        var phoneNumbers = phoneNumberService.GetBySearchterm( searchValue ).DistinctBy( pn => pn.PersonId ).Take(20).ToList();

                        foreach ( var phoneNumber in phoneNumbers )
                        {
                            var person = phoneNumber.Person;
                            var family = person.GetFamilies().FirstOrDefault();
                            var cFamily = new CheckInFamily();
                            cFamily.Group = family.Clone( false );
                            cFamily.Group.LoadAttributes( rockContext );
                            cFamily.Caption = family.ToString();
                            cFamily.SubCaption = "";
                            checkInState.CheckIn.Families.Add( cFamily );
                            var cPerson = new CheckInPerson();
                            cPerson.Person = person;
                            cFamily.People = new List<CheckInPerson>() { cPerson };
                        }
                        return true;
                    }
                }
                return true;
            }
            return false;
        }
    }
}