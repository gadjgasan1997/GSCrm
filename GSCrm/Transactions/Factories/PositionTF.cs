using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Factories.OrgNotFactories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GSCrm.Repository;
using static GSCrm.CommonConsts;
using GSCrm.Notifications;

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
                        Position position = (Position)transaction.GetParameterValue("RecordToRemove");
                        SendPosDeletedNotifications(position);
                        RemovePosNotifications(position);
                        break;
                    case OperationType.Update:
                        SendPosUpdatedNotifications();
                        break;
                    case OperationType.ChangePositionDivision:
                        SendPosChangedDivNotifications();
                        break;
                }
            }
        }

        /// <summary>
        /// Метод отсылает уведомления всем пользователям, занимающим должность о ее удалении
        /// </summary>
        /// <param name="position">Удаленная должность</param>
        private void SendPosDeletedNotifications(Position position)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            List<EmployeePosition> employeePositions = (List<EmployeePosition>)transaction.GetParameterValue("PosEmployees");

            // Для каждого сотрудника необходимо сделать новое уведомление, так как признак "IsPrimary" везде разный
            employeePositions.ForEach(employeePosition =>
            {
                PosDeleteParams posDeleteParams = new PosDeleteParams()
                {
                    Organization = currentOrganization,
                    RemovedPosition = position,
                    IsPrimary = employeePosition.Employee.PrimaryPositionId == position.Id
                };
                PosDeleteNotFactory posDeleteNotFactory = new PosDeleteNotFactory(serviceProvider, context, posDeleteParams);
                posDeleteNotFactory.Send(currentOrganization.Id, new List<Employee>() { employeePosition.Employee });
            });
        }

        /// <summary>
        /// Метод удаляет все уведомления, связанные с удаленной должностью
        /// </summary>
        /// <param name="position">Удаленная должность</param>
        private void RemovePosNotifications(Position position)
        {
            Func<InboxNotification, bool> predicate = not => not.NotificationType == NotificationType.PosUpdate && not.Attrib1 == position.Id.ToString();
            InboxNotificationRepository inboxNotRepository = new InboxNotificationRepository(serviceProvider, context);
            context.InboxNotifications.AsNoTracking().Where(predicate).ToList().ForEach(inboxNot => inboxNotRepository.TryDelete(inboxNot));
        }

        /// <summary>
        /// Метод отсылает уведомления всем пользователям, занимающим должность о ее изменении
        /// </summary>
        private void SendPosUpdatedNotifications()
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            Position position = (Position)transaction.GetParameterValue("ChangedRecord");
            PosUpdateParams posUpdateParams = new PosUpdateParams()
            {
                ChangedPosition = position,
                DivisionChanged = false,
                Organization = currentOrganization
            };
            PosUpdateNotFactory posUpdateNotFactory = new PosUpdateNotFactory(serviceProvider, context, posUpdateParams);
            List<Employee> employees = context.EmployeePositions.AsNoTracking()
                .Include(empPos => empPos.Employee)
                .Where(pos => pos.PositionId == position.Id)
                .Select(emp => emp.Employee).ToList();
            posUpdateNotFactory.Send(currentOrganization.Id, employees);
        }

        private void SendPosChangedDivNotifications()
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            List<EmployeePosition> employeePositions = (List<EmployeePosition>)transaction.GetParameterValue("PosEmployees");
            Position position = (Position)transaction.GetParameterValue("ChangedPosition");

            // Для каждого сотрудника необходимо сделать новое уведомление, так как признак "IsPrimary" везде разный
            employeePositions.ForEach(employeePosition =>
            {
                PosUpdateParams posUpdateParams = new PosUpdateParams()
                {
                    ChangedPosition = position,
                    DivisionChanged = true,
                    Organization = currentOrganization,
                    IsPrimary = employeePosition.Employee.PrimaryPositionId == position.Id
                };
                PosUpdateNotFactory posUpdateNotFactory = new PosUpdateNotFactory(serviceProvider, context, posUpdateParams);
                posUpdateNotFactory.Send(currentOrganization.Id, new List<Employee>() { employeePosition.Employee });
            });
        }
    }
}
