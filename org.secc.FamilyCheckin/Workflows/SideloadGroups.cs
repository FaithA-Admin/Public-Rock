﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;

namespace org.secc.FamilyCheckin
{
    /// <summary>
    /// Injects groups into the check-in object which may have been filtered out previously
    /// </summary>
    [ActionCategory( "SECC > Check-In" )]
    [Description( "Injects groups into the check-in object which may have been filtered out previously" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Sideload Groups" )]
    [AttributeField( Rock.SystemGuid.EntityType.PERSON, "Checkin Groups Attribute", "The person attribute which contains the groups that person is allowed to check-into." )]
    public class SideloadGroups : CheckInActionComponent
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
            if ( checkInState != null )
            {
                var checkinAttributeKey = "";
                var checkinAttributeGuid = GetAttributeValue( action, "CheckinGroupsAttribute" );
                if ( !string.IsNullOrWhiteSpace( checkinAttributeGuid ) )
                {
                    var attributeCache = AttributeCache.Read( checkinAttributeGuid.AsGuid() );
                    if ( attributeCache != null )
                    {
                        checkinAttributeKey = attributeCache.Key;
                    }
                }
                foreach ( var family in checkInState.CheckIn.GetFamilies( true ) )
                {
                    foreach ( var person in family.GetPeople( false ) )
                    {
                        if ( person.Person.Attributes == null )
                        {
                            person.Person.LoadAttributes();
                        }
                        var groupGuidsString = person.Person.GetAttributeValue( checkinAttributeKey );
                        if ( !string.IsNullOrWhiteSpace( groupGuidsString ) )
                        {
                            var guids = groupGuidsString
                                .Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries )
                                .Select( g => g.AsGuid() )
                                .ToList();

                            foreach ( var groupType in person.GroupTypes )
                            {
                                var kioskGroup = checkInState.Kiosk.ActiveGroupTypes( checkInState.ConfiguredGroupTypes )
                                    .Where( g => g.GroupType.Id == groupType.GroupType.Id )
                                    .FirstOrDefault();

                                if ( kioskGroup != null )
                                {
                                    var injectGroups = kioskGroup.KioskGroups
                                        .Where( g =>
                                         g.IsCheckInActive
                                         && guids.Contains( g.Group.Guid )
                                         );

                                    if ( !injectGroups.Any() )
                                    {
                                        continue;
                                    }

                                    foreach ( var injectGroup in injectGroups )
                                    {
                                        var checkInGroup = new CheckInGroup();
                                        checkInGroup.Group = injectGroup.Group.Clone( false );
                                        checkInGroup.Group.CopyAttributesFrom( injectGroup.Group );
                                        groupType.Groups.Insert( 0, checkInGroup );
                                    }
                                    groupType.Groups = groupType.Groups.DistinctBy( g => g.Group.Id ).ToList();
                                }
                            }
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }
}