using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using GSCrm.Data;
using static GSCrm.CommonConsts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GSCrm.Mapping;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(NOT_SETTING)]
    public class NotificationsSettingController : MainController<OrgNotificationsSetting, OrgNotificationsSettingViewModel>
    {
        public NotificationsSettingController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpGet("NotificationsSetting/{id}")]
        public ViewResult NotificationsSetting(string id)
        {
            UserNotificationsSettingRepository userNotSettingRepository = new UserNotificationsSettingRepository(serviceProvider, context);
            if (userNotSettingRepository.TryGetItemById(id, out UserNotificationsSetting userNotificationsSetting))
            {
                // Заполнение модели с данными настроек уведомления для пользователя
                AllNotificationsSettingsViewModel allSettingsViewModel = cachService.GetCachedItem<AllNotificationsSettingsViewModel>(currentUser.Id, NOT_SETTINGS);
                allSettingsViewModel.UserNotificationsSettingViewModel = new UserNotificationsSettingMap(serviceProvider, context).DataToViewModel(userNotificationsSetting);

                // Прикрепление списка моделей с настройками уведомлений для организаций, в которых состоит пользователь
                OrgNotificationsSettingRepository orgNotSettingRepository = new OrgNotificationsSettingRepository(serviceProvider, context);
                orgNotSettingRepository.AttachSettings(ref allSettingsViewModel);
                return View(NOT_SETTINGS, allSettingsViewModel);
            }
            return View("Error");
        }

        [HttpGet("OrgNotificationsSettings/{pageNumber}")]
        public ViewResult OrgNotificationsSettings(int pageNumber)
        {
            AllNotificationsSettingsViewModel allSettingsViewModel = cachService.GetCachedItem<AllNotificationsSettingsViewModel>(currentUser.Id, NOT_SETTINGS);
            OrgNotificationsSettingRepository orgNotSettingRepository = new OrgNotificationsSettingRepository(serviceProvider, context);
            orgNotSettingRepository.SetViewInfo(currentUser.Id, NOT_SETTINGS, pageNumber);
            orgNotSettingRepository.AttachSettings(ref allSettingsViewModel);
            return View(NOT_SETTINGS, allSettingsViewModel);
        }

        [HttpPost("CommitOrgSettings")]
        public IActionResult CommitOrgSettings(AllNotificationsSettingsViewModel allSettingsViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrgNotificationsSettingRepository(serviceProvider, context).TryCommitOrgSettings(allSettingsViewModel, modelState))
                return BadRequest(modelState);
            return Json("");
        }

        [HttpPost("CommitUserSettings")]
        public IActionResult CommitUserSettings(AllNotificationsSettingsViewModel allSettingsViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            UserNotificationsSettingViewModel userNotViewModel = allSettingsViewModel.UserNotificationsSettingViewModel;
            if (!new UserNotificationsSettingRepository(serviceProvider, context).TryUpdate(ref userNotViewModel, modelState))
                return BadRequest(modelState);
            return Json("");
        }

        [HttpGet("SetOrgNotSettingsToDefault")]
        public IActionResult SetOrgNotSettingsToDefault()
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrgNotificationsSettingRepository(serviceProvider, context).SetNotSettingsToDefault(modelState))
                return BadRequest(modelState);
            return Json("");
        }

        [HttpGet("SetUserNotSettingsToDefault")]
        public IActionResult SetUserNotSettingsToDefault()
        {
            ModelStateDictionary modelState = ModelState;
            if (!new UserNotificationsSettingRepository(serviceProvider, context).SetNotSettingsToDefault(modelState))
                return BadRequest(modelState);
            return Json("");
        }
    }
}
