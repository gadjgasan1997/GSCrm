class Utils {
    // ������� ��� ������ � �������������� ������������
    static NO_RES_PATTERN = "NoRes";

    /**
     * ����� ������������ ������ ������
     * @param {*} errorsArray ������ � ���������� ������ ��������
     * @param {*} errorsTypeCodes ������ � ������ ����� ������
     */
    static CommonErrosHandling(errorsArray, errorsTypeCodes) {
        !Utils.IsNullOrEmpty(errorsTypeCodes) && errorsTypeCodes.map(errorsTypeCode => {
            let typeErrors = ErrorsManager.GetError(errorsTypeCode);
            let errorAlertsArray = [];
            if (typeErrors != undefined) {
                for(let errorName in errorsArray) {
                    let errorText = Array.isArray(errorsArray[errorName]) ? errorsArray[errorName][0] : errorsArray[errorName];

                    // ��������� ������ ���� �������� � ��������, ����� ������ ������� ���������� �������� ������� ������
                    let errorHandlers = Utils.GetErrorHandlers(errorName, typeErrors);

                    // ���� ����������� �� �������, ������� ���������� ���������� ������
                    if (errorHandlers.length == 0) {
                        let alertError = Utils.GetDefaultErrorMessage({
                            messageName: errorName,
                            errorText: errorText
                        });
                        alertError != undefined && errorAlertsArray.push(alertError);
                    }
        
                    // ��������� ������
                    errorHandlers.map(errorHandler => {
                        let errorSettings = errorHandler[1];
                        let errorType = errorSettings["type"];
                        switch(errorType) {
                            // ����� ������ ������ ����������� � �������
                            case "attach":
                                Utils.ErrorTypeAttachHandler({
                                    elements: errorSettings["elements"],
                                    errorText: errorText
                                });
                                break;

                            // ����� ������ ����������� �����
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
                Swal.queue(errorAlertsArray);
            }
        });
    }

    /**
     * ��������� ������ �� ���������
     * @param {*} errorsArray 
     */
    static DefaultErrorHandling(errorsArray) {
        let errorAlertsArray = [];
        // ������������ ������� � ��������
        for (let errorName in errorsArray) {
            errorAlertsArray.push(Utils.GetDefaultErrorMessage({
                messageName: errorName,
                errorText: errorsArray[errorName]
            }));
        }
        // ����� �������� �� �����
        if (errorAlertsArray.length > 0) {
            Swal.queue(errorAlertsArray);
        }
    }

    /**
     * ����� ���������� ������ ������������
     * @param {*} errorCode
     * @param {*} typeErrors ���� ������, ��� ������� ���� �������� �����������
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
     * ����� ������������ ������ � ����� "Message"
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
     * ����� ���������� ��������� ��� ������ �� ���������
     * @param {*} errorSettings
     */
    static GetDefaultErrorMessage(errorSettings) {
        let messageName = errorSettings["messageName"];
        let errorText = errorSettings["errorText"];
        switch(messageName) {
            // �������������� ����������
            case "UnhandledException":
                return MessageManager.Invoke("CommonError", { "error": Localization.GetString("unhandledException") });
                
            // ������ �� �������
            case "RecordNotFound":
                return MessageManager.Invoke("CommonError", { "error": Localization.GetString("recordNotFound") });
            
            // ��������� ������, ���� ������� ������������� ������������ ���������
            default:
                // ��������� ������ ��������������� ����������
                if (messageName.split(Utils.NO_RES_PATTERN).length > 1) {
                    return MessageManager.Invoke("HasNotPermissions", { "error": errorText });
                }
        }
    }

    /**
     * ����� ������������ ������ � ����� "Attach"
     * @param {*} errorSettings 
     */
    static ErrorTypeAttachHandler(errorSettings) {
        let elementSelectors = errorSettings["elements"];
        if (elementSelectors != undefined) {
            elementSelectors.map(elementSelector => {
                $(elementSelector).each((index, item) => {
                    // ��������� ���������� � ������� ���� ��� ��� ���������
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

    /** ������� ������ */
    static ClearErrors() {
        $('.under-field-error').empty();
        $(".is-invalid").each((index, item) => $(item).removeClass("is-invalid"));
    }

    /**
     * ����� ���������, �������� �� ������ ������
     * @param {*} obj 
     */
    static IsNullOrEmpty(obj) {
        if (obj == undefined) return true;
        if (obj == null) return true;
        if (obj == "") return true;
        return false;
    }

    /**
     * ����� ���������� ��� �������, �, ���� ��� �����, ���������� true
     * @param {*} arrayOne 
     * @param {*} arrayTwo 
     */
    static CheckArraysSame(arrayOne, arrayTwo) {
        let isEqual = true;
        // �������� �� �����
        if (arrayOne.length != arrayTwo.length) {
            isEqual = false;
            return;
        }

        // �������� ��������� �� ���������
        arrayOne.map((item, index) => {
            if (isEqual && arrayTwo[index] != item) {
                isEqual = false;
            }
        })
        return isEqual;
    }
}