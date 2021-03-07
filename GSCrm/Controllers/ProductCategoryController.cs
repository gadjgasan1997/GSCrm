using System;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Data.ApplicationInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using static GSCrm.CommonConsts;

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
            => Json(cachService.GetCachedCurrentEntity<ProductCategoryViewModel>(currentUser));

        protected override IActionResult CreateSuccessHandler()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, currentOrganization.Id, PROD_CATS);
            return Redirect($"/{ORGANIZATION}/{currentOrganization.Id}/GetProductCategoriesData/{viewInfo.CurrentPageNumber}/");
        }

        protected override IActionResult UpdateSuccessHandler()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, currentOrganization.Id, PROD_CATS);
            return Redirect($"/{ORGANIZATION}/{currentOrganization.Id}/GetProductCategoriesData/{viewInfo.CurrentPageNumber}/");
        }

        protected override IActionResult DeleteSuccessHandler()
        {
            Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
            ViewInfo viewInfo = cachService.GetViewInfo(currentUser.Id, currentOrganization.Id, PROD_CATS);
            string returnUrl = Url.Content($"/{ORGANIZATION}/{currentOrganization.Id}/GetProductCategoriesData/{viewInfo.CurrentPageNumber}/");
            return Json(returnUrl);
        }
    }
}
