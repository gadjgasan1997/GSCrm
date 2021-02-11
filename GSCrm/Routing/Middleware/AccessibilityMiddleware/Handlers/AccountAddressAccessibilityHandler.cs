using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
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
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_ADDRESS}", false);
                        else if (!accountRepository.HasPermissionsForSeeItem(accountAddress.GetAccount(accessibilityHandlerData.Context)))
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_ADDRESS}", false);

                        // Если все ок, то кеширование результата
                        else
                        {
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_ADDRESS}", true);
                            cachService.AddOrUpdateEntity(currentUser, accountAddress);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
