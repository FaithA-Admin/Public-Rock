using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;

using Rock.Data;
using Rock.Model;

namespace org.willowcreek.Cars.Model
{
    /// <summary>
    /// Vehicle
    /// </summary>
    [Table( "_org_willowcreek_Cars_Vehicle" )]
    [DataContract]
    public class Vehicle : Model<Vehicle>, IRockEntity
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the donor person alias identifier.
        /// </summary>
        /// <value>
        /// The donor person alias identifier.
        /// </value>
        [DataMember]
        public int? DonorPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the DonorType.
        /// </summary>
        /// <value>
        /// The DonorType.
        /// </value>
        [Required]
        [DataMember]
        public DonorType DonorType { get; set; }

        /// <summary>
        /// Will the vehicle be dropped off.
        /// </summary>
        /// <value>
        /// The is drop off.
        /// </value>
        [DataMember]
        public bool? IsDropOff { get; set; }

        /// <summary>
        /// Gets or sets the location identifier.
        /// </summary>
        /// <value>
        /// The location identifier.
        /// </value>
        [DataMember]
        public int? PickUpLocationId { get; set; }

        /// <summary>
        /// Gets or sets the entered date.
        /// </summary>
        /// <value>
        /// The entered date.
        /// </value>
        [Required]
        [DataMember]
        public DateTime DateEntered { get; set; }

        /// <summary>
        /// Gets or sets the date in inventory.
        /// </summary>
        /// <value>
        /// The date in inventory.
        /// </value>
        [DataMember]
        public DateTime? DateInInventory { get; set; }

        /// <summary>
        /// Gets or sets the completed date.
        /// </summary>
        /// <value>
        /// The completed date.
        /// </value>
        [DataMember]
        public DateTime? DateCompleted { get; set; }

        /// <summary>
        /// Gets or sets the make value identifier.
        /// </summary>
        /// <value>
        /// The make value identifier.
        /// </value>
        [DataMember]
        [DefinedValue( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MAKE )]
        public int? MakeValueId { get; set; }

        /// <summary>
        /// Gets or sets the model value identifier.
        /// </summary>
        /// <value>
        /// The model value identifier.
        /// </value>
        [DataMember]
        [DefinedValue( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MODEL )]
        public int? ModelValueId { get; set; }

        /// <summary>
        /// Gets or sets the year identifier.
        /// </summary>
        /// <value>
        /// The year identifier.
        /// </value>
        [DataMember]
        public int? Year { get; set; }

        /// <summary>
        /// Gets or sets the mileage identifier.
        /// </summary>
        /// <value>
        /// The  mileage identifier.
        /// </value>
        [DataMember]
        public int? Mileage { get; set; }

        /// <summary>
        /// Gets or sets the color value identifier.
        /// </summary>
        /// <value>
        /// The color value identifier.
        /// </value>
        [DataMember]
        [DefinedValue( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_COLOR )]
        public int? ColorValueId { get; set; }

        /// <summary>
        /// Gets or sets the body style value identifier.
        /// </summary>
        /// <value>
        /// The body style value identifier.
        /// </value>
        [DataMember]
        [DefinedValue( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_BODY_STYLE )]
        public int? BodyStyleValueId { get; set; }

        /// <summary>
        /// Gets or sets the vin.
        /// </summary>
        /// <value>
        /// The vin.
        /// </value>
        [MaxLength( 25 )]
        [DataMember]
        public string Vin { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the assessed value type.
        /// </summary>
        /// <value>
        /// The assessed value type.
        /// </value>
        [DataMember]
        public AssessedValueType? AssessedValueType { get; set; }

        /// <summary>
        /// Gets or sets the Tax1098Summary.
        /// </summary>
        /// <value>
        /// The Tax1098Summary.
        /// </value>
        [MaxLength( 30 )]
        [DataMember]
        public string Tax1098Summary { get; set; }

        /// <summary>
        /// Gets or sets the Condition Type.
        /// </summary>
        /// <value>
        /// The Condition Type.
        /// </value>
        [DataMember]
        public ConditionType? Condition { get; set; }

        /// <summary>
        /// Gets or sets the estimated value.
        /// </summary>
        /// <value>
        /// The estimated value.
        /// </value>
        [DataMember]
        [BoundFieldTypeAttribute( typeof( Rock.Web.UI.Controls.CurrencyField ) )]
        public decimal? EstimatedValue { get; set; }

        /// <summary>
        /// Gets or sets the Status Type.
        /// </summary>
        /// <value>
        /// The Status Type.
        /// </value>
        [DataMember]
        public StatusType? Status { get; set; }

        /// <summary>
        /// Gets or sets the sub status value identifier.
        /// </summary>
        /// <value>
        /// The sub status value identifier.
        /// </value>
        [DataMember]
        [DefinedValue( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_BODY_STYLE )]
        public int? SubStatusValueId { get; set; }

        /// <summary>
        /// Gets or sets the stock number.
        /// </summary>
        /// <value>
        /// The stock number.
        /// </value>
        [Required]
        [DataMember]
        public int? TStockNumber { get; set; }

        /// <summary>
        /// Gets or sets the p stock number.
        /// </summary>
        /// <value>
        /// The p stock number.
        /// </value>
        [DataMember]
        public int? PStockNumber { get; set; }

        /// <summary>
        /// Gets or sets the photo1 identifier.
        /// </summary>
        /// <value>
        /// The photo1 identifier.
        /// </value>
        [DataMember]
        public int? Photo1Id { get; set; }

        /// <summary>
        /// Gets or sets the photo2 identifier.
        /// </summary>
        /// <value>
        /// The photo2 identifier.
        /// </value>
        [DataMember]
        public int? Photo2Id { get; set; }

        /// <summary>
        /// Gets or sets the photo3 identifier.
        /// </summary>
        /// <value>
        /// The photo3 identifier.
        /// </value>
        [DataMember]
        public int? Photo3Id { get; set; }

        /// <summary>
        /// Gets or sets the photo4 identifier.
        /// </summary>
        /// <value>
        /// The photo4 identifier.
        /// </value>
        [DataMember]
        public int? Photo4Id { get; set; }

        /// <summary>
        /// Gets or sets the Disposition Type Value identifier.
        /// </summary>
        /// <value>
        /// The Disposition Type Value identifier.
        /// </value>
        [DataMember]
        [DefinedValue()]
        public int? DispositionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the recipient person alias identifier.
        /// </summary>
        /// <value>
        /// The recipient person alias identifier.
        /// </value>
        [DataMember]
        public int? RecipientPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the disposition amount.
        /// </summary>
        /// <value>
        /// A <see cref="System.Decimal"/> representing the disposition amount.
        /// </value>
        [DataMember]
        [BoundFieldTypeAttribute( typeof( Rock.Web.UI.Controls.CurrencyField ) )]
        public decimal? DispositionAmount { get; set; }

        /// <summary>
        /// Gets or sets the disposition payment Type value identifier.
        /// </summary>
        /// <value>
        /// The disposition payment type value identifier.
        /// </value>
        [DataMember]
        [DefinedValue()]
        public int? DispositionPaymentTypeValueId { get; set; }

        /// <summary>
        /// Gets or sets the disposition note.
        /// </summary>
        /// <value>
        /// The disposition note.
        /// </value>
        [DataMember]
        public string DispositionNote { get; set; }

        /// <summary>
        /// Gets or sets the title file identifier.
        /// </summary>
        /// <value>
        /// The title file identifier.
        /// </value>
        [DataMember]
        public int? TitleFileId { get; set; }

        /// <summary>
        /// Gets or sets the last donar letter send date.
        /// </summary>
        /// <value>
        /// The last donar letter send date.
        /// </value>
        [DataMember]
        public DateTime? LastDonarLetterSendDate { get; set; }

        /// <summary>
        /// Gets or sets the last sold letter send date.
        /// </summary>
        /// <value>
        /// The last sold letter send date.
        /// </value>
        [DataMember]
        public DateTime? LastSoldLetterSendDate { get; set; }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the sub status value.
        /// </summary>
        /// <value>
        /// The sub status value.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue SubStatusValue { get; set; }

        /// <summary>
        /// Gets or sets the donar person alias.
        /// </summary>
        /// <value>
        /// The donar person alias.
        /// </value>
        [LavaInclude]
        public virtual PersonAlias DonorPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the pickup location.
        /// </summary>
        /// <value>
        /// The pickup location.
        /// </value>
        [LavaInclude]
        public virtual Location PickUpLocation { get; set; }

        /// <summary>
        /// Gets or sets the make value.
        /// </summary>
        /// <value>
        /// The make value.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue MakeValue { get; set; }

        /// <summary>
        /// Gets or sets the model value.
        /// </summary>
        /// <value>
        /// The model value.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue ModelValue { get; set; }

        /// <summary>
        /// Gets or sets the color value.
        /// </summary>
        /// <value>
        /// The color value.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue ColorValue { get; set; }

        /// <summary>
        /// Gets or sets the body style value.
        /// </summary>
        /// <value>
        /// The body style value.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue BodyStyleValue { get; set; }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        /// <value>
        /// The photo.
        /// </value>
        [LavaInclude]
        public virtual BinaryFile Photo1 { get; set; }

        /// <summary>
        /// Gets or sets the photo2.
        /// </summary>
        /// <value>
        /// The photo2.
        /// </value>
        [LavaInclude]
        public virtual BinaryFile Photo2 { get; set; }

        /// <summary>
        /// Gets or sets the photo3.
        /// </summary>
        /// <value>
        /// The photo3.
        /// </value>
        [LavaInclude]
        public virtual BinaryFile Photo3 { get; set; }

        /// <summary>
        /// Gets or sets the photo4.
        /// </summary>
        /// <value>
        /// The photo4.
        /// </value>
        [LavaInclude]
        public virtual BinaryFile Photo4 { get; set; }

        /// <summary>
        /// Gets or sets the type of the disposition.
        /// </summary>
        /// <value>
        /// The type of the disposition.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue DispositionType { get; set; }

        /// <summary>
        /// Gets or sets the disposition recipient.
        /// </summary>
        /// <value>
        /// The disposition recipient.
        /// </value>
        [LavaInclude]
        public virtual PersonAlias RecipientPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the disposition payment type value.
        /// </summary>
        /// <value>
        /// The disposition payment type value.
        /// </value>
        [LavaInclude]
        public virtual DefinedValue DispositionPaymentTypeValue { get; set; }

        /// <summary>
        /// Gets or sets the title file.
        /// </summary>
        /// <value>
        /// The title file.
        /// </value>
        [LavaInclude]
        public virtual BinaryFile TitleFile { get; set; }

        [LavaInclude]
        public virtual string StockNumber
        {
            get
            {
                return PStockNumber.HasValue ?
                    string.Format( "P{0:00000}", PStockNumber.Value ) :
                    ( TStockNumber.HasValue ?
                        string.Format( "T{0:00000}", TStockNumber.Value ) :
                        "None" );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method that will be called on an entity immediately before the item is saved by context
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="entry"></param>
        public override void PreSaveChanges( Rock.Data.DbContext dbContext, DbEntityEntry entry )
        {
            var vehicle = entry.Entity as Vehicle;
            if ( vehicle != null )
            {
                switch( vehicle.Status )
                {
                    case StatusType.Pending:
                        {
                            if ( !vehicle.TStockNumber.HasValue || vehicle.TStockNumber <= 0 )
                            {
                                vehicle.TStockNumber = ( new VehicleService( (RockContext)dbContext )
                                    .Queryable().AsNoTracking()
                                    .Where( v => v.TStockNumber.HasValue )
                                    .Select( v => (int?)v.TStockNumber ).Max() ?? 0 ) + 1;
                            }
                            break;
                        }
                    case StatusType.Inventory:
                        {
                            if ( !vehicle.PStockNumber.HasValue || vehicle.PStockNumber.Value <= 0 )
                            {
                                vehicle.PStockNumber = ( new VehicleService( (RockContext)dbContext )
                                    .Queryable().AsNoTracking()
                                    .Where( v => v.PStockNumber.HasValue )
                                    .Select( v => (int?)v.PStockNumber ).Max() ?? 0 ) + 1;
                            }
                            break;
                        }
                }
            }

            base.PreSaveChanges( dbContext, entry );
        }

        #endregion

    }
    /// <summary>
    /// 
    /// </summary>
    public partial class VehicleConfiguration : EntityTypeConfiguration<Vehicle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConfiguration"/> class.
        /// </summary>
        public VehicleConfiguration()
        {
            this.HasOptional( v => v.DonorPersonAlias ).WithMany().HasForeignKey( v => v.DonorPersonAliasId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.PickUpLocation ).WithMany().HasForeignKey( a => a.PickUpLocationId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.SubStatusValue).WithMany().HasForeignKey( a => a.SubStatusValueId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.MakeValue ).WithMany().HasForeignKey( a => a.MakeValueId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.ModelValue ).WithMany().HasForeignKey( a => a.ModelValueId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.ColorValue ).WithMany().HasForeignKey( a => a.ColorValueId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.BodyStyleValue ).WithMany().HasForeignKey( a => a.BodyStyleValueId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.Photo1 ).WithMany().HasForeignKey( a => a.Photo1Id ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.Photo2 ).WithMany().HasForeignKey( a => a.Photo2Id ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.Photo3 ).WithMany().HasForeignKey( a => a.Photo3Id ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.Photo4 ).WithMany().HasForeignKey( a => a.Photo4Id ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.DispositionType ).WithMany().HasForeignKey( a => a.DispositionTypeId ).WillCascadeOnDelete( false );
            this.HasRequired( v => v.RecipientPersonAlias ).WithMany().HasForeignKey( v => v.RecipientPersonAliasId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.DispositionPaymentTypeValue ).WithMany().HasForeignKey( a => a.DispositionPaymentTypeValueId ).WillCascadeOnDelete( false );
            this.HasOptional( a => a.TitleFile ).WithMany().HasForeignKey( a => a.TitleFileId ).WillCascadeOnDelete( false );

            this.HasEntitySetName( "Vehicles" );
        }
    }
    /// <summary>
    /// Donar Type
    /// </summary>
    public enum DonorType
    {
        /// <summary>
        /// Person
        /// </summary>
        Person = 0,

        /// <summary>
        /// Business
        /// </summary>
        Business = 1
    }

    /// <summary>
    /// Assessed Value Type
    /// </summary>
    public enum AssessedValueType
    {
        /// <summary>
        /// Donor Actual Value
        /// </summary>
        DonorActualValue = 0,

        /// <summary>
        /// KellyBlueBook
        /// </summary>
        KellyBlueBook = 1
    }

    /// <summary>
    /// Condition Type
    /// </summary>
    public enum ConditionType
    {
        /// <summary>
        /// Not Running
        /// </summary>
        NotRunning = 0,

        /// <summary>
        /// Running
        /// </summary>
        Running = 1
    }

    /// <summary>
    /// Status Type
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Inventory
        /// </summary>
        Inventory = 1,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 2,
    }
}
