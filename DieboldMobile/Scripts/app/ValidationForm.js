
$(document).ready(function() {
    var validatorSettings = $.data($('form')[0], 'validator').settings;
    validatorSettings.ignore = "";
});

function validateSection(section, errorList) {

    if (errorList == undefined)
        errorList = [];

    var validator = $('form').validate();

    $(section.selector + ' input[data-val=true]').each(function() {
        if (validator.element(this) != undefined && !validator.element(this)) {
            errorList.push(validator.errorList[0].message);
        }
    });

    ShowValiationSummaryErrors(errorList);

    var anyError = errorList.length > 0;
    errorList = [];

    if (anyError) {
        return false;
    }
    else {
        return true;
    }
}

function ShowValiationSummaryErrors(errorList) {
    var container = $("[data-valmsg-summary=true]"), list = container.find("ul");

    if (list && list.length && errorList.length) {
        list.empty();
        container.addClass("validation-summary-errors").removeClass("validation-summary-valid");

        for (var i = 0; i < errorList.length; i++) {
            $(list).append($("<li />").html(errorList[i]));
        }
    }
}

function ClearValidationSummary() {
    var container = $('form').find('[data-valmsg-summary="true"]');
    var list = container.find('ul');

    if (list && list.length) {
        list.empty();
        container.addClass('validation-summary-valid').removeClass('validation-summary-errors');
    }
}

function alreadyExistsDialog(dialogContent) {
    $('<div id="dialogInfo" title="Information"> <p>The ' + dialogContent + ' already exists. </p></div>').dialog({
        autoOpen: true,
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        }
    });
}

function inputFieldEmptyDialog(dialogContent) {
    $('<div id="dialogInfo" title="Information"> <p>The ' + dialogContent + ' field is required. </p></div>').dialog({
        autoOpen: true,
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        }
    });
}

function inputFieldDialog(dialogContent) {
    $('<div id="dialogInfo" title="Information"> <p> ' + dialogContent + ' </p></div>').dialog({
        autoOpen: true,
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        }
    });
}

function SuccessfullyDialog(dialogContent) {
    $('<div id="dialogInfo" title="Information"> <p>The ' + dialogContent + ' has been saved successfully. </p></div>').dialog({
        autoOpen: true,
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        }
    });
}

function ConfirmationEMCDialog() {
    $('<div id="dialogInfo" title="Warning"><p></p><p>EMC is offline or the account ID is invalid.</p> <p>Do you want to continue anyway?</p> </div>').dialog({
        autoOpen: true,
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Confirm": function () {
                $(this).dialog("close");
                $('#CreateIfEMCFail').val(true);
                $("form").submit();
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });
}