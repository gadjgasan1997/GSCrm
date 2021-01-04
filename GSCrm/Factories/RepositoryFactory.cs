using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using System;

namespace GSCrm.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepository<TDataModel, TViewModel> GetRepository<TDataModel, TViewModel>(IServiceProvider serviceProvider, ApplicationDbContext context)
            where TDataModel : BaseDataModel, new()
            where TViewModel : BaseViewModel, new()
            => typeof(TDataModel).Name switch
            {
                "Organization" => (IRepository<TDataModel, TViewModel>)new OrganizationRepository(serviceProvider, context),
                "Division" => (IRepository<TDataModel, TViewModel>)new DivisionRepository(serviceProvider, context),
                "Position" => (IRepository<TDataModel, TViewModel>)new PositionRepository(serviceProvider, context),
                "Employee" => (IRepository<TDataModel, TViewModel>)new EmployeeRepository(serviceProvider, context),
                "EmployeePosition" => (IRepository<TDataModel, TViewModel>)new EmployeePositionRepository(serviceProvider, context),
                "EmployeeContact" => (IRepository<TDataModel, TViewModel>)new EmployeeContactRepository(serviceProvider, context),
                "EmployeeResponsibility" => (IRepository<TDataModel, TViewModel>)new EmployeeResponsibilityRepository(serviceProvider, context),
                "Responsibility" => (IRepository<TDataModel, TViewModel>)new ResponsibilityRepository(serviceProvider, context),
                "Account" => (IRepository<TDataModel, TViewModel>)new AccountRepository(serviceProvider, context),
                "AccountAddress" => (IRepository<TDataModel, TViewModel>)new AccountAddressRepository(serviceProvider, context),
                "AccountContact" => (IRepository<TDataModel, TViewModel>)new AccountContactRepository(serviceProvider, context),
                "AccountInvoice" => (IRepository<TDataModel, TViewModel>)new AccountInvoiceRepository(serviceProvider, context),
                "AccountManager" => (IRepository<TDataModel, TViewModel>)new AccountManagerRepository(serviceProvider, context),
                "AccountQuote" => (IRepository<TDataModel, TViewModel>)new AccountQuoteRepository(serviceProvider, context),
                "InboxNotification" => (IRepository<TDataModel, TViewModel>)new InboxNotificationRepository(serviceProvider, context),
                "OrgNotificationsSetting" => (IRepository<TDataModel, TViewModel>)new OrgNotificationsSettingRepository(serviceProvider, context),
                "UserNotificationsSetting" => (IRepository<TDataModel, TViewModel>)new UserNotificationsSettingRepository(serviceProvider, context),
                _ => default
            };
    }
}
