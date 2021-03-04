using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Mapping;
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
                            EmployeeRepository employeeRepository = new EmployeeRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            if (!employeeRepository.TryGetItemById(employeeId, out Employee employee))
                            {
                                accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                                return;
                            }

                            Organization organization = employee.GetOrganization(accessibilityHandlerData.Context);
                            if (!organizationRepository.HasPermissionsForSeeItem(organization))
                            {
                                accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                                return;
                            }
                            if (!employeeRepository.HasPermissionsForSeeItem(employee))
                            {
                                accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
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
                    accessibilityHandlerData.CacheCurrentEmployee(RequestSourceType.Form);
                    break;

                case "ClearContactSearch":
                case "ClearSubordinateSearch":
                    accessibilityHandlerData.CacheCurrentEmployee(RequestSourceType.RouteValues);
                    break;

                case "Create":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    break;

                case "Update":
                case "Unlock":
                case "ChangeDivision":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.CacheCurrentEmployee();
                    break;

                default:
                    break;
            }
        }

        private void CheckPermissionsAndCache(AccessibilityHandlerData accessibilityHandlerData, string employeeId)
        {
            
        }
    }
}
