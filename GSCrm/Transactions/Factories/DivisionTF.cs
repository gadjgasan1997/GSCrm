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

        /// <summary>
        /// Перед попыткой коммита на удаление необходимо заблокировать должности и сотрудников
        /// </summary>
        protected override void BeforeCommit(OperationType operationType)
        {
            if (operationType == OperationType.Delete)
            {
                Division division = (Division)transaction.GetParameterValue("RecordToRemove");
                LockPositions(division);
                LockEmployees(division);
            }
        }

        /// <summary>
        /// Метод блокирует должности при удалении подразделения
        /// </summary>
        /// <param name="division"></param>
        private void LockPositions(Division division)
        {
            division.GetPositions(context).ForEach(position =>
            {
                position.Lock(PositionLockReason.DivisionAbsent);
                position.DivisionId = null;
                transaction.AddChange(position, EntityState.Modified);
            });
        }

        /// <summary>
        /// Метод блокирует сотрудников при удалении подразделения
        /// </summary>
        /// <param name="division"></param>
        private void LockEmployees(Division division)
        {
            division.GetEmployees(context).ForEach(employee =>
            {
                employee.Lock(EmployeeLockReason.DivisionAbsent);
                employee.DivisionId = null;
                transaction.AddChange(employee, EntityState.Modified);
            });
        }

        protected override void CloseHandler(TransactionStatus transactionStatus)
        {
            if (transactionStatus == TransactionStatus.Success)
            {
                switch (transaction.OperationType)
                {
                    case OperationType.Delete:
                        SendNotifications();
                        break;
                }
            }
        }

        /// <summary>
        /// Метод отсылает уведомления всем пользователям, состоящим в подразделении о его удалении
        /// </summary>
        private void SendNotifications()
        {
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
        }
    }
}
