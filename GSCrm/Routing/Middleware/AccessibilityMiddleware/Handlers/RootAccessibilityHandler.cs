using System.Linq;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using Microsoft.EntityFrameworkCore;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class RootAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "NotificationsSetting":
                    // Проверка наличия настроек уведомлений
                    User currentUser = accessibilityHandlerData.GetCurrentUser();
                    UserNotificationsSetting userNotificationsSetting = accessibilityHandlerData.Context.UserNotificationsSettings
                        .AsNoTracking().FirstOrDefault(notSetting => notSetting.UserId == currentUser.Id);
                    if (userNotificationsSetting == null)
                        accessibilityHandlerData.Redirect($"/{NOT_SETTING}/HasNoPermissionsForSee");
                    else
                    {
                        // Если все ок, то модель кешируется
                        ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService(typeof(ICachService)) as ICachService;
                        cachService.AddOrUpdateEntity(currentUser, userNotificationsSetting);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
