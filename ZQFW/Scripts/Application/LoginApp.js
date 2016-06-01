Array.prototype.In_Array = function (e) {  //载入Array.js时此段代码应删除
    var s = String.fromCharCode(2);
    var r = new RegExp(s + e + s);

    return (r.test(s + this.join(s) + s));
};

window.App = angular.module('Login', []).run(function ($http) {
    $http.defaults.responseType = 'json';
    $http.defaults.method = 'get';
    $http.defaults.cache = true;  //启用数据数据缓存

    $(function () {
        $(document).keydown(function() {
            if (event.keyCode == 13 || event.which == 13) {
                $scope.Fn.Login.In();
            }
        });
    });
});

App.factory('Animate', function () {

    return {
        Element: {
            Self: $('.M-Top')
        },
        Shake: function () {
            if (!this.Element.Self.is(":animated")) {
                this.Element.Right = parseInt(this.Element.Self.css('right').replace(/px/, ''));
                this.Element.Self.animate({ 'right': this.Element.Right - 10 + 'px' }, 100)
                .animate({ 'right': this.Element.Right + 9 + 'px' }, 80)
                .animate({ 'right': this.Element.Right - 9 + 'px' }, 50)
                .animate({ 'right': this.Element.Right + 8 + 'px' }, 40)
                .animate({ 'right': this.Element.Right - 8 + 'px' }, 30)
                .animate({ 'right': this.Element.Right + 'px' }, 20);
            }
        },
        Slide: function (position) {
            if (!this.Element.Self.is(":animated")) {
                this.Element.Right = parseInt(this.Element.Self.css('right').replace(/px/, ''));  //y轴的位置可能变动，需重新获取
                this.Element.Self.animate({ 'right': this.Element.Right + (position == "Left" ? 350 : -350) + 'px' }, 800);
            }
        }
    };
});

App.directive('toggle', ['Animate', function (animate) {
    return function ($scope, $element) {
        $(function () {
            $element.click(function () {
                $(this).toggleClass("Quickly");
                if ($(this).hasClass("Quickly")) {
                    animate.Slide("Left");
                    $scope.Attr.Login.Quick.Enable = true;

                    if (!$scope.Attr.Login.Quick.City.Left) {
                        var arr = $scope.Attr.Login.History.FullName.split("-");
                        $scope.Attr.Login.Quick.City.Left = arr[1] || undefined;
                        $scope.Attr.Login.Quick.County.Left = arr[2] || undefined;
                        $scope.Attr.Login.Quick.Town.Left = arr[3] || undefined;
                    }
                } else {
                    animate.Slide("Right");
                    $scope.Attr.Login.Quick.Enable = false;
                }
                $scope.$apply();
            });
        });
    };
} ]);

App.directive('disableKey', function () {
    return function (scope, element, attr) {
        var keycode = undefined;
        switch (attr["disableKey"]) {
            case "space":
                keycode = 32;
                break;
            default:
                keycode = 13;
        }
        element.keydown(function () {
            if (event.keyCode == keycode || event.which == keycode) {
                return false;
            }
        });
    }
});


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

App.controller('Main', ['$scope', '$http', 'Animate', function ($scope, $http, animate) {
    $scope.Attr = {
        District: {
            Province: {
                Left: {  //浙江省 33000000  湖南省 43000000 吉林省 22000000 黑龙江省 23000000  内蒙古 15000000  江西省 36000000 广西 45000000 福建省 35000000
                    UnitName: "福建省",
                    UnitCode: "35000000"
                }
            },
            City: {
                Left: {
                    Select: [{ UnitName: '———————', UnitCode: ''}],
                    Value: {
                        UnitName: "———————",
                        UnitCode: ""
                    }
                }
            },
            County: {
                Left: {
                    Select: [{ UnitName: '———————', UnitCode: ''}],
                    Value: {
                        UnitName: "———————",
                        UnitCode: ""
                    }
                }
            },
            Town: {
                Left: {
                    Select: [{ UnitName: '———————', UnitCode: ''}],
                    Value: {
                        UnitName: "———————",
                        UnitCode: ""
                    }
                }
            }
        },
        Login: {
            User: {
                Password: undefined,
                UnitName: undefined,
                FullName: undefined,
                UnitCode: undefined,
                Limit: undefined
            },
            History: {
                UnitName: $.cookie("unitname"),
                FullName: $.cookie("fullname"),
                UnitCode: $.cookie("unitcode"),
                Limit: $.cookie("limit")
            },
            Quick: {
                City: {
                    Left: undefined
                },
                County: {
                    Left: undefined
                },
                Town: {
                    Left: undefined
                },
                Show: $.cookie("fullname") == null ? false : true,
                Enable: false   //默认“正常登录”
            }
        }
    };
    $scope.Attr.UserCode = $scope.Attr.District.Province.Left.UnitCode.slice(0, 2);
    /*if ($scope.Attr.UserCode == "15") {
        $scope.Attr.CurrentTab = "HL";
    } else {
        $scope.Attr.CurrentTab = "Normal";
    }*/
    $scope.Attr.CurrentTab = "HL";

    $scope.Fn = {
        Clear: {
            Select: function (name) {
                var clear = function (param) {
                    param = param.split(".");
                    $scope.Attr.District[param[0]][param[1]].Value = {
                        UnitName: '———————',
                        UnitCode: ''
                    };
                    $scope.Attr.District[param[0]][param[1]].Select = [angular.copy($scope.Attr.District[param[0]][param[1]].Value)]; //删除引用
                };

                switch (name) {
                    case "City.Left":
                        clear("City.Left");
                    case "County.Left":
                        clear("County.Left");
                    case "Town.Left":
                        clear("Town.Left");
                }
            }
        },
        GetEvt: function () {
            return $(event.target || event.srcElement);
        },
        OnChange: {
            Select: function (name) {
                name = name || $scope.Fn.GetEvt().attr("name");
                var arr = name.split(".");
                var obj = $scope.Attr.District[arr[0]][arr[1]];
                obj.Value.UnitName = obj.Select.Find("UnitCode", obj.Value.UnitCode, "UnitName");

                switch (name) {
                    case "City.Left":
                        $scope.Fn.Clear.Select("County.Left"); //清空县级、乡镇的Select

                        if ($scope.Attr.District[arr[0]][arr[1]].Value.UnitName != "———————") {
                            $http.get('/Login/Read?unitcode=' + $scope.Attr.District.City.Left.Value.UnitCode)
                                .success(function (data) { //加载县级左边
                                    $scope.Attr.District.County.Left.Select = $scope.Attr.District.County.Left.Select.concat(data);
                                });
                        }
                        break;
                    case "County.Left":
                        $scope.Fn.Clear.Select("Town.Left"); //清空乡镇

                        if ($scope.Attr.District[arr[0]][arr[1]].Value.UnitName != "———————") {
                            $http.get('/Login/Read?unitcode=' + $scope.Attr.District.County.Left.Value.UnitCode)
                                .success(function (data) { //加载乡镇
                                    $scope.Attr.District.Town.Left.Select = $scope.Attr.District.Town.Left.Select.concat(data);
                                });
                        }
                        break;
                }
            }
        },
        Login: {
            In: function () {
                if ($scope.Attr.Login.Quick.Enable) {
                    this.Quick();
                } else {
                    this.Normal();
                }
            },
            Quick: function () {
                angular.extend($scope.Attr.Login.User, $scope.Attr.Login.History);
                this.Validate();
            },
            Normal: function () {
                var tmp = undefined;
                var fn = function (name, units) {
                    name = name.split(".");
                    tmp = App.Tools.Unit.Info({
                        code: $scope.Attr.District[name[0]][name[1]].Value.UnitCode,
                        rootName: $scope.Attr.District.Province.Left.UnitName,
                        units: units
                    });
                    $scope.Attr.Login.User.Limit = tmp.Limit;
                    $scope.Attr.Login.User.UnitCode = $scope.Attr.District[name[0]][name[1]].Value.UnitCode;
                    $scope.Attr.Login.User.UnitName = $scope.Attr.District[name[0]][name[1]].Value.UnitName;
                    $scope.Attr.Login.User.FullName = tmp.FullName;
                };

                if ($scope.Attr.District.City.Left.Value.UnitName == "———————") { //市级未选择，省级登录
                    $scope.Attr.Login.User.Limit = 2;
                    $scope.Attr.Login.User.UnitCode = $scope.Attr.District.Province.Left.UnitCode;
                    $scope.Attr.Login.User.UnitName = $scope.Attr.District.Province.Left.UnitName;
                    $scope.Attr.Login.User.FullName = $scope.Attr.District.Province.Left.UnitName;
                } else if ($scope.Attr.District.County.Left.Value.UnitName == "———————") { //市级已选择，县级未选择，市级登录
                    fn("City.Left", $scope.Attr.District.City.Left.Select);
                } else if ($scope.Attr.District.Town.Left.Value.UnitName == "———————") { //县级已选择，乡镇未选择，县级登录
                    tmp = $scope.Attr.District;
                    fn("County.Left", tmp.City.Left.Select.concat(tmp.County.Left.Select));
                } else { //乡镇已选择，乡镇级登录
                    tmp = $scope.Attr.District;
                    fn("Town.Left", tmp.City.Left.Select.concat(tmp.County.Left.Select).concat(tmp.Town.Left.Select));
                }

                this.Validate();
            },
            Validate: function () {
                if (navigator.userAgent.indexOf('MSIE') >= 0 && navigator.userAgent.indexOf('Opera') < 0) {
                    alert("本系统不支持该浏览器，无法登录，请在该界面下方选择并下载安装所支持的浏览器");
                    return false;
                }

                if ($scope.Attr.Login.User.Password == undefined || $scope.Attr.Login.User.Password == null || $scope.Attr.Login.User.Password.trim() == "") {
                    $(".Error p").html("密码不能为空");
                    animate.Shake(); //抖动提示
                } else {
                    $.ajax({
                        url: '/Login/Validate',
                        type: 'post',
                        data: {
                            limit: $scope.Attr.Login.User.Limit,
                            unitcode: $scope.Attr.Login.User.UnitCode,
                            password: $scope.Attr.Login.User.Password,
                            type: $scope.Attr.CurrentTab,
                            fresh: Math.random()   //解决浏览器的缓存问题
                        },
                        beforeSend: function () {
                            $('#Login').addClass('Logining'); //将按钮显示为登录中
                        },
                        complete: function () {
                            $('#Login').removeClass('Logining'); //移除按钮的登录中状态
                        },
                        success: function (data) {
                            if (data.indexOf("Error:") == 0) {
                                alert(data);
                                throw data;
                            } else if (Number(data) == 0) {
                                $(".Error p").html("密码错误");
                                animate.Shake();
                            } else if (Number(data) > 0) {
                                $.cookie("unitcode", $scope.Attr.Login.User.UnitCode);
                                $.cookie("unitname", $scope.Attr.Login.User.UnitName);
                                $.cookie("fullname", $scope.Attr.Login.User.FullName);
                                $.cookie("limit", $scope.Attr.Login.User.Limit);

                                if ($scope.Attr.CurrentTab == "HL") {
                                    var fn = function() {
                                        var url = "";
                                        if ($scope.Attr.District.Province.Left.UnitCode == "33000000") {
                                            url = "/main";
                                        } else {
                                            switch (Number($scope.Attr.Login.User.Limit)) {
                                            case 5:
                                                url = "/town";
                                                break;
                                            default:
                                                url = "/main";
                                                break;
                                            }
                                        }

                                        if (window.location.href.indexOf("?debug=1") > 0) {
                                            url += "?debug=1";
                                        }

                                        window.location.href = url;
                                    };

                                    if (Number(data) > 1) {
                                        fn(); //Alert("注意：当前账户已被多人使用<br>（4秒后登录......）", 4000, true, fn); 湖南
                                    } else {
                                        fn();
                                    }
                                } else if ($scope.Attr.CurrentTab == "SH") {
                                    //var url = Number($scope.Attr.Login.User.Limit) > 2 ? "/index/sh" : "/main/sh";

                                    var url = "/sh/main";
                                    if (window.location.href.indexOf("?debug=1") > 0) {
                                        url += "?debug=1";
                                    }
                                    $.cookie("ord_code", "SH01");
                                    window.location.href = url;
                                }
                            } else {
                                throw data;
                            }
                        },
                        error: function (xhr, status) {
                            $scope.Attr.Login.User.Message = "网络超时，状态：" + status;
                            animate.Shake();
                        }
                    });
                }
            }
        }
    };

    $scope.Fn.Clear.Select("City.Left");
    $http.get('/Login/Read?unitcode=' + $scope.Attr.District.Province.Left.UnitCode)
        .success(function (data) { //加载县级左边
            $scope.Attr.District.City.Left.Select = $scope.Attr.District.City.Left.Select.concat(data);
        });
    window.$scope = $scope;
} ]);
