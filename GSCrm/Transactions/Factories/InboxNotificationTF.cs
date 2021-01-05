﻿using GSCrm.Data;
using GSCrm.Models;
using System;

namespace GSCrm.Transactions.Factories
{
    public class InboxNotificationTF : TransactionFactory<InboxNotification>
    {
        public InboxNotificationTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
