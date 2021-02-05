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
                ParentProductCategoryId = productCategory.ParentProductCategoryId.ToString()
            };

        public override ProductCategory OnModelCreate(ProductCategoryViewModel prodCatViewModel)
        {
            base.OnModelCreate(prodCatViewModel);
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            ProductCategory parentProductCategory = (ProductCategory)transaction.GetParameterValue("ParentProductCategory");
            return new ProductCategory()
            {
                Id = Guid.NewGuid(),
                OrganizationId = currentOrganization.Id,
                Name = prodCatViewModel.Name,
                Description = prodCatViewModel.Description,
                ParentProductCategoryId = parentProductCategory?.Id,
                RootCategoryId = parentProductCategory?.RootCategoryId ?? parentProductCategory?.Id
            };
        }

        public override ProductCategory OnModelUpdate(ProductCategoryViewModel prodCatViewModel)
        {
            ProductCategory productCategory = base.OnModelUpdate(prodCatViewModel);
            productCategory.Name = prodCatViewModel.Name;
            productCategory.Description = prodCatViewModel.Description;
            return productCategory;
        }
    }
}
