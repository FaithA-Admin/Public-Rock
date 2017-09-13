using Rock.Plugin;

namespace org.willowcreek.ProtectionApp.Migrations
{
    [MigrationNumber(5, "1.2")]
    public class AllowReferenceNulls : Migration
    {
        /// <summary>
        /// Ups this instance.
        /// </summary>
        public override void Up()
        {
            string sql = @"
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [AttendWCCC] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [LegalGuardian] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [PornographyVulnerable] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [DcfsInvestigation] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [OrderOfProtection] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [CommittedOrAccused] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [RelationshipVulnerable] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [AskedToLeave] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [CorrectInfo] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [AuthorizeRelease] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Questionnaire] ALTER COLUMN [AuthorizeReference] BIT NULL

            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [JuniorHighStudent] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [KnownMoreThanOneYear] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [IsReference18] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [MaintainRelationships] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [RespectHealthyRelationalBoundaries] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [CriminalOffenses] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [ManipulativeBehavior] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [InflictedEmotionalHarm] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [TrustInChildCare] BIT NULL
            ALTER TABLE [_org_willowcreek_ProtectionApp_Reference] ALTER COLUMN [WouldRecommend] BIT NULL";

            Sql(sql);
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
        }
    }
}