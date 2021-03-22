class BaseAutocompleteManager {
    static Initialize(control) {
        BaseAutocompleteManager.Attach($(control).closest(".autocomplete"));
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

                    let baseUrl = $(control).find(".autocomplete-link").attr("href");
                    fetch(location.origin + BaseAutocompleteManager.GetUrl(control, baseUrl, input))
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
                let selectedItemId = BaseAutocompleteManager.GetSelectedItemId(control);
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
                BaseAutocompleteManager.OnSubmit(control, result);
            },

            getResultValue: result => {
                // Если результат не является объектом, то запись просто добавлется в пиклист
                if (typeof(result) != (typeof{})) return result;
                return result[BaseAutocompleteManager.GetPropertyName(control)];
            },
            
            renderResult: (result, props) => {
                // Значение ровно результату, если результат не является объектом
                let value = result;

                // Иначе свойству из объекта
                let newItemId = result;
                if (typeof(result) == (typeof{})) {
                    value = result[BaseAutocompleteManager.GetPropertyName(control)];
                    newItemId = result["id"];
                }

                // Текущее выбранное значение
                let selectedItemId = $(document).find("#" + BaseAutocompleteManager.GetSelectedItemId(control)).val();
                if (selectedItemId == newItemId) {
                    let element = $(`<li ${props}>${value}</li>`);
                    $(element).addClass("atc-current-val");
                    return $(element)[0].outerHTML;
                }
                return `<li ${props}>${value}</li>`;
            }
        })
    }

    static GetSelectedItemId(control) {
        return $(control).attr("id") + "_selectedItem";
    }

    /**
     * Читате json файл с конфигурацией автокмплитов и возвращает Url для запроса на сервер
     * @param {*} control контрол с автокомплитом
     * @param {*} baseUrl Базовая ссылка запроса
     * @param {*} input значение, введенное в автокомплит
     */
    static GetUrl(control, baseUrl, input) {
        let defaultUrl = baseUrl + input;
        let getPropsResult = BaseAutocompleteManager.TryGetAutocompliteProps(control);
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
                baseUrl = autocompleteUrl;
                return baseUrl + value;
            }

            // Иначе необходимо получить текст, который должен прибавиться к Url, если есть такая необходимость
            // Получение текста Url из тега "UrlAppendPardText"
            let urlAppendPardText = currentAutocompliteProps["UrlAppendPardText"];
            if (urlAppendPardText != undefined && urlAppendPardText != null && urlAppendPardText.length > 0) {
                baseUrl = baseUrl + urlAppendPardText + value;
                return baseUrl;
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
                baseUrl = baseUrl + elementVal + "/" + input;
                return baseUrl;
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
        let getPropsResult = BaseAutocompleteManager.TryGetAutocompliteProps(control);
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
        let getPropsResult = BaseAutocompleteManager.TryGetAutocompliteProps(control);
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
        if (Utils.IsNullOrEmpty(autocompliteType)) return { Success: false };

        let autocompliteTypes = ConfigsData.autocomplitesData["AutocompliteTypes"];
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
}

class GetUrl {
    static Invoke(methodName, control, inputProperties) {
        let methods = new {
            GetPrimaryAccountManager: class {
                Initialize(control, inputProperties) {
                    let baseUrl = $(control).attr("data-link");
                    let selectedOrg = $($("#userOrgsChoiseList").find(".active")[0]);
                    let selectedOrgId = $(selectedOrg).find(".choise-userorg-id").text();
                    let managerName = $("#accManagerVal").val();
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

/** Автокомплит в возможностью выбора */
class SelectAutocompleteManager {
    static Initialize(control) {
        $(control)
            .append(SelectAutocompleteManager.GetAutocompliteHTML())
            .find("input")
            .autocomplete({
                create: function () {
                    $(this).data('ui-autocomplete')._renderMenu = function (ul, items) {
                        var self = this;
                        items.map(item => self._renderItemData(ul, item))
                    };

                    $(this).data('ui-autocomplete')._renderItemData = function (ul, item) {
                        return this._renderItem(ul, item).data("ui-autocomplete-item", item);
                    };

                    $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                        return $(SelectAutocompleteManager.RenderItem(control, item)).appendTo(ul);
                    };
                },

                open: function(event, ui) {
                    $(".select-autocomplete-result").attr("data-source-id", $(control).attr("id"));
                    let icon = $(control).find(".select-icon");
                    $(icon).find(".icon-chevron-thin-right").addClass("d-none");
                    $(icon).find(".icon-chevron-thin-left").removeClass("d-none");
                },

                source: ((request, response) => {
                    let inputParameter = request["term"];
                    if (inputParameter == "GetAllValues") {
                        inputParameter = $(control).find("input").val();
                    }

                    let baseUrl = $(control).attr("data-link");
                    $.ajax({
                        type: "Get",
                        contentType: "application/json",
                        dataType: "json",
                        url: BaseAutocompleteManager.GetUrl(control, baseUrl, inputParameter)
                    }).then(result => response(result))
                    return;
                }),

                focus: function(event, ui) {
                    // Снятие подсветки со всех записей в автокомплите
                    $(".ui-menu-item").removeClass("focused-item");
                    
                    // Подсветка выбранной записи
                    let currentTarget = $(".select-autocomplete-result").find(".ui-state-active").closest(".ui-menu-item");
                    currentTarget.addClass("focused-item");
                    return false;
                },

                select: function(event, ui) {
                    // Если в объекте есть свойство value, то используется оно
                    let propertyName;
                    let newItemId;
                    if (!Utils.IsNullOrEmpty(ui["item"]["value"])) {
                        propertyName = "value";
                        newItemId = ui["item"]["value"];
                    }
                    // Иначе название свойства находится с помощью метода GetPropertyName
                    else {
                        propertyName = BaseAutocompleteManager.GetPropertyName(control);
                        newItemId = ui["item"]["id"]
                    }

                    this.value = ui["item"][propertyName];
                    $(control).find("input").val(ui["item"][propertyName]);
                    
                    // Запоминание выбранной записи
                    let autocomplite = $(event.target).closest(".select-autocomplete");
                    let selectedRecord = $(autocomplite).find(".selected-autocomplite-record");
                    if (selectedRecord.length == 0) {
                        $(autocomplite).append('<span hidden="hidden" value="' + newItemId + '" class="selected-autocomplite-record"></span>');
                    }
                    else $(selectedRecord).attr("value", newItemId);

                    // Вызов событие, которое можно обработать
                    BaseAutocompleteManager.OnSubmit(control, ui["item"]);
                    return false;
                },

                change: function(event, ui) {
                    // Очистка запомненного выбранного значения в случае очистки автокомплита
                    // Необходимо проверять не только значение из "ui", но и поле input, так как
                    // значение могут не только пикнуть, но и просто вбить
                    let valueFromInput = $(event.target).val();
                    if (Utils.IsNullOrEmpty(ui["item"]) && Utils.IsNullOrEmpty(valueFromInput)) {
                        let selectedRecord = $(event.target).closest(".select-autocomplete").find(".selected-autocomplite-record")
                        if (!Utils.IsNullOrEmpty(selectedRecord)) {
                            $(selectedRecord).remove();
                        }
                    }

                    // В случае, если пользователь вбил значение в пиклист вручную, а не выбрал, необходимо запомнить его как текущее
                },

                close: function(event, ui) {
                    $(".select-autocomplete-result").removeAttr("data-source-id");
                    let icon = $(control).find(".select-icon");
                    $(icon).find(".icon-chevron-thin-right").removeClass("d-none");
                    $(icon).find(".icon-chevron-thin-left").addClass("d-none");
                },

                classes: {
                    "ui-autocomplete": "select-autocomplete-result"
                }
            });
    }

    static RenderItem(control, item) {
        if (!Utils.IsNullOrEmpty(item)) {
            let value;
            let newItemId;

            // Если в объекте есть свойство value, то значение равно этому свойству
            if (!Utils.IsNullOrEmpty(item["value"])) {
                value = item["value"];
                newItemId = item["value"];
            }
            // Иначе название свойство, значение которого будет записываться в переменную value, берется с помощью метода GetPropertyName
            else {
                value = item[BaseAutocompleteManager.GetPropertyName(control)];
                newItemId = item["id"];
            }

            // Текущее выбранное значение
            let itemBaseHTML = '<span hidden="hidden" value="' + newItemId + '" class="avtocomplite-record-id"></span><p class="label-xs">' + value + '</p>';
            let selectedRecordId = $(control).find(".selected-autocomplite-record").attr("value");
            if (selectedRecordId == newItemId) {
                return '<li class="selected-item">' + itemBaseHTML + '</li>';
            }
            return '<li>' + itemBaseHTML + '</li>';
        }
    }

    static GetAutocompliteHTML() {
        return '<span class="select-divider"></span><div class="select-icon"><span class="icon-chevron-thin-right"></span><span class="icon-chevron-thin-left d-none"></span></div>';
    }
}

$(".select-autocomplete").off("click", ".select-icon").on("click", ".select-icon", event => {
    let autocomplite = $(event.currentTarget).closest(".select-autocomplete");
    if (!$(autocomplite).hasClass("readonly-autocomplete")) {
        let autocompliteId = $(autocomplite).attr("id");
        let sourceId = $(".select-autocomplete-result").attr("data-source-id");
        if (autocompliteId != sourceId) {
            $(autocomplite).find("input").autocomplete("search", "GetAllValues");
            $(autocomplite).find("input").focus();
        }
    }
})