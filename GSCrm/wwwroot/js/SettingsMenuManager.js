class SettingsMenuManager {
    static GetData(settingsMenuId) {
        if (Utils.IsNullOrEmpty(settingsMenuId))
            return null;
        let settingsMenuList = ConfigsData.settingsMenuData["nemuSettings"];
        if (Utils.IsNullOrEmpty(settingsMenuList))
            return null;
        return settingsMenuList[settingsMenuId];
    }
    
    /**
     * Открывает меню с настройками
     * @param {*} event 
     */
    Initialize(event) {
        let settingsMenuId = $(event.currentTarget).attr("data-settings-menu-id");
        let settingsMenuEl = $(document).find("#" + settingsMenuId);

        // Проставление размеров и местоположения
        let top = $(event.currentTarget).offset().top;
        let documentWidth = $("body")[0].offsetWidth;
        if (documentWidth < 576) {
            $(settingsMenuEl).css("top", top)
        }
        else {
            let menuPXWidth = $(settingsMenuEl).css("width");
            let menuWidth = Number.parseInt(menuPXWidth.split("px")[0]);
            let left = $(event.currentTarget).offset().left;
            if (documentWidth < left + menuWidth)
                left = left - menuWidth;

            $(settingsMenuEl).css({
                "top": top,
                "left": left,
                "width": menuWidth
            })
        }
            
        // Проставление id цели, с которой было открыто меню
        // Получение конфига с настройками для меню
        let settingsMenuConfig = SettingsMenuManager.GetData(settingsMenuId);
        if (!Utils.IsNullOrEmpty(settingsMenuConfig)) {

            // Получение названия класса, выступающего обработчиком при открытии меню
            let handlerName = settingsMenuConfig["settingsMenuHandler"];
            if (!Utils.IsNullOrEmpty(handlerName)) {

                // Вызов обработчика
                let handlerInstance = this.CreateSettingsMenuInstanse(handlerName);
                handlerInstance.OpenHandle(event, settingsMenuEl);
            }
        }
        
        $(settingsMenuEl).removeClass("d-none");
    }

    CreateSettingsMenuInstanse(settingsMenuName) {
        return new Map([
            ['ProdCatSettingsMenuHandler', new ProdCatSettingsMenuHandler()]
        ]).get(settingsMenuName)
    }
}

$(document).off("click", ".settings-menu-btn").on("click", ".settings-menu-btn", event => {
    event.stopPropagation();
    let settingsMenuManager= new SettingsMenuManager();
    settingsMenuManager.Initialize(event);
})

$(document).off("click", ".settings-menu").on("click", ".settings-menu", event => {
    event.stopPropagation();
})