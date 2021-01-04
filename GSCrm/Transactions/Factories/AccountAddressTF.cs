using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class AccountAddressTF : TransactionFactory<AccountAddressViewModel>
    {
        public AccountAddressTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountAddressViewModel addressViewModel)
        {
            if (operationType.IsInList(baseOperationTypes.With(OperationType.ChangeAccountLegalAddress)))
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }
    }
}
