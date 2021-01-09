using GSCrm.Data;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using System;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class UpdateContactNotMap : EmpUpdateNotMap<UpdateContactNotViewModel>
    {
        public UpdateContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }
    }
}
