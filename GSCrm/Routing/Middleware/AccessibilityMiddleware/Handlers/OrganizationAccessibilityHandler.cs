using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class OrganizationAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Organization":
                case "ProductCategories":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        // Поптыка получить организацию, на которую перешел пользователь и проверка прав на ее просмотр
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!organizationRepository.TryGetItemById(id.ToString(), out Organization organization))
                            accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                        else if (!organizationRepository.HasPermissionsForSeeItem(organization))
                            accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                        else
                        {
                            // Кеширование найденной организации
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;
                            cachService.AddOrUpdateEntity(currentUser, organization);
                        }
                    }
                    else accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                    break;
                default:
                    break;
            }
        }
    }
}
