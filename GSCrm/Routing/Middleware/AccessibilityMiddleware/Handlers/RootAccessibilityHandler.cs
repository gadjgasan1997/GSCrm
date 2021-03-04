using System.Linq;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static GSCrm.CommonConsts;
using GSCrm.Mapping;
using GSCrm.Repository;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class RootAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
            User currentUser = accessibilityHandlerData.GetCurrentUser();
            switch (accessibilityHandlerData.ActionName)
            {
                case "Organizations":
                    {
                        if (cachService.GetCachedCurrentEntity<OrganizationsViewModel>(currentUser) is not OrganizationsViewModel)
                            cachService.CacheCurrentEntity(currentUser, new OrganizationsViewModel());
                    }
                    break;
                case "AllAccounts":
                case "CurrentAccounts":
                    {
                        if (cachService.GetCachedCurrentEntity<AccountsViewModel>(currentUser) is not AccountsViewModel)
                            cachService.CacheCurrentEntity(currentUser, new AccountsViewModel());
                    }
                    break;
                case "UserNotifications":
                    {
                        if (cachService.GetCachedCurrentEntity<UserNotificationsViewModel>(currentUser) is not UserNotificationsViewModel)
                            cachService.CacheCurrentEntity(currentUser, new UserNotificationsViewModel());
                    }
                    break;
                case "NotificationsSettings":
                    {
                        // Проверка наличия настроек уведомлений
                        UserNotificationsSetting userNotSetting = accessibilityHandlerData.Context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(notSetting => notSetting.UserId == currentUser.Id);
                        if (userNotSetting == null)
                        {
                            accessibilityHandlerData.Redirect($"/{NOTS_SETTING}/HasNoPermissionsForSee");
                            return;
                        }

                        // Маппинг и кеширование модели
                        UserNotificationsSettingMap userNotSettingMap = new UserNotificationsSettingMap(accessibilityHandlerData.ServiceProvider, accessibilityHandlerData.Context);
                        UserNotificationsSettingViewModel userNotSettingViewModel = userNotSettingMap.DataToViewModel(userNotSetting);
                        cachService.CacheEntity(currentUser, userNotSetting);
                        cachService.CacheCurrentEntity(currentUser, userNotSetting);
                        cachService.CacheEntity(currentUser, userNotSettingViewModel);
                        cachService.CacheCurrentEntity(currentUser, userNotSettingViewModel);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
