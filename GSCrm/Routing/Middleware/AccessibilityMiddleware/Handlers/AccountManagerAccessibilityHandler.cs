using GSCrm.Helpers;
using GSCrm.Models.Enums;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountManagerAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            switch (accessibilityHandlerData.ActionName)
            {
                case "GetManagers":
                case "NextAllRecords":
                case "PreviousAllRecords":
                case "ClearAccTeamManagementSearch":
                case "ClearAllManagersSearch":
                case "ClearSelectedManagersSearch":
                    accessibilityHandlerData.TryCacheCurrentAccount(RequestSourceType.RouteValues, "accountId");
                    break;
                case "SearchAllManagers":
                case "SearchSelectedManagers":
                    accessibilityHandlerData.TryCacheCurrentAccount();
                    break;
                case "Synchronize":
                    accessibilityHandlerData.TryCacheCurrentAccount();
                    break;
                default:
                    break;
            }
        }
    }
}
