using System.Collections.Generic;
using static GSCrm.CommonConsts;

namespace GSCrm.Data.ApplicationInfo
{
    public class ViewInfo
    {
        /// <summary>
        /// Конструктор для представлений с уникальным названием
        /// </summary>
        /// <param name="viewName"></param>
        public ViewInfo(string viewName)
        {
            Name = viewName;
            Type = viewName;
            ItemsCount = !ViewItemsCount.ContainsKey(viewName) ? DEFAULT_ITEMS_COUNT : ViewItemsCount[viewName];
            RenderName = !NeedJSRender() || !ViewRenderers.ContainsKey(viewName) ? string.Empty : ViewRenderers[viewName];
        }

        /// <summary>
        /// Конструктор для представлений, название которых не является уникальным
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewType"></param>
        public ViewInfo(string viewName, string viewType)
        {
            // Количество элементов и прочие настройки для этих представлений должны находиться по типу, а не названию, так как название каждый раз будет разное
            Name = viewName;
            Type = viewType;
            ItemsCount = !ViewItemsCount.ContainsKey(viewType) ? DEFAULT_ITEMS_COUNT : ViewItemsCount[viewType];
            RenderName = !NeedJSRender() || !ViewRenderers.ContainsKey(viewType) ? string.Empty : ViewRenderers[viewType];
        }

        /// <summary>
        /// Уникальное название представления
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Неуникальный тип представления
        /// </summary>
        public string Type { get; private set; }
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
            => Type switch
            {
                PROD_CATS => true,
                _ => false
            };
    }
}
