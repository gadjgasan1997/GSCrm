using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Models.ViewModels.Notifications
{
    public abstract class EmpUpdateNotViewModel : UserNotificationViewModel
    {
        public Organization Organization { get; set; }
        public Employee ChangedEmployee { get; set; }
        public EmpUpdateType EmpUpdateType { get; set; }
    }
}
