using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications.Params.EmpUpdate;
using GSCrm.Notifications.Factories.OrgNotFactories.EmpUpdate;

namespace GSCrm.Transactions.Factories
{
    public class SyncRespsTF : TransactionFactory<SyncRespsViewModel>
    {
        public SyncRespsTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (transactionStatus == TransactionStatus.Success && operationType == OperationType.EmployeeResponsibilitiesManagement)
                SendNotifications();
        }

        /// <summary>
        /// Метод рассылает уведомления при изменениях в списке должностей сотрудника
        /// </summary>
        private void SendNotifications()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            Employee employee = cachService.GetCachedCurrentEntity<Employee>(currentUser);
            SyncRespsParams syncRespsParams = new SyncRespsParams()
            {
                Organization = currentOrganization,
                ChangedEmployee = employee
            };
            SyncRespsNotFactory syncRespsNotFactory = new SyncRespsNotFactory(serviceProvider, context, syncRespsParams);
            syncRespsNotFactory.Send(currentOrganization.Id, new List<Employee>() { employee });
        }
    }
}
