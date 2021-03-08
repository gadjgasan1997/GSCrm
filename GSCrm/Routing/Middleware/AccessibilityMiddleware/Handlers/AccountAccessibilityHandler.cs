using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Mapping;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Account":
                case "Contacts":
                case "Addresses":
                case "Invoices":
                    string accountId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                    if (!string.IsNullOrEmpty(accountId))
                    {
                        // Проверки на наличие клиента и доступа к нему у пользователя
                        AccountRepository accountRepository = new(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!accountRepository.TryGetItemById(accountId, out Account account) ||
                            !accountRepository.HasPermissionsForSeeItem(account))
                        {
                            accessibilityHandlerData.Redirect($"/{ACCOUNT}/HasNoPermissionsForSee");
                            return;
                        }

                        // Кеширование найденного клиента
                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        cachService.CacheEntity(currentUser, account);
                        cachService.CacheCurrentEntity(currentUser, account);

                        // Маппинг в модель отображения
                        AccountMap accountMap = new AccountMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        AccountViewModel accountViewModel = accountMap.DataToViewModel(account);

                        // Обновление полей в модели из кеша, если он существует
                        if (cachService.TryGetCachedEntity(currentUser, accountViewModel.Id, out AccountViewModel cachedViewModel))
                            accountViewModel.Refresh(cachedViewModel);

                        // Кеширование модели
                        cachService.CacheEntity(currentUser, accountViewModel);
                        cachService.CacheCurrentEntity(currentUser, accountViewModel);
                    }
                    break;

                case "Update":
                case "UnlockAccount":
                case "ChangePrimaryContact":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form);
                    break;

                case "ChangeLegalAddress":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "accountId");
                    break;

                case "ChangeSite":
                case "HasAccNotLegalAddress":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.RouteValues);
                    break;

                case "SearchContact":
                case "SearchAddress":
                case "SearchInvoice":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "id", RequestBreakType.Redirect);
                    break;

                case "ClearContactSearch":
                case "ClearAddressSearch":
                case "ClearInvoiceSearch":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.RouteValues, "id", RequestBreakType.Redirect);
                    break;

                default:
                    break;
            }
        }
    }
}
