using System;
using System.Collections.Generic;
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

        public List<Position> Positions { get; set; }

        public Division()
        {
            Positions = new List<Position>();
        }
    }
}
