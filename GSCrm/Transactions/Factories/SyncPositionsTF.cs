using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params.EmpUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate;

namespace GSCrm.Transactions.Factories
{
    public class SyncPositionsTF : TransactionFactory<SyncPositionsViewModel>
    {
        public SyncPositionsTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

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
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
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
