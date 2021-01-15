using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Mapping
{
    public class ProductCategoryMap : BaseMap<ProductCategory, ProductCategoryViewModel>
    {
        public ProductCategoryMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override ProductCategoryViewModel DataToViewModel(ProductCategory productCategory)
            => new ProductCategoryViewModel()
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                Description = productCategory.Description,
                ParentProductCategoryId = productCategory.ParentProductCategoryId
            };
    }
}
