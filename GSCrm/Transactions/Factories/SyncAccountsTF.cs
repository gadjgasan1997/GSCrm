using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Transactions.Factories
{
    public class SyncAccountsTF : TransactionFactory<SyncAccountViewModel>
    {
        public SyncAccountsTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, SyncAccountViewModel entity)
        {
            if (operationType == OperationType.AccountTeamManagement)
            {
                Account currentAccount = cachService.GetMainEntity(currentUser, MainEntityType.AccountData) as Account;
                transaction.AddParameter("CurrentAccount", currentAccount);
            }
        }
    }
}
