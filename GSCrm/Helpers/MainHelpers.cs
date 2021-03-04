using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using GSCrm.Models.Enums;
using GSCrm.Data.ApplicationInfo;
using static GSCrm.CommonConsts;
using static GSCrm.Utils.AppUtils;

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
                "AccountViewModel" => Url.Action(ACCOUNTS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER }),
                "OrganizationViewModel" => Url.Action(ORGANIZATIONS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER }),
                _ => string.Empty
            };

        public static bool NeedCheckResps(this User user, Organization currentOrganization) => currentOrganization.OwnerId != user.Id;

        /// <summary>
        /// Метод определяет, что поданный на вход тип операции "operationType" содерджится в списке "types"
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool IsInList(this OperationType operationType, params OperationType[] types) => types.Contains(operationType);

        /// <summary>
        /// Метод задействует перенаправление на страницы в зависимостпи от кодов ошибок
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UseStatusCodePagesRedirect(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseStatusCodePages(async codeContext => {
                switch (codeContext.HttpContext.Response.StatusCode)
                {
                    case 400:
                        codeContext.HttpContext.Response.Redirect($"/Shared/Error");
                        break;
                    case 404:
                        codeContext.HttpContext.Response.Redirect($"/Shared/ViewNotFound");
                        break;
                    default:
                        break;
                }
            });

        /// <summary>
        /// Метод возвращает новый номер страницы исходя из направления прокрутки
        /// </summary>
        /// <param name="viewInfo"></param>
        /// <param name="navigateDirection"></param>
        /// <returns></returns>
        public static int GetNewPageNumber(this ViewInfo viewInfo, NavigateDirection navigateDirection)
            => navigateDirection switch
            {
                NavigateDirection.Forward => viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP,
                NavigateDirection.Backward => viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP,
                _ => 0
            };

        /// <summary>
        /// Метод возвращает форму из запроса
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static IFormCollection GetForm(this HttpContext httpContext)
        {
            if (httpContext.Request.Method != "POST")
                return null;
            try
            {
                return httpContext.Request.Form;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
