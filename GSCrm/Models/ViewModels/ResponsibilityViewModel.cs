using System;

namespace GSCrm.Models.ViewModels
{
    public class ResponsibilityViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        #region Organization
        public bool OrgDelete { get; set; }
        #endregion

        #region Division
        public bool DivCreate { get; set; }
        public bool DivDelete { get; set; }
        #endregion

        #region Position
        public bool PosCreate { get; set; }
        public bool PosDelete { get; set; }
        public bool PosUpdate { get; set; }
        public bool PosChangeDiv { get; set; }
        public bool PosUnlock { get; set; }
        #endregion

        #region Employee
        public bool EmpCreate { get; set; }
        public bool EmpDelete { get; set; }
        public bool EmpUpdate { get; set; }
        public bool EmpChangeDiv { get; set; }
        public bool EmpUnlock { get; set; }
        public bool EmpPossManagement { get; set; }
        public bool EmpRespsManagement { get; set; }
        public bool EmpContactCreate { get; set; }
        public bool EmpContactUpdate { get; set; }
        public bool EmpContactDelete { get; set; }
        #endregion

        #region Responsibility
        public bool RespCreate { get; set; }
        public bool RespUpdate { get; set; }
        public bool RespDelete { get; set; }
        #endregion

        #region Account
        public bool AccCreate { get; set; }
        public bool AccUpdate { get; set; }
        public bool AccDelete { get; set; }
        public bool AccChangeType { get; set; }
        public bool AccUnlock { get; set; }
        public bool AccTeamManagement { get; set; }
        public bool AccContactCreate { get; set; }
        public bool AccContactUpdate { get; set; }
        public bool AccContactDelete { get; set; }
        public bool AccAddressCreate { get; set; }
        public bool AccAddressUpdate { get; set; }
        public bool AccAddressDelete { get; set; }
        public bool AccInvoiceCreate { get; set; }
        public bool AccInvoiceUpdate { get; set; }
        public bool AccInvoiceDelete { get; set; }
        #endregion
    }
}
