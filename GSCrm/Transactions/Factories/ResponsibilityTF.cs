using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels;

namespace GSCrm.Transactions.Factories
{
    public class ResponsibilityTF : TransactionFactory<ResponsibilityViewModel>
    {
        public ResponsibilityTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, ResponsibilityViewModel responsibilityViewModel)
        {
            if (operationType.IsInList(baseOperationTypes))
            {
                if (cachService.TryGetEntityCache(currentUser, out Organization currentOrganization))
                {
                    transaction.AddParameter("CurrentOrganization", currentOrganization);
                    transaction.AddParameter("Responsibilities", currentOrganization.GetResponsibilities(context));
                }
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            if (cachService.TryGetEntityCache(currentUser, out Organization currentOrganization))
                transaction.AddParameter("CurrentOrganization", currentOrganization);
        }
    }
}
