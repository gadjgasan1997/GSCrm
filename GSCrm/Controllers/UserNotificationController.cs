﻿using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using GSCrm.Data;
using static GSCrm.CommonConsts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(USER_NOT)]
    public class UserNotificationController : MainController<UserNotification, UserNotificationViewModel>
    {
        public UserNotificationController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpGet("ListOfNotifications/{pageNumber}")]
        public IActionResult UserNotifications(int pageNumber)
        {
            UserNotificationsViewModel userNotsViewModel = cachService.GetCachedItem<UserNotificationsViewModel>(currentUser.Id, USER_NOTS);
            UserNotificationRepository userNotRepository = new UserNotificationRepository(serviceProvider, context);
            userNotRepository.SetViewInfo(USER_NOTS, pageNumber);
            userNotRepository.AttachNotifications(ref userNotsViewModel);
            UserNotificationsSetting userNotSetting = context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(u => u.UserId == currentUser.Id);
            userNotsViewModel.UserNotificationsSettingId = userNotSetting.Id.ToString();
            return View(USER_NOTS, userNotsViewModel);
        }

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
