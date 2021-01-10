using GSCrm.Models;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Notifications.Params
{
    public class AccTeamManagementParams : INotificationParams
    {
        public Organization OwnerOrg { get; set; }
        public Account Account { get; set; }
        public AccTeamManagementNotType AccTeamManagementNotType { get; set; }
    }
}
