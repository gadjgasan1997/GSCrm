using GSCrm.Data;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using System;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class SyncPossNotMap : EmpUpdateNotMap<SyncPossNotViewModel>
    {
        public SyncPossNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
