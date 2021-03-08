using GSCrm.Helpers;
using GSCrm.Models.Enums;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class EmployeeResponsibilityAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "GetResponsibilities":
                case "NextAllRecords":
                case "PreviousAllRecords":
                case "NextSelectedRecords":
                case "PreviousSelectedRecords":
                case "ClearResponsibilityManagementSearch":
                case "ClearAllResponsibilitiesSearch":
                case "ClearSelectedResponsibilitiesSearch":
                    accessibilityHandlerData.TryCacheCurrentEmployee(RequestSourceType.RouteValues, "employeeId");
                    break;
                case "SearchAllResponsibilities":
                case "SearchSelectedResponsibilities":
                    accessibilityHandlerData.TryCacheCurrentEmployee();
                    break;
                case "Synchronize":
                    accessibilityHandlerData.CacheCurrentOrganization();
                    accessibilityHandlerData.TryCacheCurrentEmployee();
                    break;
                default:
                    break;
            }
        }
    }
}
