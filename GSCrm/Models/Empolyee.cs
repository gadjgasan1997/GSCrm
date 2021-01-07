using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GSCrm.Models
{
    public class Employee : BaseDataModel
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid? PrimaryPositionId { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; } = EmployeeStatus.None;
        public EmployeeLockReason EmployeeLockReason { get; set; } = EmployeeLockReason.None;

        public List<EmployeePosition> EmployeePositions { get; set; }
        public List<EmployeeContact> EmployeeContacts { get; set; }
        public List<AccountManager> AccountManagers { get; set; }
        public List<EmployeeResponsibility> EmployeeResponsibilities { get; set; }

        public Employee()
        {
            EmployeePositions = new List<EmployeePosition>();
            EmployeeContacts = new List<EmployeeContact>();
            AccountManagers = new List<AccountManager>();
            EmployeeResponsibilities = new List<EmployeeResponsibility>();
        }
    }

    public class EmployeeComparer : IEqualityComparer<Employee>
    {
        public bool Equals([AllowNull] Employee x, [AllowNull] Employee y) => x?.Id == y?.Id;

        public int GetHashCode([DisallowNull] Employee obj) => obj.GetFullName().GetHashCode();
    }
}
