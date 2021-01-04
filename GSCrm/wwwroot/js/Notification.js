class Notification {

}

// Чекбоксы уведомлений
$(document)
    .off("click", ".cbx-not-onload").on("click", ".cbx-not-onload", event => {
        $(event.currentTarget).removeClass("cbx-not-onload");
        $(event.currentTarget).addClass("cbx-not");
    })
    .off("click", ".cbx-not-non-active").on("click", ".cbx-not-non-active", event => {
        $(event.currentTarget).removeClass("cbx-not-non-active");
        $(event.currentTarget).addClass("cbx-not");
    })
    .off("click", ".cbx-not").on("click", ".cbx-not", event => {
        $(event.currentTarget).removeClass("cbx-not");
        $(event.currentTarget).addClass("cbx-not-non-active");
    })