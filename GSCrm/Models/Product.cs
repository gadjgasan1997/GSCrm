using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSCrm.Models
{
    public class Product : BaseDataModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        [ForeignKey("ProductCategory")]
        public Guid ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
