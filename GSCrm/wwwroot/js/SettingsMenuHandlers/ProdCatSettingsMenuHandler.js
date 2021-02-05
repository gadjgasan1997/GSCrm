class ProdCatSettingsMenuHandler {
    /**
     * ����� ������������ ������� �������� ����
     * @param {*} event 
     * @param {*} settingsMenuEl 
     */
    OpenHandle(event, settingsMenuEl) {
        let categoryRow = $(event.currentTarget).closest(".category-row");
        let dataCategoryId = $(categoryRow).attr("data-category-id");
        $(settingsMenuEl).attr("data-source-id", dataCategoryId);
    }
}