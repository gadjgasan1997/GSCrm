using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Models.ViewModels.Notifications
{
    public abstract class AccUpdateNotViewModel : UserNotificationViewModel
    {
        public Organization OwnerOrg { get; set; }
        public Account ChangedAccount { get; set; }
        public AccUpdateType AccUpdateType { get; set; }
    }
}
