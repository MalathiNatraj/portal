function LaunchModalCreate(entityName, getURL, postURL, callback) {
    showEntityForm(entityName, getURL, postURL, false, callback);
}

function LaunchModalEdit(entityName, getURL, postURL, callback) {
    showEntityForm(entityName, getURL, postURL, true, callback);
}

function LaunchModalDelete(entityName, deleteURL, callback) {
    var buttonOpts = {};
    
    var containerId = "frmDelete" + entityName;
    var title = "Delete " + entityName;

    buttonOpts["Yes"] = function () {

        //if (ajaxDelete(deleteURL)) {
        if (postChanges(deleteURL, null, true)){

            $(this).dialog("close");

            callback();
        }
    };

    buttonOpts["No"] = function () {
        $(this).dialog("close");
    };

    $("<div/>").attr("id", containerId).dialog({
            hide: 'fade',
            show: 'fade',
            title: title, 
            autoOpen: true,
            width: '400px',
            height: '400px',
            modal: true,
            resizable: true,
            autoResize: true,
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            
            buttons: buttonOpts,
            
            open: function (event, ui) {
                $(this).append("<div id='errorListContainer'></div>");
                $(this).append("<p>Confirm?</p>");
            },
            close: function () {
                $(this).remove();
            }
        });        
}

function showEntityForm(entityName, getURL, postURL, isEdit, callback) {
    var buttonOpts = {};
    var containerId;
    var buttonText;
    var title;

    if (isEdit) {
        containerId = "frmEdit" + entityName;
        buttonText = "Save";
        title = "Edit " + entityName;
    }
    else {
        containerId = "frmCreate" + entityName;
        buttonText = "Create"
        title = "New " + entityName;
    }


    buttonOpts[buttonText] = function () {
        //tomo el form adentro del container
        form = $("#" + containerId + " form");

        removeErrors($("#errorListContainer"));

        if (form.valid()) {
            //leer resultado de validaciones de server y mostrarlos...
            //si devuelve true, hacer el close, sino dejarlo.
                        
            if (postChanges(postURL, form, false)) {

                $(this).dialog("close");

                callback();
            }
        }
        else {
            var errMessArr = [];

            $.each(form.data('validator').errorList, function (index, value) {
                errMessArr.push(value.message);
            });

            displayErrors($("#errorListContainer"), errMessArr);
        }
    };
    
    buttonOpts["Cancel"] = function () {
        $(this).dialog("close");
    };

    $("<div/>").attr("id", containerId).load(getURL, function (responseText, textStatus, XMLHttpRequest) {
        //para hacer el parse hay que hacerlo aca adentro, cuadno ya termino de ejecutarse el load...
        $.validator.unobtrusive.parse("#" + containerId + " form");

        //poner la inicializacion de controles tuneados x jquery
        $("select").bsmSelect();

        //alert('fin load');
    }).dialog({

        //$("<div/>").attr("id", containerId).append($("#innerFrm").html()).dialog({
        //$("<div/>").attr("id", containerId).dialog({
        hide: 'fade',
        show: 'fade',
        title: title,
        autoOpen: true,
        //width: '400px',
        //height: '800px',
        modal: true,
        //  resizable: true,
        //autoResize: true,
        overlay: {
            opacity: 0.5,
            background: "black"
        },

        buttons: buttonOpts,

        create: function (event, ui) {
            //$(this).html($("#innerFrm").html());
            //alert('create load');
        },

        open: function (event, ui) {
        },
        close: function () {
            $(this).remove();
        }
    });         
}

function postChanges(postURL, form, isDelete) {

    var result = false;
    var dataToPost;

    if (!isDelete) {
        dataToPost = form.serialize();
    }

    $.ajax({
        async: false,
        type: "POST",
        url: postURL,
        data: dataToPost,
        dataType: "json",
        beforeSend: function (jqXHR, settings) {
            blockPage();
        },
        complete: function (jqXHR, textStatus) {
            unBlockPage();
        },
        success: function (data) {
            result = true;
        },
        error: function (jqXHR, textStatus, errorThrow) {
            
            var errMessArr = [];

            $.each($.parseJSON(jqXHR.responseText).messages, function (index, value) {
                errMessArr.push(value);
            });

            displayErrors($("#errorListContainer"), errMessArr);
            
            result = false;            
        }
    });

    return result;
}

function displayErrors(errorContainer, errorMessages) {
    var errorList = $("<ul></ul>");
    var errorItem;

    $.each(errorMessages, function (index, value) {
        errorItem = $("<li />").html(value);
        errorList.append(errorItem);
    });

    errorContainer.addClass("ui-state-error");
    errorContainer.append(errorList);
}

function removeErrors(errorContainer) {
    errorContainer.empty();
    errorContainer.removeClass("ui-state-error");
}

function blockPage() {
    /*
    form.block({
        centery: false,
    });
    */

    $.blockUI({
            message: '<img src="../../Content/Default/loading.gif" />'
        });
}

function unBlockPage() {
    //form.unblock();
    $.unblockUI();
}