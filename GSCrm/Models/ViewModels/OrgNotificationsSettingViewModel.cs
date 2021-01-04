using GSCrm.Notifications;
using System;

namespace GSCrm.Models.ViewModels
{
    public class OrgNotificationsSettingViewModel : BaseViewModel
    {
        public Guid UserOrganizationId { get; set; }
        public UserOrganization UserOrganization { get; set; }
        public string OrganizationName { get; set; }

        #region Division
        public bool DivDeleteNot { get; set; }
        public NotificationTarget TDivDeleteNot { get; set; }
        #endregion

        #region Position
        public bool PosDeleteNot { get; set; }
        public NotificationTarget TPosDeleteNot { get; set; }
        public bool PosUpdateNot { get; set; }
        public NotificationTarget TPosUpdateNot { get; set; }
        #endregion

        #region Employee
        public bool EmpDelete { get; set; }
        public NotificationTarget TEmpDelete { get; set; }
        public bool EmpUpdate { get; set; }
        public NotificationTarget TEmpUpdate { get; set; }
        #endregion

        #region Account
        public bool AccDelete { get; set; }
        public NotificationTarget TAccDelete { get; set; }
        public bool AccUpdate { get; set; }
        public NotificationTarget TAccUpdate { get; set; }
        public bool AccTeamManagement { get; set; }
        public NotificationTarget TAccTeamManagement { get; set; }
        #endregion
    }
}
