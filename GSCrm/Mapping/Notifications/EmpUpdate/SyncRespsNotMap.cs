using GSCrm.Data;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using System;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class SyncRespsNotMap : EmpUpdateNotMap<SyncRespsNotViewModel>
    {
        public SyncRespsNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
