class ConfigsData {
    static localizationData = {};
    static errorsData = {};
    static autocomplitesData = {};
    static settingsMenuData = {};

    /**
     * Метод записывает данные конфига в объект
     * @param {*} configName 
     * @param {*} data 
     */
    static SetData(configName, data) {
        switch (configName) {
            case "Localization":
                ConfigsData.localizationData = data;
                break;
            case "Errors":
                ConfigsData.errorsData = data;
                break;
            case "Autocomplites":
                ConfigsData.autocomplitesData = data;
                break;
            case "SettingsMenu":
                ConfigsData.settingsMenuData = data;
                break;
        }
    }
}