using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class ProductCategoriesViewModel : BaseViewModel
    {
        public OrganizationViewModel OrganizationViewModel { get; set; }
        public List<ProductCategoryViewModel> ProductCategoryViewModels { get; set; }
        public Dictionary<string, List<ProductViewModel>> CategoriesProducts { get; set; }
            = new Dictionary<string, List<ProductViewModel>>() { };

        public Guid OrganizationId { get; set; }
        public string SearchProductCategoryName { get; set; }
        public string SearchProductName { get; set; }
        public string SearchMinConst { get; set; }
        public string SearchMaxConst { get; set; }
    }
}
