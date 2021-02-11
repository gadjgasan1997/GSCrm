using GSCrm.Models;
using GSCrm.Routing.Middleware.AccessibilityMiddleware;

namespace GSCrm.Helpers
{
    public static class MiddlewareHelpers
    {
        /// <summary>
        /// Метод перенаправляет запрос на другой url
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <param name="accessibilityHandlerData"></param>
        public static void Redirect(this AccessibilityHandlerData accessibilityHandlerData, string redirectUrl)
        {
            accessibilityHandlerData.NeedBreakRequest = true;
            accessibilityHandlerData.HttpContext.Response.Redirect(redirectUrl);
        }

        public static User GetCurrentUser(this AccessibilityHandlerData accessibilityHandlerData)
            => accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);
    }
}
