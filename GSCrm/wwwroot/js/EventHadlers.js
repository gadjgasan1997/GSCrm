// Инициализация автокомплита
$(document).ready(() => {
    Initializer.Execute().then(() =>
        Renderer.Render());
})

$(document).click(function () {
    $('.dropdown-el').removeClass('expanded');
    $('.popover').popover("hide");
    $(".settings-menu").addClass("d-none");
});

// Переход в профиль пользователя
$(document)
    .off("click", ".navbar .navbar-avatar").on("click", ".navbar .navbar-avatar", event => {
        let profileUrl = $(event.currentTarget).find("a").attr("href");
        location.replace(location.origin + profileUrl);
    })
    .off("click", ".navbar .navbar-avatar a").on("click", ".navbar .navbar-avatar a", event => {
        event.stopPropagation();
    })

var test = () => {
    let request = new AjaxRequests();
    request.JsonGetRequest("https://localhost:44376/EmployeePosition/bde3f125-b686-4857-d1ca-08d84deccbb1/NextAllRecords/").then(response => {
        console.log(response)
    })
}