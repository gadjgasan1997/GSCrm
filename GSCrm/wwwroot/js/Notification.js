class Notification {
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
}

// Чекбоксы уведомлений
$("#notificationsList")
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