using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class AddInvoiceNotMap : AccUpdateNotMap<AddInvoiceNotViewModel>
    {
        public AddInvoiceNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override AddInvoiceNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            AddInvoiceNotViewModel addInvoiceNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                addInvoiceNotViewModel.NewAccountInvoice = inboxNot.ReadObjectFromAttr3<AccountInvoice>();
            return addInvoiceNotViewModel;
        }
    }
}
