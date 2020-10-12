class Tab {
    Remember(event, tabName) {
        localStorage.setItem(tabName, $(event.currentTarget).find(".nav-link").attr("href"));
    }

    ClearAll() {
        this.ClearEmpTabs();
        this.ClearPosTabs();
        this.ClearAccsTab();
        this.ClearOrgTab();
        this.ClearQuotesTab();
    }

    ClearEmpTabs() {
        localStorage.removeItem("currentEmpTab");
        localStorage.removeItem("currentEmpConnectedTab");
    }

    ClearPosTabs() {
        localStorage.removeItem("currentPosTab");
    }

    ClearAccTab() {
        localStorage.removeItem("currentAccTab");
    }

    ClearAccsTab() {
        localStorage.removeItem("currentAccsTab");
        localStorage.removeItem("currentAccsConnectedTab");
    }

    ClearOrgTab() {
        localStorage.removeItem("currentOrgTab");
        localStorage.removeItem("currentOrgConnectedTab");
    }

    ClearQuotesTab() {
        localStorage.removeItem("currentQuotesTab");
        localStorage.removeItem("currentQuotesConnectedTab");
    }
}

class NavTab extends Tab {
    Restore() {
        // Вкладки на карточке организации
        if ($("#organizationForm").length > 0) {
            this.GenericRestore("currentOrgTab", "#divisions");
        }

        // Вкладки на карточке сотрудника
        if ($("#employeeForm").length > 0) {
            this.GenericRestore("currentEmpTab", "#positions");
        }

        // Вкладки на списке клиентов
        if ($("#accountsForm").length > 0) {
            this.GenericRestore("currentAccsTab", "#currentAccounts");
        }

        // Вкладки на списке сделок
        if ($("#quotesForm").length > 0) {
            this.GenericRestore("currentQuotesTab", "#currentQuotes");
        }

        // Вкладки на списке сделок
        if ($("#positionForm").length > 0) {
            this.GenericRestore("currentPosTab", "#employees");
        }
    }
    
    /**
     * Восстанавливает текущую вкладку из localStorage. Если текущая вкладка не найден, запоминает дефолтовую
     * @param {*} currentTabName Название вкладки, которую необходимо получить из localStorage
     * @param {*} currentTabValue Значение текущей вкладки, которую надо установить, если в localStorage вкладка не найдена
     */
    GenericRestore(currentTabName, currentTabValue) {
        let currentTab = localStorage.getItem(currentTabName);
        if (currentTab != null) {
            $('[href="' + currentTab + '"]').tab('show');
        }

        else {
            localStorage.setItem(currentTabName, currentTabValue);
            $('[href="' + currentTabValue + '"]').tab('show');   
        }
    }

    Click(event) {
        $(".radio-tabs .form-check").removeClass("active");
        $(".radio-tabs .form-check").find("input").removeAttr("checked");
        $(event.currentTarget).addClass("active").find("input").attr("checked","checked");
        $(".tabs-content .tabs-content-item").hide();
        var activeTab = $(event.currentTarget).find("input").val();
        $('#' + activeTab).fadeIn();
    }
}

class NavConnectedTab extends Tab {
    Restore() {
        // Вкладки на карточке организации
        if ($("#organizationForm").length > 0) {
            this.GenericRestore("#organizationForm", "currentOrgConnectedTab", "#divisions");
        }

        // Вкладки на карточке сотрудника
        if ($("#employeeForm").length > 0) {
            this.GenericRestore("#employeeForm", "currentEmpConnectedTab", "#positions");
        }

        // Вкладки на списке клиентов
        if ($("#accountsForm").length > 0) {
            this.GenericRestore("#accountsForm", "currentAccsConnectedTab", "#currentAccounts");
        }

        // Вкладки на списке сделок
        if ($("#quotesForm").length > 0) {
            this.GenericRestore("#quotesForm", "currentQuotesConnectedTab");
        }
    }

    /**
     * Метод восстанавливает связанные вкладки
     * @param {*} formSelector Форма, в которой находятся связанные вкладки
     * @param {*} connectedTabName Название связанной вкладки
     * @param {*} connectedTabValue Значение связанной вкладки
     */
    GenericRestore(formSelector, connectedTabName, connectedTabValue) {
        let currentOrgTabRef = localStorage.getItem(connectedTabName);
        if (currentOrgTabRef == null) {
            currentOrgTabRef = connectedTabValue;
            localStorage.setItem(connectedTabName, connectedTabValue);
        }
        let connectedTabs = $(formSelector).find(".nav-connected-tabs");
        this.HideShowTabs(connectedTabs, currentOrgTabRef);
    }

    SelectNavTab(event) {
        let selectedNavTabsId = $(event.currentTarget).closest(".nav-tabs").attr("id");
        let selectedNavTabRef = $(event.currentTarget).find("a").attr("href");
        let tabNodes = document.querySelectorAll("[nav-tabs='" + selectedNavTabsId + "']");
        Array.from(tabNodes).map(connectedTabs => this.HideShowTabs(connectedTabs, selectedNavTabRef));
    }

    HideShowTabs(connectedTabs, selectedNavTabRef) {
        $(connectedTabs).find(".nav-connected-tab").each((tabIndex, connectedTab) => {
            if ($(connectedTab).attr("nav-tab") == selectedNavTabRef) {
                $(connectedTab).removeClass("d-none");
                $(connectedTab).removeClass("fade");
                $(connectedTab).addClass("show");
            }
            else {
                $(connectedTab).addClass("d-none");
                $(connectedTab).addClass("fade");
                $(connectedTab).removeClass("show");
            }
        });
    }
}

class VertNavTab {
    Remember(event, tabName) {
        localStorage.setItem(tabName, $(event.currentTarget).attr("id"));
    }

    Restore() {
        if ($("#accountAddInfoForm").length > 0) {
            this.RestoreAccountAddInfoForm();
        }
    }

    // Восстановление вкладок на карточке клиента
    RestoreAccountAddInfoForm() {
        let currentAccTab = localStorage.getItem("currentAccTab");

        // Если вкладка не была запомнена, выбирается первая
        if (currentAccTab == null || currentAccTab == undefined || currentAccTab == "") {
            let vertNavTabs = $("#accountAddInfoForm").find(".vert-nav-tabs .vert-nav-tab");
            Array.from(vertNavTabs).map(vertNavTab => $(vertNavTab).removeClass("active"));
            let firstNavTab = $(vertNavTabs)[0];
            $(firstNavTab).addClass("active");
        }

        // Иначе восстановление выбранной вкладки
        else {
            let vertNavTabs = $("#" + currentAccTab).closest(".vert-nav-tabs").find(".vert-nav-tab");
            Array.from(vertNavTabs).map(vertNavTab => $(vertNavTab).removeClass("active"));
            $("#" + currentAccTab).addClass("active");
        }
    }
}

class VertNavConnectedTab {
    Restore() {
        // Вкладки на карточке клиента
        if ($("#accountAddInfoForm").length > 0) {
            this.RestoreAccountAddInfoForm();
        }
    }

    // Восстановление вкладок на карточке клиента
    RestoreAccountAddInfoForm() {
        let currentAccTab = localStorage.getItem("currentAccTab");
        if (currentAccTab != null) {
            let vertNavTabsId = $("#" + currentAccTab).closest(".vert-nav-tabs").attr("id");

            // Снятие класса active со всех соединенных вкладок
            let vertNavConnectedTabsBlock = document.querySelectorAll("[vert-nav-tabs='" + vertNavTabsId + "']")[0];
            if (vertNavConnectedTabsBlock != undefined) {
                let vertNavConnectedTabs = $(vertNavConnectedTabsBlock).find(".vert-nav-connected-tab");
                Array.from(vertNavConnectedTabs).map(vertNavConnectedTab => {
                    $(vertNavConnectedTab).removeClass("active");
                    $(vertNavConnectedTab).css("display", "none");
                });

                // Проставление класса active для выбранной вкладки
                let selectedVertNavTab = document.querySelectorAll("[vert-nav-tab='" + currentAccTab + "']")[0];
                if (selectedVertNavTab != undefined) {
                    let selectedTabId = $(selectedVertNavTab).attr("id");
                    $("#" + selectedTabId).addClass("active");
                    $(selectedVertNavTab).css("display", "list-item");
                }
            }
        }
    }
}