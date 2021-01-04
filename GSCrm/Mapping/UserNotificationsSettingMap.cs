using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;
using GSCrm.Data;
using GSCrm.Notifications;

namespace GSCrm.Mapping
{
    public class UserNotificationsSettingMap : BaseMap<UserNotificationsSetting, UserNotificationsSettingViewModel>
    {
        public UserNotificationsSettingMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override UserNotificationsSettingViewModel DataToViewModel(UserNotificationsSetting userNotSetting)
            => new UserNotificationsSettingViewModel()
            {
                Id = userNotSetting.Id,
                OrgInvoiceNot = userNotSetting.OrgInvoiceNot,
                TOrgInvoiceNot = userNotSetting.TOrgInvoiceNot
            };

        public override UserNotificationsSetting OnModelUpdate(UserNotificationsSettingViewModel userNotViewModel)
        {
            UserNotificationsSetting userNotSetting = base.OnModelUpdate(userNotViewModel);
            userNotSetting.OrgInvoiceNot = userNotViewModel.OrgInvoiceNot;
            userNotSetting.TOrgInvoiceNot = userNotViewModel.TOrgInvoiceNot;
            return userNotSetting;
        }

        /// <summary>
        /// Метод инициализирует настройки уведомлений по умолчанию
        /// </summary>
        /// <param name="userNotSetting"></param>
        public UserNotificationsSetting InitNotSetting(UserNotificationsSetting userNotSetting)
        {
            userNotSetting.OrgInvoiceNot = true;
            userNotSetting.TOrgInvoiceNot = NotificationTarget.Inbox;
            return userNotSetting;
        }
    }
}
