class Utils {
    // Паттерн для ошибок с недостаточными полномочиями
    static NO_RES_PATTERN = "NoRes";

    /**
     * Метод обрабатывает список ошибок
     * @param {*} errorsArray Массив с названиями ошибок ошибками
     * @param {*} errorsTypeCodes Массив с кодами типов ошибок
     */
    static CommonErrosHandling(errorsArray, errorsTypeCodes) {
        return new Promise((resolve, reject) => {
            !Utils.IsNullOrEmpty(errorsTypeCodes) && errorsTypeCodes.map(errorsTypeCode => {
                let typeErrors = ErrorsManager.GetError(errorsTypeCode);
                let errorAlertsArray = [];
                if (typeErrors != undefined) {
                    for(let errorName in errorsArray) {
                        let errorText = Array.isArray(errorsArray[errorName]) ? errorsArray[errorName][0] : errorsArray[errorName];
    
                        // Получение списка всех массивов с ошибками, среди ключей которых содержится название текущей ошибки
                        let errorHandlers = Utils.GetErrorHandlers(errorName, typeErrors);
    
                        // Если обработчики не найдены, попытка обработать дефолтовые ошибки
                        if (errorHandlers.length == 0) {
                            let alertError = Utils.GetDefaultErrorMessage({
                                messageName: errorName,
                                errorText: errorText
                            });
                            alertError != undefined && errorAlertsArray.push(alertError);
                        }
            
                        // Обработка ошибок
                        errorHandlers.map(errorHandler => {
                            let errorSettings = errorHandler[1];
                            let errorType = errorSettings["type"];
                            switch(errorType) {
                                // Когда ошибки должны добавляться в элемент
                                case "attach":
                                    Utils.ErrorTypeAttachHandler({
                                        elements: errorSettings["elements"],
                                        errorText: errorText
                                    });
                                    break;
    
                                // Когда должен происходить алерт
                                case "swal":
                                    errorAlertsArray.push(Utils.GetErrorMessage({
                                        messageName: errorSettings["messageName"],
                                        errorText: errorText
                                    }));
                                    break;
                            }
                        });
                    }
                }
    
                if (errorAlertsArray.length > 0) {
                    Swal.queue(errorAlertsArray).then(() => resolve());
                }
                else resolve();
            });
        })
    }

    /**
     * Обработка ошибки по умолчанию
     * @param {*} errorsArray 
     */
    static DefaultErrorHandling(errorsArray) {
        let errorAlertsArray = [];
        // Формирование массива с алертами
        for (let errorName in errorsArray) {
            errorAlertsArray.push(Utils.GetDefaultErrorMessage({
                messageName: errorName,
                errorText: errorsArray[errorName]
            }));
        }
        // Вывод очередью на экран
        if (errorAlertsArray.length > 0) {
            Swal.queue(errorAlertsArray);
        }
    }

    /**
     * Метод возвращает список обработчиков
     * @param {*} errorCode
     * @param {*} typeErrors Типы ошибок, для которых надо получить обработчики
     */
    static GetErrorHandlers(errorCode, typeErrors) {
        return typeErrors.filter(typeError => {
            let errorCodes = typeError[0];
            if (errorCodes.indexOf(errorCode) != -1) {
                return typeError;
            }
        });
    }

    /**
     * Метод обрабатывает ошибки с типом "Message"
     * @param {*} errorSettings 
     */
    static GetErrorMessage(errorSettings) {
        let messageName = errorSettings["messageName"];
        if (messageName == undefined) {
            messageName = "CommonError";
        }
        return MessageManager.Invoke(messageName, { "error": errorSettings["errorText"] });
    }

    /**
     * Метод возвращает сообщения для ошибок по умолчанию
     * @param {*} errorSettings
     */
    static GetDefaultErrorMessage(errorSettings) {
        let messageName = errorSettings["messageName"];
        let errorText = errorSettings["errorText"];
        switch(messageName) {
            // необработанное исключение
            case "UnhandledException":
                return MessageManager.Invoke("CommonError", { "error": !Utils.IsNullOrEmpty(errorText) ? errorText : Localization.GetString("unhandledException") });
                
            // Запись не найдена
            case "RecordNotFound":
                return MessageManager.Invoke("CommonError", { "error": !Utils.IsNullOrEmpty(errorText) ? errorText : Localization.GetString("recordNotFound") });
            
            // Обработка ошибок, коды которых удовлетворяют определенным паттернам
            default:
                // Обработка ошибок недостаточности полномочий
                if (messageName.split(Utils.NO_RES_PATTERN).length > 1) {
                    return MessageManager.Invoke("HasNotPermissions", { "error": errorText });
                }
        }
    }

    /**
     * Метод обрабатывает ошибки с типом "Attach"
     * @param {*} errorSettings 
     */
    static ErrorTypeAttachHandler(errorSettings) {
        let elementSelectors = errorSettings["elements"];
        if (elementSelectors != undefined) {
            elementSelectors.map(elementSelector => {
                $(elementSelector).each((index, item) => {
                    // Получение связанного с ошибкой поля для его подсветки
                    let connectedFieldId = $(item).attr("data-connect-el");
                    if (!Utils.IsNullOrEmpty(connectedFieldId)) {
                        let connectedField = $("" + connectedFieldId);
                        if (connectedField.length > 0) {
                            $(connectedField).addClass("is-invalid");
                        }
                    }
                    $(item).removeClass("d-none").append("<li>" + errorSettings["errorText"] + "</li>");
                });
            });
        }
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

    /**
     * Метод сравнивает два массива, и, если они равны, возвращает true
     * @param {*} arrayOne 
     * @param {*} arrayTwo 
     */
    static CheckArraysSame(arrayOne, arrayTwo) {
        let isEqual = true;
        // Проверка на длину
        if (arrayOne.length != arrayTwo.length) {
            isEqual = false;
            return;
        }

        // Проверка элементов на равенство
        arrayOne.map((item, index) => {
            if (isEqual && arrayTwo[index] != item) {
                isEqual = false;
            }
        })
        return isEqual;
    }
}