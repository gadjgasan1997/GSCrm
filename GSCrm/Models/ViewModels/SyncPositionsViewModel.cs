using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class SyncPositionsViewModel : BaseViewModel
    {
        public Guid OrganizationId { get; set; }
        public string PrimaryPositionName { get; set; }
        public List<string> PositionsToAdd { get; set; } = new List<string>();
        public List<string> PositionsToRemove { get; set; } = new List<string>();
    }
}
