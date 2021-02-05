class BaseProductCategoriesRender {
    /**
     * Базовый обработчик разворачивания категории
     * @param {*} categoryId Id развернутой категории
     */
    ExpandCategory(categoryId) {
        // Запоминание категории как развернутой
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (Utils.IsNullOrEmpty(categoriesCash)) {
            categoriesCash = {};
        }

        if (Utils.IsNullOrEmpty(categoriesCash["ExpandedCategories"])) {
            categoriesCash["ExpandedCategories"] = [ categoryId ];
            localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
        }

        else if (!categoriesCash["ExpandedCategories"].includes(categoryId)) {
            let expandedCategories = categoriesCash["ExpandedCategories"];
            expandedCategories.push(categoryId);
            categoriesCash["ExpandedCategories"] = expandedCategories;
            localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
        }
    }

    /**
     * Базовый обработчик сворачивания категории
     * @param {*} categoryId Id свернутой категории
     */
    CollapseCategory(categoryId) {
        // Удаление категории из списка развернутых
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (!Utils.IsNullOrEmpty(categoriesCash)) {
            let expandedCategories = categoriesCash["ExpandedCategories"];
            if (!Utils.IsNullOrEmpty(expandedCategories) && expandedCategories.includes(categoryId)) {
                expandedCategories.splice(expandedCategories.indexOf(categoryId), 1);
                categoriesCash["ExpandedCategories"] = expandedCategories;
                localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
            }
        }
    }
}