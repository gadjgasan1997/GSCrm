using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Localization;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountInvoiceAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Invoice":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string accountInvoiceId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        if (!string.IsNullOrEmpty(accountInvoiceId))
                        {
                            // Поптыка получить реквизиты
                            AccountInvoiceRepository invoiceRepository = new AccountInvoiceRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            if (!invoiceRepository.TryGetItemById(accountInvoiceId, out AccountInvoice accountInvoice))
                            {
                                accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountInvoiceNotFound", resManager));
                                return;
                            }

                            // Попытка закешировать клиента как текущего
                            User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                            if (accessibilityHandlerData.TryCacheCurrentAccount(cachService, resManager, currentUser, accountInvoice.AccountId))
                            {
                                // Маппинг в модель отображения и ее кеширование
                                AccountInvoiceMap accountInvoiceMap = new AccountInvoiceMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                                AccountInvoiceViewModel invoiceViewModel = accountInvoiceMap.DataToViewModel(accountInvoice);
                                CacheAccountInvoice(cachService, currentUser, accountInvoice, invoiceViewModel);
                            }
                        }
                        else accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountInvoiceNotFound", resManager));
                    }
                    break;

                case "Create":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "accountId");
                    break;

                case "Update":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string accountInvoiceId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.Form, "id");
                        if (string.IsNullOrEmpty(accountInvoiceId))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountInvoiceNotFound", resManager));
                            return;
                        }
                        
                        // Попытка получить реквизиты из кеша
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        if (!cachService.TryGetCachedEntity(currentUser, accountInvoiceId, out AccountInvoice accountInvoice) ||
                            !cachService.TryGetCachedEntity(currentUser, accountInvoiceId, out AccountInvoiceViewModel invoiceViewModel))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountInvoiceNotFound", resManager));
                            return;
                        }

                        // Кеширование текущего клиента и банковских реквизитов
                        if (accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "accountId"))
                            CacheAccountInvoice(cachService, currentUser, accountInvoice, invoiceViewModel);
                    }
                    break;

                default:
                    break;
            }
        }

        private void CacheAccountInvoice(ICachService cachService, User currentUser, AccountInvoice accountInvoice, AccountInvoiceViewModel invoiceViewModel)
        {
            cachService.CacheEntity(currentUser, accountInvoice);
            cachService.CacheCurrentEntity(currentUser, accountInvoice);
            cachService.CacheEntity(currentUser, invoiceViewModel);
            cachService.CacheCurrentEntity(currentUser, invoiceViewModel);
        }
    }
}
