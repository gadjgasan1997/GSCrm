class ProductCategoriesRender {
    static DepthConst = 46;

    //#region Rendering
    Render() {
        let productCategoriesUrl = Localization.GetUri("productCategories");
        this.RenderCategoriesRequest(productCategoriesUrl);
    }

    /**
     * Получение и рендеринг списка категория продуктов
     * @param {*} url Ссылка для получения списка категорий
     */
    RenderCategoriesRequest(url) {
        let request = new AjaxRequests();
        request.CommonGetRequest(url).then(response => {

            // Рендеринг категорий
            let categories = response["ProductCategoryViewModels"];
            if (!Utils.IsNullOrEmpty(categories)) {
                this.RenderCategories(categories);
            }
        })
    }

    /**
     * Рендеринг категорий
     * @param {*} categories Категории 
     */
    RenderCategories(categories) {
        // Очистка дерева
        $("#productCategoriesTree").empty();

        // Рендер корневых директорий 
        categories.filter(category => {
            return category["ParentProductCategoryId"] == null;
        }).map(category => {
            // Есть ли у текущей категории дочерние
            let hasChildrens = categories.filter(item => item["ParentProductCategoryId"] == category["Id"]).length > 0;
            if (hasChildrens) {
                $("#productCategoriesTree").append(this.GetRootCategoryHTML(category));
            }
            else $("#productCategoriesTree").append(this.GetEmptyRootCategoryHTML(category));
            categories = categories.filter(item => item["Id"] != category["Id"]);

            // Рендер дочерних категорий
            this.RenderChildCategories(categories, category["Id"]);
        });
    }

    /**
     * Метод возвращает корневую категорию
     * @param {*} category 
     */
    GetRootCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + '>' +
            '<div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            '<div class="col-auto category-expand mt-2"><div class="block-center"><span class="icon-plus-square"></span></div></div>' +
            '<div class="col-auto category-name mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * Метод возвращает корневую пустую категорию
     * @param {*} category 
     */
    GetEmptyRootCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + '>' +
            '<div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            '<div class="col-auto category-name mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * Метод возвращает дочернюю категорию
     * @param {*} category 
     */
    GetChildCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + ' style="padding-left: ' +
            ProductCategoriesRender.DepthConst + 'px"><div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            '<div class="col-auto category-expand mt-2"><div class="block-center"><span class="icon-plus-square"></span></div></div>' +
            '<div class="col-auto category-name mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * Метод возвращает дочернюю пустую категорию
     * @param {*} category 
     */
    GetEmptyChildCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + ' style="padding-left: ' +
            ProductCategoriesRender.DepthConst + 'px"><div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            '<div class="col-auto category-name mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /**
     * Метод рекурсивно рендерит дочерние категории
     * @param {*} categories Список неотрендеренных категорий
     * @param {*} rootCategoryId Id родительской категории
     */
    RenderChildCategories(categories, rootCategoryId) {
        categories.filter(category => {
            return category["ParentProductCategoryId"] == rootCategoryId;
        }).map(childCategory => {
            // Есть ли у этой категории дочерние
            let hasChildrens = categories.filter(item => item["ParentProductCategoryId"] == childCategory["Id"]).length > 0;
            let childCategoryEl = hasChildrens ? this.GetChildCategoryHTML(childCategory) : this.GetEmptyChildCategoryHTML(childCategory);
            let rootCategoryEl = document.querySelectorAll("[data-category-id='" + rootCategoryId + "']")[0];
            $(rootCategoryEl).children(".child-category").append(childCategoryEl);
            categories = categories.filter(item => item["Id"] != childCategory["Id"]);
            
            // Рендер дочерних
            if (hasChildrens) {
                this.RenderChildCategories(categories, childCategory["Id"]);
            }
        });
    }
    //#endregion

    //#region Actions
    Navigate(event) {
        event.preventDefault();
        let productCategoriesUrl = $(event.currentTarget).attr("data-href");
        let renderer = new ProductCategoriesRender();
        renderer.RenderCategoriesRequest(productCategoriesUrl);
    }

    /**
     * Развернуть категорию
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
     * Свернуть категорию
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

    /**
     * Выполняет поиск по категориям и продуктам
     * @param {*} event 
     */
    Search(event) {
        event.preventDefault();
        let searchUrl = $("#productCategoriesFilter").attr("action");
        let searchData = this.SearchGetData();
        let request = new AjaxRequests();
        request.CommonPostRequest(searchUrl, searchData).then(response => {
            this.RenderCategories(response["ProductCategoryViewModels"]);
        })
    }

    SearchGetData() {
        return {
            SearchProductCategoryName: $("#SearchProductCategoryName").val(),
            SearchProductName: $("#SearchProductName").val(),
            MinConst: $("#prodMinCost").val(),
            MaxConst: $("#prodMaxCost").val()
        }
    }

    /**
     * Очищает поиск по категориям и продуктам
     * @param {*} event 
     */
    ClearSearch(event) {
        event.preventDefault();
        let clearSearchUrl = $("#clearProdsSearch").attr("href");
        let request = new AjaxRequests();
        request.CommonGetRequest(clearSearchUrl).then(response => {

            // Рендеринг категорий
            let categories = response["ProductCategoryViewModels"];
            if (!Utils.IsNullOrEmpty(categories)) {
                this.RenderCategories(categories);
            }
            
            // Очистка полей
            $("#SearchProductCategoryName").val(response["SearchProductCategoryName"]);
            $("#SearchProductName").val(response["SearchProductName"]);
            $("#prodMinCost").val(response["MinConst"]);
            $("#prodMaxCost").val(response["MaxConst"]);
        })
    }
    //#endregion
}

$("#productCategoriesForm")
    .off("click", ".nav-previous .nav-url").on("click", ".nav-previous .nav-url", event => {
        let renderer = new ProductCategoriesRender();
        renderer.Navigate(event);
    })
    .off("click", ".nav-next .nav-url").on("click", ".nav-next .nav-url", event => {
        let renderer = new ProductCategoriesRender();
        renderer.Navigate(event);
    })
    .off("click", "#prodsSearch").on("click", "#prodsSearch", event => {
        let renderer = new ProductCategoriesRender();
        renderer.Search(event);
    })
    .off("click", "#clearProdsSearch").on("click", "#clearProdsSearch", event => {
        let renderer = new ProductCategoriesRender();
        renderer.ClearSearch(event);
    });

$("#productCategoriesTree")
    .off("click", ".category-expand").on("click", ".category-expand", event => {
        let renderer = new ProductCategoriesRender();
        renderer.ExpandCategory(event);
    })
    .off("click", ".category-collapse").on("click", ".category-collapse", event => {
        let renderer = new ProductCategoriesRender();
        renderer.CollapseCategory(event);
    });

// Меню настроек категории
$("#prodCatSettingsMenu")
    .off("click", "#addCategoryBtn").on("click", "#addCategoryBtn", event => {
        event.stopPropagation();
        event.preventDefault();
        $("#createProdCatName").val("");
        $("#createProdCatDesc").val("");
        $("#addCategoryModal").modal();
    })
    .off("click", "#addProductBtn").on("click", "#addProductBtn", event => {
        event.stopPropagation();
        event.preventDefault();
        $("#createProdName").val("");
        $("#createProdDesc").val("");
        $("#createProdCost").val("");
        $("#addProductModal").modal();
    })
    .off("click", "#deleteCategoryBtn").on("click", "#deleteCategoryBtn", event => {
        event.stopPropagation();
        event.preventDefault();
        let productCategory = new ProductCategory();
        productCategory.Delete(event);
    })
    .off("click", "input").on("click", "input", event => {
        $("#prodCatSettingsMenu").addClass("d-none");
    });

// Меню настроек продуктов
$("#prodSettingsMenu")
    .off("click", "#deleteProductBtn").on("click", "#deleteProductBtn", event => {
        event.stopPropagation();
        event.preventDefault();
        let product = new Product();
        product.Delete(event);
    });