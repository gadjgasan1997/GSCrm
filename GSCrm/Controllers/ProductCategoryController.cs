using GSCrm.Mapping;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using GSCrm.Data;
using GSCrm.Models.Enums;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(PROD_CAT)]
    public class ProductCategoryController : MainController<ProductCategory, ProductCategoryViewModel>
    {
        public ProductCategoryController(IServiceProvider serviceProvider, ApplicationDbContext context) : base(context, serviceProvider)
        { }

        [HttpGet("ProductCategories/{pageNumber}")]
        public IActionResult ProductCategories(int pageNumber)
        {
            // Запрос сюда возможен после получения представления продуктовых категорий, а значит, организация в этот момент уже инициализирована
            Organization organization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            if (organization != null)
            {
                ProductCategoriesViewModel prodCatsViewModel = cachService.GetCachedItem<ProductCategoriesViewModel>(currentUser.Id, PROD_CATS);
                ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(serviceProvider, context);
                productCategoryRepository.SetViewInfo(PROD_CATS, pageNumber);
                productCategoryRepository.InitProductCategoriesViewModel(ref prodCatsViewModel);
                return Json(prodCatsViewModel, serializerSettings);
            }
            return Json("");
        }

        [HttpPost("Search")]
        public IActionResult SearchByCategoryName(ProductCategoriesViewModel productCategoriesViewModel)
        {
            cachService.CacheItem(currentUser.Id, PROD_CATS, productCategoriesViewModel);
            return RedirectToAction(PROD_CATS, PROD_CAT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearSearch")]
        public IActionResult ClearSearchByCategoryName()
        {
            new ProductCategoryRepository(serviceProvider, context).ClearSearch();
            return RedirectToAction(PROD_CATS, PROD_CAT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }
    }
}
