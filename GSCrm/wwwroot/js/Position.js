class Position {
    Create() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let createPosUrl = $("#positionModal form").attr("action");
            let createPosData = this.GetCreationData();

            request.JsonPostRequest(createPosUrl, createPosData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreatePosition"]);
                })
                .done(() => {
                    $("#positionModal").modal("hide");
                    location.reload();
                });
        })
    }

    GetCreationData() {
        return {
            Name: $("#positionName").val(),
            OrganizationId: $("#orgId").val(),
            DivisionName: BaseAutocomplete.GetValue($("#positionDivModal")),
            ParentPositionName: BaseAutocomplete.GetValue($("#parentPositionModal")),
            PrimaryEmployeeId: $("#positionModal #primaryEmployeeId").val(),
            PrimaryEmployeeInitialName: $("#positionModal #primaryEmployeeModalVal").val()
        }
    }

    /**
     * Удаление должности
     * @param {*} event 
     */
    Remove(event) {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemovePositionConfirmation")).then(result => {
                if (result.value) {
                    let request = new AjaxRequests();
                    let removePosUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");
        
                    request.CommonDeleteRequest(removePosUrl)
                        .fail(response => {
                            Utils.DefaultErrorHandling(response["responseJSON"]);
                            reject();
                        })
                        .done(() => location.reload());
                }
            });
        })
    }

    Cancel() {
        this.ClearFields();
        Utils.ClearErrors();
        $("#positionModal").modal("hide");
    }

    /**
     * Запрашивает у пользователя подтверждение на обновление подразделения
     * Выполняет запрос на сервер, меняя подразделение
     * При наличии ошибок обрабатывает их
     */
    ChangeDivision() {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("ChangePosDivisionConfirmation")).then(result => {
                Utils.ClearErrors();
                if (result.value) {
                    let request = new AjaxRequests();
                    let changeDivisionUrl = $("#changePositionDivisionModal form").attr("action");
                    let changeDivisionData = this.ChangeDivisionGetData();
                    
                    request.JsonPostRequest(changeDivisionUrl, changeDivisionData)
                        .fail(response => {
                            Utils.CommonErrosHandling(response["responseJSON"], ["ChangePosDivision"]);
                        })
                        .done(() => location.reload())
                }
            });
        })
    }
    
    /**
     * Формирует данные для отправки на сервер при смене подразделения
     */
    ChangeDivisionGetData() {
        return {
            Id: $("#positionId").val(),
            OrganizationId: $("#OrganizationId").val(),
            DivisionName: BaseAutocomplete.GetValue($("#positionNewDiv"))
        }
    }

    ClearFields() {
        $("#positionName").val("");
        $("#positionDivModal .autocomplete-input").val("");
        $("#parentPositionModal .autocomplete-input").val("");
    }

    /**
     * Обновление должности
     * @param {*} event 
     */
    Update(event) {
        return new Promise((resolve, reject) => {
            let updatePosUrl = $(event.currentTarget).closest("#positionForm").find("form").attr("action");
            let updatePosData = this.UpdateGetData();
            let request = new AjaxRequests();
            request.JsonPostRequest(updatePosUrl, updatePosData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdatePosition"]);
                })
                .done(updatedPosUrl => location.replace(location.origin + updatedPosUrl))
        });
    }

    UpdateGetData() {
        return {
            Id: $("#positionId").val(),
            Name: $("#updatePosName").val(),
            DivisionName: $("#positionDiv").val(),
            ParentPositionName: $("#ParentPositionName").val(),
            PrimaryEmployeeId: $("#primaryEmployeeId").val(),
            PrimaryEmployeeInitialName: $("#PrimaryEmployeeInitialName").val(),
            OrganizationId: $("#OrganizationId").val()
        }
    }

    static RedirectToPosition(event) {
        let target = $(event.currentTarget);
        if (!target.hasClass("position-current-item")) {
            let selectedPositionUrl = target.find(".position-hierarchy-link a").attr("href");
            let request = new AjaxRequests();
            request.CommonGetRequest(selectedPositionUrl);
        }
    }
}

// Список должностей
$("#positionsList").off("click", ".remove-item-btn").on("click", ".remove-item-btn", event => {
    event.preventDefault();
    let position = new Position();
    position.Remove(event);
})

$("#changePositionDivisionModal").off("click", "#changeDivisionBtn").on("click", "#changeDivisionBtn", event => {
    let position = new Position();
    position.ChangeDivision();
});

// Модальное окно создания должности
$("#positionModal")
    .off("click", "#createPositionBtn").on("click", "#createPositionBtn", event => {
        event.preventDefault();
        let position = new Position();
        position.Create();
    })
    .off("click", "#cancelCreationPostitionBtn").on("click", "#cancelCreationPostitionBtn", event => {
        event.preventDefault();
        let position = new Position();
        position.Cancel();
    });

$("#positionForm")
    .off("click", "#updatePosBtn").on("click", "#updatePosBtn", event => {
        event.preventDefault();
        let position = new Position();
        position.Update(event);
    })
    .off("click", "#posTabs .nav-item").on("click", "#posTabs .nav-item", event => {
        let navTab = new NavTab();
        navTab.Remember(event, "currentPosTab");
    })