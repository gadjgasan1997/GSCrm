using System;
using GSCrm.Data;
using GSCrm.Models;

namespace GSCrm.Transactions.Factories
{
    public class UserNotificationTF : TransactionFactory<UserNotification>
    {
        public UserNotificationTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
