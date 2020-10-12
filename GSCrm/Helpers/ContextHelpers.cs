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
        /// <summary>
        /// Метод возвращает список всех организаций, в которых состоит текущий пользователь
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static List<Organization> GetOrganizations(this ApplicationDbContext context, User currentUser = null)
        {
            if (currentUser == null) return context.Organizations.ToList();
            Func<Organization, bool> predicate = org => org.UserOrganizations.Select(i => i.UserId).ToList().Contains(currentUser.Id);
            return context.Organizations.Include(orgs => orgs.UserOrganizations).Where(predicate).ToList();
        }

        /// <summary>
        /// Получает список подразделений с должностями
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="divisionName"></param>
        /// <returns></returns>
        public static List<Division> GetOrgDivisions(this ApplicationDbContext context, Guid organizationId)
            => context.Divisions.Include(pos => pos.Positions).Where(orgId => orgId.OrganizationId == organizationId).ToList();

        /// <summary>
        /// Получает список должностей организации
        /// </summary>
        /// <param name="context"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static List<Position> GetOrgPositions(this ApplicationDbContext context, Guid organizationId)
        {
            List<Position> orgPositions = new List<Position>();
            List<Division> orgDivisions = GetOrgDivisions(context, organizationId);
            orgDivisions.ForEach(orgDivision => orgPositions.AddRange(orgDivision.Positions));
            return orgPositions;
        }

        /// <summary>
        /// Получает список сотрудников организации
        /// </summary>
        /// <param name="context"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static List<Employee> GetOrgEmployees(this ApplicationDbContext context, Guid organizationId)
        {
            List<Employee> divisionsEmployees = new List<Employee>();
            GetOrgDivisions(context, organizationId).ForEach(division => divisionsEmployees.AddRange(division.GetEmployees(context)));
            return divisionsEmployees;
        }

        /// <summary>
        /// Метод возвращает список всех клиентов организации по типу
        /// </summary>
        /// <param name="context"></param>
        /// <param name="organizationId">Id организации</param>
        /// <param name="accountType">Тип клиента</param>
        /// <returns></returns>
        public static List<Account> GetAccountsByType(this ApplicationDbContext context, Guid organizationId, AccountType accountType)
            => context.Accounts.Where(acc => acc.OrganizationId == organizationId && acc.AccountType == accountType).ToList();

        /// <summary>
        /// Методы возвращают разные списки с клиентами
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<Account> GetAllAccounts(this ApplicationDbContext context, User currentUser)
        {
            List<Account> accounts = new List<Account>();
            List<UserOrganization> userOrganizations = context.UserOrganizations.Where(userId => userId.UserId == currentUser.Id).ToList();
            userOrganizations.ForEach(userOrganization => accounts.AddRange(context.GetOrgAccounts(userOrganization.OrganizationId)));
            return accounts;
        }

        public static List<Account> GetCurrentAccounts(this ApplicationDbContext context, User currentUser)
            => context.GetOrgAccounts(currentUser.PrimaryOrganizationId);

        public static List<Account> GetOrgAccounts(this ApplicationDbContext context, Guid organizationId)
            => context.Accounts.Where(acc => acc.OrganizationId == organizationId).ToList();

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
            => context.Quotes.Where(orgId => orgId.OrganizationId == organizationId).ToList();
    }
}
