using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class EmployeeContact : Contact
    {
        [ForeignKey("Empolyee")]
        public Guid EmployeeId { get; set; }
        public Employee Empolyee { get; set; }
    }
}
