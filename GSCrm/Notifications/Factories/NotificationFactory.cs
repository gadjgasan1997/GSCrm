using GSCrm.Data;
using GSCrm.Factories;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Notifications.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        protected readonly IResManager resManager;
        protected readonly IServiceProvider serviceProvider;
        protected readonly ApplicationDbContext context;
        protected readonly HttpContext httpContext;
        protected readonly IConfiguration configuration;
        /// <summary>
        /// Параметры для конкретного типа уведомлений
        /// </summary>
        protected readonly TNotificationParams notificationParams;
        /// <summary>
        /// Уведомление, высылаемое по почте
        /// </summary>
        protected EmailNotification emailNotification;
        /// <summary>
        /// Уведомление, высылаемое в личные сообщения
        /// </summary>
        protected InboxNotification inboxNotification;
        /// <summary>
        /// Признак, является уведомление новым или оно уже создано
        /// </summary>
        private bool isNewNotification;
        /// <summary>
        /// Хелпер для работы с урлами
        /// </summary>
        protected readonly IUrlHelper urlHelper;

        protected NotificationFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TNotificationParams notificationParams)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            resManager = serviceProvider.GetService(typeof(IResManager)) as IResManager;
            this.notificationParams = notificationParams;
            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            httpContext = userContextServices.HttpContext;
            configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            urlHelper = serviceProvider.GetService(typeof(IUrlHelper)) as IUrlHelper;
        }

        /// <summary>
        /// Метод создает уведомление
        /// </summary>
        /// <param name="notificationTarget"></param>
        /// <returns></returns>
        protected abstract Notification Create(NotificationTarget notificationTarget);

        /// <summary>
        /// Метод иниализирует уведомления по умолчанию
        /// </summary>
        /// <param name="notificationTarget"></param>
        protected virtual void InitNotification(NotificationTarget notificationTarget)
        {
            switch (notificationTarget)
            {
                case NotificationTarget.Email:
                    if (emailNotification == null)
                        emailNotification = (EmailNotification)Create(notificationTarget);
                    break;
                case NotificationTarget.Inbox:
                    if (inboxNotification == null)
                    {
                        isNewNotification = true;
                        inboxNotification = (InboxNotification)Create(notificationTarget);
                    }
                    else isNewNotification = false;
                    break;
            }
        }

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
                    InitNotification(notificationTarget);
                    await new EmailNotificationService(serviceProvider, context).SendEmailAsync(emailNotification, targetUser);
                    break;
                default:
                case NotificationTarget.Inbox:
                    InitNotification(notificationTarget);
                    if (isNewNotification)
                        new InboxNotificationService(serviceProvider, context).SendNewNotification(inboxNotification, targetUser);
                    else new InboxNotificationService(serviceProvider, context).SendExistsNotification(inboxNotification, targetUser);
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
                    InitNotification(notificationTarget);
                    new EmailNotificationService(serviceProvider, context).SendEmail(emailNotification, targetUser);
                    break;
                default:
                case NotificationTarget.Inbox:
                    InitNotification(notificationTarget);
                    if (isNewNotification)
                        new InboxNotificationService(serviceProvider, context).SendNewNotification(inboxNotification, targetUser);
                    else new InboxNotificationService(serviceProvider, context).SendExistsNotification(inboxNotification, targetUser);
                    break;
            }
        }

        protected abstract string GetEmailTemplate();
    }
}
