﻿using GSCrm.Data;
using GSCrm.Data.Cash;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using static GSCrm.Helpers.MainHelpers;
using GSCrm.Factories;

namespace GSCrm.Controllers
{
    public class MainController<TDataModel, TViewModel> : Controller
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        #region Declarations
        /// <summary>
        /// Контекст приложения
        /// </summary>
        protected readonly ApplicationDbContext context;
        protected readonly IServiceProvider serviceProvider;
        /// <summary>
        /// Информация о представлении
        /// </summary>
        protected readonly IViewsInfo viewsInfo;
        /// <summary>
        /// Кеш сервис
        /// </summary>
        protected readonly ICachService cachService;
        /// <summary>
        /// Менеджер ресурсов
        /// </summary>
        protected readonly IResManager resManager;
        /// <summary>
        /// Преобразователь для маппинга
        /// </summary>
        protected IMap<TDataModel, TViewModel> map;
        /// <summary>
        /// Репозиторий
        /// </summary>
        protected IRepository<TDataModel, TViewModel> repository;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        protected readonly User currentUser;
        #endregion

        #region Constructs
        public MainController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.context = context;

            IMapFactory mapFactory = serviceProvider.GetService(typeof(IMapFactory)) as IMapFactory;
            IRepositoryFactory repositoryFactory = serviceProvider.GetService(typeof(IRepositoryFactory)) as IRepositoryFactory;
            map = mapFactory.GetMap<TDataModel, TViewModel>(serviceProvider, context);
            repository = repositoryFactory.GetRepository<TDataModel, TViewModel>(serviceProvider, context);
            viewsInfo = serviceProvider.GetService(typeof(IViewsInfo)) as IViewsInfo;
            cachService = serviceProvider.GetService(typeof(ICachService)) as ICachService;
            resManager = serviceProvider.GetService(typeof(IResManager)) as IResManager;

            IUserContextFactory userContextServices = serviceProvider.GetService(typeof(IUserContextFactory)) as IUserContextFactory;
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
        }
        #endregion

        [HttpPost("Create")]
        public virtual IActionResult Create(TViewModel viewModel)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryCreate(ref viewModel, modelState, currentUser))
                {
                    string entityName = typeof(TViewModel).Name.GetEntityNameForUrl();
                    return Json(Url.Action(entityName, entityName, new { id = repository.NewRecord.Id.ToString() }));
                }
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public virtual IActionResult Update(TViewModel viewModel)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryUpdate(ref viewModel, modelState, currentUser))
                {
                    string entityName = typeof(TViewModel).Name.GetEntityNameForUrl();
                    return Json(Url.Action(entityName, entityName, new { id = repository.ChangedRecord.Id.ToString() }));
                }
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("Delete")]
        public virtual IActionResult Delete(string id)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryDelete(id, modelState, currentUser))
                    return Json(typeof(TViewModel).Name.GetReturnUrl(Url));
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
