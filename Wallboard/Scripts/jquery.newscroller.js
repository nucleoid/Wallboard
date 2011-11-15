(function ($) {
    $.fn.newsScroller = function(options) {
        var defaults = {
            speed: 400,
            list_item_height: '109px'
        },
            settings = $.extend({ }, defaults, options),
            $this = $(this);
        setInterval(function() {
            return $this.children('li:first').animate({ marginTop: '-' + settings.list_item_height, opacity: 'hide' }, settings.speed,
                function() {
                    $this.children('li:first').appendTo($this).css('marginTop', '0').fadeIn(300);
                });
        }, 3000);
    };
})(jQuery);