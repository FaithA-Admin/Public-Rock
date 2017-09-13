using Rock.Rest;
using org.willowcreek.Model;

namespace org.willowcreek.Rest
{
    /// <summary>
    /// WillowEventItemMinistry REST API
    /// </summary>
    public partial class WillowEventItemMinistryController : ApiController<WillowEventItemMinistry>
    {
        public WillowEventItemMinistryController() : base(new WillowEventItemMinistryService(new WillowContext())) { }
    }
}
