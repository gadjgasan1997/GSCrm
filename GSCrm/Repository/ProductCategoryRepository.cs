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
    public class ProductCategoryRepository : BaseRepository<ProductCategory, ProductCategoryViewModel>
    {
        public ProductCategoryRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        public void InitProductCategoriesViewModel(ref ProductCategoriesViewModel prodCatsViewModel)
        {
            ProductCategoriesViewModel viewModel = prodCatsViewModel;
            viewModel.ProductCategoryViewModels = context.GetOrgProdCats(prodCatsViewModel.OrganizationViewModel.Id)
                .MapToViewModels(map, prodCats => GetLimitedProdCatsList(prodCats, viewModel));
            prodCatsViewModel = viewModel;
        }

        private List<ProductCategory> GetLimitedProdCatsList(List<ProductCategory> prodCats, ProductCategoriesViewModel prodCatsViewModel)
        {
            // Получение корневых категорий и ограничение их по количеству
            List<ProductCategory> rootProdCats = prodCats.Where(prodCat => prodCat.ParentProductCategoryId == null).OrderBy(n => n.Name).ToList();
            LimitListByPageNumber(PROD_CATS, ref rootProdCats);

            // Затем для всех рут директорий находятся дочерние
            Func<ProductCategory, bool> predicate = prodCat => prodCat.RootCategoryId != null && rootProdCats.Select(i => i.Id).Contains((Guid)prodCat.RootCategoryId);
            List<ProductCategory> limitedProdCats = rootProdCats.Concat(prodCats.Where(predicate)).ToList();
            LimitByCategoryName(ref limitedProdCats, prodCatsViewModel);

            // Для каждой дочерней категории необходимо получить список проудктов
            ProductMap productMap = new ProductMap(serviceProvider, context);
            ProductRepository productRepository = new ProductRepository(serviceProvider, context);
            limitedProdCats.ForEach(productCategory =>
            {
                List<ProductViewModel> productViewModels = productCategory.GetProducts(context)
                    .MapToViewModels(productMap, products =>
                        productRepository.GetLimitedProductList(products, prodCatsViewModel));
                if (productViewModels.Count > 0)
                {
                    string categoryId = productCategory.Id.ToString();
                    if (!prodCatsViewModel.CategoriesProducts.ContainsKey(categoryId))
                        prodCatsViewModel.CategoriesProducts.Add(categoryId, productViewModels);
                    else prodCatsViewModel.CategoriesProducts[categoryId] = productViewModels;
                }
            });
            return limitedProdCats.OrderBy(n => n.Name).ToList();
        }

        private void LimitByCategoryName(ref List<ProductCategory> productCategories, ProductCategoriesViewModel prodCatsViewModel)
        {
            string categoryName = prodCatsViewModel.SearchProductCategoryName?.ToLower().TrimStartAndEnd();
            if (!string.IsNullOrEmpty(categoryName))
                productCategories = productCategories.Where(cat => cat.Name.ToLower().Contains(categoryName)).ToList();
        }

        #region Searching
        public void Search(ProductCategoriesViewModel prodCatsViewModel)
        {
            ProductCategoriesViewModel prodCatsCache = cachService.GetCachedItem<ProductCategoriesViewModel>(currentUser.Id, PROD_CATS);
            prodCatsCache.SearchProductCategoryName = prodCatsViewModel.SearchProductCategoryName;
            prodCatsCache.SearchProductName = prodCatsViewModel.SearchProductName;
            prodCatsCache.MinConst = prodCatsViewModel.MinConst;
            prodCatsCache.MaxConst = prodCatsViewModel.MaxConst;
            cachService.CacheItem(currentUser.Id, PROD_CATS, prodCatsCache);
        }

        /// <summary>
        /// Метод очищает поиск по продуктам и категориям
        /// </summary>
        public void ClearSearch()
        {
            ProductCategoriesViewModel prodCatsCached = cachService.GetCachedItem<ProductCategoriesViewModel>(currentUser.Id, PROD_CATS);
            prodCatsCached.SearchProductCategoryName = default;
            prodCatsCached.SearchProductName = default;
            prodCatsCached.MinConst = default;
            prodCatsCached.MaxConst = default;
            cachService.CacheItem(currentUser.Id, PROD_CATS, prodCatsCached);
        }
        #endregion
    }
}
