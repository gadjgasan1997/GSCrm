using GSCrm.Helpers;
using GSCrm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Data.ApplicationInfo;
using static GSCrm.CommonConsts;
using GSCrm.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace GSCrm.Controllers
{
    [Route(APP_DATA)]
    public class AppDataController : Controller
    {
        #region Declarations
        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;
        /// <summary>
        /// Кеш сервис
        /// </summary>
        private readonly ICachService cachService;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private readonly User currentUser;
        #endregion

        public AppDataController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            cachService = serviceProvider.GetService(typeof(ICachService)) as ICachService;
            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
        }

        [HttpGet("InitalizeAppData")]
        public IActionResult InitalizeAppData()
        {
            if (currentUser == null) return Json(new AppData());
            if (cachService.TryGetValue(currentUser, "NotsCount", out int notsCount))
            {
                if (notsCount == 0)
                {
                    notsCount = context.UserNotifications.AsNoTracking().Where(userNot => userNot.UserId == currentUser.Id && !userNot.HasRead).Count();
                    cachService.AddOrUpdate(currentUser, "NotsCount", notsCount);
                }
            }
            else
            {
                notsCount = context.UserNotifications.AsNoTracking().Where(userNot => userNot.UserId == currentUser.Id && !userNot.HasRead).Count();
                cachService.AddOrUpdate(currentUser, "NotsCount", notsCount);
            }
            AppData appData = new AppData()
            {
                NotsCount = Convert.ToInt32(notsCount),
                ViewInfo = cachService.GetCurrentViewInfo(currentUser.Id)
            };
            return Json(appData);
        }

        [HttpGet("GetTestInfo")]
        public IActionResult GetTestInfo()
        {
            Dictionary<string, MemoryCache> cashItems = cachService.GetCashItems();
            Dictionary<string, Dictionary<string, ViewInfo>> cashViews = cachService.GetCashViews();
            cashItems.TryGetValue(currentUser.Id, out MemoryCache memoryCache);
            memoryCache.TryGetValue("CurrentOrganization", out object org);
            memoryCache.TryGetValue("CurrentPosition", out object pos);
            memoryCache.TryGetValue("CurrentEmployee", out object emp);
            memoryCache.TryGetValue("CurrentResponsibility", out object resp);
            memoryCache.TryGetValue("CurrentEmployeeContact", out object empCont);
            memoryCache.TryGetValue("CurrentProductCategory", out object prodCat);
            Dictionary<string, object> result = new Dictionary<string, object>()
            {
                { "CacheData._cashItems", cashItems },
                { "CacheData._cashViews", cashViews },
                { "CacheData.CurrentOrganization", org },
                { "CacheData.CurrentPosition", pos },
                { "CacheData.CurrentEmployee", emp },
                { "CacheData.CurrentResponsibility", resp },
                { "CacheData.CurrentEmployeeContact", empCont },
                { "CacheData.CurrentProductCategory", prodCat },
            };
            return Json(result);
        }
    }
}
