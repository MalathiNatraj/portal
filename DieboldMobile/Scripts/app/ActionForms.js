
function removeItem(itemId, actionPath, jqgrid) {
    confirmDialog('<div id="confirmDialog" title="Delete Item"> <p>Are you sure you want to delete this item?</p></div>', itemId, actionPath, jqgrid);
}

function revokeItem(itemId, actionPath, jqgrid) {
    confirmDialog('<div id="confirmDialog" title="Revoke certificate Item"> <p>Are you sure you want to revoke certifivate of this item?</p></div>', itemId, actionPath, jqgrid);
}

function enableItem(itemId, actionPath, jqgrid) {
    confirmDialog('<div id="confirmDialog" title="Enable Item"> <p>Are you sure you want to enable this item?</p></div>', itemId, actionPath, jqgrid);
}

function acknowledgeAlert(itemId, actionPath) {
    confirmAcknowledgeAlert('<div id="confirmDialog" title="Acknowledge Alert"> <p>Are you sure you want to Acknowledge this Alert?</p></div>', itemId, actionPath);
}

function disableItem(itemId, actionPath, jqgrid) {
    confirmDialog('<div id="confirmDialog" title="Disable Item"> <p>Are you sure you want to disable this item?</p></div>', itemId, actionPath, jqgrid);
}

function doPost(itemId, actionPath, jqgrid) {

    callWithBlockUI(actionPath, { id: itemId }, jqgrid);
}

function doDiagnosticAction(itemId, actionPath, title) {
    confirmDialog('<div id="confirmDialog"> <p>Do you want to continue?</p></div>', itemId, actionPath, null, title);
}

function confirmDialog(dialogContent, itemId, actionPath, jqgrid, title) {
    $(dialogContent).dialog({
            autoOpen: true,
            resizable: false,
            title: (title != undefined) ? title : '',
            height: 140,
            modal: true,
            buttons: {
                "Confirm": function() {
                    $(this).dialog("close");
                    doPost(itemId, actionPath, jqgrid);
                },
                Cancel: function() {
                    $(this).dialog("close");
                }
            }
        });
}

function fillMonitoringOverview() {
    fillPartial("Dashboard/Overview", null, "#monitoringBox", "#monitoringBox");
}

function confirmAcknowledgeAlert(dialogContent, itemId, actionPath) {
    $(dialogContent).dialog({
        autoOpen: true,
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Confirm": function () {
                $(this).dialog("close");
                $.ajax({
                    url: actionPath,
                    type: 'POST',
                    data: { id: itemId },
                    success: function (data) {
                        $("#alertList").trigger("reloadGrid");
                        $("#alertListComplete").trigger("reloadGrid");

                        //refresh monitoring overview
                        fillMonitoringOverview();

                        $(this).dialog("close");
                        return;
                    },
                    error: function (xhr) {
                        alert("An error occurred while processing this action.");
                    }
                });
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });
}