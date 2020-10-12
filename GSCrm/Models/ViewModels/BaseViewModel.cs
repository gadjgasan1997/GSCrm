using GSCrm.Data.ApplicationInfo;
using System;
using System.Collections.Generic;

namespace GSCrm.Models.ViewModels
{
    public class BaseViewModel
    {
        public Guid Id { get; set; }
        public Dictionary<string, Guid> IdCash { get; set; } = new Dictionary<string, Guid>();
    }
}
