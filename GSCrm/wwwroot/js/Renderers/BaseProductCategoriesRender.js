class BaseProductCategoriesRender {
    /**
     * Базовый обработчик разворачивания категории
     * @param {*} categoryId Id развернутой категории
     */
    ExpandCategory(categoryId) {
        // Запоминание категории как развернутой
        let expandedCategories = JSON.parse(localStorage.getItem("ExpandedCategories"));
        if (Utils.IsNullOrEmpty(expandedCategories)) {
            expandedCategories = [ categoryId ];
            localStorage.setItem("ExpandedCategories", JSON.stringify(expandedCategories));
        }
        else {
            if (!expandedCategories.includes(categoryId)) {
                expandedCategories.push(categoryId);
                localStorage.setItem("ExpandedCategories", JSON.stringify(expandedCategories));
            }
        }
    }

    /**
     * Базовый обработчик сворачивания категории
     * @param {*} categoryId Id свернутой категории
     */
    CollapseCategory(categoryId) {
        // Удаление категории из списка развернутых
        let expandedCategories = JSON.parse(localStorage.getItem("ExpandedCategories"));
        if (!Utils.IsNullOrEmpty(expandedCategories) && expandedCategories.includes(categoryId)) {
            expandedCategories.splice(categoryId, 1);
            localStorage.setItem("ExpandedCategories", JSON.stringify(expandedCategories));
        }
    }
}