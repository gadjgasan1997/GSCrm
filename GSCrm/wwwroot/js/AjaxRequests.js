class AjaxRequests {
    JsonPostRequest(url, data) {
        return $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            dataType: "json",
            url: url,
            data: data
        });
    }

    JsonGetRequest(url) {
        return $.ajax({
            type: "Get",
            contentType: "application/json",
            dataType: "json",
            url: url
        });
    }

    CommonPostRequest(url, data) {
        return $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: url,
            data: data
        });
    }

    CommonGetRequest(url) {
        return $.ajax({
            type: "Get",
            contentType: "application/json",
            url: url
        });
    }

    CommonHtmlRequest(url) {
        return $.ajax({
            type: "Get",
            contentType: "application/json",
            url: url,
            dataType: "html"
        });
    }

    CommonDeleteRequest(url) {
        return $.ajax({
            type: "Delete",
            url: url
        });
    }
}