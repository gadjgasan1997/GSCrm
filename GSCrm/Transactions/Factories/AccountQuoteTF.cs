using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Transactions.Factories
{
    public class AccountQuoteTF : TransactionFactory<AccountQuoteViewModel>
    {
        public AccountQuoteTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
