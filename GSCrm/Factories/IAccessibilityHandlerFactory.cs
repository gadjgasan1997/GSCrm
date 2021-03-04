using GSCrm.Routing.Middleware.AccessibilityMiddleware;

namespace GSCrm.Factories
{
    /// <summary>
    /// Фабрика для получения обработчика доступности адреса, на который перешел пользователь
    /// </summary>
    public interface IAccessibilityHandlerFactory
    {
        BaseAccessibilityHandler GetAccessibilityHandler(string controllerName);
    }
}
