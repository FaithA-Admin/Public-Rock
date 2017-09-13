using System.Data.Entity;
using Rock.Model;
using org.willowcreek.ProtectionApp.Model;

namespace org.willowcreek.ProtectionApp.Data
{
    /// <summary>
    /// Class ProtectionAppContext.
    /// </summary>
    public partial class ProtectionAppContext : Rock.Data.DbContext
    {

        #region Models
        /// <summary>
        /// Gets or sets the aliases.
        /// </summary>
        /// <value>The aliases.</value>
        public DbSet<PersonAlias> Aliases { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public DbSet<Group> Groups { get; set; }
        /// <summary>
        /// Gets or sets the group locations.
        /// </summary>
        /// <value>The group locations.</value>
        public DbSet<GroupLocation> GroupLocations { get; set; }
        /// <summary>
        /// Gets or sets the group members.
        /// </summary>
        /// <value>The group members.</value>
        public DbSet<GroupMember> GroupMembers { get; set; }
        /// <summary>
        /// Gets or sets the locations.
        /// </summary>
        /// <value>The locations.</value>
        public DbSet<Location> Locations { get; set; }
        /// <summary>
        /// Gets or sets the people.
        /// </summary>
        /// <value>The people.</value>
        public DbSet<Person> People { get; set; }

        /// <summary>
        /// Gets or sets the questionnaires.
        /// </summary>
        /// <value>The questionnaires.</value>
        public DbSet<Questionnaire> Questionnaires { get; set; }

        /// <summary>
        /// Gets or sets the references.
        /// </summary>
        /// <value>The references.</value>
        public DbSet<Reference> References { get; set; }
        /// <summary>
        /// Gets or sets the User Default values for protection app
        /// </summary>
        /// <value>UserDefaults entity</value>
        public DbSet<UserDefaults> UsersDefaults { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionAppContext"/> class.
        /// </summary>
        public ProtectionAppContext()
            : base("RockContext")
        {
            //intentionally left blank
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // we don't want this context to create a database or look for EF Migrations, do set the Initializer to null
            Database.SetInitializer<ProtectionAppContext>(new NullDatabaseInitializer<ProtectionAppContext>());

            Rock.Data.ContextHelper.AddConfigurations(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

    }
}
