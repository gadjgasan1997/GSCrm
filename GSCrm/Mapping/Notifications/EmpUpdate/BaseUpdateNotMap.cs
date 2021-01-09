using GSCrm.Data;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using System;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class BaseUpdateNotMap : EmpUpdateNotMap<BaseUpdateNotViewModel>
    {
        public BaseUpdateNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
