using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;

namespace GSCrm.Transactions.Factories
{
    public class QuoteTF : TransactionFactory<QuoteViewModel>
    {
        public QuoteTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }
    }
}
