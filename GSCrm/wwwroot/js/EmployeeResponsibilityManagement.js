class EmployeeResponsibilityManagement {
    static responsibilitiesToAdd = [];
    static responsibilitiesToRemove = [];

    /** Метод инициализирует попап управления полномочиями сотрудника */
    InitializeResps() {
        return new Promise((resolve, reject) => {
            let empRespsUrl = $("#employeeForm").find("#initializeRespsUrl").attr("href");
            let request = new AjaxRequests();
            request.CommonGetRequest(empRespsUrl)
                .fail(error => reject(error))
                .done(response => {
                    this.ResetLists();
                    this.Render(response);
                    resolve(response);
                });
        });
    }

    /** Метод сбрасывает заполненные списки */
    ResetLists() {
        $("#empRespManagement #allResps tbody").empty();
        $("#empRespManagement #selectedResps tbody").empty();
    }

    /**
     * Метод отрисовывает модальное окно при иницилазиации
     * @param {*} response 
     */
    Render(response) {
        this.RenderAllResps(response["allResponsibilitiesVMs"]);
        this.RenderSelectedResps(response["selectedResponsibilitiesVMs"]);
    }

    /**
     * Метод отрисовывает список со всеми полномочиями организации
     * @param {*} allResps 
     */
    RenderAllResps(allResps) {
        allResps.map(responsibility => {
            $("#allResps tbody").append("<tr>" +
                "<td class='d-none responsibility-id'>" + responsibility["id"] + "</td>" +
                "<td>" + responsibility["name"] + "</td>" +
                "<td class='checkmark'><span class='icon-checkmark'></span></td></tr>")
        });
    }

    /**
     * Метод отрисовывает список с выбранными полномочиями сотрудника
     * @param {*} selectedResps 
     */
    RenderSelectedResps(selectedResps) {
        selectedResps.map(responsibility => {
            $("#selectedResps tbody").append("<tr>" +
                "<td class='d-none responsibility-id'>" + responsibility["id"] + "</td>" +
                "<td>" + responsibility["name"] + "</td>" +
                "<td class='cross'><span class='icon-close'></span></td></tr>")
        });
    }

    /**
     * Метод заполняет поля фильтров пришедшими с сервера значениями
     * @param {*} response 
     */
    RenderSearchFields(response) {
        let allRespsModel = response["allResponsibilitiesVM"];
        let selectedRespsModel = response["selectedResponsibilitiesVM"];
        $("#SearchAllRespName").val(allRespsModel["searchAllRespName"]);
        $("#SearchSelectedRespName").val(selectedRespsModel["searchSelectedRespName"]);
    }

    /**
     * Метод получает записи для модального окна управления полномочиями
     * @param {*} event 
     */
    GetRecords(event) {
        return new Promise((resolve, reject) => {
            let empRespsUrl = $(event.currentTarget).attr("data-href");
            let request = new AjaxRequests();
            request.JsonGetRequest(empRespsUrl)
                .fail(error => reject(error))
                .done(response => resolve(response));
        })
    }

    /**
     * Промотка вперед всех полномочий организации
     * @param {*} event 
     */
    NextAllResps(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                $("#allResps tbody").empty();
                this.RenderAllResps(response);
                this.RestoreResponsibilitiesToAdd();
            });
        });
    }

    /**
     * Промотка назад всех полномочий организации
     * @param {*} event 
     */
    PreviousAllResps(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                $("#allResps tbody").empty();
                this.RenderAllResps(response);
                this.RestoreResponsibilitiesToAdd();
            });
        });
    }
    
    /**
     * Промотка вперед выбранных полномочий сотрудника
     * @param {*} event 
     */
    NextSelectedResps(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                $("#selectedResps tbody").empty();
                this.RenderSelectedResps(response);
                this.RestoreResponsibilitiesToRemove();
            });
        });
    }

    /**
     * Промотка назад выбранных полномочий сотрудника
     * @param {*} event 
     */
    PreviousSelectedResps(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                $("#selectedResps tbody").empty();
                this.RenderSelectedResps(response);
                this.RestoreResponsibilitiesToRemove();
            });
        });
    }

    /**
     * Поиск по всем полномочиям организации
     * @param {*} event 
     */
    AllRespsSearch(event) {
        return new Promise((resolve, reject) => {
            let allRespsSearchUrl = $(event.currentTarget).attr("data-href");
            let allRespsSearchData = this.GetAllRespsSearchData();
            let request = new AjaxRequests();

            request.CommonPostRequest(allRespsSearchUrl, allRespsSearchData)
                .fail(error => reject(error))
                .done(response => {
                    $("#allResps tbody").empty();
                    this.RenderAllResps(response["allResponsibilitiesVMs"]);
                    this.RenderSearchFields(response);
                    resolve();
                })
        });
    }

    /** Возвращает данные, необходимые при поиске по всем полномочиям организации */
    GetAllRespsSearchData() {
        return {
            SearchAllRespName: $("#SearchAllRespName").val()
        }
    }

    /**
     * Очистка поиска по всем полномочиям организации
     * @param {*} event 
     */
    ClearAllRespsSearch(event) {
        return new Promise((resolve, reject) => {
            let clearAllRespsSearchUrl = $(event.currentTarget).attr("data-href");
            let request = new AjaxRequests();

            request.CommonGetRequest(clearAllRespsSearchUrl)
                .fail(error => reject(error))
                .done(response => {
                    $("#allResps tbody").empty();
                    this.RenderAllResps(response["allResponsibilitiesVMs"]);
                    this.RenderSearchFields(response);
                    resolve();
                })
        });
    }

    /**
     * Поиск по выбранным полномочиям сотрудника
     * @param {*} event 
     */
    SelectedRespsSearch(event) {
        return new Promise((resolve, reject) => {
            let selectedRespsSearchUrl = $(event.currentTarget).attr("data-href");
            let selectedRespsSearchData = this.GetSelectedRespsSearchData();
            let request = new AjaxRequests();

            request.CommonPostRequest(selectedRespsSearchUrl, selectedRespsSearchData)
                .fail(error => reject(error))
                .done(response => {
                    $("#selectedResps tbody").empty();
                    this.RenderSelectedResps(response["selectedResponsibilitiesVMs"]);
                    resolve();
                })
        });
    }

    /** Возвращает данные, необходимые при поиске по выбранным полномочиям сотрудника */
    GetSelectedRespsSearchData() {
        return {
            SearchSelectedRespName: $("#SearchSelectedRespName").val()
        }
    }

    /**
     * Очистка поиска по выбранным полномочиям сотрудника
     * @param {*} event 
     */
    ClearSelectedRespsSearch(event) {
        return new Promise((resolve, reject) => {
            let clearSelectedRespsSearchUrl = $(event.currentTarget).attr("data-href");
            let request = new AjaxRequests();

            request.CommonGetRequest(clearSelectedRespsSearchUrl)
                .fail(error => reject(error))
                .done(response => {
                    $("#selectedResps tbody").empty();
                    this.RenderSelectedResps(response["selectedResponsibilitiesVMs"]);
                    this.RenderSearchFields(response);
                    resolve();
                })
        });
    }
    
    /** Метод восстанавливает полномочия, выбраных для добавления */
    RestoreResponsibilitiesToAdd() {
        EmployeeResponsibilityManagement.responsibilitiesToAdd.map(responsibilitId => {
            $("#empRespManagement #allResps tbody .responsibility-id").each((index, responsibilitIdElement) => {
                let row = $(responsibilitIdElement).closest("tr");
                if ($(responsibilitIdElement).text() == responsibilitId) {
                    $(row).find(".checkmark").addClass("checkmark-checked");
                }
            });
        });
    }
    
    /** Метод восстанавливает полномочия, выбраных для удаления */
    RestoreResponsibilitiesToRemove() {
        EmployeeResponsibilityManagement.responsibilitiesToRemove.map(responsibilitId => {
            $("#empRespManagement #selectedResps tbody .responsibility-id").each((index, responsibilitIdElement) => {
                let row = $(responsibilitIdElement).closest("tr");
                if ($(responsibilitIdElement).text() == responsibilitId) {
                    $(row).find(".cross").addClass("cross-crossed");
                }
            });
        });
    }

    /** Метод проверяет, существуют ли незафиксированные изменения */
    ExistsEmpRespChanges() {
        return EmployeeResponsibilityManagement.responsibilitiesToAdd.length > 0 || EmployeeResponsibilityManagement.responsibilitiesToRemove.length > 0;
    }

    /** Очистка фильтрации в модальном окне управления полномочиями */
    ClearEmpRespManagementSearch() {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let clearEmpRespSearch = LocalizationManager.GetUri("clearEmpRespSearch");
            request.CommonGetRequest(clearEmpRespSearch)
                .fail(() => reject(error))
                .done(() => resolve())
        })
    }

    /** Метод очищает историю изменений в модальном окне управления полномочиями */
    ClearEmpRespsChangesHistory() {
        EmployeeResponsibilityManagement.responsibilitiesToAdd = [];
        EmployeeResponsibilityManagement.responsibilitiesToRemove = [];
    }

    /**
     * При нажатии на кнопку добавления полномочия
     * @param {*} event 
     */
    OnResponsibilityAddBtnClick(event) {
        let row = $(event.currentTarget).closest("tr");
        let responsibilitId = $(row).find(".responsibility-id").text();
        if (!EmployeeResponsibilityManagement.responsibilitiesToAdd.includes(responsibilitId)) {
            EmployeeResponsibilityManagement.responsibilitiesToAdd.push(responsibilitId);
        }
        else {
            let responsibilityIndex = EmployeeResponsibilityManagement.responsibilitiesToAdd.indexOf(responsibilitId);
            if (responsibilityIndex != -1) {
                EmployeeResponsibilityManagement.responsibilitiesToAdd.splice(responsibilityIndex, 1);
            }
        }
    }

    /**
     * При нажатии на кнопку удаления полномочия
     * @param {*} event 
     */
    OnResponsibilityRemoveBtnClick(event) {
        let row = $(event.currentTarget).closest("tr");
        let responsibilitId = $(row).find(".responsibility-id").text();
        if (!EmployeeResponsibilityManagement.responsibilitiesToRemove.includes(responsibilitId)) {
            EmployeeResponsibilityManagement.responsibilitiesToRemove.push(responsibilitId);
        }
        else {
            let responsibilityIndex = EmployeeResponsibilityManagement.responsibilitiesToRemove.indexOf(responsibilitId);
            if (responsibilityIndex != -1) {
                EmployeeResponsibilityManagement.responsibilitiesToRemove.splice(responsibilityIndex, 1);
            }
        }
    }

    /**
     * Событие, происходящее при закрытии модального окна
     * @param {*} event 
     */
    OnEmpRespModalClosed(event) {
        // Если есть запрет на закрытие модельного окна и существуют незафиксированные изменения
        if (this.ExistsEmpRespChanges()) {
            event.preventDefault();

            // Запрос подтверждения на закрытие, в случае успеха обнуление переменных и закрытие окна
            Swal.fire(MessageManager.Invoke("NotCommitModalClosedConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    this.ClearEmpRespManagementSearch().then(() => {
                        this.ClearEmpRespsChangesHistory();
                        $(event.currentTarget).modal("hide");
                    });
                }
            });
        }

        // Иначе осуществляется перезагрузка страницы
        else {
            this.ClearEmpRespManagementSearch().then(() => location.reload());
        }
    }

    /**
     * Метод выполняет синхронизацию полномочий
     * @param {*} event 
     */
    SynchronizeResponsibilities(event) {
        return new Promise((resolve, reject) => {
            let modal = $(event.currentTarget).closest("#empRespManagement");
            let syncRespsUrl = $(modal).find("form").attr("action");
            let syncRespsData = this.SynchronizeRespsGetData();
            let request = new AjaxRequests();

            request.JsonPostRequest(syncRespsUrl, syncRespsData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["SynchronizeResponsibilities"]);
                    reject(response);
                })
                .done(response => {
                    this.ClearEmpRespsChangesHistory();
                    this.InitializeResps();
                    resolve();
                });
        });
    }

    /** Метод возвращает данные, необходимые при синхронизации полномочий */
    SynchronizeRespsGetData() {
        return {
            EmployeeId: $("#employeeId").val(),
            ResponsibilitiesToAdd: EmployeeResponsibilityManagement.responsibilitiesToAdd,
            ResponsibilitiesToRemove: EmployeeResponsibilityManagement.responsibilitiesToRemove
        }
    }
}

// Модальное окно управления полномочиями сотрудника
$("#empRespManagement")
    .off("click", "#syncEmpRespsBtn").on("click", "#syncEmpRespsBtn", event => {
        event.preventDefault();
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.SynchronizeResponsibilities(event);
    })
    .off("checkmark-check", ".checkmark").on("checkmark-check", ".checkmark", event => {
        let block= new Block();
        block.CheckmarkCheck(event);
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.OnResponsibilityAddBtnClick(event);
    })
    .off("cross-click", ".cross").on("cross-click", ".cross", event => {
        let block= new Block();
        block.CrossClick(event);
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.OnResponsibilityRemoveBtnClick(event);
    })
    .off("click", "#allRespsNav .nav-next .nav-url").on("click", "#allRespsNav .nav-next .nav-url", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.NextAllResps(event);
    })
    .off("click", "#allRespsNav .nav-previous .nav-url").on("click", "#allRespsNav .nav-previous .nav-url", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.PreviousAllResps(event);
    })
    .off("click", "#selectedRespsNav .nav-next .nav-url").on("click", "#selectedRespsNav .nav-next .nav-url", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.NextSelectedResps(event);
    })
    .off("click", "#selectedRespsNav .nav-previous .nav-url").on("click", "#selectedRespsNav .nav-previous .nav-url", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.PreviousSelectedResps(event);
    })
    .off("click", "#allRespsSearch").on("click", "#allRespsSearch", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.AllRespsSearch(event).then(() => {
            employeeResponsibilityManagement.RestoreResponsibilitiesToAdd();
        });
    })
    .off("click", "#selectedRespsSearch").on("click", "#selectedRespsSearch", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.SelectedRespsSearch(event).then(() => {
            employeeResponsibilityManagement.RestoreResponsibilitiesToRemove();
        });
    })
    .off("click", "#clearAllRespsSearch").on("click", "#clearAllRespsSearch", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.ClearAllRespsSearch(event).then(() => {
            employeeResponsibilityManagement.RestoreResponsibilitiesToAdd();
        });
    })
    .off("click", "#clearSelectedRespsSearch").on("click", "#clearSelectedRespsSearch", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.ClearSelectedRespsSearch(event).then(() => {
            employeeResponsibilityManagement.RestoreResponsibilitiesToRemove();
        });
    })
    .off('hide.bs.modal').on("hide.bs.modal", event => {
        let employeeResponsibilityManagement = new EmployeeResponsibilityManagement();
        employeeResponsibilityManagement.OnEmpRespModalClosed(event);
    });