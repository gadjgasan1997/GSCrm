using System;

namespace GSCrm.Models.ViewModels
{
    public class ProductCategoryViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentProductCategoryId { get; set; }
    }
}
