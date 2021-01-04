using System.Collections.Generic;
using static GSCrm.CommonConsts;

namespace GSCrm.Data.ApplicationInfo
{
    public class ViewInfo
    {
        public int CurrentPageNumber { get; set; } = DEFAULT_MIN_PAGE_NUMBER;
        public int SkipSteps { get; set; }
        public int ItemsCount { get; set; } = DEFAULT_ITEMS_COUNT;
    }
}
