using GSCrm.Notifications;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class OrgNotificationsSetting : BaseDataModel
    {
        [ForeignKey("UserOrganization")]
        public Guid UserOrganizationId { get; set; }
        public UserOrganization UserOrganization { get; set; }

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
        public bool EmpDeleteNot { get; set; }
        public NotificationTarget TEmpDeleteNot { get; set; }
        public bool EmpUpdateNot { get; set; }
        public NotificationTarget TEmpUpdateNot { get; set; }
        #endregion

        #region Account
        public bool AccDeleteNot { get; set; }
        public NotificationTarget TAccDeleteNot { get; set; }
        public bool AccUpdateNot { get; set; }
        public NotificationTarget TAccUpdateNot { get; set; }
        public bool AccTeamManagementNot { get; set; }
        public NotificationTarget TAccTeamManagementNot { get; set; }
        #endregion
    }
}
