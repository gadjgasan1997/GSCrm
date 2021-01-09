using GSCrm.Models;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Notifications.Params
{
    public abstract class EmpUpdateParams : INotificationParams
    {
        public Organization Organization { get; set; }
        public Employee ChangedEmployee { get; set; }
        public EmpUpdateType EmpUpdateType { get; protected set; }
    }
}
