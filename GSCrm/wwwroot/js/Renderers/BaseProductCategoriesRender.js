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
        
        // ��������� id ������� �����������
        let organizationId = Utils.GetParamFromUrlByIndex(2);
        if (Utils.IsNullOrEmpty(categoriesCash[organizationId])) {
            categoriesCash[organizationId] = {};
        }

        // ��������� ������ ����������� ��������� ��� ��������� �����������
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
     * ������� ���������� ������������ ���������
     * @param {*} categoryId Id ��������� ���������
     */
    CollapseCategory(categoryId) {
        // ������� �������� ��� ���������
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (!Utils.IsNullOrEmpty(categoriesCash)) {
            
            // ��������� id ������� �����������
            let organizationId = Utils.GetParamFromUrlByIndex(2);
            if (!Utils.IsNullOrEmpty(categoriesCash[organizationId])) {

                // ��������� ���������� � ����������� ���������� ��� ��������� �����������
                let expandedCategories = categoriesCash[organizationId]["ExpandedCategories"];
                if (!Utils.IsNullOrEmpty(expandedCategories) && expandedCategories.includes(categoryId)) {
    
                    // �������� ��������� �� ������ �����������
                    expandedCategories.splice(expandedCategories.indexOf(categoryId), 1);
                    categoriesCash[organizationId]["ExpandedCategories"] = expandedCategories;
                    localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
                }
            }
        }
    }
}