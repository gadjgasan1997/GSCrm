using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class ProductCategory : BaseDataModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentProductCategoryId { get; set; }
        public Guid? RootCategoryId { get; set; }

        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public List<Product> Products { get; set; }
        public ProductCategory()
        {
            Products = new List<Product>();
        }
    }
}
