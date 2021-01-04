using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Transactions.Factories
{
    public class SyncRespsTF : TransactionFactory<SyncRespsViewModel>
    {
        public SyncRespsTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, SyncRespsViewModel respsViewModel)
        {
            if (operationType == OperationType.EmployeeResponsibilitiesManagement)
            {
                Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
            }
        }
    }
}
