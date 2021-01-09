using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Factories.OrgNotFactories;
using System.Collections.Generic;
using static GSCrm.CommonConsts;
using Microsoft.AspNetCore.Mvc;

namespace GSCrm.Transactions.Factories
{
    public class PositionTF : TransactionFactory<PositionViewModel>
    {
        public PositionTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CreateHandler(OperationType operationType, PositionViewModel positionViewModel)
        {
            if (operationType.IsInList(baseOperationTypes.With(OperationType.ChangePositionDivision, OperationType.UnlockPosition)))
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

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                switch (operationType)
                {
                    case OperationType.Delete:
                        SendNotifications();
                        break;
                }
            }
        }

        /// <summary>
        /// Метод отсылает уведомления всем пользователям, занимающим должность о ее удалении
        /// </summary>
        private void SendNotifications()
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            Position position = (Position)transaction.GetParameterValue("RecordToRemove");
            List<EmployeePosition> employeePositions = (List<EmployeePosition>)transaction.GetParameterValue("PosEmployees");

            // Для каждой должности необходимо сделать новое уведомление, так как признак "IsPrimary" везде разный
            employeePositions.ForEach(employeePosition =>
            {
                PosDeleteParams posDeleteParams = new PosDeleteParams()
                {
                    Organization = currentOrganization,
                    OrganizationUrl = urlHelper.Action(ORGANIZATION, ORGANIZATION, new { id = currentOrganization.Id }, httpContext.Request.Scheme),
                    RemovedPosition = position,
                    IsPrimary = employeePosition.Employee.PrimaryPositionId == position.Id
                };
                PosDeleteNotFactory posDeleteNotFactory = new PosDeleteNotFactory(serviceProvider, context, posDeleteParams);
                posDeleteNotFactory.Send(currentOrganization.Id, new List<Employee>() { employeePosition.Employee });
            });
        }
    }
}
