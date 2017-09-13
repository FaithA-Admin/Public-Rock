using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;

namespace org.willowcreek.Cars.Model
{
    /// <summary>
    /// ServiceAreaBan Service class
    /// </summary>
    public partial class VehicleService : Service<Vehicle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public VehicleService( RockContext context ) : base( context )
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( Vehicle item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class ServiceAreaBanExtensionMethods
    {
        /// <summary>
        /// Clones this ServiceAreaBan object to a new ServiceAreaBan object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static Vehicle Clone( this Vehicle source, bool deepCopy )
        {
            if ( deepCopy )
            {
                return source.Clone() as Vehicle;
            }
            else
            {
                var target = new Vehicle();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another ServiceAreaBan object to this ServiceAreaBan object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this Vehicle target, Vehicle source )
        {
            target.Id = source.Id;
            target.ForeignGuid = source.ForeignGuid;
            target.ForeignKey = source.ForeignKey;
            target.CreatedDateTime = source.CreatedDateTime;
            target.ModifiedDateTime = source.ModifiedDateTime;
            target.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            target.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
