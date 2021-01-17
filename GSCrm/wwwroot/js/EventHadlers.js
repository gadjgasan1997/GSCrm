// Инициализация автокомплита
$(document).ready(() => {
    Initializer.Execute().then(() =>
        Renderer.Render());
})

$(document).click(function () {
    $('.dropdown-el').removeClass('expanded');
    $('.popover').popover("hide");
    $("#orgSettingsMenu").addClass("d-none");
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