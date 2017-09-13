using System;
using System.Collections.Generic;
using System.Web;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Transactions;
using Rock.Web.Cache;

namespace org.willowcreek.Financial
{
    public class GeneratePledgeStatusLetterTransaction : ITransaction
    {
        
        #region Properties

        /// <summary>
        /// Gets or sets the type of the binary file.
        /// </summary>
        public BinaryFileType BinaryFileType { get; set; }

        /// <summary>
        /// Gets or sets the requestor.
        /// </summary>
        public Person Requestor { get; set; }

        /// <summary>
        /// The AccountId to find contributions for
        /// </summary>
        public int AccountId { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeContributorsWithNoPledge { get; set; }
        public string Title { get; set; }
        public string AddressLine { get; set; }
        public string GivingId { get; set; }
        /// <summary>
        /// Gets or sets the size of the chapter.
        /// </summary>
        public int ChapterSize { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public HttpContext Context { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public HttpResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the database timeout.
        /// </summary>
        public int? DatabaseTimeout { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContributionStatementTransaction"/> class.
        /// </summary>
        public GeneratePledgeStatusLetterTransaction()
        {
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            if ( string.IsNullOrWhiteSpace( GivingId ) )
            {
                PledgeStatusLetters.GenerateAll( BinaryFileType.Id, AccountId, EndDate, IncludeContributorsWithNoPledge, Title, AddressLine, ChapterSize );
            }
            else
            {
                PledgeStatusLetters.GenerateOne( BinaryFileType.Id, false, AccountId, EndDate, Title, AddressLine, GivingId );
            }
            using ( var rockContext = new RockContext() )
            {
                // Send email
                var emailMergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null );
                emailMergeFields.Add( "Person", Requestor );

                var appRoot = GlobalAttributesCache.Read( rockContext ).GetValue( "ExternalApplicationRoot" );

                var recipients = new List<Rock.Communication.RecipientData>();
                recipients.Add( new Rock.Communication.RecipientData( Requestor.Email, emailMergeFields ) );
                Email.Send( Requestor.Email, Requestor.FullName, "Pledge Status Letters have been generated", new List<string> { Requestor.Email }, "Files can be found in App_Data/Files. Remember to open each one, select all, and hit F9 to correct page numbers.", appRoot, createCommunicationHistory: true );
            }
        }
        #endregion
    }
}
