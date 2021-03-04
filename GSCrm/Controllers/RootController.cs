using System;
using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route("")]
    public class RootController : Controller
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
            cachService = serviceProvider.GetService<ICachService>();
            IUserContextFactory userContextServices = serviceProvider.GetService<IUserContextFactory>();
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
        }

        #region Organizations
        /// <summary>
        /// Получение списка всех организаций
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Organizations/{pageNumber}")]
        public ViewResult Organizations(int pageNumber)
        {
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            organizationRepository.SetViewInfo(ORGANIZATIONS, pageNumber);
            return View($"{ORG_VIEWS_REL_PATH}{ORGANIZATIONS}.cshtml", organizationRepository.LoadOrganizationsView());
        }

        [HttpPost("SearchOrganizations")]
        public IActionResult SearchOrganizations(OrganizationsViewModel orgViewModels)
        {
            new OrganizationRepository(serviceProvider, context).Search(orgViewModels);
            return RedirectToAction(ORGANIZATIONS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearOrganizationsSearch")]
        public IActionResult ClearOrganizationsSearch()
        {
            new OrganizationRepository(serviceProvider, context).ClearSearch();
            return RedirectToAction(ORGANIZATIONS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }
        #endregion

        #region Accounts
        /// <summary>
        /// Получение списка всех клиентов
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("AllAccounts/{pageNumber}")]
        public ViewResult AllAccounts(int pageNumber)
        {
            AccountRepository accountRepository = new AccountRepository(serviceProvider, context);
            accountRepository.SetViewInfo(ALL_ACCS, pageNumber);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNTS}.cshtml", accountRepository.LoadView());
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
            accountRepository.SetViewInfo(CURRENT_ACCS, pageNumber);
            return View($"{ACC_VIEWS_REL_PATH}{ACCOUNTS}.cshtml", accountRepository.LoadView());
        }

        [HttpPost("SearchAllAccounts")]
        public IActionResult SearchAllAccounts(AccountsViewModel accountsViewModel)
        {
            new AccountRepository(serviceProvider, context).SearchAllAccounts(accountsViewModel);
            return RedirectToAction(ALL_ACCS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearAllAccountsSearch")]
        public IActionResult ClearAllAccountsSearch()
        {
            new AccountRepository(serviceProvider, context).ClearAllAccountsSearch();
            return RedirectToAction(ALL_ACCS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpPost("SearchCurrentAccounts")]
        public IActionResult SearchCurrentAccounts(AccountsViewModel accountsViewModel)
        {
            new AccountRepository(serviceProvider, context).SearchCurrentAccounts(accountsViewModel);
            return RedirectToAction(CURRENT_ACCS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearCurrentAccountsSearch")]
        public IActionResult ClearCurrentAccountsSearch()
        {
            new AccountRepository(serviceProvider, context).ClearCurrentAccountsSearch();
            return RedirectToAction(CURRENT_ACCS, "Root", new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }
        #endregion

        #region Notifications
        /// <summary>
        /// Получение списка уведомлений пользователя
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("Notifications/{pageNumber}")]
        public IActionResult UserNotifications(int pageNumber)
        {
            UserNotificationRepository userNotRepository = new UserNotificationRepository(serviceProvider, context);
            userNotRepository.SetViewInfo(USER_NOTS, pageNumber);
            return View($"{USER_NOT_VIEWS_REL_PATH}{USER_NOTS}.cshtml", userNotRepository.LoadNotificationsView());
        }

        /// <summary>
        /// Получение настроек уведомлений
        /// </summary
        /// <returns></returns>
        [HttpGet("NotificationsSettings")]
        public ViewResult NotificationsSettings() => GetNotificationsSettings();

        /// <summary>
        /// Получение настроек уведомлений от организаций
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("OrgsNotificationsSettings/{pageNumber}")]
        public ViewResult OrgsNotificationsSettings(int pageNumber)
        {
            OrgNotificationsSettingRepository orgNotSettingRepository = new OrgNotificationsSettingRepository(serviceProvider, context);
            orgNotSettingRepository.SetViewInfo(ORGS_NOTS_SETTINGS, pageNumber);
            return GetNotificationsSettings();
        }
        #endregion

        #region Addtion Methods
        private ViewResult GetNotificationsSettings()
        {
            AllNotificationsSettingRepository allNotSettingRepository = new AllNotificationsSettingRepository(serviceProvider, context);
            return View($"{NOT_SETTING_VIEWS_REL_PATH}{NOTS_SETTINGS}.cshtml", allNotSettingRepository.LoadView());
        }
        #endregion
    }
}
