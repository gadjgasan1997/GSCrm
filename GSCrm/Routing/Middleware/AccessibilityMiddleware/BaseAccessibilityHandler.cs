namespace GSCrm.Routing.Middleware.AccessibilityMiddleware
{
    /// <summary>
    /// Базовый класс для обработчиков доступности адреса, на который перешел пользователь
    /// </summary>
    public abstract class BaseAccessibilityHandler
    {
        /// <summary>
        /// Обработчик доступности
        /// </summary>
        /// <param name="accessibilityHandler">Данные, необходимые для обработчиков</param>
        public abstract void Handle(AccessibilityHandlerData accessibilityHandler);
    }
}
