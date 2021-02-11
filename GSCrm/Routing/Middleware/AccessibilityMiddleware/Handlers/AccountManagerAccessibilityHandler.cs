using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Repository;
using GSCrm.Data.Cash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;

namespace GSCrm.Routing.Middleware.AccessibilityMiddleware.Handlers
{
    public class AccountManagerAccessibilityHandler : BaseAccessibilityHandler
    {
        public override void Handle(AccessibilityHandlerData accessibilityHandlerData)
        {
            // Неактивно, так как все запросы к команде по клиенту берут закешированного при заходе на карточку клиента
            /*switch (actionName)
            {
                case "InitializeAccTeam":
                case "NextAllRecords":
                case "PreviousAllRecords":
                    if (routeValues.TryGetValue("id", out object id))
                    {
                        ICachService cachService = serviceProvider.GetService(typeof(ICachService)) as ICachService;

                        // Проверка наличия клиента и доступа у пользователя к нему
                        AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
                    }
                    break;
                default:
                    break;
            }*/
        }
    }
}
