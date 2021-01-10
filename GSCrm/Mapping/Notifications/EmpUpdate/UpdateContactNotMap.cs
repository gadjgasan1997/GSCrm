using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class UpdateContactNotMap : EmpUpdateNotMap<UpdateContactNotViewModel>
    {
        public UpdateContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override UpdateContactNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, EmpUpdateType parsedUpdateType)
        {
            UpdateContactNotViewModel addContactNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                addContactNotViewModel.OldEmployeeContact = inboxNot.ReadObjectFromAttr3<EmployeeContact>();
            if (!string.IsNullOrEmpty(inboxNot.Attrib4))
                addContactNotViewModel.NewEmployeeContact = inboxNot.ReadObjectFromAttr4<EmployeeContact>();
            return addContactNotViewModel;
        }
    }
}
