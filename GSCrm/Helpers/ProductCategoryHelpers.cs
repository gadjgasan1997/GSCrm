using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class ProductCategoryHelpers
    {
        public static List<Product> GetProducts(this ProductCategory productCategory, ApplicationDbContext context)
            => context.Products.AsNoTracking().Where(prod => prod.ProductCategoryId == productCategory.Id).ToList();

        public static Organization GetOrganization(this ProductCategory productCategory, ApplicationDbContext context)
            => context.Organizations.AsNoTracking().FirstOrDefault(i => i.Id == productCategory.OrganizationId);

        public static void Normalize(this ProductCategoryViewModel prodCatViewModel)
        {
            prodCatViewModel.Name = prodCatViewModel.Name?.TrimStartAndEnd();
            prodCatViewModel.Description = prodCatViewModel.Description?.TrimStartAndEnd();
        }

        public static void NormalizeSearch(this ProductCategoriesViewModel prodCatsViewModel)
        {
            prodCatsViewModel.SearchProductName = prodCatsViewModel.SearchProductName?.ToLower().TrimStartAndEnd();
            prodCatsViewModel.SearchProductCategoryName = prodCatsViewModel.SearchProductCategoryName?.ToLower().TrimStartAndEnd();
        }
    }
}
