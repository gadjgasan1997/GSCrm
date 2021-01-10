using GSCrm.Models;

namespace GSCrm.Notifications.Params
{
    public class AccDeleteParams : INotificationParams
    {
        public Organization OwnerOrg { get; set; }
        public Account RemovedAccount { get; set; }
    }
}
