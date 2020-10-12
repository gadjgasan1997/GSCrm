using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class EmployeeResponsibility : BaseDataModel
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public Employee User { get; set; }

        [ForeignKey("Responsibility")]
        public Guid ResponsibilityId { get; set; }
        public Responsibility Responsibility { get; set; }
    }
}
