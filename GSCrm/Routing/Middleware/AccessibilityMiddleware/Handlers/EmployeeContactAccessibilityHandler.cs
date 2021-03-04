using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Localization;
using Microsoft.Extensions.DependencyInjection;
using GSCrm.Models.ViewModels;
using GSCrm.Mapping;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class EmployeeContactAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Contact":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        EmployeeContactRepository contactRepository = new EmployeeContactRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);

                        // Попытка получить контакт
                        if (!contactRepository.TryGetItemById(id.ToString(), out EmployeeContact employeeContact))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("EmployeeContactNotFound", resManager));
                            return;
                        }

                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();

                        // Попытка получить сотрудника
                        if (!cachService.TryGetCachedEntity(currentUser, employeeContact.EmployeeId, out Employee employee) ||
                            !cachService.TryGetCachedEntity(currentUser, employeeContact.EmployeeId, out EmployeeViewModel employeeViewModel))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("EmployeeNotFound", resManager));
                            return;
                        }

                        // Попытка получить организацию
                        if (!cachService.TryGetCachedEntity(currentUser, employee.OrganizationId, out Organization organization) ||
                            !cachService.TryGetCachedEntity(currentUser, employee.OrganizationId, out OrganizationViewModel organizationViewModel))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("OrganizationNotFound", resManager));
                            return;
                        }

                        // Маппинг контакта в модель отображения
                        EmployeeContactMap employeeContactMap = new EmployeeContactMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        EmployeeContactViewModel contactViewModel = employeeContactMap.DataToViewModel(employeeContact);

                        // Кешируется текущий контакт, сотрудник и текущая организация(предполагается, что она уже есть в кеше)
                        cachService.CacheCurrentEntity(currentUser, organization);
                        cachService.CacheCurrentEntity(currentUser, organizationViewModel);
                        cachService.CacheCurrentEntity(currentUser, employee);
                        cachService.CacheCurrentEntity(currentUser, employeeViewModel);
                        cachService.CacheEntity(currentUser, employeeContact);
                        cachService.CacheCurrentEntity(currentUser, employeeContact);
                        cachService.CacheEntity(currentUser, contactViewModel);
                        cachService.CacheCurrentEntity(currentUser, contactViewModel);
                    }
                    break;
                case "Create":
                case "Update":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.CacheCurrentEmployee();
                    break;
                default:
                    break;
            }
        }
    }
}
