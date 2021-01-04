using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class AccountTF : TransactionFactory<AccountViewModel>
    {
        public AccountTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, AccountViewModel accountViewModel)
        {
            if (operationType.IsInList(OperationType.Update, OperationType.ChangeAccountPrimaryContact, OperationType.UnlockAccount))
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            if (operationType.IsInList(OperationType.Update, OperationType.Delete))
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }
    }
}
