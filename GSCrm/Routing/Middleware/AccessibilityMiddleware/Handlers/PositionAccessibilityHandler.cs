using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Models.Enums;
using GSCrm.Mapping;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
                case "PositionEmployees":
                case "PositionSubPositions":
                    {
                        // Получение id должности из ссылки запроса
                        string positionId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        if (!string.IsNullOrEmpty(positionId))
                        {
                            // Проверки на наличие должности и доступа к ней у пользователя
                            PositionRepository positionRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            OrganizationRepository organizationRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            if (!positionRepository.TryGetItemById(positionId, out Position position))
                            {
                                accessibilityHandlerData.Redirect($"/{POSITION}/HasNoPermissionsForSee");
                                return;
                            }

                            Organization organization = position.GetOrganization(accessibilityHandlerData.Context);
                            if (!organizationRepository.HasPermissionsForSeeItem(organization) ||
                                !positionRepository.HasPermissionsForSeeItem(position))
                            {
                                accessibilityHandlerData.Redirect($"/{ORGANIZATION}/HasNoPermissionsForSee");
                                return;
                            }

                            // Кеширование найденной должности и организации(на случай прямого перехода по ссылке на должность)
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                            cachService.CacheEntity(currentUser, organization);
                            cachService.CacheCurrentEntity(currentUser, organization);
                            cachService.CacheEntity(currentUser, position);
                            cachService.CacheCurrentEntity(currentUser, position);

                            // Маппинг в данные отображения
                            PositionViewModel positionViewModel = new PositionMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context).DataToViewModelExt(position);

                            // Обновление полей в модели из кеша, если он существует
                            if (cachService.TryGetCachedEntity(currentUser, positionViewModel.Id, out PositionViewModel cachedViewModel))
                                positionViewModel.Refresh(cachedViewModel);

                            // Кеширование модели
                            cachService.CacheEntity(currentUser, positionViewModel);
                            cachService.CacheCurrentEntity(currentUser, positionViewModel);
                        }
                        else accessibilityHandlerData.Redirect($"/{POSITION}/HasNoPermissionsForSee");
                    }
                    break;

                case "SearchEmployee":
                case "SearchSubPosition":
                    accessibilityHandlerData.CacheCurrentPosition(RequestSourceType.Form);
                    break;

                case "ClearSearchEmployee":
                case "ClearSearchSubPosition":
                    accessibilityHandlerData.CacheCurrentPosition(RequestSourceType.RouteValues);
                    break;

                case "Create":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    break;

                case "Update":
                case "Unlock":
                case "ChangeDivision":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.CacheCurrentPosition();
                    break;

                default:
                    break;
            }
        }

        private void CheckPermissionsAndCache(AccessibilityHandlerData accessibilityHandlerData, string positionId)
        {
            
        }
    }
}
