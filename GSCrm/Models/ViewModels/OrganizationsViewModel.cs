using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class OrganizationsViewModel : BaseViewModel
    {
        public string SearchName { get; set; }
        public Dictionary<string, string> SearchNameCash { get; set; } = new Dictionary<string, string>();
        public IEnumerable<OrganizationViewModel> OrganizationViewModels { get; set; }
    }
}
