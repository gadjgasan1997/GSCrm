using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class AddAddressNotMap : AccUpdateNotMap<AddAddressNotViewModel>
    {
        public AddAddressNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override AddAddressNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            AddAddressNotViewModel addAddressNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                addAddressNotViewModel.NewAccountAddress = inboxNot.ReadObjectFromAttr3<AccountAddress>();
            return addAddressNotViewModel;
        }
    }
}
