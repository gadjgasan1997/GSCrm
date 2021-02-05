class BaseProductCategoriesRender {
    /**
     * ������� ���������� �������������� ���������
     * @param {*} categoryId Id ����������� ���������
     */
    ExpandCategory(categoryId) {
        // ����������� ��������� ��� �����������
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
     * ������� ���������� ������������ ���������
     * @param {*} categoryId Id ��������� ���������
     */
    CollapseCategory(categoryId) {
        // �������� ��������� �� ������ �����������
        let expandedCategories = JSON.parse(localStorage.getItem("ExpandedCategories"));
        if (!Utils.IsNullOrEmpty(expandedCategories) && expandedCategories.includes(categoryId)) {
            expandedCategories.splice(categoryId, 1);
            localStorage.setItem("ExpandedCategories", JSON.stringify(expandedCategories));
        }
    }
}