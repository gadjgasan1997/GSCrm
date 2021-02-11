using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountAddressAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Address":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        User currentUser = accessibilityHandlerData.GetCurrentUser();
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;

                        // Поптыка получить адрес
                        AccountAddressRepository addressRepository = new AccountAddressRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        AccountRepository accountRepository = new AccountRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!addressRepository.TryGetItemById(id.ToString(), out AccountAddress accountAddress))
                            cachService.CacheItem(currentUser.Id, $"{PC}{ACC_ADDRESS}", false.ToString());
                        else if (!accountRepository.HasPermissionsForSeeItem(accountAddress.GetAccount(accessibilityHandlerData.Context)))
                            cachService.CacheItem(currentUser.Id, $"{PC}{ACC_ADDRESS}", false.ToString());

                        // Если все ок, то кеширование результата
                        else
                        {
                            cachService.CacheItem(currentUser.Id, $"{PC}{ACC_ADDRESS}", true.ToString());
                            cachService.CacheItem(currentUser.Id, "CurrentAccountAddressData", accountAddress);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
