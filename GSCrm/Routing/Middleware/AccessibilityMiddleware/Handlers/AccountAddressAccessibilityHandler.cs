using GSCrm.Models;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Repository;
using GSCrm.Localization;
using GSCrm.Models.Enums;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountAddressAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "Address":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string accountAddressId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.RouteValues, "id");
                        if (!string.IsNullOrEmpty(accountAddressId))
                        {
                            // Поптыка получить адрес
                            AccountAddressRepository addressRepository = new AccountAddressRepository(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                            if (!addressRepository.TryGetItemById(accountAddressId, out AccountAddress accountAddress))
                            {
                                accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountAddressNotFound", resManager));
                                return;
                            }

                            // Попытка закешировать клиента как текущего
                            User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                            if (accessibilityHandlerData.TryCacheCurrentAccount(currentUser, cachService, accountAddress.AccountId))
                            {
                                // Маппинг в модель отображения и ее кеширование
                                AccountAddressMap accountAddressMap = new AccountAddressMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                                AccountAddressViewModel addressViewModel = accountAddressMap.DataToViewModel(accountAddress);
                                CacheAccountAddress(cachService, currentUser, accountAddress, addressViewModel);
                            }
                        }
                        else accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountAddressNotFound", resManager));
                    }
                    break;

                case "Create":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.Form, "accountId");
                    break;

                case "Update":
                    {
                        IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
                        string accountAddressId = accessibilityHandlerData.GetIdFromRequest(RequestSourceType.Form, "id");
                        if (string.IsNullOrEmpty(accountAddressId))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountAddressNotFound", resManager));
                            return;
                        }

                        // Попытка получить адрес из кеша
                        User currentUser = accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                        if (!cachService.TryGetCachedEntity(currentUser, accountAddressId, out AccountAddress accountAddress) ||
                            !cachService.TryGetCachedEntity(currentUser, accountAddressId, out AccountAddressViewModel addressViewModel))
                        {
                            accessibilityHandlerData.BreakRequest(404, GetRecordNotFoundMessage("AccountAddressNotFound", resManager));
                            return;
                        }

                        // Кеширование текущего клиента и адреса
                        if (accessibilityHandlerData.TryCacheCurrentAccount(currentUser, cachService, accountAddress.AccountId))
                            CacheAccountAddress(cachService, currentUser, accountAddress, addressViewModel);
                    }
                    break;

                default:
                    break;
            }
        }

        private void CacheAccountAddress(ICachService cachService, User currentUser, AccountAddress accountAddress, AccountAddressViewModel addressViewModel)
        {
            cachService.CacheEntity(currentUser, accountAddress);
            cachService.CacheCurrentEntity(currentUser, accountAddress);
            cachService.CacheEntity(currentUser, addressViewModel);
            cachService.CacheCurrentEntity(currentUser, addressViewModel);
        }
    }
}
