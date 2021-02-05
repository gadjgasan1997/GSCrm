using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using GSCrm.Data;
using GSCrm.Models.Enums;
using GSCrm.Data.ApplicationInfo;
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
                ProductCategoriesViewModel prodCatsCached = cachService.GetCachedItem<ProductCategoriesViewModel>(currentUser.Id, PROD_CATS);
                ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(serviceProvider, context);
                productCategoryRepository.SetViewInfo(PROD_CATS, pageNumber);
                productCategoryRepository.AttachProductCategories(ref prodCatsCached);
                return Json(prodCatsCached, serializerSettings);
            }
            return Json("");
        }

        [HttpPost("Search")]
        public IActionResult Search(ProductCategoriesViewModel prodCatsViewModel)
        {
            new ProductCategoryRepository(serviceProvider, context).Search(prodCatsViewModel);
            return RedirectToAction(PROD_CATS, PROD_CAT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("ClearSearch")]
        public IActionResult ClearSearch()
        {
            new ProductCategoryRepository(serviceProvider, context).ClearSearch();
            return RedirectToAction(PROD_CATS, PROD_CAT, new { pageNumber = DEFAULT_MIN_PAGE_NUMBER });
        }

        [HttpGet("NextRecords")]
        public IActionResult NextRecords()
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, PROD_CATS);
            return RedirectToAction(PROD_CATS, PROD_CAT, new { pageNumber = viewInfo.CurrentPageNumber + DEFAULT_PAGE_STEP });
        }

        [HttpGet("PreviousRecords")]
        public IActionResult PreviousRecords()
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, PROD_CATS);
            return RedirectToAction(PROD_CATS, PROD_CAT, new { pageNumber = viewInfo.CurrentPageNumber - DEFAULT_PAGE_STEP });
        }
    }
}
