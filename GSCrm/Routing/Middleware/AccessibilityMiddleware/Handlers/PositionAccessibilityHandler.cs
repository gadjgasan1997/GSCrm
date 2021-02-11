using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class PositionAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Position":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        // Проверки на наличие должности и доступа к ней у пользователя
                        PositionRepository positionRepository = new PositionRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!positionRepository.TryGetItemById(id.ToString(), out Position position))
                            accessibilityHandlerData.Redirect($"/{POSITION}/HasNoPermissionsForSee");
                        else if (!organizationRepository.HasPermissionsForSeeItem(position.GetOrganization(accessibilityHandlerData.Context)))
                            accessibilityHandlerData.Redirect($"/{POSITION}/HasNoPermissionsForSee");
                        else if (!positionRepository.HasPermissionsForSeeItem(position))
                            accessibilityHandlerData.Redirect($"/{POSITION}/HasNoPermissionsForSee");
                        else
                        {
                            // Кеширование найденной должности
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;
                            cachService.CacheItem(currentUser.Id, "CurrentPositionData", position);
                        }
                    }
                    else accessibilityHandlerData.Redirect($"/{POSITION}/HasNoPermissionsForSee");
                    break;
                default:
                    break;
            }
        }
    }
}
