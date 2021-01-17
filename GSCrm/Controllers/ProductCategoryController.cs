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
            cachService.GetCachedOrganization(currentUser, out Organization organization, out OrganizationViewModel orgViewModel);
            if (organization == null)
            {
                ProductCategoriesViewModel prodCatsViewModel = new ProductCategoriesViewModel()
                {
                    OrganizationViewModel = orgViewModel
                };
                ProductCategoryRepository productCategoryRepository = new ProductCategoryRepository(serviceProvider, context);
                productCategoryRepository.SetViewInfo(PROD_CATS, pageNumber);
                productCategoryRepository.AttachProductCategories(ref prodCatsViewModel);
                return Json(prodCatsViewModel, serializerSettings);
            }
            return Json("");
        }
    }
}
