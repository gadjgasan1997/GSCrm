class Utils {
    /**
     * Метод обрабатывает список ошибок
     * @param {*} errors Объект с ошибками
     * @param {*} errorsTypeCodes Массив с кодами типов ошибок
     */
    static CommonErrosHandling(errorsArray, errorsTypeCodes) {
        !Utils.IsNullOrEmpty(errorsTypeCodes) && errorsTypeCodes.map(errorsTypeCode => {
            let typeErrors = ErrorsManager.GetError(errorsTypeCode);
            let errorAlertsArray = [];
            if (typeErrors != undefined) {
                for(let error in errorsArray) {
                    let errorText = Array.isArray(errorsArray[error]) ? errorsArray[error][0] : errorsArray[error];

                    // Получение списка всех массивов с ошибками, среди ключей которых содержится название текущей ошибки
                    let errorHandlers = typeErrors.filter(typeError => {
                        let errorCodes = typeError[0];
                        if (errorCodes.indexOf(error) != -1) {
                            return typeError;
                        }
                    });

                    // Если обработчики не найдены, попытка обработать дефолтовые ошибки
                    if (errorHandlers.length == 0) {
                        switch(error) {
                            case "UnhandledException":
                                errorAlertsArray.push(MessageManager.Invoke("CommonError", { "error": Localization.GetString("unhandledException") }));
                                break;
                            case "RecordNotFound":
                                errorAlertsArray.push(MessageManager.Invoke("CommonError", { "error": Localization.GetString("recordNotFound") }));
                                break;
                            default:
                                break;
                        }
                    }
        
                    // Обработка ошибок
                    errorHandlers.map(errorHandler => {
                        let errorSettings = errorHandler[1];
                        let errorType = errorSettings["type"];
                        switch(errorType) {
                            // Когда ошибки должны добавляться в элемент
                            case "attach":
                                let elementSelectors = errorSettings["elements"];
                                if (elementSelectors != undefined) {
                                    elementSelectors.map(elementSelector => {
                                        $(elementSelector).each((index, item) => {
                                            // Получение связанного с ошибкой поля для его подсветки
                                            let connectedFieldId = $(item).attr("data-connect-el");
                                            if (!Utils.IsNullOrEmpty(connectedFieldId)) {
                                                let connectedField = $("#" + connectedFieldId);
                                                if (connectedField.length > 0) {
                                                    $(connectedField).addClass("is-invalid");
                                                }
                                            }
                                            $(item).removeClass("d-none").append("<li>" + errorText + "</li>");
                                        });
                                    });
                                }
                                break;

                            // Когда должен происходить алерт
                            case "swal":
                                let messageName = errorSettings["messageName"];
                                if (messageName == undefined) {
                                    messageName = "CommonError";
                                }
                                errorAlertsArray.push(MessageManager.Invoke(messageName, { "error": errorText }));
                                break;
                        }
                    });
                }
            }

            if (errorAlertsArray.length > 0) {
                Swal.queue(errorAlertsArray);
            }
        });
    }

    /** Очистка ошибок */
    static ClearErrors() {
        $('.under-field-error').empty();
        $(".is-invalid").each((index, item) => $(item).removeClass("is-invalid"));
    }

    /**
     * Метод проверяет, является ли объект пустым
     * @param {*} obj 
     */
    static IsNullOrEmpty(obj) {
        if (obj == undefined) return true;
        if (obj == null) return true;
        if (obj == "") return true;
        return false;
    }
}