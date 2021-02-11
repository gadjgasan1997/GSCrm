using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route("")]
    public class RootController :  Controller
    {
        #region Declarations
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationDbContext context;
        private readonly ICachService cachService;
        private readonly User currentUser;
        #endregion

        public RootController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;
            cachService = serviceProvider.GetService(typeof(ICachService)) as ICachService;
            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
        }

        /// <summary>
        /// Получение списка всех организаций
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Organizations/{pageNumber}")]
        public ViewResult Organizations(int pageNumber)
        {
            if (!cachService.TryGetEntityCache(currentUser, out OrganizationsViewModel orgsViewModel, ORGANIZATIONS))
                orgsViewModel = new OrganizationsViewModel();
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(ORGANIZATIONS, pageNumber);
            organizationRepository.AttachOrganizations(ref orgsViewModel);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATIONS}.cshtml", orgsViewModel);
        }

        /// <summary>
        /// Получение списка всех клиентов
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("AllAccounts/{pageNumber}")]
        public ViewResult AllAccounts(int pageNumber)
        {
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            AccountsViewModel accountsViewModel = new AccountsViewModel();
            new AccountMap(serviceProvider, context).InitializeAccountsViewModel(accountsViewModel);
            accountRepository.SetViewInfo(ALL_ACCS, pageNumber);
            accountRepository.AttachAccounts(ref accountsViewModel);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNTS}.cshtml", accountsViewModel);
        }

        /// <summary>
        /// Получение списка клиентов основной организации пользователя
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("CurrentAccounts/{pageNumber}")]
        public ViewResult CurrentAccounts(int pageNumber)
        {
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            AccountsViewModel accountsViewModel = new AccountsViewModel();
            new AccountMap(serviceProvider, context).InitializeAccountsViewModel(accountsViewModel);
            accountRepository.SetViewInfo(CURRENT_ACCS, pageNumber);
            accountRepository.AttachAccounts(ref accountsViewModel);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNTS}.cshtml", accountsViewModel);
        }

        /// <summary>
        /// Получение списка уведомлений пользователя
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Notifications/{pageNumber}")]
        public IActionResult UserNotifications(int pageNumber)
        {
            if (!cachService.TryGetEntityCache(currentUser, out UserNotificationsViewModel userNotsViewModel, USER_NOTS))
                userNotsViewModel = new UserNotificationsViewModel();
            UserNotificationRepository userNotRepository = new UserNotificationRepository(serviceProvider, context);
            userNotRepository.SetViewInfo(USER_NOTS, pageNumber);
            userNotRepository.AttachNotifications(ref userNotsViewModel);
            UserNotificationsSetting userNotSetting = context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(u => u.UserId == currentUser.Id);
            userNotsViewModel.UserNotificationsSettingId = userNotSetting.Id.ToString();
            return View($"{USER_NOT_VIEWS_REL_PATH}{USER_NOTS}.cshtml", userNotsViewModel);
        }

        /// <summary>
        /// Получение настроек уведомлений
        /// </summary
        /// <returns></returns>
        [HttpGet("NotificationsSetting")]
        public ViewResult NotificationsSetting()
        {
            if (cachService.TryGetEntityCache(currentUser, out UserNotificationsSetting userNotificationsSetting))
            {
                AllNotificationsSettingsViewModel allSettingsViewModel = new AllNotificationsSettingsViewModel();
                UserNotificationsSettingMap notificationsSettingMap = new UserNotificationsSettingMap(serviceProvider, context);
                allSettingsViewModel.UserNotificationsSettingViewModel = notificationsSettingMap.DataToViewModel(userNotificationsSetting);
                OrgNotificationsSettingRepository orgNotSettingRepository = new OrgNotificationsSettingRepository(serviceProvider, context);
                orgNotSettingRepository.AttachSettings(ref allSettingsViewModel);
                return View($"{NOT_SETTING_VIEWS_REL_PATH}{NOT_SETTINGS}.cshtml", allSettingsViewModel);
            }
            return View("Error");
        }
    }
}
