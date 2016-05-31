App.directive('ngTree', function () {
    return function ($scope) {
        $scope[$scope.Attr.NameSpace].Tree.Refresh.NodeState();
        $scope[$scope.Attr.NameSpace].Tree.Refresh.Report();
    };
});

App.directive('ngWdatepicker', ['$parse', function($parse) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function($scope, $element, $attrs, ngModelCtrl) {
            $(function () {
                $element.click(function () {
                    var setting = $(this).attr("ng-wdatepicker").trim().length > 0 ? JSON.parse($(this).attr("ng-wdatepicker")) : {};
                    if (setting.minDate == "cur_year") {
                        setting.minDate = new Date().getFullYear() + "-01-01";
                    } else {
                        setting.minDate = setting.minDate ? $parse(setting.minDate)($scope) : "1900-01-01";
                    }
                    WdatePicker({
                        dateFmt: setting.dateFmt ? setting.dateFmt : "yyyy-MM-dd",
                        minDate: setting.minDate,
                        maxDate: setting.maxDate ? $parse(setting.maxDate)($scope) : "2091-06-22",
                        onpicked: function() {
                            $element.scope().$apply(function () {
                                var M = $dp.cal.date.M, d = $dp.cal.date.d, H, m, s;
                                switch (setting.dateFmt) {
                                    case "yyyy-MM-dd HH":
                                        H = $dp.cal.date.H;
                                        ngModelCtrl.$setViewValue($dp.cal.date.y + "-" + (M < 10 ? "0" + M : M) + "-" + (d < 10 ? "0" + d : d) + " " + (H < 10 ? "0" + H : H));
                                        break;
                                    case "yyyy":
                                        ngModelCtrl.$setViewValue($dp.cal.date.y);
                                        break;
                                    case "MM":
                                        ngModelCtrl.$setViewValue(M < 10 ? "0" + M : M);
                                        break;
                                    case "dd":
                                        ngModelCtrl.$setViewValue(d < 10 ? "0" + d : d);
                                        break;
                                    case "yyyy-MM-dd HH:mm:ss":
                                        H = $dp.cal.date.H;
                                        m = $dp.cal.date.m;
                                        s = $dp.cal.date.s;
                                        ngModelCtrl.$setViewValue($dp.cal.date.y + "-" + (M < 10 ? "0" + M : M) + "-" + (d < 10 ? "0" + d : d) + " " + (H < 10 ? "0" + H : H) + ":" + (m < 10 ? "0" + m : m) + ":" + (s < 10 ? "0" + s : s));    
                                        break;
                                    default:
                                        ngModelCtrl.$setViewValue($dp.cal.date.y + "-" + (M < 10 ? "0" + M : M) + "-" + (d < 10 ? "0" + d : d));
                                        break;
                                }
                                if (setting.callback) {
                                    $parse(setting.callback)($scope)();
                                }
                            });
                        }
                    });
                });
            });
        }
    };
}]);

App.directive("ngAutowidth", function () {
    return function ($scope, $element) {
        if ($scope.$last) {
            if ($scope.$index > 0) {
                var width = ($scope.$index + 1) * 500 + 45;
                $element.parent().css("min-width", width);
            } else {
                $element.css("width", "100%");
            }
        }
    };
});

App.directive("ngDeathtree", function () {
    return function ($scope, $element) {
        if ($scope.BaseData.DeathReason) {
            $(function () {
                $element.click(function () {
                    $scope.BaseData.DeathReason.zTree.parent().show().css({
                        "top": ($(this).offset().top + $(this).height()) + "px",
                        "left": ($(this).offset().left - 6) + "px"
                    });
                    $(document.body).bind("mousedown", $scope.BaseData.DeathReason.Hide);
                    $scope.BaseData.DeathReason.Model = $element.scope().hl012;
                });
            });
        }
    };
});

/*App.directive('draggable', function ($document) {
    var startX = 0, startY = 0, x = 0, y = 0;
    return function (scope, element, attr) {
        element.css({ position: 'absolute', cursor: 'pointer' });
        element.bind('mousedown', function (event) {
            startX = event.screenX - x;
            startY = event.screenY - y;
            $document.bind('mousemove', mousemove);
            $document.bind('mouseup', mouseup);
        });

        function mousemove(event) {
            y = event.screenY - startY;
            x = event.screenX - startX;
            element.css({ top: y + 'px', left: x + 'px' });
        }

        function mouseup() {
            $document.unbind('mousemove', mousemove);
            $document.unbind('mouseup', mouseup);
        }
    };
});*/

App.directive('disableKey', function() {
    return function (scope, element, attr) {
        var keycode, arr;
        if (typeof attr["disableKey"] == "string" && attr["disableKey"].length > 0) {
            arr = attr["disableKey"].split(',');
        } else {
            arr = ['enter'];
        }

        angular.forEach(arr, function(key) {
            switch (key) {
            case "space":
                keycode = 32;
                break;
            default:
                keycode = 13;
            }
            element.keydown(function() {
                if (event.keyCode == keycode || event.which == keycode) {
                    return false;
                }
            });
        });
    }
});

App.directive("ngFileupload", function () {
    return function ($scope, $element) {
        $(function () {
            $element.uploadify({
                'auto': false,
                'swf': '../../CSS/Plugins/Uploadify/uploadify.swf',
                'uploader': '/AffixOperater/UploadFiles',
                'buttonText': $.cookie('limit') == 5 ? '添加附件' : '添加',
                'fileTypeExts': '*.*',  //pdf;*.rar;*.doc;*.xlsx;*.docx;*.xls;
                'queueSizeLimit': 20,
                'uploadLimit': 20,
                'fileSizeLimit': '5MB',
                'width': 60,
                'height': 25,
                'removeTimeout': 1,
                'onUploadSuccess': function (file, data) {
                    data = eval("(" + data + ")");
                    if (angular.isObject(data) && !data.Message) {
                        data.name = file.name;

                        if ($scope.BaseData.Unit.Local.Limit != 5) {
                            if ($scope.NameSapce != "View") {
                                $scope.Open.Report.Current.Affix.push(data);
                            } else {
                                $scope.Report.Current.Affix.push(data);
                            }
                        } else {
                            $scope.New.Report.Current.Affix.push(data);
                        }
                    } else {
                        Alert("附件保存失败!");
                    }
                },
                'onQueueComplete': function (queueData) {
                    $scope.$apply();
                    Alert("保存成功", 1000);
                },
                'onFallback': function() {
                    var span = "<span id='fileUpload' style='margin-left:8px;font-size:14px;'>您的电脑没装Flash Player，无法上传文件，点击" +
                        "<a style='text-decoration:none;font-weight:bolder;' href='javascript:void(0)'>这里</a>" + 
                        "下载安装，安装后重启浏览器即可上传文件</sapn>";
                    $("#file_upload").replaceWith(span);

                    $("#fileUpload a").click(function () {
                        var report = $scope.Open.Report;

                        if ($scope.NameSapce == "View") {
                            report = $scope.Report;
                        }

                        if (report.Current.ReportTitle.PageNO == 0 || report.Current.Attr.Data_Changed) {
                            Alert("您的报表尚未保存，请先保存再下载安装Flash Player", 3000);
                            return true;
                        }

                        $.ajax({
                            url: '/AffixOperater/DownloadFile?file_name=Flash Player（点击安装）.exe',
                            success: function(result) {
                                if (result == 1) {
                                    window.location.href = '../Document/File/Flash Player（点击安装）.exe';
                                } else {
                                    Alert("Flash Player（点击安装）.exe文件不存在");
                                }
                            }
                        });
                    });
                },
                onSelect: function () {
                    if ($scope.NameSapce != "View" && $scope.Open.Report.Current.ReportTitle.PageNO > 0) {  //修改时自动上传
                        $scope.Open.Report.Fn.Core.Upload.Affix();   
                    }
                }
            });
        });
    };
});

App.directive('tableFixed', ['$timeout', function ($timeout) {
    return function ($scope, $element, $attr) {
        if (App.Plugin) {
            $timeout(function() {
                App.Plugin.TableFixed.Fix({
                    Index: $scope.Report[$scope.Attr.NameSpace].Current.Attr.TableIndex,
                    Element: $element,
                    Type: $attr["tableFixed"] == null ? "All" : $attr["tableFixed"]
                });
            }, 0, false);
        }
    };
}]);

App.directive('ajaxLoad', ['$http', '$timeout', function($http, $timeout) {
    return function($scope, $element) {
        if ($scope.BaseData.Unit.Local.Limit == 2) {
            var scroll = function() {
                if (this.scrollTop + this.clientHeight >= this.scrollHeight) { //报表的滚动条已经滚到底部
                    var obj = $scope.BaseData.Unit.Unders.Next("UnitName", $scope.BaseData.Reservoir.Last().City);
                    if (angular.isObject(obj)) {
                        $(this).find(".Loader").show();
                        $http.post('Index/GetNMReservoir?unitcode=' + obj.UnitCode + "&unitname=" + obj.UnitName + "&pageNO=" + $scope[$scope.Attr.NameSpace].Report.Current.ReportTitle.PageNO).success(function (data) {
                            if (data.Reservoir.Unders.length > 0) {
                                if ($.isEmptyObject($scope.BaseData.Reservoir.Find("City", data.Reservoir.City))) {
                                    $scope.BaseData.Reservoir.push(data.Reservoir);

                                    var np011S = App.Models.NP.NP01.NP011.Array([data.Reservoir], data.NP011);
                                    $.each(np011S, function() {
                                        np011S[0].ZKR = App.Tools.Calculator.Addition(np011S[0].ZKR, this.ZKR);
                                        np011S[0].SKR = App.Tools.Calculator.Addition(np011S[0].SKR, this.SKR);
                                        np011S[0].DQXSL = App.Tools.Calculator.Addition(np011S[0].DQXSL, this.DQXSL);
                                    });

                                    $scope[$scope.Attr.NameSpace].Report.Current.NP011 = $scope[$scope.Attr.NameSpace].Report.Current.NP011.concat(np011S);
                                    $timeout(function() {
                                        App.Plugin.TableFixed.FixedPage[$scope[$scope.Attr.NameSpace].Report.Current.Attr.TableIndex].Resize();
                                        $element.find(".Loader").hide();
                                    });
                                } else {
                                    $element.find(".Loader").hide();
                                }
                            } else {
                                $element.find(".Loader").hide();
                                /*if ($scope.BaseData.Reservoir.Last().City == $scope.BaseData.Unit.Unders.Last().UnitName) {
                                    $element.unbind('scroll', scroll);
                                }*/
                                if ($scope.BaseData.Reservoir.length == $scope.BaseData.Unit.Unders.length - 2) {  //排除二连浩特市、满洲里市
                                    $element.unbind('scroll', scroll);
                                }
                            }
                        });
                    }
                }
            };
            $element.bind('scroll', scroll);
        }
    };
}]);

App.directive("notAllow", function () {
    return function ($scope, $element, $attr) {
        $(function () {
            $element.on('keydown', function () {
                if (event.ctrlKey) {
                    if ($attr["notAllow"] == "") {
                        if (event.keyCode == 67 || event.which == 67) {
                            $('.Error p').text('不允许复制');
                            return false;
                        } else if (event.keyCode == 86 || event.which == 86) {
                            $('.Error p').text('不允许粘贴');
                            return false;
                        }
                    }

                    setTimeout(function () {
                        $(".Error p").text("");
                    }, 4000);
                }
            });
        });
    };
});

App.directive("posTop", function () {
    return function($scope, $element, $attr) {
        if ($scope.Attr.SSO > 0) {
            $element.css("top", $attr["posTop"] + "px");
        }
    };
});