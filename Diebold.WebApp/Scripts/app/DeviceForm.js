
$(document).ready(function () {

    if ($('#CompanyId').val() == null || $('#CompanyId').val() == '') {
        $('#HealthCheckVersion').attr("disabled", "disabled");
    }

    $("#DeviceType").CascadingDropDown("#HealthCheckVersion", '/Device/AsyncDeviceTypes',
        {
            postData: function () {
                return { healthCheckVersion: $('#HealthCheckVersion').val() };
            },
            onLoaded: function () {
                $('#DeviceType').trigger("liszt:updated");
            },
            onReseted: function () {
                $('#DeviceType').trigger("liszt:updated");
            }
        });

    $("#GatewayId").CascadingDropDown("#SiteId", '/Device/AsyncGateways',
        {
            postData: function () {
                return { id: $('#SiteId').val() };
            },
            onLoaded: function () {
                $('#GatewayId').trigger("liszt:updated");
            }
            ,
            onReseted: function () {
                $('#GatewayId').trigger("liszt:updated");
            }
        });

    $("#SiteId").CascadingDropDown("#CompanyId", '/Device/AsyncSites',
        {
            postData: function () {
                return { id: $('#CompanyId').val() };
            },
            onLoaded: function () {
                $('#SiteId').trigger("liszt:updated");

                if ($('#CompanyId').val() != '')
                    $('#HealthCheckVersion').removeAttr("disabled");
                else
                    $('#HealthCheckVersion').attr("disabled", "disabled");

                $('#HealthCheckVersion').trigger("liszt:updated");

                if ($('#DeviceType').val() != '')
                    $('#DeviceType').trigger('change');
            }
            ,
            onReseted: function () {
                $('#SiteId').trigger("liszt:updated");

                if ($('#CompanyId').val() != '')
                    $('#HealthCheckVersion').removeAttr("disabled");
                else
                    $('#HealthCheckVersion').attr("disabled", "disabled");

                $('#HealthCheckVersion').trigger("liszt:updated");

                if ($('#DeviceType').val() != '')
                    $('#DeviceType').trigger('change');
            }
        });

    $(".chzn-select").chosen();


    $('.chzn-select').bind("change", function () {
        $('form').validate().element($(this));
    });


    $('.step1').show();


    $('#deviceCameraTable input').attr('readonly', true);


    $('#btnAddCameras').live('click', function () {
        if (ValidNumberOfCameras()) {
            AddCameras(false);

            HideStep();
            $('.step2').show();
        }
        else {
            inputFieldDialog('The number of cameras should be between 1 and 32');
        }
    });


    $('#btnFinishAddCameras').live('click', function () {
        HideStep();
        $('.step1').show();
    });


    $('#btnNewCamera').live('click', function () {
        HideStep();
        $('.step3').show();

    });


    $('#btnCancelAddCamera').live('click', function () {
        CleanInputControlValues();
        HideStep();
        $('.step2').show();
    });


    $('#btnAddCamera').live('click', function () {

        var validator = $("form").validate();
        var anyError = false;

        $('.step3').find("input").each(function () {
            if (!validator.element(this)) {
                anyError = true;
            }
        });

        if (anyError)
            return false;
        else {
            if ($('#RowId').val() != '') {
                UpdateValuesInTable();
            }
            else {
                CreateCamera();
            }

            HideStep();
            $('.step2').show();

            return true;
        }
    });


    $("#removeCamera").live('click', function () {
        $(this).closest('tr').hide();
    });


    $("#editCamera").live('click', function () {
        var rowId = GetRowId($(this).closest('tr'));

        $('#CameraName').val($("input[name$='[" + rowId + "].CameraName']").val());
        $('#Channel').val($("input[name$='[" + rowId + "].Channel']").val());
        $('#RowId').val(rowId);

        var activeStatus = $("input[name$='[" + rowId + "].Active']").val();

        if (activeStatus.toLowerCase() == "true")
            $('#Active').attr('checked', true);
        else
            $('#Active').removeAttr('checked');


        HideStep();
        $('.step3').show();
    });

    $('#btnSubmit').click(function (e) {
        $('#CameraName').val('-');
        $('#Channel').val('-');

        if ($("form").valid()) {
            if (!ValidNumberOfCameras()) {
                inputFieldDialog('The number of cameras should be between 1 and 32');
                e.preventDefault();
                e.stopPropagation();
            }
            else if (ValidIfNumberOfCamerasWasChanged()) {
                confirmDialog('<div id="confirmDialog" title="Cameras"> <p>The Number of cameras was changed</p> <p>Do you want to add/remove the cameras automatically?</p></div>', false);
            }
            else {
                blockUI('html');
                if ($("form").attr("action").indexOf("/Device/Edit") == 0) {
                    $("form").submit();
                }
                else {
                    ChangeToCreateAction(e);
                }
            }
        }
    });

    $('#btnCreateAnother').click(function (e) {
        $('#CameraName').val('-');
        $('#Channel').val('-');

        if ($("form").valid()) {
            if (!ValidNumberOfCameras()) {
                inputFieldDialog('The number of cameras should be between 1 and 32');
                e.preventDefault();
                e.stopPropagation();
            }
            else if (ValidIfNumberOfCamerasWasChanged()) {
                confirmDialog('<div id="confirmDialog" title="Cameras"> <p>The Number of cameras was changed</p> <p>Do you want to add/remove the cameras automatically?</p></div>', true);
            }
            else {
                blockUI('html');
                ChangeToCreateAnotherAction(e);
            }
        }
    });

    $('#DeviceType').change(function () {
        
        if ($('#DeviceType').val() == '') {
            $('#alarmConfigurationTable tbody').remove();
            return false;
        }

        $.ajax({
            url: '/Device/AsyncAlarmConfigurations/',
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html',
            data: { deviceType: $('#DeviceType').val(), companyId: $('#CompanyId').val() },
            beforeSend: function () {
                $('#alarmConfigurationContainer').block({
                    message: '<img src="../../Content/Default/loading.gif" />'
                });
            },
            complete: function () {
                $('#alarmConfigurationContainer').unblock();
            },
            success: function (result) {
                if (result != '') {
                    $('#alarmConfigurationTable').html(result);
                }
                else {
                    $('#alarmConfigurationTable tbody').remove();
                }
            },
            error: (function (xhr, status) {
                alert(xhr.responseText);
            })
        });
    });
});


function ChangeToEditAction(e) {
    e.preventDefault();
    e.stopPropagation();
    $("form").attr("action", "/Device/Edit");
    $("form").submit();
}


function ChangeToCreateAction(e) {
    e.preventDefault();
    e.stopPropagation();
    $("form").attr("action", "/Device/Create");
    $("form").submit();
}


function ChangeToCreateAnotherAction(e) {
    e.preventDefault();
    e.stopPropagation();
    $("form").attr("action", "/Device/CreateAnother");
    $("form").submit();
}


function HideStep() {
    $(".wizard-step:visible").hide();
}


function CleanInputControlValues() {
    $('#CameraName').val('');
    $('#Channel').val('');
    $('#RowId').val('');
    $('#Active').removeAttr('checked');
}


function UpdateValuesInTable() {
    $('table tr').find("input[name$='[" + $('#RowId').val() + "].CameraName']").val($('#CameraName').val());
    $('table tr').find("input[name$='[" + $('#RowId').val() + "].Channel']").val($('#Channel').val());
    $('table tr').find("input[name$='[" + $('#RowId').val() + "].Active']").val(($("#Active:checked").val() != undefined) ? $("#Active:checked").val() : false);

    if ($("#Active:checked").val() == undefined && $("#Active:checked").val() != 'true') {
        $('table tr').find("input[name$='[" + $('#RowId').val() + "].Active']").parent().find("span").removeClass('bulletGreen').addClass('icon bulletRed bold');
    }
    else {
        $('table tr').find("input[name$='[" + $('#RowId').val() + "].Active']").parent().find("span").removeClass('bulletRed').addClass('icon bulletGreen bold');
    }

    CleanInputControlValues();
}

function hasActiveAlerts(result, channel) {

    var hasAlert = false;

    $.each(result, function (index) {
        if (result[index] != null && result[index].Data.Channel == channel)
            hasAlert = true;
    });

    return hasAlert;
}
var pageShouldSubmit = false;
function onSuccessGetCamerasWithActiveAlerts(result) {
    var cameraValue = $('#NumberOfCameras').val();
    var rows = $("#deviceCameraTable tr").size() - 1;
    var deletedCameras = 0;
    var camerasToProcess = $("#deviceCameraTable tr").size() - 1;
    var hasAlert = false;
    while (deletedCameras != (rows - cameraValue) && camerasToProcess >= 0) {
        var channel = $("#deviceCameraTable tr").eq(camerasToProcess).find("input[name*=Channel]").val();

        if (hasActiveAlerts(result, channel)) {
            hasAlert = true;
        }
        deletedCameras += 1;
        camerasToProcess -= 1;
    }

    if (hasAlert == true) {
        confirmDialogForRemoveActiveCamera('<div id="confirmDialogForRemoveActiveCamera" title="Disable Item"> <p>Some camera have active alert.Do you want remove?</p></div>', result, pageShouldSubmit, "Remove Camera");
    }
    else {
        RemoveCameras(result, pageShouldSubmit);
    }
}

function RemoveCameras(result, NeedToSubmit) {
    var cameraValue = $('#NumberOfCameras').val();
    var rows = $("#deviceCameraTable tr").size() - 1;
    var deletedCameras = 0;
    var camerasToProcess = $("#deviceCameraTable tr").size() - 1;    
    while (deletedCameras != (rows - cameraValue) && camerasToProcess >= 0) {
         var channel = $("#deviceCameraTable tr").eq(camerasToProcess).find("input[name*=Channel]").val();
         if (!hasActiveAlerts(result, channel)) {
              $("#deviceCameraTable tr").eq(camerasToProcess).remove(); 
              deletedCameras += 1;
         }
           camerasToProcess -= 1;
     }    
    if (cameraValue != $("#deviceCameraTable tr").size() - 1) {
        $('#NumberOfCameras').val($("#deviceCameraTable tr").size());
        showDialog("Some cameras could not be removed because they have active alerts", "Cameras");
    }
    if (NeedToSubmit) {        
        PostData();
    }
    else {
        GetCameraIds();
    }
}

function OverrideActiveCameras(NeedToSubmit) {

    var cameraValue = $('#NumberOfCameras').val();
    var rows = $("#deviceCameraTable tr").size() - 1;

    var deletedCameras = 0;
    var camerasToProcess = $("#deviceCameraTable tr").size() - 1;
    while (deletedCameras != (rows - cameraValue) && camerasToProcess >= 0) {
        var channel = $("#deviceCameraTable tr").eq(camerasToProcess).find("input[name*=Channel]").val();

        $("#deviceCameraTable tr").eq(camerasToProcess).remove();
        deletedCameras += 1;
        camerasToProcess -= 1;
    }   
    if (NeedToSubmit) {
        PostData();
    }
    else {
        GetCameraIds();
    }
}

function AddCameras(NeedToSubmit) {
    pageShouldSubmit = NeedToSubmit;
    if (ValidNumberOfCameras()) {

        var cameraValue = $('#NumberOfCameras').val();
        var rows = $("#deviceCameraTable tr").size() - 1;
        var rowsTocreate = cameraValue - rows;

        if (rows < cameraValue) {
            for (var i = 0; i < rowsTocreate; i++) {
                CreateCamera();
            }
            if (NeedToSubmit) {
                PostData();
            }
        }
        else if (rows > cameraValue) {
            if ($('#Id').val() != undefined) {
                postPartial("/Device/AsyncGetCamerasWithActiveAlerts", { deviceId: $('#Id').val() }, true, $('#deviceCameraTable'), onSuccessGetCamerasWithActiveAlerts, null, true);
            }
            else {
                for (var itemCount = (rows - cameraValue); itemCount > 0; itemCount--) {
                    $("#deviceCameraTable tr").last().remove();
                }

                if (NeedToSubmit) {
                    PostData();
                }
                else {
                    PostData();
                }
            }
        }
    }
}


function CreateCamera() {
    //var defaultCameraName = 'Camera ' + $("#deviceCameraTable tr").size();
    //var numberOfChannelInit = $("#deviceCameraTable tr").size();
    var MaxId = GetMaxChennalId() + 1;
    var defaultCameraName = 'Camera ' + MaxId;
    var numberOfChannelInit = MaxId;
    
    var index = $("#deviceCameraTable tr").size() - 1;

    var newRow = $("<tr/>");
    //var newRow = (index % 2 == 0) ? $("<tr/>") : $("<tr class='ui-widget-content dieboldAlternativeGridRow'/>");

    $(newRow).append($('<td/>').append($('<input/>').attr('name', '[' + index + '].Active').attr('type', 'hidden')
												            .attr('value', $('input[name=Active]').is(':checked')))
                                       .append($("<span class='icon bulletRed bold'/>")));

    $(newRow).append($('<td/>').append($('<input/>').attr('name', '[' + index + '].CameraName').attr('type', 'text')
                                                            .attr('value', defaultCameraName).attr('readonly', "readonly").text(defaultCameraName)));

    $(newRow).append($('<td/>').append($('<input/>').attr('name', '[' + index + '].Channel').attr('type', 'text')
                                                            .attr('value', numberOfChannelInit).attr('readonly', 'readonly').text(numberOfChannelInit)));

    var actions = $('<td/>');
    $(actions).append("<a title='Edit' href='#' id='editCamera'><img src='../../Content/images/icons/edit.png'/></a>");
    $(newRow).append(actions);

    $("#deviceCameraTable").append(newRow);
    CleanInputControlValues();
}


function GetRowId(element) {
    return $(element).index();
}


function ValidNumberOfCameras() {
    return ($("#NumberOfCameras").valid() == 1 && $("#NumberOfCameras").val() > 0 && $("#NumberOfCameras").val() < 33);
}


function ValidIfNumberOfCamerasWasChanged() {
    return (!($("#NumberOfCameras").val() == $("#deviceCameraTable tr").size() - 1));
}

function GetMaxChennalId() {
    var camerasToProcess = $("#deviceCameraTable tr").size() - 1;
    var CameraIds = null; 
    var MaxId = 0;
    var RowId = 0;
    while (camerasToProcess >= 0) {
        var channel = $("#deviceCameraTable tr").eq(RowId).find("input[name*=Channel]").val();        
        if (channel != undefined) {
            MaxId = channel;
            if (MaxId == null) {
                MaxId = channel;
            }
            else {
                if (MaxId > channel) {                    
                    MaxId = channel;
                }
            }
        }
        RowId += 1;
        camerasToProcess -= 1;
    }
    return parseInt(MaxId);
}

function GetCameraIds() {
    var camerasToProcess = $("#deviceCameraTable tr").size() - 1;
    var CameraIds = null; 
    while (camerasToProcess >= 0) {
        var channel = $("#deviceCameraTable tr").eq(camerasToProcess).find("input[name*=Channel]").val();
        if (channel != undefined) {
            if (CameraIds != null) {
                CameraIds = CameraIds + "," + channel;
            }
            else {
                CameraIds = channel;
            }
        }
        camerasToProcess -= 1;
    }
    $('#UpdatedCamera').val(CameraIds); 
}

function PostData() {
    GetCameraIds();
    $('#CameraName').val('-');
    $('#Channel').val('-');
    blockUI('html');
    pageShouldSubmit = false;
    if ($("form").attr("action").indexOf("/Device/Edit") == 0) {                
        $("form").submit();
    }
    else {       
        $("form").attr("action", "/Device/Create");
        $("form").submit();
    }
}

function confirmDialogForRemoveActiveCamera(dialogContent, resultInfo, NeedToSubmit, title) {
    $(dialogContent).dialog({
        autoOpen: true,
        resizable: false,
        title: (title != undefined) ? title : '',
        height: 140,
        modal: true,
        buttons: {
            "Confirm": function () {
                $(this).dialog("close");
                OverrideActiveCameras(NeedToSubmit);
                //RemoveCameras(resultInfo, NeedToSubmit);
            },
            Cancel: function () {
                $(this).dialog("close");
                RemoveCameras(resultInfo, NeedToSubmit);
            }
        }
    });
}


function confirmDialog(dialogContent, createAnother) {
    $(dialogContent).dialog({
        autoOpen: true,
        resizable: false,
        height: 168,
        modal: true,
        buttons: {
            "Confirm": function (e) {
                $(this).dialog("close");
                if (createAnother)
                    AddCameras(false);
                else {
                    AddCameras(true);
                    return false;
                }

                $('#CameraName').val('-');
                $('#Channel').val('-');

                blockUI('html');

                if (createAnother)
                    ChangeToCreateAnotherAction(e);
                else
                    if ($("form").attr("action").indexOf("/Device/Edit") != 0) {
                        ChangeToCreateAction(e);
                    }
            },
            Cancel: function (e) {
                $(this).dialog("close");
                $("#NumberOfCameras").val($("#deviceCameraTable tr").size() - 1);

                blockUI('html');

                if (createAnother)
                    ChangeToCreateAnotherAction(e);
                else
                    if ($("form").attr("action").indexOf("/Device/Edit") == 0) {
                        $("form").submit();
                    }
                    else {
                        ChangeToCreateAction(e);
                    }
            }
        }
    });
}