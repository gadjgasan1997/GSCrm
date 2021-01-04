using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class SyncRespsViewModel : BaseViewModel
    {
        public Guid EmployeeId { get; set; }
        public List<string> ResponsibilitiesToAdd { get; set; } = new List<string>();
        public List<string> ResponsibilitiesToRemove { get; set; } = new List<string>();
    }
}
