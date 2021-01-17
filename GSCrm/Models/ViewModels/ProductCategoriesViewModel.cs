using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class ProductCategoriesViewModel : BaseViewModel
    {
        public OrganizationViewModel OrganizationViewModel { get; set; }
        public List<ProductCategoryViewModel> ProductCategoryViewModels { get; set; }
        public Dictionary<string, List<ProductViewModel>> CategoriesProducts { get; set; }
            = new Dictionary<string, List<ProductViewModel>>() { };

        public string SearchProductCategoryName { get; set; }
        public string SearchProductName { get; set; }
        public string MinConst { get; set; }
        public string MaxConst { get; set; }
    }
}
