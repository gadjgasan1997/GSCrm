using GSCrm.Models;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Notifications.Params
{
    public abstract class AccUpdateParams : INotificationParams
    {
        public Organization OwnerOrg { get; set; }
        public Account ChangedAccount { get; set; }
        public AccUpdateType AccUpdateType { get; protected set; }
    }
}
