class EmployeeResponsibilityManagement {
    /** Метод инициализирует попап управления полномочиями сотрудника */
    InitializeResps() {
        return new Promise((resolve, reject) => {
            let accountTeamUrl = $("#accountForm").find("#accTeamManagementUrl").attr("href");
            let request = new AjaxRequests();
            request.CommonGetRequest(accountTeamUrl)
                .fail(error => reject(error))
                .done(response => {
                    this.ResetLists();
                    this.Render(response);
                    this.InitializePrimaryManager();
                    resolve(response);
                });
        });
    }
}