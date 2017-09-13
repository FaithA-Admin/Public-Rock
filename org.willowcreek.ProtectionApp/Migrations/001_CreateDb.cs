using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace org.willowcreek.ProtectionApp.Migrations
{
    [MigrationNumber(1, "1.2")]
    public class CreateDb : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            CreateQuestionnaireTable();  
            CreateReferenceTable();
        }

        /// <summary>
        /// Creates the questionnaire table.
        /// </summary>
        private void CreateQuestionnaireTable()
        {
            Sql(@"
CREATE TABLE [dbo].[_org_willowcreek_ProtectionApp_Questionnaire](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubmissionDate] [datetime] NOT NULL,
	[ApplicantPersonAliasGuid] [uniqueidentifier] NOT NULL,
	[ApplicantSsn] [varchar](100) NULL,
	[LegalFirstName] [varchar](100) NULL,
	[MiddleName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[FullLegalName] [varchar](100) NULL,
	[Nickname] [varchar](100) NULL,
	[MaidenName] [varchar](100) NULL,
	[CurrentAddressStreet] [varchar](100) NULL,
	[CurrentAddressCity] [varchar](50) NULL,
	[CurrentAddressState] [varchar](50) NULL,
	[CurrentAddressZip] [varchar](50) NULL,
	[TimeAtCurrentAddress] [int] NULL,
	[PreviousAddressStreet] [varchar](100) NULL,
	[PreviousAddressCity] [varchar](50) NULL,
	[PreviousAddressState] [varchar](50) NULL,
	[PreviousAddressZip] [varchar](50) NULL,
	[HomePhone] [varchar](15) NULL,
	[MobilePhone] [varchar](15) NULL,
	[EmailAddress] [varchar](100) NULL,
	[DateOfBirth] [datetime] NULL,
	[Gender] [int] NULL,
	[MaritalStatus] [int] NULL,
	[ChildrenCount] [int] NULL,
	[ChildrenAges] [varchar](100) NULL,
	[LegalGuardian] [bit] NULL,
	[AttendWccc] [bit] NULL,
	[StartWccc] [datetime] NULL,
	[PornographyAddiction] [int] NULL,
	[AlcoholAddiction] [int] NULL,
	[DrugAddiction] [int] NULL,
	[AddictionExplain] [varchar](2000) NULL,
	[PornographyVulnerable] [bit] NULL,
	[PornographyVulnerableExplain] [varchar](2000) NULL,
	[DcfsInvestigation] [bit] NULL,
	[DcfsInvestigationExplain] [varchar](2000) NULL,
	[OrderOfProtection] [bit] NULL,
	[OrderOfProtectionExplain] [varchar](2000) NULL,
	[CommittedOrAccused] [bit] NULL,
	[CommittedOrAccusedExplain] [varchar](2000) NULL,
	[RelationshipVulnerable] [bit] NULL,
	[RelationshipVulnerableExplain] [varchar](2000) NULL,
	[AskedToLeave] [bit] NULL,
	[AskedToLeaveExplain] [varchar](2000) NULL,
	[Reference1Name] [varchar](100) NULL,
	[Reference1Phone] [varchar](15) NULL,
	[Reference1Email] [varchar](100) NULL,
	[Reference1NatureOfAssociation] [varchar](100) NULL,
	[Reference2Name] [varchar](100) NULL,
	[Reference2Phone] [varchar](15) NULL,
	[Reference2Email] [varchar](100) NULL,
	[Reference2NatureOfAssociation] [varchar](100) NULL,
	[Reference3Name] [varchar](100) NULL,
	[Reference3Phone] [varchar](15) NULL,
	[Reference3Email] [varchar](100) NULL,
	[Reference3NatureOfAssociation] [varchar](100) NULL,
	[CorrectInfo] [bit] NULL,
	[AuthorizeRelease] [bit] NULL,
	[AuthorizeReference] [bit] NULL,
	[Signature] [varchar](100) NULL,
	[SignatureDate] [datetime] NULL,
	[GuardianSignature] [varchar](100) NULL,
	[GuardianSignatureDate] [datetime] NULL,
	[WorkflowId] [varchar](50) NULL,
	[CampusId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[CreatedByPersonAliasId] [int] NULL,
	[ModifiedByPersonAliasId] [int] NULL,
	[Guid] [uniqueidentifier] NULL,
	[ForeignId] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo._org_willowcreek_ProtectionApp_Questionnaire] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
");
        }

        /// <summary>
        /// Creates the reference table.
        /// </summary>
        private void CreateReferenceTable()
        {
            Sql(@"
CREATE TABLE [dbo].[_org_willowcreek_ProtectionApp_Reference](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubmissionDate] [datetime] NULL,
	[QuestionnaireId] [int] NULL,
	[ReferencePersonAliasGuid] [uniqueidentifier] NOT NULL,
	[FirstName] [varchar](100) NULL,
	[MiddleName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[JuniorHighStudent] [bit] NULL,
	[KnownMoreThanOneYear] [bit] NULL,
	[IsReference18] [bit] NULL,
	[NatureOfRelationship] [varchar](100) NULL,
	[MaintainRelationships] [bit] NULL,
	[MaintainRelationshipsExplain] [varchar](2000) NULL,
	[RespectHealthyRelationalBoundaries] [bit] NULL,
	[RespectHealthyRelationalBoundariesExplain] [varchar](2000) NULL,
	[CriminalOffenses] [bit] NULL,
	[CriminalOffensesExplain] [varchar](2000) NULL,
	[ManipulativeBehavior] [bit] NULL,
	[ManipulativeBehaviorExplain] [varchar](2000) NULL,
	[InflictedEmotionalHarm] [bit] NULL,
	[InflictedEmotionalHarmExplain] [varchar](2000) NULL,
	[TrustInChildCare] [bit] NULL,
	[TrustInChildCareExplain] [varchar](2000) NULL,
	[WouldRecommend] [bit] NULL,
	[WouldRecommendExplain] [varchar](2000) NULL,
	[Signature] [varchar](100) NULL,
	[SignatureDate] [datetime] NULL,
	[CampusId] [int] NULL,
	[WorkflowId] [varchar](50) NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[CreatedByPersonAliasId] [int] NULL,
	[ModifiedByPersonAliasId] [int] NULL,
	[Guid] [uniqueidentifier] NULL,
	[ForeignId] [nvarchar](50) NULL,
    [RefNumber] [int] NULL,
 CONSTRAINT [PK_dbo._org_willowcreek_ProtectionApp_Reference] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
");
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            Sql(@"DROP TABLE [dbo].[_org_willowcreek_ProtectionApp_Questionnaire]" );
            Sql(@"DROP TABLE [dbo].[_org_willowcreek_ProtectionApp_Reference]" );
        }
    }
}
