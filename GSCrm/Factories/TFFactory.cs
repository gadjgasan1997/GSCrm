using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Transactions;
using GSCrm.Transactions.Factories;
using System;

namespace GSCrm.Factories
{
    public class TFFactory : ITFFactory
    {
        public ITransactionFactory<TEntity> GetTransactionFactory<TEntity>(IServiceProvider serviceProvider, ApplicationDbContext context)
            where TEntity : IMainEntity
            => typeof(TEntity).Name switch
            {
                "OrganizationViewModel" => (ITransactionFactory<TEntity>)new OrganizationTF(serviceProvider, context),
                "DivisionViewModel" => (ITransactionFactory<TEntity>)new DivisionTF(serviceProvider, context),
                "PositionViewModel" => (ITransactionFactory<TEntity>)new PositionTF(serviceProvider, context),
                "EmployeeViewModel" => (ITransactionFactory<TEntity>)new EmployeeTF(serviceProvider, context),
                "EmployeePositionViewModel" => (ITransactionFactory<TEntity>)new EmployeePositionTF(serviceProvider, context),
                "SyncPositionsViewModel" => (ITransactionFactory<TEntity>)new SyncPositionsTF(serviceProvider, context),
                "EmployeeContactViewModel" => (ITransactionFactory<TEntity>)new EmployeeContactTF(serviceProvider, context),
                "EmployeeResponsibilityViewModel" => (ITransactionFactory<TEntity>)new EmployeeResponsibilityTF(serviceProvider, context),
                "SyncRespsViewModel" => (ITransactionFactory<TEntity>)new SyncRespsTF(serviceProvider, context),
                "ResponsibilityViewModel" => (ITransactionFactory<TEntity>)new ResponsibilityTF(serviceProvider, context),
                "AccountViewModel" => (ITransactionFactory<TEntity>)new AccountTF(serviceProvider, context),
                "SyncAccountViewModel" => (ITransactionFactory<TEntity>)new SyncAccountsTF(serviceProvider, context),
                "AccountAddressViewModel" => (ITransactionFactory<TEntity>)new AccountAddressTF(serviceProvider, context),
                "AccountContactViewModel" => (ITransactionFactory<TEntity>)new AccountContactTF(serviceProvider, context),
                "AccountInvoiceViewModel" => (ITransactionFactory<TEntity>)new AccountInvoiceTF(serviceProvider, context),
                "AccountManagerViewModel" => (ITransactionFactory<TEntity>)new AccountManagerTF(serviceProvider, context),
                "AccountQuoteViewModel" => (ITransactionFactory<TEntity>)new AccountQuoteTF(serviceProvider, context),
                "Notification" => (ITransactionFactory<TEntity>)new NotificationTF(serviceProvider, context),
                "UserNotification" => (ITransactionFactory<TEntity>)new UserNotificationTF(serviceProvider, context),
                "InboxNotification" => (ITransactionFactory<TEntity>)new InboxNotificationTF(serviceProvider, context),
                "OrgNotificationsSettingViewModel" => (ITransactionFactory<TEntity>)new OrgNotificationsSettingTF(serviceProvider, context),
                "UserNotificationsSettingViewModel" => (ITransactionFactory<TEntity>)new UserNotificationsSettingTF(serviceProvider, context),
                "UserViewModel" => (ITransactionFactory<TEntity>)new UserTF(serviceProvider, context),
                _ => default
            };
    }
}
