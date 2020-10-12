using System;
using System.Collections.Generic;

namespace GSCrm.Models
{
    public class Responsibility : BaseDataModel
    {
        public string Name { get; set; }
        public Guid OrganizationId { get; set; }
        public bool OrgDelete { get; set; }
        public bool DivCreate { get; set; }
        public bool DivDelete { get; set; }
        public bool PosCreate { get; set; }
        public bool PosDelete { get; set; }
        public bool PosUpdate { get; set; }
        public bool PosChangeDiv { get; set; }
        public bool EmpCreate { get; set; }
        public bool EmpDelete { get; set; }
        public bool EmpUpdate { get; set; }
        public bool EmpChangeDiv { get; set; }
        public bool EmpPosDelete { get; set; }
        public bool EmpPossManagement { get; set; }
        public bool EmpContactCreate { get; set; }
        public bool EmpContactDelete { get; set; }
        public bool AccCreate { get; set; }
        public bool AccUpdate { get; set; }
        public bool AccDelete { get; set; }
        public bool AccChangeType { get; set; }
        public bool AccTeamManagement { get; set; }
        public bool AccContactCreate { get; set; }
        public bool AccContactUpdate { get; set; }
        public bool AccContactDelete { get; set; }
        public bool AccContactChangePrimary { get; set; }
        public bool AccAddressCreate { get; set; }
        public bool AccAddressUpdate { get; set; }
        public bool AccAddressDelete { get; set; }
        public bool AccInvoiceCreate { get; set; }
        public bool AccInvoiceUpdate { get; set; }
        public bool AccInvoiceDelete { get; set; }

        public List<EmployeeResponsibility> EmployeeResponsibilities { get; set; }
        public Responsibility()
        {
            EmployeeResponsibilities = new List<EmployeeResponsibility>();
        }
    }
}
