using GSCrm.Helpers;
using GSCrm.Models.Enums;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class EmployeePositionAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "GetPositions":
                case "NextAllRecords":
                case "PreviousAllRecords":
                case "NextSelectedRecords":
                case "PreviousSelectedRecords":
                case "ClearPositionManagementSearch":
                case "ClearAllPositionSearch":
                case "ClearSelectedPositionSearch":
                    accessibilityHandlerData.CacheCurrentEmployee(RequestSourceType.RouteValues, "employeeId");
                    break;
                case "SearchAllPosition":
                case "SearchSelectedPosition":
                    accessibilityHandlerData.CacheCurrentEmployee();
                    break;
                case "Synchronize":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.CacheCurrentEmployee();
                    break;
                default:
                    break;
            }
        }
    }
}
