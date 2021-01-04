using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Transactions.Factories
{
    public class AccountManagerTF : TransactionFactory<AccountManagerViewModel>
    {
        public AccountManagerTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
