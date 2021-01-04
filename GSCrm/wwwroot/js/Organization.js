class Organization {
    Create() {
        return new Promise((resolve, reject) => {
            $(".form-shadow").removeClass("d-none");
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let createOrgUrl = $("#organizationModal form").attr("action");
            let createOrgData = this.GetCreationData();
            
            request.CommonPostRequest(createOrgUrl, createOrgData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateOrganization"]);
                    $(".form-shadow").addClass("d-none");
                })
                .done(newOrganizationUrl => {
                    $("#organizationModal").modal("hide");
                    location.replace(newOrganizationUrl);
                })
        })
    }

    Remove() {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemoveOrgConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    let request = new AjaxRequests();
                    let removeOrgUrl = $("#removeOrgForm #orgUrl a").attr("href");
    
                    request.CommonDeleteRequest(removeOrgUrl)
                        .fail(response => {
                            Utils.CommonErrosHandling(response["responseJSON"], ["RemoveOrg"]);
                            reject(response);
                        })
                        .done(orgsListUrl => {
                            location.replace(orgsListUrl);
                            resolve();
                        });
                }
            })
        })
    }

    /** Покинуть организацию */
    Leave() {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("LeaveOrgConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    let request = new AjaxRequests();
                    let removeOrgUrl = $("#leaveOrgForm #orgUrl a").attr("href");
                    request.JsonGetRequest(removeOrgUrl)
                        .fail(response => {
                            Utils.CommonErrosHandling(response["responseJSON"], ["LeaveOrg"]);
                            reject(response);
                        })
                        .done(orgsListUrl => {
                            location.replace(orgsListUrl);
                            resolve();
                        });
                }
            })
        });
    }

    Cancel() {
        $("#orgName").val();
        Utils.ClearErrors();
        $("#organizationModal").modal("hide");
    }

    GetCreationData() {
        return {
            Name: $("#orgName").val()
        }
    }

    /**
     * Создание полномочия
     * @param {*} event 
     */
    CreateResponsibility(event) {
        return new Promise((resolve, reject) => {
            let modal = $(event.currentTarget).closest("#responsibilitiesManagmentModal");
            let createResponsibilityUrl = $(modal).find("#newRespBlock").attr("action");
            let createResponsibilityData = this.CreateResponsibilityGetData();
            let request = new AjaxRequests();
            request.CommonPostRequest(createResponsibilityUrl, createResponsibilityData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateResponsibility"]);
                })
                .done(newResponsibilityUrl => {
                    $("#responsibilitiesManagmentModal").modal("hide");
                    location.replace(newResponsibilityUrl);
                });
        });
    }

    /** Получение данных для создания полномочия */
    CreateResponsibilityGetData() {
        return {
            Name: $("#responsibilityName").val(),
            OrganizationId: $("#orgId").val(),
        }
    }

    /**
     * Поиск по полномочиям
     * @param {*} event 
     */
    SearchResponsibility(event) {
        return new Promise((resolve, reject) => {
            let modal = $(event.currentTarget).closest("#responsibilitiesManagmentModal");
            let searchResponsibilityUrl = $(modal).find("#searchRespBlock").attr("action");
            let searchResponsibilityData = this.SearchResponsibilityGetData();
            let request = new AjaxRequests();
            request.CommonPostRequest(searchResponsibilityUrl, searchResponsibilityData).done(responsibilities => {
                this.RednerResponsibilities(responsibilities);
            });
        });
    }

    /** Получение данных для поиска по полномочиям */
    SearchResponsibilityGetData() {
        return {
            Id: $("#orgId").val(),
            SeacrhResponsibilityName: $("#SeacrhResponsibilityName").val()
        }
    }

    /**
     * Очистка поиска по полномочиям
     * @param {*} event 
     */
    ClearResponsibilitySearch(event) {
        return new Promise((resolve, reject) => {
            let clearSearchResponsibilityUrl = $(event.currentTarget).attr("href");
            let request = new AjaxRequests();
            request.CommonGetRequest(clearSearchResponsibilityUrl).done(responsibilities => {
                this.RednerResponsibilities(responsibilities);
                $("#SeacrhResponsibilityName").val("");
            });
        });
    }

    /**
     * Получение списка полномочий
     * @param {*} event 
     */
    GetResponsibilities(event) {
        return new Promise((resolve, reject) => {
            let getResponsibilitiesUrl = $(event.currentTarget).attr("data-href");
            if (!Utils.IsNullOrEmpty(getResponsibilitiesUrl)) {
                let request = new AjaxRequests();
                request.JsonGetRequest(getResponsibilitiesUrl).done(responsibilities => this.RednerResponsibilities(responsibilities));
            }
        });
    }

    /**
     * Рендеринг полученных полномочий
     * @param {*} responsibilities 
     */
    RednerResponsibilities(responsibilities) {
        $("#responsibilitiesList").empty();
        responsibilities.map(responsibility => {
            $("#responsibilitiesList").append("<li class='list-group-item'>" +
            "<div class='row'><div class='col'>" +
            "<a href='" + Localization.GetUri("responsibility") + responsibility["id"] + "'>" + responsibility["name"] +"</a>" +
            "</div><div class='col-auto remove-item-btn'>" +
            "<div class='remove-item-url' hidden='hidden'>" +
            "<a href='" + Localization.GetUri("deleteResponsibility") + responsibility["id"] + "'>" + responsibility["name"] +"</a>" +
            "</div><span class='icon-bin'></span></div></li>")
        });
    }

    /**
     * Удаление полномочия
     * @param {*} event 
     */
    RemoveResponsibility(event) {
        return new Promise((resolve, reject) => {
            let removeResponsibilityUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");
            let request = new AjaxRequests();
            request.CommonDeleteRequest(removeResponsibilityUrl)
                .fail(response => {
                    Utils.DefaultErrorHandling(response["responseJSON"]);
                    reject(response);
                })
                .done(responsibilities => {
                    this.RednerResponsibilities(responsibilities);
                    resolve();
                });
        })
    }

    /**
     * Метод проставляет организацию основной
     * @param {*} event 
     */
    SetUpPrimaryOrg(event) {
        let button = new Button();
        let table = $(event.currentTarget).closest(".fl-table");
        let checkMarks = $(table).find(".oval-mark-readonly");

        // Скрытие галок для остальных выбранных контактов
        Array.from(checkMarks).map(item => {
            button.HideOvalCheckmarkReadonly(item);
        });

        // Проставление признака организации для выбранной записи
        button.OvalCheckmarkReadonly($(event.currentTarget));

        // Отображение блока с фиксацией изменений
        this.ShowSavePrimaryOrgBlock(event);
    }

    /** Метод отображает/скрывает блок с фиксацией выбором основной организации */
    ShowSavePrimaryOrgBlock(event) {
        let organizationsList = $(document).find("#organizationsList");
        let markCheck = $(organizationsList).find(".oval-mark-readonly");
        let primaryOrganizationId = $("#PrimaryOrganizationId").val();
        let row = $(event.currentTarget).closest("tr");
        let organizationId = $(row).find(".organization-id").text();

        if (primaryOrganizationId == "") {
            if (markCheck.length > 0) {
                $("#changePrimaryOrg").slideDown("slow");
            }
            else {
                $("#changePrimaryOrg").slideUp("slow");
            }
        }

        else {
            if (markCheck.length == 0) {
                $("#changePrimaryOrg").slideDown("slow");
            }
            else {
                if (primaryOrganizationId != organizationId) {
                    $("#changePrimaryOrg").slideDown("slow");
                }
                else {
                    $("#changePrimaryOrg").slideUp("slow");
                }
            }
        }
    }

    /** Метод изменяет основную организацию */
    ChangePrimaryOrg(event) {
        return new Promise((resolve, reject) => {
            let row = $("#organizationsList").find(".oval-mark-readonly").closest("tr");
            let organizationId = $(row).find(".organization-id").text();
            let changePrimaryOrgUrl = $(event.currentTarget).closest("form").attr("action") + "/" + organizationId;
            let changePrimaryOrgData = this.ChangePrimaryOrgGetData(event);
            let request = new AjaxRequests();
            request.JsonGetRequest(changePrimaryOrgUrl, changePrimaryOrgData)
                .fail(response => {
                    Utils.DefaultErrorHandling(response["responseJSON"]);
                    reject(response);
                })
                .done(() => {
                    Swal.fire(MessageManager.Invoke("PrimaryOrgHasBeenChanged")).then(() => location.reload());
                    resolve();
                });
        });
    }

    /**
     * Метод возвращает информацию, необходимую для изменения основной организации
     * @param {*} event 
     */
    ChangePrimaryOrgGetData(event) {
        // Поиск новой выбранной основной организации
        let markCheck = $("#organizationsList").find(".oval-mark-readonly")[0];
        let primaryOrgId = "";
        if (markCheck != undefined) {
            let row = $(markCheck).closest("tr");
            primaryOrgId = $(row).find(".organization-id").text();
        }

        // Возврат
        return {
            PrimaryOrgId: primaryOrgId
        }
    }

    /** Согласия на вступление в организацию */
    AcceptInvite() {
        return new Promise((resolve, reject) => {
            let acceptInviteUrl = $("#acceptInvite").closest("form").attr("action");
            let requests = new AjaxRequests();
            let hasErrors = false;
            requests.CommonGetRequest(acceptInviteUrl)
                .catch(response => {
                    hasErrors = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["AcceptInvite"]);
                })
                .then(() => {
                    if (!hasErrors) {

                    }
                });
        })
    }

    /** Отказ от вступления в организацию */
    RejectInvite() {
        return new Promise((resolve, reject) => {
            let rejectInviteUrl = $("#rejectInvite").closest("form").attr("action");
            let requests = new AjaxRequests();
            requests.CommonGetRequest(rejectInviteUrl).then(returnUrl => {
                Swal.fire(MessageManager.Invoke("InviteHasBeenRejected")).then(() => location.replace(returnUrl));
            });
        })
    }

    static SetDefaultTab() {
        let tabs = $("#employeeModal .form-group .radio-tabs");

        tabs.find(".form-check").each((index, tab) => {
            $(tab).removeClass("active");
            let checkInput = $(tab).find(".form-check-input");
            $(checkInput).removeAttr("checked");
        });

        let firstTab = tabs.find(".form-check")[0];
        $(firstTab).addClass("active");
        let firstTabCheckInput = $(firstTab).find(".form-check-input");
        $(firstTabCheckInput).attr("checked", "checked");
    }

    ClearSearch() {
        $("#SearchName").val();
    }
}

// Форма создания организации
$("#createOrgForm")
    .off("click", "#clearOrgSearch").on("click", "#clearOrgSearch", event => {
        let organization = new Organization();
        organization.ClearSearch();
    });

// Список организаций
$("#organizationsList")
    .off("click", ".organization-item").on("click", ".organization-item", event => {
        let tab = new Tab();
        tab.ClearOrgTab();
    })
    .off("click", ".oval-mark").on("click", ".oval-mark", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.SetUpPrimaryOrg(event);
    });

// Модальное окно создания организации
$("#organizationModal")
    .off("click", "#createOrgBtn").on("click", "#createOrgBtn", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.Create();
    })
    .off("click", "#cancelCreationOrgBtn").on("click", "#cancelCreationOrgBtn", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.Cancel();
    });

// Форма удаления организации
$("#removeOrgForm").off("click", "#removeOrgBtn").on("click", "#removeOrgBtn", event => {
    event.preventDefault();
    let organization = new Organization();
    organization.Remove();
});

// Форма выхода из организации
$("#leaveOrgForm").off("click", "#leaveOrgBtn").on("click", "#leaveOrgBtn", event => {
    event.preventDefault();
    let organization = new Organization();
    organization.Leave();
});

// Карточка организации
$("#organizationForm")
    .off("click", "#openEmployeeModalBtn").on("click", "#openEmployeeModalBtn", event => {
        Organization.SetDefaultTab();
        $("#selectUserOption").css("display", "block");
        $("#createUserOption").css("display", "none");
    })
    .off("click", "#orgTabs .nav-item").on("click", "#orgTabs .nav-item", event => {
        let navTab = new NavTab();
        navTab.Remember(event, "currentOrgTab");
        let navConnectedTab = new NavConnectedTab();
        navConnectedTab.Remember(event, "currentOrgConnectedTab");
    })
    .off("click", ".employee-item").on("click", ".employee-item", event => {
        let tab = new Tab();
        tab.ClearEmpTabs()
    });

$("#changePrimaryOrg")
    .off("click", "#changePrimaryOrgBtn").on("click", "#changePrimaryOrgBtn", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.ChangePrimaryOrg(event);
    });

// Управление полномочиями организации
$("#responsibilitiesManagmentModal")
    .off("click", "#searchResps").on("click", "#searchResps", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.SearchResponsibility(event);
    })
    .off("click", "#clearSearchResps").on("click", "#clearSearchResps", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.ClearResponsibilitySearch(event);
    })
    .off("click", "#responsibilitiesList .remove-item-btn").on("click", "#responsibilitiesList .remove-item-btn", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.RemoveResponsibility(event);
    })
    .off("click", ".nav-previous").on("click", ".nav-previous .nav-url", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.GetResponsibilities(event);
    })
    .off("click", ".nav-next").on("click", ".nav-next .nav-url", event => {
        event.preventDefault();
        let organization = new Organization();
        organization.GetResponsibilities(event);
    })
    .off("click", ".icon-add-outline").on("click", ".icon-add-outline", event => {
        $("#responsibilitiesManagmentModal").find("#respsBlockMask").css("display", "block");
        $("#responsibilitiesManagmentModal").find("#modalHeaderMask").css("display", "block");
        $("#newRespBlock").slideDown("slow");
    })
    .off("hide.bs.modal").on("hide.bs.modal", event => {
        setTimeout(() => {
            $("#newRespBlock").slideUp();
            $("#responsibilitiesManagmentModal").find("#respsBlockMask").css("display", "none");
            $("#responsibilitiesManagmentModal").find("#modalHeaderMask").css("display", "none");
        }, 200);
    })
    .off("click", "#newRespBlockCancel").on("click", "#newRespBlockCancel", event => {
        $("#responsibilitiesManagmentModal").find("#newRespBlock").slideUp("slow");
        setTimeout(() => {
            Utils.ClearErrors();
            $("#responsibilitiesManagmentModal").find("#respsBlockMask").css("display", "none");
            $("#responsibilitiesManagmentModal").find("#modalHeaderMask").css("display", "none");
            $("#newRespBlock .form-control").val("");
        }, 600);
    })
    .off("click", "#newRespBlockSave").on("click", "#newRespBlockSave", event => {
        let organization = new Organization();
        organization.CreateResponsibility(event);
    });

// Форма приглашения в организацию
$("#inviteActionsForm")
    .off("click", "#acceptInvite").on("click", "#acceptInvite", event => {
        event.preventDefault();
        event.stopPropagation();
        let organization = new Organization();
        organization.AcceptInvite();
    })
    .off("click", "#rejectInvite").on("click", "#rejectInvite", event => {
        event.preventDefault();
        event.stopPropagation();
        let organization = new Organization();
        organization.RejectInvite();
    })