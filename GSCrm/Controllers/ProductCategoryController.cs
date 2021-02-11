using System;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models.Enums;
using GSCrm.Data.ApplicationInfo;
using GSCrm.Helpers;
using static GSCrm.CommonConsts;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(PROD_CAT)]
    public class ProductCategoryController : MainController<ProductCategory, ProductCategoryViewModel>
    {
        public ProductCategoryController(IServiceProvider serviceProvider, ApplicationDbContext context) : base(context, serviceProvider)
        { }

        [HttpGet("ProductCategories")]
        public IActionResult ProductCategories()
        {
            // Запрос сюда возможен после получения представления продуктовых категорий, а значит, организация в этот момент уже инициализирована
            Organization organization = cachService.GetMainEntity(currentUser, MainEntityType.OrganizationData) as Organization;
            if (organization.Id != Guid.Empty)
            {
                ProductCategoriesViewModel prodCatsCached = cachService.GetCachedItem<ProductCategoriesViewModel>(currentUser.Id, PROD_CATS);
                ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(serviceProvider, context);
                ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, PROD_CATS);
                productCategoryRepository.SetViewInfo(PROD_CATS, viewInfo.CurrentPageNumber);
                productCategoryRepository.AttachProductCategories(ref prodCatsCached);
                return Json(prodCatsCached, serializerSettings);
            }
            return Json("");
        }

        [HttpGet("Initalize/{id}")]
        public IActionResult Initalize()
        {
            // Получение признака проверки категории и прав пользователя на ее просмотр
            if (bool.TryParse(cachService.GetCachedItem(currentUser.Id, $"{PC}{PROD_CATS}"), out bool isCheckCorrect) && isCheckCorrect)
            {
                ProductCategory productCategory = cachService.GetCachedItem<ProductCategory>(currentUser.Id, "CurrentProductCategoryData");
                ProductCategoryViewModel prodCatViewModel = new ProductCategoryMap(serviceProvider, context).DataToViewModel(productCategory);
                return Json(prodCatViewModel);
            }
            return Json("");
        }

        protected override IActionResult CreateSuccessHandler()
            => RedirectToAction(PROD_CATS, PROD_CAT);

        protected override IActionResult UpdateSuccessHandler()
            => RedirectToAction(PROD_CATS, PROD_CAT);

        protected override IActionResult DeleteSuccessHandler()
            => Json(Url.Action(PROD_CATS, PROD_CAT));

        [HttpPost("Search")]
        public IActionResult Search(ProductCategoriesViewModel prodCatsViewModel)
        {
            new ProductCategoryRepository(serviceProvider, context).Search(prodCatsViewModel);
            return RedirectToAction(PROD_CATS, PROD_CAT);
        }

        [HttpGet("ClearSearch")]
        public IActionResult ClearSearch()
        {
            new ProductCategoryRepository(serviceProvider, context).ClearSearch();
            return RedirectToAction(PROD_CATS, PROD_CAT);
        }

        [HttpGet("NextRecords")]
        public IActionResult NextRecords()
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, PROD_CATS);
            viewInfo.CurrentPageNumber++;
            return RedirectToAction(PROD_CATS, PROD_CAT);
        }

        [HttpGet("PreviousRecords")]
        public IActionResult PreviousRecords()
        {
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, PROD_CATS);
            viewInfo.CurrentPageNumber--;
            return RedirectToAction(PROD_CATS, PROD_CAT);
        }
    }
}
