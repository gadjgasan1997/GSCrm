using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using GSCrm.Mapping.Notifications;
using GSCrm.Notifications.Auxiliary;
using GSCrm.Mapping.Notifications.EmpUpdate;
using GSCrm.Mapping.Notifications.AccUpdate;
using Microsoft.EntityFrameworkCore;
using EmpBaseUpdateNotMap = GSCrm.Mapping.Notifications.EmpUpdate.BaseUpdateNotMap;
using AccBaseUpdateNotMap = GSCrm.Mapping.Notifications.AccUpdate.BaseUpdateNotMap;
using EmpAddContactNotMap = GSCrm.Mapping.Notifications.EmpUpdate.AddContactNotMap;
using AccAddContactNotMap = GSCrm.Mapping.Notifications.AccUpdate.AddContactNotMap;
using EmpUpdateContactNotMap = GSCrm.Mapping.Notifications.EmpUpdate.UpdateContactNotMap;
using AccUpdateContactNotMap = GSCrm.Mapping.Notifications.AccUpdate.UpdateContactNotMap;
using EmpDeleteContactNotMap = GSCrm.Mapping.Notifications.EmpUpdate.DeleteContactNotMap;
using AccDeleteContactNotMap = GSCrm.Mapping.Notifications.AccUpdate.DeleteContactNotMap;

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
                            EmpUpdateType.BaseUpdate => new EmpBaseUpdateNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.ChangeDivision => new ChangeDivisionNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.AddContact => new EmpAddContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.UpdateContact => new EmpUpdateContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.DeleteContact => new EmpDeleteContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.SyncPoss => new SyncPossNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            EmpUpdateType.SyncResps => new SyncRespsNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, empUpdateType),
                            _ => default
                        };
                    }
                    else return default;

                // Изменение данных сотрудника
                case NotificationType.AccUpdate:
                    if (Enum.TryParse(inboxNot.Attrib1, out AccUpdateType accUpdateType))
                    {
                        // В зависимости от типа обновления клиента
                        return accUpdateType switch
                        {
                            AccUpdateType.BaseUpdate => new AccBaseUpdateNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.AddContact => new AccAddContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.UpdateContact => new AccUpdateContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.DeleteContact => new AccDeleteContactNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.AddAddress => new AddAddressNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.UpdateAddress => new UpdateAddressNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.DeleteAddress => new DeleteAddressNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.AddInvoice => new AddInvoiceNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.UpdateInvoice => new UpdateInvoiceNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
                            AccUpdateType.DeleteInvoice => new DeleteInvoiceNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot, accUpdateType),
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
                        NotificationType.AccTeamManagement => new AccTeamManagementNotMap(serviceProvider, context).DataToViewModel(userNot, inboxNot),
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
            => new TNotViewModel()
            {
                Id = userNot.Id,
                HasRead = userNot.HasRead,
                NotificationId = inboxNot.Id,
                NotificationSource = inboxNot.NotificationSource,
                NotificationType = inboxNot.NotificationType
            };
    }
}
