class Division {
    Create() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let request = new AjaxRequests();
            let createDivUrl = $("#divisionModal form").attr("action");
            let createDivData = this.GetCreationData();

            request.JsonPostRequest(createDivUrl, createDivData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["CreateDivision"]);
                })
                .done(() => {
                    $("#divisionModal").modal("hide");
                    location.reload();
                });
        })
    }

    Remove(event) {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemoveDivConfirmation")).then(result => {
                if (result.value) {
                    let request = new AjaxRequests();
                    let removeDivUrl = $(event.currentTarget).find(".remove-item-url a").attr("href");
                    
                    request.CommonDeleteRequest(removeDivUrl)
                        .fail(response => {
                            Utils.DefaultErrorHandling(response["responseJSON"]);
                            reject();
                        })
                        .done(() => location.reload());
                }
            })
        })
    }
    
    Cancel() {
        this.ClearFields();
        $("#divisionModal").modal("hide");
    }

    GetCreationData() {
        return {
            Name: $("#divName").val(),
            OrganizationId: $("#orgId").val(),
            ParentDivisionName: AutocompleteManager.GetValue($("#parentDiv"))
        }
    }

    ClearFields() {
        $("#divName").val("");
    }
}

// Список подразделений
$("#divisionsList").off("click", ".remove-item-btn").on("click", ".remove-item-btn", event => {
    event.preventDefault();
    let division = new Division();
    division.Remove(event);
})

// Модальное окно создания подразделения
$("#divisionModal").off("click", "#createDivBtn").on("click", "#createDivBtn", event => {
    event.preventDefault();
    let division = new Division();
    division.Create();
});

$("#divisionModal").off("click", "#cancelCreationDivBtn").on("click", "#cancelCreationDivBtn", event => {
    event.preventDefault();
    let division = new Division();
    division.Cancel();
});