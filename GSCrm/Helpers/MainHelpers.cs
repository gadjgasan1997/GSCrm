using GSCrm.Data;
using GSCrm.Factories;
using GSCrm.Models;
using GSCrm.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.AppUtils;
using Microsoft.AspNetCore.Mvc;

namespace GSCrm.Helpers
{
    public static class MainHelpers
    {
        /// <summary>
        /// Метод осуществляет подготовку приложения
        /// </summary>
        public static void Prepare(this IApplicationBuilder app) => InitializeLocations();

        /// <summary>
        /// Метод возвращает текущего пользователя из HTTP контекста
        /// </summary>
        /// <param name="httpContext">HTTP контекст</param>
        /// <param name="context">Контекст приложения</param>
        /// <returns></returns>
        public static User GetCurrentUser(this HttpContext? httpContext, ApplicationDbContext context)
        {
            if (httpContext == null) return null;
            if (httpContext.User.Identity.IsAuthenticated)
                return context.Users.AsNoTracking().FirstOrDefault(n => n.UserName == httpContext.User.Identity.Name);
            return null;
        }

        /// <summary>
        /// Возвращает модель пользователя
        /// </summary>
        /// <param name="User"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static User GetUserModel(this ClaimsPrincipal User, ApplicationDbContext context)
            => context.Users.AsNoTracking().FirstOrDefault(n => n.UserName == User.Identity.Name);

        /// <summary>
        /// Получение названия основной сущности по названию поданной на вход
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetMainEntityName(this string entityName)
            => entityName switch
            {
                "ResponsibilityViewModel" => "OrganizationViewModel",
                "PositionViewModel" => "OrganizationViewModel",
                "EmployeeViewModel" => "OrganizationViewModel",
                "DivisionViewModel" => "OrganizationViewModel",
                "EmployeeResponsibilityViewModel" => "EmployeeViewModel",
                "EmployeePositionViewModel" => "EmployeeViewModel",
                "EmployeeContactViewModel" => "EmployeeViewModel",
                "AccountQuoteViewModel" => "AccountViewModel",
                "AccountManagerViewModel" => "AccountViewModel",
                "AccountInvoiceViewModel" => "AccountViewModel",
                "AccountContactViewModel" => "AccountViewModel",
                "AccountAddressViewModel" => "AccountViewModel",
                _ => entityName
            };

        /// <summary>
        /// Метод возвращает название действия для урла
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetEntityNameForUrl(this string entityName)
            => entityName switch
            {
                "EmployeeViewModel" => EMPLOYEE,
                "AccountViewModel" => ACCOUNT,
                "OrganizationViewModel" => ORGANIZATION,
                "PositionViewModel" => POSITION,
                "ResponsibilityViewModel" => RESPONSIBILITY,
                _ => string.Empty
            };

        /// <summary>
        /// Метод возвращает ссылку для возврата назад
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetReturnUrl(this string entityName, IUrlHelper Url)
            => entityName switch
            {
                "AccountViewModel" => Url.Action("ListOfOrganizations", ORGANIZATION, new { id = DEFAULT_MIN_PAGE_NUMBER }),
                "OrganizationViewModel" => Url.Action("ListOfOrganizations", ORGANIZATION, new { id = DEFAULT_MIN_PAGE_NUMBER }),
                _ => string.Empty
            };

        public static bool NeedCheckResps(this User user, Organization currentOrganization) => currentOrganization.OwnerId != user.Id;

        /// <summary>
        /// Метод определяет, что поданный на вход тип операции "operationType" содердится в списке "types"
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool IsInList(this OperationType operationType, params OperationType[] types) => types.Contains(operationType);
    }
}
