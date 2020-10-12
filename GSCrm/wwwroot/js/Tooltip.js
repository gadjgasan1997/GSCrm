class Tooltip {
    Initialize() {
        let srcTooltips = $(".tooltip-cell-src");
        Array.from(srcTooltips).map(tooltip => {
            if ($(tooltip).hasClass("tooltip-cell-link")) {
                this.AddLinkToolTipHtml(tooltip, "src");
            }
            else this.AddToolTipHtml(tooltip, "src");
        });
    }

    AddLinkToolTipHtml(tooltipElement, tooltipTheme) {
        if (tooltipElement != undefined) {
            let tooltip = $(tooltipElement).find("a");
            if (tooltip[0] != undefined) {
                let tooltipLink = tooltip[0].outerHTML;
                let tooltipLabel = tooltip[0].innerHTML;
                let href = $(tooltipLink).attr("href");
                let tooltipHtml = "<a href=" + href + "><div class='tooltip-text'>" + tooltipLabel + "</div></a><div class='sys-tooltip-cell tooltip-" + tooltipTheme + "'><p>" + $(tooltip).text() + "</p></div>";
                $(tooltipElement).text("").append(tooltipHtml);
            }
        }
    }

    AddToolTipHtml(tooltipElement, tooltipTheme) {
        if (tooltipElement != undefined) {
            let tooltipHtml = "<div class='tooltip-text'>" + $(tooltipElement).text() + "</div><div class='sys-tooltip-cell tooltip-" + tooltipTheme + "'><p>" + $(tooltipElement).text() + "</p></div>";
            $(tooltipElement).text("");
            $(tooltipElement).append(tooltipHtml);
            $(tooltipElement).text("").append(tooltipHtml);
        }
    }

    /**
     * Метод устанавливает местоположение для тултипа
     * @param {*} tooltip 
     */
    SetLocation(tooltip) {
        let table = $(tooltip).closest(".fl-table");
        let tooltipHeight = $(tooltip).find(".sys-tooltip-cell").height();
        let tooltipTop = $(tooltip)[0].offsetTop + $(table)[0].offsetTop - tooltipHeight - 15;
        let tooltipLeft = $(tooltip).offset().left + $(tooltip)[0].offsetWidth / 2;
        $(tooltip).find(".sys-tooltip-cell").css("top", tooltipTop);
        $(tooltip).find(".sys-tooltip-cell").css("left", tooltipLeft);
    }
}

$(document)
    .off("mouseenter", ".tooltip-cell-src").on("mouseenter", ".tooltip-cell-src", event => {
        let tooltip = new Tooltip();
        tooltip.SetLocation($(event.currentTarget));
        let tooltipText = $(event.currentTarget).find(".tooltip-text");
        if (tooltipText[0] != undefined && tooltipText[0].scrollWidth > tooltipText[0].clientWidth) {
            $(event.currentTarget).addClass("tooltip-pointer");
            $(event.currentTarget).find(".sys-tooltip-cell").css("display", "block");
        }
    })
    .off("mouseleave", ".tooltip-cell-src").on("mouseleave", ".tooltip-cell-src", event => {
        $(event.currentTarget).removeClass("tooltip-pointer");
        $(event.currentTarget).find(".sys-tooltip-cell").css("display", "none");
    });