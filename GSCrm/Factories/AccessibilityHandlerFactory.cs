using GSCrm.Routing.Middleware.AccessibilityMiddleware;
using GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers;

namespace GSCrm.Factories
{
    public class AccessibilityHandlerFactory : IAccessibilityHandlerFactory
    {
        public BaseAccessibilityHandler GetAccessibilityHandler(string controllerName)
            => controllerName switch
            {
                "Organization" => new OrganizationAccessibilityHandler(),
                "Position" => new PositionAccessibilityHandler(),
                "Employee" => new EmployeeAccessibilityHandler(),
                "Responsibility" => new ResponsibilityAccessibilityHandler(),
                "ProductCategory" => new ProductCategoryAccessibilityHandler(),
                "Account" => new AccountAccessibilityHandler(),
                "AccountInvoice" => new AccountInvoiceAccessibilityHandler(),
                "AccountContact" => new AccountContactAccessibilityHandler(),
                "AccountAddress" => new AccountAddressAccessibilityHandler(),
                "Root" => new RootAccessibilityHandler(),
                "EmployeeContact" => new EmployeeContactAccessibilityHandler(),
                "Division" => new DivisionAccessibilityHandler(),
                "EmployeePosition" => new EmployeePositionAccessibilityHandler(),
                "EmployeeResponsibility" => new EmployeeResponsibilityAccessibilityHandler(),
                "AccountManager" => new AccountManagerAccessibilityHandler(),
                _ => null
            };
    }
}
