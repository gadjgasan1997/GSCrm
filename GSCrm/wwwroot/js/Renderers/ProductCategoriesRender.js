class ProductCategoriesRender {
    static DepthConst = 46;

    Render() {
        // ��������� ������ ��������� ���������
        let productCategoriesUrl = Localization.GetUri("productCategories");
        let request = new AjaxRequests();
        request.CommonGetRequest(productCategoriesUrl).then(categories => {
            if (!Utils.IsNullOrEmpty(categories)) {
                // ��������� ���������
                this.RenderCategories(categories["ProductCategoryViewModels"]);
            }
        })
    }

    RenderCategories(categories) {
        // ������ �������� ���������� 
        categories.filter(category => {
            return category["ParentProductCategoryId"] == null;
        }).map(category => {
            let hasChildrens = categories.filter(item => item["ParentProductCategoryId"] == category["Id"]).length > 0;
            if (hasChildrens) {
                $("#productCategoriesTree").append(this.GetRootCategoryHTML(category));
            }
            else $("#productCategoriesTree").append(this.GetEmptyRootCategoryHTML(category));
            categories = categories.filter(item => item["Id"] != category["Id"]);

            // ������ �������� ���������
            this.RenderChildCategories(categories, category["Id"]);
        });
    }

    /**
     * ����� ���������� �������� ���������
     * @param {*} category 
     */
    GetRootCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + '>' +
            '<div class="row">' +
            '<div class="col-auto category-expand"><div class="block-center"><span class="icon-plus-square"></span></div></div>' +
            '<div class="col-auto category-name"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * ����� ���������� �������� ������ ���������
     * @param {*} category 
     */
    GetEmptyRootCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + '>' +
            '<div class="row">' +
            '<div class="col-auto category-name"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * ����� ���������� �������� ���������
     * @param {*} category 
     */
    GetChildCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + ' style="padding-left: ' +
            ProductCategoriesRender.DepthConst + 'px"><div class="row">' +
            '<div class="col-auto category-expand"><div class="block-center"><span class="icon-plus-square"></span></div></div>' +
            '<div class="col-auto category-name"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * ����� ���������� �������� ������ ���������
     * @param {*} category 
     */
    GetEmptyChildCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + ' style="padding-left: ' +
            ProductCategoriesRender.DepthConst + 'px"><div class="row">' +
            '<div class="col-auto category-name"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * ����� ���������� �������� �������� ���������
     * @param {*} categories ������ ��������������� ���������
     * @param {*} rootCategoryId Id ������������ ���������
     */
    RenderChildCategories(categories, rootCategoryId) {
        categories.filter(category => {
            return category["ParentProductCategoryId"] == rootCategoryId;
        }).map(childCategory => {
            // ���� �� � ���� ��������� ��������
            let hasChildrens = categories.filter(item => item["ParentProductCategoryId"] == childCategory["Id"]).length > 0;
            let childCategoryEl = hasChildrens ? this.GetChildCategoryHTML(childCategory) : this.GetEmptyChildCategoryHTML(childCategory);
            let rootCategoryEl = document.querySelectorAll("[data-category-id='" + rootCategoryId + "']")[0];
            $(rootCategoryEl).children(".child-category").append(childCategoryEl);
            categories = categories.filter(item => item["Id"] != childCategory["Id"]);
            
            // ������ ��������
            if (hasChildrens) {
                this.RenderChildCategories(categories, childCategory["Id"]);
            }
        });
    }

    /**
     * ���������� ���������
     * @param {*} event 
     */
    ExpandCategory(event) {
        let categoryRow = $(event.currentTarget).closest(".category-row");
        let childCategory = $(categoryRow).children(".child-category")[0];
        if (childCategory.length != 0) {
            $(childCategory).removeClass("d-none");
            $(event.currentTarget)
                .removeClass("category-expand")
                .addClass("category-collapse")
                .find(".icon-plus-square")
                .removeClass("icon-plus-square")
                .addClass("icon-minus");
        }
    }

    /**
     * �������� ���������
     * @param {*} event 
     */
    CollapseCategory(event) {
        let categoryRow = $(event.currentTarget).closest(".category-row");
        let childCategory = $(categoryRow).children(".child-category")[0];
        if (childCategory.length != 0) {
            $(childCategory).addClass("d-none");
            $(event.currentTarget)
                .addClass("category-expand")
                .removeClass("category-collapse")
                .find(".icon-minus")
                .removeClass("icon-minus")
                .addClass("icon-plus-square");
        }
    }
}

$("#productCategoriesTree")
    .off("click", ".category-expand").on("click", ".category-expand", event => {
        let renderer = new ProductCategoriesRender();
        renderer.ExpandCategory(event);
    })
    .off("click", ".category-collapse").on("click", ".category-collapse", event => {
        let renderer = new ProductCategoriesRender();
        renderer.CollapseCategory(event);
    });