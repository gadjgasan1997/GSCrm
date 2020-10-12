using System.Collections.Generic;

namespace GSCrm.Models
{
    public class Organization : BaseDataModel
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }

        public List<UserOrganization> UserOrganizations { get; set; }
        public List<Division> Divisions { get; set; }
        public Organization()
        {
            UserOrganizations = new List<UserOrganization>();
            Divisions = new List<Division>();
        }
    }
}
