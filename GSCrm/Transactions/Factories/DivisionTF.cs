using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Notifications;
using System.Collections.Generic;
using GSCrm.Notifications.Factories.OrgNotFactories;
using GSCrm.Notifications.Params;
using static GSCrm.CommonConsts;
using Microsoft.AspNetCore.Mvc;

namespace GSCrm.Transactions.Factories
{
    public class DivisionTF : TransactionFactory<DivisionViewModel>
    {
        public DivisionTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, DivisionViewModel divisionViewModel)
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

        protected override void CloseHandler(TransactionStatus transactionStatus)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                switch (transaction.OperationType)
                {
                    case OperationType.Delete:
                        Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
                        Division division = (Division)transaction.GetParameterValue("RecordToRemove");
                        List<Employee> divEmployees = context.Employees.AsNoTracking().Where(div => div.DivisionId == division.Id).ToList();
                        DivDeleteParams divDeleteParams = new DivDeleteParams()
                        {
                            Organization = currentOrganization,
                            OrganizationUrl = urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = currentOrganization.Id }, httpContext.Request.Scheme),
                            RemovedDivision = division
                        };
                        DivDeleteNotFactory divDeleteNotFactory = new DivDeleteNotFactory(serviceProvider, context, divDeleteParams);
                        divDeleteNotFactory.Send(division.OrganizationId, divEmployees);
                        break;
                }
            }
        }
    }
}
