using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Notifications.Factories
{
    /// <summary>
    /// Фабрика для рассылки уведомлений, внутри организации
    /// </summary>
    public abstract class OrgNotificationFactory<TNotificationParams> : NotificationFactory<TNotificationParams>
        where TNotificationParams : INotificationParams
    {
        protected OrgNotificationFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TNotificationParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        /// <summary>
        /// Метод отправляет уведомление адресатам внутри опредлеленной организации
        /// </summary>
        /// <param name="organizationId">Id организации, внутри которой происходит рассылка</param>
        /// <param name="targetEmployees">Список сотрудников, которым необходимо отправить уведомление</param>
        /// <returns></returns>
        public void Send(Guid organizationId, List<Employee> targetEmployees)
        {
            foreach (Guid targetuserId in targetEmployees.Select(emp => emp.UserId))
            {
                // Получение организации, в которой состоит пользователь
                UserOrganization userOrganization = context.UserOrganizations.AsNoTracking()
                    .Include(u => u.User)
                    .Include(not => not.OrgNotificationsSetting)
                    .FirstOrDefault(i => i.UserId == targetuserId.ToString() && i.OrganizationId == organizationId);

                // Если требуется рассылка
                if (userOrganization?.OrgNotificationsSetting != null && NeedNotification(userOrganization.OrgNotificationsSetting))
                {
                    // Для всех способов рассыкли, доступных для этого типа уведомления
                    GetNotificationTargets(userOrganization.OrgNotificationsSetting).ForEach(async notificationTarget =>
                        await SendAsync(userOrganization.User, notificationTarget));
                }
            }
        }

        /// <summary>
        /// Метод отсылает уведомление пользователю, беря настройки уведомлений из кеша транзакции
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="userId"></param>
        public void Send(ITransaction transaction, Guid userId)
        {
            User targetUser = context.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId.ToString());
            OrgNotificationsSetting orgNotSetting = (OrgNotificationsSetting)transaction.GetParameterValue("OrgNotificationsSetting");
            if (orgNotSetting != null && NeedNotification(orgNotSetting))
            {
                GetNotificationTargets(orgNotSetting).ForEach(async notificationTarget =>
                    await SendAsync(targetUser, notificationTarget));
            }
        }

        /// <summary>
        /// Метод возвращает типы рассылок в зависимости от типа уведомления
        /// </summary>
        /// <param name="orgNotSetting"></param>
        /// <returns></returns>
        protected abstract List<NotificationTarget> GetNotificationTargets(OrgNotificationsSetting orgNotSetting);

        /// <summary>
        /// Метод возвращает признак, надо ли отправлять уведомление пользователю
        /// </summary>
        /// <param name="orgNotSetting"></param>
        /// <returns></returns>
        protected abstract bool NeedNotification(OrgNotificationsSetting orgNotSetting);
    }
}
