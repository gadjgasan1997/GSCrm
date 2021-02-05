using System;
using System.Collections;
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

    public class ProductCategoryEqualityComparer : IEqualityComparer<ProductCategory>
    {
        public bool Equals(ProductCategory prodCat1, ProductCategory prodCat2) => prodCat1.Id == prodCat2.Id;

        public int GetHashCode(ProductCategory prodCat) => prodCat.Name.GetHashCode();
    }
}
