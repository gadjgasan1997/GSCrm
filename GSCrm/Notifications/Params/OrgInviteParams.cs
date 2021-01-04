using GSCrm.Models;

namespace GSCrm.Notifications.Params
{
    public class OrgInviteParams : INotificationParams
    {
        public Organization Organization { get; set; }
        public string OrgInviteUrl { get; set; }
    }
}
