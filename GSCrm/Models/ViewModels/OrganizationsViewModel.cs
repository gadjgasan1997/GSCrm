using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class OrganizationsViewModel : BaseViewModel
    {
        public string SearchName { get; set; }
        public IEnumerable<OrganizationViewModel> OrganizationViewModels { get; set; }
    }
}
