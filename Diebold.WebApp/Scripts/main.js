(function (Diebold, $, undefined) {
    var debug_level = 1;
    Diebold.util = function () {
        var log = function(msg, level){
            if (debug_level >= level) {
                switch (level) {
                    case 1:
                        console.log(msg);
                        break;
                    case 2:
                        console.warn(msg);
                        break;
                    case 3:
                        console.error(msg);
                        break;
                }
            }
        };

        return {
            error: function (msg) {
                log(msg, 3);
            },
            warn: function(msg){
                log(msg, 2);
            },
            debug: function (msg) {
                log(msg, 1);
            }
        };
    };
}(window.Diebold = window.Diebold || {}, jQuery));