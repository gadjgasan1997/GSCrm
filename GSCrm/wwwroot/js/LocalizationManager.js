class LocalizationManager {
    static GetString(key, lang) {
        let item = ConfigsData.localizationData["localization"][key];
        if (item == undefined)
            return "";
        if (lang == undefined)
            return item["ru-RU"];
        else return item[lang];
    }

    static GetUri(key) {
        return ConfigsData.localizationData["uri"][key];
    }
}