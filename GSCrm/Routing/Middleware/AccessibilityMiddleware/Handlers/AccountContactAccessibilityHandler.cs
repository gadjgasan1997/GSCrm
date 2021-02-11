using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountContactAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Contact":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;

                        // Поптыка получить контакт
                        AccountContactRepository contactRepository = new AccountContactRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        AccountRepository accountRepository = new AccountRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!contactRepository.TryGetItemById(id.ToString(), out AccountContact accountContact))
                            cachService.CacheItem(currentUser.Id, $"{PC}{ACC_CONTACT}", false.ToString());
                        else if (!accountRepository.HasPermissionsForSeeItem(accountContact.GetAccount(accessibilityHandlerData.Context)))
                            cachService.CacheItem(currentUser.Id, $"{PC}{ACC_CONTACT}", false.ToString());

                        // Если все ок, то кеширование результата
                        else
                        {
                            cachService.CacheItem(currentUser.Id, $"{PC}{ACC_CONTACT}", true.ToString());
                            cachService.CacheItem(currentUser.Id, "CurrentAccountContactData", accountContact);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
