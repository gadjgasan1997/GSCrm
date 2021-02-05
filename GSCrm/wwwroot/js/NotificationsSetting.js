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
        // ���� ������� ��������� ����������� ��� �������� ������������
        if (this.IsUserNotSettingsActive()) {
            // ������� ����������
            return {
                UserNotificationsSettingViewModel: {
                    Id: $("#userNotSettingsList").find("#userNotSettingId").val(),
                    OrgInvoiceNot: $("#orgInvoiceNot").find(".switch").hasClass("switch-active"),
                    TOrgInvoiceNot: $("#orgInvoiceNot").find(".not-setting-target").val()
                }
            }
        }
        // ���� ������� ��������� ����������� ��� �����������, � ������� ������� ������������
        else {
            let notSettingGroups = [];

            // ���������� ������ � ����������� ����������� ��� �����������
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

            // ������� ����������
            return {
                OrgNotificationsSettingViewModels: notSettingGroups
            }
        }
    }

    IsUserNotSettingsActive() {
        return $("#notsTabs").find("#userNotSettingsTab").hasClass("active");
    }

    /**
     * ����� ���������� �������, �����������, ���� �� �������� ����������� ����� ����
     * @param {*} item �������, �������������� ��������� ����������� ��� ���������� �����������
     * @param {*} notType ��� �����������
     */
    GetNeedNotificationSign(item, notType) {
        return $(this.GetNotificationRow(item, notType)).find(".switch").hasClass("switch-active");
    }

    /**
     * ����� ���������� ������ ��������� ����������� ��������� ���� �������������
     * @param {*} item �������, �������������� ��������� ����������� ��� ���������� �����������
     * @param {*} notType ��� �����������
     */
    GetNotificationTarget(item, notType) {
        return $(this.GetNotificationRow(item, notType)).find(".not-setting-target").val();
    }

    /**
     * ����� ���������� ��������� �����������
     * @param {*} item 
     * @param {*} notType 
     */
    GetNotificationRow(item, notType) {
        return $(item)[0].querySelectorAll('[data-nottype="' + notType + '"]');
    }

    /**
     * ��� ��������� ��������
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
     * ��� ���������/���������� �����������
     * @param {*} event 
     */
    SwitchClick(event) {
        let notSettingOptions = $(event.currentTarget).closest(".not-setting-options");
        let cogsEl = $(notSettingOptions).find(".cogs");

        // ������� ������ � �����������
        if ($(event.currentTarget).hasClass("switch-active")) {
            $(cogsEl).closest(".col").addClass("d-none");
        }

        // ����������� ������ � �����������
        else {
            $(cogsEl).closest(".col").removeClass("d-none");
        }
    }

    /**
     * ��� ������� �� ������ �������� �����������
     * @param {*} event 
     */
    IconCogsClick(event) {
        // ������������ ���������� ��������
        $("#notSettingOptionsModal").find(".not-target").each((index, item) => {
            $(item).find("input").prop("checked", false);
        });
        let notSettingOptions = $(event.currentTarget).closest(".not-setting-options");
        let selectedTarget = $(notSettingOptions).find(".not-setting-target").val();
        let option = $("#notSettingOptionsModal").find("#" + selectedTarget + "Option");
        $(option).find("input").prop("checked", true);

        // ������������ ��������������
        let offsetLeft = $(event.currentTarget).offset().left;
        let optionsWidth = $("#notSettingOptionsModal").width();
        let bodyWidth = $("body").width();
        let left = offsetLeft;1
        if (bodyWidth < optionsWidth + offsetLeft) {
            left = offsetLeft - optionsWidth;
        }

        // �����������
        $("#notSettingOptionsModal")
            .attr("data-notsettingid", $(event.currentTarget).closest(".not-setting").attr("id"))
            .css("top", $(event.currentTarget).offset().top)
            .css("left", left)
            .removeClass("d-none");
    }

    /**
     * ��� ������ ���� �������� �����������
     * @param {*} event 
     */
    NotTargetSelect(event) {
        let notSetting = $("#" + $("#notSettingOptionsModal").attr("data-notsettingid"));
        let selectedValue = $(event.currentTarget).find("input").val();
        $(notSetting).find(".not-setting-target").val(selectedValue);
    }

    /** ����� ������������� ���������� �� ��������� ��� ����������� */
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

// ����� ����� � �������������
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

// ������ �������� ����������� ��� �����������
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

// ������ �������� ����������� ��� ������������
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

// ��������, ��������� � ����������� �����������
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

// ��������� �����������
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