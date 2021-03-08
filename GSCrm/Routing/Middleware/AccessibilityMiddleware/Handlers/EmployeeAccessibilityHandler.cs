using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class EmployeeAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Employee":
                case "Subordinates":
                case "Contacts":
                    {
                        // Получение id сотрудника из ссылки запроса
                        string employeeId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        if (!string.IsNullOrEmpty(employeeId))
                        {
                            // Попытка получить сотрудника и проверка у пользователя прав на его просмотр
                            EmployeeRepository employeeRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            OrganizationRepository organizationRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            if (!employeeRepository.TryGetItemById(employeeId, out Employee employee))
                            {
                                accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                                return;
                            }

                            Organization organization = employee.GetOrganization(accessibilityHandlerData.Context);
                            if (!organizationRepository.HasPermissionsForSeeItem(organization) ||
                                !employeeRepository.HasPermissionsForSeeItem(employee))
                            {
                                accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                                return;
                            }

                            // Кеширование найденного сотрудника и организации(на случай прямого перехода по ссылке на сотрудника)
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                            cachService.CacheEntity(currentUser, organization);
                            cachService.CacheCurrentEntity(currentUser, organization);
                            cachService.CacheEntity(currentUser, employee);
                            cachService.CacheCurrentEntity(currentUser, employee);

                            // Маппинг в данные отображения
                            EmployeeViewModel employeeViewModel = new EmployeeMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context).DataToViewModel(employee);

                            // Обновление полей в модели из кеша, если он существует
                            if (cachService.TryGetCachedEntity(currentUser, employeeViewModel.Id, out EmployeeViewModel cachedViewModel))
                                employeeViewModel.Refresh(cachedViewModel);

                            // Кеширование модели
                            cachService.CacheEntity(currentUser, employeeViewModel);
                            cachService.CacheCurrentEntity(currentUser, employeeViewModel);
                        }
                        else accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                    }
                    break;

                case "SearchContact":
                case "SearchSubordinate":
                    accessibilityHandlerData.TryCacheCurrentEmployee(RequestSourceType.Form, "id", RequestBreakType.Redirect);
                    break;

                case "ClearContactSearch":
                case "ClearSubordinateSearch":
                    accessibilityHandlerData.TryCacheCurrentEmployee(RequestSourceType.RouteValues, "id", RequestBreakType.Redirect);
                    break;

                case "Create":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    break;

                case "Update":
                case "Unlock":
                case "ChangeDivision":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.TryCacheCurrentEmployee();
                    break;

                default:
                    break;
            }
        }
    }
}
