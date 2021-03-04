using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class SyncAccountViewModel : BaseViewModel
    {
        public string PrimaryManagerId { get; set; }
        public List<string> ManagersToAdd { get; set; } = new List<string>();
        public List<string> ManagersToRemove { get; set; } = new List<string>();
    }
}
