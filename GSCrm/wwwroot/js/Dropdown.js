class Dropdowns {
    Click(event) {
        event.preventDefault();
        event.stopPropagation();
        $(event.currentTarget).toggleClass('expanded');
        $('#' + $(event.target).attr('for')).prop('checked', true);
    }

    LabelClick(event) {
        let inputId = "#" + $(event.currentTarget).attr("for");
        let value = $(inputId).attr("value");
        $(event.currentTarget).closest(".dropdown-area").find(".current-dropdown-value").val(value);
    }

    Initialize() {
        // Выпадающий список на фильтре по контактам сотрудника
        if ($("#contactsFilter").length > 0) {
            this.SetDropdownValue("SearchContactType", $("#SearchContactType").val());
        }
        
        // Выпадающий список на фильтре по контактам клиена
        if ($("#accContactsFilter").length > 0) {
            this.SetDropdownValue("SearchContactType", $("#SearchContactType").val());
        }
        
        // Выпадающий список на фильтре по адресам клиена
        if ($("#accAddressesFilter").length > 0) {
            this.SetDropdownValue("SearchAddressType", $("#SearchAddressType").val());
        }
        
        // Выпадающий список в модальном окне изменения юридического адреса клиена
        if ($("#changeLEAddrModal").length > 0) {
            this.SetDropdownValue("changeAccAddressType", "None");
        }
        
        // Выпадающий список на фильтре по сделкам текущего пользователя
        if ($("#currentAccountsFilter").length > 0) {
            this.SetDropdownValue("CurrentAccountsSearchType", $("#CurrentAccountsSearchType").val());
        }
        
        // Выпадающий список на фильтре по всем сделкам
        if ($("#allAccountsFilter").length > 0) {
            this.SetDropdownValue("AllAccountsSearchType", $("#AllAccountsSearchType").val());
        }
    }

    /**
     * Метод устанвливает новое значение для выпадающего списка по id элемента
     * @param {*} dropdownId Id элемента
     * @param {*} dropdownNewValue Новое значение
     */
    SetDropdownValue(dropdownId, dropdownNewValue) {
        let dropdownElement = $("#" + dropdownId).closest(".dropdown-area").find(".dropdown-el");
        $(dropdownElement).find("input").map((index, item) => {
            if ($(item).attr("value") == dropdownNewValue) {
                $(item).prop("checked", true);
            }
            else $(item).removeAttr("checked");
        })
    }
}

$('.dropdown-el').click(function(event) {
    let dropdown = new Dropdowns();
    dropdown.Click(event);
});

$('.dropdown-el').off("click", "label").on("click", "label", event => {
    let dropdown = new Dropdowns();
    dropdown.LabelClick(event);
})