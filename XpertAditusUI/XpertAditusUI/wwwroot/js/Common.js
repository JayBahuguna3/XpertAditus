function showSpinner() {
    $("#divSpinner").show();
}

function hideSpinner() {
    $("#divSpinner").hide();
}

function clickAndDisable(name) {
    $("#" + name).addClass("disabled");
}

function removeDisable(name) {
    $("#" + name).removeClass("disabled");
}