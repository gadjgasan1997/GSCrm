using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class ProductCategoryAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Initalize":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;

                        // Проверка наличия категории и доступа у пользователя к ней
                        ProductCategoryRepository prodCatRepository = new ProductCategoryRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!prodCatRepository.TryGetItemById(id.ToString(), out ProductCategory productCategory))
                            cachService.CacheItem(currentUser.Id, $"{PC}{PROD_CATS}", false.ToString());
                        else if (!organizationRepository.HasPermissionsForSeeItem(productCategory.GetOrganization(accessibilityHandlerData.Context)))
                            cachService.CacheItem(currentUser.Id, $"{PC}{PROD_CATS}", false.ToString());
                        else if (!prodCatRepository.HasPermissionsForSeeItem(productCategory))
                            cachService.CacheItem(currentUser.Id, $"{PC}{PROD_CATS}", false.ToString());

                        // Если все ок, то кеширование результата
                        else
                        {
                            cachService.CacheItem(currentUser.Id, $"{PC}{PROD_CATS}", true.ToString());
                            cachService.CacheItem(currentUser.Id, "CurrentProductCategoryData", productCategory);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
