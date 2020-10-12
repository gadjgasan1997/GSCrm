class EmployeeContact {
    /**
     * ¬ыполн€ет запрос на сервер дл€ создани€ контакта
     * ѕри наличии ошибок обрабатывает их
     */
    Create() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let createEmpContactUrl = $("#empContactCreateModal form").attr("action");
            let createEmpContactData = this.CreateGetData();

            request.JsonPostRequest(createEmpContactUrl, createEmpContactData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateEmployeeContact"]);
                })
                .done(() => {
                    $("#empContactCreateModal").modal("hide");
                    location.reload();
                });
        })
    }

    /**
     * ¬озвращает данные дл€ запроса на сервер при создании контакта
     */
    CreateGetData() {
        let contactType = $("#createEmpContactType").val();
        if (contactType.length == 0) {
            contactType = "None";
        }
        return {
            EmployeeId: $("#employeeId").val(),
            ContactType: contactType,
            PhoneNumber: $("#createEmpContactPhone").val(),
            Email: $("#createEmpContactEmail").val()
        }
    }

    /**
     * ќчищает пол€ в модальном окне создани€ контакта
     */
    CreateClearFields() {
        $("#createEmpContactType").val("");
        $("#createEmpContactPhone").val("");
        $("#createEmpContactEmail").val("");
        $(".dropdown-el input").map(item => $(item).removeAttr("checked"))
        let contactNoneType = $(".dropdown-el input")[0];
        $(contactNoneType).prop('checked', true);
    }

    /**
     * ƒействи€ при отмене создани€ контакта
     */
    CancelCreate() {
        $("#empContactCreateModal").modal("hide");
        this.CreateClearFields();
        Utils.ClearErrors();
    }

    /**
     * ¬ыполн€ет запрос на сервер дл€ получени€ сведений о контакте при его редактировании
     * ≈сли все успешно, полученными данными заполн€ет пол€
     * ѕри наличии ошибок обрабатывает их
     * @param {*} event 
     */
    Initialize(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let editItemBtn = $(event.currentTarget).closest(".edit-item-btn");
            let getContactUrl = $(editItemBtn).find(".edit-item-url a").attr("href");
            
            request.JsonGetRequest(getContactUrl)
                .catch(() => reject())
                .then(response => {
                    this.InitializeFields(response);
                    resolve();
                })
        })
    }

    /**
     * «аполн€ет пол€ в модальном окне редактировании контакта полученными с сервера значени€ми
     * @param {*} contactData данные о контакте
     */
    InitializeFields(contactData) {
        localStorage.setItem("contactData", contactData);
        let dropdownElement = $("#updateEmpContactType").closest(".dropdown-area").find(".dropdown-el");
        $(dropdownElement).find("input").map((index, item) => {
            if ($(item).attr("value") == contactData["contactType"]) {
                $(item).prop("checked", true);
            }
            else $(item).removeAttr("checked");
        })
        $("#empContactId").val(contactData["id"]);
        $("#updateEmpContactType").val(contactData["contactType"]);
        $("#updateEmpContactPhone").val(contactData["phoneNumber"]);
        $("#updateEmpContactEmail").val(contactData["email"]);
    }

    /**
     * ¬ыполн€ет запрос на сервер, обновл€€ контакта
     * ѕри наличии ошибок обрабатывает их
     */
    Update() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let updateEmpContactUrl = $("#empContactUpdateModal form").attr("action");
            let updateEmpContactData = this.UpdateGetData();

            request.JsonPostRequest(updateEmpContactUrl, updateEmpContactData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdateEmployeeContact"]);
                })
                .done(() => {
                    $("#empContactUpdateModal").modal("hide");
                    location.reload();
                });
        })
    }

    /**
     * ‘ормирует данные дл€ отправки запроса на сервер при обновлении контакта
     */
    UpdateGetData() {
        let contactType = $("#updateEmpContactType").val();
        if (contactType.length == 0) {
            contactType = "None";
        }
        return {
            Id: $("#empContactId").val(),
            EmployeeId: $("#employeeId").val(),
            ContactType: contactType,
            PhoneNumber: $("#updateEmpContactPhone").val(),
            Email: $("#updateEmpContactEmail").val()
        }
    }

    /**
     * ƒействи€, выполн€емые при отмене редактировании контакта
     */
    CancelUpdate() {
        $("#empContactUpdateModal").modal("hide");
        let contactData = localStorage.getItem("contactData");
        this.InitializeFields(contactData);
        Utils.ClearErrors();
    }

    /**
     * ¬ыполн€ет запрос на сервер, удал€€ контакт из списка
     * ѕри наличии ошибок обрабатывает их
     * @param {*} event 
     */
    Remove(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let removeEmpContactUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");

            request.CommonDeleteRequest(removeEmpContactUrl)
                .fail(() => {
                    Swal.fire(MessageManager.Invoke("RemoveEmployeeContactError"));
                    reject();
                })
                .done(() => location.reload());
        })
    }
}

// —писок контактов сотрудника
$("#employeeContactsList")
    .off("click", ".remove-item-btn").on("click", ".remove-item-btn", event => {
        event.preventDefault();
        let employeeContact = new EmployeeContact();
        employeeContact.Remove(event);
    })
    .off("click", ".edit-item-btn").on("click", ".edit-item-btn", event => {
        event.preventDefault();
        let employeeContact = new EmployeeContact();
        employeeContact.Initialize(event);
    });

// ћодальное окно создани€ контакта сотрудника
$("#empContactCreateModal")
    .off("click", "#createEmpContactBtn").on("click", "#createEmpContactBtn", event => {
        event.preventDefault();
        let employeeContact = new EmployeeContact();
        employeeContact.Create();
    })
    .off("click", "#cancelCreationEmpContactBtn").on("click", "#cancelCreationEmpContactBtn", event => {
        event.preventDefault();
        let employeeContact = new EmployeeContact();
        employeeContact.CancelCreate();
    });

// ћодальное окно изменени€ контакта сотрудника
$("#empContactUpdateModal")
    .off("click", "#updateEmpContactBtn").on("click", "#updateEmpContactBtn", event => {
        event.preventDefault();
        let employeeContact = new EmployeeContact();
        employeeContact.Update();
    })
    .off("click", "#cancelUpdateEmpContactBtn").on("click", "#cancelUpdateEmpContactBtn", event => {
        event.preventDefault();
        let employeeContact = new EmployeeContact();
        employeeContact.CancelUpdate();
    });