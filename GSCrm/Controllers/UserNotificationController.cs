using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using GSCrm.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(USER_NOT)]
    public class UserNotificationController : MainController<UserNotification, UserNotificationViewModel>
    {
        public UserNotificationController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpGet("HasNoPermissionsForSee")]
        public IActionResult HasNoPermissionsForSee()
            => View($"{USER_NOT_VIEWS_REL_PATH}Partial/HasNoPermissionsForSee.cshtml", new UserNotificationsSettingViewModel());

        #region HasReedSign
        [HttpGet("MakeHasReed/{id}")]
        public IActionResult MakeHasReed(string id)
        {
            new UserNotificationRepository(serviceProvider, context).SetHasReedFlag(id, true);
            return Json("");
        }

        [HttpGet("MakeHasNoReed/{id}")]
        public IActionResult MakeHasNoReed(string id)
        {
            new UserNotificationRepository(serviceProvider, context).SetHasReedFlag(id, false);
            return Json("");
        }

        [HttpGet("ReadAll")]
        public IActionResult ReadAll()
        {
            new UserNotificationRepository(serviceProvider, context).ReadAll();
            return Json("");
        }
        #endregion

        #region OrgInvite
        [HttpGet("AcceptInvite/{id}")]
        public IActionResult AcceptInvite(string id)
        {
            ModelStateDictionary modelState = ModelState;
            if (!new OrganizationRepository(serviceProvider, context).TryAcceptInvite(id, out Dictionary<string, string> errors))
            {
                AddErrorsToModel(modelState, errors);
                return BadRequest(modelState);
            }
            return Json("");
        }

        [HttpGet("RejectInvite/{notId}/{orgId}")]
        public void RejectInvite(string inboxNotId, string orgId)
            => new OrganizationRepository(serviceProvider, context).RejectInvite(orgId);
        #endregion
    }
}
