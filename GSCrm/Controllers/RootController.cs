using System;
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
            OrganizationsViewModel orgsViewModel = cachService.GetCachedItem<OrganizationsViewModel>(currentUser.Id, ORGANIZATIONS);
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
    }
}
