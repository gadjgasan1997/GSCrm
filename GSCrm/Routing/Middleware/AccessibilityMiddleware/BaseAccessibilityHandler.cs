using GSCrm.Localization;

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
        /// <param name="accessibilityHandlerData">Данные, необходимые для обработчиков</param>
        public abstract void Handle(AccessibilityHandlerData accessibilityHandlerData);

        /// <summary>
        /// Метод возвращает объект с ошибкой в случае, если запись не найдена
        /// </summary>
        /// <param name="errorCode">Код ошибки</param>
        /// <param name="resManager"></param>
        /// <returns></returns>
        protected object GetRecordNotFoundMessage(string errorCode, IResManager resManager)
            => errorCode switch
            {
                "EmployeeNotFound" => new
                {
                    RecordNotFound = resManager.GetString("EmployeeNotFound")
                },
                "EmployeeContactNotFound" => new
                {
                    RecordNotFound = resManager.GetString("EmployeeContactNotFound")
                },
                "OrganizationNotFound" => new
                {
                    RecordNotFound = resManager.GetString("OrganizationNotFound")
                },
                "ProductCategoryNotFound" => new
                {
                    RecordNotFound = resManager.GetString("ProductCategoryNotFound")
                },
                "AccountNotFound" => new
                {
                    RecordNotFound = resManager.GetString("AccountContactNotFound")
                },
                "AccountContactNotFound" => new
                {
                    RecordNotFound = resManager.GetString("AccountContactNotFound")
                },
                "AccountAddressNotFound" => new
                {
                    RecordNotFound = resManager.GetString("AccountAddressNotFound")
                },
                "AccountInvoiceNotFound" => new
                {
                    RecordNotFound = resManager.GetString("AccountInvoiceNotFound")
                },
                _ => null
            };

        protected object GetUnhandledExceptionMessage(string errorCode, IResManager resManager)
            => errorCode switch
            {
                "ProductCategoriesUnhandledException" => new
                {
                    UnhandledException = resManager.GetString("ProductCategoriesUnhandledException")
                },
                _ => new
                {
                    UnhandledException = resManager.GetString("UnhandledException")
                },
            };
    }
}
