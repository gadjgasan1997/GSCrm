using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class ResponsibilityAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Responsibility":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        // Проверки на наличие полномочия и доступа к нему у пользователя
                        ResponsibilityRepository responsibilityRepository = new ResponsibilityRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!responsibilityRepository.TryGetItemById(id.ToString(), out Responsibility responsibility))
                            accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                        else if (!organizationRepository.HasPermissionsForSeeItem(responsibility.GetOrganization(accessibilityHandlerData.Context)))
                            accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                        else if (!responsibilityRepository.HasPermissionsForSeeItem(responsibility))
                            accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                        else
                        {
                            // Кеширование найденного полномочия
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;
                            cachService.CacheItem(currentUser.Id, "CurrentResponsibilityData", responsibility);
                        }
                    }
                    else accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                    break;
                default:
                    break;
            }
        }
    }
}
