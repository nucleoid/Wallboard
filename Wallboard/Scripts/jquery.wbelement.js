/*
* Wall Board Element plugin 1.0
*
* Author: Mitchell Statz
*/

(function ($) {
    $.fn.wbelement = function (options) {
        var defaults = {
            url: '',
            delay: 60000
        };
        options = $.extend(defaults, options);
        return this.each(function () {
            var $this = $(this);
            $this.url = options.url;
            $this.delay = options.delay;
            $this.updateWbElement($this);
        });
    };

    $.fn.updateWbElement = function (el) {
        $.fn.runUpdate(el);
        setTimeout(function () { $.fn.updateWbElement(el); }, el.delay);
    };

    $.fn.runUpdate = function (el) {
        $.ajax({
            url: el.url,
            success: function (data) {
                el.html(data);
            }
        });
    };
})(jQuery);