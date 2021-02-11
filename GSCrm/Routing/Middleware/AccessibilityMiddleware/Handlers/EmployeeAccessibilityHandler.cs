using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
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
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        // Попытка получить сотрудника и проверка у пользователя прав на его просмотр
                        EmployeeRepository employeeRepository = new EmployeeRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!employeeRepository.TryGetItemById(id.ToString(), out Employee employee))
                            accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                        else if (!organizationRepository.HasPermissionsForSeeItem(employee.GetOrganization(accessibilityHandlerData.Context)))
                            accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                        else if (!employeeRepository.HasPermissionsForSeeItem(employee))
                            accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                        else
                        {
                            // Кеширование найденного сотрудника
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;
                            cachService.AddOrUpdateEntity(currentUser, employee);
                        }
                    }
                    else accessibilityHandlerData.Redirect($"/{EMPLOYEE}/HasNoPermissionsForSee");
                    break;
                default:
                    break;
            }
        }
    }
}
