using System;
using GSCrm.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware
{
    /// <summary>
    /// Данные, необходиммые для обработчиков доступности адреса
    /// </summary>
    public class AccessibilityHandlerData
    {
        /// <summary>
        /// Параметры запроса
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }
        /// <summary>
        /// Присланная форма запроса
        /// </summary>
        public IFormCollection Form { get; set; }
        /// <summary>
        /// Название вызываемого действия
        /// </summary>
        public string ActionName { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public ApplicationDbContext Context { get; set; }
        public HttpContext HttpContext { get; set; }
        /// <summary>
        /// Необходимо ли прервать выполнение запроса
        /// </summary>
        public bool NeedBreakRequest { get; set; }
        /// <summary>
        /// Обработчик, вызываемый при прерывании запроса
        /// </summary>
        public Action ErrorHandler { get; set; }
    }
}
