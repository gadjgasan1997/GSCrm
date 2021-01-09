using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using GSCrm.Mapping.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GSCrm.Mapping
{
    public class UserNotificationMap : BaseMap<UserNotification, UserNotificationViewModel>
    {
        public UserNotificationMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override sealed UserNotificationViewModel DataToViewModel(UserNotification userNot)
        {
            InboxNotification inboxNot = context.InboxNotifications.AsNoTracking().FirstOrDefault(i => i.Id == userNot.NotificationId);

            // В зависимости от типа уведомления
            return inboxNot.NotificationType switch
            {
                NotificationType.OrgInvite => new OrgInviteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                NotificationType.DivDelete => new DivDeleteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                _ => default
            };
        }

        /// <summary>
        /// Заполнение общих для всех уведомлений полей
        /// </summary>
        /// <param name="userNot"></param>
        /// <param name="inboxNot"></param>
        /// <returns></returns>
        protected TNotViewModel GetNewNotViewModel<TNotViewModel>(UserNotification userNot, InboxNotification inboxNot)
            where TNotViewModel : UserNotificationViewModel, new()
        {
            TNotViewModel userNotViewModel = new TNotViewModel()
            {
                Id = userNot.Id,
                HasRead = userNot.HasRead,
                NotificationId = inboxNot.Id,
                NotificationSource = inboxNot.NotificationSource,
                NotificationType = inboxNot.NotificationType,
            };
            return userNotViewModel;
        }
    }
}
