class BaseProductCategoriesRender {
    /**
     * ������� ���������� �������������� ���������
     * @param {*} categoryId Id ����������� ���������
     */
    ExpandCategory(categoryId) {
        // ����������� ��������� ��� �����������
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
     * ������� ���������� ������������ ���������
     * @param {*} categoryId Id ��������� ���������
     */
    CollapseCategory(categoryId) {
        // �������� ��������� �� ������ �����������
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