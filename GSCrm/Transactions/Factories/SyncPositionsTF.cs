using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params.EmpUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate;

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

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success && operationType == OperationType.EmployeePositionsManagement)
                SendNotifications();
        }

        /// <summary>
        /// Метод рассылает уведомления при изменениях в списке должностей сотрудника
        /// </summary>
        private void SendNotifications()
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            Employee employee = (Employee)transaction.GetParameterValue("Employee");
            SyncPossParams syncPossParams = new SyncPossParams()
            {
                Organization = currentOrganization,
                ChangedEmployee = employee
            };
            SyncPossNotFactory syncPossNotFactory = new SyncPossNotFactory(serviceProvider, context, syncPossParams);
            syncPossNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }
    }
}
