using GSCrm.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GSCrm.Models
{
    public class Position : BaseDataModel
    {
        public string Name { get; set; }
        public Guid? DivisionId { get; set; }
        public PositionStatus PositionStatus { get; set; } = PositionStatus.Active;
        public PositionLockReason PositionLockReason { get; set; } = PositionLockReason.None;
        public Guid? ParentPositionId { get; set; }
        public Guid? PrimaryEmployeeId { get; set; }

        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public List<EmployeePosition> EmployeePositions { get; set; }

        public Position()
        {
            EmployeePositions = new List<EmployeePosition>();
        }
    }

    public class PositionEqualityComparer : IEqualityComparer<Position>
    {
        public bool Equals([AllowNull] Position x, [AllowNull] Position y) => x.Name == y.Name;

        public int GetHashCode([DisallowNull] Position obj) => obj.Name.GetHashCode();
    }
}
