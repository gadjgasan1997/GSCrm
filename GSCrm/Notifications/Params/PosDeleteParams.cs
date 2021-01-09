using GSCrm.Models;

namespace GSCrm.Notifications.Params
{
    public class PosDeleteParams : INotificationParams
    {
        public Organization Organization { get; set; }
        public string OrganizationUrl { get; set; }
        public Position RemovedPosition { get; set; }
        public bool IsPrimary { get; set; }
    }
}
