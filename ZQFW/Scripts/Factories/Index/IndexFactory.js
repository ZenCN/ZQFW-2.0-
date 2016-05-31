App.factory('ngCss', ['$timeout', function ($timeout) {
    return function (selector, boxName) {
        $timeout(function () {
            var $element = $(selector);
            var setting = JSON.parse($element.attr("ng-css"));
            if (setting) {
                $element.css(setting.attr, $(setting.selector + "." + boxName).css(setting.attr));
            }
        }, 0, false);
    };
}]);
