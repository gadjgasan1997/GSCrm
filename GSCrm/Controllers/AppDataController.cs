using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;
using static GSCrm.Repository.AccountRepository;
using GSCrm.Factories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Data.ApplicationInfo;

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
            string notsCount = cachService.GetCachedItem(currentUser.Id, "NotsCount");
            if (string.IsNullOrEmpty(notsCount))
            {
                notsCount = context.UserNotifications.AsNoTracking().Where(userNot => userNot.UserId == currentUser.Id && !userNot.HasRead).Count().ToString();
                cachService.CacheItem(currentUser.Id, "NotsCount", notsCount);
            }
            AppData appData = new AppData()
            {
                NotsCount = Convert.ToInt32(notsCount),
                ViewName = cachService.GetCurrentViewName(currentUser.Id)
            };
            return Json(appData);
        }
    }
}
