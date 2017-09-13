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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class TransactionSettledInfo : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            AddColumn("dbo.FinancialTransaction", "IsSettled", c => c.Boolean());
            AddColumn("dbo.FinancialTransaction", "SettledGroupId", c => c.String(maxLength: 100));
            AddColumn("dbo.FinancialTransaction", "SettledDate", c => c.DateTime());
            AddColumn("dbo.FinancialTransaction", "IsReconciled", c => c.Boolean());
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropColumn("dbo.FinancialTransaction", "IsReconciled");
            DropColumn("dbo.FinancialTransaction", "SettledDate");
            DropColumn("dbo.FinancialTransaction", "SettledGroupId");
            DropColumn("dbo.FinancialTransaction", "IsSettled");
        }
    }
}
