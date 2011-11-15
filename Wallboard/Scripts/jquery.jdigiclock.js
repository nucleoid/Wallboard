/*
 * jDigiClock plugin 2.1
 *
 * http://www.radoslavdimov.com/jquery-plugins/jquery-plugin-digiclock/
 *
 * Copyright (c) 2009 Radoslav Dimov
 *
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 *
 */


(function ($) {
    $.fn.jdigiclock = function(options) {

        var defaults = {
            clockImagesPath: 'images/clock/',
            weatherImagesPath: 'images/weather/',
            lang: 'en',
            am_pm: false,
            weatherLocationCode: 'EUR|BG|BU002|BOURGAS',
            weatherMetric: 'C',
            weatherUpdate: 0,
            proxyType: 'php'

        };

        var regional = [];
        regional['en'] = {
            monthNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            dayNames: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
        };


        options = $.extend(defaults, options);

        return this.each(function() {

            var $this = $(this);
            var o = options;
            $this.clockImagesPath = o.clockImagesPath;
            $this.weatherImagesPath = o.weatherImagesPath;
            $this.lang = regional[o.lang] == undefined ? regional['en'] : regional[o.lang];
            $this.am_pm = o.am_pm;
            $this.weatherLocationCode = o.weatherLocationCode;
            $this.weatherMetric = o.weatherMetric == 'C' ? 1 : 0;
            $this.weatherUpdate = o.weatherUpdate;
            $this.proxyType = o.proxyType;
            $this.currDate = '';
            $this.timeUpdate = '';


            var html = '<div id="plugin_container">';
            html += '<p id="left_arrow"></p>';
            html += '<p id="right_arrow"></p>';
            html += '<div id="digital_container">';
            html += '<div id="clock"></div>';
            html += '<div id="weather"></div>';
            html += '</div>';
            html += '<div id="forecast_container"></div>';
            html += '</div>';

            $this.html(html);

            $this.displayClock($this);

            $this.displayWeather($this);

            var panelPos = ($('#plugin_container > div').length - 1) * 500;
            var next = function() {
                $('#right_arrow').unbind('click', next);
                $('#plugin_container > div').filter(function(i) {
                    $(this).animate({ 'left': (i * 500) - 500 + 'px' }, 400, function() {
                        if (i == 0) {
                            $(this).appendTo('#plugin_container').css({ 'left': panelPos + 'px' });
                        }
                        $('#right_arrow').bind('click', next);
                    });
                });
            };
            $('#right_arrow').bind('click', next);

            var prev = function() {
                $('#left_arrow').unbind('click', prev);
                $('#plugin_container > div:last').prependTo('#plugin_container').css({ 'left': '-500px' });
                $('#plugin_container > div').filter(function(i) {
                    $(this).animate({ 'left': i * 500 + 'px' }, 400, function() {
                        $('#left_arrow').bind('click', prev);
                    });
                });
            };
            $('#left_arrow').bind('click', prev);

        });
    };

    $.fn.displayClock = function (el) {
        $.fn.getTime(el);
        setTimeout(function () { $.fn.displayClock(el); }, $.fn.delay());
    };

    $.fn.displayWeather = function (el) {
        $.fn.getWeather(el);
        if (el.weatherUpdate > 0) {
            setTimeout(function () { $.fn.displayWeather(el); }, (el.weatherUpdate * 60 * 1000));
        }
    };

    $.fn.delay = function () {
        var now = new Date();
        var delay = (60 - now.getSeconds()) * 1000;

        return delay;
    };

    $.fn.getTime = function (el) {
        var now = new Date();
        var old = new Date();
        old.setTime(now.getTime() - 60000);

        var nowHours, nowMinutes, oldHours, oldMinutes, timeOld = '';
        nowHours = now.getHours();
        nowMinutes = now.getMinutes();
        oldHours = old.getHours();
        oldMinutes = old.getMinutes();

        if (el.am_pm) {
            var amPm = nowHours > 11 ? 'pm' : 'am';
            nowHours = ((nowHours > 12) ? nowHours - 12 : nowHours);
            oldHours = ((oldHours > 12) ? oldHours - 12 : oldHours);
        }

        nowHours = ((nowHours < 10) ? "0" : "") + nowHours;
        nowMinutes = ((nowMinutes < 10) ? "0" : "") + nowMinutes;
        oldHours = ((oldHours < 10) ? "0" : "") + oldHours;
        oldMinutes = ((oldMinutes < 10) ? "0" : "") + oldMinutes;
        // date
        el.currDate = el.lang.dayNames[now.getDay()] + ',&nbsp;' + now.getDate() + '&nbsp;' + el.lang.monthNames[now.getMonth()];
        // time update
        el.timeUpdate = el.currDate + ',&nbsp;' + nowHours + ':' + nowMinutes;

        var firstHourDigit = oldHours.substr(0, 1);
        var secondHourDigit = oldHours.substr(1, 1);
        var firstMinuteDigit = oldMinutes.substr(0, 1);
        var secondMinuteDigit = oldMinutes.substr(1, 1);

        timeOld += '<div id="hours"><div class="line"></div>';
        timeOld += '<div id="hours_bg"><img src="' + el.clockImagesPath + 'clockbg1.png" /></div>';
        timeOld += '<img src="' + el.clockImagesPath + firstHourDigit + '.png" id="fhd" class="first_digit" />';
        timeOld += '<img src="' + el.clockImagesPath + secondHourDigit + '.png" id="shd" class="second_digit" />';
        timeOld += '</div>';
        timeOld += '<div id="minutes"><div class="line"></div>';
        if (el.am_pm) {
            timeOld += '<div id="am_pm"><img src="' + el.clockImagesPath + amPm + '.png" /></div>';
        }
        timeOld += '<div id="minutes_bg"><img src="' + el.clockImagesPath + 'clockbg1.png" /></div>';
        timeOld += '<img src="' + el.clockImagesPath + firstMinuteDigit + '.png" id="fmd" class="first_digit" />';
        timeOld += '<img src="' + el.clockImagesPath + secondMinuteDigit + '.png" id="smd" class="second_digit" />';
        timeOld += '</div>';

        el.find('#clock').html(timeOld);

        // set minutes
        if (secondMinuteDigit != '9') {
            firstMinuteDigit = firstMinuteDigit + '1';
        }

        if (oldMinutes == '59') {
            firstMinuteDigit = '511';
        }

        setTimeout(function () {
            $('#fmd').attr('src', el.clockImagesPath + firstMinuteDigit + '-1.png');
            $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg2.png');
        }, 200);
        setTimeout(function () { $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg3.png'); }, 250);
        setTimeout(function () {
            $('#fmd').attr('src', el.clockImagesPath + firstMinuteDigit + '-2.png');
            $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg4.png');
        }, 400);
        setTimeout(function () { $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg5.png'); }, 450);
        setTimeout(function () {
            $('#fmd').attr('src', el.clockImagesPath + firstMinuteDigit + '-3.png');
            $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg6.png');
        }, 600);

        setTimeout(function () {
            $('#smd').attr('src', el.clockImagesPath + secondMinuteDigit + '-1.png');
            $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg2.png');
        }, 200);
        setTimeout(function () { $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg3.png'); }, 250);
        setTimeout(function () {
            $('#smd').attr('src', el.clockImagesPath + secondMinuteDigit + '-2.png');
            $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg4.png');
        }, 400);
        setTimeout(function () { $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg5.png'); }, 450);
        setTimeout(function () {
            $('#smd').attr('src', el.clockImagesPath + secondMinuteDigit + '-3.png');
            $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg6.png');
        }, 600);

        setTimeout(function () { $('#fmd').attr('src', el.clockImagesPath + nowMinutes.substr(0, 1) + '.png'); }, 800);
        setTimeout(function () { $('#smd').attr('src', el.clockImagesPath + nowMinutes.substr(1, 1) + '.png'); }, 800);
        setTimeout(function () { $('#minutes_bg img').attr('src', el.clockImagesPath + 'clockbg1.png'); }, 850);

        // set hours
        if (nowMinutes == '00') {

            if (el.am_pm) {
                if (nowHours == '00') {
                    firstHourDigit = firstHourDigit + '1';
                    nowHours = '12';
                } else if (nowHours == '01') {
                    firstHourDigit = '001';
                    secondHourDigit = '111';
                } else {
                    firstHourDigit = firstHourDigit + '1';
                }
            } else {
                if (nowHours != '10') {
                    firstHourDigit = firstHourDigit + '1';
                }

                if (nowHours == '20') {
                    firstHourDigit = '1';
                }

                if (nowHours == '00') {
                    firstHourDigit = firstHourDigit + '1';
                    secondHourDigit = secondHourDigit + '11';
                }
            }

            setTimeout(function () {
                $('#fhd').attr('src', el.clockImagesPath + firstHourDigit + '-1.png');
                $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg2.png');
            }, 200);
            setTimeout(function () { $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg3.png'); }, 250);
            setTimeout(function () {
                $('#fhd').attr('src', el.clockImagesPath + firstHourDigit + '-2.png');
                $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg4.png');
            }, 400);
            setTimeout(function () { $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg5.png'); }, 450);
            setTimeout(function () {
                $('#fhd').attr('src', el.clockImagesPath + firstHourDigit + '-3.png');
                $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg6.png');
            }, 600);

            setTimeout(function () {
                $('#shd').attr('src', el.clockImagesPath + secondHourDigit + '-1.png');
                $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg2.png');
            }, 200);
            setTimeout(function () { $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg3.png'); }, 250);
            setTimeout(function () {
                $('#shd').attr('src', el.clockImagesPath + secondHourDigit + '-2.png');
                $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg4.png');
            }, 400);
            setTimeout(function () { $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg5.png'); }, 450);
            setTimeout(function () {
                $('#shd').attr('src', el.clockImagesPath + secondHourDigit + '-3.png');
                $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg6.png');
            }, 600);

            setTimeout(function () { $('#fhd').attr('src', el.clockImagesPath + nowHours.substr(0, 1) + '.png'); }, 800);
            setTimeout(function () { $('#shd').attr('src', el.clockImagesPath + nowHours.substr(1, 1) + '.png'); }, 800);
            setTimeout(function () { $('#hours_bg img').attr('src', el.clockImagesPath + 'clockbg1.png'); }, 850);
        }
    };

    $.fn.getWeather = function (el) {

        el.find('#weather').html('<p class="loading">Update Weather ...</p>');
        el.find('#forecast_container').html('<p class="loading">Update Weather ...</p>');
        var metric = el.weatherMetric == 1 ? 'C' : 'F';
        var proxy = '';

        switch (el.proxyType) {
            case 'php':
                proxy = 'php/proxy.php';
                break;
            case 'asp':
                proxy = 'asp/WeatherProxy.aspx';
                break;
        }

        $.getJSON('/Scripts/proxy/' + proxy + '?location=' + el.weatherLocationCode + '&metric=' + el.weatherMetric, function (data) {

            el.find('#weather .loading, #forecast_container .loading').hide();

            var currTemp = '<p class="temp">' + data.curr_temp + '&deg;<span class="metric">' + metric + '</span></p>';

            el.find('#weather').css('background', 'url(' + el.weatherImagesPath + data.curr_icon + '.png) 50% 100% no-repeat');
            var weather = '<div id="local"><p class="city">' + data.city + '</p><p>' + data.curr_text + '</p></div>';
            weather += '<div id="temp"><p id="date">' + el.currDate + '</p>' + currTemp + '</div>';
            el.find('#weather').html(weather);

            // forecast
            el.find('#forecast_container').append('<div id="current"></div>');
            var currFor = currTemp + '<p class="high_low">' + data.forecast[0].day_htemp + '&deg;&nbsp;/&nbsp;' + data.forecast[0].day_ltemp + '&deg;</p>';
            currFor += '<p class="city">' + data.city + '</p>';
            currFor += '<p class="text">' + data.forecast[0].day_text + '</p>';
            el.find('#current').css('background', 'url(' + el.weatherImagesPath + data.forecast[0].day_icon + '.png) 50% 0 no-repeat').append(currFor);

            el.find('#forecast_container').append('<ul id="forecast"></ul>');
            data.forecast.shift();
            for (var i in data.forecast) {
                var dDate = new Date(data.forecast[i].day_date);
                var dayName = el.lang.dayNames[dDate.getDay()];
                var forecast = '<li>';
                forecast += '<p>' + dayName + '</p>';
                forecast += '<img src="' + el.weatherImagesPath + data.forecast[i].day_icon + '.png" alt="' + data.forecast[i].day_text + '" title="' + data.forecast[i].day_text + '" />';
                forecast += '<p>' + data.forecast[i].day_htemp + '&deg;&nbsp;/&nbsp;' + data.forecast[i].day_ltemp + '&deg;</p>';
                forecast += '</li>';
                el.find('#forecast').append(forecast);
            }
        });
    };
})(jQuery);