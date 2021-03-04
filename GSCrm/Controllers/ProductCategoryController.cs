using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Mapping;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static GSCrm.CommonConsts;
using GSCrm.Data.ApplicationInfo;

namespace GSCrm.Controllers
{
    [Authorize]
    [Route(PROD_CAT)]
    public class ProductCategoryController : MainController<ProductCategory, ProductCategoryViewModel>
    {
        public ProductCategoryController(IServiceProvider serviceProvider, ApplicationDbContext context) : base(context, serviceProvider)
        { }

        [HttpGet("{id}/Initialize")]
        public IActionResult Initalize()
        {
            ProductCategory productCategory = cachService.GetCachedCurrentEntity<ProductCategory>(currentUser);
            ProductCategoryViewModel prodCatViewModel = new ProductCategoryMap(serviceProvider, context).DataToViewModel(productCategory);
            return Json(prodCatViewModel);
        }

        protected override IActionResult CreateSuccessHandler()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, currentOrganization.Id, PROD_CATS);
            return Redirect($"/{ORGANIZATION}/{currentOrganization.Id}/GetProductCategoriesData/{viewInfo.CurrentPageNumber}/");
        }

        protected override IActionResult UpdateSuccessHandler()
            => RedirectToAction(PROD_CATS, PROD_CAT);

        protected override IActionResult DeleteSuccessHandler()
            => Json(Url.Action(PROD_CATS, PROD_CAT));
    }
}
