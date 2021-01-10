using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using GSCrm.Mapping.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Mapping.Notifications.EmpUpdate;

namespace GSCrm.Mapping
{
    public class UserNotificationMap : BaseMap<UserNotification, UserNotificationViewModel>
    {
        public UserNotificationMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override sealed UserNotificationViewModel DataToViewModel(UserNotification userNot)
        {
            InboxNotification inboxNot = context.InboxNotifications.AsNoTracking().FirstOrDefault(i => i.Id == userNot.NotificationId);
            switch (inboxNot.NotificationType)
            {
                // Если тип уведомления - изменение данных сотрудника
                case NotificationType.EmpUpdate:
                    if (Enum.TryParse(inboxNot.Attrib1, out EmpUpdateType empUpdateType))
                    {
                        // В зависимости от типа обновления сотрудника
                        return empUpdateType switch
                        {
                            EmpUpdateType.BaseUpdate => new BaseUpdateNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.ChangeDivision => new ChangeDivisionNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.AddContact => new AddContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.UpdateContact => new UpdateContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.DeleteContact => new DeleteContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.SyncPoss => new SyncPossNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.SyncResps => new SyncRespsNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            _ => default
                        };
                    }
                    else return default;

                // Иначе в зависимости от типа уведомления
                default:
                    return inboxNot.NotificationType switch
                    {
                        NotificationType.OrgInvite => new OrgInviteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                        NotificationType.DivDelete => new DivDeleteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                        NotificationType.PosDelete => new PosDeleteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                        NotificationType.PosUpdate => new PosUpdateNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                        NotificationType.EmpDelete => new EmpDeleteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                        NotificationType.AccDelete => new AccDeleteNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
                        _ => default
                    };
            }
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
                NotificationType = inboxNot.NotificationType
            };
            return userNotViewModel;
        }
    }
}
