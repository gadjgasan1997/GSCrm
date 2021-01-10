using GSCrm.Models;
using System;
using GSCrm.Mapping;
using GSCrm.Models.ViewModels;
using GSCrm.Data;

namespace GSCrm.Factories
{
    public class MapFactory : IMapFactory
    {
        public IMap<TDataModel, TViewModel> GetMap<TDataModel, TViewModel>(IServiceProvider serviceProvider, ApplicationDbContext context)
            where TDataModel : BaseDataModel, new()
            where TViewModel : BaseViewModel, new()
            => (typeof(TDataModel).Name, typeof(TViewModel).Name) switch
            {
                ("AccountAddress", "AccountAddressViewModel") => (IMap<TDataModel, TViewModel>)new AccountAddressMap(serviceProvider, context),
                ("AccountContact", "AccountContactViewModel") => (IMap<TDataModel, TViewModel>)new AccountContactMap(serviceProvider, context),
                ("AccountInvoice", "AccountInvoiceViewModel") => (IMap<TDataModel, TViewModel>)new AccountInvoiceMap(serviceProvider, context),
                ("AccountManager", "AccountManagerViewModel") => (IMap<TDataModel, TViewModel>)new AccountManagerMap(serviceProvider, context),
                ("Account", "AccountViewModel") => (IMap<TDataModel, TViewModel>)new AccountMap(serviceProvider, context),
                ("AccountQuote", "AccountQuoteViewModel") => (IMap<TDataModel, TViewModel>)new AccountQuoteMap(serviceProvider, context),
                ("Division", "DivisionViewModel") => (IMap<TDataModel, TViewModel>)new DivisionMap(serviceProvider, context),
                ("EmployeeContact", "EmployeeContactViewModel") => (IMap<TDataModel, TViewModel>)new EmployeeContactMap(serviceProvider, context),
                ("Employee", "EmployeeViewModel") => (IMap<TDataModel, TViewModel>)new EmployeeMap(serviceProvider, context),
                ("EmployeePosition", "EmployeePositionViewModel") => (IMap<TDataModel, TViewModel>)new EmployeePositionMap(serviceProvider, context),
                ("EmployeeResponsibility", "EmployeeResponsibilityViewModel") => (IMap<TDataModel, TViewModel>)new EmployeeResponsibilityMap(serviceProvider, context),
                ("Organization", "OrganizationViewModel") => (IMap<TDataModel, TViewModel>)new OrganizationMap(serviceProvider, context),
                ("Position", "PositionViewModel") => (IMap<TDataModel, TViewModel>)new PositionMap(serviceProvider, context),
                ("Responsibility", "ResponsibilityViewModel") => (IMap<TDataModel, TViewModel>)new ResponsibilityMap(serviceProvider, context),
                ("UserNotification", "UserNotificationViewModel") => (IMap<TDataModel, TViewModel>)new UserNotificationMap(serviceProvider, context),
                ("OrgNotificationsSetting", "OrgNotificationsSettingViewModel") => (IMap<TDataModel, TViewModel>)new OrgNotificationsSettingMap(serviceProvider, context),
                ("UserNotificationsSetting", "UserNotificationsSettingViewModel") => (IMap<TDataModel, TViewModel>)new UserNotificationsSettingMap(serviceProvider, context),
                _ => default
            };
    }
}
