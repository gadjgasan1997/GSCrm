using GSCrm.Data;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Notifications.Factories
{
    /// <summary>
    /// Фабрика для рассылки личных уведомлений
    /// </summary>
    public abstract class UserNotificationFactory<TNotificationParams> :  NotificationFactory<TNotificationParams>
        where TNotificationParams : INotificationParams
    {
        public UserNotificationFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TNotificationParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        /// <summary>
        /// Метод отправляет уведомление адресатам без привязки к опредлеленной организации
        /// </summary>
        /// <param name="targetUserIdList"></param>
        /// <returns></returns>
        public void Send(IEnumerable<Guid> targetUserIdList)
        {
            foreach (Guid targetUserId in targetUserIdList)
            {
                UserNotificationsSetting userNotSetting = context.UserNotificationsSettings
                    .Include(u => u.User)
                    .AsNoTracking().FirstOrDefault(i => i.UserId == targetUserId.ToString());

                // Если требуется раассылка
                if (userNotSetting != null && NeedNotification(userNotSetting))
                {
                    // Для всех способов рассыкли, доступных для этого типа уведомления
                    GetNotificationTargets(userNotSetting).ForEach(async notificationTarget => 
                        await SendAsync(userNotSetting.User, notificationTarget));
                }
            }
        }

        /// <summary>
        /// Метод отправляет уведомление одному адресату без привязки к опредлеленной организации
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns></returns>
        public void Send(Guid targetUserId) => Send(new List<Guid>() { targetUserId });

        /// <summary>
        /// Метод возвращает типы рассылок в зависимости от типа уведомления
        /// </summary>
        /// <param name="userNotSetting"></param>
        /// <returns></returns>
        protected abstract List<NotificationTarget> GetNotificationTargets(UserNotificationsSetting userNotSetting);

        /// <summary>
        /// Метод возвращает признак, надо ли отправлять уведомление пользователю
        /// </summary>
        /// <param name="userNotSetting"></param>
        /// <returns></returns>
        protected abstract bool NeedNotification(UserNotificationsSetting userNotSetting);
    }
}
