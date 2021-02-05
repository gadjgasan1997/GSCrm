using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models.Enums;
using GSCrm.Data.ApplicationInfo;
using static GSCrm.CommonConsts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GSCrm.Helpers;

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
            if (organization != null)
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

        [HttpGet("Initalize/{categoryId?}")]
        public IActionResult Initalize(string categoryId = "")
        {
            // Проверки, что категория существует и пользователь имеет права на просмотр организации
            if (!repository.TryGetItemById(categoryId, out ProductCategory productCategory))
                return Json("");
            OrganizationRepository organizationRepository = new OrganizationRepository(serviceProvider, context);
            if (!organizationRepository.HasPermissionsForSeeItem(productCategory.GetOrganization(context)))
                return Json("");
            if (!repository.HasPermissionsForSeeItem(productCategory))
                return Json("");

            // Возврат данных категории
            else
            {
                ProductCategoryViewModel prodCatViewModel = new ProductCategoryMap(serviceProvider, context).DataToViewModel(productCategory);
                return Json(prodCatViewModel);
            }
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
