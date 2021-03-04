﻿using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using GSCrm.Repository;
using GSCrm.Localization;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using static GSCrm.CommonConsts;
using static GSCrm.Helpers.MainHelpers;

namespace GSCrm.Controllers
{
    public class MainController<TDataModel, TViewModel> : Controller
        where TDataModel : BaseDataModel, new()
        where TViewModel : BaseViewModel, new()
    {
        #region Declarations
        protected readonly ApplicationDbContext context;
        protected readonly IServiceProvider serviceProvider;
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
        protected readonly IMap<TDataModel, TViewModel> map;
        /// <summary>
        /// Репозиторий
        /// </summary>
        protected readonly IRepository<TDataModel, TViewModel> repository;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        protected readonly User currentUser;
        /// <summary>
        /// Настройки сериализации
        /// </summary>
        protected readonly JsonSerializerSettings serializerSettings =
            new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        #endregion

        #region Constructs
        public MainController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            // Вспомогательные сервисы
            IMapFactory mapFactory = serviceProvider.GetService<IMapFactory>();
            IRepositoryFactory repositoryFactory = serviceProvider.GetService<IRepositoryFactory>();
            IUserContextFactory userContextServices = serviceProvider.GetService<IUserContextFactory>();

            // Прочее
            this.serviceProvider = serviceProvider;
            this.context = context;
            map = mapFactory.GetMap<TDataModel, TViewModel>(serviceProvider, context);
            repository = repositoryFactory.GetRepository<TDataModel, TViewModel>(serviceProvider, context);
            cachService = serviceProvider.GetService<ICachService>();
            resManager = serviceProvider.GetService<IResManager>();
            currentUser = userContextServices.HttpContext.GetCurrentUser(context);
        }
        #endregion

        [HttpGet("HasNoPermissionsForSee")]
        public ViewResult HasNoPermissionsForSee()
            => typeof(TViewModel).Name switch
            {
                "OrganizationViewModel" => View($"{ORG_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new OrganizationViewModel()),
                "PositionViewModel" => View($"{POS_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new PositionViewModel()),
                "EmployeeViewModel" => View($"{EMP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new EmployeeViewModel()),
                "ResponsibilityViewModel" => View($"{RESP_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new ResponsibilityViewModel()),
                "AccountViewModel" => View($"{ACC_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new AccountViewModel()),
                "UserNotificationsSettingViewModel" => View($"{USER_NOT_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new UserNotificationsSettingViewModel()),
                _ => View("Error")
            };

        [HttpPost("Create")]
        public IActionResult Create(TViewModel viewModel)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryCreate(ref viewModel, modelState, currentUser))
                    return CreateSuccessHandler();
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        protected virtual IActionResult CreateSuccessHandler()
        {
            string entityName = typeof(TViewModel).Name.GetEntityNameForUrl();
            return Json(Url.Action(entityName, entityName, new { id = repository.NewRecord.Id.ToString() }));
        }

        [HttpPost("Update")]
        public IActionResult Update(TViewModel viewModel)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryUpdate(ref viewModel, modelState, currentUser))
                    return UpdateSuccessHandler();
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        protected virtual IActionResult UpdateSuccessHandler()
        {
            string entityName = typeof(TViewModel).Name.GetEntityNameForUrl();
            return Json(Url.Action(entityName, entityName, new { id = repository.ChangedRecord.Id.ToString() }));
        }

        [HttpDelete("Delete")]
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                ModelStateDictionary modelState = ModelState;
                if (repository.TryDelete(id, modelState, currentUser))
                    return DeleteSuccessHandler();
                return BadRequest(modelState);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        protected virtual IActionResult DeleteSuccessHandler()
            => Json(typeof(TViewModel).Name.GetReturnUrl(Url));

        /// <summary>
        /// Добавляет ошибки в модель
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="errors"></param>
        protected virtual void AddErrorsToModel(ModelStateDictionary modelState, Dictionary<string, string> errors)
        {
            foreach (KeyValuePair<string, string> error in errors)
                modelState.AddModelError(error.Key, error.Value);
        }
    }
}
