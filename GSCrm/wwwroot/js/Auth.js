class Authorization {
    Signup() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let requests = new AjaxRequests();
            let signupUrl = $("#signup form").attr('action');
            let signupData = this.SignupGetData();

            requests.JsonPostRequest(signupUrl, signupData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["Signup"]);
                    return reject();
                })
                .done(url => {
                    this.ClearFields();
                    window.location.href = url;
                    resolve();
                });
        })
    }

    SignupGetData() {
        return {
            UserName: $("#SignupUserName").val(),
            Email: $("#SignupEmail").val(),
            Password: $("#SignupPassword").val(),
            ConfirmPassword: $("#SignupConfirmPassword").val()
        }
    }

    Login() {
        return new Promise((resolve, reject) => {
            Utils.ClearErrors();
            let requests = new AjaxRequests();
            let loginUrl = $("#login form").attr('action');
            let loginData = this.LoginGetData();

            requests.JsonPostRequest(loginUrl, loginData)
                .fail(response => {
                    Utils.CommonErrosHandling(response["responseJSON"], ["Login"]);
                    return reject();
                })
                .done(url => {
                    this.ClearFields();
                    location.replace(location.origin + url);
                    resolve();
                });
        })
    }

    LoginGetData() {
        return {
            UserName: $("#LoginUserName").val(),
            Password: $("#LoginPassword").val()
        }
    }

    ClearFields() {
        $('.form-control').map((index, item) => $(item).val(""));
    }
}

// Страница с авторизацией
$("#signup").off("click", "#signupBtn").on("click", "#signupBtn", event => {
    event.preventDefault();
    $(".register-form .form-shadow").removeClass("d-none");
    let auth = new Authorization();
    auth.Signup().finally(() => $(".register-form .form-shadow").addClass("d-none"));
});

$("#login").off("click", "#loginBtn").on("click", "#loginBtn", event => {
    event.preventDefault();
    $(".register-form .form-shadow").removeClass("d-none");
    let auth = new Authorization();
    auth.Login().finally(() => $(".register-form .form-shadow").addClass("d-none"));
});