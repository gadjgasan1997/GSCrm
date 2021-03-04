class ProductCategory {
    //#region Actions
    Initialize() {
        return new Promise((resolve, reject) => {
            let sourceId = $("#prodCatSettingsMenu").attr("data-source-id");
            let getCategoryUrl = LocalizationManager.GetUri("initalizeProductCategory").replace("{id}", sourceId);
            let request = new AjaxRequests();
            request.JsonGetRequest(getCategoryUrl).then(response => {

                // Простановка пришедших параметров
                if (!Utils.IsNullOrEmpty(response)) {
                    $("#updateProdCatName").val(response["name"]);
                    $("#updateProdCatDesc").val(response["description"]);
                    resolve();
                }

                // Иначе сообщение об ошибке
                else Swal.fire(MessageManager.Invoke("ProductCategoryInitializeError"));
            })
        })
    } 

    /** Создание подкатегории */
    CreateSubCategory() {
        this.CreateCategory(this.CreateGetSubData());
    }

    CreateGetSubData() {
        let createData = this.CreateGetRootData();
        createData["ParentProductCategoryId"] = $("#prodCatSettingsMenu").attr("data-source-id");
        return createData;
    }

    /** Создание корневой категории */
    CreateRootCategory() {
        this.CreateCategory(this.CreateGetRootData());
    }

    CreateGetRootData() {
        return {
            OrganizationId: $("#organizationId").val(),
            Name: $("#createProdCatName").val(),
            Description: $("#createProdCatDesc").val()
        }
    }

    CreateCategory(createCategoryData) {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let modal = $("#addCategoryModal");
            let createCategoryUrl = $(modal).find("form").attr("action");
            let request = new AjaxRequests();
            let hasErrors = false;
            request.CommonPostRequest(createCategoryUrl, createCategoryData)
                .catch(response => {
                    hasErrors = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateProductCategory"]);
                    reject();
                })
                .then(response => {
                    if (!hasErrors) {
                        // При добавлении элемента в нераскрытую категорию ее необходимо развернуть
                        let categoryId = createCategoryData["ParentProductCategoryId"];
                        if (!Utils.IsNullOrEmpty(categoryId)) {
                            let baseRenderer = new BaseProductCategoriesRender();
                            baseRenderer.ExpandCategory(categoryId);
                        }

                        // Закрытие окна и перерендеринг представления
                        let renderer = new ProductCategoriesRender();
                        $("#addCategoryModal").modal("hide");
                        renderer.RenderCategories(response["ProductCategoryViewModels"]);
                    }
                });
        })
    }

    Update(event) {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let updateCategoryUrl = $("#updateCategoryModal").find("form").attr("action");
            let updateCategoryData = this.UpdateGetData();
            let request = new AjaxRequests();
            let hasErrors = false;
            request.CommonPostRequest(updateCategoryUrl, updateCategoryData)
                .catch(response => {
                    hasErrors = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdateProductCategory"]);
                    reject();
                })
                .then(response => {
                    if (!hasErrors) {
                        // Закрытие окна и перерендеринг представления
                        $("#updateCategoryModal").modal("hide");
                        let renderer = new ProductCategoriesRender();
                        renderer.RenderCategories(response["ProductCategoryViewModels"]);
                    }
                });
        })
    }

    UpdateGetData() {
        return {
            Id: $("#prodCatSettingsMenu").attr("data-source-id"),
            OrganizationId: $("#organizationId").val(),
            Name: $("#updateProdCatName").val(),
            Description: $("#updateProdCatDesc").val()
        }
    }

    Delete(event) {
        return new Promise((resolve, reject) => {
            // Диалоговое окно с предупреждением
            Swal.fire(MessageManager.Invoke("RemoveCategoryConfirmation")).then(dialogResult => {
                if (dialogResult.value) {

                    // Получение ссылки на удаление и запрос
                    let deleteCategoryUrl = $(event.currentTarget).closest("form").attr("action") +
                        "/" + $("#prodCatSettingsMenu").attr("data-source-id");
                    let request = new AjaxRequests();
                    let hasErrors = false;
                    request.CommonDeleteRequest(deleteCategoryUrl)
                        .catch(response => {
                            hasErrors = true;
                            Utils.DefaultErrorHandling(response["responseJSON"]);
                            reject();
                        })
                        .then(productCategoriesUrl => {
                            if (!hasErrors) {
                                // Перерендеринг представления
                                let renderer = new ProductCategoriesRender();
                                renderer.RenderCategoriesRequest(productCategoriesUrl);
                            }
                        })
                }
            })
        })
    }

    /** Метод проставляет признак необходимости сохранить кеш категорий продуктов после перезагрузки страницы */
    SetSaveCategoriesCacheSign(needSave) {
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (Utils.IsNullOrEmpty(categoriesCash)) {
            categoriesCash = {
                SaveCategoriesCache: needSave
            }
        }
        else categoriesCash["SaveCategoriesCache"] = needSave;
        localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
    }

    /** Метод чистит лишний кеш при необходимости */
    ClearCache() {
        let categoriesCash = JSON.parse(localStorage.getItem("ProductCategoriesCash"));
        if (!Utils.IsNullOrEmpty(categoriesCash)) {

            // Получение признака необходимости сохранить кеш
            // Если признак отсутствует или проставлен в false, происходит чистка
            let saveCategoriesCache = categoriesCash["SaveCategoriesCache"];
            if (Utils.IsNullOrEmpty(saveCategoriesCache) || !saveCategoriesCache) {
                localStorage.removeItem("ProductCategoriesCash");
            }
            // Иначе чистка не происходит и признак проставляется в false
            else {
                categoriesCash["SaveCategoriesCache"] = false;
                localStorage.setItem("ProductCategoriesCash", JSON.stringify(categoriesCash));
            }
        }
    }
    
    Navigate(event) {
        event.preventDefault();
        let productCategoriesUrl = $(event.currentTarget).attr("data-href");
        let renderer = new ProductCategoriesRender();
        renderer.RenderCategoriesRequest(productCategoriesUrl);
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
        
        // Запрос на применение поиска
        request.CommonPostRequest(searchUrl, searchData)
            .catch(response => {
                Utils.DefaultErrorHandling(response["responseJSON"]);  
            })
            .then(response => {

                // Перерендеринг представления
                let renderer = new ProductCategoriesRender();
                renderer.RenderCategories(response["ProductCategoryViewModels"]);
            })
    }

    SearchGetData() {
        return {
            Id: $("#OrganizationId").val(),
            OrganizationId: $("#OrganizationId").val(),
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
            $("#SearchProductCategoryName").val("");
            $("#SearchProductName").val("");
            $("#prodMinCost").val("");
            $("#prodMaxCost").val("");
            let renderer = new ProductCategoriesRender();
            renderer.RenderCategories(response["ProductCategoryViewModels"]);
        })
    }
    //#endregion

    //#region Handlers
    CreateProdCatBtnClick(event) {
        event.preventDefault();
        let isRoot = $("#addCategoryModal").attr("data-root-category");
        if (isRoot == "true") {
            this.CreateRootCategory();
        }
        else this.CreateSubCategory();
    }
    //#endregion
}

// Окно добавления категории
$("#addCategoryModal")
    .off("click", "#createProdCatBtn").on("click", "#createProdCatBtn", event => {
        let productCategory = new ProductCategory();
        productCategory.CreateProdCatBtnClick(event);
    })
    .off("click", "#cancelCreationProdCatBtn").on("click", "#cancelCreationProdCatBtn", event => {
        event.preventDefault();
        $("#addCategoryModal").modal();
    })

// Окно изменения категории
$("#updateCategoryModal")
    .off("click", "#updateProdCatBtn").on("click", "#updateProdCatBtn", event => {
        event.preventDefault();
        let productCategory = new ProductCategory();
        productCategory.Update();
    })
    .off("click", "#cancelUpdateProdCatBtn").on("click", "#cancelUpdateProdCatBtn", event => {
        event.preventDefault();
        $("#updateCategoryModal").modal();
    })