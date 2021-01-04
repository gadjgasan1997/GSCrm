using System;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Notifications;
using GSCrm.Transactions;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Mapping
{
    public class OrgNotificationsSettingMap : BaseMap<OrgNotificationsSetting, OrgNotificationsSettingViewModel>
    {
        public OrgNotificationsSettingMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override OrgNotificationsSettingViewModel DataToViewModel(OrgNotificationsSetting notificationsSetting)
        {
            UserOrganization userOrganization = context.UserOrganizations.AsNoTracking().FirstOrDefault(i => i.Id == notificationsSetting.UserOrganizationId);
            Organization organization = context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == userOrganization.OrganizationId);
            return new OrgNotificationsSettingViewModel()
            {
                Id = notificationsSetting.Id,
                UserOrganization = notificationsSetting.UserOrganization,
                UserOrganizationId = notificationsSetting.UserOrganizationId,
                OrganizationName = organization?.Name,
                DivDeleteNot = notificationsSetting.DivDeleteNot,
                TDivDeleteNot = notificationsSetting.TDivDeleteNot,
                PosDeleteNot = notificationsSetting.PosDeleteNot,
                TPosDeleteNot = notificationsSetting.TPosDeleteNot,
                PosUpdateNot = notificationsSetting.PosUpdateNot,
                TPosUpdateNot = notificationsSetting.TPosUpdateNot,
                EmpDelete = notificationsSetting.EmpDeleteNot,
                TEmpDelete = notificationsSetting.TEmpDeleteNot,
                EmpUpdate = notificationsSetting.EmpUpdateNot,
                TEmpUpdate = notificationsSetting.TEmpUpdateNot,
                AccDelete = notificationsSetting.AccDeleteNot,
                TAccDelete = notificationsSetting.TAccDeleteNot,
                AccUpdate = notificationsSetting.AccUpdateNot,
                TAccUpdate = notificationsSetting.TAccUpdateNot,
                AccTeamManagement = notificationsSetting.AccTeamManagementNot,
                TAccTeamManagement = notificationsSetting.TAccTeamManagementNot
            };
        }

        public override OrgNotificationsSetting OnModelUpdate(OrgNotificationsSettingViewModel settingViewModel)
        {
            OrgNotificationsSetting notificationsSetting = base.OnModelUpdate(settingViewModel);
            notificationsSetting.AccDeleteNot = settingViewModel.AccDelete;
            notificationsSetting.TAccDeleteNot = settingViewModel.TAccDelete;
            notificationsSetting.AccUpdateNot = settingViewModel.AccUpdate;
            notificationsSetting.TAccUpdateNot = settingViewModel.TAccUpdate;
            notificationsSetting.AccTeamManagementNot = settingViewModel.AccTeamManagement;
            notificationsSetting.TAccTeamManagementNot = settingViewModel.TAccTeamManagement;
            notificationsSetting.DivDeleteNot = settingViewModel.DivDeleteNot;
            notificationsSetting.TDivDeleteNot = settingViewModel.TDivDeleteNot;
            notificationsSetting.PosDeleteNot = settingViewModel.PosDeleteNot;
            notificationsSetting.TPosDeleteNot = settingViewModel.TPosDeleteNot;
            notificationsSetting.PosUpdateNot = settingViewModel.PosUpdateNot;
            notificationsSetting.TPosUpdateNot = settingViewModel.TPosUpdateNot;
            notificationsSetting.EmpDeleteNot = settingViewModel.EmpDelete;
            notificationsSetting.TEmpDeleteNot = settingViewModel.TEmpDelete;
            notificationsSetting.EmpUpdateNot = settingViewModel.EmpUpdate;
            notificationsSetting.TEmpUpdateNot = settingViewModel.TEmpUpdate;
            return notificationsSetting;
        }

        /// <summary>
        /// Метод инициализирует настройки уведомлений по умолчанию
        /// </summary>
        /// <param name="orgNotSetting"></param>
        public OrgNotificationsSetting InitNotSetting(OrgNotificationsSetting orgNotSetting)
        {
            orgNotSetting.DivDeleteNot = true;
            orgNotSetting.TDivDeleteNot = NotificationTarget.Inbox;
            orgNotSetting.PosDeleteNot = true;
            orgNotSetting.TPosDeleteNot = NotificationTarget.Inbox;
            orgNotSetting.PosUpdateNot = false;
            orgNotSetting.TPosUpdateNot = NotificationTarget.Inbox;
            orgNotSetting.EmpDeleteNot = true;
            orgNotSetting.TEmpDeleteNot = NotificationTarget.Inbox;
            orgNotSetting.EmpUpdateNot = false;
            orgNotSetting.TEmpUpdateNot = NotificationTarget.Inbox;
            orgNotSetting.AccDeleteNot = true;
            orgNotSetting.TAccDeleteNot = NotificationTarget.Inbox;
            orgNotSetting.AccUpdateNot = false;
            orgNotSetting.TAccUpdateNot = NotificationTarget.Inbox;
            orgNotSetting.AccTeamManagementNot = true;
            orgNotSetting.TAccTeamManagementNot = NotificationTarget.Inbox;
            return orgNotSetting;
        }
    }
}
