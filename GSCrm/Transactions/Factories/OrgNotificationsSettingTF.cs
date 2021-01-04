using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Transactions.Factories
{
    public class OrgNotificationsSettingTF : TransactionFactory<OrgNotificationsSettingViewModel>
    {
        public OrgNotificationsSettingTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
