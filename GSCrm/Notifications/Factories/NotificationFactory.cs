using GSCrm.Data;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Notifications.Services;
using Microsoft.Extensions.Configuration;
using System;
using Task = System.Threading.Tasks.Task;

namespace GSCrm.Notifications.Factories
{
    public abstract class NotificationFactory<TNotificationParams>
        where TNotificationParams : INotificationParams
    {
        /// <summary>
        /// Менеджер ресурсов для доступа к переводам
        /// </summary>
        protected IResManager resManager;
        protected IServiceProvider serviceProvider;
        protected ApplicationDbContext context;
        protected IConfiguration configuration;
        /// <summary>
        /// Параметры для конкретного типа уведомлений
        /// </summary>
        protected readonly TNotificationParams notificationParams;

        public NotificationFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TNotificationParams notificationParams)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            resManager = serviceProvider.GetService(typeof(IResManager)) as IResManager;
            this.notificationParams = notificationParams;
            configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
        }

        /// <summary>
        /// Метод создает уведомление
        /// </summary>
        /// <param name="notificationTarget"></param>
        /// <returns></returns>
        protected abstract Notification Create(NotificationTarget notificationTarget);

        /// <summary>
        /// Метод создает и ассинхронно отсылает уведомление пользователю(вызывается из дочерних классов)
        /// </summary>
        /// <param name="targetUser"></param>
        /// <param name="notificationTarget"></param>
        /// <returns></returns>
        protected async Task SendAsync(User targetUser, NotificationTarget notificationTarget)
        {
            switch (notificationTarget)
            {
                case NotificationTarget.Email:
                    await new EmailNotificationService(serviceProvider, context).SendAsync((EmailNotification)Create(notificationTarget), targetUser);
                    break;
                default:
                case NotificationTarget.Inbox:
                    new InboxNotificationService(serviceProvider, context).Send((InboxNotification)Create(notificationTarget), targetUser);
                    break;
            }
        }

        /// <summary>
        /// Метод создает и синхронно отсылает уведомление пользователю(вызывается из дочерних классов)
        /// </summary>
        /// <param name="targetUser"></param>
        /// <param name="notificationTarget"></param>
        protected void Send(User targetUser, NotificationTarget notificationTarget)
        {
            switch (notificationTarget)
            {
                case NotificationTarget.Email:
                    new EmailNotificationService(serviceProvider, context).Send((EmailNotification)Create(notificationTarget), targetUser);
                    break;
                default:
                case NotificationTarget.Inbox:
                    new InboxNotificationService(serviceProvider, context).Send((InboxNotification)Create(notificationTarget), targetUser);
                    break;
            }
        }

        protected abstract string GetEmailTemplate();
    }
}
