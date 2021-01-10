using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using GSCrm.Notifications.Auxiliary;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class AddContactNotMap : EmpUpdateNotMap<AddContactNotViewModel>
    {
        public AddContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override AddContactNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, EmpUpdateType parsedUpdateType)
        {
            AddContactNotViewModel addContactNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                addContactNotViewModel.NewEmployeeContact = inboxNot.ReadObjectFromAttr3<EmployeeContact>();
            return addContactNotViewModel;
        }
    }
}
