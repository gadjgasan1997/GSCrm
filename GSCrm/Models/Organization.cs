using System.Collections.Generic;

namespace GSCrm.Models
{
    public class Organization : BaseDataModel
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }

        public List<UserOrganization> UserOrganizations { get; set; }
        public List<Division> Divisions { get; set; }
        public List<Position> Positions { get; set; }
        public List<Employee> Employees { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public Organization()
        {
            UserOrganizations = new List<UserOrganization>();
            Divisions = new List<Division>();
            Positions = new List<Position>();
            Employees = new List<Employee>();
            ProductCategories = new List<ProductCategory>();
        }
    }
}
