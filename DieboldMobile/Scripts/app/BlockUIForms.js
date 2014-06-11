
function BlockUI() {
    $.blockUI(
        {
            message: $('#waitMessage')
        });
}

function UnBlockUI() {
    $.unblockUI();
}

function BlockUIInfoMessage(content) {
    $.growlUI(content + ' Notification', 'The ' + content + ' was created successfully');
}