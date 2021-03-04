using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Localization;
using Microsoft.Extensions.DependencyInjection;

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
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();

                        // Проверка наличия категории и доступа у пользователя к ней
                        ProductCategoryRepository prodCatRepository = new ProductCategoryRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!prodCatRepository.TryGetItemById(id.ToString(), out ProductCategory productCategory))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("ProductCategoryNotFound", resManager));
                            return;
                        }

                        Organization organization = productCategory.GetOrganization(accessibilityHandlerData.Context);
                        if (!organizationRepository.HasPermissionsForSeeItem(organization))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("OrganizationNotFound", resManager));
                            return;
                        }
                        if (!prodCatRepository.HasPermissionsForSeeItem(productCategory))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("ProductCategoryNotFound", resManager));
                            return;
                        }

                        // Кешируется текущая организация
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        cachService.CacheEntity(currentUser, organization);
                        cachService.CacheCurrentEntity(currentUser, organization);
                        cachService.CacheEntity(currentUser, productCategory);
                        cachService.CacheCurrentEntity(currentUser, productCategory);
                    }
                    break;

                case "Create":
                case "Delete":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    break;

                case "Update":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    // TO DO Добавить кеширование продуктовой категории при изменении
                    break;
                default:
                    break;
            }
        }
    }
}
