using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Factories.OrgNotFactories;

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
                // Необходимо запомнить список сотрудников, находящихся в удаляемом подразделении до выполнения коммита
                // Так как после коммита у всех сотрудников подразделения будет очищен DivisionId и их невозможно будет найти
                List<Employee> divEmployees = context.Employees.AsNoTracking().Where(div => div.DivisionId == division.Id).ToList();
                transaction.AddParameter("DivEmployees", divEmployees);

                // блокировка должностей и сотрудников
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
        /// Метод отсылает уведомления всем пользователям, состоящим в подразделении о его удалении
        /// </summary>
        private void SendNotifications()
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            Division division = (Division)transaction.GetParameterValue("RecordToRemove");
            DivDeleteParams divDeleteParams = new DivDeleteParams()
            {
                Organization = currentOrganization,
                RemovedDivision = division
            };
            DivDeleteNotFactory divDeleteNotFactory = new DivDeleteNotFactory(serviceProvider, context, divDeleteParams);
            divDeleteNotFactory.Send(division.OrganizationId, (List<Employee>)transaction.GetParameterValue("DivEmployees"));
        }
    }
}
