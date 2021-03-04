class ProductCategoriesRender extends BaseProductCategoriesRender {
    static DepthConst = 46;

    //#region Rendering
    Render() {
        let productCategoriesUrl = $("#productCategoriesForm").attr("data-href");
        this.RenderCategoriesRequest(productCategoriesUrl);
    }

    /**
     * Получение и рендеринг списка категория продуктов
     * @param {*} url Ссылка для получения списка категорий
     */
    RenderCategoriesRequest(url) {
        let request = new AjaxRequests();
        request.JsonGetRequest(url)
            .catch(response => Utils.DefaultErrorHandling(response["responseJSON"]))
            .then(response => {

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
        return new Promise((resolve, reject) => {
            // Очистка дерева
            $("#productCategoriesTree").empty();

            // Рендер корневых директорий 
            categories.filter(category => {
                return Utils.IsNullOrEmpty(category["ParentProductCategoryId"]);
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

            resolve();
        })
    }

    /**
     * Метод возвращает корневую категорию
     * @param {*} category 
     */
    GetRootCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + '>' +
            '<div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            this.GetExpandHTML(category["Id"]) + '</div>' +
            '<div class="col-auto category-name' + (this.NeedBacklight(category) ? " searched-category-name" : "") + ' mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category' + (this.IsCategoryExpand(category["Id"]) ? "" : " d-none") + '"></div></div>';
    }

    /**
     * Метод возвращает корневую пустую категорию
     * @param {*} category 
     */
    GetEmptyRootCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + '>' +
            '<div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            '<div class="col-auto category-name' + (this.NeedBacklight(category) ? " searched-category-name" : "") + ' mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
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
            this.GetExpandHTML(category["Id"]) + '</div>' +
            '<div class="col-auto category-name' + (this.NeedBacklight(category) ? " searched-category-name" : "") + ' mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category' + (this.IsCategoryExpand(category["Id"]) ? "" : " d-none") + '"></div></div>';
    }

    /**
     * Метод возвращает дочернюю пустую категорию
     * @param {*} category 
     */
    GetEmptyChildCategoryHTML(category) {
        return '<div class="category-row mt-3" data-category-id=' + category["Id"] + ' style="padding-left: ' +
            ProductCategoriesRender.DepthConst + 'px"><div class="category-row-main-line">' +
            '<div class="settings-menu-btn col-auto mt-2" data-settings-menu-id="prodCatSettingsMenu"><div class="block-center"><span class="icon-dots-three-vertical"></span></div></div>' +
            '<div class="col-auto category-name' + (this.NeedBacklight(category) ? " searched-category-name" : "") + ' mt-2 text-center"><p class="label-md">' + category["Name"] + '</p></div>' +
            '</div><div class="child-category d-none"></div></div>';
    }

    /** Метод возвращает значок сворачивания/разворота категории */
    GetExpandHTML(categoryId) {
        if (this.IsCategoryExpand(categoryId)) {
            return '<div class="col-auto category-collapse mt-2"><div class="block-center"><span class="icon-minus"></span></div>';
        }
        return '<div class="col-auto category-expand mt-2"><div class="block-center"><span class="icon-plus-square"></span></div>';
    }

    /**
     * Метод определяет, была ли развернута категория
     * @param {*} categoryId Id категории
     */
    IsCategoryExpand(categoryId) {
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (Utils.IsNullOrEmpty(categoriesCash))
            return false;
        let expandedCategories = categoriesCash["ExpandedCategories"];
        return !Utils.IsNullOrEmpty(expandedCategories) && expandedCategories.includes(categoryId);
    }

    /**
     * Метод определяет, необходима ли подсветка для поданной на вход категории
     * @param {*} category Категория
     */
    NeedBacklight(category) {
        // Введенное в поиск название категории
        let searchProductCategoryName = $("#SearchProductCategoryName").val();
        if (!Utils.IsNullOrEmpty(searchProductCategoryName)) {
            if (category["Name"].toLowerCase().includes(searchProductCategoryName.toLowerCase())) {
                return true;
            }
        }
        return false;
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
        super.ExpandCategory($(categoryRow).attr("data-category-id"));
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
        super.CollapseCategory($(categoryRow).attr("data-category-id"));
    }
    //#endregion

    //#region Handlers
    /** Добавление категории */
    AddCategoryBtnClick(event) {
        event.stopPropagation();
        event.preventDefault();
        $("#createProdCatName").val("");
        $("#createProdCatDesc").val("");
        Utils.ClearErrors();
        setTimeout(() => {
            $("#addCategoryModal").attr("data-root-category", false).modal();
        }, 50);
    }

    /** Изменение категории */
    UpdateCategoryBtnClick(event) {
        event.stopPropagation();
        event.preventDefault();
        Utils.ClearErrors();
        let productCategory = new ProductCategory();
        productCategory.Initialize().then(() =>
            $("#updateCategoryModal").modal());
    }

    /** Добавление продукта */
    AddProductBtnClick(event) {
        event.stopPropagation();
        event.preventDefault();
        $("#createProdName").val("");
        $("#createProdDesc").val("");
        $("#createProdCost").val("");
        Utils.ClearErrors();
        setTimeout(() => {
            $("#addProductModal").modal()
        }, 50);
    }

    /** Удаление категории */
    DeleteCategoryBtnClick(event) {
        event.stopPropagation();
        event.preventDefault();
        let productCategory = new ProductCategory();
        productCategory.Delete(event);
    }

    /** Удаление продукта */
    DeleteProductBtnClick(event) {
        event.stopPropagation();
        event.preventDefault();
        let product = new Product();
        product.Delete(event);
    }
    //#endregion
}

$("#productCategoriesForm")
    .off("click", ".nav-previous .nav-url").on("click", ".nav-previous .nav-url", event => {
        let productCategory = new ProductCategory();
        productCategory.Navigate(event);
    })
    .off("click", ".nav-next .nav-url").on("click", ".nav-next .nav-url", event => {
        let productCategory = new ProductCategory();
        productCategory.Navigate(event);
    })
    .off("click", "#prodsSearch").on("click", "#prodsSearch", event => {
        let productCategory = new ProductCategory();
        productCategory.Search(event);
    })
    .off("click", "#clearProdsSearch").on("click", "#clearProdsSearch", event => {
        let productCategory = new ProductCategory();
        productCategory.ClearSearch(event);
    })
    .off("click", "#addRootCategory").on("click", "#addRootCategory", event => {
        event.preventDefault();
        $("#createProdCatName").val("");
        $("#createProdCatDesc").val("");
        Utils.ClearErrors();
        $("#addCategoryModal").attr("data-root-category", "true").modal();
    });

$("#productCategoriesTree")
    .off("click", ".category-expand").on("click", ".category-expand", event => {
        let renderer = new ProductCategoriesRender();
        renderer.ExpandCategory(event);
    })
    .off("click", ".category-collapse").on("click", ".category-collapse", event => {
        let renderer = new ProductCategoriesRender();
        renderer.CollapseCategory(event);
    })
    .off("click", ".category-collapse").on("click", ".category-collapse", event => {
        let renderer = new ProductCategoriesRender();
        renderer.CollapseCategory(event);
    });

// Меню настроек категории
$("#prodCatSettingsMenu")
    .off("click", "#addCategoryBtn").on("click", "#addCategoryBtn", event => {
        let renderer = new ProductCategoriesRender();
        renderer.AddCategoryBtnClick(event);
    })
    .off("click", "#updateCategoryBtn").on("click", "#updateCategoryBtn", event => {
        let renderer = new ProductCategoriesRender();
        renderer.UpdateCategoryBtnClick(event);
    })
    .off("click", "#addProductBtn").on("click", "#addProductBtn", event => {
        let renderer = new ProductCategoriesRender();
        renderer.AddProductBtnClick(event);
    })
    .off("click", "#deleteCategoryBtn").on("click", "#deleteCategoryBtn", event => {
        let renderer = new ProductCategoriesRender();
        renderer.DeleteCategoryBtnClick(event);
    })
    .off("click", "input").on("click", "input", event => {
        $("#prodCatSettingsMenu").addClass("d-none");
    });

// Меню настроек продуктов
$("#prodSettingsMenu")
    .off("click", "#deleteProductBtn").on("click", "#deleteProductBtn", event => {
        let renderer = new ProductCategoriesRender();
        renderer.DeleteProductBtnClick(event);
    });