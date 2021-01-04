using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class ResponsibilityTF : TransactionFactory<ResponsibilityViewModel>
    {
        public ResponsibilityTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, ResponsibilityViewModel responsibilityViewModel)
        {
            if (operationType.IsInList(baseOperationTypes))
            {
                Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
                transaction.AddParameter("CurrentOrganization", currentOrganization);
                transaction.AddParameter("Responsibilities", currentOrganization.GetResponsibilities(context));
            }
        }

        protected override void CreateHandler(OperationType operationType, string recordId)
        {
            Organization currentOrganization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            transaction.AddParameter("CurrentOrganization", currentOrganization);
        }
    }
}
