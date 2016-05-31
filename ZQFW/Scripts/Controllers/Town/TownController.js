App.service('BaseData', function () {
    return $.extend({
        Unit: {
            Local: {
                Limit: parseInt($.cookie("limit")),
                UnitCode: $.cookie("unitcode"),
                UnitName: $.cookie("unitname"),
                RealName: $.cookie("realname")
            }
        }
    }, window.basedata);
});

App.controller('HeadCtrl', ['$rootScope', '$state', 'BaseData', function ($rootScope, $state, baseData) {
    //-------------------------------------<Public State>---------------------------------------------
    $rootScope.NameSpace = "Head.Menu";
    $rootScope.SysUserCode = $.cookie('unitcode').slice(0, 2);
    $rootScope.BaseData = baseData;
    $rootScope.CurrentUrl = window.location.hash.replace("#", "");
    $rootScope.RedictUrl = function (url) {
        $rootScope.CurrentUrl = url;
        $state.go($rootScope.NameSpace + url.replace("/", "."));
    };
    //$rootScope.BaseData.RelationCheck.Constant.ZRKS = 99999;  //暂时没有查出总人口数
    /*    $rootScope.BaseData.RelationCheck.Formula = [
    { Left: "SZRK", Middle: "<=", Right: "SZRK", Message: "受灾人口应 <= 乡镇总人口（" + $rootScope.BaseData.RelationCheck.Constant.ZRKS + "）" },
    { Left: "SZRK", Middle: ">=", Right: "ZYRK", Message: "受灾人口应 >= 转移人口" },
    /*        { Left: "SZRK", Middle: ">", Right: "SZRKR", Message: "受灾人口应 > 失踪人口" },#1#
    { Left: "SZRKR", Middle: ">", Right: "SWRK", Message: "失踪人口应 > 死亡人口" },
    { Left: "SZRK", Middle: ">", Right: "SZRKR+SWRK", Message: "受灾人口应 > 失踪人口 + 死亡人口" },
    { Left: "ZJJJZSS", Middle: ">=", Right: "SLSSZJJJSS", Message: "直接经济损失应 > 水利设施损失" }
    ];*/
    $rootScope.BaseData.DeathReason.Hide = function (event) {
        var tree = $rootScope.BaseData.DeathReason.zTree.parent()[0];
        if ((event.srcElement ? event.srcElement : event.target) != tree && !$.contains(tree, (event.srcElement ? event.srcElement : event.target))) {
            $(tree).hide();
            $(document.body).unbind("mousedown", $rootScope.BaseData.DeathReason.Hide);
        }
    };
    $rootScope.InitTree = function (tree, simpleData, setting) {
        if (typeof (tree) == "string") {
            tree = $(".ztree#" + tree);
        }

        return $.fn.zTree.init(tree, $.extend({
            data: {
                simpleData: { enable: true }
            },
            view: { selectedMulti: false }
        }, setting), simpleData);
    };
    $rootScope.Fn = {
        GetEvt: function () {
            return $(event.target || event.srcElement);
        },
        Check: {
            Number: function (len) {
                var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                var obj = $scope.Fn.GetEvt().scope()[arr[0].toLowerCase()];
                if (Number(obj[arr[1]]) > 0) {
                    if (obj[arr[1]].length > Number(len)) {
                        obj[arr[1]] = obj[arr[1]].slice(0, len);
                    }
                } else {
                    obj[arr[1]] = undefined;
                }
            },
            Length: function (len) {
                var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                var obj = $scope.Fn.GetEvt().scope();

                if (arr.length > 2) {
                    $.each(arr, function (i) {
                        if (i == arr.length - 1) {
                            return false;
                        } else {
                            obj = obj[arr[i]];
                        }
                    });
                } else {
                    obj = obj[arr[0]];
                }

                if (typeof obj[arr[arr.length - 1]] == "string") {
                    obj[arr[arr.length - 1]] = obj[arr[arr.length - 1]].slice(0, len > 0 ? len : 0);
                }
            },
            Quot: function () {
                try {
                    var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                    var obj = $scope.Fn.GetEvt().scope();

                    if (arr.length > 2) {
                        $.each(arr, function (i) {
                            if (i == arr.length - 1) {
                                return false;
                            } else {
                                obj = obj[arr[i]];
                            }
                        });
                    } else {
                        obj = obj[arr[0]];
                    }

                    if (typeof obj[arr[arr.length - 1]] == "string") {
                        obj[arr[arr.length - 1]] = obj[arr[arr.length - 1]]
                            .replaceAll("\"", "‘").replaceAll("'", "‘")
                            .replaceAll("<", "").replaceAll(">", "")
                            .replaceAll("&", "").replaceAll(" ", ""); ;
                    }
                } catch (e) {
                    throw e;
                }
            }
        },
        Ajax: function (url, data, type, async) {
            var result = null;
            async = async == undefined ? false : async;
            type = type ? type : 'post';
            $.ajax({
                url: url,
                data: data,
                async: async,
                type: type,
                success: function (data) {
                    if (data.indexOf("{") >= 0) {
                        result = eval("(" + data + ")");
                    } else if (!isNaN(data)) {
                        if (data.indexOf(".") > 0) {
                            result = parseFloat(data);
                        } else {
                            result = parseInt(data);
                        }
                    } else {
                        result = data;
                    }
                },
                error: function (xhr) {
                    alert("向" + url + "发送请求出错，状态：" + xhr.status);
                    throw "向" + url + "发送请求出错，状态：" + xhr.status;
                }
            });

            return result;
        },
        UpdatePassword: function () {
            $rootScope.Dialog.Config({
                Title: "修改密码",
                Layout: "Table",
                Content: [
                    { Name: "原密码", Type: "password", Model: "" },
                    { Name: "新密码", Type: "password", Model: "" },
                    { Name: "确认密码", Type: "password", Model: "" }
                ],
                Button: {
                    Left: {
                        Text: "确认修改",
                        CallBack: function () {
                            if ($scope.Dialog.Attr.Content[0].Model == "") {
                                $scope.Dialog.Attr.Message = "原密码不能为空";
                            } else if ($scope.Dialog.Attr.Content[1].Model == "") {
                                $scope.Dialog.Attr.Message = "新密码不能为空";
                            } else if ($scope.Dialog.Attr.Content[2].Model == "") {
                                $scope.Dialog.Attr.Message = "确认密码不能为空";
                            } else if ($scope.Dialog.Attr.Content[1].Model != $scope.Dialog.Attr.Content[2].Model) {
                                $scope.Dialog.Attr.Message = "两次输入的密码不一致";
                            } else if ($scope.Dialog.Attr.Content[1].Model == $scope.Dialog.Attr.Content[0].Model) {
                                $scope.Dialog.Attr.Message = "新密码不能与原密码相同";
                            } else {
                                var result = $scope.Fn.Ajax("Head/ModifyPwd", {
                                    oldPwd: $scope.Dialog.Attr.Content[0].Model,
                                    newPwd: $scope.Dialog.Attr.Content[1].Model
                                });

                                if (result == 0) {
                                    $scope.Dialog.Attr.Message = "原密码错误";
                                } else if (result === 1) {
                                    $scope.Dialog.Attr.Show = false;
                                    Alert("密码修改成功！");
                                } else {
                                    throw result;
                                }
                            }
                        }
                    },
                    Right: {
                        Text: "取消",
                        CallBack: function () {
                            $scope.Dialog.Attr.Message = "";
                            $.each($scope.Dialog.Attr.Content, function () {
                                this.Model = "";
                            });
                            $scope.Dialog.Attr.Show = false;
                        }
                    }
                }
            });
        }
    };
    //-------------------------------------</Public State>---------------------------------------------
    //-------------------------------------<New State>-------------------------------------------------
    $rootScope.New = {
        Attr: {
            Page: {
                Index: 0,
                Count: 22
            },
            Readonly: false,
            SearchStartTime: App.Tools.Date.GetOneDay(-7),
            SearchEndTime: App.Tools.Date.GetToday(),
            IsFirstTimeLoad: true,
            CheckedAll: false
        },
        Report: {
            List: [],
            Content: [],
            Current: undefined,
            Fn: {
                Check: function (param) {  //0:保存时校核，1:数字校核
                    var obj = undefined;
                    var $target = $scope.Fn.GetEvt();
                    $scope.Fn.GetEvt().data('target', $target);

                    if (typeof (param) == "number") {
                        obj = $scope.New.Report.Current.HL011[0];
                        var field = $target.attr("ng-model").substr($target.attr("ng-model").lastIndexOf(".") + 1);
                        obj[field] = Number(obj[field]) > 0 ? parseFloat(obj[field]).toFixed(param) : undefined;
                        obj[field] = Number(obj[field]) > 0 ? obj[field] : undefined;
                    } else {
                        /*/*var passTheCheck = true;

                        angular.forEach($scope.New.Report.Current.Attr.Message, function (field) {
                        obj = baseData.RelationCheck.Constant[baseData.Unit.Local.UnitCode + "-" + field];

                        if (obj && Number($scope.New.Report.Current.HL011[0][field]) > Number(obj.value)) {
                        $scope.New.Report.Current.Attr.Message[field] = obj.name + "不能大于" + obj.value;
                        passTheCheck = false;
                        } else {
                        $scope.New.Report.Current.Attr.Message[field] = undefined;
                        }
                        });

                        var left = undefined;
                        var right = undefined;
                        var check = true;
                        obj = $scope.New.Report.Current.HL011[0];
                        param = param ? param : event.$target.attr("ng-model").substr(event.$target.attr("ng-model").lastIndexOf(".") + 1);
                        $.each($rootScope.BaseData.RelationCheck.Formula, function () {

                        if (this.Left == param) {
                        left = Number(obj[this.Left]);
                        left = left > 0 ? left : 0;

                        if (this.Right == "SZRK") {
                        if ($scope.BaseData.RelationCheck.Constant.SZRK) {
                        right = Number($scope.BaseData.RelationCheck.Constant.SZRK);
                        } else {
                        return true;  //没有设置总人口数时忽略
                        }
                        } else if (this.Right.indexOf("+") > 0) {
                        right = this.Right.split("+");
                        right[0] = Number(obj[right[0]]);
                        right[0] = right[0] > 0 ? right[0] : 0;
                        right[1] = Number(obj[right[1]]);
                        right[1] = right[1] > 0 ? right[1] : 0;
                        right = obj[right[0]] + obj[right[1]];
                        } else {
                        right = Number(obj[this.Right]);
                        right = right > 0 ? right : 0;
                        }

                        switch (this.Middle) {
                        case "<=":
                        if (left > right) {
                        passTheCheck = false;
                        }
                        break;
                        case ">":
                        if (left <= right) {
                        passTheCheck = false;
                        check = false;
                        }
                        break;
                        case ">=":
                        if (left < right) {
                        passTheCheck = false;
                        check = false;
                        }
                        break;
                        }

                        if (check) {
                        $scope.New.Report.Current.Attr.Message[this.Left] = undefined;
                        } else {
                        $scope.New.Report.Current.Attr.Message[this.Left] = this.Message;
                        }
                        }#1#

                        //});

                        //$scope.New.Report.Current.Attr.HasCheckErrors = !passTheCheck;*/
                    }
                },
                Create: function () {
                    var report = App.Models.Report();

                    $.extend(report.ReportTitle, {
                        StartDateTime: App.Tools.Date.GetToday(),
                        EndDateTime: App.Tools.Date.GetToday(),
                        PageNO: 0,
                        State: 0
                    });

                    report.Attr = {
                        Message: App.Models.HL011()
                    };

                    $scope.New.Report.Current = report;
                    $scope.New.Attr.Readonly = false;

                    return this;
                },
                DeadPeople: {
                    Add: function () {
                        $scope.New.Report.Current.HL012.push($.extend(App.Models.HL012(), {
                            SWXM: "未知",
                            DataType: "死亡",
                            SWXB: "未知",
                            SWNL: undefined,
                            SWHJ: $.cookie("fullname"),
                            SWDD: $.cookie("fullname"),
                            SWSJ: App.Tools.Date.GetToday(),
                            DeathReason: "降雨-其它",
                            DeathReasonCode: "JY10"
                        }));

                        $scope.New.Report.Fn.OnChange.DataType();
                    },
                    Del: function () {
                        if ($scope.New.Report.Current.HL012.SomeBy("Checked", true)) {
                            if (confirm("确认删除么？")) {
                                $scope.New.Report.Current.HL012 = $scope.New.Report.Current.HL012.RemoveBy("Checked", true);
                                $scope.New.Attr.CheckedAll = false;
                                $scope.New.Report.Fn.OnChange.DataType();
                            }
                        } else {
                            Alert("未选择死亡人员信息");
                        }
                    },
                    Hide: function () {
                        $(".Shadow").stop(true).animate({ width: "0px" }, "normal");
                        $(".OpenLayer").stop(true).animate({ width: "0px" }, "fast");
                    },
                    SelectAll: function () {
                        $scope.New.Report.Current.HL012.Every({ Checked: (event.srcElement ? event.srcElement : event.target).checked });
                    },
                    Show: function () {
                        if ($(window).width() < 900) {
                            $(".Shadow").stop(true).animate({ width: ($(window).width() * 0.4) + "px" }, "normal");
                            $(".OpenLayer").stop(true).animate({ width: "900px" }, "fast");
                        } else {
                            $(".Shadow").stop(true).animate({ width: ($(window).width() * 0.4) + "px" }, "normal");
                            $(".OpenLayer").stop(true).animate({ width: ($(window).width() * 0.60) + "px" }, "fast");
                        }
                    }
                },
                Delete: {
                    Report: function () {
                        if (confirm("确认删除么？")) {
                            var result = $scope.Fn.Ajax("Town/DeleteReport", {
                                pageno: $scope.New.Report.Current.ReportTitle.PageNO
                            });

                            if (result > 0) {
                                result = $scope.New.Report.Current.ReportTitle.PageNO;
                                $scope.New.Report.Content = $scope.New.Report.Content.RemoveBy("PageNO", result);
                                $scope.New.Report.List = $scope.New.Report.List.RemoveBy("PageNO", result);
                                $scope.New.Report.Current = undefined;
                                $scope.New.Report.Fn.Create();
                                Alert("删除成功！");
                            } else {
                                alert(result);
                                throw result;
                            }
                        }
                    },
                    Affix: function (index, tbno, url) {
                        if (confirm("确认删除么？")) {
                            $scope.New.Report.Current.DeletedAffix.push({
                                TBNO: tbno,
                                DownloadURL: url
                            });
                            $scope.New.Report.Current.Affix.splice(index, 1);
                        }
                    }
                },
                Jumper: function () {

                },
                Modify: function () {
                    $scope.New.Attr.Readonly = false;
                },
                OnChange: {
                    DataType: function () {
                        var swrk = 0;
                        var szrkr = 0;

                        $.each($scope.New.Report.Current.HL012, function () {
                            if (this.DataType == "死亡") {
                                swrk++;
                            } else {
                                szrkr++;
                            }
                        });

                        swrk = swrk ? swrk : undefined;
                        szrkr = szrkr ? szrkr : undefined;

                        $scope.New.Report.Current.HL011[0].SWRK = swrk;
                        $scope.New.Report.Current.HL011[0].SZRKR = szrkr;
                        //$scope.New.Report.Fn.Check('SZRKR');
                    }
                },
                Open: function (pageno) {
                    var report = $scope.New.Report.Content.Find("ReportTitle.PageNO", pageno);

                    if ($.isEmptyObject(report)) { //不在缓存里
                        report = $scope.Fn.Ajax("Town/OpenReport", { pageno: pageno });
                        report = {
                            ReportTitle: {
                                StartDateTime: new Date(report.ReportTitle.StartDateTime).Format("yyyy-MM-dd"),
                                EndDateTime: new Date(report.ReportTitle.EndDateTime).Format("yyyy-MM-dd"),
                                State: Number(report.ReportTitle.State),
                                PageNO: Number(report.ReportTitle.PageNO),
                                Remark: report.ReportTitle.Remark
                            },
                            HL011: report.HL011,
                            HL012: report.HL012,
                            Affix: report.Affix,
                            Attr: {
                                Message: App.Models.HL011()
                            },
                            DeletedAffix: []
                        };
                        $scope.New.Report.Content.push(report);
                    }

                    $scope.New.Report.Current = report;
                    $scope.New.Attr.Readonly = true;
                },
                Save: function () {
                    if (!$scope.New.Report.Current.Attr.HasCheckErrors) {
                        var report = angular.copy($scope.New.Report.Current);
                        delete report.Attr;

                        if (report.HL011[0].isEmpty()) {
                            report.HL011 = [];
                        }

                        var result = $scope.Fn.Ajax("Town/SaveOrUpdateRpt", {
                            report: angular.toJson(report)
                        });

                        if (isNaN(result)) {
                            alert(result);
                            throw result;
                        } else {
                            if ($scope.New.Report.Current.ReportTitle.PageNO == 0) {
                                $scope.New.Report.Current.ReportTitle.PageNO = result;
                                $scope.New.Report.Content.push($scope.New.Report.Current);
                                $scope.New.Report.List.InsertAt(0, {
                                    StartDateTime: $scope.New.Report.Current.ReportTitle.StartDateTime,
                                    EndDateTime: $scope.New.Report.Current.ReportTitle.EndDateTime,
                                    State: 0,
                                    PageNO: result
                                });
                            } else {
                                var reporttitle = $scope.New.Report.List.Find('PageNO', $scope.New.Report.Current.ReportTitle.PageNO);
                                reporttitle.StartDateTime = $scope.New.Report.Current.ReportTitle.StartDateTime;
                                reporttitle.EndDateTime = $scope.New.Report.Current.ReportTitle.EndDateTime;
                            }

                            if ($(".File #file_upload-queue").children().length > 0) {  //存在附件
                                var uploadify = $('#file_upload');
                                uploadify.uploadify("settings", "formData", {
                                    'PageNo': result,
                                    'limit': baseData.Unit.Local.Limit,
                                    'unitcode': baseData.Unit.Local.UnitCode,
                                    'rptType': 'HL01'
                                });
                                uploadify.uploadify('upload', '*');
                            }

                            $scope.New.Attr.Readonly = true;
                            Alert("保存成功！");
                        }
                    } else {
                        Alert("报表未通过校核，请修改后再保存");
                    }
                },
                Search: function () {
                    $scope.New.Report.List = $scope.Fn.Ajax("Town/SerachReportList", {
                        startDate: $scope.New.Attr.SearchStartTime,
                        endDate: $scope.New.Attr.SearchEndTime
                    }).ReportList;

                    $scope.New.Report.Content.splice(0);  //重新搜索后，清空缓存
                    if ($scope.New.Report.Current && $scope.New.Report.Current.ReportTitle.PageNO != 0) {
                        $scope.New.Report.Content.push($scope.New.Report.Current);
                    }

                    return this;
                },
                Send: function () {
                    if (confirm("确认报送？")) {
                        var result = $scope.Fn.Ajax("Town/SendReport", { pageno: $scope.New.Report.Current.ReportTitle.PageNO });

                        if (result > 0) {
                            $scope.New.Report.Current.ReportTitle.State = 3;
                            $scope.New.Report.List.Find("PageNO", $scope.New.Report.Current.ReportTitle.PageNO).State = 3;
                            $scope.New.Attr.Readonly = true;
                            Alert("报送成功！");
                        } else {
                            Alert("上报出错");
                            throw result;
                        }
                    }
                }
            }
        }
    };

    /*$rootScope.$watch('New.Attr.Readonly', function (to) {
    if (to) {
    //$("#file_upload").hide(); //.uploadify('disable', true);
    } else {
    //$("#file_upload").show(); //.uploadify('disable', false);
    }
    });*/
    $rootScope.NRFC = $rootScope.New.Report.Fn.Check;
    $rootScope.FCL = $rootScope.Fn.Check.Length;
    $rootScope.FCN = function () {
        var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
        var obj = $scope.Fn.GetEvt().scope()[arr[0].toLowerCase()];
        obj[arr[1]] = Number(obj[arr[1]]);
        obj[arr[1]] = obj[arr[1]] > 0 ? parseInt(obj[arr[1]]) : undefined;
    };
    //-------------------------------------</New State>-------------------------------------------------
    window.$scope = $rootScope;

} ]);

App.controller('MenuCtrl', ['$rootScope', function ($rootScope) {
    $rootScope.Dialog = {
        Config: function (config) {
            this.Attr = $.extend({
                Show: true,
                Title: undefined,
                Layout: undefined,
                Message: undefined,
                Content: undefined,
                Input: {
                    Width: "inherit",
                    OnChange: angular.noop
                },
                Button: {
                    Left: {
                        Text: undefined,
                        CallBack: undefined
                    },
                    Right: {
                        Text: undefined,
                        CallBack: undefined
                    }
                }
            }, config);
        }
    };
}]);

App.controller('TownNewCtrl', ['$scope', function ($scope) {

    if ($scope.New.Attr.IsFirstTimeLoad) {
        $scope.New.Report.Fn.Create().Search();
        $scope.New.Attr.IsFirstTimeLoad = false;
    }
    
}]);

App.controller('TownSecretaryCtrl', ['$scope', function() {

}]);

App.directive('stretch', function () {
    return function ($scope, $element, $attr) {
        $(function () {
            if ($element.is('div')) {
                $(document).click(function () {
                    if ($scope.Fn.GetEvt().is("." + $attr["class"]) || $.contains($element[0], $scope.Fn.GetEvt()[0])) { //点击的是自己或子级DOM
                        $element.addClass("Stretch-On");
                        $element.animate({ 'height': $attr['stretch'].split("-")[1] + 'px' }, 200);
                    } else {
                        $element.css({ 'height': $attr['stretch'].split("-")[0] + 'px' });
                        $element.removeClass("Stretch-On");
                    }
                });
            } else {
                $element.focus(function () {
                    $scope.Fn.GetEvt().addClass("Stretch-On");
                    $scope.Fn.GetEvt().animate({ 'height': $attr['stretch'].split("-")[1] + 'px' }, 200);
                });
                $element.blur(function () {
                    $scope.Fn.GetEvt().css({ 'height': $attr['stretch'].split("-")[0] + 'px' });
                    $scope.Fn.GetEvt().removeClass("Stretch-On");
                });
            }
        });
    };
});

Object.defineProperty(Object.prototype, 'isEmpty', {
    value: function () {
        var isEmpty = true;
        for (var key in this) {
            if (this[key] != undefined) {
                isEmpty = false;
            }
        }

        return isEmpty;
    },
    writable: true
});
