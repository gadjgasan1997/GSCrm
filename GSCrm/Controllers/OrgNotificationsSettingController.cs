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
    [Route(ORG_NOTS_SETTING)]
    public class OrgNotificationsSettingController : MainController<OrgNotificationsSetting, OrgNotificationsSettingViewModel>
    {
        public OrgNotificationsSettingController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpPost("CommitSettings")]
        public IActionResult CommitSettings(AllNotificationsSettingsViewModel allSettingsViewModel)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrgNotificationsSettingRepository(serviceProvider, context).TryCommitOrgSettings(allSettingsViewModel, modelState))
                return BadRequest(modelState);
            return Json("");
        }

        [HttpGet("SetSettingsToDefault")]
        public IActionResult SetSettingsToDefault()
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrgNotificationsSettingRepository(serviceProvider, context).SetNotSettingsToDefault(modelState))
                return BadRequest(modelState);
            return Json("");
        }
    }
}
