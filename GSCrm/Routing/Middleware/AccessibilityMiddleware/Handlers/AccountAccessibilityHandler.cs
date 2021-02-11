using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using Microsoft.AspNetCore.Http;
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
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        // Проверки на наличие клиента и доступа к нему у пользователя
                        AccountRepository accountRepository = new AccountRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!accountRepository.TryGetItemById(id.ToString(), out Account account))
                            accessibilityHandlerData.Redirect($"/{ACCOUNT}/HasNoPermissionsForSee");
                        else if (!accountRepository.HasPermissionsForSeeItem(account))
                            accessibilityHandlerData.Redirect($"/{ACCOUNT}/HasNoPermissionsForSee");
                        else
                        {
                            // Кеширование найденного клиента
                            User currentUser = accessibilityHandlerData.GetCurrentUser();
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;
                            cachService.CacheItem(currentUser.Id, "CurrentAccountData", account);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
