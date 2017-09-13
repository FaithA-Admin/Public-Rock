using org.willowcreek.ProtectionApp.Data;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// Class QuestionnaireService.
    /// </summary>
    public class QuestionnaireService : ProtectionAppService<Questionnaire>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public QuestionnaireService( ProtectionAppContext context ) : base( context ) { }
    }
}
