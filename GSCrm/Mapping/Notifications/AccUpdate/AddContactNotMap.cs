using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;
using GSCrm.Notifications.Auxiliary;
using System;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class AddContactNotMap : AccUpdateNotMap<AddContactNotViewModel>
    {
        public AddContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override AddContactNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            AddContactNotViewModel addContactNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                addContactNotViewModel.NewAccountContact = inboxNot.ReadObjectFromAttr3<AccountContact>();
            return addContactNotViewModel;
        }
    }
}
