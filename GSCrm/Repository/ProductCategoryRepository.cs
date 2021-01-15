using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class ProductCategoryRepository : BaseRepository<ProductCategory, ProductCategoryViewModel>
    {
        public ProductCategoryRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public void AttachProductCategories(ref ProductCategoriesViewModel productCategoriesViewModel)
        {
            productCategoriesViewModel.ProductCategoryViewModels = context.GetOrgProdCats(productCategoriesViewModel.OrganizationViewModel.Id)
                .MapToViewModels(map, GetLimitedProdCatsList);
        }

        private List<ProductCategory> GetLimitedProdCatsList(List<ProductCategory> prodCats)
        {
            List<ProductCategory> limitedProdCats = prodCats;
            LimitListByPageNumber(PROD_CATS, ref prodCats);
            return limitedProdCats;
        }
    }
}
