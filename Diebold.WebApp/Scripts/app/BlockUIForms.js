function BlockElement(elementId, message) {
    var dialogMessage = '<img src="../../Content/Default/loading.gif")" alt="Loading"/>';
    if (message) {
        dialogMessage = dialogMessage + ' ' +message;
    }
    $('#'+elementId).block({
        message: dialogMessage
    });
}
function UnBlockElement(elementId) {
    $('#'+elementId).unblock();
};