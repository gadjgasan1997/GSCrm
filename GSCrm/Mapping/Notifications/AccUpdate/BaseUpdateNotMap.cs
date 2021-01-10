using GSCrm.Data;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;
using System;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class BaseUpdateNotMap : AccUpdateNotMap<BaseUpdateNotViewModel>
    {
        public BaseUpdateNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
