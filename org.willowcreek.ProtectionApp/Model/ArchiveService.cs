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
using org.willowcreek.ProtectionApp.Data;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// Archive Service class
    /// </summary>
    public partial class ArchiveService : Service<Archive>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public ArchiveService(ProtectionAppContext context)
            : base(context)
        {
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class ArchiveExtensionMethods
    {
        /// <summary>
        /// Clones this Archive object to a new Archive object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static Archive Clone( this Archive source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as Archive;
            }
            else
            {
                var target = new Archive();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another Archive object to this Archive object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this Archive target, Archive source )
        {
            target.Id = source.Id;
            target.ApplicationBinaryFileId = source.ApplicationBinaryFileId;
            target.ApplicationDate = source.ApplicationDate;
            target.ArchiveDateTime = source.ArchiveDateTime;
            target.BackgroundCheckBinaryFileId = source.BackgroundCheckBinaryFileId;
            target.BackgroundCheckDate = source.BackgroundCheckDate;
            target.BackgroundCheckResult = source.BackgroundCheckResult;
            target.PersonAliasGuid = source.PersonAliasGuid;
            target.PolicyAcknowledgmentDate = source.PolicyAcknowledgmentDate;
            target.ProtectionStatus = source.ProtectionStatus;
            target.Reference1BinaryFileId = source.Reference1BinaryFileId;
            target.Reference1Date = source.Reference1Date;
            target.Reference2BinaryFileId = source.Reference2BinaryFileId;
            target.Reference2Date = source.Reference2Date;
            target.Reference3BinaryFileId = source.Reference3BinaryFileId;
            target.Reference3Date = source.Reference3Date;
            target.CreatedDateTime = source.CreatedDateTime;
            target.ModifiedDateTime = source.ModifiedDateTime;
            target.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            target.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
