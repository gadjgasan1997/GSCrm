// Инициализация автокомплита
$(document).ready(() => Initializer.Execute())

// Dropdowns
$(document).click(function () {
    $('.dropdown-el').removeClass('expanded');
});

// Tabs
$(document).off("click", ".radio-tabs .form-check").on("click", ".radio-tabs .form-check", event => {
    let navTab = new NavTab();
    navTab.Click(event);
})

$(document).off("click", ".nav-tabs .nav-item").on("click", ".nav-tabs .nav-item", event => {
    let navConnectedTab = new NavConnectedTab();
    navConnectedTab.SelectNavTab(event);
})

// Checkmark
$(document).off("click", ".checkmark").on("click", ".checkmark", event => {
    $(event.currentTarget).trigger("checkmark-check", [{
        Event: event
    }])
})

$(document).off("click", ".hide-checkmark").on("click", ".hide-checkmark", event => {
    $(event.currentTarget).trigger("hide-checkmark-click", [{
        Event: event
    }])
})

// Cross
$(document).off("click", ".cross").on("click", ".cross", event => {
    $(event.currentTarget).trigger("cross-click", [{
        Event: event
    }])
})

// Navnext
$(document).off("click", ".nav-next .nav-url").on("click", ".nav-next .nav-url", event => {
    event.preventDefault();
    $(event.currentTarget).trigger("nav-next-click", [{
        Event: event
    }]);
    let href = $(event.currentTarget).attr("href");
    if (href != undefined) {
        window.location.replace($(event.currentTarget).attr("href"));
    }
})

// Navprevious
$(document).off("click", ".nav-previous .nav-url").on("click", ".nav-previous .nav-url", event => {
    event.preventDefault();
    $(event.currentTarget).trigger("nav-previous-click", [{
        Event: event
    }])
    let href = $(event.currentTarget).attr("href");
    if (href != undefined) {
        window.location.replace($(event.currentTarget).attr("href"));
    }
})

// Radio-tabs
$(document)
    .off("click", ".form-check").on("click", ".form-check", event => {
        $(event.currentTarget).closest(".radio-tabs").find(".form-check").each((index, radio) => {
            $(radio).removeClass("active");
            $(radio).find(".form-check-input").removeAttr("checked");
        });
        $(event.currentTarget).addClass("active");
        $(event.currentTarget).find(".form-check-input").prop("checked", "checked");
    })
    .off("click", ".form-check-wrap").on("click", ".form-check-wrap", event => {
        let disabledAttr = $(event.currentTarget).find(".form-check-input").attr("disabled");

        // Если была нажата таже радио кнопка или радио кнопка ридонли
        if ($(event.currentTarget).closest(".form-check").hasClass("active") || disabledAttr != undefined || disabledAttr == true) {
            event.preventDefault();
            event.stopPropagation();
        }
    });

// Vertical nav tabs
$(document).off("click", ".naccs .menu div").on("click", ".naccs .menu div", event => {
    var numberIndex = $(event.currentTarget).index();

    // Остановка текущей анимации
    $(".naccs ul li").stop(true, true).stop();

    // Скрытие открытых вкладок и отбражение выбранной
    if (!$(event.currentTarget).hasClass("active")) {
        $(".naccs .menu div").removeClass("active");
        $(".naccs ul li").fadeOut(500);

        $(event.currentTarget).addClass("active");
        $(".naccs ul").find("li:eq(" + numberIndex + ")").delay(500).fadeIn(500);
    }
});

// Switch
$(document).off("click", ".switch").on("click", ".switch", event => {
    if ($(event.currentTarget).hasClass("switch-active")) {
        $(event.currentTarget).removeClass("switch-active");
        $(event.currentTarget).addClass("switch-disable");
    }
    else {
        $(event.currentTarget).addClass("switch-active");
        $(event.currentTarget).removeClass("switch-disable");
    }
})

// Главный навбар
$(document).off("click", ".vertical-nav .nav-link").on("click", ".vertical-nav .nav-link", event => {
    let tab = new Tab();
    tab.ClearAll();
});

// Переход в профиль пользователя
$(document)
    .off("click", ".navbar .navbar-avatar").on("click", ".navbar .navbar-avatar", event => {
        let profileUrl = $(event.currentTarget).find("a").attr("href");
        location.replace(location.origin + profileUrl);
    })
    .off("click", ".navbar .navbar-avatar a").on("click", ".navbar .navbar-avatar a", event => {
        event.stopPropagation();
    })

// Всплывающие подсказки
$(document).off("click", ".popover-source").on("click", ".popover-source", event => {
    event.stopPropagation();
    event.preventDefault();
})

// Скрытие всех всплывающих окон
$(document).off("click", "body").on("click", "body", event => {
    $('.popover').popover("hide");
    $("#orgSettingsMenu").addClass("d-none");
})