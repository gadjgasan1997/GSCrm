﻿using GSCrm.Data;
using GSCrm.Models;
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
        public OrgNotificationFactory(IServiceProvider serviceProvider, ApplicationDbContext context, TNotificationParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        /// <summary>
        /// Метод отправляет уведомление адресатам внутри опредлеленной организации
        /// </summary>
        /// <param name="organizationId">Id организации, внутри которой происходит рассылка</param>
        /// <param name="targetUserIdList">Список пользователей, которым необходимо отправить уведомление</param>
        public void Send(Guid organizationId, IEnumerable<Guid> targetUserIdList)
        {
            foreach (Guid targetuserId in targetUserIdList)
            {
                // Получение организации, в которой состоит пользователь
                UserOrganization userOrganization = context.UserOrganizations.AsNoTracking()
                    .Include(u => u.User)
                    .Include(not => not.OrgNotificationsSetting)
                    .FirstOrDefault(i => i.UserId == targetuserId.ToString() && i.OrganizationId == organizationId);

                // Если требуется раассылка
                if (userOrganization?.OrgNotificationsSetting != null && NeedNotification(userOrganization.OrgNotificationsSetting))
                {
                    // Для всех способов рассыкли, доступных для этого типа уведомления
                    GetNotificationTargets(userOrganization.OrgNotificationsSetting).ForEach(async notificationTarget =>
                        await SendAsync(userOrganization.User, notificationTarget));
                }
            }
        }

        /// <summary>
        /// Метод отправляет уведомление адресатам внутри опредлеленной организации
        /// </summary>
        /// <param name="organizationId">Id организации, внутри которой происходит рассылка</param>
        /// <param name="targetEmployees">Список сотрудников, которым необходимо отправить уведомление</param>
        /// <returns></returns>
        public void Send(Guid organizationId, List<Employee> targetEmployees)
        {
            foreach (Employee employee in targetEmployees)
                Send(organizationId, new List<Guid>() { employee.UserId });
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
