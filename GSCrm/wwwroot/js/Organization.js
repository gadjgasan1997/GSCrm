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
    
    RemoveDialog() {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemoveOrgConfirmation")).then(dialogResult => {
                this.Remove(dialogResult)
                    .catch(error => Swal.fire(MessageManager.Invoke("RemoveOrgError")))
                    .then(() => window.history.back());
            })
        })
    }

    Remove(dialogResult) {
        return new Promise((resolve, reject) => {
            if (dialogResult.value) {
                let request = new AjaxRequests();
                let removeOrgUrl = $("#removeOrgForm #orgUrl a").attr("href");

                request.CommonDeleteRequest(removeOrgUrl)
                    .fail(error => reject(error))
                    .done(() => resolve());
            }
        })
    }

    Cancel() {
        this.ClearFields();
        $("#organizationModal").modal("hide");
    }

    GetCreationData() {
        return {
            Name: $("#orgName").val()
        }
    }

    ClearFields() {
        $("#orgName").val("");
    }

    SearchResponsibility(event) {
        return new Promise((resolve, reject) => {
            let modal = $(event.currentTarget).closest("#responsibilitiesManagmentModal");
            let searchResponsibilityUrl = $(modal).find("form").attr("action");
            let searchResponsibilityData = this.SearchResponsibilityGetData();
            let request = new AjaxRequests();
            request.CommonPostRequest(searchResponsibilityUrl, searchResponsibilityData).done(response => {

            });
        });
    }

    SearchResponsibilityGetData() {
        return {

        }
    }

    ClearResponsibilitySearch(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            request.CommonGetRequest(clearSearchResponsibilityUrl).done(response => {

            });
        });
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
        return new Promise((resolve, reject) => {
            $("#SearchName").val();
        });
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
    organization.RemoveDialog();
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
    })
    .off("click", ".employee-item").on("click", ".employee-item", event => {
        let tab = new Tab();
        tab.ClearEmpTabs()
    })
    .off("click", ".employee-item").on("click", ".employee-item", event => {
        let tab = new Tab();
        tab.ClearEmpTabs()
    });

$("#responsibilitiesManagmentModal")
    .off("click", "#searchResps").on("click", "#searchResps", event => {
        let organization = new Organization();
        organization.SearchResponsibility(event);
    })
    .off("click", "#clearSearchResps").on("click", "#clearSearchResps", event => {
        let organization = new Organization();
        organization.ClearResponsibilitySearch(event);
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
    .off("click", "#newRespBlockClose").on("click", "#newRespBlockClose", event => {
        $("#responsibilitiesManagmentModal").find("#newRespBlock").slideUp("slow");
        setTimeout(() => {
            $("#responsibilitiesManagmentModal").find("#respsBlockMask").css("display", "none");
            $("#responsibilitiesManagmentModal").find("#modalHeaderMask").css("display", "none");
            $("#newRespBlock .form-control").val("");
        }, 600);
    })
    .off("click", "newRespBlockSave").on("click", "newRespBlockSave", event => {
        
    });