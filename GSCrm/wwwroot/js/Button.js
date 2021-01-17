class Button {
    // Метод инициализирует чекбоксы при перезагрузке страницы
    InitializeCheckMarks() {
        // Инициализация на фильтре по конатктам клиента
        if ($("#accContactsFilter").length > 0) {
            this.InitMarksOn_AccContactsFilter();
        }
    }

    /**
     * Инициализация чекбоксов на фильтре по контактам клиента
     */
    InitMarksOn_AccContactsFilter() {
        let currentCheckValue = $("#SearchContactPrimary").val();
        if (currentCheckValue == "True") {
            $("#searchContactPrimarySign").addClass("oval-mark-check").removeClass("oval-mark").append("<div class='icon-checkmark'></div>");
        }
        else {
            $("#searchContactPrimarySign").removeClass("oval-mark-check").addClass("oval-mark").empty();
        }
    }

    /**
     * Нажатие по кнопке с галкой
     * @param {*} event 
     */
    CheckmarkCheck(event) {
        let target = $(event.currentTarget);
        if (target.hasClass("checkmark-checked"))
            target.removeClass("checkmark-checked");
        else target.addClass("checkmark-checked");
        $(target).stop().addClass('checkmark-click'), setTimeout(function() {
            $(target).removeClass('checkmark-click');
        }, 600);
    }

    /**
     * Нажатие по кнопке с галкой в кружке(Без возможности снять галку)
     * @param {*} checkMark элемент, для которого требуется добавить галку 
     */
    OvalCheckmarkReadonly(checkMark) {
        $(checkMark).removeClass("oval-mark").addClass("oval-mark-readonly");
        $(checkMark).append("<div class='icon-checkmark'></div>");
    }
    
    /**
     * Снятие галки с элемента
     * @param {*} checkMark элемент, для которого требуется снять галку 
     */
    HideOvalCheckmarkReadonly(checkMark) {
        $(checkMark).removeClass("oval-mark-readonly").addClass("oval-mark");
        $(checkMark).empty();
    }

    /**
     * Нажатие по кнопке с галкой в кружке(С возможностью снять галку)
     * @param {*} checkMark элемент, для которого требуется добавить галку 
     */
    OvalCheckmarkCheck(checkMark) {
        // Проставление галки
        if (!$(checkMark).hasClass("oval-mark-check")) {
            $(checkMark).removeClass("oval-mark").addClass("oval-mark-check");
            $(checkMark).append("<div class='icon-checkmark'></div>");
        }
        // Снятие галки
        else {
            $(checkMark).removeClass("oval-mark-check").addClass("oval-mark");
            $(checkMark).empty();
        }
    }
    
    /**
     * Снятие галки с элемента
     * @param {*} checkMark элемент, для которого требуется снять галку 
     */
    HideOvalCheckmarkCheck(checkMark) {
        $(checkMark).removeClass("oval-mark-check").addClass("oval-mark");
        $(checkMark).empty();
    }

    /**
     * Нажатиме на кнопку со скрытой галкой
     * @param {*} event 
     */
    HideCheckmarkCheck(event) {
        let target = $(event.currentTarget);

        target.closest("#employeePosisionModal").find(".readonly-checkmark").each((index, checkmark) => {
            $(checkmark).removeClass("readonly-checkmark");
            $(checkmark).addClass("hide-checkmark");
        })

        target.closest("#employeePosisionModal").find(".readonly-cross").each((index, checkmark) => {
            $(checkmark).removeClass("readonly-cross");
        })
        
        target.closest("tr").find(".cross").addClass("readonly-cross").removeClass("cross-crossed");
        target.removeClass("hide-checkmark");
        target.addClass("readonly-checkmark");
    }

    /**
     * Нажатие по кнопке с крестиком
     * @param {*} event 
     */
    CrossClick(event) {
        let target = $(event.currentTarget);
        let checkmark = target.closest("tr").find(".readonly-checkmark");
        if (checkmark.length == 0) {
            if (target.hasClass("cross-crossed"))
                target.removeClass("cross-crossed");
            else target.addClass("cross-crossed");
            $(target).stop().addClass('cross-click'), setTimeout(function() {
                $(target).removeClass('cross-click');
            }, 600);
        }
    }
}

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

// Всплывающие подсказки
$(document).off("click", ".popover-source").on("click", ".popover-source", event => {
    event.stopPropagation();
    event.preventDefault();
})

// Главный навбар
$(document).off("click", ".vertical-nav .nav-link").on("click", ".vertical-nav .nav-link", event => {
    let tab = new Tab();
    tab.ClearAll();
});

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