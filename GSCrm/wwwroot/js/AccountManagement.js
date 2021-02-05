class AccountManagement {
    static managersToAdd = [];
    static managersToRemove = [];
    static primaryManagerId = "";
    static primaryManagerIdCash = "";

    /**
     * Метод инициализирует попап управления командой по клиенту
     */
    InitializeAccTeam() {
        return new Promise((resolve, reject) => {
            let accountTeamUrl = $("#accountForm").find("#accTeamManagementUrl").attr("href");
            let request = new AjaxRequests();
            request.CommonGetRequest(accountTeamUrl)
                .fail(error => reject(error))
                .done(response => {
                    this.ResetLists();
                    this.Render(response);
                    this.InitializePrimaryManager();
                    resolve(response);
                });
        });
    }

    /**
     * Метод получает записи для модального окна управления командой по клиенту
     */
    GetRecords(event) {
        return new Promise((resolve, reject) => {
            let accountTeamUrl = $(event.currentTarget).attr("data-href");
            let request = new AjaxRequests();
            request.JsonGetRequest(accountTeamUrl)
                .fail(error => reject(error))
                .done(response => resolve(response));
        })
    }
    
    /**
     * Метод сбрасывает заполненные списки
     */
    ResetLists() {
        $("#teamAllEmployees #allEmployeesList tbody").empty();
        $("#selectedEmployeesList .list-card").empty();
    }

    /**
     * Метод отрисовывает модальное окно при иницилазиации
     * @param {*} response 
     */
    Render(response) {
        this.RenderTeamAllEmployees(response["teamAllEmployees"]);
        this.RenderTeamSelectedEmployees(response["teamSelectedEmployees"]);
    }

    /**
     * Метод отрисовывает список всех возможных менеджеров
     * @param {*} teamAllEmployees список всех сотрудников организации, которая создала клиента
     */
    RenderTeamAllEmployees(teamAllEmployees) {
        let allEmployeesList = $("#teamAllEmployees #allEmployeesList tbody");
        $(allEmployeesList).empty();
        teamAllEmployees.map(employee => {
            let employeeRow = this.GetAllEmployeesRow(employee);
            $(allEmployeesList).append($(employeeRow));
        });
    }

    /**
     * Метод отрисовывает команду по клиенту
     * @param {*} teamSelectedEmployees Команда по клиенту
     */
    RenderTeamSelectedEmployees(teamSelectedEmployees) {
        let selectedEmployeesList = $("#teamSelectedEmployees #selectedEmployeesList .list-card");
        $(selectedEmployeesList).empty();
        teamSelectedEmployees.map(employee => {
            let employeeCard = this.GetSelectedEmployeeCard(employee);
            $("#teamSelectedEmployees #selectedEmployeesList .list-card").append($(employeeCard));
        });
    }

    /**
     * Метод восстанавливает сотрудников, выбраных для добавления в команду по клиенту
     */
    RestoreEmployeesToAdd() {
        AccountManagement.managersToAdd.map(managerId => {
            $("#accTeamManagementModal #allEmployeesList tbody .employee-id").each((index, managerIdElement) => {
                let row = $(managerIdElement).closest("tr");
                if ($(managerIdElement).text() == managerId) {
                    $(row).find(".checkmark").addClass("checkmark-checked");
                }
            })
        });
    }
    
    /**
     * Метод восстанавливает выделенных для удаления менеджеров
     */
    RestoreEmployeesToRemove() {
        AccountManagement.managersToRemove.map(employee => {
            $("#accTeamManagementModal #selectedEmployeesList .list-card-content").each((index, card) => {
                let managerId = $(card).find(".manager-id").val();
                if (managerId == employee) {
                    $(card).find(".list-group-item").addClass("account-manager-selected");
                }
            })
        });
    }

    /**
     * Метод восстанавливает основного выбранного менеджера для команды по клиенту
     */
    RestorePrimaryEmployee() {
        let modal = $("#accTeamManagementModal");
        let selectedEmployees = $(modal).find("#teamSelectedEmployees #selectedEmployeesList .list-card");
        $(selectedEmployees).find(".manager-id").each((index, managerId) => {
            let card = $(managerId).closest(".list-card-content");
            let listGroupItem = $(card).find(".list-group-item");

            // Если менеджер был выбран как основной, восстановление признака
            if (AccountManagement.primaryManagerIdCash == $(managerId).val()) {
                this.SetUpPrimaryManager(listGroupItem);

                // Удаление записи из списка менеджеров для удаления
                let managerIndex = AccountManagement.managersToRemove.indexOf(managerId);
                if (managerIndex != -1) {
                    AccountManagement.managersToRemove.splice(managerIndex, 1);
                }
            }

            // Иначе признак оснонвого снимается с карточки
            else {
                this.ClearPrimaryManagerSign(listGroupItem);
            }
        })
    }

    /**
     * Метод возвращает карточку одного сотрудника для модального окна управления командой
     * @param {*} employee 
     */
    GetSelectedEmployeeCard(employee) {
        let initialName = employee["initialName"];
        let positionName = employee["positionName"] == null ? "" : employee["positionName"];
        let phoneNumber = employee["phoneNumber"] == null ? "" : employee["phoneNumber"];

        // Карточка менеджера
        let employeeCard = $("<div class='list-card-content'><input class='manager-id' type='hidden' value='" + employee["id"] + "' />" +
            "<a class='list-group-item list-group-item-action account-manager'>" +
            "<div class='d-flex w-100 justify-content-between'>" +
            "<h5 class='mr-1'>" + initialName + "</h5>" +
            "<div><div class='oval-mark'></div></div></div>" +
            "<p class='mb-1'>" + (positionName == null ? "" : positionName) + "</p>" +
            "<small class='mb-1'>" + phoneNumber + "</small>" +
            "</a></div>");

        // Карточка основного менеджера по клиенту
        let primaryEmployeeCard = $("<div class='list-card-content'><input class='manager-id' type='hidden' value='" + employee["id"] + "' />" +
            "<a class='list-group-item account-manager primary-manager'>" +
            "<div class='d-flex w-100 justify-content-between'>" +
            "<h5 class='mr-1'>" + initialName + "</h5>" +
            "<div><div class='oval-mark-readonly'><div class='icon-checkmark'></div></div></div></div>" +
            "<p class='mb-1'>" + (positionName == null ? "" : positionName) + "</p>" +
            "<small class='mb-1'>" + phoneNumber + "</small>" +
            "</a></div>");

        return employee["isPrimary"] ? primaryEmployeeCard : employeeCard;
    }

    /**
     * Метод возвращает запись одного сотрудника для модального окна управления командой
     * @param {*} employee 
     */
    GetAllEmployeesRow(employee) {
        let positionName = employee["primaryPositionName"];
        if (positionName == null) positionName = "";
        return "<tr>" +
            "<td class='employee-id d-none'>" + employee["id"] + "</td>" +
            "<td>" + employee["fullInitialName"] + "</td>" +
            "<td>" + employee["divisionName"] + "</td>" +
            "<td>" + positionName + "</td>" +
            '<td class="checkmark"><span class="icon-checkmark"></span></td>' +
            "</tr>";
    }

    /**
     * Действие при промотке списка со всеми сотрудниками вперед
     * @param {*} event 
     */
    NextAllEmployees(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderTeamAllEmployees(response);
                this.RestoreEmployeesToAdd();
                resolve();
            });
        })
    }
    
    /**
     * Действие при промотке списка со всеми сотрудниками назад
     * @param {*} event 
     */
    PreviousAllEmployees(event) {
        return new Promise((resolve, reject) => {
            this.GetRecords(event).then(response => {
                this.RenderTeamAllEmployees(response);
                this.RestoreEmployeesToAdd();
                resolve();
            });
        })
    }

    /**
     * Поиск по всем сотрудника организации, создавшей клиента
     * @param {*} event 
     */
    AllEmployeesSearch(event) {
        return new Promise((resolve, reject) => {
            let allEmployeesSearchUrl = $(event.currentTarget).attr("data-href");
            let allEmployeesSearchData = this.GetAllEmployeesSearchData();
            let request = new AjaxRequests();

            request.CommonPostRequest(allEmployeesSearchUrl, allEmployeesSearchData)
                .fail(error => reject(error))
                .done(response => {
                    this.RenderTeamAllEmployees(response["teamAllEmployees"]);
                    this.RenderEmployeesFilterFields(response);
                    resolve();
                })
        });
    }

    /**
     * Метод возвращает данные для отправки на сервер при поиске по списку всех сотрудников
     */
    GetAllEmployeesSearchData() {
        return {
            OrganizationId: $("#organizationId").val(),
            SearchAllManagersName: $("#SearchAllManagersName").val(),
            SearchAllManagersDivision: $("#SearchAllManagersDivision").val(),
            SearchAllManagersPosition: $("#SearchAllManagersPosition").val()
        }
    }

    /**
     * Поиск по команде клиента
     * @param {*} event 
     */
    SelectedEmployeesSearch(event) {
        return new Promise((resolve, reject) => {
            let selectedEmployeesSearchUrl = $(event.currentTarget).attr("data-href");
            let selectedEmployeesSearchData = this.GetSelectedEmployeesSearchData();
            let request = new AjaxRequests();

            request.CommonPostRequest(selectedEmployeesSearchUrl, selectedEmployeesSearchData)
                .fail(error => reject(error))
                .done(response => {
                    this.RenderTeamSelectedEmployees(response["teamSelectedEmployees"]);
                    this.RenderEmployeesFilterFields(response);
                    resolve();
                })
        });
    }
    
    /**
     * Метод возвращает данные для отправки на сервер при поиске по списку всех сотрудников
     */
    GetSelectedEmployeesSearchData() {
        return {
            OrganizationId: $("#organizationId").val(),
            SearchSelectedManagersName: $("#SearchSelectedManagersName").val(),
            SearchSelectedManagersPosition: $("#SearchSelectedManagersPosition").val(),
            SearchSelectedManagersPhone: $("#SearchSelectedManagersPhone").val()
        }
    }

    /**
     * Очистка поиска по всем сотрудника организации, создавшей клиента
     * @param {*} event 
     */
    ClearAllEmployeesSearch(event) {
        return new Promise((resolve, reject) => {
            let clearAllEmployeesSearchUrl = $(event.currentTarget).attr("data-href");
            let request = new AjaxRequests();

            request.CommonGetRequest(clearAllEmployeesSearchUrl)
                .fail(error => reject(error))
                .done(response => {
                    this.RenderTeamAllEmployees(response["teamAllEmployees"]);
                    this.RenderEmployeesFilterFields(response);
                    resolve();
                })
        });
    }

    /**
     * Очистка поиска по команде клиента
     * @param {*} event 
     */
    ClearSelectedEmployeesSearch(event) {
        return new Promise((resolve, reject) => {
            let clearSelectedEmployeesSearchUrl = $(event.currentTarget).attr("data-href");
            let request = new AjaxRequests();

            request.CommonGetRequest(clearSelectedEmployeesSearchUrl)
                .fail(error => reject(error))
                .done(response => {
                    this.RenderTeamSelectedEmployees(response["teamSelectedEmployees"]);
                    this.RenderEmployeesFilterFields(response);
                    resolve();
                })
        });
    }

    /**
     * Метод заполняет поля фильтров значениями, пришедшими с сервера
     * @param {*} response Ответ, пришедший с сервера с данными для фильтров
     */
    RenderEmployeesFilterFields(response) {
        let teamAllEmployeesVM = response["teamAllEmployeesVM"];
        let teamSelectedEmployeesVM = response["teamSelectedEmployeesVM"];
        $("#SearchAllManagersName").val(teamAllEmployeesVM["searchAllManagersName"]);
        $("#SearchAllManagersDivision").val(teamAllEmployeesVM["searchAllManagersDivision"]);
        $("#SearchAllManagersPosition").val(teamAllEmployeesVM["searchAllManagersPosition"]);
        $("#SearchSelectedManagersName").val(teamSelectedEmployeesVM["searchSelectedManagersName"]);
        $("#SearchSelectedManagersPosition").val(teamSelectedEmployeesVM["searchSelectedManagersPosition"]);
        $("#SearchSelectedManagersPhone").val(teamSelectedEmployeesVM["searchSelectedManagersPhone"]);
    }

    /**
     * При нажатии на кнопку добавления сотрудника в команду по клиенту
     */
    OnEmployeeAddBtnClick(event) {
        let row = $(event.currentTarget).closest("tr");
        let employeeId = $(row).find(".employee-id").text();
        if (!AccountManagement.managersToAdd.includes(employeeId)) {
            AccountManagement.managersToAdd.push(employeeId);
        }
        else {
            let employeeIndex = AccountManagement.managersToAdd.indexOf(employeeId);
            if (employeeIndex != -1) {
                AccountManagement.managersToAdd.splice(employeeIndex, 1);
            }
        }
    }

    /**
     * При нажатии на кнопку удаления менеджера из команды по клиенту
     */
    OnManagerRemoveBtnClick(event) {
        // Необходимо получить id выбранного основного менеджера и сравнить его с выбранным на текущий момент
        // Если они отличваются, значит текщуго менеджера можно добавлять в список на удаление
        let card = $(event.currentTarget).closest(".list-card-content");
        let managerId = $(card).find(".manager-id").val();
        if (managerId != AccountManagement.primaryManagerIdCash) {
            if (!AccountManagement.managersToRemove.includes(managerId)) {
                AccountManagement.managersToRemove.push(managerId);
            }
            else {
                let managerIndex = AccountManagement.managersToRemove.indexOf(managerId);
                if (managerIndex != -1) {
                    AccountManagement.managersToRemove.splice(managerIndex, 1);
                }
            }
        }
    }
    
    /**
     * Метод устанавливает выбранного менеджера основным
     * @param {*} event 
     */
    CheckEmployeePrimary(event) {
        event.stopPropagation();
        let selectedEmployeesList = $("#selectedEmployeesList .list-card");

        // Снятие выделения с основного менеджера
        $(selectedEmployeesList).find(".primary-manager").each((index, employee) => {
            this.ClearPrimaryManagerSign(employee);
        })

        // Простановка основного менеджера для выбранного сотрудника
        let accountManager = $(event.currentTarget).closest(".account-manager");
        this.SetUpPrimaryManager(accountManager);

        // Запоминание выбранного сотрудника в кеш
        let card = $(accountManager).closest(".list-card-content");
        let managerId = $(card).find(".manager-id").val();
        AccountManagement.primaryManagerIdCash = managerId;

        // Удаление записи из списка менеджеров для удаления
        let managerIndex = AccountManagement.managersToRemove.indexOf(managerId);
        if (managerIndex != -1) {
            AccountManagement.managersToRemove.splice(managerIndex, 1);
        }
    }

    /**
     * Метод запоминает основного сотрудника при открытии модального окна
     */
    InitializePrimaryManager() {
        let modal = $("#accTeamManagementModal");
        let selectedEmployees = $(modal).find("#teamSelectedEmployees #selectedEmployeesList .list-card");
        let primaryEmployee = $(selectedEmployees).find(".primary-manager");
        let primaryManagerId = $(primaryEmployee).closest(".list-card-content").find(".manager-id").val();
        AccountManagement.primaryManagerId = primaryManagerId;
        AccountManagement.primaryManagerIdCash = primaryManagerId;
    }

    /**
     * Метод снимает признак основного менеджера с карточки сотрудника
     * @param {*} accountManager Карточка сотрудника
     */
    ClearPrimaryManagerSign(accountManager) {
        $(accountManager).addClass("list-group-item-action");
        $(accountManager).removeClass("primary-manager");
        let checkMark = $(accountManager).find(".oval-mark-readonly");
        if (checkMark.length == 0) {
            checkMark = $(accountManager).find(".oval-mark-check");
        }
        $(checkMark).removeClass("oval-mark-readonly").addClass("oval-mark");
        $(checkMark).empty();
    }

    /**
     * Метод помечает карточку сотрудника как у основного сотрудника
     * @param {*} accountManager Карточка сотрудника
     */
    SetUpPrimaryManager(accountManager) {
        $(accountManager).removeClass("account-manager-selected");
        $(accountManager).removeClass("list-group-item-action");
        $(accountManager).addClass("primary-manager");
        let checkMark = $(accountManager).find(".oval-mark");
        let block= new Block();
        block.OvalCheckmarkReadonly(checkMark);
    }

    /**
     * Событие, происходящее при закрытии модального окна
     */
    OnAccTeamModalClosed(event) {
        // Если есть запрет на закрытие модельного окна и существуют незафиксированные изменения
        if (this.ExistsAccTeamChanges()) {
            event.preventDefault();

            // Запрос подтверждения на закрытие, в случае успеха обнуление переменных и закрытие окна
            Swal.fire(MessageManager.Invoke("NotCommitModalClosedConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    this.ClearAccTeamManagementSearch().then(() => {
                        this.ClearAccTeamChangesHistory();
                        $(event.currentTarget).modal("hide");
                    });
                }
            });
        }

        // Иначе осуществляется перезагрузка страницы
        else {
            this.ClearAccTeamManagementSearch().then(() => location.reload());
        }
    }

    /**
     * Метод проверяет, существуют ли незафиксированные изменения
     */
    ExistsAccTeamChanges() {
        return AccountManagement.managersToAdd.length > 0 || AccountManagement.managersToRemove.length > 0 ||
            (AccountManagement.primaryManagerId != "" && AccountManagement.primaryManagerIdCash != AccountManagement.primaryManagerId);
    }

    /**
     * Очистка фильтрации в модальном окне управления командой по клиенту
     */
    ClearAccTeamManagementSearch() {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let clearAccTeamSearchUrl = LocalizationManager.GetUri("clearAccTeamSearch");
            request.CommonGetRequest(clearAccTeamSearchUrl)
                .fail(() => reject(error))
                .done(() => resolve())
        })
    }

    /**
     * Метод очищает историю изменений в модальном окне управления командой по клиенту
     */
    ClearAccTeamChangesHistory() {
        AccountManagement.managersToAdd = [];
        AccountManagement.managersToRemove = [];
        AccountManagement.primaryManagerId = "";
    }

    /**
     * Метод выполняет синхронизацию команды по клиенту
     * @param {*} event 
     */
    SynchronizeAccTeam(event) {
        return new Promise((resolve, reject) => {
            let syncAccTeamUrl = $(event.currentTarget).attr("data-href");
            let syncAccTeamData = this.SynchronizeAccTeamGetData();
            let request = new AjaxRequests();

            request.JsonPostRequest(syncAccTeamUrl, syncAccTeamData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["SynchronizeManagers"]);
                    reject(errors);
                })
                .done(response => {
                    this.ClearAccTeamChangesHistory();
                    this.ResetLists();
                    this.Render(response);
                    this.InitializePrimaryManager();
                    Initializer.ReInitScrools();
                    resolve();
                });
        })
    }

    /**
     * Метод возвращает данные, необходимые при синхронизации команды по клиенту
     */
    SynchronizeAccTeamGetData() {
        return {
            AccountId: $("#accountId").val(),
            PrimaryManagerId: AccountManagement.primaryManagerIdCash,
            ManagersToAdd: AccountManagement.managersToAdd,
            ManagersToRemove: AccountManagement.managersToRemove
        }
    }
}

$("#accTeamManagementModal")
    .off("click", "#selectedEmployeesList .account-manager").on("click", "#selectedEmployeesList .account-manager", event => {
        if ($(event.currentTarget).hasClass("account-manager-selected")) {
            $(event.currentTarget).removeClass("account-manager-selected");
        }
        else if (!$(event.currentTarget).hasClass("primary-manager")) {
            $(event.currentTarget).addClass("account-manager-selected");
        }
        let accountManagement = new AccountManagement();
        accountManagement.OnManagerRemoveBtnClick(event);
    })
    .off("click", "#selectedEmployeesList .oval-mark").on("click", "#selectedEmployeesList .oval-mark", event => {
        let accountManagement = new AccountManagement();
        accountManagement.CheckEmployeePrimary(event);
    })
    .off("checkmark-check", ".checkmark").on("checkmark-check", ".checkmark", event => {
        let block= new Block();
        block.CheckmarkCheck(event);
        let accountManagement = new AccountManagement();
        accountManagement.OnEmployeeAddBtnClick(event);
    })
    .off("click", "#allEmployeesNav .nav-next .nav-url").on("click", "#allEmployeesNav .nav-next .nav-url", event => {
        let accountManagement = new AccountManagement();
        accountManagement.NextAllEmployees(event);
    })
    .off("click", "#allEmployeesNav .nav-previous .nav-url").on("click", "#allEmployeesNav .nav-previous .nav-url", event => {
        let accountManagement = new AccountManagement();
        accountManagement.PreviousAllEmployees(event);
    })
    .off("click", "#allEmployeesSearch").on("click", "#allEmployeesSearch", event => {
        let accountManagement = new AccountManagement();
        accountManagement.AllEmployeesSearch(event).then(() => {
            accountManagement.RestoreEmployeesToAdd();
        });
    })
    .off("click", "#selectedEmployeesSearch").on("click", "#selectedEmployeesSearch", event => {
        let accountManagement = new AccountManagement();
        accountManagement.SelectedEmployeesSearch(event).then(() => {
            accountManagement.RestoreEmployeesToRemove();
            accountManagement.RestorePrimaryEmployee();
        });
    })
    .off("click", "#clearAllEmployeesSearch").on("click", "#clearAllEmployeesSearch", event => {
        let accountManagement = new AccountManagement();
        accountManagement.ClearAllEmployeesSearch(event).then(() => {
            accountManagement.RestoreEmployeesToAdd();
        });
    })
    .off("click", "#clearSelectedEmployeesSearch").on("click", "#clearSelectedEmployeesSearch", event => {
        let accountManagement = new AccountManagement();
        accountManagement.ClearSelectedEmployeesSearch(event).then(() => {
            accountManagement.RestoreEmployeesToRemove();
            accountManagement.RestorePrimaryEmployee();
        });
    })
    .off("click", '#syncAccTeamBtn').on("click", "#syncAccTeamBtn", event => {
        let accountManagement = new AccountManagement();
        accountManagement.SynchronizeAccTeam(event);
    })
    .off('hide.bs.modal').on("hide.bs.modal", event => {
        let accountManagement = new AccountManagement();
        accountManagement.OnAccTeamModalClosed(event);
    });