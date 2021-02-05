class Account {
    /** Создание клиента */
    Create() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let createEmpUrl = $("#accountModal form").attr("action");
            let createEmpData = this.CreateGetData();
            let accountType = createEmpData["AccountType"];

            request.JsonPostRequest(createEmpUrl, createEmpData)
                .fail(response => {
                    switch(accountType) {
                        case "Individual":
                            Utils.CommonErrosHandling(response["responseJSON"], ["CreateAccount", "CreateAccountIndividual"]);
                            break;
                        case "IndividualEntrepreneur":
                            Utils.CommonErrosHandling(response["responseJSON"], ["CreateAccount", "CreateAccountIE"]);
                            break;
                        case "LegalEntity":
                            Utils.CommonErrosHandling(response["responseJSON"], ["CreateAccount", "CreateAccountLE"]);
                            break;
                    }
                })
                .done(newAccountUrl => {
                    $("#accountModal").modal("hide");
                    location.replace(newAccountUrl);
                });
        })
    }

    /** Получение данных для отправки при создании клиента */
    CreateGetData() {
        let accountType = this.GetAccountType();
        let selectedOrg = $($("#userOrgsChoiseList").find(".active")[0]);
        return {
            FirstName: $("#accFirstName").val(),
            LastName: $("#accLastName").val(),
            MiddleName: $("#accMiddleName").val(),
            Name: accountType == "IndividualEntrepreneur" ? $("#accNameIE").val() : $("#accNameLE").val(),
            INN: this.GetAccountINN(accountType),
            KPP: $("#accKPP").val(),
            OKPO: $("#accOKPO").val(),
            OGRN: $("#accOGRN").val(),
            Country: $("#accCountry .autocomplete-input").val(),
            AppointMe: $("#appointMeBody").find(".oval-mark-check").length > 0,
            OrganizationId: $(selectedOrg).find(".choise-userorg-id").text(),
            PrimaryManagerInitialName: $("#accManager .autocomplete-input").val(),
            AccountType: accountType
        }
    }

    /** Показывает, выбрал ли пользователь физ. лицо, ип или юр. лицо как тип клиента */
    GetAccountType() {
        let check = $("#accountModal .radio-tabs .form-check");
        if ($(check[0]).hasClass("active")) return "Individual";
        else if ($(check[1]).hasClass("active")) return "IndividualEntrepreneur";
        return "LegalEntity";
    }

    /**
     * В зависимости от типа клиента возвращает поле, где записан его ИНН
     * @param {*} accountType тип клиента
     */
    GetAccountINN(accountType) {
        let INN = "";
        switch(accountType) {
            case "Individual":
                INN = $("#accINNIndividual").val();
                break;
            case "IndividualEntrepreneur":
                INN = $("#accINNIE").val();
                break;
            case "LegalEntity":
                INN = $("#accINNLE").val();
                break;
        }
        return INN;
    }

    /** Очищает поля в модальном окне создания клиента */
    CreateClearFields() {
        ["#accCountry .autocomplete-input", "#accManager .autocomplete-input"]
            .map(item => $(item).val(""));
        ["#accFirstName", "#accLastName", "#accMiddleName", "#accINNIndividual", "#accNameIE", "#accINNIE", "#accNameLE", "#accINNLE", "#accKPP", "#accOKPO", "#accOGRN"]
            .map(item => $(item).val(""));  
    }

    /** Действия при отмене создания клиента */
    CancelCreate() {
        $("#accountModal").modal("hide");
        setTimeout(() => {
            // Установка первой организации пользователя как выбранную
            $("#userOrgsChoiseList .active").each((index, item) => {
                $(item).removeClass("active");
            })
            $($("#userOrgsChoiseList .list-group-item")[0]).addClass("active");

            // Проставление галки "Назначить меня"
            let mark = $("#appointMeBody").find(".oval-mark");
            if (mark.length > 0) {
                let block= new Block();
                block.OvalCheckmarkCheck($(mark));
            }

            // Проставление поля с выбором менеджера в "ридонли"
            $("#accManagerVal").attr("readonly", true);

            // Очистка полей, ошибок и проставление вкладки по умолчанию
            this.CreateClearFields();
            Utils.ClearErrors();
            Account.SetDefaultTab();
        }, 150);
    }

    /** Изменение сайта клиента */
    ChangeSite(event) {
        let siteInput = $(event.currentTarget).closest(".input-group").find(".form-control-url a");
        let changeSiteUrl = $(event.currentTarget).find(".icon-pencil").attr("href");
        Swal.fire(MessageManager.Invoke("NewSiteMessage", {
            "siteInput": siteInput,
            "changeSiteUrl": changeSiteUrl
        }));
    }

    /**
     * Метод открывает модальное окно с изменением юридического адреса клиента
     * @param {*} event 
     */
    ChangeLegalAddressShowModal(event) {
        return new Promise((resolve, reject) => {
            this.HasAccNotLegalAddress()
                .catch(error => reject(error))
                .then(hasAccounts => {
                    // Если нет адресов выводится сообщение об ошибке
                    if (!hasAccounts) {
                        Swal.fire(MessageManager.Invoke("AddressListIsEmpty"))
                    }

                    // Иначе открывается модальное окно с выбором адреса
                    else {
                        $("#changeLEAddrModal").find("#targetFormId").val("accountForm");
                        $("#changeLEAddrModal").modal("show");
                        let allAddresses = $("#changeLEAddrModal #accAddressNotLegalList").find(".list-group-item");
                        Array.from($(allAddresses)).map((address, index) => {
                            if (index == 0) {
                                $(address).addClass("active");
                            }
                            else {
                                $(address).removeClass("active");
                            }
                        })

                        let dropdown = new Dropdowns();
                        dropdown.SetDropdownValue("changeAccAddressType", "None");
                        $("#changeAccAddressType").val("None");
                    }
                });
        });
    }

    /**
     * Метод получает признак, имеются ли на клиенте адреса за исключением юридического
     */
    HasAccNotLegalAddress() {
        return new Promise((resolve, reject) => {
            let accountId = $("#accountId").val();
            let getAddressesUrl = LocalizationManager.GetUri("hasAccNotLegalAddress") + accountId;
            let request = new AjaxRequests();
            request.JsonGetRequest(getAddressesUrl)
                .catch(error => reject(error))
                .then(response => resolve(response));
        })
    }

    /**
     * Изменение юридического адреса клиента в зависимости от того, откуда было открыто модальное окно
     */
    ChangeLegalAddressDispatcher(event) {
        return new Promise((resolve, reject) => {
            let modal = $(event.currentTarget).closest("#changeLEAddrModal");
            let targetFormId = $(modal).find("#targetFormId").val();

            // В зависимости от того, откда было открыто модальное окно
            switch(targetFormId) {
                // Если оно было открыто из окна изменения данных адреса
                // Необходимо запомнить выбранные значения, так как запрос с этими данными пойдет на сервер только при подтверждении изменений в адресе
                case "accAddressUpdateModal":
                    this.ChangeLegalAddressFromUpdateModal();
                    break;
                    
                // Если оно было открыто с формы клиента, необходимо сразу изменить юридический адрес клиента
                case "accountForm":
                    this.ChangeLegalAddressFromAccountForm();
                    break;
            }
            
        })
    }

    /**
     * Изменение типа юридического адреса, запускаемое из модального окна изменения данных адреса
     */
    ChangeLegalAddressFromUpdateModal() {
        let selectedAddressId = this.GetSelectedAddressId();
        localStorage.setItem("selectedAddressId", selectedAddressId);
        $("#changeLEAddrModal").modal("hide");
    }

    /**
     * Изменение типа юридического адреса,запускаемое с формы клиента
     */
    ChangeLegalAddressFromAccountForm() {
        return new Promise((resolve, reject) => {
            let changeLegalAddressUrl = $("#changeLEAddrModal").find("form").attr("action");
            let changeLegalAddressData = this.ChangeLegalAddressGetData();
            let request = new AjaxRequests();
            let needStop = false;

            request.JsonPostRequest(changeLegalAddressUrl, changeLegalAddressData)
                .catch(response => {
                    needStop = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["ChangeLegalAddress"]);
                    reject(response);
                })
                .then(() => {
                    if (!needStop) {
                        Swal.fire(MessageManager.Invoke("LegalAddressHasBeenChanged")).then(() => location.reload());
                    }
                    else reject();
                })
        })
    }

    /**
     * Метод возвращает данные, необходимые при изменении юридического адреса клиента
     */
    ChangeLegalAddressGetData() {
        return {
            AccountId: $("#accountId").val(),
            NewLegalAddressId: this.GetSelectedAddressId(),
            CurrentAddressNewType: $("#changeAccAddressType").val()
        }
    }

    /**
     * Метод возвращает id выбранного абреса из модального окна выбора нового юридического адреса
     */
    GetSelectedAddressId() {
        // Получение выбранного адреса
        let allAddresses = $("#changeLEAddrModal #accAddressNotLegalList").find(".list-group-item");
        let selectedAddress = Array.from(allAddresses).filter(address => {
            return $(address).hasClass("active");
        })[0];

        // Получение id выбранного адреса
        let selectedAddressId = "";
        if (selectedAddress != undefined) {
            selectedAddressId = $(selectedAddress).find("p").text();
        }

        return selectedAddressId;
    }

    /**
     * Отмена изменения юридического адреса клиента
     */
    CancelChangeLegalAddress() {
        let addressList = $("#changeLEAddrModal #accAddressNotLegalList");
        Array.from($(addressList).find(".list-group-item")).map((address, index) => {
            if (index == 0) {
                $(address).addClass("active");
            }
            else {
                $(address).removeClass("active");
            }
        });
    }

    /** Удаляет клиента, если пользователь дал согласие */
    Remove(event) {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemoveAccountConfirmation")).then(result => {
                if (result["value"]) {
                    let removeAccUrl = $(event.currentTarget).attr("href");
                    let request = new AjaxRequests();
                    request.CommonDeleteRequest(removeAccUrl)
                        .fail(response => {
                            Utils.DefaultErrorHandling(response["responseJSON"]);
                            reject();
                        })
                        .done(() => {
                            let accountsBackUrl = $("#accountsBackUrl a").attr("href");
                            location.replace(location.origin + accountsBackUrl);
                        });
                }
            })
        })
    }

    /**
     * Обновление клиента
     * @param {*} event 
     */
    Update(event) {
        return new Promise((resolve, reject) => {
            let updateAccUrl = $(event.currentTarget).closest("#accountForm").find("form").attr("action");
            let updateAccData = this.UpdateGetData();
            let request = new AjaxRequests();
            request.JsonPostRequest(updateAccUrl, updateAccData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdateAccount"]);
                })
                .done(updatedAccUrl => location.replace(location.origin + updatedAccUrl))
        });
    }

    UpdateGetData() {
        let accountType = $("#AccountType").val();
        let kpp, okpo, ogrn = "";
        if (accountType == "LegalEntity") {
            kpp = $("#accountKPP").val();
            okpo = $("#accountOKPO").val();
            ogrn = $("#accountOGRN").val();
        }
        return {
            Id: $("#accountId").val(),
            OrganizationId: $("#OrganizationId").val(),
            AccountType: accountType,
            Name: $("#accountName").val(),
            INN: $("#accountINN").val(),
            KPP: kpp,
            OKPO: okpo,
            OGRN: ogrn
        }
    }

    AddAccManager(event) {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let modal = $(event.currentTarget).closest("#addAccountManagerModal");
            let addAccManagerUrl = $(modal).find("form").attr("action");
            let addAccManagerData = this.AddAccManagerGetData();
            let request = new AjaxRequests();
            request.CommonPostRequest(addAccManagerUrl, addAccManagerData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["AddAccManager"]);
                })
                .done(accUrl => {
                    $("#addAccountManagerModal").hide();
                    location.replace(accUrl);
                });
        })
    }

    AddAccManagerGetData() {
        return {
            Id: $("#accountId").val(),
            NewPrimaryManagerId: $("#accManagerId").val()
        }
    }

    /**
     * Устанавливает вкладку по умолчанию в модальном окне создания клиента
     */
    static SetDefaultTab() {
        let modal = $("#accountModal");
        let checkGroup = modal.find(".radio-tabs .form-check");
        modal.modal("hide");
        $(checkGroup).removeClass("active");
        $(checkGroup).find("input").removeAttr("checked");
        $(checkGroup[0]).addClass("active").find("input").prop("checked", "checked");
        modal.find(".tabs-content .tabs-content-item").hide();
        modal.find("#IndividualOption").fadeIn();
    }
}

// Модальное окно создания клиента
$("#accountModal")
    .off("click", "#createAccBtn").on("click", "#createAccBtn", event => {
        event.preventDefault();
        let account = new Account();
        account.Create();
    })
    .off("click", "#cancelCreationAccBtn").on("click", "#cancelCreationAccBtn", event => {
        event.preventDefault();
        let account = new Account();
        account.CancelCreate();
    })
    .off("click", ".oval-mark").on("click", ".oval-mark", event => {
        event.preventDefault();
        let block= new Block();
        block.OvalCheckmarkCheck($(event.currentTarget));
        $("#accManagerVal").attr("readonly", true); 
    })
    .off("click", ".oval-mark-check").on("click", ".oval-mark-check", event => {
        event.preventDefault();
        let block= new Block();
        block.OvalCheckmarkCheck($(event.currentTarget));
        $("#accManagerVal").attr("readonly", false); 
    })
    .off("click", "#userOrgsChoiseList .list-group-item").on("click", "#userOrgsChoiseList .list-group-item", event => {
        event.preventDefault();
        if (!$(event.currentTarget).hasClass("active")) {
            $("#userOrgsChoiseList .active").each((index, item) => {
                $(item).removeClass("active");
            })
            $(event.currentTarget).addClass("active");
        }
    });

// Список клиентов
$("#accountsForm")
    .off("click", ".account-item").on("click", ".account-item", event => {
        let navTab = new Tab();
        navTab.ClearAccTab();
    })
    .off("click", "#accountTabs .nav-item").on("click", "#accountTabs .nav-item", event => {
        let navTab = new NavTab();
        navTab.Remember(event, "currentAccountsTab");
    });

// Карточка клиента
$("#accountForm")
    .off("click", ".change-url-btn").on("click", ".change-url-btn", event => {
        let account = new Account();
        account.ChangeSite(event);
    })
    .off("click", ".change-address-btn").on("click", ".change-address-btn", event => {
        let account = new Account();
        account.ChangeLegalAddressShowModal(event);
    })
    .off("click", "#showAllManagers").on("click", "#showAllManagers", event => {
        $("#accountTeam").find(".account-manager").each((index, managerItem) => {
            // Пропуск карточки первого менеджера и разворот остальных
            if (index != 0) {
                $(managerItem).slideToggle();
            }
        })
    })
    .off("click", "#accTeamManagementBtn").on("click", "#accTeamManagementBtn", event => {
        let accountManagement = new AccountManagement();
        accountManagement.InitializeAccTeam();
    })
    .off("click", "#updateAccBtn").on("click", "#updateAccBtn", event => {
        event.preventDefault();
        let account = new Account();
        account.Update(event);
    })
    .off("click", "#removeAccBtn").on("click", "#removeAccBtn", event => {
        event.preventDefault();
        let account = new Account();
        account.Remove(event);
    });

$("#accountAddInfoForm")
    .off("click", ".vert-nav-tab").on("click", ".vert-nav-tab", event => {
        let vertNavTab = new VertNavTab();
        vertNavTab.Remember(event, "currentAccTab");
    });

// Изменение юридического адреса клиента
$("#changeLEAddrModal")
    .off("click", "#accAddressNotLegalList .list-group-item").on("click", "#accAddressNotLegalList .list-group-item", event => {
        if (!$(event.currentTarget).hasClass("active")) {
            let allAddresses = $("#changeLEAddrModal #accAddressNotLegalList").find(".list-group-item");
            Array.from(allAddresses).map(address => $(address).removeClass("active"));
            $(event.currentTarget).addClass("active");
        }
    })
    .off("click", "#changeLegalAddress").on("click", "#changeLegalAddress", event => {
        let account = new Account();
        account.ChangeLegalAddressDispatcher(event);
    })
    .off("click", "#cancelChangeLegalAddress").on("click", "#cancelChangeLegalAddress", event => {
        setTimeout(() => {
            let account = new Account();
            account.CancelChangeLegalAddress();
        }, 300);
    });

$("#addAccountManagerModal").off("click", "#addAccManager").on("click", "#addAccManager", event => {
    let account = new Account();
    account.AddAccManager(event);
})