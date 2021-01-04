using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class EmployeeContactTF : TransactionFactory<EmployeeContactViewModel>
    {
        public EmployeeContactTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, EmployeeContactViewModel contactViewModel)
        {
            if (operationType.IsInList(baseOperationTypes))
            {
                Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            transaction.AddParameter("CurrentOrganization", currentOrganization);
        }
    }
}
