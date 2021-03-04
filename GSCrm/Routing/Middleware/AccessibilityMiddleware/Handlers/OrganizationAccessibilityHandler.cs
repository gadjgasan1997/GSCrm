using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using GSCrm.Mapping;
using GSCrm.Localization;
using GSCrm.Data.ApplicationInfo;
using Microsoft.Extensions.DependencyInjection;
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
                case "Divisions":
                case "Positions":
                case "Employees":
                case "Responsibilities":
                    CheckPermissionsAndCache(accessibilityHandlerData);
                    break;

                case "ProductCategories":
                    {
                        CheckPermissionsAndCache(accessibilityHandlerData);

                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();

                        // Если к организации есть доступ, то установка ее как текущей, на которой находится пользователь
                        OrganizationViewModel orgViewModel = cachService.GetCachedCurrentEntity<OrganizationViewModel>(currentUser);

                        // Простановка текущего представления
                        ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, orgViewModel.Id, PROD_CATS);
                        productCategoryRepository.SetViewInfo(orgViewModel.Id, PROD_CATS, viewInfo.CurrentPageNumber);
                        ProductCategoriesViewModel productCategoriesViewModel = new ProductCategoriesViewModel()
                        {
                            Id = orgViewModel.Id,
                            OrganizationViewModel = orgViewModel,
                            OrganizationId = orgViewModel.Id
                        };
                        cachService.CacheEntity(currentUser, productCategoriesViewModel);
                        cachService.CacheCurrentEntity(currentUser, productCategoriesViewModel);
                    }
                    break;

                case "GetProductCategoriesData":
                    {
                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        
                        // Попытка получить продуктовую модель из кеша
                        // Если все ок, то она кешируется как текущая
                        string organizationId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        if (cachService.TryGetCachedEntity(currentUser, organizationId, out ProductCategoriesViewModel productCategoriesViewModel))
                            cachService.CacheCurrentEntity(currentUser, productCategoriesViewModel);
                        else accessibilityHandlerData.BreakRequest(400, GetUnhandledExceptionMessage("ProductCategoriesUnhandledException", resManager));
                    }
                    break;

                case "SearchDivision":
                case "SearchPosition":
                case "SearchEmployee":
                case "SearchResponsibility":
                case "SearchProductCategories":
                    accessibilityHandlerData.CacheCurrentOrganization(RequestSourceType.Form, "id");
                    break;

                case "ClearDivisionSearch":
                case "ClearPositionSearch":
                case "ClearEmployeeSearch":
                case "ClearResponsibilitySearch":
                case "ProductCategoriesClearSearch":
                    accessibilityHandlerData.CacheCurrentOrganization(RequestSourceType.RouteValues, "id");
                    break;

                default:
                    break;
            }
        }

        private void CheckPermissionsAndCache(AccessibilityHandlerData accessibilityHandlerData)
        {
            // Получение id организации из ссылки запроса
            string organizationId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
            if (!string.IsNullOrEmpty(organizationId))
            {
                // Попытка получить организацию, на которую перешел пользователь и проверка прав на ее просмотр
                OrganizationRepository organizationRepository = new OrganizationRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                if (!organizationRepository.TryGetItemById(organizationId, out Organization organization))
                {
                    accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                    return;
                }

                if (!organizationRepository.HasPermissionsForSeeItem(organization))
                {
                    accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                    return;
                }

                // Кеширование найденной организации
                User currentUser = accessibilityHandlerData.GetCurrentUser();
                ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                cachService.CacheEntity(currentUser, organization);
                cachService.CacheCurrentEntity(currentUser, organization);

                // Маппинг в модель отображения
                OrganizationViewModel orgViewModel = new OrganizationMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context).DataToViewModel(organization);

                // Обновление полей в модели из кеша, если он существует
                if (cachService.TryGetCachedEntity(currentUser, orgViewModel.Id, out OrganizationViewModel cachedViewModel))
                    orgViewModel.Refresh(cachedViewModel);

                // Кеширование модели
                cachService.CacheEntity(currentUser, orgViewModel);
                cachService.CacheCurrentEntity(currentUser, orgViewModel);
            }
            else accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
        }
    }
}
