using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class ProductCategoriesViewModel
    {
        public OrganizationViewModel OrganizationViewModel { get; set; }
        public List<ProductCategoryViewModel> ProductCategoryViewModels { get; set; }
    }
}
