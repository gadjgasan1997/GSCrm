class Notification {
    //#region HasReedSign
    /** ����� �������� ����������� ��� ����������� */
    MakeHasReed(event) {
        return new Promise((resolve, reject) => {
            // ��������� ����� �����������
            let notificationItem = $(event.currentTarget).closest(".notification-item");
            $(notificationItem)
                .find(".alert")
                .removeClass("alert-dark")
                .addClass("alert-light")
                .addClass("alert-not-light");

            // ������ �� ����������� ��������
            let notificationItemId = $(notificationItem).find(".notification-item-id").val();
            let makeHasReedBaseUrl = $(event.currentTarget).closest("form").attr("action");
            let makeHasReedUrl = makeHasReedBaseUrl + Localization.GetUri("makeNotHasReed") + notificationItemId;
            let request = new AjaxRequests();
            request.CommonGetRequest(makeHasReedUrl);
        })
    }

    /** ����� �������� ����������� ��� �� ����������� */
    MakeHasNoReed(event) {
        return new Promise((resolve, reject) => {
            // ��������� ����� �����������
            let notificationItem = $(event.currentTarget).closest(".notification-item");
            $(notificationItem)
                .find(".alert")
                .removeClass("alert-light")
                .removeClass("alert-not-light")
                .addClass("alert-dark");

            // ������ �� ����������� ��������
            let notificationItemId = $(notificationItem).find(".notification-item-id").val();
            let makeHasNoReedBaseUrl = $(event.currentTarget).closest("form").attr("action");
            let makeHasNoReedUrl = makeHasNoReedBaseUrl + Localization.GetUri("makeNotHasNoReed") + notificationItemId;
            let request = new AjaxRequests();
            request.CommonGetRequest(makeHasNoReedUrl);
        })
    }

    /** ����� �������� ��� ����������� ��� ����������� */
    ReadAll() {
        return new Promise((resolve, reject) => {
            let readAllUrl = $("#readAllNots").closest("form").attr("action");
            let request = new AjaxRequests();
            request.CommonGetRequest(readAllUrl).then(() => location.reload());
        })
    }
    //#endregion

    //#region OrgInvite
    /**
     * �������� �� ���������� � �����������
     * @param {*} event 
     */
    AcceptInvite(event) {
        return new Promise((resolve, reject) => {
            let acceptInviteUrl = $(event.currentTarget).closest("form").attr("action");
            let requests = new AjaxRequests();
            let hasErrors = false;
            requests.CommonGetRequest(acceptInviteUrl)
                .catch(response => {
                    hasErrors = true;
                    Utils.CommonErrosHandling(response["responseJSON"], ["AcceptInvite"]).then(() => location.reload())
                })
                .then(() => {
                    if (!hasErrors) {
                        Swal.fire(MessageManager.Invoke("InviteHasBeenAccepted")).then(() => location.reload());
                    }
                });
        })
    }

    /**
     * ����� �� ���������� � �����������
     * @param {*} event 
     */
    RejectInvite(event) {
        return new Promise((resolve, reject) => {
            let rejectInviteUrl = $(event.currentTarget).closest("form").attr("action");
            let requests = new AjaxRequests();
            requests.CommonGetRequest(rejectInviteUrl).then(() => {
                Swal.fire(MessageManager.Invoke("InviteHasBeenRejected")).then(() => location.reload());
            });
        })
    }
    //#endregion
}

// �������� �����������
$("#notificationsList")
    // �������� ��� �����������
    .off("click", "#readAllNots").on("click", "#readAllNots", event => {
        event.stopPropagation();
        event.preventDefault();
        let notification = new Notification();
        notification.ReadAll();
    })
    // ������������ ��������
    .off("click", ".cbx-not-onload").on("click", ".cbx-not-onload", event => {
        $(event.currentTarget).removeClass("cbx-not-onload");
        $(event.currentTarget).addClass("cbx-not");
        let notification = new Notification();
        notification.MakeHasNoReed(event);
    })
    // ������������ ��������
    .off("click", ".cbx-not-non-active").on("click", ".cbx-not-non-active", event => {
        $(event.currentTarget).removeClass("cbx-not-non-active");
        $(event.currentTarget).addClass("cbx-not");
        let notification = new Notification();
        notification.MakeHasNoReed(event);
    })
    // ������ ��������
    .off("click", ".cbx-not").on("click", ".cbx-not", event => {
        $(event.currentTarget).removeClass("cbx-not");
        $(event.currentTarget).addClass("cbx-not-non-active");
        let notification = new Notification();
        notification.MakeHasReed(event);
    })
    // �������� ����������� � �����������
    .off("click", ".accept-invite-btn").on("click", ".accept-invite-btn", event => {
        event.preventDefault();
        event.stopPropagation();
        let notification = new Notification();
        notification.AcceptInvite(event);
    })
    // ����� �� ����������� � �����������
    .off("click", ".reject-invite-btn").on("click", ".reject-invite-btn", event => {
        event.preventDefault();
        event.stopPropagation();
        let notification = new Notification();
        notification.RejectInvite(event);
    })