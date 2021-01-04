using GSCrm.Data;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Transactions.Factories
{
    public class UserNotificationsSettingTF : TransactionFactory<UserNotificationsSettingViewModel>
    {
        public UserNotificationsSettingTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
