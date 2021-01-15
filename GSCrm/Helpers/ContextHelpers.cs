using GSCrm.Data;
using GSCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class ContextHelpers
    {
        #region Organization
        public static List<Organization> GetOrganizations(this ApplicationDbContext context, User currentUser)
        {
            Func<Organization, bool> predicate = org => org.UserOrganizations.Where(userOrg => userOrg.Accepted).Select(userOrg => userOrg.UserId).ToList().Contains(currentUser.Id);
            return context.Organizations.AsNoTracking().Include(orgs => orgs.UserOrganizations).Where(predicate).ToList();
        }

        public static List<Division> GetOrgDivisions(this ApplicationDbContext context, Guid organizationId)
            => context.Divisions.AsNoTracking().Where(orgId => orgId.OrganizationId == organizationId).ToList();

        public static List<Position> GetOrgPositions(this ApplicationDbContext context, Guid organizationId)
        {
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == organizationId);
            return organization.GetPositions(context);
        }

        public static List<Employee> GetOrgEmployees(this ApplicationDbContext context, Guid organizationId)
        {
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == organizationId);
            return organization.GetEmployees(context);
        }

        public static List<ProductCategory> GetOrgProdCats(this ApplicationDbContext context, Guid organizationId)
        {
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == organizationId);
            return organization.GetProductCategories(context);
        }
        #endregion

        #region Accounts
        /// <summary>
        /// Метод возвращает список всех клиентов организации по типу
        /// </summary>
        /// <param name="context"></param>
        /// <param name="organizationId">Id организации</param>
        /// <param name="accountType">Тип клиента</param>
        /// <returns></returns>
        public static List<Account> GetAccountsByType(this ApplicationDbContext context, Guid organizationId, AccountType accountType)
            => context.Accounts.AsNoTracking().Where(acc => acc.OrganizationId == organizationId && acc.AccountType == accountType).ToList();

        /// <summary>
        /// Методы возвращают разные списки с клиентами
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<Account> GetAllAccounts(this ApplicationDbContext context, User currentUser)
        {
            List<Account> accounts = new List<Account>();
            Func<UserOrganization, bool> predicate = userOrg => userOrg.UserId == currentUser.Id && userOrg.Accepted;
            List<UserOrganization> userOrganizations = context.UserOrganizations.AsNoTracking().Where(predicate).ToList();
            userOrganizations.ForEach(userOrganization => accounts.AddRange(context.GetOrgAccounts(userOrganization.OrganizationId)));
            return accounts;
        }

        public static List<Account> GetCurrentAccounts(this ApplicationDbContext context, User currentUser)
            => context.GetOrgAccounts(currentUser.PrimaryOrganizationId);

        public static List<Account> GetOrgAccounts(this ApplicationDbContext context, Guid organizationId)
            => context.Accounts.AsNoTracking().Where(acc => acc.OrganizationId == organizationId).ToList();
        #endregion

        #region Quotes
        /// <summary>
        /// Методы возвращают разные списки со сделками
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<Quote> GetAllQuotes(this ApplicationDbContext context, User currentUser)
        {
            List<Quote> quotes = new List<Quote>();
            List<UserOrganization> userOrganizations = context.UserOrganizations.Where(userId => userId.UserId == currentUser.Id).ToList();
            userOrganizations.ForEach(userOrganization => quotes.AddRange(context.GetOrgQuotes(userOrganization.OrganizationId)));
            return quotes;
        }

        public static List<Quote> GetCurrentQuotes(this ApplicationDbContext context, User currentUser)
            => context.GetOrgQuotes(currentUser.PrimaryOrganizationId);

        public static List<Quote> GetOrgQuotes(this ApplicationDbContext context, Guid organizationId)
            => context.Quotes.AsNoTracking().Where(orgId => orgId.OrganizationId == organizationId).ToList();
        #endregion

        #region Notification
        /// <summary>
        /// Метод возвращает список всех уведомлений, адресованных пользователю
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<UserNotification> GetUserNotifications(this ApplicationDbContext context, User currentUser)
            => context.UserNotifications.AsNoTracking().Where(userNot => userNot.UserId == currentUser.Id).ToList();

        /// <summary>
        /// Метод возвращает список всех уведомлений, адресованных пользователю с притягиванием моделей "Notification"
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<UserNotification> GetUserNotificationsExt(this ApplicationDbContext context, User currentUser)
            => context.UserNotifications.Include(userNot => userNot.Notification).AsNoTracking().Where(userNot => userNot.UserId == currentUser.Id).ToList();

        public static List<InboxNotification> GetInboxNotifications(this ApplicationDbContext context, User currentUser)
        {
            Func<InboxNotification, bool> predicate = not => not.UserNotifications.Select(i => i.UserId).ToList().Contains(currentUser.Id);
            return context.InboxNotifications.AsNoTracking().Include(not => not.UserNotifications).Where(predicate).ToList();
        }

        /// <summary>
        /// Метод возвращает список всех настроек уведомлений пользователя во всех организациях
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<OrgNotificationsSetting> GetNotificationsSettings(this ApplicationDbContext context, User currentUser)
            => context.UserOrganizations
                .AsNoTracking().Include(userOrg => userOrg.OrgNotificationsSetting)
                .Where(userOrg => userOrg.UserId == currentUser.Id && userOrg.Accepted)
                .Select(userOrg => userOrg.OrgNotificationsSetting).ToList();

        /// <summary>
        /// Метод возвращает список всех настроек уведомлений пользователя
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<UserNotificationsSetting> GetUserNotificationsSettings(this ApplicationDbContext context, User currentUser)
            => context.UserNotificationsSettings.AsNoTracking().Where(i => i.UserId == currentUser.Id).ToList();
        #endregion

        #region Employee
        /// <summary>
        /// Метод возвращает текущего сотрудника по id Организации и id пользователя
        /// </summary>
        /// <param name="context"></param>
        /// <param name="organizationId">Id организации</param>
        /// <param name="currentUserId">Id текущего пользователя</param>
        /// <returns></returns>
        public static Employee GetCurrentEmployee(this ApplicationDbContext context, Guid organizationId, Guid currentUserId)
        {
            if (organizationId == null || currentUserId == null) return null;
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == organizationId);
            return GetCurrentEmployee(context, organization, currentUserId);
        }

        public static Employee GetCurrentEmployee(this ApplicationDbContext context, Organization organization, Guid currentUserId)
        {
            if (organization == null || currentUserId == null) return null;
            return context.Employees.AsNoTracking().FirstOrDefault(emp => emp.OrganizationId == organization.Id && emp.UserId == currentUserId);
        }
        #endregion
    }
}
