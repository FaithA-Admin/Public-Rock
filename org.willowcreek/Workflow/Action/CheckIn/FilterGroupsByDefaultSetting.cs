using System;
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

namespace org.willowcreek.Workflow.Action.CheckIn
{
    /// <summary>
    /// Removes (or excludes) the groups for each selected family member that are not specific to their age or birthdate.
    /// </summary>
    [ActionCategory( "Check-In" )]
    [Description( "Removes (or excludes) groups marked as Default if there are any groups available that are not Default" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Filter Groups By Default Setting" )]

    [BooleanField( "Remove", "Select 'Yes' if groups should be be removed.  Select 'No' if they should just be marked as excluded.", true, "", 0 )]
    public class FilterGroupsByDefaultSetting : CheckInActionComponent
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
            if ( checkInState == null )
            {
                return false;
            }

            var family = checkInState.CheckIn.CurrentFamily;
            if ( family != null )
            {
                var remove = GetAttributeValue( action, "Remove" ).AsBoolean();

                foreach ( var person in family.People )
                {
                    // If the person qualifies for any member only groups
                    if ( person.GroupTypes.SelectMany( t => t.Groups.Select( g => g.Group ) ).Any( g => ( g.GetAttributeValue( "IsDefault" ).AsBooleanOrNull() ?? false ) == false ) )
                    {
                        // Remove all the default groups they qualified for
                        foreach ( var groupType in person.GroupTypes.ToList() )
                        {
                            foreach ( var group in groupType.Groups.ToList() )
                            {
                                var isDefault = group.Group.GetAttributeValue( "IsDefault" ).AsBooleanOrNull() ?? false;

                                if ( isDefault )
                                {
                                    if ( remove )
                                    {
                                        groupType.Groups.Remove( group );
                                    }
                                    else
                                    {
                                        group.ExcludedByFilter = true;
                                    }
                                }
                            }
                        }
                    }
                    // If the person does not qualify for any member only groups
                    //else
                    //{
                    //    // Remove all but the highest ordered default group

                    //    var groupTypesWithGroups = person.GroupTypes.Where( x => x.Groups.Any() ).Select( x => new GroupTypeStack { GroupType = x, Orders = new List<int>() } ).ToList();

                    //    if ( groupTypesWithGroups.Any() )
                    //    {
                    //        foreach ( var groupTypeWithGroups in groupTypesWithGroups )
                    //        {
                    //            groupTypeWithGroups.Orders = PushOrders( groupTypeWithGroups.GroupType.GroupType, new Stack<int>() ).ToList();
                    //        }

                    //        var query = groupTypesWithGroups.AsEnumerable().OrderByDescending(x => x.Orders[0]);
                    //        var max = groupTypesWithGroups.Max( x => x.Orders.Count );
                    //        for ( var i = 1; i < max; i++ )
                    //        {
                    //            var z = i as int?; // Can't use i directly because it will be out of range when materializing the query
                    //            query = query.ThenByDescending( x => x.Orders.Count > z.Value ? x.Orders[z.Value] : 0 );
                    //        }
                    //        var defaultGroupType = query.Select( x => x.GroupType ).First();

                    //        foreach ( var groupType in person.GroupTypes.Where(x => x != defaultGroupType).ToList() )
                    //        {
                    //            foreach ( var group in groupType.Groups.ToList() )
                    //            {
                    //                if ( remove )
                    //                {
                    //                    groupType.Groups.Remove( group );
                    //                }
                    //                else
                    //                {
                    //                    group.ExcludedByFilter = true;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    
                }
            }

            return true;
        }

        //public class GroupTypeStack
        //{
        //    public CheckInGroupType GroupType;
        //    public List<int> Orders;
        //}

        //public Stack<int> PushOrders( GroupTypeCache gt, Stack<int> orders )
        //{
        //    orders.Push( gt.Order );
        //    if ( gt.ParentGroupTypes.Count == 1 )
        //    { 
        //        return PushOrders( gt.ParentGroupTypes.Single(), orders );
        //    }
        //    else
        //    {
        //        return orders;
        //    }
        //}
    }
}