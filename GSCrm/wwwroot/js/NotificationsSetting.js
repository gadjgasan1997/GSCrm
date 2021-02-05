class NotificationsSetting {
    Commit() {
        return new Promise((resolve, reject) => {
            let notificationsSettingUrl = this.CommitGetUrl();
            let notificationsSettingData = this.CommitGetData();
            let request = new AjaxRequests();
            let hasErros = false;
            request.JsonPostRequest(notificationsSettingUrl, notificationsSettingData)
                .catch(response => {
                    hasErros = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["NotSettingCommit"]);
                })
                .then(() => {
                    if (!hasErros) {
                        Swal.fire(MessageManager.Invoke("NotSettingsCommitSuccess"));
                    }
                });
        })
    }

    CommitGetUrl() {
        let appendUrlPath = this.IsUserNotSettingsActive() ? LocalizationManager.GetUri("commitUserSettingsPath") : LocalizationManager.GetUri("commitOrgSettingsPath");
        return $("#notSettingsActions").attr("action") + appendUrlPath;
    }

    CommitGetData() {
        // Если выбраны настройки уведомлений для аккаунта пользователя
        if (this.IsUserNotSettingsActive()) {
            // Возврат результата
            return {
                UserNotificationsSettingViewModel: {
                    Id: $("#userNotSettingsList").find("#userNotSettingId").val(),
                    OrgInvoiceNot: $("#orgInvoiceNot").find(".switch").hasClass("switch-active"),
                    TOrgInvoiceNot: $("#orgInvoiceNot").find(".not-setting-target").val()
                }
            }
        }
        // Если выбраны настройки уведомлений для организаций, в которых состоит пользователь
        else {
            let notSettingGroups = [];

            // Заполнение списка с настройками уведомлений для организаций
            $(".not-setting-group").each((index, item) => {
                notSettingGroups.push({
                    Id: $(item).find(".not-setting-id").val(),
                    UserOrganizationId: $(item).find(".user-org-id").val(),
                    DivDeleteNot: this.GetNeedNotificationSign(item, "DivisionDeleteNot"),
                    TDivDeleteNot: this.GetNotificationTarget(item, "DivisionDeleteNot"),
                    PosDeleteNot: this.GetNeedNotificationSign(item, "PositionDeleteNot"),
                    TPosDeleteNot: this.GetNotificationTarget(item, "PositionDeleteNot"),
                    PosUpdateNot: this.GetNeedNotificationSign(item, "PositionUpdateNot"),
                    TPosUpdateNot: this.GetNotificationTarget(item, "PositionUpdateNot"),
                    EmpDelete: this.GetNeedNotificationSign(item, "EmployeeDeleteNot"),
                    TEmpDelete: this.GetNotificationTarget(item, "EmployeeDeleteNot"),
                    EmpUpdate: this.GetNeedNotificationSign(item, "EmployeeUpdateNot"),
                    TEmpUpdate: this.GetNotificationTarget(item, "EmployeeUpdateNot"),
                    AccDelete: this.GetNeedNotificationSign(item, "AccountDeleteNot"),
                    TAccDelete: this.GetNotificationTarget(item, "AccountDeleteNot"),
                    AccUpdate: this.GetNeedNotificationSign(item, "AccountUpdateNot"),
                    TAccUpdate: this.GetNotificationTarget(item, "AccountUpdateNot"),
                    AccTeamManagement: this.GetNeedNotificationSign(item, "AccountTeamManagementNot"),
                    TAccTeamManagement: this.GetNotificationTarget(item, "AccountTeamManagementNot"),
                });
            });

            // Возврат результата
            return {
                OrgNotificationsSettingViewModels: notSettingGroups
            }
        }
    }

    IsUserNotSettingsActive() {
        return $("#notsTabs").find("#userNotSettingsTab").hasClass("active");
    }

    /**
     * Метод возвращает признак, указывающий, надо ли отсылать уведомление этого типа
     * @param {*} item Элемент, представляющий настройки уведомлений для конкретной организации
     * @param {*} notType Тип уведомления
     */
    GetNeedNotificationSign(item, notType) {
        return $(this.GetNotificationRow(item, notType)).find(".switch").hasClass("switch-active");
    }

    /**
     * Метод возвращает способ получения уведомления заданного типа пользователем
     * @param {*} item Элемент, представляющий настройки уведомлений для конкретной организации
     * @param {*} notType Тип уведомления
     */
    GetNotificationTarget(item, notType) {
        return $(this.GetNotificationRow(item, notType)).find(".not-setting-target").val();
    }

    /**
     * Метод возвращает настройку уведомления
     * @param {*} item 
     * @param {*} notType 
     */
    GetNotificationRow(item, notType) {
        return $(item)[0].querySelectorAll('[data-nottype="' + notType + '"]');
    }

    /**
     * При развороте настроек
     * @param {*} event 
     */
    ArrowTtogglerClick(event) {
        let group = $(event.currentTarget).closest(".not-setting-group");
        let setting = $(group).find(".not-settings");
        if ($(setting).css("display") == "none") {
            $(setting).slideDown("fast");
            $(event.currentTarget)
                .find(".icon-chevron-thin-right")
                .removeClass("icon-chevron-thin-right")
                .addClass("icon-chevron-thin-left")
        }
        else {
            $(setting).slideUp("fast");
            $(event.currentTarget)
                .find(".icon-chevron-thin-left")
                .removeClass("icon-chevron-thin-left")
                .addClass("icon-chevron-thin-right")
        }
    }

    /**
     * При включении/выключении уведомления
     * @param {*} event 
     */
    SwitchClick(event) {
        let notSettingOptions = $(event.currentTarget).closest(".not-setting-options");
        let cogsEl = $(notSettingOptions).find(".cogs");

        // Скрытие кнокпи с настройками
        if ($(event.currentTarget).hasClass("switch-active")) {
            $(cogsEl).closest(".col").addClass("d-none");
        }

        // Отображение кнокпи с настройками
        else {
            $(cogsEl).closest(".col").removeClass("d-none");
        }
    }

    /**
     * При нажатии на кнопку настроек уведомления
     * @param {*} event 
     */
    IconCogsClick(event) {
        // Проставление выбранного значения
        $("#notSettingOptionsModal").find(".not-target").each((index, item) => {
            $(item).find("input").prop("checked", false);
        });
        let notSettingOptions = $(event.currentTarget).closest(".not-setting-options");
        let selectedTarget = $(notSettingOptions).find(".not-setting-target").val();
        let option = $("#notSettingOptionsModal").find("#" + selectedTarget + "Option");
        $(option).find("input").prop("checked", true);

        // Проставление местоположения
        let offsetLeft = $(event.currentTarget).offset().left;
        let optionsWidth = $("#notSettingOptionsModal").width();
        let bodyWidth = $("body").width();
        let left = offsetLeft;1
        if (bodyWidth < optionsWidth + offsetLeft) {
            left = offsetLeft - optionsWidth;
        }

        // Отображение
        $("#notSettingOptionsModal")
            .attr("data-notsettingid", $(event.currentTarget).closest(".not-setting").attr("id"))
            .css("top", $(event.currentTarget).offset().top)
            .css("left", left)
            .removeClass("d-none");
    }

    /**
     * При выборе типа рассылки уведомлений
     * @param {*} event 
     */
    NotTargetSelect(event) {
        let notSetting = $("#" + $("#notSettingOptionsModal").attr("data-notsettingid"));
        let selectedValue = $(event.currentTarget).find("input").val();
        $(notSetting).find(".not-setting-target").val(selectedValue);
    }

    /** Метод устанавливает накстройки по умолчанию для уведомлений */
    SetNotSettingsToDefault() {
        let messageName = this.IsUserNotSettingsActive() ? "ResetUserNotSettingsConfirmation" : "ResetOrgNotSettingsConfirmation";
        Swal.fire(MessageManager.Invoke(messageName)).then(dialogResult => {
            if (dialogResult.value) {
                return new Promise((resolve, reject) => {
                    let setDefSettingsUrl = this.SetNotSettingsToDefaultGetUrl();
                    let request = new AjaxRequests();
                    let hasErros = false;
                    request.CommonGetRequest(setDefSettingsUrl)
                        .catch(response => {
                            hasErros = true;
                            Utils.DefaultErrorHandling(response["responseJSON"]);
                        })
                        .then(() => {
                            if (!hasErros) {
                                Swal.fire(MessageManager.Invoke("NotSettingsCommitSuccess")).then(() => location.reload());
                            }
                        });
                })
            }
        })
    }

    SetNotSettingsToDefaultGetUrl() {
        let appendUrlPath = this.IsUserNotSettingsActive() ? LocalizationManager.GetUri("setUserNotSettingsToDefaultPath") : LocalizationManager.GetUri("setOrgNotSettingsToDefaultPath");
        return $("#setNotSettingsToDefault").closest("form").attr("action") + appendUrlPath;
    }
}

// Общая форма с уведомлениями
$("#notSettingsForm")
    .off("click", "#setNotSettingsToDefault").on("click", "#setNotSettingsToDefault", event => {
        event.preventDefault();
        event.stopPropagation();
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.SetNotSettingsToDefault();
    })
    .off("click", "#notsTabs .nav-item").on("click", "#notsTabs .nav-item", event => {
        let navTab = new NavTab();
        navTab.Remember(event, "currentNotsTab");
    });

// Список настроек уведомлений для организаций
$("#orgNotSettingsList")
    .off("click", ".arrow-toggler").on("click", ".arrow-toggler", event => {
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.ArrowTtogglerClick(event);
    })
    .off("click", ".switch").on("click", ".switch", event => {
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.SwitchClick(event);
    })
    .off("click", ".icon-cogs").on("click", ".icon-cogs", event => {
        event.stopPropagation();
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.IconCogsClick(event);
    });

// Список настроек уведомлений для пользователя
$("#userNotSettingsList")
    .off("click", ".switch").on("click", ".switch", event => {
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.SwitchClick(event);
    })
    .off("click", ".icon-cogs").on("click", ".icon-cogs", event => {
        event.stopPropagation();
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.IconCogsClick(event);
    });

// Действия, доступные с настройками уведомлений
$("#notSettingsActions")
    .off("click", ".update-not-setting").on("click", ".update-not-setting", event => {
        event.preventDefault();
        event.stopPropagation();
        let notificationsSetting = new NotificationsSetting();
        notificationsSetting.Commit();
    })
    .off("click", ".cancel-update-not-setting").on("click", ".cancel-update-not-setting", event => {
        event.preventDefault();
        event.stopPropagation();
        location.reload();
    });

// Настройки уведомления
$("#notSettingOptionsModal").off("click", ".not-target").on("click", ".not-target", event => {
    let notificationsSetting = new NotificationsSetting();
    notificationsSetting.NotTargetSelect(event);
})
    
$(document)
    .off("click", "html").on("click", "html", event => {
        if ($("#notSettingOptionsModal").length > 0) {
            $("#notSettingOptionsModal").addClass("d-none");
        }
    })
    .off("click", "#notSettingOptionsModal").on("click", "#notSettingOptionsModal", event => {
        event.stopPropagation();
    });