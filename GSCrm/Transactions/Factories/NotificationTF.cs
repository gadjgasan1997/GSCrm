using GSCrm.Data;
using GSCrm.Models;
using System;

namespace GSCrm.Transactions.Factories
{
    public class NotificationTF : TransactionFactory<Notification>
    {
        public NotificationTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
