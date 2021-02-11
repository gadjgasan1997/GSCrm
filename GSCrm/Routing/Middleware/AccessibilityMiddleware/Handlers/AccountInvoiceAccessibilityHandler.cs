using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountInvoiceAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Invoice":
                    if (accessibilityHandlerData.RouteValues.TryGetValue("id", out object id))
                    {
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;

                        // Поптыка получить реквизиты
                        AccountInvoiceRepository invoiceRepository = new AccountInvoiceRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        AccountRepository accountRepository = new AccountRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        if (!invoiceRepository.TryGetItemById(id.ToString(), out AccountInvoice accountInvoice))
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_INVOICE}", false);
                        else if (!accountRepository.HasPermissionsForSeeItem(accountInvoice.GetAccount(accessibilityHandlerData.Context)))
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_INVOICE}", false);

                        // Если все ок, то кеширование результата
                        else
                        {
                            cachService.AddOrUpdate(currentUser, $"{PC}{ACC_INVOICE}", true);
                            cachService.AddOrUpdateEntity(currentUser, accountInvoice);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
