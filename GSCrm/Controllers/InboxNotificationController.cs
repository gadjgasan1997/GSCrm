using GSCrm.Models;
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

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(INBOX_NOT)]
    public class InboxNotificationController : MainController<InboxNotification, InboxNotificationViewModel>
    {
        /// <summary>
        /// Количество уведомлений для отображения
        /// </summary>
        private readonly int INBOX_NOTS_COUNT = 5;

        public InboxNotificationController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpGet("ListOfNotifications/{pageNumber}")]
        public IActionResult InboxNotifications(int pageNumber)
        {
            InboxNotificationsViewModel inboxNotsViewModel = cachService.GetCachedItem<InboxNotificationsViewModel>(currentUser.Id, INBOX_NOTS);
            InboxNotificationRepository inboxNotRepository = new InboxNotificationRepository(serviceProvider, context);
            inboxNotRepository.SetViewInfo(INBOX_NOTS, pageNumber, INBOX_NOTS_COUNT);
            inboxNotRepository.AttachNotifications(ref inboxNotsViewModel);
            UserNotificationsSetting userNotSetting = context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(u => u.UserId == currentUser.Id);
            inboxNotsViewModel.UserNotificationsSettingId = userNotSetting.Id.ToString();
            return View(INBOX_NOTS, inboxNotsViewModel);
        }

        [HttpGet("MakeHasReed/{id}")]
        public IActionResult MakeHasReed(string id)
        {
            new InboxNotificationRepository(serviceProvider, context).SetHasReedFlag(id, true);
            return Json("");
        }

        [HttpGet("MakeHasNoReed/{id}")]
        public IActionResult MakeHasNoReed(string id)
        {
            new InboxNotificationRepository(serviceProvider, context).SetHasReedFlag(id, false);
            return Json("");
        }
    }
}
