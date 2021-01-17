using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using System;

namespace GSCrm.Mapping
{
    public class ProductMap : BaseMap<Product, ProductViewModel>
    {
        public ProductMap(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public override ProductViewModel DataToViewModel(Product product)
            => new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Cost = product.Cost,
                Description = product.Description
            };
    }
}
