
function showDialog(content, title, onCloseExternal, optionsExternal) {
    var dialogContent = $('<div>' + content + '</div>').not('script');
    var scriptContent = $(content).filter('script');

    var onClose = function() {
        $(this).dialog('destroy');
        $(this).remove();
        scriptContent.remove();

        if (onCloseExternal) onCloseExternal();
    };

    var options = {
        title: title,
        resizable: false,
        autoOpen: false,
        draggable: false,
        modal: true,
        close: onClose,
        buttons: {
            Close: onClose
        }
    };

    if (optionsExternal && optionsExternal.height) options.height = optionsExternal.height;
    if (optionsExternal && optionsExternal.width) options.width = optionsExternal.width;
    if (optionsExternal && optionsExternal.buttons) options.buttons = optionsExternal.buttons;

    dialogContent.dialog(options);
    scriptContent.appendTo('body');
    dialogContent.dialog('open');
}

function call(url, data, onSuccess, onError) {

    $.post(url, data,
        function (result) {

            if (result.Status == "OK") {
                if (onSuccess) onSuccess();

            } else {
                showDialog(result.Message, "Error");
                if (onError) onError();
            }

        }).error(function () { showDialog("An error occurred while processing this action.", "Error"); });
}

function callWithBlockUI(url, data, jqgrid, onSuccess, onError) {

    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'json',
        data: data,
        success: function (result) {

            if (result.Status == "OK") {
                
                if (result.Message != undefined)
                    showDialog(result.Message, "Action Response");
                
                if (onSuccess) onSuccess();

            } else {
                showDialog(result.Message, "Error");
                if (onError) onError();
            }
        },
        error: function (xhr) {
            showDialog("An error occurred while processing this action.", "Error");
        },
        beforeSend: function () {
            blockUI('html');
        },
        complete: function () {

            if (jqgrid != undefined) {
                $(jqgrid).trigger("reloadGrid");
            }

            $('html').unblock();
        }
    });
    
}


function fillPartial(url, data, containerId, blockArea, onSuccess) {

    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function (result) {
            $(containerId).html(result);

            if (onSuccess) onSuccess();
        },
        error: function (xhr) {
            $(blockArea).html("<div class='inBoxMessage'><div>Information is not available</div></div>");
        },
        beforeSend: function (jqXHR, settings) {
            $(blockArea).block({
                message: '<img src="../../Content/Default/loading.gif" />',
                css: { border: 'none' },
                overlayCSS: { backgroundColor: '#fff' }
            });
        },
        complete: function (jqXHR, textStatus) {
            $(blockArea).unblock();
        }
    });
}

function blockUI(blockArea) {
    $(blockArea).block({
            message: '<div id="waitMessage"> ' +
                '<img  src="../../Content/Default/loading.gif"/>' +
                '<span>Please wait...</span>' +
            '</div>'
    });
};

function blockUIImageOnly(blockArea) {
    $(blockArea).block({
        message: '<img src="../../Content/Default/loading.gif" />',
        css: { border: 'none' },
        overlayCSS: { backgroundColor: '#fff' }
    });
};

function growlUI(messageTitle, messageBody) {
    $.growlUI(messageTitle, messageBody);
};

function actionCall(url, data, onSuccess, pk) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function (result) {
            if (onSuccess) onSuccess(result, pk);
        },
        error: function (xhr) {
            showDialog("An error occurred while processing this action.", "Error");
        },
        beforeSend: function () {
            blockUI('html');
        },
        complete: function () {
            $('html').unblock();
        }
    });
}

function postPartial(url, data, async, blockArea, onSuccess, onComplete, toBlockUI) {
    $.ajax({
        url: url,
        async: async,
        type: 'POST',
        data: data,
        success: function (result) {

            if (result.Status != undefined && result.Status == "Error") {
                showDialog(result.Message, "Action Response");
            }

            if (onSuccess) onSuccess(result);
        },
        error: function () {
            showDialog("An error occurred while processing this action.", "Error");
        },
        beforeSend: function () {

            if (toBlockUI != undefined && toBlockUI) {
                $(blockArea).block({
                    message: '<img src="../../Content/Default/loading.gif" />',
                    css: { border: 'none' },
                    overlayCSS: { backgroundColor: '#fff' }
                });
            }
            
        },
        complete: function () {

            if (toBlockUI != undefined && toBlockUI) {
                $(blockArea).unblock();
            }
            
            if (onComplete) onComplete();
        }
    });
}
function centerViewPortDialog(dialogId) {
    var viewportWidth = jQuery(window).width();
    var viewportHeight = jQuery(window).height();
    document.getElementById('my_overlay').style.visibility = 'visible';
    $(dialogId).css({
        "z-index": "201",
        "position": "fixed",
        "display": "block",
        "visibility": "visible",
        "top": ((viewportHeight / 2) - ($(dialogId).height() / 2)) + "px",
        "left": ((viewportWidth / 2) - ($(dialogId).width() / 2)) + "px"
    });
}
function centerViewPortDialog1(dialogId) {
    var viewportWidth = jQuery(window).width();
    var viewportHeight = jQuery(window).height();   
    document.getElementById('my_overlay').style.visibility = 'visible'; 
    $(dialogId).css({
        "z-index": "201",
        "position": "fixed",
        "display": "block",
        "visibility": "visible",
        "top": ((viewportHeight / 2) - ($(dialogId).height() / 2)) + "px",
        "left": ((viewportWidth / 2) - ($(dialogId).width() / 2)) + "px"
    });
}

function centerViewPortDialogBig(dialogId) {
    var viewportWidth = jQuery(window).width();
    var viewportHeight = jQuery(window).height();
    document.getElementById('my_overlay').style.visibility = 'visible';
    $(dialogId).css({
        "z-index": "201",
        "position": "fixed",
        "display": "block",
        "visibility": "visible",
        "top": ((viewportHeight / 2) - ($(dialogId).height() / 2)) + "px",
        "left": ((viewportWidth / 2) - ($(dialogId).width() / 2)) + "px",
    });
}

function fnShowAlertInfo(message) {
    $("#lblDieboldAlert").text(message);
    $("#DieboldAlertWindow").data("kendoWindow").center().open();
}
function ClosewndAlertInformation() {
    $("#DieboldAlertWindow").data("kendoWindow").close();
}

function timeIt(minutes, target) {
    // set the date we're counting down to
    var target_date = new Date().getTime() + (minutes * 60 * 1000);

    // variables for time units
    var minutes, seconds;

    // get tag element
    var countdown = document.getElementById(target);

    // update the tag with id "countdown" every 1 second
    setTimeout(function () {
        var countdownId = setInterval(function () {

            // find the amount of "seconds" between now and target
            var current_date = new Date().getTime();
            var seconds_left = (target_date - current_date) / 1000;

            // do some time calculations
            minutes = parseInt(seconds_left / 60);
            seconds = parseInt(seconds_left % 60);
            if (minutes == 0 && seconds == 0) {
                countdown.innerHTML = '0:00';
                clearInterval(countdownId);
            } else {
                if (seconds < 10) {
                    seconds = "0" + seconds;
                }
                // format countdown string + set tag value
                countdown.innerHTML = minutes + " : " + seconds;
            }

        }, 1000);
    }, 10);
}