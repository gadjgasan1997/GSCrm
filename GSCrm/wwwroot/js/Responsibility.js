class Responsibility {
    static switchers = {
        "Active": [],
        "Disable": []
    }

    /** Запоминание выбранных настроек */
    RememberFlags() {
        let switchArr = Array.from($("#responsibilityForm").find(".resp-setting .switch"));
        switchArr.map(switchItem => {
            if ($(switchItem).hasClass("switch-disable")) {
                Responsibility.switchers["Disable"].push($(switchItem).closest(".resp-setting").attr("id"))
            }
            else Responsibility.switchers["Active"].push($(switchItem).closest(".resp-setting").attr("id"))
        });
    }

    /** Метод инициализирует переключатели групп настроек */
    SetupGroupSwtichers() {
        // Для каждой группы ищутся отключенные элементы
        // Если есть хотя бы один такой, то флаг группы также становится откюченным
        let allGroups = Array.from($("#responsibilityForm .resp-setting-group"));
        allGroups.map(group => this.HasDisabledSettings(group));
    }

    /**
     * Метод определяет для поданной на вход группы, имеются ли вней отключенный элементы
     * Если таковых имеются, то включает флаг для всей группы, иначе отключает 
     * @param {*} group 
     */
    HasDisabledSettings(group) {
        let groupBlock = $(group).parent();
        let hasDisabledSettings = false;
        Array.from($(groupBlock).find(".resp-setting")).map(respSetting => {
            if (!hasDisabledSettings && $(respSetting).find(".switch").hasClass("switch-disable")) {
                hasDisabledSettings = true;
            }
        });

        // Проверка фалага наличия отключенных элементов
        if (!hasDisabledSettings) {
            $(group).find(".switch").removeClass("switch-disable").addClass("switch-active");
        }
        else $(group).find(".switch").removeClass("switch-active").addClass("switch-disable");
    }

    /**
     * Проверяет наличие изменений
     * @param {*} event 
     */
    CheckForChanges(event) {
        setTimeout(() => {
            let switchArr = Array.from($("#responsibilityForm").find(".resp-setting .switch"));

            // Возможности, помеченные активными для полномочия
            let activeItems = switchArr.filter(switchItem => {
                if ($(switchItem).hasClass("switch-active")) return switchItem;
            }).map(switchItem => {
                return $(switchItem).closest(".resp-setting").attr("id");
            });

            // Возможности, отключенные для полномочия
            let disableItems = switchArr.filter(switchItem => {
                if ($(switchItem).hasClass("switch-disable")) return switchItem;
            }).map(switchItem => {
                return $(switchItem).closest(".resp-setting").attr("id");
            });

            if (!Utils.CheckArraysSame(Responsibility.switchers["Active"], activeItems)) {
                $("#responsibilityForm #responsibilityActions").find("button").removeAttr("disabled");
            }

            else if (!Utils.CheckArraysSame(Responsibility.switchers["Disable"], disableItems)) {
                $("#responsibilityForm #responsibilityActions").find("button").removeAttr("disabled");
            }

            else $("#responsibilityForm #responsibilityActions").find("button").attr("disabled", "disabled");
        });
    }

    Update(event) {
        return new Promise((resolve, reject) => {
            let updateRespData = this.UpdateGetData();
            let updateRespUrl = $(event.currentTarget).closest("form").attr("action");
            let request = new AjaxRequests();
            request.CommonPostRequest(updateRespUrl, updateRespData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["UpdateResponsibility"]);
                })
                .done(() => location.reload());
        })
    }

    UpdateGetData() {
        return {
            Id: $("#respId").val(),
            Name: $("#orgName").text(),
            OrganizationId: $("#orgId").val(),
            OrgDelete: $("#OrgDeleteRespBlock").find(".switch").hasClass("switch-active"),
            DivCreate: $("#DivCreateRespBlock").find(".switch").hasClass("switch-active"),
            DivDelete: $("#DivDeleteRespBlock").find(".switch").hasClass("switch-active"),
            PosCreate: $("#PosCreateRespBlock").find(".switch").hasClass("switch-active"),
            PosDelete: $("#PosDeleteRespBlock").find(".switch").hasClass("switch-active"),
            PosUpdate: $("#PosUpdateRespBlock").find(".switch").hasClass("switch-active"),
            PosChangeDiv: $("#PosChangeDivRespBlock").find(".switch").hasClass("switch-active"),
            PosUnlock: $("#PosUnlockRespBlock").find(".switch").hasClass("switch-active"),
            EmpCreate: $("#EmpCreateRespBlock").find(".switch").hasClass("switch-active"),
            EmpDelete: $("#EmpDeleteRespBlock").find(".switch").hasClass("switch-active"),
            EmpUpdate: $("#EmpUpdateRespBlock").find(".switch").hasClass("switch-active"),
            EmpUnlock: $("#EmpUnlockRespBlock").find(".switch").hasClass("switch-active"),
            EmpChangeDiv: $("#EmpChangeDivRespBlock").find(".switch").hasClass("switch-active"),
            EmpPossManagement: $("#EmpPossManagementRespBlock").find(".switch").hasClass("switch-active"),
            EmpRespsManagement: $("#EmpRespsManagementRespBlock").find(".switch").hasClass("switch-active"),
            EmpContactCreate: $("#EmpContactCreateRespBlock").find(".switch").hasClass("switch-active"),
            EmpContactUpdate: $("#EmpContactUpdateRespBlock").find(".switch").hasClass("switch-active"),
            EmpContactDelete: $("#EmpContactDeleteRespBlock").find(".switch").hasClass("switch-active"),
            RespCreate: $("#RespCreateRespBlock").find(".switch").hasClass("switch-active"),
            RespUpdate: $("#RespUpdateRespBlock").find(".switch").hasClass("switch-active"),
            RespDelete: $("#RespDeleteRespBlock").find(".switch").hasClass("switch-active"),
            AccAddressCreate: $("#AccAddressCreateRespBlock").find(".switch").hasClass("switch-active"),
            AccAddressDelete: $("#AccAddressDeleteRespBlock").find(".switch").hasClass("switch-active"),
            AccAddressUpdate: $("#AccAddressUpdateRespBlock").find(".switch").hasClass("switch-active"),
            AccChangeType: $("#AccChangeTypeRespBlock").find(".switch").hasClass("switch-active"),
            AccUnlock: $("#AccUnlockRespBlock").find(".switch").hasClass("switch-active"),
            AccContactCreate: $("#AccContactCreateRespBlock").find(".switch").hasClass("switch-active"),
            AccContactDelete: $("#AccContactDeleteRespBlock").find(".switch").hasClass("switch-active"),
            AccContactUpdate: $("#AccContactUpdateRespBlock").find(".switch").hasClass("switch-active"),
            AccCreate: $("#AccCreateRespBlock").find(".switch").hasClass("switch-active"),
            AccDelete: $("#AccDeleteRespBlock").find(".switch").hasClass("switch-active"),
            AccInvoiceCreate: $("#AccInvoiceCreateRespBlock").find(".switch").hasClass("switch-active"),
            AccInvoiceDelete: $("#AccInvoiceDeleteRespBlock").find(".switch").hasClass("switch-active"),
            AccInvoiceUpdate: $("#AccInvoiceUpdateRespBlock").find(".switch").hasClass("switch-active"),
            AccTeamManagement: $("#AccTeamManagementRespBlock").find(".switch").hasClass("switch-active"),
            AccUpdate: $("#AccUpdateRespBlock").find(".switch").hasClass("switch-active"),
        }
    }

    /**
     * Метод разворачивает катгорию с настройками полномочий
     * @param {*} event 
     */
    ExpandCategory(event) {
        // Смена галочки
        $(event.currentTarget).find(".icon-chevron-thin-right").addClass("d-none");
        $(event.currentTarget).find(".icon-chevron-thin-left").removeClass("d-none");

        // Отображение блока с настрйоками
        let respSettingGroup = $(event.currentTarget).closest(".resp-setting-group").parent();
        let respSetting = $(respSettingGroup).find(".resp-setting");
        $(respSetting).removeClass("d-none");
    }

    /**
     * Метод сворачиваеет катгорию с настройками полномочий
     * @param {*} event 
     */
    CollapseCategory(event) {
        // Смена галочки
        $(event.currentTarget).find(".icon-chevron-thin-right").removeClass("d-none");
        $(event.currentTarget).find(".icon-chevron-thin-left").addClass("d-none");

        // Скрытие блока с настрйоками
        let respSettingGroup = $(event.currentTarget).closest(".resp-setting-group").parent();
        let respSetting = $(respSettingGroup).find(".resp-setting");
        $(respSetting).addClass("d-none");
    }
}

$("#responsibilityHeader")
    .off("click", "#changeRespNameBtn").on("click", "#changeRespNameBtn", event => {
        Swal.fire(MessageManager.Invoke("ChangingReponsibilityName", {
            currentReponsibilityName: $("#orgName").text()
        })).then((result) => {
            if (result["isConfirmed"]) {
                $("#orgName").text(result["value"]);
                $("#responsibilityForm #responsibilityActions").find("button").removeAttr("disabled");
            }
        })
    });

$("#responsibilityForm")
    .off("click", ".resp-setting .switch").on("click", ".resp-setting .switch", event => {
        // При простановке флага необходимо проверять наличие в группе отключенных элементов
        // Если они есть, то флаг всей группы также надо отключать
        // Таймаут, чтобы вначале проставился флаг для выбранной настройки, и только затем - для группы
        setTimeout(() => {
            let groupBlock = $(event.currentTarget).closest(".resp-setting").parent();
            let group = $(groupBlock).find(".resp-setting-group");
            let responsibility = new Responsibility();
            responsibility.HasDisabledSettings(group);
            responsibility.CheckForChanges();
        })
    })
    .off("click", ".resp-setting-group .switch").on("click", ".resp-setting-group .switch", event => {
        // Получение всех пунктов для данной группы настроек полномочия
        let connectedSettings = Array.from($(event.currentTarget).closest(".resp-setting-group").parent().find(".resp-setting"));

        // Если в данный момент(до отработки нажатия по switch) текущий элемент имеет класс switch-disable, значит, нажатие происходит по отключенному флагу
        // Т.е. все пункты для данной группы настроек полномочия необходимо сделать активными
        let isActive = $(event.currentTarget).hasClass("switch-disable");
        
        // Включение или отключение всех пунктов для данной группы настроек полномочия
        connectedSettings.map(connectedSetting => {
            if (isActive) {
                $(connectedSetting).find(".switch").removeClass("switch-disable").addClass("switch-active");
            }
            else {
                $(connectedSetting).find(".switch").removeClass("switch-active").addClass("switch-disable");
            }
        });

        // Проверка на необходимость фиксации изменений
        let responsibility = new Responsibility();
        responsibility.CheckForChanges();
    })
    .off("click", ".group-expand-icon").on("click", ".group-expand-icon", event => {
        let responsibility = new Responsibility();
        if ($(event.currentTarget).find(".icon-chevron-thin-left").hasClass("d-none")) {
            responsibility.ExpandCategory(event);
        }
        else responsibility.CollapseCategory(event);
    })
    .off("click", "#updateResp").on("click", "#updateResp", event => {
        event.preventDefault();
        if ($(event.currentTarget).attr("disabled") == undefined) {
            let responsibility = new Responsibility();
            responsibility.Update(event);
        }
    })
    .off("click", "#cancelUpdateResp").on("click", "#cancelUpdateResp", event => {
        event.preventDefault();
        location.reload();
    });

$(document).ready(event => {
    let responsibility = new Responsibility();
    responsibility.RememberFlags();
    responsibility.SetupGroupSwtichers();
})