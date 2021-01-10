using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Models.ViewModels.Notifications
{
    public class AccTeamManagementNotViewModel : UserNotificationViewModel
    {
        public Organization OwnerOrg { get; set; }
        public Account Account { get; set; }
        public AccTeamManagementNotType AccTeamManagementNotType { get; set; }
    }
}
