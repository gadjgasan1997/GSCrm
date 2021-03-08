using System;
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
                    {
                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        TryCacheOrganization(cachService, currentUser, accessibilityHandlerData, out OrganizationViewModel _);
                    }
                    break;

                case "ProductCategories":
                    {
                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        if (TryCacheOrganization(cachService, currentUser, accessibilityHandlerData, out OrganizationViewModel orgViewModel))
                        {
                            // Простановка текущего представления
                            ProductCategoryRepository productCategoryRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, orgViewModel.Id, PROD_CATS);
                            productCategoryRepository.SetViewInfo(orgViewModel.Id, PROD_CATS, viewInfo.CurrentPageNumber);
                            ProductCategoriesViewModel prodCatsViewModel = new()
                            {
                                Id = orgViewModel.Id,
                                OrganizationViewModel = orgViewModel,
                                OrganizationId = orgViewModel.Id
                            };

                            // Попытка обновить модель из кеша
                            if (cachService.TryGetCachedEntity(currentUser, orgViewModel.Id, out ProductCategoriesViewModel cachedViewModel))
                                prodCatsViewModel.Refresh(cachedViewModel);

                            // Кеширование модели
                            cachService.CacheEntity(currentUser, prodCatsViewModel);
                            cachService.CacheCurrentEntity(currentUser, prodCatsViewModel);
                        }
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
                    accessibilityHandlerData.CacheCurrentOrganization(RequestSourceType.Form, "id", RequestBreakType.Redirect);
                    break;

                case "SearchResponsibility":
                    accessibilityHandlerData.CacheCurrentOrganization(RequestSourceType.Form, "id");
                    break;

                case "SearchProductCategories":
                    {
                        string organizationId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.Form, "organizationId");
                        CacheProductCategoriesModel(accessibilityHandlerData, organizationId);
                    }
                    break;

                case "ClearDivisionSearch":
                case "ClearPositionSearch":
                case "ClearEmployeeSearch":
                    accessibilityHandlerData.CacheCurrentOrganization(RequestSourceType.RouteValues, "id", RequestBreakType.Redirect);
                    break;

                case "ClearResponsibilitySearch":
                    accessibilityHandlerData.CacheCurrentOrganization(RequestSourceType.RouteValues, "id");
                    break;

                case "ProductCategoriesClearSearch":
                    {
                        string organizationId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        CacheProductCategoriesModel(accessibilityHandlerData, organizationId);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Метод пытается закешировать организацию как текущую
        /// </summary>
        /// <param name="cachService"></param>
        /// <param name="currentUser"></param>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="orgViewModel"></param>
        /// <returns></returns>
        private bool TryCacheOrganization(ICachService cachService, User currentUser, AccessibilityHandlerData accessibilityHandlerData, out OrganizationViewModel orgViewModel)
        {
            // Получение id организации из ссылки запроса
            orgViewModel = default;
            string organizationId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
            if (string.IsNullOrEmpty(organizationId))
            {
                accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                return false;
            }

            // Попытка получить организацию, на которую перешел пользователь и проверка прав на ее просмотр
            OrganizationRepository organizationRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
            if (!organizationRepository.TryGetItemById(organizationId, out Organization organization) ||
                !organizationRepository.HasPermissionsForSeeItem(organization))
            {
                accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                return false;
            }

            // В случае, если переход осуществляется не на карточку организации и пользователь не принял в нее приглашение, необходимо прервать запрос
            if (accessibilityHandlerData.ActionName != "Organization" && !organizationRepository.HasPermissionsForSeeOrgItem())
            {
                accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                return false;
            }


            // Кеширование найденной организации
            cachService.CacheEntity(currentUser, organization);
            cachService.CacheCurrentEntity(currentUser, organization);

            // Маппинг в модель отображения
            orgViewModel = new OrganizationMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context).DataToViewModel(organization);

            // Обновление полей в модели из кеша, если он существует
            if (cachService.TryGetCachedEntity(currentUser, orgViewModel.Id, out OrganizationViewModel cachedViewModel))
                orgViewModel.Refresh(cachedViewModel);

            // Кеширование модели
            cachService.CacheEntity(currentUser, orgViewModel);
            cachService.CacheCurrentEntity(currentUser, orgViewModel);
            return true;
        }

        /// <summary>
        /// Метод пытается закешировать продуктовую модель организации и саму организацию как текущую
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="organizationId">Id организации</param>
        private void CacheProductCategoriesModel(AccessibilityHandlerData accessibilityHandlerData, string organizationId)
        {
            // Попытка получить id организации
            IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
            if (string.IsNullOrEmpty(organizationId) || !Guid.TryParse(organizationId, out Guid guid))
            {
                accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("OrganizationNotFound", resManager));
                return;
            }

            // Попытка установить организацию как текущую в кеше
            User currentUser = accessibilityHandlerData.GetCurrentUser();
            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
            if (accessibilityHandlerData.TryCacheCurrentOrganization(currentUser, cachService, guid))
            {
                // Попытка получить кеш продуктовой модели, RequestBreakType.Error, $"/{ORGANIZATION}/HasNoPermissionsForSee", 22
                if (!cachService.TryGetCachedEntity(currentUser, guid, out ProductCategoriesViewModel prodCatsViewModel))
                {
                    accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("ProductCategoriesNotFound", resManager));
                    return;
                }

                // Иначе кеширование продуктовой модели как текушей
                cachService.CacheCurrentEntity(currentUser, prodCatsViewModel);
            }
        }
    }
}
