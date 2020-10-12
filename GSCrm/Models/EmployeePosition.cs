using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class EmployeePosition : BaseDataModel
    {
        [ForeignKey("Position")]
        public Guid? PositionId { get; set; }
        public Position Position { get; set; }

        [ForeignKey("Employee")]
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
