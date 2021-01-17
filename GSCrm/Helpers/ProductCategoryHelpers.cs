using GSCrm.Data;
using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class ProductCategoryHelpers
    {
        public static List<Product> GetProducts(this ProductCategory productCategory, ApplicationDbContext context)
            => context.Products.AsNoTracking().Where(prod => prod.ProductCategoryId == productCategory.Id).ToList();
    }
}
