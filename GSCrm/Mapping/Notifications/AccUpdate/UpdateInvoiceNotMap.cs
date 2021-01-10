using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Models.ViewModels.Notifications.AccUpdate;

namespace GSCrm.Mapping.Notifications.AccUpdate
{
    public class UpdateInvoiceNotMap : AccUpdateNotMap<UpdateInvoiceNotViewModel>
    {
        public UpdateInvoiceNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override UpdateInvoiceNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, AccUpdateType parsedUpdateType)
        {
            UpdateInvoiceNotViewModel updateInvoiceNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (!string.IsNullOrEmpty(inboxNot.Attrib3))
                updateInvoiceNotViewModel.OldAccountInvoice = inboxNot.ReadObjectFromAttr3<AccountInvoice>();
            if (!string.IsNullOrEmpty(inboxNot.Attrib4))
                updateInvoiceNotViewModel.NewAccountInvoice = inboxNot.ReadObjectFromAttr4<AccountInvoice>();
            return updateInvoiceNotViewModel;
        }
    }
}
