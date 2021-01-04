class EmployeePositionsManagement {
    static positionsToAdd = [];
    static positionsToRemove = [];
    static primaryPositionName = "";
    static primaryPositionChanged = false;

    /**
     * Инициализация списка должностей в модальном окне управления должностями
     * Выполняет запрос на сервер, затем вызывает метод отрисовки полей из полученных данных
     */
    InitializePositions() {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let getPositionsUrl = $("#initializePositionsUrl").attr("href");

            request.JsonGetRequest(getPositionsUrl)
                .catch(error => reject(error))
                .then(response => {
                    this.InitializePositionFields(response);
                    resolve(response);
                })
        })
    }

    /**
     * Инициализация полей в модальном окне в зависимости от типа должностей
     * @param {*} positionData ответ с сервера со списком должностей подразделения и выбранных должностей сотрудника
     */
    InitializePositionFields(positionData) {
        this.InitializeAllPositions(positionData["allPositions"]);
        this.InitializeSelectedPositions(positionData["selectedPositions"]);
    }

    /**
     * Инициализация списка со всеми должностями подразделения в модальном окне
     * @param {*} allPositions список всех должностей подразделения
     */
    InitializeAllPositions(allPositions) {
        $("#allPositions tbody").empty();
        allPositions.map(position => {
            let newPositionRow = this.GetNewAllPositionRow(position);
            $("#allPositions tbody").append(newPositionRow);
        });
    }

    /**
     * Инициализация списка с должностями сотрудника в модальном окне
     * @param {*} selectedPositions список должностей сотрудника
     */
    InitializeSelectedPositions(selectedPositions) {
        $("#selectedPositions tbody").empty();
        selectedPositions.map(position => {
            let newPositionRow = this.GetNewSelectedPositionRow(position);
            $("#selectedPositions tbody").append(newPositionRow);
        });
    }

    /**
     * Получение строки для списка с должностями подразделения в модальном окне
     * @param {*} position Объект с информацией о должности
     */
    GetNewAllPositionRow(position) {
        let parentPositionName = position["parentPositionName"];
        let newPositionRow = "";
        if (parentPositionName != null && parentPositionName.length > 0) {
            newPositionRow = "<tr>" + this.GetAllPositionsNameCell(position) +
                '<td class="label-non-select">' + position["parentPositionName"] + '</td>' +
                this.GetCheckmarkCell() + "</tr>";
        }
        else {
            newPositionRow = "<tr>" + this.GetAllPositionsNameCell(position) +
                '<td class="label-non-select"></td>' +
                this.GetCheckmarkCell() + "</tr>";
        }
        return newPositionRow;
    }

    /**
     * Получение строки для списка с должностями сотрудника в модальном окне
     * @param {*} position Объект с информацией о должности
     */
    GetNewSelectedPositionRow(position) {
        let parentPositionName = position["parentPositionName"];
        let newPositionRow = "";
        if (parentPositionName != null && parentPositionName.length > 0) {
            newPositionRow = "<tr>" + this.GetSelectedPositionsNameCell(position) +
                '<td class="label-non-select">' + position["parentPositionName"] + '</td>' +
                this.GetPrimarySignCell(position) +
                this.GetCrossCell(position) + "</tr>";
        }
        else {
            newPositionRow = "<tr>" + this.GetSelectedPositionsNameCell(position) +
                '<td class="label-non-select"></td>' +
                this.GetPrimarySignCell(position) +
                this.GetCrossCell(position) + "</tr>";
        }
        return newPositionRow;
    }

    /**
     * Возвращает ячейку с названием должности для списка всех должностей подразделения в модальном окне
     * @param {*} position Объект с информацией о должности
     */
    GetAllPositionsNameCell(position) {
        return '<td class="label-non-select position-name">' + position["name"] + '</td>';
    }
    
    /**
     * Возвращает ячейку с названием должности для списка должностей сотрудника в модальном окне
     * @param {*} position Объект с информацией о должности
     */
    GetSelectedPositionsNameCell(position) {
        return '<td class="label-non-select position-name">' + position["positionName"] + '</td>';
    }

    /**
     * Вовзращает счейку с признаком, является ли должность основной для списка должностей сотрудника
     * @param {*} position Объект с информацией о должности
     */
    GetPrimarySignCell(position) {
        if (position["isPrimary"]) {
            // Если не была изменена основная должность, запоминаем ее название
            if (!EmployeePositionsManagement.primaryPositionChanged) {
                EmployeePositionsManagement.primaryPositionName = position["positionName"];
            }
            return '<td class="readonly-checkmark"><span class="icon-checkmark"></span></td>';
        }
        return '<td class="hide-checkmark"><span class="icon-checkmark"></span></td>';
    }

    /**
     * Возвращает ячейку с кнопкой удаления должности из списка должностей сотрудника в модальном окне
     * @param {*} position Объект с информацией о должности
     */
    GetCrossCell(position) {
        if (!position["isPrimary"]) {
            return '<td class="cross"><span class="icon-close"></span></td>';
        }
        return '<td class="cross readonly-cross"><span class="icon-close"></span></td>';
    }

    /**
     * Возвращает ячейку с кнопкой добавления должности в список должностей сотрудника в модальном окне
     */
    GetCheckmarkCell() {
        return '<td class="checkmark"><span class="icon-checkmark"></span></td>';
    }

    /**
     * Выполняет запрос на сервер для синхронизации должностей(добавления и удаления из списка должностей сотрудника)
     * Затем выполняет обновление модального окна или вывод ошибок
     */
    SynchronizePositions() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let syncPositionsUrl = $("#employeePosisionModal form").attr("action");
            let syncPositionsData = this.GetSyncPositionsData();

            request.JsonPostRequest(syncPositionsUrl, syncPositionsData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["SyncPositions"]);
                    reject();
                })
                .done(() => {
                    this.ClearPositionChangesHistory();
                    this.InitializePositions()
                        .catch(error => reject(error))
                        .then(response => resolve());
                    resolve();
                })
        })
    }

    /**
     * Формирует данные для запроса на сервер при синхронизации должностей
     */
    GetSyncPositionsData() {
        let checkmark = $("#selectedPositions .readonly-checkmark")[0];
        let primaryPositionName = EmployeePositionsManagement.primaryPositionName == "" ? $(checkmark).closest("tr").find(".position-name").text() : EmployeePositionsManagement.primaryPositionName;

        return {
            EmployeeId: $("#employeeId").val(),
            PrimaryPositionName: primaryPositionName,
            PositionsToAdd: EmployeePositionsManagement.positionsToAdd,
            PositionsToRemove: EmployeePositionsManagement.positionsToRemove
        }
    }

    /**
     * Поиск по списку всех должностей подразделения
     * @param {*} event 
     */
    AllPositionsSearch(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let allPositionsUrl = $(event.currentTarget).attr("data-href");
            let allPositionsSearchData = this.AllPositionsSearchGetData();

            request.CommonPostRequest(allPositionsUrl, allPositionsSearchData).done(response => {
                this.RenderPositionsFilterFields(response);
                this.RenderAllPositionsList(response["allPositions"]);
                resolve();
            });
        })
    }

    /**
     * Получает данные для отправки на сервер при поиске по списку всех должностей подразделения
     */
    AllPositionsSearchGetData() {
        return {
            Id: $("#employeeId").val(),
            DivisionId: $("#DivisionId").val(),
            OrganizationId: $("#OrganizationId").val(),
            SearchAllPosName: $("#SearchAllPosName").val(),
            SearchAllParentPosName: $("#SearchAllParentPosName").val(),
        }
    }
    
    /**
     * Сброс поиска по списку всех должностей подразделения
     * @param {*} event 
     */
    ClearAllPositionsSearch(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderPositionsFilterFields(response);
                this.RenderAllPositionsList(response["allPositions"]);
                resolve();
            });
        })
    }

    /**
     * Поиск по списку должностей сотрудлника
     * @param {*} event
     */
    SelectedPositionsSearch(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let selectedPositionsUrl = $(event.currentTarget).attr("data-href");
            let selectedPositionsSearchData = this.SelectedPositionsSearchGetData();

            request.CommonPostRequest(selectedPositionsUrl, selectedPositionsSearchData).done(response => {
                this.RenderPositionsFilterFields(response);
                this.RenderSelectedPositionsList(response["selectedPositions"]);
                resolve();
            });
        })
    }

    /**
     * Получает данные для отправки на сервер при поиске по списку всех должностей сотрудника
     */
    SelectedPositionsSearchGetData() {
        return {
            Id: $("#employeeId").val(),
            DivisionId: $("#DivisionId").val(),
            OrganizationId: $("#OrganizationId").val(),
            SearchSelectedPosName: $("#SearchSelectedPosName").val(),
            SearchSelectedParentPosName: $("#SearchSelectedParentPosName").val(),
        }
    }

    /**
     * Сброс поиска по списку всех должностей сотрудлника
     * @param {*} event 
     */
    ClearSelectedPositionsSearch(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderPositionsFilterFields(response);
                this.RenderSelectedPositionsList(response["selectedPositions"]);
                resolve();
            });
        })
    }

    /**
     * Заполняет поля фильтров по спискам должностей значениями(в модальном окне урпавления должностями)
     * @param {*} response 
     */
    RenderPositionsFilterFields(response) {
        let allPositionsVM = response["allPositionsVM"];
        let selectedPositionsVM = response["selectedPositionsVM"];
        $("#SearchAllPosName").val(allPositionsVM["searchAllPosName"]);
        $("#SearchAllParentPosName").val(allPositionsVM["searchAllParentPosName"]);
        $("#SearchSelectedPosName").val(selectedPositionsVM["searchSelectedPosName"]);
        $("#SearchSelectedParentPosName").val(selectedPositionsVM["searchSelectedParentPosName"]);
    }

    /**
     * Действие при промотке списка со всеми должностями вперед
     * @param {*} event 
     */
    NextAllPositions(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderAllPositionsList(response);
                resolve();
            });
        })
    }
    
    /**
     * Действие при промотке списка со всеми должностями назад
     * @param {*} event 
     */
    PreviousAllPositions(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderAllPositionsList(response);
                resolve();
            });
        })
    }

    /**
     * Отрисовывает список всех должностей
     * @param {*} allPositions список всех должностей подразделения
     */
    RenderAllPositionsList(allPositions) {
        this.InitializeAllPositions(allPositions);
        this.RestorePositionsToAdd();
    }

    /**
     * При нажатии на кнопку добавления должности
     * @param {*} event 
     */
    OnPositionAddBtnClick(event) {
        let row = $(event.currentTarget).closest("tr");
        let positionName = $(row).find(".position-name").text();
        if (!EmployeePositionsManagement.positionsToAdd.includes(positionName)) {
            EmployeePositionsManagement.positionsToAdd.push(positionName);
        }
        else {
            let positionIndex = EmployeePositionsManagement.positionsToAdd.indexOf(positionName);
            if (positionIndex != -1) {
                EmployeePositionsManagement.positionsToAdd.splice(positionIndex, 1);
            }
        }
    }

    /**
     * При нажатии на кнопку удаления должности
     * @param {*} event 
     */
    OnPositionRemoveBtnClick(event) {
        let row = $(event.currentTarget).closest("tr");
        let positionName = $(row).find(".position-name").text();
        if (!$(event.currentTarget).hasClass("readonly-cross")) {
            if (!EmployeePositionsManagement.positionsToRemove.includes(positionName)) {
                EmployeePositionsManagement.positionsToRemove.push(positionName);
            }
            else {
                let positionIndex = EmployeePositionsManagement.positionsToRemove.indexOf(positionName);
                if (positionIndex != -1) {
                    EmployeePositionsManagement.positionsToRemove.splice(positionIndex, 1);
                }
            }
        }
    }

    /**
     * Восстанавливает выбранные для добавления должности после промотки
     */
    RestorePositionsToAdd() {
        EmployeePositionsManagement.positionsToAdd.map(positionName => {
            $("#employeePosisionModal #allPositions tbody .position-name").each((index, positionNameElement) => {
                let row = $(positionNameElement).closest("tr");
                if ($(positionNameElement).text() == positionName) {
                    $(row).find(".checkmark").addClass("checkmark-checked");
                }
            })
        });
    }
    
    /**
     * Действие при промотке списка с должностями сотрудника вперед
     * @param {*} event 
     */
    NextSelectedPositions(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderSelectedPositionsList(response);
                resolve();
            });
        })
    }
    
    /**
     * Действие при промотке списка с должностями сотрудника назад
     * @param {*} event 
     */
    PreviousSelectedPositions(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderSelectedPositionsList(response);
                resolve();
            });
        })
    }

    /**
     * Отрисоывает список должностей сотрудника
     * @param {*} selectedPositions список должностей сотрудника
     */
    RenderSelectedPositionsList(selectedPositions) {
        this.InitializeSelectedPositions(selectedPositions);
        this.RestorePositionsToRemove();
        this.RestorePrimaryPosition();
    }

    /** Событие, происходящее при изменении основной должности */
    OnPrimaryPositionChange() {
        let checkmark = $("#employeePosisionModal #selectedPositions tbody .readonly-checkmark");
        if (checkmark.length != 0) {
            let row = $(checkmark).closest("tr");
            let primaryPositionName = $(row).find(".position-name").text();
            EmployeePositionsManagement.primaryPositionName = primaryPositionName;
            let positionToRemoveIndex = EmployeePositionsManagement.positionsToRemove.indexOf(primaryPositionName);
            if (positionToRemoveIndex != -1) {
                EmployeePositionsManagement.positionsToRemove.splice(positionToRemoveIndex, 1);
            }
        }
    }

    /**
     * Восстанавливает выбранные для удаления должности после промотки
     */
    RestorePositionsToRemove() {
        EmployeePositionsManagement.positionsToRemove.map(positionName => {
            $("#employeePosisionModal #selectedPositions tbody .position-name").each((index, positionNameElement) => {
                let row = $(positionNameElement).closest("tr");
                if ($(positionNameElement).text() == positionName) {
                    $(row).find(".cross").addClass("cross-crossed");
                }
            })
        });
    }

    /**
     * Восстанавливает основную выбранную должность
     */
    RestorePrimaryPosition() {
        $("#employeePosisionModal #selectedPositions tbody .position-name").each((index, positionNameElement) => {
            let positionName = $(positionNameElement).text();
            let row = $(positionNameElement).closest("tr");

            // Проставление галочки на основной должности
            if (positionName == EmployeePositionsManagement.primaryPositionName) {
                $(row).find(".hide-checkmark").removeClass("hide-checkmark").addClass("readonly-checkmark");
                $(row).find(".cross").addClass("readonly-cross");
            }

            // Снятие галочек с остальных должностей
            else {
                $(row).find(".readonly-checkmark").removeClass("readonly-checkmark").addClass("hide-checkmark");
                $(row).find(".readonly-cross").removeClass("readonly-cross");
            }
        });
    }

    /**
     * Получает записи для списка должностей
     * @param {*} event 
     */
    GetRecords(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let url = $(event.currentTarget).attr("data-href");

            request.JsonGetRequest(url)
                .fail(error => reject(error))
                .done(response => resolve(response));
        })
    }

    /**
     * Обработка события при закрытии модального окна управления должностями сотрудника
     * @param {*} event 
     */
    OnPositionModalClosed(event) {
        // Если есть запрет на закрытие модельного окна и существуют незафиксированные изменения
        if (this.ExistsPositionChanges()) {
            event.preventDefault();

            // Запрос подтверждения на закрытие, в случае успеха обнуление переменных и закрытие окна
            Swal.fire(MessageManager.Invoke("PositionModalClosedConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    this.ClearPositionManagementSearch().then(() => {
                        this.ClearPositionChangesHistory();
                        $(event.currentTarget).modal("hide");
                    });
                }
            });
        }

        // Иначе осуществляется перезагрузка страницы
        else {
            this.ClearPositionManagementSearch().then(() => location.reload());
        }
    }

    /**
     * Метод возвращает значение, указывающее, произошли ли изменения при изменении должностей
     */
    ExistsPositionChanges() {
        let modal = $("#employeePosisionModal");
        let allPositions = $(modal).find("#allPositions");
        let selectedPositions = $(modal).find("#selectedPositions");
        let positionsToAdd = $(allPositions).find(".checkmark-checked");
        let positionsToRemove = $(selectedPositions).find(".cross-crossed");
        return EmployeePositionsManagement.positionsToAdd.length > 0 || positionsToAdd.length > 0 || 
            EmployeePositionsManagement.positionsToRemove.length > 0 || positionsToRemove.length > 0 ||
            EmployeePositionsManagement.primaryPositionChanged == true;
    }
    
    /**
     * Очищает информации об изменениях, произошедших при работе с должностями сотрудника
     */
    ClearPositionChangesHistory() {
        EmployeePositionsManagement.positionsToAdd = [];
        EmployeePositionsManagement.positionsToRemove = [];
        EmployeePositionsManagement.primaryPositionName = "";
        EmployeePositionsManagement.primaryPositionChanged = false;
        let modal = $("#employeePosisionModal");
        let allPositions = $(modal).find("#allPositions");
        $(allPositions).find(".checkmark-checked").removeClass("checkmark-checked");
        let selectedPositions = $(modal).find("#selectedPositions");
        $(selectedPositions).find(".cross-crossed").removeClass("cross-crossed");
    }

    /**
     * Метод осуществляет запрос на бек, очищая фильтр по должностям
     */
    ClearPositionManagementSearch() {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let clearPositionSearchUrl = Localization.GetUri("clearPositionSearch");

            request.CommonGetRequest(clearPositionSearchUrl)
                .fail(() => reject(error))
                .done(() => resolve())
        })
    }
}

// Модальное окно управления должностями сотрудника
$("#employeePosisionModal")
    .off("click", "#syncEmpPositionsBtn").on("click", "#syncEmpPositionsBtn", event => {
        event.preventDefault();
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.SynchronizePositions();
    })
    .off("checkmark-check", ".checkmark").on("checkmark-check", ".checkmark", event => {
        let button = new Button();
        button.CheckmarkCheck(event);
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.OnPositionAddBtnClick(event);
    })
    .off("hide-checkmark-click", ".hide-checkmark").on("hide-checkmark-click", ".hide-checkmark", event => {
        let button = new Button();
        button.HideCheckmarkCheck(event);
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.OnPrimaryPositionChange();
        EmployeePositionsManagement.primaryPositionChanged = true;
    })
    .off("cross-click", ".cross").on("cross-click", ".cross", event => {
        let button = new Button();
        button.CrossClick(event);
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.OnPositionRemoveBtnClick(event);
    })
    .off("click", "#allPositionsNav .nav-next .nav-url").on("click", "#allPositionsNav .nav-next .nav-url", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.NextAllPositions(event);
    })
    .off("click", "#allPositionsNav .nav-previous .nav-url").on("click", "#allPositionsNav .nav-previous .nav-url", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.PreviousAllPositions(event);
    })
    .off("click", "#selectedPositionsNav .nav-next .nav-url").on("click", "#selectedPositionsNav .nav-next .nav-url", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.NextSelectedPositions(event);
    })
    .off("click", "#selectedPositionsNav .nav-previous .nav-url").on("click", "#selectedPositionsNav .nav-previous .nav-url", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.PreviousSelectedPositions(event);
    })
    .off("click", "#allPositionsSearch").on("click", "#allPositionsSearch", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.AllPositionsSearch(event);
    })
    .off("click", "#selectedPositionsSearch").on("click", "#selectedPositionsSearch", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.SelectedPositionsSearch(event);
    })
    .off("click", "#clearAllPositionsSearch").on("click", "#clearAllPositionsSearch", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.ClearAllPositionsSearch(event);
    })
    .off("click", "#clearSelectedPositionsSearch").on("click", "#clearSelectedPositionsSearch", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.ClearSelectedPositionsSearch(event);
    })
    .off('hide.bs.modal').on("hide.bs.modal", event => {
        let employeePositionsManagement = new EmployeePositionsManagement();
        employeePositionsManagement.OnPositionModalClosed(event);
    });