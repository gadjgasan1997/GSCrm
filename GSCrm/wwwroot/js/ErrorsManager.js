class ErrorsManager {
    static errors = {};

    static SetData(data) {
        ErrorsManager.errors = data;
    }

    static GetError(errorCode) {
        return ErrorsManager.errors[errorCode];
    }
}