class Notification {
    //#region HasReedSign
    /** Метод помечает уведомление как прочитанное */
    MakeHasReed(event) {
        return new Promise((resolve, reject) => {
            // Изменения цвета уведомления
            let notificationItem = $(event.currentTarget).closest(".notification-item");
            $(notificationItem)
                .find(".alert")
                .removeClass("alert-dark")
                .addClass("alert-light")
                .addClass("alert-not-light");

            // Запрос на простановку признака
            let notificationItemId = $(notificationItem).find(".notification-item-id").val();
            let makeHasReedBaseUrl = $(event.currentTarget).closest("form").attr("action");
            let makeHasReedUrl = makeHasReedBaseUrl + Localization.GetUri("makeNotHasReed") + notificationItemId;
            let request = new AjaxRequests();
            request.CommonGetRequest(makeHasReedUrl);
        })
    }

    /** Метод помечает уведомление как не прочитанное */
    MakeHasNoReed(event) {
        return new Promise((resolve, reject) => {
            // Изменения цвета уведомления
            let notificationItem = $(event.currentTarget).closest(".notification-item");
            $(notificationItem)
                .find(".alert")
                .removeClass("alert-light")
                .removeClass("alert-not-light")
                .addClass("alert-dark");

            // Запрос на простановку признака
            let notificationItemId = $(notificationItem).find(".notification-item-id").val();
            let makeHasNoReedBaseUrl = $(event.currentTarget).closest("form").attr("action");
            let makeHasNoReedUrl = makeHasNoReedBaseUrl + Localization.GetUri("makeNotHasNoReed") + notificationItemId;
            let request = new AjaxRequests();
            request.CommonGetRequest(makeHasNoReedUrl);
        })
    }

    /** Метод помечает все уведомления как прочитанные */
    ReadAll() {
        return new Promise((resolve, reject) => {
            let readAllUrl = $("#readAllNots").closest("form").attr("action");
            let request = new AjaxRequests();
            request.CommonGetRequest(readAllUrl).then(() => location.reload());
        })
    }
    //#endregion

    //#region OrgInvite
    /**
     * Согласие на вступление в организацию
     * @param {*} event 
     */
    AcceptInvite(event) {
        return new Promise((resolve, reject) => {
            let acceptInviteUrl = $(event.currentTarget).closest("form").attr("action");
            let requests = new AjaxRequests();
            let hasErrors = false;
            requests.CommonGetRequest(acceptInviteUrl)
                .catch(response => {
                    hasErrors = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["AcceptInvite"]).then(() => location.reload())
                })
                .then(() => {
                    if (!hasErrors) {
                        Swal.fire(MessageManager.Invoke("InviteHasBeenAccepted")).then(() => location.reload());
                    }
                });
        })
    }

    /**
     * Отказ от вступления в организацию
     * @param {*} event 
     */
    RejectInvite(event) {
        return new Promise((resolve, reject) => {
            let rejectInviteUrl = $(event.currentTarget).closest("form").attr("action");
            let requests = new AjaxRequests();
            requests.CommonGetRequest(rejectInviteUrl).then(() => {
                Swal.fire(MessageManager.Invoke("InviteHasBeenRejected")).then(() => location.reload());
            });
        })
    }
    //#endregion
}

// Чекбоксы уведомлений
$("#notificationsList")
    // Прочесть все уведомления
    .off("click", "#readAllNots").on("click", "#readAllNots", event => {
        event.stopPropagation();
        event.preventDefault();
        let notification = new Notification();
        notification.ReadAll();
    })
    // Проставление чекбокса
    .off("click", ".cbx-not-onload").on("click", ".cbx-not-onload", event => {
        $(event.currentTarget).removeClass("cbx-not-onload");
        $(event.currentTarget).addClass("cbx-not");
        let notification = new Notification();
        notification.MakeHasNoReed(event);
    })
    // Проставление чекбокса
    .off("click", ".cbx-not-non-active").on("click", ".cbx-not-non-active", event => {
        $(event.currentTarget).removeClass("cbx-not-non-active");
        $(event.currentTarget).addClass("cbx-not");
        let notification = new Notification();
        notification.MakeHasNoReed(event);
    })
    // Снятие чекбокса
    .off("click", ".cbx-not").on("click", ".cbx-not", event => {
        $(event.currentTarget).removeClass("cbx-not");
        $(event.currentTarget).addClass("cbx-not-non-active");
        let notification = new Notification();
        notification.MakeHasReed(event);
    })
    // Принятие приглашения в организацию
    .off("click", ".accept-invite-btn").on("click", ".accept-invite-btn", event => {
        event.preventDefault();
        event.stopPropagation();
        let notification = new Notification();
        notification.AcceptInvite(event);
    })
    // Отказ от приглашения в организацию
    .off("click", ".reject-invite-btn").on("click", ".reject-invite-btn", event => {
        event.preventDefault();
        event.stopPropagation();
        let notification = new Notification();
        notification.RejectInvite(event);
    })