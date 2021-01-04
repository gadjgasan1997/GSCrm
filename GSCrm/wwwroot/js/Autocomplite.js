class BaseAutocomplete {
    static autocomplitesData = {};
    static Initialize(control) {
        BaseAutocomplete.Attach($(control).closest(".autocomplete"));
    }

    static SetData(data) {
        BaseAutocomplete.autocomplitesData = data;
    }

    /**
     * Прикрепляет автокомплиты к полям, обрабатывает действия, произошедшие в автокомплитах
     * @param {*} control 
     */
    static Attach(control) {
        new Autocomplete("#" + $(control).attr("id"), {
            search: input => {
                // Автокомплит не должен работать для ридонли полей
                if ((control).find(".autocomplete-input").attr("readonly") != undefined) return;
                return new Promise(resolve => {
                    if (input.length < 3) {
                        return resolve([])
                    }

                    fetch(location.origin + BaseAutocomplete.GetUrl(control, input))
                        .then(response => {
                            switch (response["status"]) {
                                case 404:
                                    break;
                                case 200:
                                    return response.json();
                            }
                        })
                        .then(data => resolve(data))
                })
            },

            onSubmit: result => {
                // Добавление id выбранной сущности
                // Создание элемента если требуется
                let selectedItemId = BaseAutocomplete.GetSelecteditemId(control);
                let selectedItem = "<input hidden='hidden' id='" + selectedItemId + "' />";
                if ($(document).find($("#" + selectedItemId)).length == 0) {
                    $(selectedItem).insertBefore($(control));
                }
                // Запись в элемент
                if (typeof(result) != (typeof{})) {
                    $(document).find($("#" + selectedItemId)).val(result);
                }
                else {
                    $(document).find($("#" + selectedItemId)).val(result["id"]);
                }
                // Вызов событие, которое можно обработать
                BaseAutocomplete.OnSubmit(control, result);
            },

            getResultValue: result => {
                // Если результат не является объектом, то запись просто добавлется в пиклист
                if (typeof(result) != (typeof{})) return result;
                return result[BaseAutocomplete.GetPropertyName(control)];
            },
            
            renderResult: (result, props) => {
                // Значение ровно результату, если результат не является объектом
                let value = result;

                // Иначе свойству из объекта
                let newItemId = result;
                if (typeof(result) == (typeof{})) {
                    value = result[BaseAutocomplete.GetPropertyName(control)];
                    newItemId = result["id"];
                }

                // Текущее выбранное значение
                let selectedItemId = $(document).find("#" + BaseAutocomplete.GetSelecteditemId(control)).val();
                if (selectedItemId == newItemId) {
                    let element = $(`<li ${props}>${value}</li>`);
                    $(element).addClass("atc-current-val");
                    return $(element)[0].outerHTML;
                }
                return `<li ${props}>${value}</li>`;
            },
        })
    }

    static GetSelecteditemId(control) {
        return $(control).attr("id") + "_selectedItem";
    }

    /**
     * Читате json файл с конфигурацией автокмплитов и возвращает Url для запроса на сервер
     * @param {*} control контрол с автокомплитом
     * @param {*} input значение, введенное в автокомплит
     */
    static GetUrl(control, input) {
        let returnUrl = $(control).find(".autocomplete-link").attr("href");
        let defaultUrl = returnUrl + input;
        let getPropsResult = BaseAutocomplete.TryGetAutocompliteProps(control);
        if (getPropsResult["Success"]) {
            let currentAutocompliteProps = getPropsResult["Result"];

            // Получение метода переопределения текущего
            // Если название не пустое, попытка его вызова
            let overrideName = currentAutocompliteProps["GetUrlOverride"];
            if (!Utils.IsNullOrEmpty(overrideName)) {
                return GetUrl.Invoke(overrideName, control, currentAutocompliteProps);
            }

            // Если Url не пустой, то возвращается он + введенное значение
            let autocompleteUrl = currentAutocompliteProps["Url"];
            if (autocompleteUrl != undefined && autocompleteUrl != null && autocompleteUrl.length > 0) {
                returnUrl = autocompleteUrl;
                return returnUrl + value;
            }

            // Иначе необходимо получить текст, который должен прибавиться к Url, если есть такая необходимость
            // Получение текста Url из тега "UrlAppendPardText"
            let urlAppendPardText = currentAutocompliteProps["UrlAppendPardText"];
            if (urlAppendPardText != undefined && urlAppendPardText != null && urlAppendPardText.length > 0) {
                returnUrl = returnUrl + urlAppendPardText + value;
                return returnUrl;
            }

            // Получение текста Url из тега "UrlAppendPardEl"(из элемента разметки)
            let urlAppendPardEl = currentAutocompliteProps["UrlAppendPardEl"];
            if (urlAppendPardEl != undefined && urlAppendPardEl != null && urlAppendPardEl.length > 0) {
                let element = $(urlAppendPardEl);
                if (element.length == 0) return defaultUrl;
                
                let elementVal = $(element).val();
                if (elementVal.length == 0) {
                    elementVal = "null";
                }
                returnUrl = returnUrl + elementVal + "/" + input;
                return returnUrl;
            }
        }
        
        return defaultUrl;
    }

    /**
     * Получает название сввойства объекта, котороу будет отображаться из настроек автокомплита
     * @param {*} control контрол с автокомплитом
     */
    static GetPropertyName(control) {
        let propertyName = "name";
        let getPropsResult = BaseAutocomplete.TryGetAutocompliteProps(control);
        if (getPropsResult["Success"]) {
            let selectPropertyName = getPropsResult["Result"]["SelectPropertyName"];
            if (selectPropertyName != undefined && selectPropertyName != null && selectPropertyName.length > 0) {
                propertyName = selectPropertyName;
            }
        }
        return propertyName;
    }

    /**
     * Действие, происходящее при выборе из автокомплита
     * @param {*} control Элемент с автокомплитом 
     * @param {*} result Выбранный результат
     */
    static OnSubmit(control, result) {
        let getPropsResult = BaseAutocomplete.TryGetAutocompliteProps(control);
        if (getPropsResult["Success"]) {
            let onSubmitMethodName = getPropsResult["Result"]["OnSubmit"];
            if (onSubmitMethodName != undefined && onSubmitMethodName != null && onSubmitMethodName.length > 0) {
                AutocompleteSubmit.Invoke(onSubmitMethodName, control, result);
            }
        }
    }

    /**
     * Метод пытается получить настройки автокомплита
     * @param {*} control Контрол, на котором висит автокомплит
     */
    static TryGetAutocompliteProps(control) {
        let autocompliteType = $(control).attr("data-autocomplite-type");
        let autocompliteTypes = BaseAutocomplete.autocomplitesData["AutocompliteTypes"];
        autocompliteType = autocompliteTypes[autocompliteType];
        if (autocompliteType == undefined) return { Success: false };
        
        // В типе ищутся объекты, содержащие в теге "AutocompliteNames" название текущего автокомплита
        let autocompliteName = $(control).attr("data-autocomplite-name");
        let autocompliteNames = autocompliteType.filter(typeItem => {
            return typeItem["AutocompliteNames"].filter(name => name == autocompliteName).length > 0;
        });

        if (autocompliteNames.length == 0) return { Success: false };

        // Если найдено название автокомплита, то берется первый объект из массива найденных
        return {
            Success: true,
            Result: autocompliteNames[0]
        };
    }

    /**
     * Возвращает текущее значение, выбранное в автокомплите
     * @param {*} control контрол с автокомплитом
     */
    static GetValue(control) {
        if ($(control) != undefined && $(control).find(".autocomplete-input") != undefined)
            return $(control).find(".autocomplete-input").val();
        return "";
    }
}

class GetUrl {
    static Invoke(methodName, control, inputProperties) {
        let methods = new {
            GetPrimaryAccountManager: class {
                Initialize(control, inputProperties) {
                    let baseUrl = $(control).find(".autocomplete-link").attr("href");
                    let selectedOrg = $($("#userOrgsChoiseList").find(".active")[0]);
                    let selectedOrgId = $(selectedOrg).find(".choise-userorg-id").text();
                    let managerName = $(control).find(".autocomplete-input").val();
                    return baseUrl + selectedOrgId + "/" + managerName;
                }
            }
        }[methodName];
        return methods.Initialize(control, inputProperties);
    }
}

class AutocompleteSubmit {
    static Invoke(methodName, control, inputProperties) {
        let methods = new {
            PrimaryEmployeeModalSubmit: class {
                Initialize(control, inputProperties) {
                    $("#positionModal #primaryEmployeeId").val(inputProperties["id"]);
                }
            },
        
            PrimaryEmployeeSubmit: class {
                Initialize(control, inputProperties) {
                    $("#positionForm #primaryEmployeeId").val(inputProperties["id"]);
                }
            },
            
            QuoteAccountSubmit: class {
                Initialize(control, inputProperties) {
                    $("#quoteCreateModal #quoteAccountId").val(inputProperties["id"]);
                }
            },

            AccountManagerSubmit: class {
                Initialize(control, inputProperties) {
                    $("#addAccountManagerModal #accManagerId").val(inputProperties["id"]);
                }
            }
        }[methodName];
        return methods.Initialize(control, inputProperties);
    }
}