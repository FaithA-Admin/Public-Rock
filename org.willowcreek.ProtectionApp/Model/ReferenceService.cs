using org.willowcreek.ProtectionApp.Data;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// Class ReferenceService.
    /// </summary>
    public class ReferenceService : ProtectionAppService<Reference>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ReferenceService(ProtectionAppContext context) : base(context)
        {
        }
    }
}