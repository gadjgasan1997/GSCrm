using System;
using GSCrm.Data;
using GSCrm.Factories;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Notifications.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace GSCrm.Notifications.Factories
{
    public abstract class NotificationFactory<TNotificationParams>
        where TNotificationParams : INotificationParams
    {
        #region Declarations
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
        #endregion

        protected NotificationFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TNotificationParams notificationParams)
        {
            // Вспомогательные сервисы
            IUserContextFactory userContextServices = serviceProvider.GetService<IUserContextFactory>();
            IActionContextAccessor actionAccessor = serviceProvider.GetService<IActionContextAccessor>();
            IUrlHelperFactory urlHelperFactory = serviceProvider.GetService<IUrlHelperFactory>();

            // ActionContext может быть null, если этот конструткор будет вызываться из компонентов MiddleWare
            httpContext = userContextServices.HttpContext;
            if (actionAccessor.ActionContext != null)
                urlHelper = urlHelperFactory.GetUrlHelper(actionAccessor.ActionContext);

            // Прочее
            resManager = serviceProvider.GetService<IResManager>();
            configuration = serviceProvider.GetService<IConfiguration>();
            this.serviceProvider = serviceProvider;
            this.context = context;
            this.notificationParams = notificationParams;

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
