class Quote {
    /** Создание сделки */
    Create() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let createEmpUrl = $("#quoteCreateModal form").attr("action");
            let createEmpData = this.CreateGetData();

            request.JsonPostRequest(createEmpUrl, createEmpData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateQuote"]);
                })
                .done(() => {
                    $("#quoteCreateModal").modal("hide");
                    location.reload();
                });
        });
    }

    /** Получение данных для отправки при создании сделки */
    CreateGetData() {
        return {
            Amount: $("#amountName").val(),
            AccountName: $("#quoteAccount").find(".autocomplete-input").val(),
            ManagerInitialName: $("#quoteManager").find(".autocomplete-input").val()
        }
    }

    CancelCreate() {
        $("#quoteCreateModal").modal("hide");
        this.CreateClearFields();
        Utils.ClearErrors();
    }

    /** Очищает поля в модальном окне создания сделки */
    CreateClearFields() {
        // ["#accCountry .autocomplete-input", "#accManager .autocomplete-input"]
        //     .map(item => $(item).val(""));
        // ["#accFirstName", "#accLastName", "#accMiddleName", "#accINNIndividual", "#accNameIE", "#accINNIE", "#accNameLE", "#accINNLE", "#accKPP", "#accOKPO", "#accOGRN"]
        //     .map(item => $(item).val(""));
    }
}


$("#quotesForm")
    .off("click", "#quoteTabs .nav-item").on("click", "#quoteTabs .nav-item", event => {
        let navTab = new NavTab();
        navTab.Remember(event, "currentQuotesTab");
        let navConnectedTab = new NavConnectedTab();
        navConnectedTab.Remember(event, "currentQuotesConnectedTab");
    })

$("#quoteCreateModal")
    .off("click", "#createQuoteBtn").on("click", "#createQuoteBtn", event => {
        let quote = new Quote();
        quote.Create();
    })
    .off("click", "#cancelCreationQuoteBtn").on("click", "#cancelCreationQuoteBtn", event => {
        
    });