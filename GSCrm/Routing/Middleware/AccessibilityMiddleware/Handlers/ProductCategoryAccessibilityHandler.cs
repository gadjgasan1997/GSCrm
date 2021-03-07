using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Localization;
using Microsoft.Extensions.DependencyInjection;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Mapping;

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
                        ProductCategoryRepository prodCatRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!prodCatRepository.TryGetItemById(id.ToString(), out ProductCategory productCategory))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("ProductCategoryNotFound", resManager));
                            return;
                        }

                        // Кеширование текущей организации и категории
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        if (accessibilityHandlerData.TryCacheCurrentOrganization(currentUser, cachService, productCategory.OrganizationId))
                        {
                            // Маппинг в модель отображения
                            ProductCategoryMap productCategoryMap = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            ProductCategoryViewModel prodCatViewModel = productCategoryMap.DataToViewModel(productCategory);
                            CacheCurrentProductCategory(currentUser, cachService, productCategory, prodCatViewModel);
                        }
                    }
                    break;

                case "Create":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    break;

                case "Update":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string prodCatId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.Form, "id");
                        if (string.IsNullOrEmpty(prodCatId))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("ProductCategoryNotFound", resManager));
                            return;
                        }

                        // Попытка получить категорию из кеша
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        if (!cachService.TryGetCachedEntity(currentUser, prodCatId, out ProductCategory productCategory) ||
                            !cachService.TryGetCachedEntity(currentUser, prodCatId, out ProductCategoryViewModel prodCatViewModel))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("ProductCategoryNotFound", resManager));
                            return;
                        }

                        // Кеширование текущей организации и категории
                        if (accessibilityHandlerData.TryCacheCurrentOrganization(currentUser, cachService, productCategory.OrganizationId))
                            CacheCurrentProductCategory(currentUser, cachService, productCategory, prodCatViewModel);
                    }
                    break;
                default:
                    break;
            }
        }

        private void CacheCurrentProductCategory(User currentUser, ICachService cachService, ProductCategory productCategory, ProductCategoryViewModel prodCatViewModel)
        {
            cachService.CacheEntity(currentUser, productCategory);
            cachService.CacheCurrentEntity(currentUser, productCategory);
            cachService.CacheEntity(currentUser, prodCatViewModel);
            cachService.CacheCurrentEntity(currentUser, prodCatViewModel);
        }
    }
}
