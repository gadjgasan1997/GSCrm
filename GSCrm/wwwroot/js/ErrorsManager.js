class ErrorsManager {
    static GetError(errorCode) {
        return ConfigsData.errorsData[errorCode];
    }
}