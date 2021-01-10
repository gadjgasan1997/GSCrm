using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class Division : BaseDataModel
    {
        public string Name { get; set; }
        public Guid? ParentDivisionId { get; set; }
        
        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
