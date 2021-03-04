using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
                        if (!responsibilityRepository.TryGetItemById(id.ToString(), out Responsibility responsibility))
                        {
                            accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                            return;
                        }

                        // Проверки на наличие организации и доступа к ней у пользователя
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        Organization organization = responsibility.GetOrganization(accessibilityHandlerData.Context);
                        if (!organizationRepository.HasPermissionsForSeeItem(organization))
                        {
                            accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                            return;
                        }

                        if (!responsibilityRepository.HasPermissionsForSeeItem(responsibility))
                        {
                            accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                            return;
                        }

                        // Маппинг организации и полномочия в модель отображения
                        OrganizationMap organizationMap = new OrganizationMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        OrganizationViewModel organizationViewModel = organizationMap.DataToViewModel(organization);
                        ResponsibilityMap responsibilityMap = new ResponsibilityMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        ResponsibilityViewModel responsibilityViewModel = responsibilityMap.DataToViewModel(responsibility);

                        // Кеширование найденного полномочия и организации
                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        cachService.CacheEntity(currentUser, organization);
                        cachService.CacheCurrentEntity(currentUser, organization);
                        cachService.CacheEntity(currentUser, organizationViewModel);
                        cachService.CacheCurrentEntity(currentUser, organizationViewModel);
                        cachService.CacheEntity(currentUser, responsibility);
                        cachService.CacheCurrentEntity(currentUser, responsibility);
                        cachService.CacheEntity(currentUser, responsibilityViewModel);
                        cachService.CacheCurrentEntity(currentUser, responsibilityViewModel);
                    }
                    else accessibilityHandlerData.Redirect($"/{RESPONSIBILITY}/HasNoPermissionsForSee");
                    break;
                case "Create":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    break;
                case "Update":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.CacheCurrentResponsibility();
                    break;
                default:
                    break;
            }
        }
    }
}
