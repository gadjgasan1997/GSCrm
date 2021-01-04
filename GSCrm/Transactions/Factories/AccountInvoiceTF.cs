using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class AccountInvoiceTF : TransactionFactory<AccountInvoiceViewModel>
    {
        public AccountInvoiceTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountInvoiceViewModel invoiceViewModel)
        {
            if (operationType.IsInList(baseOperationTypes))
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            if (operationType == OperationType.Delete)
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }
    }
}
