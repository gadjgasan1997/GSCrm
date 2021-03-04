using GSCrm.Helpers;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class DivisionAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            if (accessibilityHandlerData.ActionName == "Create")
                accessibilityHandlerData.CacheCurrentOrganization();
        }
    }
}
