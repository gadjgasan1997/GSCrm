using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels.Notifications.EmpUpdate;
using GSCrm.Notifications.Auxiliary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping.Notifications.EmpUpdate
{
    public class AddContactNotMap : EmpUpdateNotMap<AddContactNotViewModel>
    {
        public AddContactNotMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override AddContactNotViewModel DataToViewModel(UserNotification userNot, InboxNotification inboxNot, EmpUpdateType parsedUpdateType)
        {
            AddContactNotViewModel addContactNotViewModel = base.DataToViewModel(userNot, inboxNot, parsedUpdateType);
            if (Guid.TryParse(inboxNot.Attrib3, out Guid newEmployeeContactId))
                addContactNotViewModel.NewEmployeeContact = context.EmployeeContacts.AsNoTracking().FirstOrDefault(i => i.Id == newEmployeeContactId);
            return addContactNotViewModel;
        }
    }
}
