using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class ChangeDivisionNotMap : EmpUpdateNotMap<ChangeDivisionNotViewModel>
    {
        public ChangeDivisionNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override ChangeDivisionNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, EmpUpdateType parsedUpdateType)
        {
            ChangeDivisionNotViewModel changeDivisionNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                changeDivisionNotViewModel.NewEmployeeDivision = inboxNot.ReadObjectFromAttr3<Division>();
            if (!string.IsNullOrEmpty(inboxNot.Attrib4))
                changeDivisionNotViewModel.NewEmployeePosition = inboxNot.ReadObjectFromAttr4<Position>();
            return changeDivisionNotViewModel;
        }
    }
}
