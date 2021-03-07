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
        
        // Получение id Текущей организации
        let organizationId = Utils.GetParamFromUrlByIndex(2);
        if (Utils.IsNullOrEmpty(categoriesCash[organizationId])) {
            categoriesCash[organizationId] = {};
        }

        // Получение списка развернутых категорий для выбранной организации
        if (Utils.IsNullOrEmpty(categoriesCash[organizationId]["ExpandedCategories"])) {
            categoriesCash[organizationId]["ExpandedCategories"] = [ categoryId ];
            localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
        }

        else if (!categoriesCash[organizationId]["ExpandedCategories"].includes(categoryId)) {
            let expandedCategories = categoriesCash[organizationId]["ExpandedCategories"];
            expandedCategories.push(categoryId);
            categoriesCash[organizationId]["ExpandedCategories"] = expandedCategories;
            localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
        }
    }

    /**
     * Базовый обработчик сворачивания категории
     * @param {*} categoryId Id свернутой категории
     */
    CollapseCategory(categoryId) {
        // Попытка получить кеш категорий
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (!Utils.IsNullOrEmpty(categoriesCash)) {
            
            // Получение id Текущей организации
            let organizationId = Utils.GetParamFromUrlByIndex(2);
            if (!Utils.IsNullOrEmpty(categoriesCash[organizationId])) {

                // Получение информации о развернутых категориях для выбранной организации
                let expandedCategories = categoriesCash[organizationId]["ExpandedCategories"];
                if (!Utils.IsNullOrEmpty(expandedCategories) && expandedCategories.includes(categoryId)) {
    
                    // Удаление категории из списка развернутых
                    expandedCategories.splice(expandedCategories.indexOf(categoryId), 1);
                    categoriesCash[organizationId]["ExpandedCategories"] = expandedCategories;
                    localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
                }
            }
        }
    }
}