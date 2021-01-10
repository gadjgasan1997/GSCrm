using GSCrm.Data;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using System;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class DeleteContactNotMap : EmpUpdateNotMap<DeleteContactNotViewModel>
    {
        public DeleteContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
