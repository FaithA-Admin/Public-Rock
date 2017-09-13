//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
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
using System.Linq;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// Metric Service class
    /// </summary>
    public partial class MetricService : Service<Metric>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public MetricService(RockContext context) : base(context)
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
        public bool CanDelete( Metric item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class MetricExtensionMethods
    {
        /// <summary>
        /// Clones this Metric object to a new Metric object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static Metric Clone( this Metric source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as Metric;
            }
            else
            {
                var target = new Metric();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another Metric object to this Metric object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this Metric target, Metric source )
        {
            target.Id = source.Id;
            target.AdminPersonAliasId = source.AdminPersonAliasId;
            target.DataViewId = source.DataViewId;
            target.Description = source.Description;
            target.EnableAnalytics = source.EnableAnalytics;
            target.ForeignGuid = source.ForeignGuid;
            target.ForeignKey = source.ForeignKey;
            target.IconCssClass = source.IconCssClass;
            target.IsCumulative = source.IsCumulative;
            target.IsSystem = source.IsSystem;
            target.LastRunDateTime = source.LastRunDateTime;
            target.MetricChampionPersonAliasId = source.MetricChampionPersonAliasId;
            target.NumericDataType = source.NumericDataType;
            target.ScheduleId = source.ScheduleId;
            target.SourceSql = source.SourceSql;
            target.SourceValueTypeId = source.SourceValueTypeId;
            target.Subtitle = source.Subtitle;
            target.Title = source.Title;
            target.XAxisLabel = source.XAxisLabel;
            target.YAxisLabel = source.YAxisLabel;
            target.CreatedDateTime = source.CreatedDateTime;
            target.ModifiedDateTime = source.ModifiedDateTime;
            target.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            target.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}