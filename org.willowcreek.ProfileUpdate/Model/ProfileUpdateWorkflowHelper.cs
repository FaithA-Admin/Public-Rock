using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.willowcreek.ProfileUpdate.Model
{
    public class ProfileUpdateWorkflowHelper
    {
        public static Guid PROFILE_UPDATE_WORKFLOW_TYPE
        {
            get
            {
                return new Guid("A84EA226-1CB2-453B-87B6-81F5360BAD3D");
            }
        }
        public static Guid PROFILE_UPDATE_WORKFLOW_RECEIVED_UPDATE_ACTIVITY_TYPE
        {
            get
            {
                return new Guid("990AA978-9393-4585-A797-C995EF666571");
            }
        }
    }
}
