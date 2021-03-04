using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(USER_NOTS_SETTING)]
    public class UserNotificationsSettingController : MainController<UserNotificationsSetting, UserNotificationsSettingViewModel>
    {
        public UserNotificationsSettingController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpPost("CommitSettings")]
        public IActionResult CommitUserSettings(AllNotificationsSettingsViewModel allSettingsViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            UserNotificationsSettingViewModel userNotViewModel = allSettingsViewModel.UserNotificationsSettingViewModel;
            if (!new UserNotificationsSettingRepository(serviceProvider, context).TryUpdate(ref userNotViewModel, modelState))
                return BadRequest(modelState);
            return Json("");
        }

        [HttpGet("SetSettingsToDefault")]
        public IActionResult SetUserNotSettingsToDefault()
        {
            ModelStateDictionary modelState = ModelState;
            if (!new UserNotificationsSettingRepository(serviceProvider, context).SetNotSettingsToDefault(modelState))
                return BadRequest(modelState);
            return Json("");
        }
    }
}
