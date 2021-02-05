class AccountContact {
    /**
     * Создание контакта
     * @param {*} event 
     */
    Create(event) {
        return new Promise((resolve, reject) => {
            this.Clear();
            let modal = $(event.currentTarget).closest("#accContactCreateModal");
            let createContactUrl = $(modal).find("form").attr("action");
            let createContactData = this.CreateGetData();
            let request = new AjaxRequests();

            request.JsonPostRequest(createContactUrl, createContactData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateAccountContact"]);
                    reject(response);
                })
                .done(() => {
                    $("#accContactCreateModal").modal("hide");
                    location.reload();
                });
        });
    }

    /**
     * Получение данных, необходимых при создании контакта
     */
    CreateGetData() {
        let contactType = $("#createAccContactType").val();
        if (contactType.length == 0) {
            contactType = "None";
        }
        return {
            AccountId: $("#accountId").val(),
            FirstName: $("#createAccContactFName").val(),
            LastName: $("#createAccContactLName").val(),
            MiddleName: $("#createAccContactMName").val(),
            ContactType: contactType,
            PhoneNumber: $("#createAccContactPhone").val(),
            Email: $("#createAccContactEmail").val()
        }
    }

    /**
     * Отмена создания контакта
     */
    CancelCreate() {
        $("#accContactCreateModal").modal("hide");
        this.CreateClearFields();
        this.Clear();
    }

    /**
     * Очищает поля в модальном окне создания контакта
     */
    CreateClearFields() {
        $("#createAccContactFName").val("");
        $("#createAccContactLName").val("");
        $("#createAccContactMName").val("");
        $("#createAccContactPhone").val("");
        $("#createAccContactEmail").val("");
        $(".dropdown-el input").map(item => $(item).removeAttr("checked"));
        let contactNoneType = $(".dropdown-el input")[0];
        $(contactNoneType).prop('checked', true);
    }

    /**
     * Выполняет запрос на сервер для получения сведений о контакте при его редактировании
     * Если все успешно, полученными данными заполняет поля
     * При наличии ошибок обрабатывает их
     * @param {*} event 
     */
    Initialize(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let editItemBtn = $(event.currentTarget).closest(".edit-item-btn");
            let getContactUrl = $(editItemBtn).find(".edit-item-url a").attr("href");
            
            request.JsonGetRequest(getContactUrl)
                .fail(() => reject())
                .done(response => {
                    this.InitializeFields(response);
                    resolve();
                })
        })
    }

    /**
     * Заполняет поля в модальном окне редактировании контакта полученными с сервера значениями
     * @param {*} contactData данные о контакте
     */
    InitializeFields(contactData) {
        localStorage.setItem("accountContactData", contactData);
        let dropdownElement = $("#updateAccContactType").closest(".dropdown-area").find(".dropdown-el");
        $(dropdownElement).find("input").map((index, item) => {
            if ($(item).attr("value") == contactData["contactType"]) {
                $(item).prop("checked", true);
            }
            else $(item).removeAttr("checked");
        })
        $("#accContactId").val(contactData["id"]);
        $("#updateAccContactFName").val(contactData["firstName"]);
        $("#updateAccContactLName").val(contactData["lastName"]);
        $("#updateAccContactMName").val(contactData["middleName"]);
        $("#updateAccContactType").val(contactData["contactType"]);
        $("#updateAccContactPhone").val(contactData["phoneNumber"]);
        $("#updateAccContactEmail").val(contactData["email"]);
    }

    /**
     * Обновление контакта
     * @param {*} event 
     */
    Update(event) {
        return new Promise((resolve, reject) => {
            this.Clear();
            let modal = $(event.currentTarget).closest("#accContactUpdateModal");
            let updateContactUrl = $(modal).find("form").attr("action");
            let updateContactData = this.UpdateGetData();
            let request = new AjaxRequests();

            request.JsonPostRequest(updateContactUrl, updateContactData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdateAccountContact"]);
                    reject(response);
                })
                .done(() => {
                    $("#accContactUpdateModal").modal("hide");
                    location.reload();
                });
        });
    }

    /**
     * Получение данных, необходимых при обновлении контакта
     */
    UpdateGetData() {
        let contactType = $("#updateAccContactType").val();
        if (contactType.length == 0) {
            contactType = "None";
        }
        return {
            Id: $("#accContactId").val(),
            FirstName: $("#updateAccContactFName").val(),
            LastName: $("#updateAccContactLName").val(),
            MiddleName: $("#updateAccContactMName").val(),
            ContactType: contactType,
            PhoneNumber: $("#updateAccContactPhone").val(),
            Email: $("#updateAccContactEmail").val()
        }
    }

    /**
     * Отмена обновления контакта
     */
    CancelUpdate() {
        $("#accContactUpdateModal").modal("hide");
        this.Clear();
    }

    /**
     * Очищает поля с ошибками
     */
    Clear() {
        $('.under-field-error').empty();
    }

    /**
     * Удаление контакта клиента
     * @param {*} event 
     */
    Remove(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let removeAccContactUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");

            request.CommonDeleteRequest(removeAccContactUrl)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["RemoveAccountContact"]);
                    reject();
                })
                .done(() => location.reload());
        });
    }

    /**
     * Метод отображает/скрывает блок с сохранением измененного основного контакта
     * @param {*} event 
     */
    ShowSavePrimaryContactBlock(event) {
        let accForm = $("#accountAddInfoForm");
        let contactsList = $(accForm).find("#accContactsList");
        let markCheck = $(contactsList).find(".oval-mark-check");
        let primaryContactId = $("#PrimaryContactId").val();
        let row = $(event.currentTarget).closest("tr");
        let contactId = $(row).find(".contact-id").text();

        if (primaryContactId == "") {
            if (markCheck.length > 0) {
                $("#changePrimaryContact").slideDown("slow");
            }
            else {
                $("#changePrimaryContact").slideUp("slow");
            }
        }

        else {
            if (markCheck.length == 0) {
                $("#changePrimaryContact").slideDown("slow");
            }
            else {
                if (primaryContactId != contactId) {
                    $("#changePrimaryContact").slideDown("slow");
                }
                else {
                    $("#changePrimaryContact").slideUp("slow");
                }
            }
        }
    }

    /**
     * Метод проставляет конаткт основным
     * @param {*} event 
     */
    SetUpPrimaryContact(event) {
        let accountType = $("#AccountType").val();

        // В зависимости от типа клиента
        switch(accountType) {
            case "Individual":
                this.SetUpIndividualPrimaryContact(event);
                break;
            case "IndividualEntrepreneur":
            case "LegalEntity":
                this.SetUpIELEPrimaryContact(event);
                break;
        }

        this.ShowSavePrimaryContactBlock(event);
    }

    /**
     * Установка контакта основным для физических лиц (Для них отсутствует возможность удалить основной контакт)
     * @param {*} event 
     */
    SetUpIndividualPrimaryContact(event) {
        let block= new Block();
        let table = $(event.currentTarget).closest(".fl-table");
        let checkMarks = $(table).find(".oval-mark-readonly");

        // Скрытие галок для остальных выбранных контактов
        Array.from(checkMarks).map(item => {
            let currentPrimaryContactRow = $(item).closest("tr");
            let removeItemCell = $(currentPrimaryContactRow).find(".hide-remove-item-btn");
            $(removeItemCell).removeClass("hide-remove-item-btn").addClass("remove-item-btn");
            block.HideOvalCheckmarkReadonly(item);
        });

        // Проставление признака основного контакта для выбранной записи
        block.OvalCheckmarkReadonly($(event.currentTarget));

        // Для записи, помечаемой основной, скрывается кнопка удаления
        let newPrimaryContactRow = $(event.currentTarget).closest("tr");
        $(newPrimaryContactRow).find(".remove-item-btn").removeClass("remove-item-btn").addClass("hide-remove-item-btn");
    }

    /**
     * Установка контакта основным для ИП и юридических лиц (Для них есть возможность удалить основной контакт)
     * @param {*} event 
     */
    SetUpIELEPrimaryContact(event) {
        let block= new Block();
        let table = $(event.currentTarget).closest(".fl-table");
        let checkMarks = $(table).find(".oval-mark-check");

        // Скрытие галок для остальных выбранных контактов
        Array.from(checkMarks).map(item => block.HideOvalCheckmarkCheck(item));

        // Проставление(снятие) признака основного контакта для выбранной записи
        block.OvalCheckmarkCheck($(event.currentTarget));
    }

    /**
     * Изменение основного контакта
     */
    ChangePrimaryContact(event) {
        return new Promise((resolve, reject) => {
            let changePrimaryContactUrl = $(event.currentTarget).closest("form").attr("action");
            let changePrimaryContactData = this.ChangePrimaryContactGetData(event);
            let request = new AjaxRequests();
            
            request.JsonPostRequest(changePrimaryContactUrl, changePrimaryContactData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["ChangePrimaryContact"]);
                    reject(errors);
                })
                .done(() => {
                    Swal.fire(MessageManager.Invoke("PrimaryContactHasBeenChanged")).then(() => location.reload());
                    resolve();
                });
        });
    }

    /**
     * Метод возвращает информацию, необходимую для изменения основноо контакта клиента
     * @param {*} event 
     */
    ChangePrimaryContactGetData(event) {
        let accForm = $("#accountAddInfoForm");
        let contactsList = $(accForm).find("#accContactsList");

        // В зависимости от типа клиента, получение выбранного основного контакта происходит по разному
        let accountType = $("#AccountType").val();
        let markCheck = "";
        if (accountType == "Individual") {
            markCheck = $(contactsList).find(".oval-mark-readonly")[0];
        }
        else {
            markCheck = $(contactsList).find(".oval-mark-check")[0];
        }

        // Проставление основного контакта
        let primaryContactId = "";
        if (markCheck != undefined) {
            let row = $(markCheck).closest("tr");
            primaryContactId = $(row).find(".contact-id").text();
        }

        // Возврат
        return {
            PrimaryContactId: primaryContactId
        }
    }

    /**
     * Устанавливает/снимает признак основного контакта при фильтрации
     * @param {*} event 
     */
    SearchContactSetPrimary(event) {
        if ($(event.currentTarget).hasClass("oval-mark-check")) {
            $(event.currentTarget).removeClass("oval-mark-check").addClass("oval-mark").empty();
            $("#SearchContactPrimary").val(false);
        }
        else {
            $(event.currentTarget).addClass("oval-mark-check").removeClass("oval-mark").append("<div class='icon-checkmark'></div>");
            $("#SearchContactPrimary").val(true);
        }
    }

    /**
     * Поиск по конатктам
     * @param {*} event 
     */
    SearchContact(event) {
        return new Promise((resolve, reject) => {
            let filter = $(event.currentTarget).closest("#accContactsFilter");
            let searchContactUrl = $(filter).find("form").attr("action");
            let searchContactData = this.SearchContactGetData();
            let request = new AjaxRequests();
            console.log(searchContactUrl);
            console.log(searchContactData);

            request.JsonPostRequest(searchContactUrl, searchContactData)
                .fail(error => reject(error))
                .done(response => resolve());
        });
    }

    /**
     *  Метод возвращает данные, необходимые при поиске контакта
     */
    SearchContactGetData() {
        let isPrimary = $("#searchContactPrimarySign").hasClass("oval-mark-check");
        return {
            SearchContactFullName: $("#SearchContactFullName").val(),
            SearchContactType: $("#SearchContactType").val(),
            SearchContactEmail: $("#SearchContactEmail").val(),
            SearchContactPhoneNumber: $("#SearchContactPhoneNumber").val(),
            SearchContactPrimary: isPrimary,
        }
    }
}

// Список контактов клиента
$("#accContactsList")
    .off("click", ".remove-item-btn").on("click", ".remove-item-btn", event => {
        event.preventDefault();
        let accountContact = new AccountContact();
        accountContact.Remove(event);
    })
    .off("click", ".edit-item-btn").on("click", ".edit-item-btn", event => {
        event.preventDefault();
        let accountContact = new AccountContact();
        accountContact.Initialize(event);
    })
    .off("click", ".oval-mark").on("click", ".oval-mark", event => {
        event.preventDefault();
        let accountContact = new AccountContact();
        accountContact.SetUpPrimaryContact(event);
    })
    .off("click", ".oval-mark-check").on("click", ".oval-mark-check", event => {
        event.preventDefault();
        let block= new Block();
        block.OvalCheckmarkCheck($(event.currentTarget));
        let accountContact = new AccountContact();
        accountContact.ShowSavePrimaryContactBlock(event);
    });

// Модальное окно создание контакта
$("#accContactCreateModal")
    .off("click", "#createAccContactBtn").on("click", "#createAccContactBtn", event => {
        let accountContact = new AccountContact();
        accountContact.Create(event);
    })
    .off("click", "#cancelCreationAccContactBtn").on("click", "#cancelCreationAccContactBtn", event => {
        let accountContact = new AccountContact();
        accountContact.CancelCreate();
    });
    
// Модальное окно изменения контакта
$("#accContactUpdateModal")
    .off("click", "#updateAccContactBtn").on("click", "#updateAccContactBtn", event => {
        let accountContact = new AccountContact();
        accountContact.Update(event);
    })
    .off("click", "#cancelUpdateAccContactBtn").on("click", "#cancelUpdateAccContactBtn", event => {
        let accountContact = new AccountContact();
        accountContact.CancelUpdate();
    });

// Изменение основного контакта
$("#changePrimaryContact").off("click", "#changePrimaryContactBtn").on("click", "#changePrimaryContactBtn", event => {
    event.preventDefault();
    let accountContact = new AccountContact();
    accountContact.ChangePrimaryContact(event);
});

// Признак основного контакта на фильтре по контактам
$("#accContactsFilter")
    .off("click", ".oval-mark").on("click", ".oval-mark", event => {
        event.preventDefault();
        let accountContact = new AccountContact();
        accountContact.SearchContactSetPrimary(event);
    })
    .off("click", ".oval-mark-check").on("click", ".oval-mark-check", event => {
        event.preventDefault();
        let accountContact = new AccountContact();
        accountContact.SearchContactSetPrimary(event);
    });