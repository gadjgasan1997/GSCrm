using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models.ViewModels;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class ProductRepository : BaseRepository<Product, ProductViewModel>
    {
        public ProductRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public List<Product> GetLimitedProductList(List<Product> products, ProductCategoriesViewModel productCategoriesViewModel)
        {
            List<Product> limitedProducts = products;
            LimitByName(ref products, productCategoriesViewModel);
            LimitByCost(ref products, productCategoriesViewModel);
            return limitedProducts;
        }

        private void LimitByName(ref List<Product> products, ProductCategoriesViewModel productCategoriesViewModel)
        {
            string productName = productCategoriesViewModel.SearchProductCategoryName.ToLower().TrimStartAndEnd();
            if (!string.IsNullOrEmpty(productName))
                products = products.Where(prod => prod.Name.ToLower().Contains(productName)).ToList();
        }

        private void LimitByCost(ref List<Product> products, ProductCategoriesViewModel productCategoriesViewModel)
        {
            decimal.TryParse(productCategoriesViewModel.MinConst, out decimal minCost);
            decimal.TryParse(productCategoriesViewModel.MaxConst, out decimal maxCost);
            if (minCost < maxCost)
                products = products.Where(prod => prod.Cost >= minCost && prod.Cost <= maxCost).ToList();
        }
    }
}
