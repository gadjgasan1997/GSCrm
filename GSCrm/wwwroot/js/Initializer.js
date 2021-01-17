class Initializer {
    /** Метод инициализирует данные при перезагрузке страницы */
    static Execute() {
        Initializer.InitializeConfigs().then(() => {
            Initializer.InitalizeAppData().then(() => {
                Initializer.RestoreTabs();
                Initializer.RestoreDropdowns();
                Initializer.RestoreCheckMarks();
                Initializer.AttachAutocomplite();
                Initializer.AttachMasks();
                Initializer.InitializeScrools();
                Initializer.InitializeToolTips();
                Initializer.InitializeNotsCounter();
                $('[data-toggle="popover"]').popover();
            });
        });
    }

    // Заполнение объекта с локализацией
    static InitializeConfigs() {
        return new Promise((resolve, reject) => {
            Promise.all([
                $.getJSON(location.origin + "/js/FrontResource.json", data => {
                    Localization.SetData(data);
                }),
        
                $.getJSON(location.origin + "/js/ErrorsData.json", data => {
                    ErrorsManager.SetData(data);
                }),
        
                $.getJSON(location.origin + "/js/AutocomplitesData.json", data => {
                    BaseAutocomplete.SetData(data);
                })
            ]).then(() => resolve())
        })
    }

    /** Метод запрашивает с бека данные, необходимые для работы приложения
     *  Выполняется при каждой перезагрузке приложения
     */
    static InitalizeAppData() {
        return new Promise((resolve, reject) => {
            let initalizeAppDataUrl = Localization.GetUri("initalizeAppDataUrl");
            let requests = new AjaxRequests();
            requests.CommonGetRequest(initalizeAppDataUrl).then(appData => {
                localStorage.setItem("GSCrmAppData", JSON.stringify(appData));
                resolve();
            });
        })
    }

    // Восстановление вкладок
    static RestoreTabs() {
        let navTab = new NavTab();
        navTab.Restore();
        let vertNavTab = new VertNavTab();
        vertNavTab.Restore();
        let navConnectedTab = new NavConnectedTab();
        navConnectedTab.Restore();
        let vertNavConnectedTab = new VertNavConnectedTab();
        vertNavConnectedTab.Restore();
    }

    // Восстановление выпадающих списков
    static RestoreDropdowns() {
        let dropdown = new Dropdowns();
        dropdown.Initialize();
    }

    // Восстановление чекбоксов
    static RestoreCheckMarks() {
        let button = new Button();
        button.InitializeCheckMarks();
    }

    // Инициализация автокомплитов
    static AttachAutocomplite() {
        document.querySelectorAll(".autocomplete").forEach(control => {
            BaseAutocomplete.Initialize(control);
        });
    }

    // Инициализация масок
    static AttachMasks() {
        Mask.Initialize();
    }

    // Инициализация кастомных скроллбаров
    static InitializeScrools() {
        // Иерархия должностей
        $("#positionsHierarchy").mCustomScrollbar({
            axis:"y",
            theme:"dark"
        });

        // Команда по клиенту
        $("#selectedEmployeesList").mCustomScrollbar({
            axis:"x",
            theme:"dark",
            updateOnContentResize: false
        });

        // Список возможных адресов для выбора нового юридического
        $("#changeLEAddrModal #accAddressNotLegalList").mCustomScrollbar({
            axis:"y",
            theme:"dark"
        });

        // Список организаций, в которых состоит пользователь
        $("#accountModal #userOrgsChoiseList").mCustomScrollbar({
            axis:"y",
            theme:"dark"
        });
    }

    // Метод инициализрует всплывающие подсказки
    static InitializeToolTips() {
        let tooltip = new Tooltip();
        tooltip.Initialize();
    }

    // Обновление кастомных скроллбаров
    static ReInitScrools() {
        // Команда по клиенту
        $("#selectedEmployeesList").mCustomScrollbar("destroy");
        $("#selectedEmployeesList").mCustomScrollbar({
            axis:"x",
            theme:"dark"
        });
    }

    /** Метод инициализрует счетчик с количеством уведомлений */
    static InitializeNotsCounter() {
        let appData = JSON.parse(localStorage.getItem("GSCrmAppData"));
        let notsCount = appData["notsCount"];
        // Если есть сообщшения, необходимо отобразить счетчик и поменять их количество
        if (notsCount > 0) {
            $(".nav-nots-link")
                .removeClass("nav-nots-link")
                .addClass("nav-nots-link-active");
            $(".nots-link")
                .removeClass("nots-link")
                .addClass("nots-link-active");
            $(".nav-nots-link-active").attr("data-content", "У вас " + notsCount + " непрочитанных сообщений");
            $(".nots-link-active").attr("data-content", "У вас " + notsCount + " непрочитанных сообщений");
        }
        // Иначе надо скрыть счетчик
        else {
            $(".nav-nots-link")
                .removeClass("nav-nots-link-active")
                .addClass("nav-nots-link");
            $(".nots-link")
                .removeClass("nots-link-active")
                .addClass("nots-link");
        }
    }
}