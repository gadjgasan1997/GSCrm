using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using System.Linq;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using GSCrm.Helpers;
using GSCrm.Models.Enums;

namespace GSCrm.Transactions.Factories
{
    public class PositionTF : TransactionFactory<PositionViewModel>
    {
        public PositionTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, PositionViewModel positionViewModel)
        {
            if (operationType.IsInList(baseOperationTypes.With(OperationType.ChangePositionDivision)))
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
