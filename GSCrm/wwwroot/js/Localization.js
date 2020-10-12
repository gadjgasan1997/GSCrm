class Localization {
    static localization = {};

    static SetData(data) {
        Localization.localization = data;
    }

    static GetString(key, lang) {
        let item = Localization.localization["localization"][key];
        if (item == undefined)
            return "";
        if (lang == undefined)
            return item["ru-RU"];
        else return item[lang];
    }

    static GetUri(key) {
        return Localization.localization["uri"][key];
    }
}