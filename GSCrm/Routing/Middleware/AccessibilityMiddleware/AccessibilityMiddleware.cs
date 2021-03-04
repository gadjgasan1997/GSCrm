using System;
using GSCrm.Data;
using GSCrm.Factories;
using GSCrm.Helpers;
using GSCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware
{
    /// <summary>
    /// Компонент осуществялет проверку доступности адреса, на который перешел пользователь
    /// </summary>
    public class AccessibilityMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessibilityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            // Получение текущего пользователя из контекста
            User currentUser = httpContext.GetCurrentUser(context);
            if (currentUser != null)
            {
                // Получение название контроллера и действия из параметров запроса
                RouteValueDictionary routeValues = httpContext.Request.RouteValues;
                if (routeValues.TryGetValue("controller", out object controllerName) &&
                    routeValues.TryGetValue("action", out object actionName))
                {
                    // В зависимости от того, на какой контроллер был осуществлен запрос
                    IAccessibilityHandlerFactory accessibilityHandlerFactory = serviceProvider.GetService<IAccessibilityHandlerFactory>();
                    BaseAccessibilityHandler accessibilityHandler = accessibilityHandlerFactory.GetAccessibilityHandler(controllerName.ToString());
                    if (accessibilityHandler != null)
                    {
                        AccessibilityHandlerData accessibilityHandlerData = new AccessibilityHandlerData()
                        {
                            Form = httpContext.GetForm(),
                            ActionName = actionName.ToString(),
                            Context = context,
                            HttpContext = httpContext,
                            RouteValues = routeValues,
                            ServiceProvider = serviceProvider
                        };
                        accessibilityHandler.Handle(accessibilityHandlerData);
                        if (accessibilityHandlerData.NeedBreakRequest)
                        {
                            accessibilityHandlerData.ErrorHandler?.Invoke();
                            return;
                        }
                    }
                }

                // Проставление кода "Не найдено"
                else
                {
                    httpContext.Response.StatusCode = 404;
                    return;
                }
            }

            if (!httpContext.Response.HasStarted)
                await _next(httpContext);
        }
    }

    public static class AccessibilityMiddlewareExtensions
    {
        /// <summary>
        /// Применяет при обработке запроса компонент <see cref="AccessibilityMiddleware"/>
        /// </summary>
        public static IApplicationBuilder UseAccessibilityMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<AccessibilityMiddleware>();
    }
}
