﻿using System;
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

        public void InitProductCategoriesViewModel(ref ProductCategoriesViewModel productCategoriesViewModel)
        {
            ProductCategoriesViewModel viewModel = productCategoriesViewModel;
            viewModel.ProductCategoryViewModels = context.GetOrgProdCats(productCategoriesViewModel.OrganizationViewModel.Id)
                .MapToViewModels(map, prodCats => GetLimitedProdCatsList(prodCats, viewModel));
            productCategoriesViewModel = viewModel;
        }

        private List<ProductCategory> GetLimitedProdCatsList(List<ProductCategory> prodCats, ProductCategoriesViewModel productCategoriesViewModel)
        {
            // Получение корневых категорий и ограничение их по количеству
            List<ProductCategory> rootProdCats = prodCats.Where(prodCat => prodCat.ParentProductCategoryId == null).OrderBy(n => n.Name).ToList();
            LimitListByPageNumber(PROD_CATS, ref rootProdCats);

            // Затем для всех рут директорий находятся дочерние
            Func<ProductCategory, bool> predicate = prodCat => prodCat.RootCategoryId == null || rootProdCats.Select(i => i.Id).Contains((Guid)prodCat.RootCategoryId);
            List<ProductCategory> limitedProdCats = prodCats.Where(predicate).ToList();

            // Для каждой дочерней категории необходимо получить список проудктов
            ProductMap productMap = new ProductMap(serviceProvider, context);
            ProductRepository productRepository = new ProductRepository(serviceProvider, context);
            limitedProdCats.ForEach(productCategory =>
            {
                productCategoriesViewModel.CategoriesProducts.Add(productCategory.Id.ToString(),
                    productCategory.GetProducts(context).MapToViewModels(productMap, products =>
                        productRepository.GetLimitedProductList(products, productCategoriesViewModel)));
            });
            return limitedProdCats.OrderBy(n => n.Name).ToList();
        }

        #region Searching
        /// <summary>
        /// Метод очищает поиск по продуктам и категориям
        /// </summary>
        public void ClearSearch()
        {
            ProductCategoriesViewModel productCategoriesViewModel = cachService.GetCachedItem<ProductCategoriesViewModel>(currentUser.Id, PROD_CATS);
            productCategoriesViewModel.SearchProductCategoryName = default;
            productCategoriesViewModel.SearchProductName = default;
            productCategoriesViewModel.MinConst = string.Empty;
            productCategoriesViewModel.MaxConst = string.Empty;
            cachService.CacheItem(currentUser.Id, PROD_CATS, productCategoriesViewModel);
        }
        #endregion
    }
}
