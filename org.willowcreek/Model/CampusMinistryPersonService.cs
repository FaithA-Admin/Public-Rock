//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
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

namespace org.willowcreek.Model
{
    /// <summary>
    /// CampusMinistryPerson Service class
    /// </summary>
    public partial class CampusMinistryPersonService : Service<CampusMinistryPerson>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampusMinistryPersonService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public CampusMinistryPersonService(WillowContext context) : base(context)
        {
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class CampusMinistryPersonExtensionMethods
    {
        /// <summary>
        /// Clones this CampusMinistryPerson object to a new CampusMinistryPerson object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static CampusMinistryPerson Clone( this CampusMinistryPerson source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as CampusMinistryPerson;
            }
            else
            {
                var target = new CampusMinistryPerson();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another CampusMinistryPerson object to this CampusMinistryPerson object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this CampusMinistryPerson target, CampusMinistryPerson source )
        {
            target.Id = source.Id;
            target.Campus = source.Campus;
            target.Ministry = source.Ministry;
            target.PersonAliasGuid = source.PersonAliasGuid;
            target.CreatedDateTime = source.CreatedDateTime;
            target.ModifiedDateTime = source.ModifiedDateTime;
            target.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            target.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
