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
        public InboxNotificationController(ApplicationDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        { }

        [HttpGet("ListOfNotifications/{pageNumber}")]
        public IActionResult InboxNotifications(int pageNumber)
        {
            InboxNotificationsViewModel inboxNotsViewModel = cachService.GetCachedItem<InboxNotificationsViewModel>(currentUser.Id, INBOX_NOTS);
            InboxNotificationRepository inboxNotRepository = new InboxNotificationRepository(serviceProvider, context);
            inboxNotRepository.SetViewInfo(currentUser.Id, INBOX_NOTS, pageNumber);
            inboxNotRepository.AttachNotifications(ref inboxNotsViewModel);
            UserNotificationsSetting userNotSetting = context.UserNotificationsSettings.AsNoTracking().FirstOrDefault(u => u.UserId == currentUser.Id);
            inboxNotsViewModel.UserNotificationsSettingId = userNotSetting.Id.ToString();
            return View(INBOX_NOTS, inboxNotsViewModel);
        }
    }
}
