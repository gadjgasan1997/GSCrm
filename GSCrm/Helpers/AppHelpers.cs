using GSCrm.Data;
using GSCrm.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static GSCrm.Utils.AppUtils;

namespace GSCrm.Helpers
{
    public static class AppHelpers
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
                return context.Users.FirstOrDefault(n => n.UserName == httpContext.User.Identity.Name);
            return null;
        }

        /// <summary>
        /// Возвращает модель пользователя
        /// </summary>
        /// <param name="User"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static User GetUserModel(this ClaimsPrincipal User, ApplicationDbContext context)
            => context.Users.FirstOrDefault(n => n.UserName == User.Identity.Name);

        /// <summary>
        /// Метод добавляет в словарь новое значение или заменяет существующее
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (key == null) return;
            if (dictionary == null) dictionary = new Dictionary<TKey, TValue>();
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
            else dictionary[key] = value;
        }
    }
}
