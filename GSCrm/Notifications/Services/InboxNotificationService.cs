using GSCrm.Data;
using GSCrm.Factories;
using GSCrm.Models;
using GSCrm.Repository;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GSCrm.Notifications.Services
{
    /// <summary>
    /// Отправка уведомлений в inbox-ы
    /// </summary>
    public class InboxNotificationService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationDbContext context;
        private readonly ITransactionFactory<Notification> transactionFactory;
        public InboxNotificationService(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            ITFFactory TFFactory = serviceProvider.GetService(typeof(ITFFactory)) as ITFFactory;
            transactionFactory = TFFactory.GetTransactionFactory<Notification>(serviceProvider, context);
        }

        /// <summary>
        /// Метод отправляет уведомление адресату
        /// </summary>
        /// <param name="notification">Сформированное уведомление</param>
        /// <param name="targetUser">Пользователь, которому его необходимо отправить</param>
        public void SendExistsNotification(InboxNotification notification, User targetUser) => Send(notification, targetUser, false);

        /// <summary>
        /// Метод отправляет уведомление адресату
        /// </summary>
        /// <param name="notification">Сформированное уведомление</param>
        /// <param name="targetUser">Пользователь, которому его необходимо отправить</param>
        public void SendNewNotification(InboxNotification notification, User targetUser) => Send(notification, targetUser, true);

        /// <summary>
        /// Метод отсылает уведомления, добавляя его в список изменений транзакции в зависимости от флага "isNewNotification"
        /// </summary>
        /// <param name="notification">Сформированное уведомление</param>
        /// <param name="targetUser">Пользователь, которому его необходимо отправить</param>
        /// <param name="isNewNotification"></param>
        private void Send(InboxNotification notification, User targetUser, bool isNewNotification)
        {
            ITransaction transaction = transactionFactory.Create(targetUser.Id.ToString(), OperationType.SendNotification, notification);
            if (isNewNotification)
                transaction.AddChange(notification, EntityState.Added);
            transaction.AddChange(new UserNotification()
            {
                NotificationId = notification.Id,
                UserId = targetUser.Id.ToString()
            }, EntityState.Added);

            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (transactionFactory.TryCommit(transaction, errors))
            {
                transactionFactory.Close(transaction);
                new UserNotificationRepository(serviceProvider, context).OnUserNotAdded(targetUser);
            }
            else transactionFactory.Close(transaction, TransactionStatus.Error);
        }
    }
}
