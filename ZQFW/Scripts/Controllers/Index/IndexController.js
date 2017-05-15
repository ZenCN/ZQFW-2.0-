App.controller('HeadCtrl', ['$rootScope', '$state', '$timeout', '$http' ,'BaseData' , 'ngCss', 'screen', 'loading',
    function($rootScope, $state, $timeout, $http, baseData, ngCss, screen, loading) {
    //-------------------------------------<Public State>---------------------------------------------
    $rootScope.NameSpace = "Head.Menu";
    $rootScope.Authority = parseInt($.cookie("authority"));
    $rootScope.SysUserCode = baseData.Unit.Local.UnitCode.slice(0, 2);
    $rootScope.SysORD_Code = $.cookie('ord_code');
    if ($rootScope.SysORD_Code == "NP01") {
        $.getScript('Scripts/Library/Plugins/highchart/chart_export_3d.min.js');
    }
    if ($rootScope.SysORD_Code == "HL01") {
        baseData.zTree.RptClass = [baseData.zTree.RptClass.Find("name", "%洪涝%")];
    } else {
        baseData.zTree.RptClass = [baseData.zTree.RptClass.Find("name", "%蓄水%")];
    }
    baseData.Select.RptClass = [baseData.Select.RptClass.Find('code', $rootScope.SysORD_Code)];
    $rootScope.BaseData = baseData;
    if ($rootScope.SysUserCode == "43") {
        $rootScope.BaseData.Reservoir.QNTQDXS = {}; //蓄水“历年同期”字段的值
    }
    $rootScope.Params = {};
    $rootScope.Config = {
        Page: {  //Hide方式(Hide=Disable)
            New: {},
            Open: {
                Func: {
                    TreeBoxes: {
                        Cased: (["15", "22", "23", "33","35", "36", "45", "51"].In_Array($rootScope.SysUserCode) && $rootScope.BaseData.Unit.Local.Limit == 4) || $rootScope.SysORD_Code == "NP01"
                         || $rootScope.BaseData.Unit.Local.Limit == 4 && $rootScope.SysORD_Code == "HP01"
                    }
                }
            },
            Receive: {},
            RecycleBin: {
                Func: {}
            },
            UrgeReport: {}
        }
    };
    $rootScope.Config.Page.Receive.Hide = $rootScope.Config.Page.Open.Func.TreeBoxes.Cased;
    $rootScope.Config.Page.RecycleBin.Func.Inferior = $rootScope.Config.Page.Open.Func.TreeBoxes.Cased;
    $rootScope.Config.Page.UrgeReport.Hide = $rootScope.Config.Page.Open.Func.TreeBoxes.Cased;
    $rootScope.InitTree = function (tree, simpleData, setting) {
        if (typeof (tree) == "string") {
            tree = $(".ztree#" + tree);
        }

        return $.fn.zTree.init(tree, $.extend({
            data: {
                simpleData: { enable: true }
            },
            view: {
                selectedMulti: false,
                fontCss: function(treeId, treeNode) {
                    return treeNode.is_used ? { color: "rgb(150, 149, 149)" } : {};
                }
            }
        }, setting), simpleData);
    };
    $rootScope.Assist = {
        CN_Name: App.Tools.CN_Name
    };
    $rootScope.Attr = {
        SSO: parseInt($.cookie('sso')),
        CurrentYear: new Date().getFullYear(),
        RootName: "Index",
        NameSpace: window.location.hash.replace("#/", ""),
        Preview: {
            Content: []
        }
    };
    $rootScope.Fn = {
        GetEvt: function() {
            return $(event.target || event.srcElement);
        },
        isNaN:isNaN,
        Check: {
            Number: function (len, fixed) {
                var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                var obj = $scope.Fn.GetEvt().scope()[arr[0].toLowerCase()];
                fixed = fixed ? fixed : 0;
                len = len ? len : 0;
                if (Number(obj[arr[1]]) > 0) {
                    var nums = obj[arr[1]].split(".");
                    if (len && nums[0].length > len) { //值太大，视为无效
                        obj[arr[1]] = undefined;
                        return;
                    }

                    obj[arr[1]] = parseFloat(obj[arr[1]]).toFixed(fixed);
                    if (len > 0) {
                        if (nums[0].length > Number(len)) {
                            nums[0] = nums[0].slice(0, len);
                            obj[arr[1]] = nums.join(".");
                        }
                    }
                } else {
                    if ($scope.SysORD_Code == 'NP01' && obj[arr[1]] == 0) {
                        return;
                    }
                    obj[arr[1]] = undefined;
                }
            },
            Length: function(len) {
                var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                var obj = $scope.Fn.GetEvt().scope();

                if (arr.length > 2) {
                    $.each(arr, function(i) {
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
            Quot: function() {
                try {
                    var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                    var obj = $scope.Fn.GetEvt().scope();

                    if (arr.length > 2) {
                        $.each(arr, function(i) {
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
                            .replaceAll("&", "").replaceAll(" ", "");
                    }
                } catch(e) {
                    throw e;
                } 
            },
            Enter: function() {
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
                        obj[arr[arr.length - 1]] = obj[arr[arr.length - 1]].replaceAll('[\n|\r]', '');
                    }
                } catch (e) {
                    throw e;
                }
            }
        },
        Ajax: function(url, param, type, callback) {
            var result = null;
            var async = callback == undefined ? false : ($.isFunction(callback) ? true : callback);
            param = param ? param : undefined;
            type = type ? type : 'post';
            $.ajax({
                url: url,
                data: param,
                async: async,
                type: type,
                success: function(data) {
                    if (data.indexOf("{") >= 0) {
                        data = data.replaceAll("[\r|\n]", "");
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

                    if ($.isFunction(callback)) {
                        callback(result);
                    }
                },
                error: function(xhr) {
                    alert("向" + url + "发送请求出错，状态：" + xhr.status);
                    throw "向" + url + "发送请求出错，状态：" + xhr.status;
                }
            });

            return result;
        },
        ViewUnderRpt: function(obj) {
            obj.queryUnderUnits = obj.queryUnderUnits ? 1 : 0;
            var url = "Index/ViewUnderReport?rptType=" + obj.ORD_Code + "&sourceType=" + obj.SourceType + "&level=" + obj.Limit + "&unitcode="
             + obj.UnitCode + "&pageno=" + obj.PageNO + "&queryUnderUnits=" + obj.queryUnderUnits;

            if (obj.isRiverRpt) {
                url += "&RiverRpt=true&RiverCode=" + obj.RiverCode;
            }

            if (window.location.href.toLowerCase().indexOf("debug=1") > 0) {
                url += "&debug=1";
            }

            window.open(url, "", "_blank", "");
        },
        ConvertToFloat: function(val) {
            if (isNaN(val)) {
                return 0;
            } else if (parseFloat(val) == 0) {
                return 0;
            } else {
                return parseFloat(val);
            }
        },
        Division: function(number1, number2) {
            number1 = parseFloat(number1);
            number2 = parseFloat(number2);
            if (number2 == 0) {
                return 0;
            } else {
                return number1 / number2;
            }
        },
        UpdatePassword: function() {
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
                        CallBack: function() {
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
                        CallBack: function() {
                            $scope.Dialog.Attr.Message = "";
                            $.each($scope.Dialog.Attr.Content, function() {
                                this.Model = "";
                            });
                            $scope.Dialog.Attr.Show = false;
                        }
                    }
                }
            });
        },
        OnChange: {
            CycType: function(to, obj) {
                var date = new Date();
                switch (Number(to)) {
                case 3:
                    obj.StartDateTime = App.Tools.Date.GetDay("First");
                    obj.EndDateTime = App.Tools.Date.GetDay("Last");
                    break;
                case 5:
                    obj.StartDateTime = date.getFullYear() + "-01-01";
                    obj.EndDateTime = date.getFullYear() + "-12-31";
                    break;
                case 7:
                    obj.StartDateTime = date.getFullYear() + "-01-01";
                    obj.EndDateTime = date.getFullYear() + "-09-30";
                    break;
                default:
                    obj.StartDateTime = App.Tools.Date.GetToday();
                    obj.EndDateTime = App.Tools.Date.GetToday();
                    break;
                }
            }
        },
        OnInit: function() {
            var arr = [];
            angular.forEach($rootScope.BaseData.HPDate, function(time) {
                arr.push({
                    RealTime: time,
                    FakeTime: time.split("-")[1]
                });
            });
            $rootScope.BaseData.HPDate = arr;

            if ($rootScope.SysUserCode == "15" && $rootScope.BaseData.Reservoir.length == 0 && $rootScope.SysORD_Code == 'NP01') {
                /*$rootScope.BaseData.zTree.RptClass = $.grep($rootScope.BaseData.zTree.RptClass, function(obj) {
                    return $.isEmptyObject(obj.children.Find("id", "NP01"));
                });
                $rootScope.BaseData.Select.RptClass = $rootScope.BaseData.Select.RptClass.RemoveBy("code", "NP01");*/

                easyDialog.open({
                    container: {
                        content: '该单位没有水库，不需要录入数据'
                    },
                    autoClose: 3000,
                    callback: function() {
                        window.location.href = "/Main";
                    }
                });
            }
        },
        TabSearch: {
            CheckAll: function() {
                $.each($scope[$scope.Attr.NameSpace].Box[$scope[$scope.Attr.NameSpace].Box.Current].Fake, function() {
                    this.Checked = (event.srcElement?event.srcElement:event.target).checked;
                });
            },
            ChangeCondition: function() {
                if ($scope[$scope.Attr.NameSpace].Box[$scope[$scope.Attr.NameSpace].Box.Current].Real.length > 0) {
                    $scope.Fn.TabSearch.Refresh.Native();
                }
                $.each($scope[$scope.Attr.NameSpace].Box.Name, function(i, key) {
                    $scope[$scope.Attr.NameSpace].Box[key].Refresh = true;
                });
                $scope[$scope.Attr.NameSpace].Box[$scope[$scope.Attr.NameSpace].Box.Current].Refresh = false;
            },
            Refresh: {
                Server: function(nameSpace) {
                    nameSpace = nameSpace ? nameSpace : $scope.Attr.NameSpace;

                    var curPage = $scope[nameSpace];
                    var attr = curPage.Attr;
                    var unitcode = $scope.BaseData.Unit.Local.UnitCode;
                    var param = {
                        unitCode: ["", null].In_Array(attr[curPage.Box.Current].UnitCode) ? unitcode : attr[curPage.Box.Current].UnitCode,
                        rptType: ["", null].In_Array(attr[curPage.Box.Current].ORD_Code) ? $.cookie("ord_code") : attr[curPage.Box.Current].ORD_Code,
                        startTime: attr[curPage.Box.Current].StartDateTime,
                        endTime: attr[curPage.Box.Current].EndDateTime,
                        cycType: attr[curPage.Box.Current].StatisticalCycType == "" ? -1 : attr[curPage.Box.Current].StatisticalCycType,
                        type: nameSpace
                    };

                    if (curPage.Box.Current == 'CurrentDept') {
                        param.unitCode = unitcode;
                    }

                    if (nameSpace == "Receive") {
                        param.receiveState = curPage.Fn.BoxState();
                        param.unitType = ["", null].In_Array(attr[curPage.Box.Current].UnitCode) ? 1 : 2;
                    } else {
                        param.receiveState = 3;
                        param.unitType = curPage.Box.Current == 'CurrentDept' ? 0 : (["", null].In_Array(attr[curPage.Box.Current].UnitCode) ? 1 : 2);
                    }

                    var result = $scope.Fn.Ajax("Index/InboxRecycleSearch", param).reportTitles;

                    if ($.isArray(result)) {
                        $.each(result, function() {
                            this.Checked = false;
                            this.Remark = this.Remark.replaceAll("{/r-/n}", "\n");
                        });
                        curPage.Box[curPage.Box.Current].Real = result;
                        curPage.Box[curPage.Box.Current].Fake = result;
                        curPage.Box[curPage.Box.Current].Refresh = false;

                        if (result.length > 0) {
                            ngCss(".ListHorizontal ul.Head", curPage.Box.Current); //Change the head width
                        } else if ($scope.Fn.GetEvt().is("input[name=Search]")) {
                            Alert("没有搜到报表");
                        }
                    } else {
                        alert(result);
                    }

                    if (curPage.Box.Current == 'Auditing' && Number($(".MenuBar .InboxMsg").text()) > 0 && curPage.Attr.EndDateTime == App.Tools.Date.GetToday()) {
                        $scope.Fn.Ajax('Index/ClearMsg');
                        $(".MenuBar .InboxMsg").text("");
                    }
                },
                Native: function(nameSpace) {
                    nameSpace = nameSpace ? nameSpace : $scope.Attr.NameSpace;
                    var curPage = $scope[nameSpace];
                    var arr = angular.copy(curPage.Box[curPage.Box.Current].Real);
                    var fields = ["UnitCode", "ORD_Code", "StatisticalCycType", "StartDateTime", "EndDateTime"];
                    var toCompareValue = undefined;
                    var compareValue = undefined;
                    var timeField = undefined;
                    var type = undefined;

                    if (nameSpace == "Receive") {
                        timeField = "SendTime";
                        if (curPage.Box.Current == "Auditing") {
                            fields = fields.slice(0, 3);  //接收表箱不需要发送的起止时间作为筛选条件
                        }
                    } else {
                        timeField = "EndDateTime";
                    }

                    for (var i = 0; i < fields.length; i++) {
                        toCompareValue = curPage.Attr[curPage.Box.Current][fields[i]];
                        if (toCompareValue != "" && toCompareValue != null) {
                            arr = arr.filter(function(obj) {
                                compareValue = obj[fields[i]].trim();
                                if (!isNaN(compareValue) && !isNaN(toCompareValue)) {
                                    compareValue = parseInt(compareValue);
                                    toCompareValue = parseInt(toCompareValue);
                                    type = "Number";
                                } else {
                                    type = "NaN";
                                }

                                if (type == "NaN") {
                                    if (fields[i] == "StartDateTime" && (new Date(obj[timeField])).getTime() >= (new Date(toCompareValue + " 0:00:00")).getTime()) {
                                        return true;
                                    } else if (fields[i] == "EndDateTime" && (new Date(obj[timeField])).getTime() <= (new Date(toCompareValue + " 23:59:59")).getTime()) {
                                        return true;
                                    } else if(fields[i] == "ORD_Code" && obj[fields[i]] == toCompareValue) {
                                        return true;
                                    }
                                } else if (type == "Number" && compareValue == toCompareValue) {
                                    return true;
                                }
                                
                                return false;
                            });
                        }
                    }
                    curPage.Box[curPage.Box.Current].Fake = arr;
                    ngCss(".ListHorizontal ul.Head", curPage.Box.Current);   //Change the head width
                }
            },
            ViewReport: function(report) {
                var curPage = $scope[$scope.Attr.NameSpace];
                curPage.Box.Selected = report;

                if ($scope.Attr.NameSpace == "Receive") {
                    curPage.Box.Selected.Limit = parseInt($scope.BaseData.Unit.Local.Limit) + 1;
                } else {
                    if (curPage.Box.Current == 'CurrentDept') {
                        curPage.Box.Selected.Limit = baseData.Unit.Local.Limit;
                    } else {
                        curPage.Box.Selected.Limit = parseInt($scope.BaseData.Unit.Local.Limit) + 1;
                    }
                }

                var unitcode = "";
                var result = {};
                if (!curPage.Attr.UnderUnits[report.ORD_Code][report.UnitCode]) { //之前没查过下级单位信息
                    unitcode = report.UnitCode;
                }
                if (unitcode != "" || report.ORD_Code == "HL01") { //蓄水不需要预览报表
                    var unitType = undefined;

                    if ($scope.Attr.NameSpace == "Receive") {
                        unitType = (curPage.Attr.UnitCode == "" || curPage.Attr.UnitCode == null) ? 1 : 2;
                    } else {
                        unitType = curPage.Box.Current == "CurrentDept" ? 0 : 1;
                    }

                    result = $scope.Fn.Ajax("InboxAndRecycle/ViewReport", {
                        pageno: report.PageNO,
                        unitType: unitType,
                        unitCode: unitcode,
                        rptType: report.ORD_Code,
                    });
                }
                if (unitcode != "") {
                    curPage.Attr.UnderUnits[report.ORD_Code][unitcode] = result.underUnits;
                }
                window.UnderUnits = curPage.Attr.UnderUnits; //共享下级单位信息
                switch (report.ORD_Code) {
                case "HL01":
                    if (result.reportDetails.length == 0) {
                        result.reportDetails[0] = { DW: '合计' };
                    }
                    curPage.Attr.Report.Content = [];
                    curPage.Attr.Report.Affix = result.affix;
                    curPage.Attr.Report.PageNO = report.PageNO;
                    curPage.Attr.Report.Content.push(angular.copy(result.reportDetails[0]));
                    result.reportDetails.splice(0, 1);
                    var found = false;
                    angular.forEach(curPage.Attr.UnderUnits[report.ORD_Code][report.UnitCode], function(obj) {
                        found = false;
                        $.each(result.reportDetails, function(i) {
                            if (obj.UnitName == this.DW) {
                                curPage.Attr.Report.Content.push(angular.copy(this));
                                result.reportDetails.splice(i, 1);
                                found = true;
                                return false;
                            }
                        });
                        if (!found) {
                            curPage.Attr.Report.Content.push({ DW: obj.UnitName });
                        }
                    });
                    break;
                case "HP01":
                    $scope.Fn.ViewUnderRpt(report);
                    break;
                }
                $scope.Attr.Preview.Content = curPage.Attr.Report.Content;
            },
            SwitchBox: function(boxName) {
                var curPage = $scope[$scope.Attr.NameSpace];
                curPage.Box.Current = boxName;
                
                if (curPage.Box[boxName].Inited && !curPage.Box[boxName].Refresh) {
                    ngCss(".ListHorizontal ul.Head", boxName);   
                }

                if (!curPage.Box[boxName].Inited) {
                    this.Refresh.Server();
                    curPage.Box[boxName].Inited = true;
                } else if (curPage.Box[boxName].Refresh) {
                    this.Refresh.Native();
                }
                curPage.Box[boxName].Refresh = false;
                curPage.Attr.CheckAll = false;
                curPage.Box.Selected = undefined;
            },
            StopPro: function(e) {
                var evt = e || window.event;
                evt.stopPropagation ? evt.stopPropagation() : (evt.cancelBubble = true);
            }
        }
    };

    if (["HP01", "NP01"].In_Array($rootScope.SysORD_Code)) {
        $rootScope.Fn.OnInit();  //湖南蓄水、内蒙古蓄水要用到
    }
    $rootScope.CurrentUrl = window.location.hash.replace("#", "");
    $rootScope.RedictUrl = function (url) {
        var name = url.replace('/', '');
        if ($.isFunction($scope[name].Init)) {
            loading.run();
            $scope[name].Init();
            $scope[name].Init = true;
        }

        if ($scope.CurrentUrl != '/Receive' && url == '/Receive' && Number($(".MenuBar .InboxMsg").text()) > 0) {
            $scope.Fn.Ajax('Index/ClearMsg');
            $(".MenuBar .InboxMsg").text("");
            $scope.Receive.Attr.Auditing.UnitCode = "";
            $scope.Receive.Attr.Auditing.StartDateTime = App.Tools.Date.GetToday();
            $scope.Receive.Attr.Auditing.EndDateTime = $scope.Receive.Attr.Auditing.StartDateTime;
            $scope.Receive.Attr.Auditing.ORD_Code = $.cookie("ord_code");
            $scope.Receive.Attr.Auditing.StatisticalCycType = "";
            $scope.Receive.Box.Current = 'Auditing';

            angular.forEach($scope.Receive.Box.Name, function(i) {
                $scope.Receive.Box[i].Inited = false;
                $scope.Receive.Box[i].Recresh = false;
            });
        }
        $scope.CurrentUrl = url;
        $scope.Attr.NameSpace = url.replace("/", "");
        $state.go($scope.NameSpace + url.replace("/", "."));
    };
    //-------------------------------------</Public State>---------------------------------------------
    //-------------------------------------<New State>---------------------------------------------
    $rootScope.New = {
        Init: function() {
            /*$.each($scope.BaseData.Select.CycType[$scope.New.ReportTitle.ORD_Code], function() {
                this.time = $scope.Fn.OnChange.CycType(Number(this.value), $scope.New.ReportTitle);
            });*/
            this.Init = false;
        },
        SameReport: {
            Content: [],
            Current: {},
            Search: function(apply) {
                var obj = undefined;
                switch ($scope.New.ReportTitle.ORD_Code) {
                    case "NP01":
                    case "HP01":
                        if ($scope.New.XSRQ.Time) {
                            var arr = $scope.New.XSRQ.Time.split("-");
                            arr[0] = $scope.New.XSRQ.Year + "-" + arr[0].replace("月", "-").replace("日", "");
                            arr[1] = $scope.New.XSRQ.Year + "-" + arr[1].replace("月", "-").replace("日", "");
                            $scope.New.XSRQ.StartDateTime = arr[0];
                            $scope.New.XSRQ.EndDateTime = arr[1];
                            obj = $.extend(angular.copy($rootScope.New.ReportTitle), {
                                StartDateTime: arr[0],
                                EndDateTime: arr[1]
                            });
                        } else {
                            obj = undefined;
                        }
                        break;
                    default:
                        obj = obj ? obj : $rootScope.New.ReportTitle;
                        break;
                }

                if (obj) {
                    this.Content = $rootScope.Fn.Ajax("Index/SearchSameReport", obj).reportList;
                    this.Current = undefined;
                    $.each(this.Content, function() {
                        this.Remark = this.Remark.replaceAll('{/r-/n}', '\n');
                    });
                    if (apply) {
                        $(".LeftTree").scope().$apply();
                    }
                }
            }
        },
        ReportTitle: {
            StatisticalCycType: $rootScope.BaseData.Select.CycType[$rootScope.SysORD_Code][0].value,
            SourceType: 0,
            StartDateTime: App.Tools.Date.GetToday(),
            EndDateTime: App.Tools.Date.GetToday(),
            ORD_Code: $rootScope.SysORD_Code
        },
        XSRQ: {
            Year: new Date().getFullYear(),
            Time: undefined,
            StartDateTime: undefined,
            EndDateTime: undefined
        },
        CreateReport: function () {
            if (!$.isEmptyObject($scope.Open.Report.Opened.Find("ReportTitle.PageNO", 0))) {
                Alert("只能同时新建一套报表");
                return;
            }

            var rptType = $rootScope.New.ReportTitle.ORD_Code;
            var needDistinctValue = false;
            if (rptType == "HP01") {
                var tmp = undefined;
                tmp = $scope.Fn.Ajax("Index/GetHPLastYearData", {
                    startDateTime: $scope.New.XSRQ.StartDateTime,
                    endDateTime: $scope.New.XSRQ.EndDateTime,
                    fresh: Math.random()
                });
                for (var key in tmp.LNTQ) {
                    if (typeof tmp.LNTQ[key] == "string") {
                        if (tmp.LNTQ[key].indexOf(",") >= 0) {
                            tmp.LNTQ[key] = tmp.LNTQ[key].replaceAll(",", "");
                        }
                    }
                }
                $scope.BaseData.Reservoir.QNTQDXS = tmp.LNTQ;
                $scope.BaseData.Reservoir.SQXSL = tmp.SQXSL;
                $scope.BaseData.Reservoir.LNTQXSL = tmp.LNTQXSL;

                tmp = new Date($scope.New.XSRQ.StartDateTime).getMonth() + 1;
                
                if (tmp != new Date().getMonth()) {
                    tmp = $scope.Fn.Ajax("Index/GetHP01Constant", { month: tmp });
                    $.extend($scope.BaseData.Reservoir, tmp);
                }
            }
            if ($scope.BaseData.Unit.Local.Limit == 2 && $rootScope.New.ReportTitle.SourceType == "2") {  //创建省级累计表，需要剔除重复值
                needDistinctValue = true;
            }
            var report = App.Models[rptType.slice(0, 2)][rptType].Report(needDistinctValue);

            report.ReportTitle = $.extend(report.ReportTitle, $rootScope.New.ReportTitle, {
                StartDateTime: $scope.New[(rptType == "HL01" ? "ReportTitle" : "XSRQ")].StartDateTime,
                EndDateTime: $scope.New[(rptType == "HL01" ? "ReportTitle" : "XSRQ")].EndDateTime,
                WriterTime: App.Tools.Date.GetToday("yyyy-MM-dd HH:mm:ss")
            });

            var fn = function() {
                report.ReportTitle = $.extend(angular.copy(baseData.RecentReportInfo[report.ReportTitle.ORD_Code]), report.ReportTitle);
                report.ReportTitle.PageNO = 0;
                report.ReportTitle.State = 0;
                report.ReportTitle.Remark = undefined;
                if (parseInt(report.ReportTitle.StatisticalCycType) == 0) {
                    report.Attr.TableIndex = 8;
                    report.Attr.SSB = true;
                }

                if (rptType == "HL01") {
                    report.HL011 = report.HL011.RemoveBy("DW", "全区/县");
                    report.HL014 = report.HL014.RemoveBy("DW", "全区/县");

                    delete report.HL013[0].RiverCode;
                    delete report.HL013[0].GCJSSJ;
                }else if (rptType == "NP01") {
                    $http.post('Index/NP_Operate?type=GetNewPageNO&EndDateTime=' + $scope.New.XSRQ.EndDateTime).success(function(pageno) {
                        report.ReportTitle.PageNO = pageno;
                        $scope.New.SameReport.Content.push({
                            EndDateTime: report.ReportTitle.EndDateTime,
                            PageNO: report.ReportTitle.PageNO,
                            Remark: report.ReportTitle.Remark,
                            SourceType: report.ReportTitle.SourceType,
                            StartDateTime: report.ReportTitle.StartDateTime,
                            WriterTime: report.ReportTitle.WriterTime
                        });
                        $scope.Config.Page.New.Func.Create = true; 
                    });
                }

                $rootScope.Params["Open"] = {
                    method: 'Create',
                    report: report
                };

                $rootScope.RedictUrl("/Open");
            };

            if (!baseData.RecentReportInfo[report.ReportTitle.ORD_Code]) {
                $http.post('Index/GetReportTitleInfo?rptType=' + report.ReportTitle.ORD_Code).success(function(data) {
                    baseData.RecentReportInfo[report.ReportTitle.ORD_Code] = data.RecentReportInfo;
                }).then(fn);
            } else {
                fn();
            }
        },
        OpenReport: function () {
            if ($scope.New.SameReport.Current && $scope.New.SameReport.Current.PageNO) {
                $scope.Params["Open"] = {
                    method: "Open",
                    object: {
                        rptType: $rootScope.New.ReportTitle.ORD_Code,
                        sourceType: $rootScope.New.ReportTitle.SourceType,
                        pageno: $rootScope.New.SameReport.Current.PageNO,
                        digest: false
                    }
                };

                $rootScope.RedictUrl("/Open");
            } else {
                Alert("未选择报表");
            }
        }
    };

    $rootScope.$watch('New.ReportTitle.StatisticalCycType', function (to, from) {
        if (to != from) {
            if ($scope.New.ReportTitle.ORD_Code == "HL01") {
                $scope.Fn.OnChange.CycType(to, $scope.New.ReportTitle);
            }
            $scope.New.SameReport.Search();
        }
    });
    $rootScope.$watch('New.ReportTitle.SourceType', function (to, from) {
        if (to != from) {
            $rootScope.New.SameReport.Search();
        }
    });
    $rootScope.$watch('New.ReportTitle.StartDateTime', function (to, from) {
        if (to != from) {
            $rootScope.New.SameReport.Search();
        }
    });
    $rootScope.$watch('New.ReportTitle.EndDateTime', function () {
        $rootScope.New.SameReport.Search();
    });
    $rootScope.$watchCollection('[New.XSRQ.Year,New.XSRQ.Time]', function(to, from) {
        if (to && ($scope.New.ReportTitle.ORD_Code == "HP01" || $scope.New.ReportTitle.ORD_Code == "NP01")) {  //要判断
            $scope.New.SameReport.Search();
        }
    });
    $rootScope.$watchCollection('[New.ReportTitle.ORD_Code,New.SameReport.Content]', function(to, from) {
        var ordCode = $scope.New.ReportTitle.ORD_Code;
        $scope.Config.Page.New.Func = {
            Create: (ordCode == 'HP01' || ordCode == 'NP01') && $scope.New.SameReport.Content.length,
            Sum: {
                //浙江、黑龙江、内蒙古县级因为没有乡镇所以没有汇总，其中内蒙古蓄水无需汇总、累计，湖南蓄水只有汇总无累计
                Unders: "NP01" == ordCode || (["15", "22", "23", "33", "35", "44", "45"].In_Array($scope.SysUserCode) || ['HP01','NP01'].In_Array(ordCode)) && $scope.BaseData.Unit.Local.Limit == 4,
                Self: ordCode == 'HP01' || ordCode == 'NP01'
            }
        };
    });
    //-------------------------------------</New State>---------------------------------------------
    //-------------------------------------<Open State>---------------------------------------------
    $rootScope.Open = {
        Init: function() {
            var name = undefined;
            if ($rootScope.SysUserCode == '43') {
                name = 'HP01';
            }else if ($rootScope.SysUserCode == '15') {
                name = 'NP01';
            }

            if (name) {
                this.Tree.State[name] = {
                    MyRpt: new App.Models.TreeModel(),
                    ReceivedRpt: new App.Models.TreeModel()
                };
            }
            this.Init = angular.noop;
        },
        ReportTitle: {
            ORD_Code: $rootScope.SysORD_Code,
            StatisticalCycType: "",
            UnitCode: baseData.Unit.Local.UnitCode
        },
        Report: {
            Screen: {
                State: 'inhreit',
                Fn: screen
            },
            Opened: [],
            Current: undefined,
            Attr: {
                Instruction: undefined,
                Explain: undefined
            },
            Fn: {
                Core: {
                    AddRow: function() {
                        var table = $rootScope.Open.Report.Fn.Other.CurTable.Name();
                        if (table) {
                            $rootScope.Open.Report.Current[table].push(App.Models.HL.HL01[table]()); //var length = 
                        }
                    },
                    AggAcc: {   //汇总、累计时没有查合计行数据
                        get_aggacc_pagenos:function() {
                            var pagenos = "";
                            $.each($scope.Open.Report.Current.Attr.AggAcc.Content, function() {
                                pagenos += this.id + ",";
                            });
                            if (pagenos.length > 0) {
                                pagenos = pagenos.slice(0, pagenos.length - 2);
                            }

                            return pagenos;
                        },
                        View: function(_this) {
                            var obj = {
                                ORD_Code: $scope.Open.ReportTitle.ORD_Code,
                                SourceType: _this.SourceType,
                                UnitCode: _this.UnitCode,
                                PageNO: _this.id,
                                queryUnderUnits: true
                            };

                            if ($scope.Open.Report.Current.ReportTitle.SourceType == 1) {  //汇总
                                if ($scope.Receive.Attr.UnderUnits[$scope.Open.ReportTitle.ORD_Code][_this.UnitCode]) {
                                    obj.queryUnderUnits = false;
                                }
                                obj.Limit = parseInt($scope.BaseData.Unit.Local.Limit) + 1;
                            } else {  //累计
                                obj.Limit = $scope.BaseData.Unit.Local.Limit;
                            }

                            $scope.Fn.ViewUnderRpt(obj);
                        },
                        Sum: function() {
                            var treeType = undefined;
                            switch ($rootScope.Open.Report.Current.ReportTitle.SourceType) {
                            case "1":
                                treeType = "ReceivedRptTree";
                                break;
                            case "2":
                                treeType = "MyRptTree";
                                break;
                            }

                            var zTree = $.fn.zTree.getZTreeObj($rootScope.Open.Report.Current.ReportTitle.ORD_Code + treeType);
                            var checkedNodes = zTree.getCheckedNodes();
                            if (checkedNodes.length > 0) {
                                var exitSameRpt = false;

                                $.each(checkedNodes, function() {
                                    if (this.id == $scope.Open.Report.Current.ReportTitle.PageNO) {
                                        exitSameRpt = true;
                                    } else {
                                        exitSameRpt = $rootScope.Open.Report.Current.Attr.AggAcc.Content.SomeBy("id", this.id);
                                    }
                                    //exitSameRpt = $rootScope.Open.Report.Current.Attr.AggAcc.Content.SomeBy("id", this.id);

                                    if (exitSameRpt) {
                                        Alert("不能添加相同的报表");
                                        return false;
                                    }
                                });

                                if (exitSameRpt) {
                                    return;
                                }

                                var pagenos = undefined;
                                var sDate = undefined;
                                var eDate = undefined;
                                var tmpArr = [[], [], []];

                                var unitCodeArr = [[], []];
                                $.each(checkedNodes, function() {
                                    sDate = new Date(this.StartDate.replace("年", "-").replace("月", "-").replace("日", ""));
                                    eDate = new Date(this.EndDate.replace("年", "-").replace("月", "-").replace("日", ""));
                                    $rootScope.Open.Report.Current.Attr.AggAcc.Content.push({
                                        id: this.id,
                                        StartDate: (sDate.getMonth() + 1) + "月" + (sDate.getDate()) + "日",
                                        EndDate: (eDate.getMonth() + 1) + "月" + (eDate.getDate()) + "日",
                                        SourceType: this.SourceType,
                                        SendOperType: this.SendOperType,
                                        UnitCode: this.UnitCode,
                                        State: this.State.toString()
                                    });
                                    if ($rootScope.BaseData.Unit.Local.Limit == 2 && $rootScope.Open.Report.Current.ReportTitle.SourceType == "2") {
                                        tmpArr[parseInt(this.SourceType)].push(this.id);
                                    }
                                    zTree.checkNode(this, false);
                                });

                                if ($rootScope.BaseData.Unit.Local.Limit == 2 && $rootScope.Open.Report.Current.ReportTitle.SourceType == "2") {
                                    //pagenos = tmpArr[0].toString() + ";" + tmpArr[1].toString() + ";" + tmpArr[2].toString();
                                    pagenos = tmpArr[0].length ? (tmpArr[0].toString() + ",") : "";
                                    pagenos += tmpArr[1].length ? (tmpArr[1].toString() + ",") : "";
                                    pagenos += tmpArr[2].length ? (tmpArr[2].toString() + ",") : "";
                                    pagenos = pagenos.slice(0, pagenos.length - 1);
                                }
                                var report = $rootScope.Fn.Ajax("Index/SummaryReport", {
                                    pagenos: pagenos ? pagenos : checkedNodes.MapBy("id").toString(),
                                    all_pagenos: $scope.Open.Report.Fn.Core.AggAcc.get_aggacc_pagenos(),
                                    rptType: $rootScope.Open.ReportTitle.ORD_Code,
                                    typeLimit: $rootScope.Open.Report.Current.ReportTitle.SourceType == "1" ? 1 : 0,
                                    operateType: "sum"
                                });

                                var obj = undefined;
                                var curObj = undefined;
                                var exceptFields = undefined;
                                var curReport = $rootScope.Open.Report.Current;
                                var tmp = undefined;

                                switch ($rootScope.Open.ReportTitle.ORD_Code) {
                                case "HL01":
                                    $scope.Open.Report.Fn.Core.Private.Call("AggAcc.Sum", true, report);

                                    unitCodeArr = [];
                                    report.HL012.UnitCodeConvert(parseInt($.cookie("limit")) + 1);
                                    report.HL013.UnitCodeConvert(parseInt($.cookie("limit")) + 1);
                                    $.each([report.HL012, report.HL013], function() {
                                        $.each(this, function() {
                                            this.Checked = false;
                                            this.RiverSelect = $rootScope.Open.Report.Fn.Comm.ToRiverArr(this.RiverCode);
                                            this.DW = baseData.Unit.Unders.Find("UnitCode", this.UnitCode, "UnitName");
                                        });
                                    });
                                    curReport.HL012 = curReport.HL012.concat(report.HL012);
                                    curReport.HL013 = curReport.HL013.concat(report.HL013);
                                    delete report.HL012;
                                    exceptFields = ["SourcePageNo", "UnitCode", "RiverCode", "DataOrder", "DistributeRate", "DW", "CSMC", "GCJSSJ", "PageNO",'ZYZJZDSS', 'GCYMLS', 'GCLJJYL', 'YMFWBL'];
                                     /* else if ($scope.BaseData.Unit.Local.Limit == 3 && $scope.Open.Report.Current.ReportTitle.SourceType == 2) { //市级累计
                                            exceptFields.push("SZFWZ");
                                    }*/
                                    if ($scope.SysUserCode == '35') {
                                        exceptFields = exceptFields.concat(['SMXGS', 'SMXGD', 'SMXGQ', 'SMXJT']);
                                    }
                                    break;
                                case "HP01":
                                    exceptFields = ["DXKXXSL", "DXKKYS", "DXZYBFB", "QNTQDXS"]; //DXKYXSL 计划蓄水
                                    $.each([report.HP012.Real.Large, report.HP012.Real.Middle], function(i) {
                                        if (this.length > 0) {
                                            unitCodeArr[1].push(i ? "B" : "A");
                                        }
                                        $.each(this, function() {
                                            curObj = this;
                                            obj = $rootScope.Open.Report.Current.HP012.Real[i ? "Middle" : "Large"].Find("UnitCode", this.UnitCode);
                                            $.each(obj, function(key, val) {
                                                if (exceptFields.In_Array(key)) {
                                                    obj[key] = App.Tools.Calculator.Addition(curObj[key], val);

                                                    if (obj[key] != undefined) { //HP012将值加到合计那一行
                                                        tmp = $scope.Open.Report.Current.HP012.Real[i ? "Middle" : "Large"][0];
                                                        tmp[key] = App.Tools.Calculator.Addition(tmp[key], curObj[key]);
                                                    }

                                                    if (!unitCodeArr[1].In_Array(curObj.UnitCode)) {
                                                        unitCodeArr[1].push(curObj.UnitCode);
                                                    }
                                                }
                                            });
                                        });
                                    });
                                    delete report.HP012;
                                    exceptFields = ["UnitCode"];
                                    break;
                                }

                                /*if ($scope.SysORD_Code == "HL01") {
                                    if ($scope.BaseData.Unit.Local.Limit == 2 && $scope.Open.Report.Current.ReportTitle.SourceType == 2) { //省级累计
                                        exceptFields.push("SZFWX");
                                        this.Distinct(report, "add");
                                    }
                                }*/

                                $.each(report, function(key, arr) {
                                    $.each(arr, function() {
                                        if (key == "HL013") {
                                            obj = {};
                                        } else {
                                            obj = curReport[key][$rootScope.Open.Report.Fn.Other.UnitIndex(this.UnitCode)];
                                        }

                                        if(obj == undefined){
                                            return true;
                                        }

                                        $.each(this, function(field, val) {
                                            if (isNaN(field)) { //isNaN(field)  剔除重复值时要去掉
                                                if (!exceptFields.In_Array(field) && !isNaN(val)) {
                                                    if (key != "HL013") { //只有HL011、HL014、HL013、HP011
                                                        obj[field] = App.Tools.Calculator.Addition(obj[field], val); //HL011、HL014将值加到自己那一行
                                                    } else {
                                                        obj[field] = Number(val) > 0 ? val : undefined;
                                                    }

                                                    if (["33","45"].In_Array($scope.SysUserCode) && ["SZFWX", "SZFWZ"].In_Array(field) && obj.DW.Contains("本级")) {
                                                        return true;
                                                    } else {
                                                        if (obj[field] != undefined) { //HL011、HL013、HL014、HP011将值加到合计那一行
                                                            curReport[key][0][field] = App.Tools.Calculator.Addition(curReport[key][0][field], val);

                                                            if (key == "HP011" && !unitCodeArr[0].In_Array(obj.UnitCode)) {
                                                                unitCodeArr[0].push(obj.UnitCode);
                                                            } else if (key.indexOf("HL01") >= 0 && obj.UnitCode && !unitCodeArr.In_Array(obj.UnitCode)) {
                                                                unitCodeArr.push(obj.UnitCode);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    });
                                });

                                switch ($scope.Open.Report.Current.ReportTitle.ORD_Code) {
                                case "HP01":
                                    if (unitCodeArr[0].length > 0) {
                                        unitCodeArr[0].push(baseData.Unit.Local.UnitCode);
                                    }

                                    if (unitCodeArr[0].length > 0 || unitCodeArr[1].length > 0) {
                                        $scope.Open.Report.Fn.Core.ReCompute(unitCodeArr, "HP01");
                                    }
                                    break;
                                case "HL01":
                                    angular.forEach(unitCodeArr, function(unitcode) {
                                        $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(unitcode);
                                    });

                                    if ($scope.SysUserCode != '42') {
                                        $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.ZYZJZDSS.DecimalCount, ["HL013", "ZYZJZDSS"]);
                                        if ($scope.SysUserCode != '35') {
                                            $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.GCYMLS.DecimalCount, ["HL013", "GCYMLS"]);
                                            $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.GCLJJYL.DecimalCount, ["HL013", "GCLJJYL"]);
                                            $scope.Open.Report.Fn.Core.HL01.Avg(baseData.Field.YMFWBL.DecimalCount, ["HL013", "YMFWBL"]);
                                        }
                                    }

                                    if (["45"].In_Array($scope.SysUserCode)) { //包含“本级”的单位
                                        this.DisposeSZFW($scope.Open.Report.Current);
                                    }
                                    break;
                                }

                                $scope.Open.Report.Fn.Core.Private.Call("Limit." + baseData.Unit.Local.Limit, true);
                                $scope.Open.Report.Fn.Core.Private.Call("Check", true, { type: 'AggAcc' });

                                $scope.Open.Report.Current.Attr.Data_Changed = true;
                            } else {
                                Alert("未选择报表！");
                            }
                        },
                        Sub: function() {
                            if ($scope.Open.Report.Attr.AggAcc && $scope.Open.Report.Attr.AggAcc.Selected) {
                                var rpt = $scope.Open.Report.Attr.AggAcc.Selected;
                                if (rpt) {
                                    var pagenos = undefined;
                                    if ($rootScope.BaseData.Unit.Local.Limit == 2 && $rootScope.Open.Report.Current.ReportTitle.SourceType == "2") {
                                        pagenos = rpt.id;
                                        /*switch (rpt.SourceType) {
                                        case "0":
                                            pagenos = rpt.id + ";;";
                                            break;
                                        case "1":
                                            pagenos = ";" + rpt.id + ";";
                                            break;
                                        case "2":
                                            pagenos = ";;" + rpt.id;
                                            break;
                                        }*/
                                    }
                                    var report = $rootScope.Fn.Ajax("Index/SummaryReport", {
                                        pagenos: pagenos ? pagenos : rpt.id,
                                        all_pagenos: $scope.Open.Report.Fn.Core.AggAcc.get_aggacc_pagenos(),
                                        rptType: $rootScope.Open.ReportTitle.ORD_Code,
                                        typeLimit: $rootScope.Open.Report.Current.ReportTitle.SourceType == "1" ? 1 : 0,
                                        operateType: "sub"
                                    });
                                    var obj = undefined;
                                    var curObj = undefined;
                                    var curReport = $rootScope.Open.Report.Current;
                                    var exceptFields = undefined;
                                    var unitCodeArr = undefined;
                                    var tmp = undefined;

                                    switch ($rootScope.Open.ReportTitle.ORD_Code) {
                                    case "HL01":
                                        $scope.Open.Report.Fn.Core.Private.Call("AggAcc.Sub", true, { report: report, pageno: rpt.id });

                                        unitCodeArr = [];
                                        $scope.Open.Report.Fn.Core.DelRow("HL012", rpt.id);
                                        $scope.Open.Report.Fn.Core.DelRow("HL013", rpt.id);
                                        exceptFields = ["SourcePageNo", "UnitCode", "RiverCode", "DataOrder", "DistributeRate", "DW", "CSMC", "GCJSSJ", "PageNO", "SZRKR", "SWRK",'ZYZJZDSS', 'GCYMLS', 'GCLJJYL', 'YMFWBL'];
                                        /*if (!($scope.SysUserCode == "33" && (baseData.Unit.Local.Limit == 4 || baseData.Unit.Local.Limit == 3))) { //浙江县级、市级（累计、汇总减表）表6不需要跟表1关联
                                            exceptFields.push("SYCS");
                                        }*/
                                        if (!($scope.SysUserCode == "33" && baseData.Unit.Local.Limit == 4)) {
                                            exceptFields.push("SYCS");
                                        }
                                         /*else if ($scope.BaseData.Unit.Local.Limit == 3 && $scope.Open.Report.Current.ReportTitle.SourceType == 2) { //市级累计
                                            exceptFields.push("SZFWZ");
                                        }*/
                                        if ($scope.SysUserCode == '35') {
                                            exceptFields = exceptFields.concat(['SMXGS', 'SMXGD', 'SMXGQ', 'SMXJT']);
                                        }
                                        delete report.HL012;
                                        delete report.HL013;
                                        break;
                                    case "HP01":
                                        unitCodeArr = [[], []];
                                        exceptFields = ["DXKXXSL", "DXKKYS", "DXZYBFB", "QNTQDXS"]; //DXKYXSL 计划蓄水
                                        $.each([report.HP012.Real.Large, report.HP012.Real.Middle], function(i) {
                                            if (this.length > 0) {
                                                unitCodeArr[1].push(i ? "B" : "A");
                                            }
                                            $.each(this, function() {
                                                curObj = this;
                                                obj = $rootScope.Open.Report.Current.HP012.Real[i ? "Middle" : "Large"].Find("UnitCode", this.UnitCode);
                                                $.each(obj, function(key, val) {
                                                    if (exceptFields.In_Array(key)) {
                                                        obj[key] = App.Tools.Calculator.Subtraction(val, curObj[key]);
                                                        obj[key] = obj[key] > 0 ? obj[key] : undefined; //防止减表出现负数，实现差之功能时，应移除此行代码

                                                        tmp = $scope.Open.Report.Current.HP012.Real[i ? "Middle" : "Large"][0];
                                                        tmp[key] = App.Tools.Calculator.Subtraction(tmp[key], curObj[key]);
                                                        tmp[key] = tmp[key] > 0 ? tmp[key] : undefined; //防止减表出现负数，实现差之功能时，应移除此行代码

                                                        if (!unitCodeArr[1].In_Array(curObj.UnitCode)) {
                                                            unitCodeArr[1].push(curObj.UnitCode);
                                                        }
                                                    }
                                                });
                                            });
                                        });
                                        delete report.HP012;
                                        exceptFields = ["UnitCode"];
                                        break;
                                    }

                                    /*if ($scope.SysORD_Code == "HL01") {
                                        if ($scope.BaseData.Unit.Local.Limit == 2 && $scope.Open.Report.Current.ReportTitle.SourceType == 2) { //省级累计
                                            exceptFields.push("SZFWX");
                                            this.Distinct(report, "sub");
                                        }
                                    }*/

                                    $.each(report, function(key, arr) {
                                        $.each(arr, function() {
                                            if (key == "HL013") {
                                                obj = curReport[key][0];
                                            } else {
                                                obj = curReport[key][$rootScope.Open.Report.Fn.Other.UnitIndex(this.UnitCode)];
                                            }

                                            if (obj == undefined) {
                                                return true;
                                            }

                                            $.each(this, function(field, val) {
                                                if (isNaN(field)) { //isNaN(field)  剔除重复值时要去掉
                                                    if (!exceptFields.In_Array(field) && !isNaN(val)) {
                                                        if (key != "HL013") {
                                                            obj[field] = App.Tools.Calculator.Subtraction(obj[field], val);
                                                            obj[field] = obj[field] > 0 ? obj[field] : undefined; //防止减表出现负数，实现差之功能时，应移除此行代码

                                                            if (key == "HP011" && !unitCodeArr[0].In_Array(obj.UnitCode)) {
                                                                unitCodeArr[0].push(obj.UnitCode);
                                                            } else if (key.indexOf("HL01") >= 0 && !unitCodeArr.In_Array(obj.UnitCode)) {
                                                                unitCodeArr.push(obj.UnitCode);
                                                            }
                                                        }
                                                        curReport[key][0][field] = App.Tools.Calculator.Subtraction(curReport[key][0][field], val);
                                                        curReport[key][0][field] = curReport[key][0][field] > 0 ? curReport[key][0][field] : undefined; //防止减表出现负数，实现差之功能时，应移除此行代码
                                                    }
                                                }
                                            });
                                        });
                                    });

                                    $rootScope.Open.Report.Current.Attr.AggAcc.Content = $rootScope.Open.Report.Current.Attr.AggAcc.Content.RemoveBy("id", rpt.id);
                                    $scope.Open.Report.Attr.AggAcc.Selected = undefined;
                                }

                                switch ($scope.Open.Report.Current.ReportTitle.ORD_Code) {
                                case "HP01":
                                    if (unitCodeArr[0].length > 0) {
                                        unitCodeArr[0].push(baseData.Unit.Local.UnitCode);
                                    }

                                    if (unitCodeArr[0].length > 0 || unitCodeArr[1].length > 0) {
                                        $scope.Open.Report.Fn.Core.ReCompute(unitCodeArr, "HP01");
                                    }
                                    break;
                                case "HL01":
                                    angular.forEach(unitCodeArr, function(unitcode) {
                                        $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(unitcode);
                                    });

                                    if ($scope.SysUserCode != '42') {
                                        $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.ZYZJZDSS.DecimalCount, ["HL013", "ZYZJZDSS"]);
                                        if ($scope.SysUserCode != '35') {
                                            $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.GCYMLS.DecimalCount, ["HL013", "GCYMLS"]);
                                            $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.GCLJJYL.DecimalCount, ["HL013", "GCLJJYL"]);
                                            $scope.Open.Report.Fn.Core.HL01.Avg(baseData.Field.YMFWBL.DecimalCount, ["HL013", "YMFWBL"]);
                                        }
                                    }

                                    if (["45"].In_Array($scope.SysUserCode)) { //包含“本级”的单位
                                        this.DisposeSZFW($scope.Open.Report.Current);
                                    }
                                    break;
                                }
                                $scope.Open.Report.Fn.Core.Private.Call("Limit." + baseData.Unit.Local.Limit, true);
                                $scope.Open.Report.Fn.Core.Private.Call("Check", true, { type: 'AggAcc' });

                                $scope.Open.Report.Current.Attr.Data_Changed = true;
                            } else {
                                Alert("未选择报表！");
                            }
                        },
                        DisposeSZFW: function(report) {
                            if ([2, 3].In_Array($scope.BaseData.Unit.Local.Limit) && $scope.Open.Report.Current.ReportTitle.SourceType == 2) {
                                var total_x = undefined, total_z = undefined;
                                $.each(report.HL011, function(i) {
                                    if (i > 0 && !this.DW.Contains("本级")) {
                                        if ($scope.BaseData.Unit.Local.Limit == 2) {
                                            total_x = App.Tools.Calculator.Addition(total_x, this.SZFWX);
                                        }
                                        total_z = App.Tools.Calculator.Addition(total_z, this.SZFWZ);
                                    }
                                });
                                if ($scope.BaseData.Unit.Local.Limit == 2) {
                                    report.HL011[0].SZFWX = total_x;
                                }
                                report.HL011[0].SZFWZ = total_z;
                            }
                        },
                        Distinct: function(report, type) {
                            var _continue = false, curReport = $scope.Open.Report.Current;
                            var sub_len = 0, units = [], obj = undefined, field = undefined;
                            if ($scope.BaseData.Unit.Local.Limit == 2 && $scope.Open.Report.Current.ReportTitle.SourceType == 2) { //省级累计
                                _continue = true;
                                sub_len = 4;
                                field = "SZFWX";
                            } /*else if ($scope.BaseData.Unit.Local.Limit == 3 && $scope.Open.Report.Current.ReportTitle.SourceType == 2) { //市级累计
                                _continue = true;
                                sub_len = 6;
                                field = "SZFWZ";
                            }*/
                            if (_continue) {
                                $.each(report.HL011, function(i) {
                                    if (sub_len == 4) {
                                        /*if (this.UnitCode && this.UnitCode.slice(4) == "0000" && !this.DW) {  //录入值
                                            obj = curReport.HL011[$scope.Open.Report.Fn.Other.UnitIndex(this.UnitCode)];
                                            if (type == "add") {
                                                obj[field] = App.Tools.Calculator.Addition(obj[field], this.SZFWX);
                                            } else {
                                                obj[field] = App.Tools.Calculator.Subtraction(obj[field], this.SZFWX);
                                            }

                                            obj = curReport.HL011[0];
                                            if (obj != undefined) {
                                                if (type == "add") {
                                                    obj[field] = App.Tools.Calculator.Addition(obj[field], this.SZFWX);
                                                } else {
                                                    obj[field] = App.Tools.Calculator.Subtraction(obj[field], this.SZFWX);
                                                }
                                            }
                                        }*/

                                        if (!this.UnitCode) {
                                            $.each(this, function(key, val) {
                                                /*if (!isNaN(key) && !units.In_Array(key)) {
                                                    units.push(key.slice(0, sub_len) + "0000");
                                                }*/
                                                obj = curReport.HL011[$scope.Open.Report.Fn.Other.UnitIndex(key.slice(0, sub_len) + "0000")];
                                                if (type == "add") {
                                                    obj[field] = App.Tools.Calculator.Addition(obj[field], val);
                                                } else {
                                                    obj[field] = App.Tools.Calculator.Subtraction(obj[field], val);
                                                }

                                                obj = curReport.HL011[0];
                                                if (obj != undefined) {
                                                    if (type == "add") {
                                                        obj[field] = App.Tools.Calculator.Addition(obj[field], val);
                                                    } else {
                                                        obj[field] = App.Tools.Calculator.Subtraction(obj[field], val);
                                                    }
                                                }
                                            });
                                        }
                                    }/* else {
                                        if (!units.In_Array(this.UnitCode)) {
                                            units.push(this.UnitCode);
                                        }
                                    }*/
                                });

                                /*angular.forEach(units, function(code) {
                                    obj = curReport.HL011[$scope.Open.Report.Fn.Other.UnitIndex(code)];
                                    if (obj != undefined) {
                                        if (type == "add") {
                                            obj[field] = App.Tools.Calculator.Addition(obj[field], 1);
                                        } else {
                                            obj[field] = App.Tools.Calculator.Subtraction(obj[field], 1);
                                        }
                                        
                                    }
                                });
                                obj = curReport.HL011[0];
                                if (obj != undefined) {
                                    if (type == "add") {
                                        obj[field] = App.Tools.Calculator.Addition(obj[field], units.length);
                                    } else {
                                        obj[field] = App.Tools.Calculator.Subtraction(obj[field], units.length);
                                    }
                                }*/
                            }
                        }
                    },
                    Close: function(pageno) {
                        pageno = pageno == undefined ? $rootScope.Open.Report.Current.ReportTitle.PageNO : pageno;
                        if (parseInt(pageno) == 0) {
                            if (!confirm("报表尚未保存，确定要关闭么？")) {
                                return;
                            }
                        } else if($scope.Open.Report.Current.Attr.Data_Changed){
                            if (!confirm("报表修改的的数据尚未保存，确定要关闭么？")) {
                                 return;
                            }
                        }

                        $.each($rootScope.Open.Report.Opened, function(i) {
                            if (this.ReportTitle.PageNO == pageno) {
                                $rootScope.Open.Report.Opened.splice(i, 1);
                                return false;
                            }
                        });

                        var length = $rootScope.Open.Report.Opened.length;
                        if (length > 0) {
                            $scope.Open.Report.Current = $rootScope.Open.Report.Opened[length - 1];
                        } else {
                            $scope.Open.Report.Current = undefined;
                            $scope.Open.Report.Screen.State = 'inhreit';
                        }

                        $scope.Open.Tree.Refresh.CheckBox();
                    },
                    ReportDetails: function() {
                        if (['45', '51'].In_Array($scope.SysUserCode)) {
                            window.plugins.preloader('start');
                            $.getScript('Index/GetFile?filename=~/Scripts/Models/ReportDetails/' + $scope.SysUserCode + '.js', function() {
                                var report = App.Models.HL.HL01.ReportDetials[$scope.SysUserCode](angular.copy($scope.Open.Report.Current), $scope.BaseData.Field);
                                var names = [], values = [], url;
                                $.each(report, function(name, value) {
                                    names.push(name);
                                    values.push(value);
                                });
                                url = "BaseData/ExportDisasterReview?report=" + names.toString() + ";" + values.toString().replaceAll("（", "").replaceAll("）", "");
                                if ($scope.SysUserCode == '51') {
                                    var callback = function() {
                                        var img_url = App.Models.HL.HL01.ReportDetials[$scope.SysUserCode].SaveSvg($scope.Open.Report.Current, $scope.BaseData.Field);
                                        window.open(url + '&img_url=' + img_url);
                                        window.plugins.preloader('stop');
                                    }

                                    if (!angular.isObject(window.Highcharts)) {
                                        $.getScript('Scripts/Library/Plugins/highchart/chart_export_3d.min.js', callback);
                                    } else {
                                        callback();
                                    }
                                } else {
                                    window.open(url);
                                }
                            });
                        } else {
                            var param = $scope.Open.Report.Current.ReportTitle.ORD_Code;
                            if (param == "HP01") { //蓄水通报
                                var tmp = undefined;
                                var endTimeArr = $scope.Open.Report.Current.ReportTitle.EndDateTime.split("-");
                                endTimeArr[0] = Number(endTimeArr[0]);
                                endTimeArr[1] = Number(endTimeArr[1]);
                                endTimeArr[2] = Number(endTimeArr[2]);
                                $.each($scope.BaseData.HPDate, function(i) {
                                    tmp = this.FakeTime.replace("月", ".").replace("日", "").split(".");
                                    if (Number(tmp[0]) == endTimeArr[1] && Number(tmp[1]) == endTimeArr[2]) {
                                        if (i == 0) { //上期是最后一个月份
                                            tmp = $scope.BaseData.HPDate[$scope.BaseData.HPDate.length - 1];
                                            endTimeArr[0] = endTimeArr[0] - 1;
                                        } else {
                                            tmp = $scope.BaseData.HPDate[i - 1];
                                        }
                                        tmp = tmp.FakeTime.replace("月", ".").replace("日", "").split(".");
                                        return false;
                                    }
                                });
                                param = "ORD_Code=" + param;
                                param += "&SQTime=" + endTimeArr[0] + "-" + tmp[0] + "-" + tmp[1];
                                param += "&TQTime=" + $scope.Open.Report.Current.ReportTitle.EndDateTime;
                            } else {
                                param = "ORD_Code=" + param;
                                param += "&UserCode=" + $scope.SysUserCode;
                            }

                            window.open("Index/ReportData?" + param, "", "", "");
                        }
                    },
                    ChangeReportState: function(stateName) {
                        stateName = stateName == undefined ? "Modified" : stateName;
                        if ($rootScope.Open.Report.Current.Attr.ReportState != stateName) {
                            $rootScope.Open.Report.Current.Attr.ReportState = stateName;
                        }
                    },
                    Delete: {
                        Affix: function(index, tbno, url) {
                            if (confirm("确认删除么？")) {
                                $rootScope.Open.Report.Current.Attr.DelAffixTBNO.push(tbno);
                                $rootScope.Open.Report.Current.Attr.DelAffixURL.push(url);
                                $rootScope.Open.Report.Current.Affix.splice(index, 1);
                            }
                        },
                        Data: function() {  //内蒙古蓄水清除手动录入数据的方法
                            if (confirm("确认清除录入的数据么？")) {
                                $.each($scope.Open.Report.Current.NP011, function() {
                                    this.DQSW = undefined;
                                    this.DQXSL = undefined;
                                    this.SFCXXSW = undefined;
                                });
                                Alert("清除完成");
                            }
                        },
                        Report: function() {
                            if (confirm("是否确认删除该报表？")) {
                                $scope.New.SameReport.Content = $scope.New.SameReport.Content.RemoveBy("PageNO", $scope.Open.Report.Current.ReportTitle.PageNO);
                                var result = $rootScope.Fn.Ajax("Index/DeleteReport", {
                                    type: "0",
                                    pageno: $scope.Open.Report.Current.ReportTitle.PageNO,
                                    state: $scope.Open.Report.Current.ReportTitle.State,
                                    limit: $.cookie("limit")
                                });

                                if (result > 0) {
                                    if ($scope.Open.Tree.Current.TreeId.indexOf("MyRptTree") > 0) {
                                        $scope.New.SameReport.Content.RemoveBy("PageNO", $scope.Open.Report.Current.ReportTitle.PageNO);
                                    }

                                    $scope.Open.Tree.Switch("MyRptTree", true).Refresh.Report().NodeState();
                                    if ($scope.Open.Report.Current.ReportTitle.SourceType == 1) {  //删除汇总表，更新已收表箱
                                        $scope.Open.Tree.Data.Load(false, "ReceivedRptTree");
                                    } //删除累计表，更新我的表箱   可优化：删除录入表，不要刷新整个Tree，只删除节点

                                    $rootScope.Open.Report.Fn.Core.Close();

                                    if ($scope.RecycleBin.Box.Current == 'CurrentDept') {
                                        $scope.Fn.TabSearch.Refresh.Server('RecycleBin');
                                    } else {
                                        $scope.RecycleBin.Box.CurrentDept.Inited = false;
                                    }

                                    Alert("删除成功");
                                } else if (result == -1) {
                                    Alert("该报表已参与累计或汇总，不能删除");
                                } else {
                                    Alert(result);
                                    throw result;
                                }
                            }
                        }
                    },
                    DelRow: function(table, sPageNO) {
                        var report = $rootScope.Open.Report;
                        table = table ? table : report.Fn.Other.CurTable.Name();
                        var arr = report.Current[table];
                        var obj = undefined;
                        var field = table === "HL013" ? "SYCS" : "SWRK";
                        var count = 0;
                        var subCount1 = 0;
                        var subCount2 = 0;
                        var unitCodeArr = [];
                        arr = $.grep(arr, function(_obj) {
                            if ((_obj.Checked || (sPageNO && _obj.SourcePageNo == sPageNO)) && _obj.UnitCode != $rootScope.BaseData.Unit.Local.UnitCode) {
                                obj = report.Current.HL011[report.Fn.Other.UnitIndex(_obj.UnitCode)];

                                if (table == "HL012") {
                                    field = _obj.DataType == "死亡" ? "SWRK" : "SZRKR";
                                }

                                count = Number(obj[field]);
                                count = count > 0 ? count : 0;
                                obj[field] = --count > 0 ? count : undefined;

                                if (field == "SZRKR") {
                                    subCount2++;
                                } else {
                                    subCount1++;
                                }

                                if (!unitCodeArr.In_Array(_obj.UnitCode)) {
                                    unitCodeArr.push(_obj.UnitCode);
                                }
                            }

                            if (sPageNO) {
                                return _obj.SourcePageNo == sPageNO;
                            } else {
                                return _obj.Checked;
                            }
                        }, true);
                        report.Current[table] = arr;

                        /*$scope.Fn.GetEvt().data('NoCallUnitEntity', true);
                        $scope.Fn.GetEvt().data('NoCheck', true);*/
                        $scope.Fn.GetEvt().data('disabledCallBack', ["UnitEntity", "RelationCheck"]);

                        if (table === "HL013") { //重新算合计那行
                            var _this = this;
                            var fields = App.Config.Field.Fn.GetModel("HL01." + $scope.SysUserCode + ".HL013");
                            var exceptField = ["CSMC", "GCJSSJ", "ZYZJZDSS", "GCYMLS", "GCLJJYL", "RiverCode", "UnitCode",'ZYZJZDSS', 'GCYMLS', 'GCLJJYL', 'YMFWBL'];
                            if ($scope.SysUserCode == '35') {
                                exceptField = exceptField.concat(['SMXGS', 'SMXGD', 'SMXGQ', 'SMXJT']);
                            }

                            if (arr.length > 1) {
                                $.each(fields, function(key) {
                                    if (!exceptField.In_Array(key)) {
                                        _this.Sum([table, key]);
                                    }
                                });
                            } else {
                                $.each(fields, function(key) {
                                    report.Current[table][0][key] = undefined;
                                });
                            }
                        }

                        obj = report.Current.HL011[0];

                        if (table == "HL012") {
                            angular.forEach(["SWRK", "SZRKR"], function(field) {
                                count = App.Tools.Calculator.Number(obj[field]);
                                obj[field] = App.Tools.Calculator.Subtraction(count, field == "SZRKR" ? subCount2 : subCount1);
                            });
                        } else {
                            count = App.Tools.Calculator.Number(obj[field]);
                            obj[field] = App.Tools.Calculator.Subtraction(count, subCount1);

                            $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.ZYZJZDSS.DecimalCount, ["HL013", "ZYZJZDSS"]);

                            if ($scope.SysUserCode != '35') {
                                $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.GCYMLS.DecimalCount, ["HL013", "GCYMLS"]);
                                $scope.Open.Report.Fn.Core.HL01.Max(baseData.Field.GCLJJYL.DecimalCount, ["HL013", "GCLJJYL"]);
                                $scope.Open.Report.Fn.Core.HL01.Avg(baseData.Field.YMFWBL.DecimalCount, ["HL013", "YMFWBL"]);
                            }
                        }

                        angular.forEach(unitCodeArr, function(unitcode) {
                            $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(unitcode);
                        });
                    },
                    NP01:{
                        Sum: {
                            DQXSL: function() {
                                var total, np011S = $scope.Open.Report.Current.NP011;
                                var hj = np011S.Find("UnitCode", $scope.BaseData.Unit.Local.UnitCode);
                                if ($.isEmptyObject(hj)) {
                                    hj = $.extend(App.Models.NP.NP01.NP011.Object(), $scope.BaseData.Unit.XSHJ[0]);
                                    $scope[$scope.Attr.NameSpace].Report.Current.NP011.InsertAt(0, hj);
                                }
                                $.each(np011S, function(i) {
                                    if (i > 0) {
                                        total = App.Tools.Calculator.Addition(total, this.DQXSL);
                                    }
                                });
                                hj.DQXSL = total;
                            }
                        },
                        Save: function(rpt) {
                            $scope.Open.Report.Fn.Core.HL01.Save(rpt);
                        },
                        Sort: function(np011) {
                            np011 = np011 ? $scope.Open.Report.Current.NP011 : np011;
                            var index = undefined;
                            var count = undefined;
                            var arr = [];
                            switch ($scope.BaseData.Unit.Local.Limit) {
                            case 2:
                                arr = arr.concat(App.Models.NP.NP01.NP011.Array(false, $scope.Open.Report.Current.NP011));
                                $scope.Open.Report.Current.NP011 = arr;
                                if ($scope.BaseData.Unit.Local.Limit == 2) {
                                    var np011S = $scope.Open.Report.Current.NP011;
                                    $.each(np011S, function() {
                                        np011S[0].ZKR = App.Tools.Calculator.Addition(np011S[0].ZKR, this.ZKR);
                                        np011S[0].SKR = App.Tools.Calculator.Addition(np011S[0].SKR, this.SKR);
                                        np011S[0].DQXSL = App.Tools.Calculator.Addition(np011S[0].DQXSL, this.DQXSL);
                                    });
                                }
                                break;
                            case 3:
                                index = 1;
                                count = 0;
                                var name = $scope.Open.Report.Current.NP011[0].UnitName;
                                $.each($scope.Open.Report.Current.NP011, function(i) {
                                    if (name != this.UnitName) {
                                        arr.push($.extend(App.Models.NP.NP01.NP011.Object(), {
                                            No: App.Tools.Convert.ToChineseNumber(index),
                                            RSName: name,
                                            UnitName: count
                                        }));
                                        arr = arr.concat($scope.Open.Report.Current.NP011.Slice(i - count, count));
                                        index++;
                                        count = 0;
                                        name = this.UnitName;
                                    }
                                    this.No = ++count;
                                });
                                $scope.Open.Report.Current.NP011 = arr;
                                break;
                            case 4:
                                $.each($scope.Open.Report.Current.NP011, function(i) {
                                    this.No = i + 1;
                                });
                                $scope.Open.Report.Current.NP011.InsertAt(0, $.extend(App.Models.NP.NP01.NP011.Object(), {
                                    No: "一",
                                    RSName: $scope.BaseData.Unit.Local.UnitName,
                                    UnitName: $scope.Open.Report.Current.NP011.length
                                }));
                                break;
                            }
                        }
                    },
                    HL01: {
                        Avg: function(fixed, field_info, not_hj) {
                            fixed = fixed ? fixed : 0;
                            var table_field = $.isArray(field_info) ? field_info : $scope.Fn.GetEvt().attr('ng-model').split(".");
                            var arr = $scope.Open.Report.Current[table_field[0].toUpperCase()];
                            var total = 0;
                            $.each(arr, function(i) {
                                if (i > 0) {
                                    if (Number(this[table_field[1]]) > 0) {
                                        this[table_field[1]] = parseFloat(this[table_field[1]]).toFixed(fixed);
                                        if (Number(this[table_field[1]]) == 0) {
                                            this[table_field[1]] = undefined;
                                        }
                                    } else {
                                        this[table_field[1]] = undefined;
                                    }
                                    total = App.Tools.Calculator.Addition(this[table_field[1]], total);
                                }
                            });

                            if (Number(total) > 0) {
                                total = App.Tools.Calculator.Division(total, arr.length - 1);
                                total = parseFloat(total).toFixed(fixed);
                                if(Number(total) == 0){
                                    total = undefined;
                                }
                            }

                            if (!not_hj) {
                                $scope.Open.Report.Current[table_field[0].toUpperCase()][0][table_field[1]] = total;
                            }
                        },
                        Save: function(rpt) {
                            $rootScope.Open.Report.Current.Attr.CheckErrors = {};
                            var exit = false;
                            var checkResult = $rootScope.Open.Report.Fn.Core.RelationCheck(0, 0, 0, 0, 1, rpt);

                            var fn = function() {
                                var config = {
                                    Title: "是否分流域",
                                    Layout: "Table",
                                    Content: [],
                                    Input: {
                                        Unit: "",
                                        Width: "60px"
                                    },
                                    Button: {
                                        Left: {
                                            Text: "分流域并保存",
                                            CallBack: function() {
                                                var obj = {};
                                                var val = 0;
                                                $rootScope.Dialog.Attr.Message = undefined;
                                                $.each($rootScope.Dialog.Attr.Content, function() {
                                                    if (!obj[this.UnitCode]) {
                                                        obj[this.UnitCode] = [];
                                                    }

                                                    obj[this.UnitCode].push({
                                                        RiverCode: this.RiverCode,
                                                        RiverData: Number(this.Model)
                                                    });
                                                });

                                                $.each(obj, function(unitcode, arr) {
                                                    val = 0;
                                                    $.each(arr, function() {
                                                        val = App.Tools.Calculator.Addition(this.RiverData, val);
                                                    });

                                                    if (Number(val) != 1) {
                                                        $rootScope.Dialog.Attr.Message = baseData.Unit.Unders.Find("UnitCode", unitcode, "UnitName") + "的流域比例设置错误";
                                                        return false;
                                                    }
                                                });

                                                if (!$rootScope.Dialog.Attr.Message) {
                                                    $rootScope.Open.Report.Current.Attr.DistributeRiver = obj;
                                                    $rootScope.Dialog.Attr.Show = false;
                                                    $rootScope.Open.Report.Fn.Core.Save();
                                                }
                                            }
                                        },
                                        Right: {
                                            Text: "不分流域直接保存",
                                            CallBack: function() {
                                                $rootScope.Dialog.Attr.Show = false;
                                                delete $scope.Open.Report.Current.Attr.DistributeRiver;
                                                $rootScope.Open.Report.Fn.Core.Save();
                                            }
                                        }
                                    }
                                };

                                if (baseData.Unit.Local.Limit == 2 && baseData.DistributeRiver.length > 0) { //只有省级可分流域
                                    $.each(baseData.DistributeRiver, function() {
                                        config.Content.push({
                                            Name: baseData.Unit.Unders.Find("UnitCode", this.UnitCode, "UnitName") + " " + this.RiverName,
                                            UnitCode: this.UnitCode,
                                            RiverCode: this.RiverCode,
                                            Type: "text",
                                            Model: this.RDRate
                                        });
                                    });

                                    $rootScope.Dialog.Config(config);
                                } else {
                                    if (baseData.Unit.Local.Limit == 2) {
                                        $rootScope.Dialog.Config(config);
                                    } else {
                                        $rootScope.Open.Report.Fn.Core.Save(false, rpt);
                                    }
                                }
                            };

                            if (!$.isEmptyObject(checkResult)) {
                                $scope.Open.Report.Current.Attr.CheckErrors = checkResult;
                                $scope.Open.Report.Current.Attr.ReportFooter = "CheckAlert";

                                $.each(checkResult, function(key) {
                                    if (key.indexOf("-Select") > 0) {
                                        /*if (key == 'HL012-Select') {
                                            exit = this.Message; //($scope.Open.Report.Current.Attr.SSB ? '死亡/失踪' : '表5') + ' 表地区必须选择';
                                        }else if (key == "HL013-Select") {
                                            exit = ($scope.Open.Report.Current.Attr.SSB ? '受淹城市' : '表6') + ' 表地区必须选择';
                                        } else {
                                            exit = ($scope.Open.Report.Current.Attr.SSB ? '受淹城市' : '表6') + ' 城市名称必须选择';
                                        }*/
                                        exit = this.Message;
                                        return false;
                                    }
                                });

                                $timeout(function() {
                                    if (exit) {
                                        Alert(exit, 3000);
                                    } else if (confirm("报表未通过校核，确定保存么？")) {
                                        fn();
                                    }
                                });
                            } else {
                                fn();
                            }
                        },
                        ViewRiverData: function() {
                            var riverData = undefined;

                            if ($scope.Open.Report.Current.Attr.RiverData && $scope.Open.Report.Current.Attr.RiverData.length > 0) {
                                riverData = $scope.Open.Report.Current.Attr.RiverData;
                            } else {
                                riverData = $scope.Fn.Ajax("Index/GetRiverPageNOByPageNO", { pageNO: $scope.Open.Report.Current.ReportTitle.PageNO }).Distribute;
                                $scope.Open.Report.Current.Attr.RiverData = riverData;
                            }

                            if (riverData.length > 0) {
                                $scope.Dialog.Config({
                                    Layout: "DIV",
                                    Message: "",
                                    Title: "流域数据",
                                    Class: "div",
                                    Content: $scope.Open.Report.Current.Attr.RiverData,
                                    CallBack: function(pageno, river_code) {
                                        $scope.Fn.ViewUnderRpt({
                                            ORD_Code: $scope.Open.Report.Current.ReportTitle.ORD_Code,
                                            SourceType: $scope.Open.Report.Current.ReportTitle.SourceType,
                                            Limit: $scope.BaseData.Unit.Local.Limit,
                                            UnitCode: $scope.BaseData.Unit.Local.UnitCode,
                                            PageNO: pageno,
                                            queryUnderUnits: 1,
                                            isRiverRpt: true,
                                            RiverCode: river_code
                                        });
                                        //$scope.Dialog.Attr.Show = false;
                                    }
                                });
                            } else {
                                Alert("该报表没有流域数据");
                            }
                        },
                        SSB: function() {
                            if ($scope.Open.Report.Current.Attr.SSB) {
                                var hl011 = $scope.Fn.GetEvt().scope().hl011;
                                $scope.Open.Report.Current.HL014[$scope.Open.Report.Fn.Other.UnitIndex(hl011.UnitCode)].XYZYQT = hl011.ZYRK;
                                /*$scope.Fn.GetEvt().data('NoCallUnitEntity', true);
                                $scope.Fn.GetEvt().data('NoCheck', true);*/
                                $scope.Fn.GetEvt().data('disabledCallBack', ["UnitEntity", "RelationCheck"]);
                                $scope.Open.Report.Fn.Core.Sum(["HL014", "XYZYQT"]);
                            }
                        },
                        Max: function(fixed, arr, not_hj) {
                            try {
                                var max = 0;
                                arr = arr ? arr : $scope.Fn.GetEvt().attr("ng-model").split(".");
                                var field = arr[1];
                                fixed = fixed ? fixed : 0;
                                arr = $scope.Open.Report.Current[arr[0].toUpperCase()];

                                if ($scope.SysUserCode == "33" && $scope.BaseData.Unit.Local.Limit == 4 && ["1", "2"].In_Array($scope.Open.Report.Current.ReportTitle.SourceType) && !$scope.Fn.GetEvt().is("[ng-blur=FCM(1)]")) {
                                    $.each($scope.Open.Report.Current.HL013[1].Max, function() {
                                        if (this[field] > Number(max)) {
                                            max = parseFloat(this[field]).toFixed(fixed);
                                        }
                                    });
                                    $scope.Open.Report.Current.HL013[1][field] = Number(max) > 0 ? max : undefined;
                                } else {
                                    $.each(arr, function(i) {
                                        if (Number(this[field]) > 0 && i > 0) {
                                            this[field] = parseFloat(this[field]).toFixed(fixed);
                                            this[field] = Number(this[field]) > 0 ? this[field] : undefined;
                                            if (Number(this[field]) > Number(max)) {
                                                max = this[field];
                                            }
                                        }
                                    });
                                }

                                if (!not_hj) {
                                    arr[0][field] = Number(max) > 0 ? max : undefined;
                                }
                            } catch (e) {
                                throw e;
                            }
                        }
                    },
                    HP01: {
                        Sum: {
                            Reservoir: function() {
                                $scope.Fn.GetEvt().data('target', $scope.Fn.GetEvt());
                                var obj = $scope.Fn.GetEvt().data('target').scope().hp012;
                                var tmp = obj.UnitCode; //水库代码
                                var field = $scope.Fn.GetEvt().data('target').attr("ng-model").split(".")[1];
                                var $element = undefined;
                                var val = undefined;
                                var fn = angular.noop;
                                var scope = undefined;
                                var arr = [];
                                var name = undefined;
                                var kysl = {
                                    field: undefined,
                                    val: undefined
                                };  //大型水库可用水量  DXKKYSL

                                if ($scope.Open.Report.Current.Attr.TableIndex == 2) {
                                    name = "Large";
                                    kysl.field = "DXKKYSL";
                                } else {
                                    name = "Middle";
                                    kysl.field = "ZXKKYSL";
                                }

                                tmp = $scope.BaseData.Reservoir.RSC_UC[name][tmp]; //水库所在的单位的单位代码

                                $.each($scope.BaseData.Reservoir.RSC_UC[name], function(key, val) {
                                    if (val == tmp) {
                                        arr.push(key);
                                    }
                                });

                                $.each($scope.Open.Report.Current.HP012.Real[name], function(i) {
                                    if (i > 0 && arr.In_Array(this.UnitCode)) {
                                        val = App.Tools.Calculator.Addition(this[field], val);
                                        kysl.val = App.Tools.Calculator.Addition(this["DXKKYS"], kysl.val);
                                    }
                                });

                                tmp = $scope.Open.Report.Fn.Other.UnitIndex(tmp);

                                if (field == "DXKXXSL") {
                                    field = Number(obj.DistributeRate) == 1 ? "DZKXXSL" : "ZZKXXSL"; // 大型水库|实际蓄水   中型水库|实际蓄水
                                } /* else {   //可用水量只读
                                    field = Number(obj.DistributeRate) == 1 ? "DXKKYSL" : "ZXKKYSL"; // 大型水库|可用水量   中型水库|可用水量
                                }*/

                                $element = $("#HP011-1 tbody tr:eq(" + tmp + ") input[ng-model='hp011." + field + "']");
                                obj = $element.scope().hp011;
                                obj[field] = val;
                                obj[kysl.field] = kysl.val;
                                $scope.Fn.GetEvt().data('target', $element);
                                /*$scope.Fn.GetEvt().data('NoCallUnitEntity', true);
                                $scope.Fn.GetEvt().data('NoCheck', true);*/
                                $scope.Fn.GetEvt().data('disabledCallBack', ["UnitEntity", "RelationCheck"]);
                                scope = $element.scope();

                                scope.FCS(['hp011', kysl.field], 0, 2);
                                scope.FCRC('XZKYSL');
                                scope.FCS(0, 0, 2);
                                scope.FCRC('XXSLZJ');
                                scope.FCRC('XZYBFB', scope.Divide(obj.XXSLZJ, obj.YXSLZJ) * 100, '%');

                                switch (field) {
                                case "DZKXXSL":
                                    fn = function() {
                                        scope.FCRC('KXZYBFB', scope.Divide(obj.DZKXXSL, obj.DZKYXSL) * 100, '%');
                                    };
                                    break;
                                case "ZZKXXSL":
                                    fn = function() {
                                        scope.FCRC('ZXZYBFB', scope.Divide(obj.ZZKXXSL, obj.ZZKYXSL) * 100, '%');
                                    };
                                    break;
                                }

                                fn();
                                $scope.Open.Report.Fn.Core.HP01.OnChange.XXSLZJ([obj.UnitCode]);

                                $scope.Fn.GetEvt().removeData('target');
                                $scope.Fn.GetEvt().removeData('disabledCallBack');
                            }
                        },
                        Save: function() {
                            $scope.Open.Report.Current.Attr.CheckErrors = {};
                            var checkResult = $rootScope.Open.Report.Fn.Core.RelationCheck(0, 0, 0, 0, 1);
                            if ($.isEmptyObject(checkResult)) {
                                $rootScope.Open.Report.Fn.Core.Save();
                                $rootScope.New.SameReport.Search();
                            } else {
                                Alert("报表未通过校核，请修改后再保存", 2000);
                                $scope.Open.Report.Current.Attr.CheckErrors = checkResult;
                                $scope.Open.Report.Current.Attr.ReportFooter = "CheckAlert";
                            }
                        },
                        OnChange: {
                            XXSLZJ: function(unitCodeArr) {
                                var arr = [];
                                var index = undefined;
                                var hp01 = undefined;
                                var left = undefined;
                                var right = undefined;
                                unitCodeArr = unitCodeArr ? unitCodeArr : [$scope.Fn.GetEvt().scope().hp011.UnitCode];

                                if (!unitCodeArr.In_Array(baseData.Unit.Local.UnitCode)) {
                                    unitCodeArr.push(baseData.Unit.Local.UnitCode);
                                }

                                if (!$.isEmptyObject($scope.BaseData.Reservoir.SQXSL)) {
                                    arr.push({ Left: 'SQXSL', Right: 'SQZJ' });
                                }

                                if (!$.isEmptyObject($scope.BaseData.Reservoir.LNTQXSL)) {
                                    arr.push({ Left: 'LNTQXSL', Right: 'LNZJ' });
                                }

                                angular.forEach(arr, function(obj) {
                                    $.each(unitCodeArr, function(i, unitcode) {
                                        index = $scope.Open.Report.Fn.Other.UnitIndex(unitcode);
                                        hp01 = $scope.Open.Report.Current.HP011[index];
                                        left = Number(hp01.XXSLZJ);
                                        right = Number($scope.BaseData.Reservoir[obj.Left][unitcode]);
                                        if (left > 0 && right > 0) {
                                            hp01[obj.Right] = (left / right * 100).toFixed(2);
                                            hp01[obj.Right] = Number(hp01[obj.Right]);
                                            hp01[obj.Right] = hp01[obj.Right] > 0 ? hp01[obj.Right] : undefined;
                                        } else { //被除数或除数为0的话，结果为0
                                            hp01[obj.Right] = undefined;
                                        }
                                    });
                                });
                            }
                        }
                    },
                    Open: function(obj) {
                        loading.run();
                        obj.digest = obj.digest == undefined ? true : obj.digest;
                        var exit = undefined;
                        var index = -1;
                        exit = $rootScope.Open.Report.Opened.some(function(rpt, i) {
                            if (rpt.ReportTitle.PageNO == obj.pageno) {
                                index = i;
                                return true;
                            }
                        });

                        if (exit) {
                            $rootScope.Open.Report.Current = $rootScope.Open.Report.Opened[index];
                        } else {
                            var report = $rootScope.Fn.Ajax("Index/OpenReport", {
                                rptType: obj.rptType,
                                sourceType: obj.sourceType,
                                pageno: obj.pageno
                            });
                            report.Attr = {
                                TableIndex: 0,
                                DelAffixTBNO: [],
                                DelAffixURL: [],
                                ReportState: "Opened",
                                ReportFooter: "ReportTitle",
                                CheckErrors: {},
                                AggAcc: {
                                    Content: [],
                                    Selected: undefined
                                },
                                Notice: obj.notice ? obj.notice : undefined
                            };

                            if (report.SourceReport) {
                                report.Attr.AggAcc.Content = angular.copy(report.SourceReport);
                            }
                            delete report.SourceReport;
                            report.ReportTitle.Remark = typeof(report.ReportTitle.Remark) == "string" ? report.ReportTitle.Remark.replaceAll("{/r-/n}", "\n") : undefined;
                            var arr = undefined;
                            var tmpObj = {};
                            var tmpArr = [];

                            angular.forEach(report.Attr.AggAcc.Content, function(obj) { //Start  剔除重复值时要去掉
                                if (obj.id) {
                                    tmpArr.push(obj);
                                }
                            });
                            report.Attr.AggAcc.Content = angular.copy(tmpArr);
                            tmpArr.splice(0); //End  剔除重复值时要去掉

                            switch (report.ReportTitle.ORD_Code) {
                            case "HL01":
                                report.Attr.SSB = false;
                                if (parseInt(report.ReportTitle.StatisticalCycType) == 0) {
                                    report.Attr.SSB = true;
                                    report.Attr.TableIndex = 8;
                                }
                                report.Attr.HB = false;
                                if (parseInt(report.ReportTitle.SendOperType) == 2) {
                                    report.Attr.HB = true;
                                }
                                arr = ["HL012", "HL013"];
                                for (var i in arr) {
                                    if (Object.hasOwnProperty.call(arr, i)) {
                                        $.each(report[arr[i]], function() {
                                            this.Checked = false;
                                            this.RiverSelect = $rootScope.Open.Report.Fn.Comm.ToRiverArr(this.RiverCode);
                                            if (typeof(this.BZ) == "string") {
                                                this.BZ = this.BZ.replaceAll("{/r-/n}", "\n");
                                            }
                                        });
                                    }
                                }
                                if (report.HL013.length == 0) {
                                    report.HL013.push($.extend(App.Models.HL.HL01.HL013(), { DW: "合计" }));
                                    if ($scope.SysUserCode == "33" && $scope.BaseData.Unit.Local.Limit == 4) { //浙江县级表六只有一行
                                        report.HL013.push($.extend(App.Models.HL.HL01.HL013(), { DW: $scope.BaseData.Unit.Local.UnitName }));
                                        if($.isEmptyObject(report.HL011.Find("DW","合计"))){  //浙江县级HL011可能不存在合计行(提示：县本级，SZFWC)
                                            report.HL011.InsertAt(0,$.extend(App.Models.HL.HL01.HL011.Object(), { DW: '合计' }));
                                        }
                                    }
                                }
                                if ($scope.SysUserCode == "33" && $scope.BaseData.Unit.Local.Limit == 4 && ["1", "2"].In_Array(report.ReportTitle.SourceType)) {
                                    report.HL013[1].Max = report.MAX;
                                    delete report.MAX;
                                }
                                arr = ["HL011", "HL014"];  //, "Delta.HL011", "Delta.HL014"

                                break;
                            case "HP01":
                                arr = ["HP011"];
                                report.HP012.Fake = {};
                                angular.forEach(["Large", "Middle"], function(key) {
                                    if (report.HP012.Real[key].length > 0) {
                                        tmpArr.splice(0);
                                        tmpArr.push(report.HP012.Real[key][0]);
                                        tmpObj = {};
                                        angular.forEach($rootScope.BaseData.Reservoir[key], function(obj) {
                                            tmpObj = report.HP012.Real[key].Find("DXSKMC", obj.DXSKMC);
                                            if ($.isEmptyObject(tmpObj)) {
                                                tmpObj = App.Models.HP.HP01.HP012.Object((key == "Large" ? "1" : "2"), {
                                                    UnitCode: obj.UnitCode,
                                                    DXSKMC: obj.DXSKMC
                                                });
                                            }
                                            tmpArr.push(tmpObj);
                                        });
                                        report.HP012.Real[key] = angular.copy(tmpArr);
                                    } else {
                                        report.HP012.Real[key] = App.Models.HP.HP01.HP012.Real[key]($rootScope);
                                    }
                                });
                                report.HP012.Fake.Large = App.Models.HP.HP01.HP012.Fake.Large(report.HP012.Real.Large, $rootScope.BaseData.Unit.Unders.length);
                                report.HP012.Fake.Middle = App.Models.HP.HP01.HP012.Fake.Middle(report.HP012.Real.Middle, $rootScope.BaseData.Unit.Unders.length);
                                break;
                            }

                            var rptType = report.ReportTitle.ORD_Code;
                            var _this = undefined;
//                            var fn = function(key, arr) {
//                                if (key.indexOf(".") < 0) {
//                                    return {
//                                        arr: arr[key],
//                                        key: key
//                                    };
//                                } else {
//                                    angular.forEach(key.split("."), function(name) {
//                                        arr = arr[name];
//                                        if ($.isArray(arr)) {
//                                            return false;
//                                        }
//                                    });
//                                    return ;
//                                }
//                            };  //Key有问题  差值表
                            angular.forEach(arr, function(key) {
                                tmpArr.splice(0);
                                _this = report[key];
                                //_this = fn(key, report);  差值表
                                if (_this.length == 0) {
                                    tmpArr = App.Models[rptType.slice(0, 2)][rptType][key].Array();
                                } else {
                                    tmpArr.push(_this[0]);
                                    _this.splice(0, 1);
                                    angular.forEach($rootScope.BaseData.Unit.Unders, function(unit) {
                                        exit = false;
                                        exit = _this.some(function(object, i) {
                                            if (unit.UnitCode == object.UnitCode) {
                                                tmpArr.push(object);
                                                _this.splice(i, 1);
                                                return true;
                                            }
                                        });
                                        if (!exit) {
                                            tmpArr.push(App.Models[rptType.slice(0, 2)][rptType][key].Object(unit.UnitCode, unit.UnitName));
                                        }
                                    });
                                }
                                //_this = angular.copy(tmpArr);
                                report[key] = angular.copy(tmpArr);
                            });

                            $rootScope.Open.Report.Opened.push(report);
                            $rootScope.Open.Report.Current = report;

                            switch (obj.rptType) {
                            case "HP01":
                                var tmp = undefined;
                                tmp = $scope.Fn.Ajax("Index/GetHPLastYearData", {
                                    startDateTime: $scope.Open.Report.Current.ReportTitle.StartDateTime,
                                    endDateTime: $scope.Open.Report.Current.ReportTitle.EndDateTime
                                });
                                $scope.BaseData.Reservoir.QNTQDXS = tmp.LNTQ;
                                $scope.BaseData.Reservoir.SQXSL = tmp.SQXSL;
                                $scope.BaseData.Reservoir.LNTQXSL = tmp.LNTQXSL;
                                break;
                            case "HL01":
                                $scope.Open.Report.Current.HL011 = $scope.Open.Report.Current.HL011.RemoveBy("DW", "全区/县");
                                $scope.Open.Report.Current.HL014 = $scope.Open.Report.Current.HL014.RemoveBy("DW", "全区/县");
                                break;
                            case "NP01":
                                switch ($scope.BaseData.Unit.Local.Limit) {
                                    case 2:
                                        $scope.Open.Report.Fn.Core.NP01.Sort();
                                        if (report.HJ) {
                                            var hj = $.extend(App.Models.NP.NP01.NP011.Object(), report.HJ);
                                            $scope[$scope.Attr.NameSpace].Report.Current.NP011.InsertAt(0, hj);
                                            delete report.HJ;
                                        }
                                        break;
                                    case 3:
                                        if ($scope.Open.Report.Current.NP011.length > 0) {  //市级目前只能根据“已填过的水库的数量”来判断下级是否已全部填过
                                            /*var length = 0;
                                            $.each($scope.BaseData.Reservoir, function() {
                                                length += this.Unders.length;
                                            });
                                            if ($scope.Open.Report.Current.NP011.length < length) {
                                                $scope.Open.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array(false, $scope.Open.Report.Current.NP011);
                                            } else {
                                                $scope.Open.Report.Fn.Core.NP01.Sort();
                                            }*/
                                            $scope.Open.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array(false, $scope.Open.Report.Current.NP011);
                                        } else {
                                            $scope.Open.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array();
                                        }

                                        if (report.HJ) {
                                            hj = $.extend(App.Models.NP.NP01.NP011.Object(), report.HJ);
                                            $scope[$scope.Attr.NameSpace].Report.Current.NP011.InsertAt(0, hj);
                                            delete report.HJ;
                                        }
                                        break;
                                    case 4:
                                        if ($scope.Open.Report.Current.NP011.length > 0) {
                                            //$scope.Open.Report.Fn.Core.NP01.Sort();  //水库信息没有变动后，可取消此注释
                                            $scope.Open.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array(false, $scope.Open.Report.Current.NP011); //打开时，依然检查有没有新的水库
                                        } else {
                                            $scope.Open.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array();
                                        }
                                        break;
                                }
                            }
                        }

                        if (obj.digest) {
                            $rootScope.$apply();
                        }
                    },
                    Print: function() {
                        if ($scope.Open.Report.Current.ReportTitle.ORD_Code == 'HL01') {
                            window.ReportTitle = $scope.Open.Report.Current.ReportTitle;
                            window.open('Index/Print', '', '', '');
                        } else {
                            Alert("暂不支持蓄水在线打印功能，请导出Excel之后，使用Excel打印");
                        }
                    },
                    Private: {
                        "33": {
                            Save: function() {
                                var report = angular.copy($scope.Open.Report.Current);

                                if ($scope.BaseData.Unit.Local.Limit == 4) { //浙江县级表六固定只有一行
                                    var exceptFields = ["$$hashKey", "Checked", "DW", "DistributeRate", "RiverCode", "RiverSelect", "SourcePageNo", "UnitCode", "GCJSSJ", "PageNO", "TBNO","DataOrder"];
                                    var isEmpty = true;

                                    $.each(report.HL013[1], function(field, value) {
                                        if (!exceptFields.In_Array(field)) {
                                            if (value != undefined && value != "" && Number(value) > 0) {
                                                isEmpty = false;
                                                return false;
                                            } else if (field == "CSMC" && typeof value == "string" && value.trim().length) {
                                                isEmpty = false;
                                                return false;
                                            }
                                        }
                                    });

                                    //delete $scope.Open.Report.Current.HL013[1].Max;
                                    
                                    if (isEmpty) {
                                        report.HL013.splice(1);

                                        /*report.HL011[0]["SYCS"] = undefined;
                                        report.HL011[1]["SYCS"] = undefined;
                                        exceptFields = ["PageNO", "TBNO", "DW", "DataOrder", "DistributeRate", "RiverCode", "UnitCode", "SZFWZ", "QXHJ", "ZJXJ"];
                                        isEmpty = true;
                                        $.each($.extend(angular.copy(report.HL011[1]), report.HL014[1]), function(field,val) {
                                            if (!exceptFields.In_Array(field) && Number(val) > 0) {
                                                isEmpty = false;
                                                return false;
                                            }
                                        });
                                        if (isEmpty) {
                                            report.HL011[1]["SZFWZ"] = undefined;
                                            report.HL011[0]["SZFWZ"] = App.Tools.Calculator.Subtraction(report.HL011[0]["SZFWZ"], 1);
                                        }*/
                                    }
                                }
                                $scope.Open.Report.Fn.Core.HL01.Save(report);
                            },
                            AggAcc: {
                                Sum: function(rpt) {
                                    if (rpt.HL013.length > 0 && baseData.Unit.Local.Limit == 4) {
                                        var obj = undefined;
                                        var report = $scope.Open.Report.Current;
                                        var exceptFields = ["DW", "DistributeRate", "RiverCode", "SourcePageNo", "UnitCode", "GCJSSJ", "CSMC", "GCYMLS", "ZYZJZDSS", "GCLJJYL"];
                                        var max = false;

                                        if (["1", "2"].In_Array(report.ReportTitle.SourceType)) {
                                            max = {
                                                GCYMLS: {},
                                                ZYZJZDSS: {},
                                                GCLJJYL: {}
                                            };
                                        }

                                        $.each(rpt.HL013, function() {
                                            obj = this;
                                            $.each(obj, function(field, value) {
                                                if (!exceptFields.In_Array(field) && Number(value) > 0) {
                                                    report.HL013[0][field] = App.Tools.Calculator.Addition(report.HL013[0][field], value);
                                                    report.HL013[1][field] = App.Tools.Calculator.Addition(report.HL013[1][field], value);
                                                }
                                            });

                                            angular.forEach(["GCYMLS", "ZYZJZDSS", "GCLJJYL"], function(field) {
                                                obj.tocompare = Number(obj[field]) > 0 ? Number(obj[field]) : 0;

                                                if (max) {
                                                    max[field][obj.PageNO] = max[field][obj.PageNO] == undefined ? 0 : max[field][obj.PageNO];
                                                    if (obj.tocompare > max[field][obj.PageNO]) {
                                                        max[field][obj.PageNO] = obj.tocompare;
                                                    }
                                                }

                                                obj.compare = Number(report.HL013[0][field]) > 0 ? Number(report.HL013[0][field]) : 0;
                                                if (obj.tocompare > obj.compare) {
                                                    report.HL013[0][field] = obj[field];
                                                    report.HL013[1][field] = obj[field];
                                                }
                                            });
                                        });

                                        if (["1", "2"].In_Array(report.ReportTitle.SourceType)) {
                                            angular.forEach(["GCYMLS", "ZYZJZDSS", "GCLJJYL"], function(field) {
                                                $.each(max[field], function(pageno, value) {
                                                    if (value > 0) {
                                                        obj = report.HL013[1].Max.Find("PageNO", pageno); //之前可能存在
                                                        if ($.isEmptyObject(obj)) {
                                                            obj = { PageNO: pageno };
                                                            obj[field] = value;
                                                            report.HL013[1].Max.push(obj);
                                                        } else {
                                                            obj[field] = value;
                                                        }
                                                    }
                                                });
                                            });
                                        }

                                        rpt.HL013.splice(0);
                                    }
                                },
                                Sub: function(obj) {
                                    if (obj.report.HL013.length > 0 && baseData.Unit.Local.Limit == 4) {
                                        var model = obj.report.HL013[0];
                                        var report = $scope.Open.Report.Current;
                                        var exceptFields = ["DW", "DistributeRate", "RiverCode", "SourcePageNo", "UnitCode", "GCJSSJ", "CSMC", "GCYMLS", "ZYZJZDSS", "GCLJJYL"]; //淹没历时、最大水深
                                        $.each(model, function(field, value) {
                                            if (!exceptFields.In_Array(field) && Number(value) > 0) {
                                                report.HL013[0][field] = App.Tools.Calculator.Subtraction(report.HL013[0][field], value);
                                                report.HL013[1][field] = App.Tools.Calculator.Subtraction(report.HL013[1][field], value);
                                            }
                                        });

                                        report.HL013[1].Max = report.HL013[1].Max.RemoveBy("PageNO", obj.pageno);
                                        obj.report.HL013.splice(0);
                                    }
                                }
                            },
                            SelectDW: function(unit) {
                                var tmp = $scope.Open.Report.Fn.Other.UnitIndex;
                                if ($scope.Fn.GetEvt()[0].checked) {
                                    $scope.Open.Report.Current.HL011[tmp(unit.UnitCode)].SYCS = 1;
                                    $scope.Open.Report.Current.HL011[0].SYCS = 1;

                                    if (typeof $scope.Open.Report.Current.HL013[1].CSMC == "string") {
                                        $scope.Open.Report.Current.HL013[1].CSMC += unit.UnitName + ";";
                                    } else {
                                        $scope.Open.Report.Current.HL013[1].CSMC = unit.UnitName + ";";
                                    }
                                } else {
                                    $scope.Open.Report.Current.HL011[tmp(unit.UnitCode)].SYCS = undefined;
                                    tmp = 0;
                                    $.each($scope.Open.Report.Current.HL011, function(i) {
                                        if (i > 0) {
                                            tmp = tmp + App.Tools.Calculator.Number(this.SYCS);
                                        }
                                    });
                                    if (tmp > 0) {
                                        $scope.Open.Report.Current.HL011[0].SYCS = 1;
                                    } else {
                                        $scope.Open.Report.Current.HL011[0].SYCS = undefined;
                                    }

                                    $scope.Open.Report.Current.HL013[1].CSMC = $scope.Open.Report.Current.HL013[1].CSMC.replace(unit.UnitName + ";", "");
                                }
                                //$scope.Fn.TabSearch.StopPro(arguments[0]);
                            },
                            Limit: {
                                "4": function() {
                                    if ($scope.Open.Report.Current.ReportTitle.SourceType == 2) {
                                        var count = 0;
                                        $.each($scope.Open.Report.Current.HL011, function(i) {
                                            if (i > 0) {
                                                count += App.Tools.Calculator.Number(this.SYCS);
                                            }
                                            if (count > 0) {
                                                return false;
                                            }
                                        });
                                        if (count > 0) {
                                            $scope.Open.Report.Current.HL011[0]["SYCS"] = 1;
                                        } else {
                                            $scope.Open.Report.Current.HL011[1]["SYCS"] = undefined;
                                        }
                                    }
                                }
                            },
                            Check: function(obj) {
                                if ($scope.BaseData.Unit.Local.Limit == 4) {
                                    obj.type = obj.type == undefined ? "Save" : obj.type;
                                    switch (obj.type) {
                                    case "Save":
                                        if (obj.report.HL013.length > 1) { //浙江县级表6城市名称必须选择
                                            if (Number(obj.report.HL011[0].SYCS) > 0) {
                                                delete obj.rptErrors["HL013-Select-CSMC"];
                                            } else {
                                                if (!obj.rptErrors["HL013-Select-CSMC"]) {
                                                    obj.rptErrors["HL013-Select-CSMC"] = {
                                                        TableIndex: 5,
                                                        TRIndex: {},
                                                        Message: "表6：必须选择城市名称",
                                                        Selected: false
                                                    };
                                                }
                                            }
                                        }
                                        if (!$scope.Open.Report.Attr.SSB) {
                                            $.each($scope.Open.Report.Current.HL011, function(i) {
                                                if (i > 0) {
                                                    if (Number(this.SHMJXJ) > 0 && Number(this.YZJCLS ? this.YZJCLS : 0) <= 0) {
                                                        if (!obj.rptErrors[this.UnitCode + "-YZJCLS"]) {
                                                            obj.rptErrors[this.UnitCode + "-YZJCLS"] = {
                                                                TableIndex: 1,
                                                                Message: "表2 " + this.DW + "：因减产粮食应 > 0",
                                                                Selected: false
                                                            };
                                                        }
                                                    } else {
                                                        delete obj.rptErrors[this.UnitCode + "-YZJCLS"];
                                                    }
                                                }
                                            });
                                        }
                                        break;
                                    case "AggAcc":
                                        if (Number($scope.Open.Report.Current.HL011[0].SYCS) > 0) {
                                            $scope.Open.Report.Current.HL013[1].CSMC = "";
                                            $.each($scope.Open.Report.Current.HL011, function(i) {
                                                if (i > 0 && Number(this.SYCS) > 0) {
                                                    $scope.Open.Report.Current.HL013[1].CSMC += this.DW + ";";
                                                }
                                            });
                                        } else {
                                            $scope.Open.Report.Current.HL013[1].CSMC = undefined;
                                        }
                                        break;
                                    }
                                }
                            }
                        },
                        Call: function(fnName, queryExcute, param) { //Sum
                            var fn = angular.noop; //namespace

                            if (angular.isObject(this[$scope.SysUserCode])) {
                                fn = this[$scope.SysUserCode];
                                angular.forEach(fnName.split("."), function(name) {
                                    fn = fn[name];
                                    if ($.isFunction(fn)) {
                                        fn(param);
                                    }
                                });
                            } else if (!queryExcute) { //queryExcute = false(默认) 只查询并执行各个省份重写的方法
                                fn = $scope.Open.Report.Fn.Core[$scope.Open.Report.Current.ReportTitle.ORD_Code];
                                angular.forEach(fnName.split("."), function(name) {
                                    fn = fn[name];
                                    if ($.isFunction(fn)) {
                                        fn(param);
                                    }
                                });

                                if (!$.isFunction(fn)) { //还未找到,就去Core Fn中去找
                                    fn = $scope.Open.Report.Fn.Core;
                                    angular.forEach(fnName.split("."), function(name) {
                                        fn = fn[name];
                                        if ($.isFunction(fn)) {
                                            fn(param);
                                        }
                                    });
                                }
                            }
                        }
                    },
                    ReCompute: function(unitCodeArr, rptType) {
                        if (rptType == "HP01") {
                            var arr = $scope.Open.Report.Current.HP011;
                            $.each(arr, function() {
                                if (unitCodeArr[0].In_Array(this.UnitCode)) { //如果该条记录需要重新计算
                                    //计算蓄水表一
                                    if (Number(this.DZKYXSL) > 0) {
                                        this.KXZYBFB = (this.DZKXXSL ? this.DZKXXSL : 0) / this.DZKYXSL * 100; //实际占计划 = 实际蓄水/计划蓄水
                                        this.KXZYBFB = this.KXZYBFB.toFixed(2); //百分数保留2位
                                        this.KXZYBFB = Number(this.KXZYBFB) > 0 ? this.KXZYBFB : undefined;
                                    }
                                    if (Number(this.YXSLZJ) > 0) {
                                        this.XZYBFB = (this.XXSLZJ ? this.XXSLZJ : 0) / this.YXSLZJ * 100; //（总计）实际占计划 = 实际蓄水/计划蓄水
                                        this.XZYBFB = this.XZYBFB.toFixed(2); //百分数保留2位
                                        this.XZYBFB = Number(this.XZYBFB) > 0 ? this.XZYBFB : undefined;
                                    }
                                    if (Number(this.ZZKYXSL)) {
                                        this.ZXZYBFB = (this.ZZKXXSL ? this.ZZKXXSL : 0) / this.ZZKYXSL * 100; //（总计）实际占计划 = 实际蓄水/计划蓄水
                                        this.ZXZYBFB = this.ZXZYBFB.toFixed(2); //百分数保留2位
                                        this.ZXZYBFB = Number(this.ZXZYBFB) > 0 ? this.ZXZYBFB : undefined;
                                    }
                                    //计算蓄水表二
                                    if (Number(this.XYKYXS) > 0) {
                                        this.XKXZYBFB = (this.XYKXXS ? this.XYKXXS : 0) / this.XYKYXS * 100; //（总计）实际占计划 = 实际蓄水/计划蓄水
                                        this.XKXZYBFB = this.XKXZYBFB.toFixed(2); //百分数保留2位
                                        this.XKXZYBFB = Number(this.XKXZYBFB) > 0 ? this.XKXZYBFB : undefined;
                                    }
                                    if (Number(this.XRKYXS) > 0) {
                                        this.XRXZYBFB = (this.XRKXXS ? this.XRKXXS : 0) / this.XRKYXS * 100; //（总计）实际占计划 = 实际蓄水/计划蓄水
                                        this.XRXZYBFB = this.XRXZYBFB.toFixed(2); //百分数保留2位
                                        this.XRXZYBFB = Number(this.XRXZYBFB) > 0 ? this.XRXZYBFB : undefined;
                                    }
                                    if (Number(this.SPTYXS) > 0) {
                                        this.TXZYBFB = (this.SPTXXS ? this.SPTXXS : 0) / this.SPTYXS * 100; //（总计）实际占计划 = 实际蓄水/计划蓄水
                                        this.TXZYBFB = this.TXZYBFB.toFixed(2); //百分数保留2位
                                        this.TXZYBFB = Number(this.TXZYBFB) > 0 ? this.TXZYBFB : undefined;
                                    }
                                }
                            });
                            arr = $scope.Open.Report.Current.HP012.Fake.Large;

                            if (Number(baseData.Unit.Local.Limit) > 2) { //省级只有前3张表
                                arr = arr.concat($scope.Open.Report.Current.HP012.Fake.Middle);
                            }

                            $.each(arr, function() {
                                $.each(this, function() {
                                    if (unitCodeArr[1].In_Array(this.UnitCode)) { //如果该条记录需要重新计算
                                        if (Number(this.DXKYXSL) > 0) {
                                            this.DXZYBFB = (this.DXKXXSL ? this.DXKXXSL : 0) / this.DXKYXSL * 100; //实际占计划 = 实际蓄水/计划蓄水
                                            this.DXZYBFB = this.DXZYBFB.toFixed(2); //百分数保留2位
                                            this.DXZYBFB = Number(this.DXZYBFB) > 0 ? this.DXZYBFB : undefined;
                                        }
                                    }
                                });
                            });

                            $scope.Open.Report.Fn.Core.HP01.OnChange.XXSLZJ(unitCodeArr[0]);
                        }
                    },
                    RelationCheck: function(leftField, rightVal, sumType, fixed, formulaType, report) {
                        var obj = undefined;
                        var right = undefined;
                        var arr = undefined;
                        var field = undefined;
                        report = angular.isObject(report) ? report : $scope.Open.Report.Current;
                        if (formulaType) { //true的话为非等式校核，默认等式校核
                            var rptErrors = undefined;
                            var uIndex = $scope.Open.Report.Fn.Other.UnitIndex;
                            var left = undefined;
                            var measureValue = undefined;
                            var checkFormula = function(object, formula) {
                                left = object[formula.Left]; //默认等式左边只有一个字段
                                right = 0;
                                field = true;
                                if (formula.Right.indexOf("+") > 0) { //默认等式右边只存在加号运算符
                                    angular.forEach(formula.Right.split("+"), function(key) {
                                        right = App.Tools.Calculator.Addition(right, object[key]);
                                    });
                                } else { //默认没有加号运算就只有一个字段
                                    right = object[formula.Right];
                                }
                                left = Number(left) > 0 ? Number(left) : 0;
                                right = Number(right) > 0 ? Number(right) : 0;

                                if (baseData.Unit.Local.Limit == 2) {
                                    if (!formula.Left.Contains('+')) {
                                        if (angular.isObject($scope.BaseData.Field[formula.Left])) { //目前只能做单个运算
                                            measureValue = $scope.BaseData.Field[formula.Left].MeasureValue;
                                            left = left * (measureValue > 0 ? measureValue : 1);
                                        }
                                        if (angular.isObject($scope.BaseData.Field[formula.Right])) { //目前只能做单个运算
                                            measureValue = $scope.BaseData.Field[formula.Right].MeasureValue;
                                            right = right * (measureValue > 0 ? measureValue : 1);
                                        }
                                    }
                                }

                                switch (formula.Middle) {
                                case ">":
                                    if ((left || right) && left <= right) {
                                        field = false; //字段没有通过校核
                                    }
                                    break;
                                case ">=":
                                    if (left < right) {
                                        field = false;
                                    }
                                    break;
                                }
                            };
                            if (leftField) { //修改错误的数据之后再验证
                                if (leftField.indexOf("Select") > 0) {  //验证表5、表6的错误
                                    arr = leftField.split("-"); //HL012-0-Select
                                    delete report.Attr.CheckErrors[arr[0] + "-Select"].TRIndex[$scope.Fn.GetEvt().scope().$index];

                                    if ($.isEmptyObject(report.Attr.CheckErrors[arr[0] + "-Select"].TRIndex)) {
                                        delete report.Attr.CheckErrors[arr[0] + "-Select"];
                                    }
                                } else {  //43010100-SHMJLS
                                    arr = leftField.split("-");
                                    right = arr[0] + "-" + arr[2];
                                    field = arr[2];
                                    obj = $scope.Fn.GetEvt().data('target').scope()[arr[1].toLowerCase()];  //$scope.Open.Report.Fn.Other.CurTable.Name().toLowerCase()
                                    rptErrors = report.Attr.CheckErrors;
                                    if (angular.isObject(rptErrors[arr[0] + "-" + arr[2]])) {
                                        if (isNaN(rptErrors[arr[0] + "-" + arr[2]].Right)) {
                                            $.each(baseData.RelationCheck.Formula.Inequality, function() {
                                                if (this.Left == field && this.Right == rptErrors[arr[0] + "-" + arr[2]].Right) {
                                                    checkFormula(obj, this);
                                                    if (field) {
                                                        delete report.Attr.CheckErrors[arr[0] + "-" + arr[2]];
                                                    }
                                                }
                                            });
                                        } else if (!isNaN(rptErrors[arr[0] + "-" + arr[2]].Right) && angular.isObject(baseData.RelationCheck.Constant[arr.join("-")])) {
                                            left = obj[field];
                                            left = isNaN(left) ? 0 : parseFloat(left);
                                            if (left <= parseFloat(baseData.RelationCheck.Constant[arr.join("-")].value)) {
                                                delete report.Attr.CheckErrors[arr[0] + "-" + arr[2]];
                                            }
                                        }
                                    }
                                    //$scope.Open.Report.Fn.Core.Private.Call("Check", true, { report: report, rptErrors: rptErrors });
                                }
                            } else {  //保存时，检查需要校核的字段
                                rptErrors = {};
                                if (report.ReportTitle.ORD_Code == "HL01") { //只有洪涝表才有校核
                                    $.each(baseData.RelationCheck.Constant, function(key) {  //基础数据校核  kry:43010100-HL011-SHMJLS
                                        arr = key.split("-");
                                        if (!report[arr[1]][uIndex(arr[0])]) {
                                            return true;
                                        }
                                        field = report[arr[1]][uIndex(arr[0])][arr[2]];
                                        if (!isNaN(field)) {
                                            if (parseFloat(field) > parseFloat(this.value)) {
                                                rptErrors[arr[0] + "-" + arr[2]] = {
                                                    Selected: false,
                                                    Right: 0
                                                };
                                                right = baseData.Unit.Unders.Find("UnitCode", arr[0], "UnitName");
                                                right = typeof(right) == "object" ? "合计" : right;
                                                rptErrors[arr[0] + "-" + arr[2]].TableIndex = report.Attr.SSB ? 8 : App.Config.Field.Fn.FieldIndex("HL01." + arr[1] + "." + arr[2]);
                                                rptErrors[arr[0] + "-" + arr[2]].Message = report.Attr.SSB ? "数据表" : "表" + (rptErrors[arr[0] + "-" + arr[2]].TableIndex + 1);
                                                rptErrors[arr[0] + "-" + arr[2]].Message += " " + right + "：" + this.name + "不能大于" + this.value;
                                            }
                                        }
                                    });
                                    angular.forEach(report.HL011, function(hl011, i) {
                                        if (i > 0) {
                                            $.each(baseData.RelationCheck.Formula.Inequality, function() {
                                                if (!angular.isObject(rptErrors[hl011.UnitCode + "-" + this.Left])) { //该字段基础数据校核已通过
                                                    checkFormula(hl011, this);
                                                    if (!field) {
                                                        field = hl011.UnitCode + "-" + this.Left;
                                                        rptErrors[field] = {
                                                            Selected: false,
                                                            Right: this.Right
                                                        };
                                                        right = baseData.Unit.Unders.Find("UnitCode", hl011.UnitCode, "UnitName");
                                                        right = typeof(right) == "object" ? "合计" : right;
                                                        rptErrors[field].TableIndex = report.Attr.SSB ? 8 : App.Config.Field.Fn.FieldIndex("HL01.HL011." + this.Left);
                                                        rptErrors[field].Message = report.Attr.SSB ? "数据表" : "表" + (rptErrors[field].TableIndex + 1);
                                                        rptErrors[field].Message += " " + right + "：" + this.Message;
                                                    }
                                                }
                                            });
                                        }
                                    });
                                    angular.forEach([report.HL012, report.HL013], function(array, i) {
                                        $.each(array, function(j) {
                                            if (this.DW != "合计" && this.UnitCode == baseData.Unit.Local.UnitCode && !($scope.SysUserCode == '33' && $scope.BaseData.Unit.Local.Limit == 4 && i == 1)) { //浙江县级表六可以不选择
                                                if (!rptErrors[(i ? "HL013" : "HL012") + "-Select"]) {
                                                    rptErrors[(i ? "HL013" : "HL012") + "-Select"] = {
                                                        TableIndex: 4 + i,
                                                        TRIndex: {},
                                                        Message: (report.Attr.SSB ? "死亡/失踪人员明细" : "表" + (5 + i)) + " 必须选择地区",
                                                        Selected: false
                                                    };
                                                }
                                                rptErrors[(i ? "HL013" : "HL012") + "-Select"].TRIndex[j] = true;
                                            }
                                        });
                                    });
                                    $scope.Open.Report.Fn.Core.Private.Call("Check", true, { report: report, rptErrors: rptErrors });
                                }
                            }
                            return rptErrors;
                        } else {
                            var leftTableName = undefined;
                            var tmp = undefined;
                            var $target = $scope.Fn.GetEvt().data('target');
                            var scope = $target.scope();
                            var decimalCount = fixed == undefined ? 2 : fixed; //默认保留2位小数
                            var tableName = $target.attr("ng-model").split(".")[0].toLowerCase(); //$scope.Open.Report.Fn.Other.CurTable.Name().toLowerCase()
                            if (leftField) {
                                if (leftField.indexOf(".") > 0) { //指定hl011.ZYRK
                                    arr = leftField.split(".");
                                    leftTableName = leftField.split(".")[0];
                                    field = arr[1];
                                } else { //ZYRK
                                    field = leftField;
                                }
                            } else {
                                arr = $target.attr("ng-model").split(".");
                                field = [arr[arr.length - 1]]; //如果指定field，则指定某个字段的校核  赋值给leftField无效
                            }

                            $.each(baseData.RelationCheck.Formula.Equality, function() {
                                obj = this;
                                if (obj.Left.trim() == field) {
                                    if (obj.Right.indexOf("COUNT") == -1) { //忽略表5、表6的Count
                                        if (!isNaN(rightVal) && rightVal != '') { //指定右值，用于直接算出较复杂的运算
                                            right = rightVal == 0 ? undefined : rightVal.toFixed(decimalCount);
                                            right = (!isNaN(right) && parseFloat(right) == 0) ? undefined : right; //保留指定的小数位数后判断是否为0
                                            scope[tableName][obj.Left] = right;
                                        } else { //未指定右值，右边为简单的运算

                                            if (obj.Right.indexOf("+") > 0) { //处理一个或多个加号
                                                right = 0;
                                                angular.forEach(obj.Right.split("+"), function(key) {
                                                    right = App.Tools.Calculator.Addition(right, scope[tableName][key]);
                                                });
                                            } else if (obj.Right.indexOf("-") > 0) { //处理一个减号
                                                right = obj.Right.split("-");
                                                tmp = $scope.BaseData.Reservoir.SKR[scope[tableName].UnitCode]; //$(event.target).scope() scope有问题

                                                if (tmp == undefined) { //水库或行政单位不存在死库容
                                                    tmp = 0;
                                                } else {
                                                    if (tableName == "hp011") {
                                                        if (tmp[right[1]] != undefined) {
                                                            tmp = tmp[right[1]];
                                                        } else {
                                                            tmp = 0;
                                                        }
                                                    }
                                                }

                                                right = App.Tools.Calculator.Subtraction(scope[tableName][right[0]], tmp); //$(event.target).scope()

                                                if (arr && ["Large", "Middle"].In_Array(arr[0])) {
                                                    leftTableName = undefined;
                                                }
                                            } else { //简单的赋值等式 left = right
                                                right = scope[tableName][obj.Right];
                                            }

                                            if (leftTableName) {
                                                report[leftTableName][$scope.Open.Report.Fn.Other.UnitIndex(scope[tableName].UnitCode)][obj.Left] = right;
                                                tableName = leftTableName;
                                            } else {
                                                scope[tableName][obj.Left] = right;
                                            }
                                        }
                                        switch (sumType) {
                                        case '%':
                                            arr = obj.Right.replace("*100", "").split("/"); //默认：Field1/Field2*100
                                            switch (tableName.toUpperCase()) {
                                            case "HP012":
                                                obj = $target.parents(".FloatTable").find("table:first tbody tr:first").scope()[tableName];
                                                break;
                                            default:
                                                obj = $target.parents("tbody").children(":first").scope()[tableName];
                                                break;
                                            }
                                            right = App.Tools.Calculator.Division(obj[arr[0]], obj[arr[1]]) * 100;
                                            obj[this.Left] = right == 0 ? undefined : right.toFixed(2);
                                            break;
                                        default:
                                            if (arr && ["Large", "Middle"].In_Array(arr[0])) {
                                                tmp = ["Real", arr[0]];
                                            } else {
                                                tmp = 0;
                                            }

                                            $scope.Open.Report.Fn.Core.Sum([tableName.toUpperCase(), obj.Left], tmp, fixed); //算“合计列”
                                            break;
                                        }
                                    }
                                }
                            });
                        }
                    },
                    ReportState: function() {

                    },
                    Save:function(type, rpt) {
                        var temp = false;
                        var report = rpt;

                        if (type == "Copy") {
                            if (confirm("确定要另存为数据表？")) {
                                report = angular.copy($rootScope.Open.Report.Current);
				                report.ReportTitle.PageNO = 0;
                                temp = true;
                            } else {
                                return;
                            }
                        }

                        if ($scope.SysORD_Code == "HP01" && $scope.Open.Report.Current.ReportTitle.PageNO == 0 && !$scope.Open.Report.Current.Attr.Data_Changed) {  //不允许保存空表
                            Alert("不能保存空表");
                            return;
                        }

                        if (!report) {
                            report = angular.copy($rootScope.Open.Report.Current);
                        }

                        report.DelAffixTBNO = report.Attr.DelAffixTBNO.toString();
                        report.DelAffixURL = report.Attr.DelAffixURL.toString();

                        if (report.ReportTitle.ORD_Code != "NP01") {
                            report.SourceReport = angular.copy($rootScope.Open.Report.Current.Attr.AggAcc.Content);
                        }
                        
                        report.ReportTitle.Remark = typeof(report.ReportTitle.Remark) == "string" ? report.ReportTitle.Remark.replaceAll("[\r|\n]", "{/r-/n}") : undefined;
                        report.ReportTitle.UnitCode = $scope.BaseData.Unit.Local.UnitCode.slice(0, 2) + "000000";
                        delete report.Delta;  //删除差值数据
                        //report.ReportTitle.UnitName = "";
                        var arr = undefined;
                        
                        switch (report.ReportTitle.ORD_Code) {
                        case "HL01":
                            //----------------------------处理差值数据----------------------------
//                            if ($scope.Open.Report.Current.ReportTitle.SourceType > 0) { //根据差值表生成对应的ReportTitle表
//                                var prev_delta_pageno = {};
//                                $.each($scope.Open.Report.Current.Attr.AggAcc.Content, function() {
//                                    if (this.SourceType == 6) {
//                                        prev_delta_pageno[this.UnitCode] = this.id;
//                                    }
//                                });
//                                var excepfield = ['$$hashKey', 'DW', 'DataOrder', 'DistributeRate', 'RiverCode', 'UnitCode'], exist, delta_rpts = {};
//                                var fn = function(key, _this) {
//                                    if (!delta_rpts[_this.UnitCode]) { //之前是否不存在ReportTitle
//                                        delta_rpts[_this.UnitCode] = {
//                                            ReportTitle: {
//                                                PageNO: prev_delta_pageno[_this.UnitCode] || 0,
//                                                UnitCode: _this.UnitCode,
//                                                UnitName: _this.DW,
//                                                CopyPageNO: 0,
//                                                Del: 0,
//                                                SourceType: 6, //锁定表的SourceType为6
//                                                ORD_Code: 'HL01',
//                                                RPTType_Code: 'XZ0',
//                                                StatisticalCycType: $scope.Open.Report.Current.ReportTitle.StatisticalCycType,
//                                                State: 3
//                                            }
//                                        };
//                                    }

//                                    delta_rpts[_this.UnitCode][key] = delta_rpts[_this.UnitCode][key] || [];
//                                    delta_rpts[this.UnitCode][key].push(_this);
//                                };
//                                angular.forEach(['HL011', 'HL014'], function(key) {
//                                    $.each($scope.Open.Report.Current.Delta[key], function() {
//                                        exist = false;
//                                        $.each(this, function(field, val) {
//                                            if (excepfield.In_Array(field))
//                                                return true;

//                                            if (!isNaN(val)) {
//                                                exist = true;
//                                                return false;
//                                            }
//                                        });

//                                        if (exist) {
//                                            fn(key, this);
//                                        }
//                                    });
//                                });

//                                var rpts = angular.copy($scope.Open.Report.Current);
//                                rpts.HL013.splice(0, 1);
//                                angular.forEach(['HL012', 'HL013'], function(key) {
//                                    $.each(rpts[key], function(i) {
//                                        if (this.PageNO == 0) { //自行增加的
//                                            fn(key, this);
//                                        }
//                                    });
//                                });

//                                rpts = [];
//                                $.each(delta_rpts, function(code, obj) {
//                                    rpts.push(obj);
//                                });

//                                var obj = undefined;
//                                var response = $scope.Fn.Ajax('index/SaveDeltaReport', { Reports: angular.toJson(rpts) });
//                                if ($.isArray(response)) {
//                                    $.each(response, function() {
//                                        _this = this;
//                                        obj = $scope.Open.Report.Current.Attr.AggAcc.Content.Find('UnitCode', _this.UnitCode);
//                                        if ($.isEmptyObject(obj)) {  //不存在，则插入，存在，则用之前的页号
//                                            obj = {
//                                                id: _this.PageNO,
//                                                UnitCode: _this.UnitCode,
//                                                SourceType: 6
//                                            };
//                                            $scope.Open.Report.Current.Attr.AggAcc.Content.push(obj);
//                                            report.Attr.AggAcc.Content.push(obj);
//                                        }

//                                        angular.forEach(['HL012', 'HL013'], function(key) {
//                                            $.each(report[key], function(i) {
//                                                if (this.PageNO == 0 && this.UnitCode == _this.UnitCode) { //自行增加的
//                                                    this.SourcePageNo = _this.PageNO; //保存页号方便减表
//                                                    $scope.Open.Report.Current[key][i].SourcePageNo = _this.PageNO;
//                                                }
//                                            });
//                                        });
//                                    });
//                                } else {
//                                    throw response;
//                                }
//                            }
                            //----------------------------处理差值数据----------------------------
                            report["HL012"].RemoveAttr(["Checked", "RiverSelect"], function(obj) {
                                obj.BZ = typeof(obj.BZ) == "string" ? obj.BZ.replaceAll("[\r|\n]", "{/r-/n}") : undefined;
                            });
                            report["HL013"].RemoveAttr(["Checked", "RiverSelect"]);
                            report.SourceReport = angular.copy(report.Attr.AggAcc.Content);
                            if (report.HL013.length == 1) { //仅有合计行
                                report.HL013.splice(0);
                            }
                            arr = ["HL011", "HL014"];

                            if (report.Attr.DistributeRiver) {
                                report.RiverRates = angular.copy(report.Attr.DistributeRiver);
                            }

                            if (baseData.Unit.Local.Limit == 2 && report.ReportTitle.PageNO != 0) { //省级修改已报送的报表并保存后,当前报表即修改后的新表需重新请求流域报表页号信息
                                delete $scope.Open.Report.Current.Attr.RiverData;
                            }
                            break;
                        case "HP01":
                            arr = ["HP011", "HP012"];
                            report.HP012 = report.HP012.Real.Large.concat(report.HP012.Real.Middle);
                            break;
                        case "NP01":
                            report.NP011 = $.grep($scope.Open.Report.Current.NP011, function(obj) {
                                return !isNaN(obj.No);
                            });
                        }
                        delete report.Attr;

                        var needDelete = undefined;
                        var exceptFields = ["UnitCode", "RiverCode", "DataOrder", "DistributeRate", "DW", "DXSKMC"];
                        var tmpArr = [];
                        for (var j in arr) {
                            if (Object.hasOwnProperty.call(arr, j)) {
                                tmpArr.splice(0);
                                $.each(report[arr[j]], function(k, obj) {
                                    needDelete = true;
                                    $.each(obj, function(key, value) {
                                        if (!exceptFields.In_Array(key)) {
                                            if (key == "SZFWX" && angular.isObject(value)) {
                                                if (value.Fake != undefined && value.Fake == "") {
                                                    needDelete = false;
                                                }
                                            } else if (value != undefined && value != "") {
                                                needDelete = false;
                                            }
                                        }
                                    });
                                    if (!needDelete) {
                                        tmpArr.push(obj);
                                    }
                                });
                                if (tmpArr.length > 0) {
                                    report[arr[j]] = angular.copy(tmpArr);
                                } else {
                                    report[arr[j]] = [];
                                }
                            }
                        }

                        var param = angular.toJson(report);
                        if ($scope.SysORD_Code == "HL01" && $scope.SysUserCode == "45") {  //有诸如XX市本级、XX市县本级的单位
                            param = JSON.parse(param);
                            if (param.HL011.length > 0 && $.isEmptyObject(param.HL011.Find("DW", "合计"))) {
                                param.HL011 = [];
                            }
                            param = angular.toJson(param);
                        }

                        var result = $scope.Fn.Ajax("Index/SaveUpdateReport", { report: param });
                        if (!isNaN(result) || angular.isObject(result)) {
                            switch ($scope.Open.Report.Current.ReportTitle.ORD_Code) {
                            case "NP01":
                                if (!$.isEmptyObject(result.TBNO)) {
                                    $.each(result.TBNO, function(rscode, tbno) {
                                        $scope.Open.Report.Current.NP011.Find("RSCode", rscode).TBNO = tbno;
                                    });
                                }
                                result = result.PageNO;
                                break;
                            case "HL01":
                            case "HP01":
                                var sameRpt = undefined;
                                var fn = function(obj) {
                                    return $.extend(obj, {
                                        EndDateTime: $scope.Open.Report.Current.ReportTitle.EndDateTime,
                                        PageNO: result.toString(),
                                        Remark: $scope.Open.Report.Current.ReportTitle.Remark,
                                        SourceType: $scope.Open.Report.Current.ReportTitle.SourceType,
                                        StartDateTime: $scope.Open.Report.Current.ReportTitle.StartDateTime,
                                        WriterTime: $scope.Open.Report.Current.ReportTitle.WriterTime
                                    });
                                };
                                if ($scope.Open.Report.Current.ReportTitle.PageNO == 0) {
                                    $scope.New.SameReport.Content.push(fn());
                                } else {
                                    sameRpt = fn($scope.New.SameReport.Content.Find("PageNO", result.toString()));
                                }
                                
                                break;
                            }
                            if (($scope.SysUserCode != '22' || temp) && $scope.Open.Report.Current.ReportTitle.PageNO != result.toString()) {
                                $scope.Open.Report.Current.ReportTitle.State = 0;
                            }
                            $scope.Open.Report.Current.ReportTitle.PageNO = result.toString();
                            if ($rootScope.Open.Tree.Current.TreeId.indexOf("MyRptTree") < 0) {
                                $rootScope.Open.Tree.Switch("ReceivedRptTree", true).Refresh.Report().NodeState(); //没有显示MyRptTree，则从后台获取数据
                                $rootScope.Open.Tree.Data.Load(false, "MyRptTree");
                            } else {
                                $rootScope.Open.Tree.Switch("MyRptTree", true).Refresh.Report().NodeState();
                            }
                            $scope.Open.Report.Current.Attr.DelAffixTBNO.splice(0);
                            $scope.Open.Report.Current.Attr.DelAffixURL.splice(0);
                            $scope.Open.Report.Fn.Core.Upload.Affix();
                            baseData.RecentReportInfo[$scope.Open.Report.Current.ReportTitle.ORD_Code] = angular.copy($scope.Open.Report.Current.ReportTitle);

                            if (type == "Copy") {
                                $scope.Open.Report.Current.Attr.HB = false;
                            } else {
                                if ($scope.SysUserCode == '22' && $scope.Open.Report.Current.ReportTitle.State == 3) {  //吉林修改已报送，只是显示已报送，报表不包送
                                    $scope.Open.Report.Current.ReportTitle.State = 4;
                                }
                            }

                            if (temp) {
                                Alert("复制报表成功！");
                            } else {
                                Alert("保存成功");
                            }

                            delete $scope.Open.Report.Current.Attr.Data_Changed;
                        } else {
                            if (result.indexOf("更新条目时出错") >= 0) {
                                Alert("当前在线人数过多，请稍后再试");
                            } else {
                                alert(result);
                            }
                        }
                    },
                    Upload: {
                        Affix: function() {
                            if ($(".Boxes #file_upload-queue").children().length > 0) { //存在附件
                                var uploadify = $('#file_upload');
                                uploadify.uploadify("settings", "formData", {
                                    'PageNo': $scope.Open.Report.Current.ReportTitle.PageNO,
                                    'limit': baseData.Unit.Local.Limit,
                                    'unitcode': baseData.Unit.Local.UnitCode,
                                    'rptType': $scope.Open.Report.Current.ReportTitle.ORD_Code
                                });
                                uploadify.uploadify('upload', '*');
                            }
                        }
                    },
                    Send: function(childrenWindow, reportTitle) {
                        if ($scope.Open.Report.Current.Attr.Data_Changed) {
                            Alert("报表的数据有变动，请先保存再报送");
                            return false;
                        }

                        var message = "确认报送？";
                        reportTitle = reportTitle ? reportTitle : $rootScope.Open.Report.Current.ReportTitle;
                        if (parseInt(reportTitle.State) == 3 && parseInt(reportTitle.SendOperType) == 1) {
                            message = "报送为核报后不能修改，确认报送？";
                        }

                        if ((childrenWindow ? childrenWindow.confirm(message) : confirm(message))) {
                            var sendType = 0;
                            if (parseInt(reportTitle.State) == 3) {
                                sendType = parseInt(reportTitle.SendOperType) + 1;
                            }
                            var result = $rootScope.Fn.Ajax("Index/SendReport", { pageno: reportTitle.PageNO, sendType: sendType });
                            if (Number(result) > 0) {
                                reportTitle.State = 3;
                                reportTitle.SendOperType = sendType;

                                if (!childrenWindow) {
                                    if ($scope.Open.Tree.Current.TreeId.indexOf("MyRptTree") > 0) {
                                        $scope.Open.Tree.Switch("MyRptTree", true);
                                        $scope.Open.Tree.Refresh.Report();
                                    } else {
                                        $scope.Open.Tree.Data.Load($scope.Open.ReportTitle.ORD_Code, "MyRptTree");
                                    }
                                }

                                if (childrenWindow) {
                                    childrenWindow.Alert("报送成功！");
                                } else {
                                    Alert("报送成功！");
                                }
                            } else {
                                Alert(result);
                            }
                        }
                    },
                    Set: function(target, disabledCallBack) {
                        if (target) {
                            $scope.Fn.GetEvt().data('target', target);
                        } else {
                            $scope.Fn.GetEvt().removeData('target');
                        }

                        if (disabledCallBack) {
                            $scope.Fn.GetEvt().data('disabledCallBack', ["RelationCheck", "UnitEntity"]);
                        } else {
                            $scope.Fn.GetEvt().removeData('disabledCallBack');
                        }

                        /*$timeout(function() {  //所有运算完成后，还原避免造成target变量污染
                            $scope.Fn.GetEvt().removeData('target');
                            $scope.Fn.GetEvt().removeData('disabledCallBack');
                        });*/
                    },
                    Sum: function(arr, path, fixed) {
                        var $target = $scope.Fn.GetEvt().data('target');
                        $target = typeof($target) == "object" ? $target : $scope.Fn.GetEvt();
                        $scope.Fn.GetEvt().data('target', $target);

                        var tableField = arr ? arr : $target.attr("ng-model").split(".");
                        tableField[0] = tableField[0].toUpperCase();
                        var sumValue = 0;
                        var fixedVal = 0;
                        var tmp = undefined;
                        var data = $rootScope.Open.Report.Current[tableField[0]];

                        if (path) {
                            tmp = data;
                            for (var j = 0; j < path.length; j++) {
                                if (Object.hasOwnProperty.call(path, j)) {
                                    tmp = tmp[path[j]];
                                }
                            }
                            data = tmp;
                        }

                        var go = true;
                        for (var i = 1; i < data.length; i++) {
                            go = true;
                            if ($scope.SysUserCode != '35') {
                                switch ($scope.Open.Report.Current.ReportTitle.ORD_Code) {
                                case 'HL01':
                                    if (typeof data[i].DW == "string" && data[i].DW.Contains('本级') && typeof tableField[1] == "string" && tableField[1].Contains("SZFW")) { //XX本级不计
                                        tmp = parseInt(data[i][tableField[1]]);
                                        if (tmp > 0) {
                                            data[i][tableField[1]] = tmp;
                                        } else {
                                            delete data[i][tableField[1]]; //删除非数字
                                        }
                                        go = false;
                                    }
                                    break;
                                }
                            }

                            if (go) {
                                data[i][tableField[1]] = Number(data[i][tableField[1]]);
                                if (data[i][tableField[1]] > 0 && data[i][tableField[1]].toString().split(".")[0].length < 9 && data[i][tableField[1]].toString().indexOf("e+") < 0) {  //最大值9千万
                                    if (fixed == undefined) {
                                        sumValue = App.Tools.Calculator.Addition(sumValue, data[i][tableField[1]]);
                                    } else {
                                        fixedVal = parseFloat(data[i][tableField[1]]).toFixed(fixed);
                                        data[i][tableField[1]] = parseFloat(fixedVal) == 0 ? undefined : fixedVal;
                                        sumValue += parseFloat(fixedVal);
                                    }
                                } else {
                                    delete data[i][tableField[1]]; //删除非数字
                                }
                            }
                        }
                        data[0][tableField[1]] = parseFloat(sumValue) === 0 ? undefined : (fixed == undefined ? sumValue : sumValue.toFixed(fixed));

                        var disabledCallBack = $scope.Fn.GetEvt().data('disabledCallBack');
                        disabledCallBack = $.isArray(disabledCallBack) ? disabledCallBack : [];
                        if (!disabledCallBack.In_Array('UnitEntity')) { //只进行合计，不调OnChange.UnitEntity方法
                            if ($scope.SysUserCode == '22' && $scope.BaseData.Unit.Local.Limit > 2 || $scope.SysUserCode != '22') {  //吉林市级、县级受灾范围县自动填1
                                tmp = $scope.Fn.GetEvt().attr("ng-model");
                                if (!($scope.SysUserCode == "33" && tmp.split(".")[0] == "hl014") && tmp && ["hl011", "hl014"].In_Array(tmp.split(".")[0])) { //HL011、HL014算合计(可能是OnChange.DW调用的，这里要用"ng-model"区分)
                                    tmp = $scope.Fn.GetEvt().scope();

                                    if (tableField[0].toLowerCase() == "hl011") {
                                        if ($scope.SysUserCode == "33") {
                                            tmp = tmp[tableField[0].toLowerCase()]; //浙江表7、8不跟表1“受淹范围”关联
                                        } else {
                                            tmp = $.extend(angular.copy(tmp[tableField[0].toLowerCase()]), $scope.Open.Report.Current.HL014[tmp.$index]);
                                        }
                                    } else {
                                        if ($scope.SysUserCode == "33") {
                                            tmp = $scope.Open.Report.Current.HL011[tmp.$index];
                                        } else {
                                            tmp = $.extend(angular.copy(tmp[tableField[0].toLowerCase()]), $scope.Open.Report.Current.HL011[tmp.$index]);
                                        }
                                    }

                                    $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(tmp);
                                }
                            }
                        }

                        if (!disabledCallBack.In_Array('RelationCheck')) { //是否校核，默认NoCheck = false(表示校核)， event. NoCheck
                            $target = $scope.Fn.GetEvt().data('target', $scope.Fn.GetEvt());
                            $target = $target.parents("table tbody").find("tr:first td").has("input[ng-model='" + $target.attr("ng-model") + "']");
                            $scope.Fn.GetEvt().data('target', $target);
                            if ($scope.Open.Report.Current.ReportTitle.ORD_Code == "HL01" && ($scope.Fn.GetEvt().parent().hasClass("WrongData") || $target.hasClass("WrongData"))) {
                                if (!$target.hasClass("WrongData")) {
                                    $target = $scope.Fn.GetEvt();
                                }

                                if ($scope.Fn.GetEvt().is("input")) {
                                    tmp = $scope.Fn.GetEvt().scope()[tableField[0].toLocaleLowerCase()].UnitCode + "-" + tableField[1];
                                } else {
                                    tmp = $scope.Open.Report.Fn.Other.CurTable.Name() + "-" + $scope.Fn.GetEvt().scope().$index + "-Select";
                                }

                                $scope.Open.Report.Fn.Core.RelationCheck(tmp, 0, 0, 0, 1);
                            }
                            $target = $scope.Fn.GetEvt(); //还原$target
                            $scope.Fn.GetEvt().data('target', $target);
                        }
                        //$scope.Open.Report.Fn.Core.Private.Call("Sum", 0, tableField);  //自动调用各省的Sum方法
                    }
                },
                Comm: {
                    OnChange: {
                        DW: function(type) { //地区改变后
                            var $element = $scope.Fn.GetEvt();
                            var scope = $element.scope();
                            var currentReport = scope.Open.Report.Current;
                            var index = parseInt($element.attr("index")); // parseInt();
                            var tableField = $element.attr("ng-model").split(".");
                            tableField[0] = tableField[0].toUpperCase();
                            var obj = currentReport[tableField[0]][index];
                            var countField = undefined;
                            var sumCount = 0;
                            var subCount = 0;
                            var formUnitCode = undefined;
                            formUnitCode = obj.DW == "—————" ? baseData.Unit.Local.UnitCode : baseData.Unit.Unders.Find("UnitName", obj.DW, "UnitCode");
                            index = $scope.Open.Report.Fn.Other.UnitIndex(formUnitCode);
                            obj.DW = baseData.Unit.Unders.Find("UnitCode", obj.UnitCode, "UnitName");
                            obj.DW = $.isEmptyObject(obj.DW) ? "—————" : obj.DW;

                            if (tableField[0] === "HL012") {
                                countField = type == "死亡" ? "SWRK" : "SZRKR";
                            } else {
                                countField = "SYCS";
                            }

                            if (index > 0) {
                                obj = currentReport.HL011[index];
                                subCount = parseInt(obj[countField]);
                                subCount = --subCount === 0 ? undefined : subCount;
                                obj[countField] = subCount;
                            }

                            index = parseInt($element.find("option:selected").attr("index"));

                            if (index > 0) {
                                obj = currentReport.HL011[index];
                                obj[countField] = Number(obj[countField]) > 0 ? obj[countField] : 0;
                                sumCount = parseInt(obj[countField]);
                                obj[countField] = ++sumCount;
                            }

                            $rootScope.Open.Report.Fn.Core.Sum(["HL011", countField], 0, 0);

                            var arr = [];

                            if (index == 0) {
                                arr = scope.BaseData.Select.River;
                            } else {
                                arr = $rootScope.Open.Report.Fn.Comm.ToRiverArr(scope.BaseData.Unit.Unders[index - 1].RiverCode);
                            }

                            if (tableField[0] == "HL012") {
                                obj = scope.hl012;
                            } else {
                                obj = scope.$parent.hl013;
                            }

                            obj.RiverSelect = arr;
                            obj.RiverCode = arr[0] ? arr[0].Code : undefined;

                            if ($scope.Fn.GetEvt().parent().hasClass("WrongData")) {
                                $scope.Open.Report.Fn.Core.RelationCheck(tableField[0].toUpperCase() + "-Select");
                            }

                            if (Number(baseData.Unit.Local.Limit) > 2) {
                                if (formUnitCode != baseData.Unit.Local.UnitCode) {
                                    index = $scope.Open.Report.Fn.Other.UnitIndex(formUnitCode);
                                    if ($scope.SysUserCode == "33") {
                                        arr = $scope.Open.Report.Current.HL011[index];
                                    } else {
                                        arr = $.extend(angular.copy($scope.Open.Report.Current.HL011[index]), $scope.Open.Report.Current.HL014[index]);
                                    }
                                    $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(arr);
                                }
                                if (obj.UnitCode != baseData.Unit.Local.UnitCode) {
                                    index = $scope.Open.Report.Fn.Other.UnitIndex(obj.UnitCode);
                                    if ($scope.SysUserCode == "33") {
                                        arr = $scope.Open.Report.Current.HL011[index];
                                    } else {
                                        arr = $.extend(angular.copy($scope.Open.Report.Current.HL011[index]), $scope.Open.Report.Current.HL014[index]);
                                    }
                                    $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(arr);
                                }
                            }
                        },
                        Type: function(type, unitcode) { //死亡类型改变后
                            if ($rootScope.BaseData.Unit.Local.UnitCode != unitcode) {
                                var newField = type == "死亡" ? "SWRK" : "SZRKR";
                                var oldField = type == "死亡" ? "SZRKR" : "SWRK";
                                var report = $rootScope.Open.Report;
                                var index = report.Fn.Other.UnitIndex(unitcode);
                                var obj = undefined;
                                $.each([0, index], function(key, value) {
                                    obj = report.Current.HL011[value];
                                    obj[newField] = Number(obj[newField]) > 0 ? Number(obj[newField]) + 1 : 1;
                                    obj[oldField] = parseInt(obj[oldField]) - 1;
                                    obj[oldField] = obj[oldField] == 0 ? undefined : obj[oldField];
                                });
                            }
                        },
                        UnitEntity: function(param) { //每条数据改变后
                            if (Number(baseData.Unit.Local.Limit) > 2) { //市级受灾范围县自动填“1”，县级受灾范围镇自动填“1”
                                var obj = undefined;
                                var count = undefined;
                                var field = undefined;
                                var exceptField = undefined;

                                if (typeof(param) == "object") {
                                    obj = param;
                                } else {  //string 
                                    var index = $scope.Open.Report.Fn.Other.UnitIndex(param);
                                    if ($scope.SysUserCode == "33") {
                                        obj = $scope.Open.Report.Current.HL011[index];
                                    } else {
                                        obj = $.extend(angular.copy($scope.Open.Report.Current.HL011[index]), $scope.Open.Report.Current.HL014[index]);
                                    }
                                }

                                if (!obj.DW.Contains('本级')) {
                                    if ($scope.SysUserCode == '35' && $scope.BaseData.Unit.Local.UnitCode == '35100000') { //平潭综合实验区是是县级
                                        field = "SZFWZ";
                                    } else {
                                        if (Number(baseData.Unit.Local.Limit) == 3 || $scope.SysUserCode == '22') { //市级
                                            field = "SZFWX";
                                        } else {
                                            field = "SZFWZ";
                                        }
                                    }
            
                                    exceptField = ["PageNO", "TBNO", "DW", "DataOrder", "DistributeRate", "RiverCode", "UnitCode", "QXHJ", "ZJXJ", field]; //field要放到最后面

                                    if ($scope.SysUserCode == "33") {
                                        exceptField.push("ZYRK");
                                        if ($scope.BaseData.Unit.Local.Limit == 4) {
                                            exceptField.push("SYCS");
                                        }
                                    }

                                    //湖北“高新区”受灾范围县不自动填1
                                    var is_gxq_hb = false;
                                    if ($scope.SysUserCode == '42') {
                                        if (baseData.Unit.Local.Limit > 3) { //县级
                                            if(baseData.Unit.Local.UnitName.Contains('高新区')){
                                                is_gxq_hb = true;
                                            }else{
                                                field = 'SZFWX';  //县级受灾范围县自动填1
                                                exceptField[exceptField.length - 1] = field;
                                            }
                                        } else {
                                            is_gxq_hb = obj.DW.Contains('高新区');  //下级单位不自动填1
                                        }
                                    }

                                    if (!is_gxq_hb) {
                                        angular.forEach(obj, function(val, key) {
                                            if (key != "$$hashKey" && !exceptField.In_Array(key) && Number(val) > 0) { //exceptField.In_Array("$$hashKey") = false
                                                count = 1; //SWFW的个数
                                                return false;
                                            }
                                        });
                                        $scope.Open.Report.Current.HL011[$scope.Open.Report.Fn.Other.UnitIndex(obj.UnitCode)][field] = count;
                                    }
                                    //特别注意: 以下data、Sum、removeData三个回调方法顺序不能打乱！
                                    $scope.Fn.GetEvt().data('disabledCallBack', ["UnitEntity", "RelationCheck"]);
                                    //湖北“高新区”受灾范围县不自动填1,所以不需要重新合计受灾范围县那一列
                                    if (!is_gxq_hb) {
                                        $scope.Open.Report.Fn.Core.Sum(["HL011", field], 0, 0);
                                    }
                                    $scope.Fn.GetEvt().removeData('disabledCallBack');
                                }
                            }
                        }
                    },
                    OnFocus: {
                        Field: function() {
                            var result = $scope.Fn.GetEvt().attr("ng-model").split(".");
                            result = result[result.length - 1]; //result = field
                            result = $rootScope.BaseData.Field[result];
                            if (result) {
                                $rootScope.Open.Report.Attr.Explain = result.InputRemark;
                            } else {
                                $rootScope.Open.Report.Attr.Explain = "";
                            }
                        }
                    },
                    ToRiverArr: function(riverStr) {
                        var rivers = [];
                        if (riverStr && riverStr != "") {
                            $.each(riverStr.split(","), function(key, val) {
                                rivers.push({ Name: $rootScope.BaseData.RiverCode[val], Code: val });
                            });
                        }

                        return rivers;
                    },
                    SelectNode: function(pageno) {
                        pageno = pageno == undefined ? $rootScope.Open.Report.Current.ReportTitle.PageNO : pageno;
                        if (parseInt(pageno) != 0) {
                            var zTree = $.fn.zTree.getZTreeObj($rootScope.Open.Tree.Current.TreeId);
                            var node = zTree.getNodeByParam("id", pageno, zTree.getNodeByParam("isFirstNode", true).getNextNode());
                            zTree.selectNode(node);
                        }
                    }
                },
                Other: {
                    CurTable: {
                        Name: function(tableName) {
                            var result = undefined;
                            var setting = {
                                HL01: {
                                    HL011: [0, 1, 2, 3],
                                    HL012: [4, 4],
                                    HL013: [5, 5],
                                    HL014: [6, 7, 8]
                                },
                                HP01: {
                                    HP011: [0, 1],
                                    HP012: [2, 3]
                                }
                            };

                            if (tableName) {
                                result = setting[$scope.Open.Report.Current.ReportTitle.ORD_Code][tableName.toUpperCase()];
                            } else {
                                $.each(setting[$scope.Open.Report.Current.ReportTitle.ORD_Code], function(key) {
                                    if (this.In_Array($scope.Open.Report.Current.Attr.TableIndex)) {
                                        result = key;
                                        return false;
                                    }
                                });
                            }

                            return result;
                        },
                        Obj: function() {
                            return $("table[ng-switch-when=" + $rootScope.Open.Report.Current.Attr.TableIndex + "]");
                        }
                    },
                    UnitIndex: function(unitcode) {
                        var index = -1;
                        if (unitcode == $rootScope.BaseData.Unit.Local.UnitCode) {
                            index = 0;
                        } else {
                            $.each($rootScope.BaseData.Unit.Unders, function(i) {
                                if (this.UnitCode === unitcode) {
                                    index = i + 1;
                                    return false;
                                }
                            });

                            if (!$.isEmptyObject($scope.BaseData.Unit.Unders.Find('UnitName', '全区/县'))) { //$scope.BaseData.Unit.Local.Limit == 4 && $scope.Open.Report.Current.ReportTitle.ORD_Code == 'HL01'
                                if ($scope.Open.Report.Current.ReportTitle.ORD_Code != "HP01") {
                                    index = index - 1; //非县级蓄水索引要算上“全区/县”
                                }
                            }
                        }

                        return index;
                    }
                }
            }
        },
        Tree: {
            Data: {
                HL01: {},
                HP01: {},
                NP01: {},
                Load: function(rptType, treeType) {
                    rptType = rptType ? rptType : $rootScope.Open.ReportTitle.ORD_Code;
                    
                    var params = {
                        unitCode: baseData.Unit.Local.UnitCode,
                        unitLimit: baseData.Unit.Local.Limit,
                        rptClass: rptType,
                        cycType: $scope.Open.ReportTitle.StatisticalCycType,
                        limitType: treeType == "MyRptTree" ? 0 : 1
                    };
                    
                    if (treeType == 'ReceivedRptTree' && $scope.Open.ReportTitle.UnitCode != baseData.Unit.Local.UnitCode) {
                        params.unitCode = $scope.Open.ReportTitle.UnitCode;
                    }
                    
                    this[rptType][treeType] = $scope.Fn.Ajax('Index/GetTreeData', params, 'post');
                }
            },
            State: {
                HL01: {
                    MyRpt: new App.Models.TreeModel(),
                    ReceivedRpt: new App.Models.TreeModel()
                }
            },
            Current: {
                TreeId: undefined,
                SimpleData: undefined
            },
            SwitchType: undefined,
            Switch: function(treeType, refresh, rptType) {
                treeType = treeType ? treeType : this.Current.TreeId.slice(4);
                refresh = refresh ? refresh : false;
                rptType = rptType ? rptType : $rootScope.Open.ReportTitle.ORD_Code;
                if (!this.Data[rptType][treeType] || refresh) {
                    this.Data.Load(rptType, treeType);
                }
                this.Current.TreeId = rptType + treeType;
                this.Current.SimpleData = this.Data[rptType][treeType];
                $rootScope.Open.Selected.Box.Tree = treeType;

                return this;
            },
            Refresh: {
                Report: function() {
                    var openReport = undefined;
                    if ($scope.Open.Tree.Current.TreeId.indexOf("MyRptTree") >= 0) {
                        openReport = function(event, treeId, treeNode) {
                            if (treeNode.id) {
                                var obj = {
                                    rptType: $scope.Open.ReportTitle.ORD_Code,
                                    sourceType: treeNode.SourceType,
                                    pageno: treeNode.id
                                };
                                $scope.Open.Report.Fn.Core.Open(obj);
                            }
                        };
                    } else {
                        openReport = function(event, treeId, treeNode) {
                            if (treeNode.id) {
                                var queryUnderUnits = false;
                                if (!$scope.Receive.Attr.UnderUnits[$scope.Open.ReportTitle.ORD_Code][treeNode.UnitCode]) {
                                    queryUnderUnits = true;
                                } //如果已存在下级单位
                                $scope.Fn.ViewUnderRpt({
                                    ORD_Code: $scope.Open.ReportTitle.ORD_Code,
                                    SourceType: treeNode.SourceType,
                                    Limit: parseInt($scope.BaseData.Unit.Local.Limit) + 1,
                                    UnitCode: treeNode.UnitCode,
                                    PageNO: treeNode.id,
                                    queryUnderUnits: queryUnderUnits
                                });
                            }
                        };
                    }

                    this.CheckBox($scope.InitTree($scope.Open.Tree.Current.TreeId, $scope.Open.Tree.Current.SimpleData, {
                        check: { enable: true },
                        callback: {
                            onClick: openReport,
                            onExpand: function (event, treeId, treeNode) {
                                if (treeNode.level == 1) {
                                    $scope.Open.Tree.FindNodeState(treeId, treeNode.name.replace("月", "")).Expand = true;
                                }
                            },
                            onCollapse: function (event, treeId, treeNode) {
                                if (treeNode.level == 1) {
                                    $scope.Open.Tree.FindNodeState(treeId, treeNode.name.replace("月", "")).Expand = false;
                                }
                            }
                        },
                        data: {
                            simpleData: { enable: true },
                            key: { title: "title" }
                        }
                    }));

                    if (angular.isFunction($scope.Open.Tree.Current.CallBack)) {
                        $scope.Open.Tree.Current.CallBack(); //尽量不要往CallBack中加参数
                        delete $scope.Open.Tree.Current.CallBack;
                    }

                    return this;
                },
                CheckBox: function(zTree) {
                    zTree = zTree ? zTree : $.fn.zTree.getZTreeObj($scope.Open.Tree.Current.TreeId);
                    var enable = false;
                    if ($scope.Open.Report.Current) {
                        switch ($scope.Open.Report.Current.ReportTitle.SourceType) {
                        case "1":
                            if ($scope.Open.Tree.Current.TreeId.indexOf("MyRptTree") >= 0) {
                                enable = false;
                            } else {
                                enable = true;
                            }
                            break;
                        case "2":
                            if ($scope.Open.Tree.Current.TreeId.indexOf("MyRptTree") >= 0) {
                                enable = true;
                            } else {
                                enable = false;
                            }
                            break;
                        }
                    }
                    if (zTree.setting.check.enable != enable) {
                        zTree.setting.check.enable = enable;
                        zTree.refresh();
                    }

                    return this;
                },
                NodeState: function(tree_id) {
                    tree_id = tree_id ? tree_id : $scope.Open.Tree.Current.TreeId;
                    $timeout(function() {
                        var zTreeObj = $.fn.zTree.getZTreeObj(tree_id);
                        var nodes = undefined;
                        $.each($scope.Open.Tree.FindNodeState(tree_id), function(month) {
                            if (this.Expand) {
                                nodes = zTreeObj.getNodesByParam("name", month + "月");
                                if (nodes.length > 0) {
                                    if (nodes[0].children && nodes[0].children.length > 0) {
                                        zTreeObj.expandNode(nodes[0], true);
                                    } else {
                                        this.Expand = false;
                                    }
                                }
                            }
                        });
                    });

                    return this;
                }
            },
            FindNodeState: function(treeId, nodeName) {
                var treeType = undefined;
                var treeName = undefined;

                if (treeId.indexOf("HL01") >= 0) {
                    treeType = "HL01";
                } else if (treeId.indexOf("HP01") >= 0) {
                    treeType = "HP01";
                } else {
                    treeType = "NP01";
                }
                treeName = treeId.replace(treeType, "").replace("Tree", "");

                if (nodeName) {
                    return $scope.Open.Tree.State[treeType][treeName][nodeName];
                } else {
                    return $scope.Open.Tree.State[treeType][treeName];
                }
            }
        },
        Selected: {
            Box: {
                Tree: "MyRptTree",
                Report: undefined
            }
        }
    };

    $rootScope.$watch("Open.ReportTitle.ORD_Code", function (to, from) {
        if (to != from && !$scope.Open.Tree.SwitchType) {
            $rootScope.Open.Tree.Switch($rootScope.Open.Selected.Box.Tree);
            $scope.Open.ReportTitle.StatisticalCycType = "";
        }

        delete $scope.Open.Tree.SwitchType;
    });
    $rootScope.$watch("Open.ReportTitle.StatisticalCycType", function (to, from) {
        if (to != from) {
            $rootScope.Open.Tree.Switch($rootScope.Open.Selected.Box.Tree, true);
            $scope.Open.Tree.Refresh.Report();
        }
    });
    $rootScope.$watch("Open.ReportTitle.UnitCode", function (to, from) {
        if (to != from) {
            $rootScope.Open.Tree.Switch($rootScope.Open.Selected.Box.Tree, true);
            $scope.Open.Tree.Refresh.Report();
        }
    });
    $rootScope.$watch("Open.Report.Current.ReportTitle.SendOperType", function(to, from) {
        if (parseInt(to) == 2) { //核报
            $scope.Open.Report.Current.Attr.HB = true;
        }
    });
    $rootScope.$watch("Open.Report.Current.ReportTitle.SourceType", function(to, from) {
        if ($scope.Open.Report.Current) {
            var treeType = undefined;
            to = parseInt(to);

            if (to == 1 || to == 2) {
                if (to == 1) {
                    treeType = "ReceivedRptTree";
                } else {
                    treeType = "MyRptTree";
                }
            } else {
                if (to == 0) {
                    treeType = "MyRptTree";
                }
            }

            if ($scope.Open.Report.Current.ReportTitle.ORD_Code == $scope.Open.ReportTitle.ORD_Code) { //同类型的报表切换
                if (treeType) {
                    if ($scope.Open.Tree.Current.TreeId && $scope.Open.Tree.Current.TreeId.indexOf(treeType) > 0) {
                        treeType = $.fn.zTree.getZTreeObj($scope.Open.Tree.Current.TreeId);
                        if (treeType != null) {
                            $scope.Open.Tree.Refresh.CheckBox(treeType);
                        }
                    } else {
                        $rootScope.Open.Tree.Switch(treeType, false, $rootScope.Open.Report.Current.ReportTitle.ORD_Code);
                    }
                } else if (from) {
                    $scope.Open.Tree.Refresh.CheckBox();
                }
            } else { //不同类型的报表切换
                $scope.Open.Tree.SwitchType = "Double";
                $rootScope.Open.Tree.Switch(treeType, false, $scope.Open.Report.Current.ReportTitle.ORD_Code);
                $scope.Open.ReportTitle.StatisticalCycType = "";
                $scope.Open.ReportTitle.ORD_Code = $scope.Open.Report.Current.ReportTitle.ORD_Code;
            }
        }
    });
    $rootScope.$watch("Open.Report.Current.ReportTitle.ORD_Code", function(to, from) {
        if (to != from && to != undefined) {
            $rootScope.Open.Report.Attr.Instruction = baseData.Select.CycType[to].Find('value', $rootScope.Open.Report.Current.ReportTitle.StatisticalCycType);
            $scope.Open.Report.Attr.Explain = "";

            if (from) {  //HL01和HP01之间的报表树切换
                var treeType = undefined;
                var sourceType = parseInt($scope.Open.Report.Current.ReportTitle.SourceType);
                if (sourceType == 1 || sourceType == 2) {
                    if (sourceType == 1) {
                        treeType = "ReceivedRptTree";
                    } else {
                        treeType = "MyRptTree";
                    }
                } else {
                    if (sourceType == 0) {
                        treeType = "MyRptTree";
                    }
                }
                    $scope.Open.Tree.SwitchType = "Double";
                $rootScope.Open.Tree.Switch(treeType, false, to);
                $scope.Open.ReportTitle.StatisticalCycType = "";
                $scope.Open.ReportTitle.ORD_Code = to;
            }
        }
    });
    $rootScope.$watch("Open.Report.Current.ReportTitle.StatisticalCycType", function (to, from) {
        var report = $rootScope.Open.Report;

        if (report.Current) {
            if (to == 0) {
                if (!report.Current.Attr.SSB) {
                    report.Current.Attr.TableIndex = 8;
                }
                report.Current.Attr.SSB = true;
            } else {
                report.Current.Attr.SSB = false;
            }
            report.Attr.Instruction = baseData.Select.CycType[report.Current.ReportTitle.ORD_Code].Find("value", report.Current.ReportTitle.StatisticalCycType);
            if (from != undefined && to != undefined && report.Current.ReportTitle.ORD_Code == "HL01" && $scope.Fn.GetEvt().is("input[name=cycType]")) {
                $scope.Fn.OnChange.CycType(to, report.Current.ReportTitle);
            }
        }
    });
    $rootScope.$watch("Open.Report.Current.Attr.TableIndex", function(to, from) {
        if (!isNaN(from) && to != undefined && $scope.Open.Report.Current) {
            if ($scope.Open.Report.Current.ReportTitle.ORD_Code == 'HP01' && to == 0) {
                $timeout(function() {
                    App.Plugin.TableFixed.Fix({
                        Index: $scope.Report[$scope.Attr.NameSpace].Current.Attr.TableIndex,
                        Element: $("table#HP011-1")
                    });
                });
            }

            if ($scope.Open.Report.Current.ReportTitle.ORD_Code == 'HP01' && from < 2 || $scope.Open.Report.Current.ReportTitle.ORD_Code == 'HL01') {
                App.Plugin.TableFixed.FixedPage[from].Destroy();
            }
        }
        if ($scope.Open.Report.Current) {
            $scope.Config.Page.Open.Func.AddRow = !(((to == 4 || to == 5) && !$scope.Open.Report.Current.Attr.HB) && !($scope.SysUserCode == '33' && $scope.BaseData.Unit.Local.Limit == 4 && to == 5));
            $scope.Config.Page.Open.Func.DelRow = $scope.Config.Page.Open.Func.AddRow;
        }
    });
    $rootScope.$watchCollection("[Open.Report.Current.Attr.HB,Open.Report.Current.ReportTitle.ORD_Code,Open.Report.Current.ReportTitle.PageNO]", function() {
        if ($scope.Open.Report.Current) {
            var obj = {
                HB: $scope.Open.Report.Current.Attr.HB,
                ORD_Code: $scope.Open.Report.Current.ReportTitle.ORD_Code,
                PageNO: $scope.Open.Report.Current.ReportTitle.PageNO,
                TableIndex: $scope.Open.Report.Current.Attr.TableIndex
            };
            $scope.Config.Page.Open.Func = $.extend($scope.Config.Page.Open.Func, {
                Save: obj.HB || obj.ORD_Code == 'NP01' && $scope.BaseData.Unit.Local.Limit == 2,
                Copy: obj.ORD_Code == 'NP01' || obj.PageNO == 0,
                Send: obj.PageNO == 0 || obj.HB || obj.ORD_Code == 'NP01',
                Delete: obj.ORD_Code == 'NP01' || obj.PageNO == 0,  //蓄水无删除只能清除数据
                Import: obj.HB || obj.ORD_Code != 'HL01',
                Export: obj.PageNO == 0,
                NRS: !(obj.PageNO != 0 && obj.ORD_Code == 'HP01' && $scope.BaseData.Unit.Local.Limit == 2), //NRS:National Report of Statistics
                DisasterReview: obj.ORD_Code != 'HL01',
                River: !(obj.ORD_Code == 'HL01' && obj.PageNO != 0 && $scope.BaseData.Unit.Local.Limit == 2),
                Notice: obj.ORD_Code != 'HP01' || $scope.BaseData.Unit.Local.Limit != 2,
                ReportTitle: {
                    StartDateTime: obj.HB || obj.ORD_Code == 'HP01' || obj.ORD_Code == 'NP01'
                }
            });
            $scope.Config.Page.Open.Func.ReportTitle.EndDateTime = $scope.Config.Page.Open.Func.ReportTitle.StartDateTime;
        }
    });
    $rootScope.BaseData.DeathReason.Hide = function (event) {
        var tree = $rootScope.BaseData.DeathReason.zTree.parent()[0];
        if ((event.srcElement?event.srcElement:event.target) != tree && !$.contains(tree, (event.srcElement?event.srcElement:event.target))) {
            $(tree).hide();
            $(document.body).unbind("mousedown", $rootScope.BaseData.DeathReason.Hide);
        }
    };
    $rootScope.BaseData.Select.River = $rootScope.Open.Report.Fn.Comm.ToRiverArr($rootScope.BaseData.Unit.Local.RiverCode);
    $rootScope.Report = $rootScope.Report || {};
    $rootScope.Report.Open = $rootScope.Open.Report;
    $rootScope.Open.Init();
    //-------------------------------------</Open State>---------------------------------------------
    //-------------------------------------<Receive State>---------------------------------------------
    $rootScope.Receive = {
        Init: function () {
            var model = function () {
                return {
                    UnitCode: "",
                    UnitName: "",
                    ORD_Code: $.cookie("ord_code"),
                    StatisticalCycType: "",
                    StartDateTime: App.Tools.Date.GetOneDay(-30),
                    EndDateTime: App.Tools.Date.GetToday()
                };
            };
            this.Attr.Auditing = new model();
            this.Attr.Audited = new model();
            this.Attr.Refused = new model();
            this.Init = angular.noop;
        },
        Attr: {
            CycTypeSelect: baseData.Select.CycType[$rootScope.SysORD_Code],  //.concat(baseData.Select.CycType["HP01"] ? baseData.Select.CycType["HP01"] : [])
            RptClass: baseData.Select.RptClass,
            CheckAll: false,
            UnderUnits: {
                HL01: {},
                HP01: {}
            },
            Report: {
                Affix: [],
                Content: {},
                PageNO: undefined
            }
        },
        Box: {
            Name: ['Auditing', 'Audited', 'Refused'],
            Current: 'Auditing',
            Selected: undefined,
            Auditing: {
                Real: [],
                Fake: [],
                Inited: false,
                Refresh: false
            },
            Audited: {
                Real: [],
                Fake: [],
                Inited: false,
                Refresh: false
            },
            Refused: {
                Real: [],
                Fake: [],
                Inited: false,
                Refresh: false
            }
        },
        Fn: {
            Operate: function(action) {
                var data = {
                    pagenos: undefined,
                    state: undefined,
                    unitType: ["", null].In_Array($scope.Receive.Attr[$scope.Receive.Box.Current].UnitCode) ? 1 : 2
                };
                var pagenos = [];
                var unitcodes = [];
                var time = [];
                var currentBox = $rootScope.Receive.Box[$rootScope.Receive.Box.Current];
                var arr = [], detials = [];
                $.each(currentBox.Fake, function(i) {
                    if (this.Checked) {
                        arr.push(this);
                        pagenos.push(this.PageNO.toString().trim());
                        
                        if (action == "Refuse") {
                            unitcodes.push(this.UnitCode);
                            time.push(this.StartDateTime + "至" + this.EndDateTime);
                            detials.push("[" + App.Tools.CN_Name("SourceType", this.SourceType) + "][初报]" + App.Tools.Date.GetToday('MM月dd日', this.StartDateTime) + "-" + App.Tools.Date.GetToday('MM月dd日', this.EndDateTime));
                        }
                    }
                });
                if (pagenos.length > 0) {
                    var stateCn = "";
                    var boxName = "";
                    var fn = function() {
                        data.pagenos = pagenos.toString();
                        var result = $rootScope.Fn.Ajax("Index/ReportOperate" + (action == 'Refuse' ? ('?unitcodes=' + unitcodes.toString() + "&time=" + time.toString()) : ""), data);
                        if (typeof result === "number") {
                            if (result > 0) {
                                var expect = [];
                                var zTree = [];
                                $.each(pagenos, function(i, pageno) {
                                    currentBox.Fake = $.grep(currentBox.Fake, function(obj) {
                                        if (obj.PageNO == pageno) {
                                            obj.Checked = false;
                                            if (!zTree.In_Array(obj.ORD_Code)) {
                                                zTree.push(obj.ORD_Code);
                                            }
                                            expect.push(obj);
                                            return false;
                                        } else {
                                            return true;
                                        }
                                    });
                                    currentBox.Real = $.grep(currentBox.Real, function(obj) {
                                        if (obj.PageNO != pageno) {
                                            return true;
                                        } else {
                                            return false;
                                        }
                                    });
                                });

                                if (action != "Delete") {
                                    currentBox = $rootScope.Receive.Box[boxName];
                                    currentBox.Fake = currentBox.Fake.concat(expect);
                                    currentBox.Real = currentBox.Real.concat(expect);
                                } else {
                                    if ($scope.RecycleBin.Box.Current == 'InferiorDept') {
                                        $scope.Fn.TabSearch.Refresh.Server('RecycleBin');
                                    } else {
                                        $scope.RecycleBin.Box.InferiorDept.Inited = false;
                                    }
                                }

                                if (action == "Audit" || $scope.Receive.Box.Current == "Audited" && action == "Delete") { //装入和已收表箱中的删除需要更新TreeData
                                    $.each(zTree, function(i, rptType) {
                                        $rootScope.Open.Tree.Switch("ReceivedRptTree", true, rptType);
                                    });
                                }

                                $rootScope.Receive.Attr.CheckAll = false;
                                Alert(stateCn + "成功！");
                            }
                        } else {
                            Alert(result); //"部分报表已参与累计或汇总，不能删除"
                        }

                        if ($scope.Receive.Box.Selected && $scope.Receive.Attr.Report.PageNO == $scope.Receive.Box.Selected.PageNO) {
                            $scope.Receive.Box.Selected = undefined;
                        }
                    }

                    switch (action) {
                        case "Recover":
                            data.state = -1;
                            stateCn = "恢复";
                            boxName = "Auditing";
                            break;
                        case "Refuse":
                            data.state = 1;
                            stateCn = "拒收";
                            boxName = "Refused";
                            break;
                        case "Audit":
                            data.state = 2;
                            stateCn = "装入";
                            boxName = "Audited";
                            break;
                        case "Delete":
                            if (!confirm("确认删除么？")) {
                                return;
                            }
                            data.state = 3;
                            stateCn = "删除";
                            boxName = "Delete";
                            break;
                    }

                    if (action == "Refuse" && $scope.SysORD_Code == 'HL01') {
                        $scope.Dialog.Config({
                            Title: "拒收原因",
                            Layout: "Refuse",
                            Message: "该报表数据有问题，请核查后再报送",
                            Confirm: function() {
                                $scope.Fn.Ajax('UrgeReport/AddUrgeReport', {
                                    receiveUnitCode: unitcodes.toString(),
                                    content: $scope.Dialog.Attr.Message,
                                    detials: detials.toString(),
                                    pagenos: pagenos.toString(),
                                    urgeReportUnit: $scope.BaseData.Unit.Local.UnitCode,
                                    msgType: 2   //0表示催报消息，1表示发送报表消息，2表示拒收消息
                                }, 0, function(result) {
                                    if (Number(result) > 0) {
                                        fn();
                                        $scope.Dialog.Attr.Show = false;
                                        $scope.$apply();
                                    } else {
                                        result = "拒收原因保存失败，" + result;
                                        Alert(result);
                                        throw result;
                                    }
                                });
                            }
                        });
                    } else {
                        fn();
                    }
                } else {
                    Alert("未选择报表");
                }
            },
            BoxState: function () {
                switch ($rootScope.Receive.Box.Current) {
                    case "Auditing":
                        return 0;
                    case "Audited":
                        return 2;
                    case "Refused":
                        return 1;
                }
            },
            Download: function () {

            }
        }
    };
    $rootScope.Receive.Init();
    //-------------------------------------</Receive State>---------------------------------------------
    //-------------------------------------<RecycleBin State>---------------------------------------------
    $rootScope.RecycleBin = {
        Init: function() {
            var model = function() {
                return {                    
                    UnitCode: "",
                    ORD_Code: $.cookie("ord_code"),
                    StatisticalCycType: "",
                    StartDateTime: App.Tools.Date.GetOneDay(-7),
                    EndDateTime: App.Tools.Date.GetToday()
                };
            };
            this.Attr.CurrentDept = new model();
            this.Attr.InferiorDept = new model();
            this.Init = angular.noop;
        },
        Attr: {
            CycTypeSelect: baseData.Select.CycType[$rootScope.SysORD_Code],
            RptClass: baseData.Select.RptClass,
            CheckAll: false,
            UnderUnits: {
                HL01: {},
                HP01: {}
            },
            Report: {
                Affix: [],
                Content: {},
                PageNO: undefined
            }
        },
        Box: {
            Name: ['CurrentDept', 'InferiorDept'],
            Current: 'CurrentDept',
            Selected: undefined,
            CurrentDept: {
                Real: [],
                Fake: [],
                Inited: false,
                Refresh: false
            },
            InferiorDept: {
                Real: [],
                Fake: [],
                Inited: false,
                Refresh: false
            }
        },
        Fn: {
            Operate: function(type) {
                var result = undefined;
                var box = $scope.RecycleBin.Box[$scope.RecycleBin.Box.Current];
                var tmp = [], zTree = [], sourceType = [];

                $.each(box.Fake, function() {
                    if (this.Checked) {
                        tmp.push(this.PageNO);
                        sourceType.push(this.SourceType);
                        if (!zTree.In_Array(this.ORD_Code)) {
                            zTree.push(this.ORD_Code);
                        }
                    }
                });

                if (tmp.length > 0) {
                    if (type == "Delete") {
                        if (!confirm("确认删除么？")) {
                            return;
                        }
                    }

                    switch (type) {
                    case "Delete":
                        result = $scope.Fn.Ajax('/InboxAndRecycle/DeleteReportAtRecycle', {
                            searchUnitLimit: $scope.RecycleBin.Box.Current == 'CurrentDept' ? 0 : 2,
                            pageNO: tmp.toString()
                        });
                        tmp = "删除";
                        break;
                    case "Resume":
                        result = $scope.Fn.Ajax('Index/ReportOperate', {
                            pagenos: tmp.toString(),
                            state: -1,
                            unitType: $scope.RecycleBin.Box.Current == 'CurrentDept' ? 0 : 2
                        });
                        tmp = "恢复";
                        break;
                    }

                    if (result > 0) {
                        if ($scope.RecycleBin.Box.Current == 'CurrentDept') {  //回收站删除或恢复都需要刷新RptTree，因为“报表的使用状态”可能存在变化
                            var treeTypeArr = [];

                            if (sourceType.In_Array(1)) {  //删除或恢复汇总刷新ReceivedRptTree
                                treeTypeArr.push("ReceivedRptTree");
                            }
                            
                            if(sourceType.In_Array(2)) {  //删除或恢复累计刷新MyRptTree
                                treeTypeArr.push("MyRptTree");
                            }
                            
                            if(type == 'Resume' && !treeTypeArr.In_Array("MyRptTree")){  //恢复录入表刷新MyRptTree, 可优化
                                treeTypeArr.push("MyRptTree");
                            }
                            
                            angular.forEach(treeTypeArr, function(treeType) {  
                                angular.forEach(zTree, function(rptType) {
                                    $scope.Open.Tree.Switch(treeType, true, rptType);
                                });
                            });
                        } else {
                            angular.forEach($scope.Receive.Box.Name, function (name) {
                                $scope.Receive.Box[name].Inited = false;
                            });
                            $scope.Fn.TabSearch.Refresh.Server('Receive');
                                
                            angular.forEach(zTree, function (rptType) {  //恢复下级表时，需刷新ReceivedRptTree
                                $scope.Open.Tree.Switch("ReceivedRptTree", true, rptType);
                            });
                        }

                        box.Real = box.Real.RemoveBy("Checked", true);
                        box.Fake = box.Fake.RemoveBy("Checked", true);
                        $scope.RecycleBin.Attr.CheckAll = false;
                        Alert(tmp + "成功！");
                    }else if (result == -1 && type == "Delete") {
                        Alert("部分报表已参与累计或汇总，不能删除");
                    } else {
                        Alert(result);
                    }

                    if ($scope.RecycleBin.Box.Selected && $scope.RecycleBin.Attr.Report.PageNO == $scope.RecycleBin.Box.Selected.PageNO) {
                        $scope.RecycleBin.Box.Selected = undefined;
                    }
                } else {
                    Alert("未选择报表");
                }
            }
        }
    };
    $rootScope.RecycleBin.Init();
    //-------------------------------------</RecycleBin State>---------------------------------------------
    //-------------------------------------<UrgeReport State>---------------------------------------------
    $rootScope.UrgeReport = {
        Init: angular.noop,
        Attr: {
            Content:undefined,
            PersonName:undefined,
            Unit: []
        },
        Fn: {
            Clear: function() {
                $scope.UrgeReport.Attr = {
                    Content: undefined,
                    PersonName: undefined,
                    Unit: []
                };
                $('#unit-select').multiSelect('deselect_all');
            },
            Urge: function() {
                if ($scope.UrgeReport.Attr.Unit.length == 0) {
                    Alert("未选择被催报的单位");
                } else if (!$scope.UrgeReport.Attr.Content || $scope.UrgeReport.Attr.Content && $scope.UrgeReport.Attr.Content.length == 0) {
                    Alert("未输入催报内容");
                } else if (!$scope.UrgeReport.Attr.PersonName || $scope.UrgeReport.Attr.PersonName && $scope.UrgeReport.Attr.PersonName.length == 0) {
                    Alert("未输入催报人");
                } else if(confirm("确认发送催报消息？")) {
                    var result = $scope.Fn.Ajax('UrgeReport/AddUrgeReport', {
                        receiveUnitCode: $scope.UrgeReport.Attr.Unit.toString(),
                        content: $scope.UrgeReport.Attr.Content.replaceAll("[\r|\n]", "{/r-/n}").replaceAll("\"","&syh;").replaceAll("'","&dyh;"),
                        urgeReportPerson: $scope.UrgeReport.Attr.PersonName,
                        urgeReportUnit: $scope.BaseData.Unit.Local.UnitCode
                    });

                    if (result > 0) {
                        Alert("催报成功！");
                        this.Clear();
                    } else {
                        alert(result);
                    }
                }
            }
        }
    };
    //-------------------------------------</UrgeReport State>---------------------------------------------
    window.$scope = $rootScope;
    $rootScope.RedictUrl($rootScope.CurrentUrl, false);
    if ($.isFunction($rootScope[$rootScope.CurrentUrl.replace('/', '')].Init)) {
        var namespace = $rootScope.CurrentUrl.replace('/', '');
        $rootScope[namespace].Init();
        $rootScope[namespace].Init = true;
    }
    //window.plugins.preloader('stop');
}]).run(['$http',function ($http) {
    $http.defaults.responseType = 'json';
    $http.defaults.method = 'post';
    $http.defaults.cache = false; //启用数据数据缓存
}]);

App.controller('MenuCtrl', ['$rootScope', function($rootScope) {
    $rootScope.Dialog = {
        Config: function(config) {
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
                Width: "inherit",
                Left: "43%",
                Top: "30%",
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

App.controller('IndexNewCtrl', ['$scope', function($scope) {
    var $CurrentScope = $scope.New;
    var zTree = $scope.InitTree("RptTypeTree", $scope.BaseData.zTree.RptClass, {
        callback: {
            onClick: function(event, treeId, treeNode) {
                if (!treeNode.isParent && treeNode.id != $CurrentScope.ReportTitle.ORD_Code) {
                    $CurrentScope.ReportTitle.ORD_Code = treeNode.id;
                    $CurrentScope.ReportTitle.StatisticalCycType = treeNode.id == "HL01" ? "6" : "2";
                    $CurrentScope.ReportTitle.SourceType = 0;
                    $CurrentScope.SameReport.Search(true);
                    $scope.$apply();
                }
            }
        }
    });

    zTree.selectNode(zTree.getNodeByParam("id", $CurrentScope.ReportTitle.ORD_Code));

    if (["HP01","NP01"].In_Array($scope.SysORD_Code) && !$scope.New.XSRQ.Time) {
        var arr, month = new Date().getMonth() + 1, last_day = new Date().getDate() > 16 ? true : false;
        $.each($scope.BaseData.HPDate, function() {
            arr = this.FakeTime.replace("月", ".").replace("日", "").split(".");
            if (Number(arr[0]) == month) {
                if (last_day && Number(arr[1]) > 16 || !last_day && Number(arr[1]) <= 16) {
                    $scope.New.XSRQ.Time = this.RealTime;
                    return false;
                } else {
                    $scope.New.XSRQ.Time = this.RealTime;  //可能每月只需要报一次
                }
            }
        });
    }
}]);

App.controller('IndexOpenCtrl', ['$scope', function ($scope) {
    var $CurrentScope = $scope.Open;
    var type = undefined;
    
    if ($scope.Params["Open"]) {
        switch ($scope.Params["Open"].method) {
            case "Create":
                $CurrentScope.Report.Opened.push($scope.Params["Open"].report);
                $CurrentScope.Report.Current = $scope.Params["Open"].report;
                switch ($CurrentScope.Report.Current.ReportTitle.ORD_Code) {
                    case "NP01":
                        $scope.Open.Report.Fn.Core.NP01.Sum.DQXSL();
                        if ($scope.BaseData.Unit.Local.Limit == 2) {
                            var obj, np011S = $scope.Open.Report.Current.NP011;
                            $.each($scope.BaseData.Unit.Unders, function() {
                                obj = np011S.Find("UnitCode", this.UnitCode);
                                if (!$.isEmptyObject(obj)) {
                                    obj = $.extend(obj, $scope.BaseData.Unit.XSHJ.Find("UnitCode", this.UnitCode));
                                }
                            });
                        }
                        break;
                    case "HP01":
                        /*var fields = [];
                        var firstParam = [];
                        var secondParam = [];
                        $.each(["Large", "Middle", "Units"], function (i, key) {
                            $.each($scope.BaseData.Reservoir[key], function () {
                                $.each(this, function (field, value) {
                                    if (field != "UnitCode" && field != "DXSKMC" && value && !fields.In_Array(field)) {
                                        fields.push(field);
                                    }
                                });
                                if (key != "Units") {
                                    return false;
                                }
                            });
                            if (fields.length > 0) {
                                if (key === "Units") {
                                    firstParam = ["HP011"];
                                    secondParam = undefined;
                                } else {
                                    firstParam = ["HP012"];
                                    secondParam = ["Real", key];
                                }
                                $.each(fields, function (i, field) {
                                    firstParam.push(field);
                                    $scope.Open.Report.Fn.Core.Sum(firstParam, secondParam);
                                    firstParam.splice(1);
                                });
                                fields.splice(0);
                            }
                        });*/
                        break;
                    case "HL01":
                        if ($scope.SysUserCode == '33' && $scope.BaseData.Unit.Local.Limit == 4) {  //浙江县级表6只有一行
                            $CurrentScope.Report.Current.HL013.push(App.Models.HL.HL01.HL013($scope));
                            $CurrentScope.Report.Current.HL013[1].DW = $scope.BaseData.Unit.Local.UnitName;
                            if (["1", "2"].In_Array($CurrentScope.Report.Current.ReportTitle.SourceType)) {
                                $CurrentScope.Report.Current.HL013[1].Max = [];  //因为表六只有一行，需记录最大值
                            }
                        }

                        if (typeof($CurrentScope.Report.Current.ReportTitle.Remark) == "string") {
                            $CurrentScope.Report.Current.ReportTitle.Remark = $CurrentScope.Report.Current.ReportTitle.Remark.replaceAll("{/r-/n}", "\n");
                        }

                        if ($CurrentScope.Report.Current.ReportTitle.SourceType > 0) {
                            var report = angular.copy($CurrentScope.Report.Current);
                            delete report.ReportTitle;
                            delete report.Affix;
                            delete report.Attr;
                            delete report.HL012;
                            delete report.HL013;
                            report.HL011.splice(0, 1); //删除和合计行
                            report.HL014.splice(0, 1);

                            $CurrentScope.Report.Current.Delta = report;
                        }
                        break;
                }
                break;
            case "Open":
                $CurrentScope.Report.Fn.Core.Open($scope.Params["Open"].object);
                break;
        }
        delete $scope.Params["Open"];
    }

    if ($CurrentScope.Tree.Current.TreeId) {
        var ordCode = undefined;

        if ($CurrentScope.Report.Current) {
            ordCode = $CurrentScope.Report.Current.ReportTitle.ORD_Code;
            switch (parseInt($CurrentScope.Report.Current.ReportTitle.SourceType)) {
            case 1:
                type = "ReceivedRptTree";
                break;
            default:
                type = "MyRptTree";
                break;
            }
        } else {
            ordCode = $scope.Open.ReportTitle.ORD_Code;
            if ($CurrentScope.Tree.Current.TreeId.indexOf("MyRptTree") > 0) {
                type = "MyRptTree";
            } else {
                type = "ReceivedRptTree";
            }
        }

        if ($CurrentScope.Tree.Current.TreeId != (ordCode + type)) {
            if ($scope.Open.ReportTitle.ORD_Code == ordCode) { //同类型的报表树切换
                $scope.Open.Tree.Switch(type);
            } else { //不同类型的报表树切换
                $scope.Open.Selected.Box.Tree = type;
                $scope.Open.ReportTitle.ORD_Code = ordCode;
            }
        }
    } else {
        type = $scope.SysORD_Code;
        if ($CurrentScope.Report.Current) {
            type = $CurrentScope.Report.Current.ReportTitle.ORD_Code;
            $CurrentScope.ReportTitle.ORD_Code = type;
        }
        $CurrentScope.Tree.Data.Load(type, "MyRptTree");
        $CurrentScope.Tree.Current.TreeId = type + "MyRptTree";
        $CurrentScope.Tree.Current.SimpleData = $CurrentScope.Tree.Data[type].MyRptTree;
    }

    $scope.FCS = $scope.Open.Report.Fn.Core.Sum;
    $scope.FCRC = $scope.Open.Report.Fn.Core.RelationCheck;
    $scope.FCOD = $scope.Open.Report.Fn.Comm.OnChange.DW;
    $scope.FCOT = $scope.Open.Report.Fn.Comm.OnChange.Type;
    $scope.FCOF = $scope.Open.Report.Fn.Comm.OnFocus.Field;
    $scope.CTF = $scope.Fn.ConvertToFloat;
    $scope.Divide = App.Tools.Calculator.Division;
    $scope.FCM = $scope.Open.Report.Fn.Core.HL01.Max;
    $scope.FCA = $scope.Open.Report.Fn.Core.HL01.Avg;
    $scope.FCHSR = $scope.Open.Report.Fn.Core.HP01.Sum.Reservoir;
    $scope.FCHS = $scope.Open.Report.Fn.Core.HL01.SSB;
    $scope.FCHOX = $scope.Open.Report.Fn.Core.HP01.OnChange.XXSLZJ;
    $scope.FCL = $scope.Fn.Check.Length;
    $scope.FCE = $scope.Fn.Check.Enter;
    $scope.FCQ = $scope.Fn.Check.Quot;
    $scope.FCN = $scope.Fn.Check.Number;
    $scope.N = App.Tools.Calculator.Number;
    $scope.UIndex = $scope.Open.Report.Fn.Other.UnitIndex;
    $scope.RptErrorsSelect = function(obj) {
        $.each($scope.Open.Report.Current.Attr.CheckErrors, function () {
            this.Selected = false;
        });
        obj.Selected = true;
        $scope.Open.Report.Current.Attr.TableIndex = obj.TableIndex;
    };

    if ($scope.SysORD_Code == "NP01" && $scope.BaseData.Unit.Local.Limit == 3) {
        $scope.FCNSQ = $scope.Open.Report.Fn.Core.NP01.Sum.DQXSL;
    }

    if ($scope.SysUserCode == '33') {
        $scope.SW = $scope.Open.Report.Fn.Core.Private[$scope.SysUserCode].SelectDW;
    }
}]);

App.controller('IndexReceiveCtrl', ['$scope', function ($scope) {

    if (!$scope.Receive.Box[$scope.Receive.Box.Current].Inited) {
        $scope.Fn.TabSearch.Refresh.Server();
        $scope.Receive.Box[$scope.Receive.Box.Current].Inited = true;
    }
}]);

App.controller('IndexRecycleBinCtrl', ['$scope', function($scope) {
    
    if (!$scope.RecycleBin.Box[$scope.RecycleBin.Box.Current].Inited) {
        $scope.Fn.TabSearch.Refresh.Server();
        $scope.RecycleBin.Box[$scope.RecycleBin.Box.Current].Inited = true;
    }
}]);

App.controller('IndexUrgeReportCtrl', ['$scope', function($scope) {
    $scope.FCQ = $scope.Fn.Check.Quot;
    $scope.FCL = $scope.Fn.Check.Length;
}]);

App.service('BaseData', function() {
    var baseData = window.basedata;
    baseData.Unit.Local = {
        Limit: parseInt($.cookie("limit")),
        UnitCode: $.cookie("unitcode"),
        UnitName: $.cookie("unitname"),
        RealName: $.cookie("realname"),
        RiverCode: baseData.Unit.RiverCode
    };
    delete baseData.Unit.RiverCode;

    return baseData;
});
