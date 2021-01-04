class AccountInvoice {
    /**
     * Создание реквизита
     * @param {*} event 
     */
    Create(event) {
        this.Clear();
        let filter = $(event.currentTarget).closest("#accInvoiceCreateModal");
        let accInvoiceCreateUrl = $(filter).find("form").attr("action");
        let accInvoiceCreateData = this.CreateGetData();
        let request = new AjaxRequests();

        request.JsonPostRequest(accInvoiceCreateUrl, accInvoiceCreateData)
            .fail(response => {
                Utils.CommonErrosHandling(response["responseJSON"], ["CreateAccountInvoice"]);
            })
            .done(() => {
                $("#accInvoiceCreateModal").modal("hide");
                location.reload();
            });
    }

    /** Метод возвращает данные, необходимы для создание реквизита */
    CreateGetData() {
        return {
            AccountId: $("#accountId").val(),
            BankName: $("#createAccInvoiceBankName").val(),
            City: $("#createAccInvoiceCity").val(),
            CheckingAccount: $("#createAccInvoiceChecking").val(),
            CorrespondentAccount: $("#createAccInvoiceCorrespondent").val(),
            BIC: $("#createAccInvoiceBIC").val(),
            SWIFT: $("#createAccInvoiceSWIFT").val()
        }
    }

    /** Отмена создания реквизита */
    CancelCreate() {
        $("#accInvoiceCreateModal").modal("hide");
        this.CreateClearFields();
        this.Clear();
    }

    /** Очищает поля в модальном окне создания реквизита */
    CreateClearFields() {
        ["createAccInvoiceBankName", "createAccInvoiceCity", "createAccInvoiceChecking", "createAccInvoiceCorrespondent", "createAccInvoiceBIC", "createAccInvoiceSWIFT"].map(item => $("#" + item).val())
    }

    /**
     * Выполняет запрос на сервер для получения сведений о реквизитах при их редактировании
     * Если все успешно, полученными данными заполняет поля
     * @param {*} event 
     */
    Initialize(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let editItemBtn = $(event.currentTarget).closest(".edit-item-btn");
            let getInvoiceUrl = $(editItemBtn).find(".edit-item-url a").attr("href");
            
            request.JsonGetRequest(getInvoiceUrl)
                .fail(() => reject())
                .done(response => {
                    this.InitializeFields(response);
                    resolve();
                })
        })
    }

    /**
     * Заполняет поля в модальном окне редактировании реквизита полученными с сервера значениями
     * @param {*} invoiceData данные об адресе
     */
    InitializeFields(invoiceData) {
        localStorage.setItem("accountinvoiceData", invoiceData);
        $("#accInvoiceId").val(invoiceData["id"]);
        $("#updateAccInvoiceBankName").val(invoiceData["bankName"]);
        $("#updateAccInvoiceCity").val(invoiceData["city"]);
        $("#updateAccInvoiceChecking").val(invoiceData["checkingAccount"]);
        $("#updateAccInvoiceCorrespondent").val(invoiceData["correspondentAccount"]);
        $("#updateAccInvoiceBIC").val(invoiceData["bic"]);
        $("#updateAccInvoiceSWIFT").val(invoiceData["swift"]);
    }


    /**
     * Обновление реквизитов
     * @param {*} event 
     */
    Update(event) {
        return new Promise((resolve, reject) => {
            let accInvoiceUpdateUrl = $(event.currentTarget).closest("#accInvoiceUpdateModal").find("form").attr("action");
            let accInvoiceUpdateData = this.UpdateGetData();
            let request = new AjaxRequests();

            request.JsonPostRequest(accInvoiceUpdateUrl, accInvoiceUpdateData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdateAccountInvoice"]);
                })
                .done(() => {
                    $("#accInvoiceUpdateModal").modal("hide");
                    location.reload();
                });
        });
    }

    /** Метод возвращает данные, необходимы для изменения реквизита */
    UpdateGetData() {
        return {
            Id: $("#accInvoiceId").val(),
            AccountId: $("#accountId").val(),
            BankName: $("#updateAccInvoiceBankName").val(),
            City: $("#updateAccInvoiceCity").val(),
            CheckingAccount: $("#updateAccInvoiceChecking").val(),
            CorrespondentAccount: $("#updateAccInvoiceCorrespondent").val(),
            BIC: $("#updateAccInvoiceBIC").val(),
            SWIFT: $("#updateAccInvoiceSWIFT").val()
        }
    }

    /** Отмена изменения реквизита */
    CancelUpdate() {
        $("#accInvoiceUpdateModal").modal("hide");
        this.UpdateClearFields();
        this.Clear();
    }

    /** Очищает поля в модальном окне изменения реквизита */
    UpdateClearFields() {
        ["updateAccInvoiceBankName", "updateAccInvoiceCity", "updateAccInvoiceChecking", "updateAccInvoiceCorrespondent", "updateAccInvoiceBIC", "updateAccInvoiceSWIFT"].map(item => $("#" + item).val())
    }

    /**
     * Удаление адреса клиента
     * @param {*} event 
     */
    Remove(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let removeAccInvoiceUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");

            request.CommonDeleteRequest(removeAccInvoiceUrl)
                .fail(response => {
                    Utils.DefaultErrorHandling(response["responseJSON"]);
                    reject();
                })
                .done(() => location.reload());
        });
    }

    /** Очищает поля с ошибками */
    Clear() {
        $('.under-field-error').empty();
    }
}

$("#accInvoicesList")
    .off("click", ".remove-item-btn").on("click", ".remove-item-btn", event => {
        event.preventDefault();
        let accountInvoice = new AccountInvoice();
        accountInvoice.Remove(event);
    })
    .off("click", ".edit-item-btn").on("click", ".edit-item-btn", event => {
        event.preventDefault();
        let accountInvoice = new AccountInvoice();
        accountInvoice.Initialize(event);
    });

$("#accInvoiceCreateModal")
    .off("click", "#createAccInvoiceBtn").on("click", "#createAccInvoiceBtn", event => {
        event.preventDefault();
        let accountInvoice = new AccountInvoice();
        accountInvoice.Create(event);
    })
    .off("click", "#cancelCreationAccInvoiceBtn").on("click", "#cancelCreationAccInvoiceBtn", event => {
        event.preventDefault();
        let accountInvoice = new AccountInvoice();
        accountInvoice.CancelCreate(event);
    });
    
$("#accInvoiceUpdateModal")
    .off("click", "#updateAccInvoiceBtn").on("click", "#updateAccInvoiceBtn", event => {
        event.preventDefault();
        let accountInvoice = new AccountInvoice();
        accountInvoice.Update(event);
    })
    .off("click", "#cancelUpdateAccInvoiceBtn").on("click", "#cancelUpdateAccInvoiceBtn", event => {
        event.preventDefault();
        let accountInvoice = new AccountInvoice();
        accountInvoice.CancelUpdate(event);
    });