App.factory('screen', ['$timeout', function ($timeout) {
    return {
        box: {
            left: undefined,
            right: undefined,
            top: undefined,
            bottom: undefined
        },
        $element: undefined,
        CallBack: function() {
            var curReport = $scope.Attr ? $scope[$scope.Attr.NameSpace].Report.Current : $scope.Open.Report.Current;
            if (['HP01', 'NP01'].In_Array(curReport.ReportTitle.ORD_Code) && curReport.Attr.TableIndex < 2 || curReport.ReportTitle.ORD_Code == 'HL01') {
                App.Plugin.TableFixed.FixedPage[curReport.Attr.TableIndex].Resize();
            }
        },
        Maximize: function (selector, obj) {
            _this = this;
            $timeout(function () {
                _this.$element = $(selector);

                if (_this.$element && !_this.$element.is(":animated")) {
                    _this.box = {
                        left: _this.$element.css("left"),
                        right: _this.$element.css("right"),
                        top: _this.$element.css("top"),
                        bottom: _this.$element.css("bottom")
                    };

                    _this.$element.animate({
                        left: '0px',
                        right: '0px',
                        top: '0px',
                        bottom: '0px'
                    }, _this.CallBack);
                    obj.State = 'full';
                }
            });
        },
        Restore: function (selector, obj) {
            _this = this;
            if (_this.$element && !_this.$element.is(":animated")) {
                $timeout(function () {
                    _this.$element.animate({
                        left: _this.box.left,
                        right: _this.box.right,
                        top: _this.box.top,
                        bottom: _this.box.bottom
                    }, _this.CallBack);
                });
                obj.State = 'inhreit';
            }
        }
    };
}]);

App.factory('loading', [function () {
    return {        
        run: function() {
            window.plugins.preloader('start');
            setTimeout(function () {
                window.plugins.preloader('stop');
            }, 400);
        },
        start: function() {
            window.plugins.preloader('start');
        },
        stop: function () {
            setTimeout(function () {
                window.plugins.preloader('stop');
            }, 400);
        }
    };
}]);