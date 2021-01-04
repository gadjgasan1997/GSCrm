using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Transactions.Factories
{
    public class SyncPositionsTF : TransactionFactory<SyncPositionsViewModel>
    {
        public SyncPositionsTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, SyncPositionsViewModel positionsViewModel)
        {
            if (operationType == OperationType.EmployeePositionsManagement)
            {
                Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
            }
        }
    }
}
