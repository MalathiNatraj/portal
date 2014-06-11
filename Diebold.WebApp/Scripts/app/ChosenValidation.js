function isChosenValid(elem) {

    var selector = '#' + elem.attr('id') + '_chzn';
    
    if (elem.val() == '') {
        $(selector).removeClass("valid").addClass("comboError");
        return false;
    }
    else {
        $(selector).removeClass("comboError").addClass("valid");
        return true;
    }
}