using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Transactions.Factories
{
    public class UserNotificationTF : TransactionFactory<UserNotificationViewModel>
    {
        public UserNotificationTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context) { }

        protected override void BeforeCommit(OperationType operationType)
        {
            if (operationType == OperationType.Delete)
            {
                // Необходимо удалить уведомление из "Notifications" в случае, если не осталось пользователей, которые его не удалили из "UserNotifications"
                UserNotification userNot = (UserNotification)transaction.GetParameterValue("RecordToRemove");
                Func<UserNotification, bool> predicate = not => not.NotificationId == userNot.NotificationId && userNot.Id != userNot.Id;
                List<UserNotification> userNotifications = context.UserNotifications.AsNoTracking().Where(predicate).ToList();
                if (userNotifications.Count == 0)
                {
                    InboxNotification inboxNot = context.InboxNotifications.AsNoTracking().FirstOrDefault(not => not.Id == userNot.NotificationId);
                    transaction.AddChange(inboxNot, EntityState.Deleted);
                }
            }
        }

        protected override void CloseHandler(TransactionStatus transactionStatus, OperationType operationType)
        {
            if (operationType == OperationType.Delete && transactionStatus == TransactionStatus.Success)
            {
                UserNotification userNot = (UserNotification)transaction.GetParameterValue("RecordToRemove");
                new UserNotificationRepository(serviceProvider, context).OnUserNotRemoved(userNot);
            }
        }
    }
}
