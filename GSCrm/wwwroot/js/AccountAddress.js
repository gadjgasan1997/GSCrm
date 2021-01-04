class AccountAddress {
    /**
     * Создание адреса
     * @param {*} event 
     */
    Create(event) {
        return new Promise((resolve, reject) => {
            this.Clear();
            let modal = $(event.currentTarget).closest("#accAddressCreateModal");
            let createAddressUrl = $(modal).find("form").attr("action");
            let createAddressData = this.CreateGetData();
            let request = new AjaxRequests();

            request.JsonPostRequest(createAddressUrl, createAddressData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateAddress"]);
                    reject(response);
                })
                .done(() => {
                    $("#accAddressCreateModal").modal("hide");
                    location.reload();
                });
        });
    }

    /**
     * Получение данных, необходимых при создании адреса
     */
    CreateGetData() {
        let addressType = $("#createAccAddressType").val();
        if (addressType.length == 0) {
            addressType = "None";
        }
        return {
            AccountId: $("#accountId").val(),
            Country: $("#createAccAddressCountry").find(".autocomplete-input").val(),
            Region: $("#createAccAddressRegion").val(),
            City: $("#createAccAddressCity").val(),
            Street: $("#createAccAddressStreet").val(),
            House: $("#createAccAddressHouse").val(),
            AddressType: addressType,
        }
    }

    /**
     * Отмена создания адреса
     */
    CancelCreate() {
        $("#accAddressCreateModal").modal("hide");
        this.CreateClearFields();
        this.Clear();
    }

    /**
     * Очищает поля в модальном окне создания адреса
     */
    CreateClearFields() {
        ["createAccAddressCountry", "createAccAddressRegion", "createAccAddressCity", "createAccAddressStreet", "createAccAddressHouse"].map(item => $(item).val(""));
        let modal = $("#accAddressCreateModal");
        $(modal).find(".dropdown-el input").map(item => $(item).removeAttr("checked"));
        let addressNoneType = $(modal).find(".dropdown-el input")[0];
        $(addressNoneType).prop('checked', true);
    }

    /**
     * Выполняет запрос на сервер для получения сведений об адресе при его редактировании
     * Если все успешно, полученными данными заполняет поля
     * При наличии ошибок обрабатывает их
     * @param {*} event 
     */
    Initialize(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let editItemBtn = $(event.currentTarget).closest(".edit-item-btn");
            let getAddressUrl = $(editItemBtn).find(".edit-item-url a").attr("href");
            
            request.JsonGetRequest(getAddressUrl)
                .fail(() => reject())
                .done(response => {
                    this.InitializeFields(response);
                    resolve();
                })
        })
    }

    /**
     * Заполняет поля в модальном окне редактировании адреса полученными с сервера значениями
     * @param {*} addressData данные об адресе
     */
    InitializeFields(addressData) {
        localStorage.setItem("accountAddressData", addressData);
        let dropdown = new Dropdowns();
        dropdown.SetDropdownValue("updateAccAddressType", addressData["addressType"]);
        $("#accAddressId").val(addressData["id"]);
        $("#updateAccAddressCountry").find(".autocomplete-input").val(addressData["country"]);
        $("#updateAccAddressRegion").val(addressData["region"]);
        $("#updateAccAddressCity").val(addressData["city"]);
        $("#updateAccAddressStreet").val(addressData["street"]);
        $("#updateAccAddressHouse").val(addressData["house"]);
        $("#updateAccAddressType").val(addressData["addressType"]);
    }

    /**
     * Обновление адреса
     * @param {*} event 
     */
    Update(event) {
        return new Promise((resolve, reject) => {
            this.Clear();
            let modal = $(event.currentTarget).closest("#accAddressUpdateModal");
            let updateAddressUrl = $(modal).find("form").attr("action");
            let updateAddressData = this.UpdateGetData();
            let needStop = false;

            // Выбор другого юридического адреса при необходимости
            this.UpdateCheckLegalAddressNotChanged(updateAddressData)
                .catch(() => {
                    this.SetAddressTypeLegal();
                    needStop = true;
                    reject();
                })
                .then(response => {
                    if (!needStop && !response["WaitForChanges"]) {
                        let request = new AjaxRequests();
                        request.JsonPostRequest(updateAddressUrl, updateAddressData)
                            .fail(response => {
                                Utils.CommonErrosHandling(response["responseJSON"], ["UpdateAddress"]);
                                reject(response);
                            })
                            .done(() => {
                                localStorage.removeItem("selectedAddressId");
                                $("#accAddressUpdateModal").modal("hide");
                                location.reload();
                            });
                    }
                });
        });
    }

    /**
     * Получение данных, необходимых при обновлении адреса
     */
    UpdateGetData() {
        let addressType = $("#updateAccAddressType").val();
        if (addressType.length == 0) {
            addressType = "None";
        }
        return {
            Id: $("#accAddressId").val(),
            AccountId: $("#accountId").val(),
            Country: $("#updateAccAddressCountry").find(".autocomplete-input").val(),
            Region: $("#updateAccAddressRegion").val(),
            City: $("#updateAccAddressCity").val(),
            Street: $("#updateAccAddressStreet").val(),
            House: $("#updateAccAddressHouse").val(),
            AddressType: addressType,
            CurrentAddressNewType: addressType,
            NewLegalAddressId: localStorage.getItem("selectedAddressId")
        }
    }

    /**
     * Метод проверяет, что при обновлении юридического адреса его тип не изменился
     * Если же тип изменился, предлагает пользователю выбрать новый адрес в качестве юридического, а у этого изменить тип
     * @param {*} updateAddressData Данные, отправляемые на сервер при изменении адреса
     */
    UpdateCheckLegalAddressNotChanged(updateAddressData) {
        return new Promise((resolve, reject) => {
            let addressList = $("#accAddressesList");
            let addressesIdList = $(addressList).find(".address-id");

            // Поиск типа текущего изменяемого адреса
            let changedAddress = Array.from(addressesIdList).filter(addressIdElement => {
                return $(addressIdElement).text() == updateAddressData["Id"];
            });
            let changedAddressType = $(changedAddress).closest("tr").find(".address-type").text();
            let addressHasBeenSelected = localStorage.getItem("selectedAddressId") != "" && localStorage.getItem("selectedAddressId") != undefined && localStorage.getItem("selectedAddressId") != null;

            // Если новый тип адреса является юридическим, то в любом случае можно осуществлять запрос на сервер
            if (updateAddressData["CurrentAddressNewType"] == "Legal") resolve({ "WaitForChanges": false });

            // Иначе, если изначально тип адреса был юридическим, необходимо выбрать другой адрес как юридический
            else if (changedAddressType == Localization.GetString("Legal") && !addressHasBeenSelected) {

                // Подтверждение выбора другого юридического адреса у пользователя
                Swal.fire(MessageManager.Invoke("ChangeLegalAddressInfo")).then(dialogResult => {

                    // Если пользователь хочет изменить тип другого адреса
                    if (dialogResult["value"]) {
                        
                        // Получение признака наличия на клиенте адресов, кроме юридического
                        let account = new Account();
                        account.HasAccNotLegalAddress()
                            .catch(() => reject())
                            .then(hasAccounts => {
                                if (!hasAccounts) {
                                    Swal.fire(MessageManager.Invoke("AddressListIsEmpty"));
                                    reject();
                                }
                                else {
                                    // Если есть адреса, открывается окно изменения юридического адреса клиента
                                    $("#accAddressUpdateModal").modal("hide");
                                    $("#changeLEAddrModal").find("#changeCurrentAddrType").addClass("d-none");
                                    $("#changeLEAddrModal").find("#targetFormId").val("accAddressUpdateModal");
                                    $("#changeLEAddrModal").modal("show");
                                    resolve({ "WaitForChanges": true });
                                }
                            })
                    }
    
                    else reject();
                })
            }

            else resolve({ "WaitForChanges": false });
        })
    }

    /**
     * Происходит при скрытии модального окна с изменением юридического адреса
     */
    OnChangeLegalAddressModalHide() {
        // В зависимости от того, откда было открыто модальное окно
        let targetFormId = $("#changeLEAddrModal").find("#targetFormId").val();
        switch(targetFormId) {

            // Если оно было открыто из окна изменения данных адреса
            case "accAddressUpdateModal":
                $("#accAddressUpdateModal").modal("show");
                break;

            // Если оно было открыто с формы клиента, необходимо сбросить выпадающий список с выбором типа адреса
            case "accountForm":
                setTimeout(() => {
                    let dropdown = new Dropdowns();
                    dropdown.SetDropdownValue("changeAccAddressType", "None");
                    $("#changeAccAddressType").val("None");
                }, 300)
                break;
        }

        // Абсолютно во всех случаях убирается скрытие с блока выбора типа текущего адреса в модальном окне
        // Скрытие необходимо добавлять из-за того, что этот блок не требуется отображать при открытии этого окна из окна изменения данных адреса
        setTimeout(() => {
            $("#changeLEAddrModal").find("#changeCurrentAddrType").removeClass("d-none");
        }, 300);
    }

    /**
     * Метод устанвливает адрес юридическим
     */
    SetAddressTypeLegal() {
        $("#updateAccAddressType").val("Legal");
        let dropdown = new Dropdowns();
        dropdown.SetDropdownValue("updateAccAddressType", "Legal");
    }

    /**
     * Отмена обновления адреса
     */
    CancelUpdate() {
        localStorage.removeItem("selectedAddressId");
        $("#accAddressUpdateModal").modal("hide");
        this.Clear();
    }

    /**
     * Удаление адреса клиента
     * @param {*} event 
     */
    Remove(event) {
        return new Promise((resolve, reject) => {
            let request = new AjaxRequests();
            let removeAccAddressUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");

            request.CommonDeleteRequest(removeAccAddressUrl)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["RemoveAddress"]);
                    reject();
                })
                .done(() => location.reload());
        });
    }

    /**
     * Очищает поля с ошибками
     */
    Clear() {
        $('.under-field-error').addClass("d-none").empty();
    }
}

// Список адресов клиента
$("#accAddressesList")
    .off("click", ".remove-item-btn").on("click", ".remove-item-btn", event => {
        event.preventDefault();
        let accountAddress = new AccountAddress();
        accountAddress.Remove(event);
    })
    .off("click", ".edit-item-btn").on("click", ".edit-item-btn", event => {
        event.preventDefault();
        let accountAddress = new AccountAddress();
        accountAddress.Initialize(event);
    });

// Модальное окно создание адреса
$("#accAddressCreateModal")
    .off("click", "#createAccAddressBtn").on("click", "#createAccAddressBtn", event => {
        let accountAddress = new AccountAddress();
        accountAddress.Create(event);
    })
    .off("click", "#cancelCreationAccAddressBtn").on("click", "#cancelCreationAccAddressBtn", event => {
        let accountAddress = new AccountAddress();
        accountAddress.CancelCreate();
    });
    
// Модальное окно изменения адреса
$("#accAddressUpdateModal")
    .off("click", "#updateAccAddressBtn").on("click", "#updateAccAddressBtn", event => {
        let accountAddress = new AccountAddress();
        accountAddress.Update(event);
    })
    .off("click", ".close").on("click", ".close", event => {
        let accountAddress = new AccountAddress();
        accountAddress.CancelUpdate();
    })
    .off("click", "#cancelUpdateAccAddressBtn").on("click", "#cancelUpdateAccAddressBtn", event => {
        let accountAddress = new AccountAddress();
        accountAddress.CancelUpdate();
    });

$("#changeLEAddrModal")
    .off("click", ".close").on("click", ".close", event => {
        event.preventDefault();
        let accountAddress = new AccountAddress();
        accountAddress.SetAddressTypeLegal();
        accountAddress.OnChangeLegalAddressModalHide();
    })
    .off("click", "#cancelChangeLegalAddress").on("click", "#cancelChangeLegalAddress", event => {
        event.preventDefault();
        let accountAddress = new AccountAddress();
        accountAddress.SetAddressTypeLegal();
        accountAddress.OnChangeLegalAddressModalHide();
    })
    .off('hide.bs.modal').on("hide.bs.modal", event => {
        let accountAddress = new AccountAddress();
        accountAddress.OnChangeLegalAddressModalHide();
    });