$.ackColumnFormatter = function(cellvalue, options, rowObject) {
    switch (cellvalue){
        case "Green":
            return "<span class='icon bulletGreen bold'></span>";
                
        case "Yellow":
            return "<span class='icon bulletYellow bold'></span>";
                
        case "Red":
            return "<span class='icon bulletRed bold'></span>";
    }
};

$.currentStatusColumnFormatter = function(cellvalue, options, rowObject) {
    if(cellvalue) {
        return "<span class='icon bulletGreen bold'></span>";
    } else {
        return "<span class='icon bulletRed bold'></span>";
    }
};

function doNotificationAction(alertId, actionPath, title) {
    confirmDialog('<div id="confirmDialog"> <p>Do you want to continue?</p></div>', alertId, actionPath, null, title);
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

function doPost(itemId, actionPath, jqgrid) {
    callWithBlockUI(actionPath, { alertStatusId: itemId }, jqgrid);
}