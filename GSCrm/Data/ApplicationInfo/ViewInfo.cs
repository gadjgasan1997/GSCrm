using System.Collections.Generic;
using static GSCrm.CommonConsts;

namespace GSCrm.Data.ApplicationInfo
{
    public class ViewInfo
    {
        public ViewInfo(string viewName)
        {
            Name = viewName;
            ItemsCount = !ViewItemsCount.ContainsKey(viewName) ? DEFAULT_ITEMS_COUNT : ViewItemsCount[viewName];
            RenderName = !NeedJSRender() || !ViewRenderers.ContainsKey(viewName) ? string.Empty : ViewRenderers[viewName];
        }

        public string Name { get; private set; }
        public int CurrentPageNumber { get; set; } = DEFAULT_MIN_PAGE_NUMBER;
        public int SkipSteps { get; set; }
        public int ItemsCount { get; }
        public string RenderName { get; private set; }

        /// <summary>
        /// Словарь с количеством элементов в представлениях
        /// </summary>
        private static Dictionary<string, int> ViewItemsCount
            => new Dictionary<string, int>()
            {
                { RESPONSIBILITIES, 5 },
                { USER_NOTS, 8 },
                { SELECTED_EMP_POSS, 5 },
                { ALL_EMP_POSS, 5 },
                { SELECTED_EMP_RESPS, 5 },
                { ALL_EMP_RESPS, 5 },
                { ACC_TEAM_SELECTED_EMPLOYEES, 5 },
                { ACC_TEAM_ALL_EMPLOYEES, 5 }
            };

        /// <summary>
        /// Словарь с названиями представлений и рендерами для них
        /// </summary>
        private static Dictionary<string, string> ViewRenderers
            => new Dictionary<string, string>()
            {
                { PROD_CATS, $"{PROD_CATS}Render" }
            };

        /// <summary>
        /// Метод определяет, надо ли рендереить представление с помощью JS
        /// </summary>
        private bool NeedJSRender()
            => Name switch
            {
                PROD_CATS => true,
                _ => false
            };
    }
}
