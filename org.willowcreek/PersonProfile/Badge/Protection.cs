using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Web.UI;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.PersonProfile;
using System.Web;

namespace org.willowcreek.PersonProfile.Badge
{
    /// <summary>
    /// Protection Badge
    /// </summary>
    [Description("Protection Badge")]
    [Export( typeof( BadgeComponent ) )]
    [ExportMetadata("ComponentName", "Protection Badge")]

    public class Protection : BadgeComponent
    {
        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        /// <param name="badge">The badge.</param>
        /// <param name="writer">The writer.</param>
        public override void Render(PersonBadgeCache badge, System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(Extensions.GetBadgeDisplay(Person));
        }
    }
}
