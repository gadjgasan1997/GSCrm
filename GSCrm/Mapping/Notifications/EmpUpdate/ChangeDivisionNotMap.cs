using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using GSCrm.Notifications.Auxiliary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class ChangeDivisionNotMap : EmpUpdateNotMap<ChangeDivisionNotViewModel>
    {
        public ChangeDivisionNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override ChangeDivisionNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, EmpUpdateType parsedUpdateType)
        {
            ChangeDivisionNotViewModel changeDivisionNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (Guid.TryParse(inboxNot.Attrib3, out Guid newEmployeePositionId))
                changeDivisionNotViewModel.NewEmployeePosition = context.Positions.AsNoTracking().FirstOrDefault(i => i.Id == newEmployeePositionId);
            return changeDivisionNotViewModel;
        }
    }
}
