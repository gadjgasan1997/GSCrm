using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Models.Enums;
using GSCrm.Localization;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountContactAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Contact":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string accountContactId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        if (!string.IsNullOrEmpty(accountContactId))
                        {
                            // Попытка получить контакт
                            AccountContactRepository contactRepository = new AccountContactRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            if (!contactRepository.TryGetItemById(accountContactId, out AccountContact accountContact))
                            {
                                accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountContactNotFound", resManager));
                                return;
                            }

                            // Попытка закешировать клиента как текущего
                            User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                            if (accessibilityHandlerData.TryCacheCurrentAccount(cachService, resManager, currentUser, accountContact.AccountId))
                            {
                                // Маппинг в модель отображения и ее кеширование
                                AccountContactMap accountContactMap = new AccountContactMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                                AccountContactViewModel contactViewModel = accountContactMap.DataToViewModel(accountContact);
                                CacheAccountContact(cachService, currentUser, accountContact, contactViewModel);
                            }
                        }
                        else accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountContactNotFound", resManager));
                    }
                    break;

                case "Create":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "accountId");
                    break;

                case "Update":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string accountContactId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.Form, "id");
                        if (string.IsNullOrEmpty(accountContactId))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountContactNotFound", resManager));
                            return;
                        }

                        // Попытка получить контакт из кеша
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        if (!cachService.TryGetCachedEntity(currentUser, accountContactId, out AccountContact accountContact) ||
                            !cachService.TryGetCachedEntity(currentUser, accountContactId, out AccountContactViewModel contactViewModel))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountContactNotFound", resManager));
                            return;
                        }

                        // Кеширование текущего клиента и контакта
                        if (accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "accountId"))
                            CacheAccountContact(cachService, currentUser, accountContact, contactViewModel);
                    }
                    break;

                default:
                    break;
            }
        }

        private void CacheAccountContact(ICachService cachService, User currentUser, AccountContact accountContact, AccountContactViewModel contactViewModel)
        {
            cachService.CacheEntity(currentUser, accountContact);
            cachService.CacheCurrentEntity(currentUser, accountContact);
            cachService.CacheEntity(currentUser, contactViewModel);
            cachService.CacheCurrentEntity(currentUser, contactViewModel);
        }
    }
}
