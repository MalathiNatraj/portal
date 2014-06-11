(function ($) {
    $.validator.addMethod("requiredcombovalidatorcheck", function (value, element, param) {
        if (value > 0)
            return true;

        return false;
    }, '');

    // register the validator
    $.validator.unobtrusive.adapters.add("requiredcombo", {}, function (options) {
        options.rules['requiredcombovalidatorcheck'] = true;
        if (options.message) options.messages["requiredcombovalidatorcheck"] = options.message;
    });
} (jQuery));