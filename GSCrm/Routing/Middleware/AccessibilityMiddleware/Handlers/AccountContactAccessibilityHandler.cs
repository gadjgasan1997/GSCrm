using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
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
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_CONTACT}", false);
                        else if (!accountRepository.HasPermissionsForSeeItem(accountContact.GetAccount(accessibilityHandlerData.Context)))
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_CONTACT}", false);

                        // Если все ок, то кеширование результата
                        else
                        {
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_CONTACT}", true);
                            cachService.AddOrUpdateEntity(currentUser, accountContact);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
