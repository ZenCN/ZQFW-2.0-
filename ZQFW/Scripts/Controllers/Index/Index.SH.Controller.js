App.controller('IndexCtrl', [
    '$rootScope', '$state', '$http', '$timeout', 'screen', 'ngCss', function ($scope, $state, $http, $timeout, screen, ngCss) {
        $scope.SysUser = {
            Code: $.cookie("unitcode").slice(0, 2),
            Name: $.cookie("realname")
        }
        $scope.Attr = {
            NameSpace: "Open",
            RootName: "Index"
        };
        $scope.BaseData = window.initData.BaseData;
        $scope.BaseData.Unit.Local = {
            UnitCode: $.cookie('unitcode'),
            UnitName: $.cookie('unitname'),
            Limit: $.cookie('limit')
        };
        delete $scope.BaseData.Unit.SH.SH03.SH031;  //也可直接删除对应的计划数据即可，这样没有计划数据的表就自动不会显示
        delete $scope.BaseData.Unit.SH.SH03.SH032;

        $scope.InitTree = function (tree, simpleData, setting) {
            if (typeof (tree) == "string") {
                tree = $(".ztree#" + tree);
            }

            return $.fn.zTree.init(tree, $.extend({
                data: {
                    simpleData: { enable: true }
                },
                view: {
                    selectedMulti: false,
                    fontCss: function (treeId, treeNode) {
                        return treeNode.is_used ? { color: "rgb(150, 149, 149)"} : {};
                    }
                }
            }, setting), simpleData);
        };

        $scope.Dialog = {
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
                    Width: "inherit",
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

        $scope.Fn = {
            GetEvt: function() {
                return $(event.target || event.srcElement);
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
            UpdatePassword: function() {
                $scope.Dialog.Config({
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
            TabSearch: {
                CheckAll: function() {
                    var name = $state.current.url.replace('/', '');
                    $.each($scope[name].Box[$scope[name].Box.Current].Fake, function() {
                        this.Checked = (event.srcElement ? event.srcElement : event.target).checked;
                    });
                },
                ChangeCondition: function() {
                    var name = $state.current.url.replace('/', '');
                    if ($scope[name].Box[$scope[name].Box.Current].Real.length > 0) {
                        $scope.Fn.TabSearch.Refresh.Native();
                    }
                    $.each($scope[name].Box.Name, function(i, key) {
                        $scope[name].Box[key].Refresh = true;
                    });
                    $scope[name].Box[$scope[name].Box.Current].Refresh = false;
                },
                Refresh: {
                    Server: function(nameSpace) {
                        nameSpace = nameSpace ? nameSpace : $state.current.url.replace('/','');

                        var curPage = $scope[nameSpace];
                        var attr = curPage.Attr;
                        var unitcode = $scope.BaseData.Unit.Local.UnitCode;
                        var param = {
                            unitCode: ["", null].In_Array(attr[curPage.Box.Current].UnitCode) ? unitcode : attr[curPage.Box.Current].UnitCode,
                            //rptType: ["", null].In_Array(attr[curPage.Box.Current].ORD_Code) ? [] : attr[curPage.Box.Current].ORD_Code,
                            startTime: attr[curPage.Box.Current].StartDateTime,
                            endTime: attr[curPage.Box.Current].EndDateTime,
                            cycType: attr[curPage.Box.Current].StatisticalCycType == "" ? -1 : attr[curPage.Box.Current].StatisticalCycType,
                            type: nameSpace
                        };

                        if (["", null].In_Array(attr[curPage.Box.Current].ORD_Code)) {
                            param.rptType = 'SH01, SH02, SH03, SH04';
                        } else {
                            param.rptType = attr[curPage.Box.Current].ORD_Code;
                        }

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
                        nameSpace = nameSpace ? nameSpace : $state.current.url.replace('/','');
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
                                fields = fields.slice(0, 3); //接收表箱不需要发送的起止时间作为筛选条件
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
                                        } else if (fields[i] == "ORD_Code" && obj[fields[i]] == toCompareValue) {
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
                        ngCss(".ListHorizontal ul.Head", curPage.Box.Current); //Change the head width
                    }
                },
                ViewReport: function(report) {
                    /*var curPage = $scope[$state.current.url.replace('/','')];
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
                    $scope.Attr.Preview.Content = curPage.Attr.Report.Content;*/
                },
                SwitchBox: function(boxName) {
                    var curPage = $scope[$state.current.url.replace('/','')];
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
        }
        //-------------------Menu State---------------------
        $scope.Menu = {
            Current: $state.current.url.replace('/', ''),
            RedirectTo: function (name) {
                if ($.isFunction($scope[name].Init)) {
                    $scope[name].Init();
                    $scope[name].Init = true;

                    if (name == 'Receive') {
                        $scope.Fn.TabSearch.Refresh.Server('Receive');
                    }
                }

                this.Current = name;
                $state.go("Head.Menu." + this.Current);
            }
        };
        //-------------------New State----------------------
        $scope.New = {
            Init: function () {
                if (angular.isObject(window.initData.New)) {
                    $.extend($scope.New.Report, window.initData.New.Report);
                    $scope.New.Fn.FormatTime();

                    $scope.BaseData.Select = {
                        RptClass: [],
                        CycType: [$scope.New.Report.CycType]
                    };
                    var obj = undefined;
                    angular.forEach($scope.BaseData.Unit.SH, function (obj, key) {
                        if ($.isEmptyObject(obj)) {
                            $scope.New.Report.TreeData.children = $scope.New.Report.TreeData.children.RemoveBy("id", key);
                        } else {
                            obj = $scope.New.Report.TreeData.children.Find('id', key);
                            if (!$.isEmptyObject(obj)) {
                                $scope.BaseData.Select.RptClass.push({
                                    Value: obj.id,
                                    Name: obj.name
                                });
                            }
                        }
                    });

                    $scope.BaseData.Select.RptClass = [
                        { "Value": "SH01", "Name": "资金情况统计表" },
                        { "Value": "SH02", "Name": "山洪沟治理进度统计表" },
                        { "Value": "SH03", "Name": "非工程措施补充完善表" },
                        { "Value": "SH04", "Name": "调查评价表" },
                        { "Value": 'SH05', "Name": '效益发挥统计表' }
                    ];

                    $scope.New.Report.Type.Code = $scope.BaseData.Select.RptClass[0].Value;
                    $scope.New.Report.Type.Name = $scope.BaseData.Select.RptClass[0].Name;

                    this.SameReport.Search();

                    this.Init = false;
                }
            },
            SameReport: {
                Content: [],
                Current: {},
                Search: function (apply) {
                    /*var time = $scope.New.Report.Date.TimeSpan.Selected.replace('日', '').split('月');
                    if (Number(time[1]) > 15) {
                        time = $scope.New.Report.Date.Year + '年' + time[0] + '月16日';
                    } else {
                        time = $scope.New.Report.Date.Year + '年' + time[0] + '月01日';
                    }*/
                    var time = $scope.New.Report.Time_Now.replace('年','-').replace('月','-').replace('日','');

                    this.Content = $scope.Fn.Ajax("Index/SearchSameReport", {
                        StatisticalCycType: $scope.New.Report.CycType.Code,
                        SourceType: $scope.BaseData.Unit.Local.Limit == 4 ? 0 : 1,
                        StartDateTime: time + ' 00:00:00',
                        EndDateTime: time + ' 23:59:59',
                        ORD_Code: $scope.New.Report.Type.Code
                    }).reportList;
                    this.Current = undefined;

                    if (apply) {
                        $scope.$apply();
                    }
                }
            },
            Report: {
                TreeData: [],
                Type: {
                    Name: undefined,
                    Code: undefined
                },
                CycType: {
                    Name: undefined,
                    Value: undefined,
                    Title: undefined
                },
                Time_Now: $scope.Fn.Ajax('Index/GetTime?format=yyyy年MM月dd日'),
                Date: {
                    Time: {
                        Array: undefined
                    }
                },
                Same: {
                    Array: undefined,
                    Selected: undefined
                }
            },
            Fn: {
                FormatTime: function () {
                    var arr, month = new Date().getMonth() + 1, last_day = new Date().getDate() > 16 ? true : false;
                    angular.forEach($scope.New.Report.Date.TimeSpan.Array, function (time) {
                        arr = time.replace("月", ".").replace("日", "").split(".");
                        arr[1] = Number(arr[1]);
                        if (Number(arr[0]) == month) {
                            if (last_day && arr[1] > 16 || !last_day && arr[1] <= 16) {
                                $scope.New.Report.Date.TimeSpan.Selected = time;
                                return false;
                            }
                        }
                    });
                    $scope.New.Report.Date.Year = new Date().getFullYear();
                },
                Report: {
                    Create: function () {
                        if ($scope.Open.Report.Current && $scope.Open.Report.Current.ReportTitle.PageNO == 0) {
                            Alert("只能同时新建一套报表");
                            return;
                        }

                        var fn = function (_this, time_span, year) {
                            var time = time_span.replace('日', '').split("月");
                            year = year ? year : $scope.New.Report.Date.Year;
                            if (Number(time[1]) > 15) {
                                _this.StartDateTime = year + '-' + time[0] + "-16";
                            } else {
                                _this.StartDateTime = year + '-' + time[0] + "-01";
                            }
                            _this.EndDateTime = year + "-" + time_span.replace('月', '-').replace('日', '');
                        };

                        var obj = {
                            PageNO: 0,
                            ORD_Code: $scope.New.Report.Type.Code,
                            RPTType_Code: "XZ0",
                            StatisticalCycType: $scope.New.Report.CycType.Code,
                            UnitName: $scope.BaseData.Unit.Local.UnitName,
                            UnitCode: $scope.BaseData.Unit.Local.UnitCode,
                            SourceType: $scope.BaseData.Unit.Local.Limit < 4 ? 1 : 0,
                            State: 0
                        };
                        fn(obj, $scope.New.Report.Date.TimeSpan.Selected);

                        $scope.Open.Report.Current = {
                            ReportTitle: new App.Models.ReportTitle(obj),
                            Attr: {
                                TableIndex: 0,
                                ReportFooter: 'ReportTitle',
                                Previous: {}, //上一期数据
                                WrongData: []
                            }
                        };
                        if ($scope.Open.Report.Current.ReportTitle.ORD_Code == 'SH03') {
                            $scope.Open.Report.Current.Attr.TableIndex = 5;
                        }

                        $scope.Open.Report.Selected.ORD_Code = $scope.New.Report.Type.Code;

                        var params = {
                            rptType: $scope.Open.Report.Current.ReportTitle.ORD_Code
                        };
                        angular.forEach($scope.New.Report.Date.TimeSpan.Array, function (time, i) {
                            if (time == $scope.New.Report.Date.TimeSpan.Selected) {
                                if (i > 0) {
                                    fn(params, $scope.New.Report.Date.TimeSpan.Array[i - 1]);
                                } else {
                                    fn(params, $scope.New.Report.Date.TimeSpan.Array.Last(), 2015);
                                }
                            }
                        });

                        params.StartDateTime = new Date().getFullYear() + "-01-01 00:00:00";
                        params.EndDateTime = $scope.New.Report.Time_Now + " 23:59:59";
                        
                        $scope.Fn.Ajax("/sh/getrecentrpt", params, 'get', function (data) {
                            if (!$.isEmptyObject(data)) {
                                $scope.Open.Report.Current.ReportTitle = $.extend(data.ReportTitle, obj);
                                if ($scope.New.Report.Type.Code == 'SH05') {
                                    $scope.Open.Report.Current.SH051 = data.SH051.RemoveAttr(["PageNO", "EntityKey", "TBNO"]);
                                    $scope.Open.Report.Current.Attr.Previous.SH051 = angular.copy($scope.Open.Report.Current.SH051);
                                } else {
                                    $.each($scope.BaseData.Unit.SH[$scope.New.Report.Type.Code], function (table) {
                                        if (this.length > 0) {
                                            if (data[table].length > 0) {
                                                $scope.Open.Report.Current[table] = data[table].RemoveAttr(["PageNO", "EntityKey", "TBNO"]);
                                                $scope.Open.Report.Current.Attr.Previous[table] = angular.copy($scope.Open.Report.Current[table]);
                                            } else {
                                                $scope.Open.Report.Current[table] = new App.Models.SH[table](this);
                                            }
                                        }
                                    });
                                }
                            } else {
                                if ($scope.New.Report.Type.Code == 'SH05') {
                                    $scope.Open.Report.Current.SH051 = App.Models.SH.SH051($scope.BaseData.Unit.Unders);
                                    $scope.Open.Report.Current.SH051.InsertAt(0, App.Models.SH.SH051([
                                        { UnitName: '合计', UnitCode: $scope.BaseData.Unit.Local.UnitCode }
                                    ])[0]);
                                } else {
                                    $.each($scope.BaseData.Unit.SH[$scope.New.Report.Type.Code], function (table) {
                                        if (this.length > 0) {
                                            $scope.Open.Report.Current[table] = new App.Models.SH[table](this);
                                        }
                                    });
                                }
                            }

                            if (["SH01", "SH03"].In_Array($scope.New.Report.Type.Code)) {
                                if ($scope.New.Report.Type.Code == "SH01") {
                                    if ($.isEmptyObject($scope.Open.Report.Current.SH011.Find("JSNR", "合计"))) {
                                        $scope.Open.Report.Current.SH011.InsertAt(0, $.extend(App.Models.SH.SH011.Object(), {
                                            DataOrder: 0,
                                            UnitCode: 'HJ',
                                            JSNR: '合计'
                                        }));
                                        $scope.FCS("NDZJYS");   //Sum计划数据
                                    }
                                } else {
                                    $.each($scope.Open.Report.Current, function(key, val) {
                                        if (key.indexOf("SH03") >= 0) {
                                            if ($.isEmptyObject(val.Find("SSX", "合计"))) {
                                                $scope.Open.Report.Current[key].InsertAt(0, $.extend(App.Models.SH[key].Object(), {
                                                    DataOrder: 0,
                                                    UnitCode: 'HJ',
                                                    SSX: '合计'
                                                }));

                                                if (App.Config.Table.PlanFields[key]) { //上期没有数据，需合计计划数据
                                                    $scope.FCS({
                                                        Table: key,
                                                        Fields: App.Config.Table.PlanFields[key]
                                                    }, 0);
                                                }
                                            }
                                        }
                                    });
                                }
                            }

                            $scope.Open.Report.Current.ReportTitle.WriterTime = $scope.Fn.Ajax('Index/GetTime');
                            $scope.Open.Report.Current.ReportTitle.StartDateTime = $scope.Open.Report.Current.ReportTitle.WriterTime;
                            $scope.Open.Report.Current.ReportTitle.EndDateTime = $scope.Open.Report.Current.ReportTitle.WriterTime;
                            $scope.Open.Report.Opened.push($scope.Open.Report.Current);

                            var refresh = true;
                            if ($scope.Open.Report.Current.ReportTitle.SourceType == 1) {
                                $scope.Open.Report.Tree.Current.Tab = "ReceivedRptTree";
                                if ($scope.Open.Report.Tree.Data[$scope.Open.Report.Current.ReportTitle.ORD_Code].ReceivedRptTree) {
                                    refresh = false;
                                }
                                $scope.Open.Report.Current.Attr.AggAcc = {
                                    Content: [],
                                    Selected: undefined
                                }
                            } else {
                                $scope.Open.Report.Tree.Current.Tab = "MyRptTree";
                                if ($scope.Open.Report.Tree.Data[$scope.Open.Report.Current.ReportTitle.ORD_Code].MyRptTree) {
                                    refresh = false;
                                }
                            }
                            $scope.Open.Report.Tree.Fn.Switch($scope.Open.Report.Tree.Current.Tab, refresh, $scope.Open.Report.Current.ReportTitle.ORD_Code)
                            
                            $scope.Menu.RedirectTo("Open");
                        });
                    },
                    Open: function () {
                        if ($scope.New.SameReport.Current && $scope.New.SameReport.Current.PageNO) {
                            $scope.Open.Fn.Core.Open({
                                rptType: $scope.New.Report.Type.Code,
                                sourceType: Number($scope.New.SameReport.Current.SourceType),
                                pageno: $scope.New.SameReport.Current.PageNO,
                                apply: false
                            });

                            $scope.Menu.RedirectTo('Open');
                        } else {
                            Alert("未选择报表");
                        }
                    }
                }
            }
        };
        $scope.New.Init();
        //-------------------Open State----------------------
        $scope.Open = {
            Init: function() {
                $scope.Open.Report.Tree.Data.Load();

                $scope.Open.Init = false;
            },
            Report: {
                Selected: {
                    ORD_Code: $scope.BaseData.Select.RptClass[0].Value,
                    StatisticalCycType: $scope.BaseData.Select.CycType[0].Code,
                    UnderUnitCode: $scope.BaseData.Unit.Local.UnitCode
                },
                Screen: {
                    State: 'inhreit',
                    Fn: screen
                },
                Opened: [],
                Current: undefined,
                Tree: {
                    Data: {
                        SH01: {},
                        SH03: {},
                        SH04: {},
                        SH05: {},
                        Load: function (rptType, treeType) {
                            rptType = rptType ? rptType : $scope.Open.Report.Selected.ORD_Code;
                            treeType = treeType ? treeType : $scope.Open.Report.Tree.Current.Tab;

                            var params = {
                                unitCode: $scope.BaseData.Unit.Local.UnitCode,
                                unitLimit: $scope.BaseData.Unit.Local.Limit,
                                rptClass: rptType,
                                cycType: $scope.Open.Report.Selected.StatisticalCycType,
                                limitType: treeType == "MyRptTree" ? 0 : 1,
                                minYear: new Date().getFullYear() - 1
                            };

                            if (treeType == 'ReceivedRptTree' && $scope.Open.Report.Selected.UnderUnitCode != $scope.BaseData.Unit.Local.UnitCode) {
                                params.unitCode = $scope.Open.Report.Selected.UnderUnitCode;
                            }

                            $.ajax({
                                url: 'SH/GetTreeData',
                                data: params,
                                async: false,
                                success: function (response) {
                                    $scope.Open.Report.Tree.Data[rptType] = $scope.Open.Report.Tree.Data[rptType] || {};
                                    $scope.Open.Report.Tree.Data[rptType][treeType] = JSON.parse(response);
                                    $scope.Open.Report.Tree.Current.SimpleData = $scope.Open.Report.Tree.Data[rptType][treeType];
                                }
                            });
                        }
                    },
                    State: {},
                    Current: {
                        Tab: 'MyRptTree',
                        Node: undefined,
                        SimpleData: undefined,
                        State: {
                            SH01: {
                                MyRpt: new App.Models.TreeModel(),
                                ReceivedRpt: new App.Models.TreeModel()
                            }
                        }
                    },
                    Fn: {
                        Switch: function (treeType, refresh, rptType) {
                            treeType = treeType ? treeType : $scope.Open.Report.Tree.Current.Tab;
                            refresh = refresh ? refresh : false;
                            rptType = rptType ? rptType : $scope.Open.Report.Selected.ORD_Code;

                            $scope.Open.Report.Tree.Data[rptType] = $scope.Open.Report.Tree.Data[rptType] || {};
                            if (!$scope.Open.Report.Tree.Data[rptType][treeType] || refresh) {
                                $scope.Open.Report.Tree.Data.Load(rptType, treeType);
                            }
                            $scope.Open.Report.Tree.Current.SimpleData = $scope.Open.Report.Tree.Data[rptType][treeType];
                            $scope.Open.Report.Tree.Current.Tab = treeType;

                            return this;
                        },
                        Refresh: {
                            Report: function () {
                                var openReport = undefined;
                                if ($scope.Open.Report.Tree.Current.Tab == "MyRptTree") {
                                    openReport = function (event, treeId, treeNode) {
                                        if (treeNode.id) {
                                            var isCurYear = treeNode.getParentNode().getParentNode().name.indexOf(new Date().getFullYear() + '年') >= 0;
                                            var obj = {
                                                rptType: $scope.Open.Report.Selected.ORD_Code,
                                                sourceType: treeNode.SourceType,
                                                pageno: treeNode.id,
                                                is_cur_year: isCurYear,
                                                apply: true
                                            };
                                            $scope.Open.Fn.Core.Open(obj);
                                        }
                                    };
                                } else {
                                    openReport = function (event, treeId, treeNode) {
                                        if (treeNode.id) {
                                            $scope.Open.Fn.Core.ViewUnderRpt({
                                                ORD_Code: $scope.Open.Report.Selected.ORD_Code,
                                                SourceType: treeNode.SourceType,
                                                Limit: parseInt($scope.BaseData.Unit.Local.Limit) + 1,
                                                UnitCode: treeNode.UnitCode,
                                                PageNO: treeNode.id
                                            });
                                        }
                                    };
                                }

                                var config = {
                                    check: { enable: true },
                                    callback: {
                                        onClick: openReport,
                                        onExpand: function(event, treeId, treeNode) {
                                            if (treeNode.level == 1) {
                                                $scope.Open.Report.Tree.Fn.FindNodeState(treeId, treeNode.name.replace("月", "")).Expand = true;
                                            }
                                        },
                                        onCollapse: function(event, treeId, treeNode) {
                                            if (treeNode.level == 1) {
                                                $scope.Open.Report.Tree.Fn.FindNodeState(treeId, treeNode.name.replace("月", "")).Expand = false;
                                            }
                                        }
                                    },
                                    data: {
                                        simpleData: { enable: true },
                                        key: { title: "title" }
                                    }
                                };

                                if ($scope.Open.Report.Tree.Current.Tab == "ReceivedRptTree") {
                                    config.callback.beforeCheck = function (treeId, treeNode) {
                                        var zTree = $.fn.zTree.getZTreeObj($scope.Open.Report.Tree.Current.Tab);
                                        var checked = zTree.getCheckedNodes(), exist = false;

                                        $.each(checked, function() {
                                            if (treeNode.UnitCode == this.UnitCode && treeNode.id != this.id) {
                                                Alert("相同的单位只能选一个");
                                                exist = true;
                                                return false;
                                            }
                                        });

                                        return !exist;
                                    };
                                }

                                this.CheckBox($scope.InitTree($scope.Open.Report.Tree.Current.Tab, $scope.Open.Report.Tree.Current.SimpleData, config));

                                if (angular.isFunction($scope.Open.Report.Tree.Current.CallBack)) {
                                    $scope.Open.Report.Tree.Current.CallBack(); //尽量不要往CallBack中加参数
                                    delete $scope.Open.Report.Tree.Current.CallBack;
                                }

                                return this;
                            },
                            CheckBox: function (zTree) {
                                zTree = zTree ? zTree : $.fn.zTree.getZTreeObj($scope.Open.Report.Tree.Current.Tab);
                                var enable = false;
                                if ($scope.Open.Report.Current) {
                                    switch (Number($scope.Open.Report.Current.ReportTitle.SourceType)) {
                                        case 1:
                                            if ($scope.Open.Report.Tree.Current.Tab == "MyRptTree") {
                                                enable = false;
                                            } else {
                                                enable = true;
                                            }
                                            break;
                                        case 2:
                                            if ($scope.Open.Report.Tree.Current.Tab == "MyRptTree") {
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
                            NodeState: function (tree_id) {
                                tree_id = tree_id ? tree_id : $scope.Open.Report.Tree.Current.Tab;
                                $timeout(function () {
                                    var zTreeObj = $.fn.zTree.getZTreeObj(tree_id);
                                    var nodes = undefined;
                                    $.each($scope.Open.Report.Tree.Fn.FindNodeState(tree_id), function (month) {
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
                        FindNodeState: function (treeId, nodeName) {
                            if (!$scope.Open.Report.Tree.State[$scope.Open.Report.Selected.ORD_Code]) {
                                $scope.Open.Report.Tree.State[$scope.Open.Report.Selected.ORD_Code] = {
                                    MyRptTree: new App.Models.TreeModel(),
                                    ReceivedRptTree: new App.Models.TreeModel()
                                };
                            }

                            if (nodeName) {
                                return $scope.Open.Report.Tree.State[$scope.Open.Report.Selected.ORD_Code][$scope.Open.Report.Tree.Current.Tab][nodeName];
                            } else {
                                return $scope.Open.Report.Tree.State[$scope.Open.Report.Selected.ORD_Code][$scope.Open.Report.Tree.Current.Tab];
                            }
                        }
                    }
                }
            },
            Fn: {
                Core: {
                    AggAcc: {
                        View: function (_this) {
                            var obj = {
                                ORD_Code: $scope.Open.Report.Selected.ORD_Code,
                                SourceType: _this.SourceType,
                                UnitCode: _this.UnitCode,
                                PageNO: _this.id,
                                queryUnderUnits: true
                            };

                            if ($scope.Open.Report.Current.ReportTitle.SourceType == 1) {  //汇总
                                obj.Limit = parseInt($scope.BaseData.Unit.Local.Limit) + 1;
                            }

                            $scope.Open.Fn.Core.ViewUnderRpt(obj);
                        },
                        Sum: function() {
                            var zTree = $.fn.zTree.getZTreeObj($scope.Open.Report.Tree.Current.Tab);
                            var checkedNodes = zTree.getCheckedNodes();
                            if (checkedNodes.length == 0) {
                                Alert('未选择报表');
                                return;
                            }

                            var pagenos = [];
                            var exit = false;
                            $.each(checkedNodes, function () {
                                exit = $scope.Open.Report.Current.Attr.AggAcc.Content.SomeBy("id", this.id);
                                if (exit) {
                                    Alert("不能添加相同的报表");
                                    return false;
                                }

                                exit = $scope.Open.Report.Current.Attr.AggAcc.Content.SomeBy("UnitCode", this.UnitCode);
                                if (exit) {
                                    Alert($scope.BaseData.Unit.Unders.Find('UnitCode', this.UnitCode, 'UnitName') + "已添加过");
                                    return false;
                                }

                                pagenos.push(this.id);
                                $scope.Open.Report.Current.Attr.AggAcc.Content.push({
                                    id: this.id,
                                    Name: this.name,
                                    UnitCode: this.UnitCode
                                });
                                zTree.checkNode(this, false);
                            });
                            if (exit) {
                                return;
                            }

                            var hj, obj, fields;
                            $http.get("SH/SumReport?pagenos=" + pagenos.join(",") + "&ord_code=" + $scope.Open.Report.Current.ReportTitle.ORD_Code)
                                .then(function(response) {
                                    //console.log(response.data);
                                    if (angular.isObject(response.data)) {
                                        switch ($scope.Open.Report.Current.ReportTitle.ORD_Code) {
                                        case 'SH01':
                                            fields = ['TBNO', 'UnitCode', 'PageNO', 'DataOrder', 'EntityKey', 'JSNR', 'NDZJYS', 'NDZJZY', 'NDZJDF'];

                                            if (!$scope.Open.Report.Current.Attr.AggAcc.isAdded) {
                                                var old_rpt = $scope.Open.Report.Current.SH011;
                                                $scope.Open.Report.Current.SH011 = App.Models.SH.SH011([
                                                    { UnitCode: 'HJ', JSNR: '合计' },
                                                    { UnitCode: 'DCPJ', JSNR: '调查评价' },
                                                    { UnitCode: 'FGCCS', JSNR: '非工程措施补充完善' },
                                                    { UnitCode: 'ZDSHG', JSNR: '重点山洪沟防洪治理' }
                                                ]);
                                                $.each($scope.Open.Report.Current.SH011, function(i) {
                                                    this.NDZJYS = old_rpt[i].NDZJYS;
                                                    this.NDZJZY = old_rpt[i].NDZJZY;
                                                    this.NDZJDF = old_rpt[i].NDZJDF;
                                                });

                                                $scope.Open.Report.Current.Attr.AggAcc.isAdded = true;
                                            }
                                            hj = $scope.Open.Report.Current.SH011[0];

                                            $.each(response.data.SH011, function() {
                                                obj = $scope.Open.Report.Current.SH011.Find('UnitCode', this.UnitCode);
                                                if (!$.isEmptyObject(obj)) {
                                                    delete this.$id;
                                                    $.each(this, function(field, val) {
                                                        if (!fields.In_Array(field) && Number(val) > 0) {
                                                            obj[field] = App.Tools.Calculator.Addition(obj[field], val);
                                                            hj[field] = App.Tools.Calculator.Addition(hj[field], val);
                                                        }
                                                    });
                                                }

                                                $scope.FCDVD(['JDWCZY', 'NDZJZY', 'JDZYBL'], [obj, hj]);
                                                $scope.FCDVD(['ZFWCZY', 'NDZJZY', 'ZFZYBL'], [obj, hj]);
                                                $scope.FCDVD(['JDWCSJ', 'DFZJSJ', 'JDSJBL'], [obj, hj]);
                                                $scope.FCDVD(['ZFWCSJ', 'DFZJSJ', 'ZFSJBL'], [obj, hj]);
                                                $scope.FCDVD(['JDWCSHIJ', 'DFZJSX', 'JDSHIJBL'], [obj, hj]);
                                                $scope.FCDVD(['ZFWCSHIJ', 'DFZJSX', 'ZFSHIJBL'], [obj, hj]);
                                                $scope.FCDVD(['JDWCXJ', 'DFZJX', 'JDXJBL'], [obj, hj]);
                                                $scope.FCDVD(['ZFWCXJ', 'DFZJX', 'ZFXJBL'], [obj, hj]);
                                            });
                                            break;
                                        case 'SH03':
                                            if (!$scope.Open.Report.Current.Attr.AggAcc.isAdded) {
                                                var old_hj = {};
                                                $.each($scope.BaseData.Unit.SH.SH03, function(table) {
                                                    if (this.length > 0) {
                                                        old_hj[table] = $scope.Open.Report.Current[table][0];
                                                        $scope.Open.Report.Current[table] = new App.Models.SH[table]([
                                                            {
                                                                DataOrder: 0,
                                                                UnitCode: 'HJ',
                                                                SSX: '合计'
                                                            }
                                                        ].concat(this));
                                                        hj = $scope.Open.Report.Current[table][0];
                                                        angular.forEach(App.Config.Table.PlanFields[table], function(field) {
                                                            hj[field] = old_hj[table][field];
                                                        });
                                                        $scope.Open.Report.Current.Attr.AggAcc.isAdded = true;
                                                    }
                                                });
                                            };

                                            var fields = {
                                                SH036: ["XYAWC", "XZYAWC", "CYAWC", "QTYAWC"],
                                                SH037: ["XCLWC", "JSPWC", "MBKWC", "GPWC", "SCWC"],
                                                SH038: ["PXCCWC", "PXRSWC", "YLCCWC", "YLRCWC"]
                                            };
                                            $.each(response.data, function(table) {
                                                hj = $scope.Open.Report.Current[table][0];
                                                $.each(this, function() {
                                                    obj = $scope.Open.Report.Current[table].Find('UnitCode', this.UnitCode);
                                                    $.each(this, function(field, val) {
                                                        if (fields[table].In_Array(field) && Number(val) > 0) {
                                                            obj[field] = App.Tools.Calculator.Addition(obj[field], val);
                                                            hj[field] = App.Tools.Calculator.Addition(hj[field], val);
                                                        }
                                                    });
                                                });
                                            });
                                            break;
                                        case 'SH05':
                                            fields = ['TBNO', 'UnitCode', 'PageNO', 'DataOrder', 'EntityKey'];
                                            if (!$scope.Open.Report.Current.Attr.AggAcc.isAdded) {
                                                $scope.Open.Report.Current.SH051 = App.Models.SH.SH051($scope.BaseData.Unit.Unders);
                                                $scope.Open.Report.Current.SH051.InsertAt(0, App.Models.SH.SH051([
                                                    { UnitName: '合计', UnitCode: $scope.BaseData.Unit.Local.UnitCode }
                                                ])[0]);
                                                $scope.Open.Report.Current.Attr.AggAcc.isAdded = true;
                                            };

                                            var hj = $scope.Open.Report.Current.SH051[0];
                                            $.each(response.data.SH051, function() {
                                                obj = $scope.Open.Report.Current.SH051.Find('UnitCode', this.UnitCode);
                                                $.each(this, function(field, val) {
                                                    if (!fields.In_Array(field) && Number(val) > 0) {
                                                        obj[field] = App.Tools.Calculator.Addition(obj[field], val);
                                                        hj[field] = App.Tools.Calculator.Addition(hj[field], val);
                                                    }
                                                });
                                            });
                                            break;
                                        }
                                    } else {
                                        Alert(response.data);
                                    }
                                });
                        },
                        Sub: function() {
                            if (!$scope.Open.Report.Current.Attr.AggAcc.Selected) {
                                Alert("未选择报表");
                                return false;
                            }

                            var except_fields = ['$id', 'TBNO', 'UnitCode', 'PageNO', 'DataOrder', 'EntityKey'];
                            $http.get("SH/SumReport?pagenos=" + $scope.Open.Report.Current.Attr.AggAcc.Selected.id + "&ord_code=" + $scope.Open.Report.Current.ReportTitle.ORD_Code)
                                .then(function (response) {
                                    //console.log(response.data);
                                    if (angular.isObject(response.data)) {
                                        switch ($scope.Open.Report.Current.ReportTitle.ORD_Code) {
                                            case 'SH01':
                                                except_fields = except_fields.concat(['JSNR', 'NDZJYS', 'NDZJZY', 'NDZJDF']);
                                                var hj = $scope.Open.Report.Current.SH011[0], obj;
                                                $.each(response.data.SH011, function () {
                                                    obj = $scope.Open.Report.Current.SH011.Find('UnitCode', this.UnitCode);
                                                    if (!$.isEmptyObject(obj)) {
                                                        delete this.$id;
                                                        $.each(this, function (field, val) {
                                                            if (!except_fields.In_Array(field) && Number(val) > 0) {
                                                                obj[field] = App.Tools.Calculator.Subtraction(obj[field], val);
                                                                hj[field] = App.Tools.Calculator.Subtraction(hj[field], val);
                                                            }
                                                        });
                                                    }

                                                    $scope.FCDVD(['JDWCZY', 'NDZJZY', 'JDZYBL'], [obj, hj]);
                                                    $scope.FCDVD(['ZFWCZY', 'NDZJZY', 'ZFZYBL'], [obj, hj]);
                                                    $scope.FCDVD(['JDWCSJ', 'DFZJSJ', 'JDSJBL'], [obj, hj]);
                                                    $scope.FCDVD(['ZFWCSJ', 'DFZJSJ', 'ZFSJBL'], [obj, hj]);
                                                    $scope.FCDVD(['JDWCSHIJ', 'DFZJSX', 'JDSHIJBL'], [obj, hj]);
                                                    $scope.FCDVD(['ZFWCSHIJ', 'DFZJSX', 'ZFSHIJBL'], [obj, hj]);
                                                    $scope.FCDVD(['JDWCXJ', 'DFZJX', 'JDXJBL'], [obj, hj]);
                                                    $scope.FCDVD(['ZFWCXJ', 'DFZJX', 'ZFXJBL'], [obj, hj]);
                                                });
                                                break;
                                            case 'SH03':
                                                var fields = {
                                                    SH036: ["XYAWC", "XZYAWC", "CYAWC", "QTYAWC"],
                                                    SH037: ["XCLWC", "JSPWC", "MBKWC", "GPWC", "SCWC"],
                                                    SH038: ["PXCCWC", "PXRSWC", "YLCCWC", "YLRCWC"]
                                                };
                                                $.each(response.data, function (table) {
                                                    hj = $scope.Open.Report.Current[table][0];
                                                    $.each(this, function () {
                                                        obj = $scope.Open.Report.Current[table].Find('UnitCode', this.UnitCode);
                                                        $.each(this, function (field, val) {
                                                            if (fields[table].In_Array(field) && Number(val) > 0) {
                                                                obj[field] = App.Tools.Calculator.Subtraction(obj[field], val);
                                                                hj[field] = App.Tools.Calculator.Subtraction(hj[field], val);
                                                            }
                                                        });
                                                    });
                                                });
                                                break;
                                            case 'SH05':
                                                fields = ['TBNO', 'UnitCode', 'PageNO', 'DataOrder', 'EntityKey'];
                                                var hj = $scope.Open.Report.Current.SH051[0];
                                                $.each(response.data.SH051, function () {
                                                    obj = $scope.Open.Report.Current.SH051.Find('UnitCode', this.UnitCode);
                                                    $.each(this, function (field, val) {
                                                        if (!fields.In_Array(field) && Number(val) > 0) {
                                                            obj[field] = App.Tools.Calculator.Subtraction(obj[field], val);
                                                            hj[field] = App.Tools.Calculator.Subtraction(hj[field], val);
                                                        }
                                                    });
                                                });
                                                break;
                                        }

                                        $scope.Open.Report.Current.Attr.AggAcc.Content = $scope.Open.Report.Current.Attr.AggAcc.Content.RemoveBy('id', $scope.Open.Report.Current.Attr.AggAcc.Selected.id);
                                        $scope.Open.Report.Current.Attr.AggAcc.Selected = undefined;
                                    }
                                });
                        }
                    },
                    ViewUnderRpt: function (obj) {
                        var url = "SH/ViewUnderReport?rptType=" + obj.ORD_Code + "&limit=" + obj.Limit + "&unitcode="
                            + obj.UnitCode + "&pageno=" + obj.PageNO

                        if (window.location.href.toLowerCase().indexOf("debug=1") > 0) {
                            url += "&debug=1";
                        }

                        window.open(url, "", "_blank", "");
                    },
                    Delete: {
                        Report: function () {
                            if (confirm("确认删除么？")) {
                                //$scope.New.SameReport.Content = $scope.New.SameReport.Content.RemoveBy("PageNO", $scope.Open.Report.Current.ReportTitle.PageNO);
                                var result = $scope.Fn.Ajax("Index/DeleteReport", {
                                    type: "0",
                                    pageno: $scope.Open.Report.Current.ReportTitle.PageNO,
                                    state: $scope.Open.Report.Current.ReportTitle.State,
                                    limit: $.cookie("limit")
                                });

                                if (result > 0) {
                                    /*if ($scope.Open.Tree.Current.TreeId.indexOf("MyRptTree") > 0) {
                                    $scope.New.SameReport.Content.RemoveBy("PageNO", $scope.Open.Report.Current.ReportTitle.PageNO);
                                    }*/

                                    $scope.Open.Report.Tree.Fn.Switch("MyRptTree", true).Refresh.Report().NodeState();
                                    if ($scope.Open.Report.Current.ReportTitle.SourceType == 1) { //删除汇总表，更新已收表箱
                                        $scope.Open.Report.Tree.Data.Load(false, "ReceivedRptTree");
                                    }

                                    $scope.Open.Fn.Core.Close.Report();

                                    /*if ($scope.RecycleBin.Box.Current == 'CurrentDept') {
                                    $scope.Fn.TabSearch.Refresh.Server('RecycleBin');
                                    } else {
                                    $scope.RecycleBin.Box.CurrentDept.Inited = false;
                                    }*/

                                    Alert("删除成功");
                                } else {
                                    Alert(result);
                                    throw result;
                                }
                            }
                        }
                    },
                    Divide: function (arr, _this) {  //arr  a/b=c  0,1,2
                        var names = _this ? undefined : $scope.Fn.GetEvt().attr('ng-model').split(".");
                        _this = _this ? _this : [$scope.Fn.GetEvt().scope()[names[0].toLowerCase()], $scope.Open.Report.Current[names[0].toUpperCase()][0]];

                        $.each(_this, function() {
                            if (this[arr[0]] > 0 && this[arr[1]] > 0) {
                                this[arr[2]] = parseFloat(App.Tools.Calculator.Division(this[arr[0]], this[arr[1]]) * 100).toFixed(2);
                                this[arr[2]] = Number(this[arr[2]]) > 0 ? this[arr[2]] : undefined;
                            } else {
                                this[arr[2]] = undefined;
                            }
                        });
                    },
                    Check_Data: function() {
                        var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                        var cur_table = $scope.Fn.GetEvt().scope()[arr[0]];
                        var pre_table = $scope.Open.Report.Current.Attr.Previous[arr[0].toUpperCase()].Find('UnitCode', cur_table.UnitCode);

                        if (Number(pre_table[arr[1]]) > 0 && Number(cur_table[arr[1]]) < Number(pre_table[arr[1]])) {
                            $scope.Open.Report.Current.Attr.WrongData.push(cur_table.UnitCode + '-' + arr[1]);
                        } else {
                            if ($scope.Open.Report.Current.Attr.WrongData.In_Array(cur_table.UnitCode + '-' + arr[1])) {
                                angular.forEach($scope.Open.Report.Current.Attr.WrongData, function(name, i) {
                                    if (name == cur_table.UnitCode + '-' + arr[1]) {
                                        $scope.Open.Report.Current.Attr.WrongData.splice(i, 1);
                                    }
                                });
                            }
                        }
                    },
                    Close: {
                        Report: function (pageno) {
                            pageno = pageno == undefined ? $scope.Open.Report.Current.ReportTitle.PageNO : pageno;
                            if (Number(pageno) == 0) {
                                if (!confirm("报表尚未保存，确认关闭么？")) {
                                    return;
                                }
                            }

                            $.each($scope.Open.Report.Opened, function (i) {
                                if (this.ReportTitle.PageNO == pageno) {
                                    $scope.Open.Report.Opened.splice(i, 1);
                                    return false;
                                }
                            });

                            var length = $scope.Open.Report.Opened.length;
                            if (length > 0) {
                                $scope.Open.Report.Current = $scope.Open.Report.Opened[length - 1];
                            } else {
                                $scope.Open.Report.Current = undefined;
                                $scope.Open.Report.Screen.State = 'inhreit';
                            }
                        }
                    },
                    Sum: function (strOrObj, fixed) {
                        fixed = fixed != undefined ? fixed : 2;
                        var arr = undefined;
                        var count = undefined;
                        var data = undefined;
                        var fn = function () {
                            count = 0;
                            $.each(data, function(i) {
                                if (i > 0) {
                                    count = App.Tools.Calculator.Addition(count, this[arr[1]]);
                                }
                            });

                            count = parseFloat(count).toFixed(fixed);
                            count = Number(count) > 0 ? count : undefined;

                            data[0][arr[1]] = count;
                        };

                        if (typeof strOrObj == "undefined" || strOrObj == 0) {
                            arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                            data = $scope.Open.Report.Current[arr[0].toUpperCase()];
                            fn();
                        }else if (typeof strOrObj == "string") {
                            arr = [$scope.Open.Fn.Common.CurTable.Code(), strOrObj];
                            data = $scope.Open.Report.Current[arr[0].toUpperCase()];
                            fn();
                        } else if (typeof strOrObj == "object") {
                            data = $scope.Open.Report.Current[strOrObj.Table];
                            angular.forEach(strOrObj.Fields, function(field) {
                                arr = [strOrObj.Table, field];
                                fn();
                            });
                        }
                    },
                    Save: function () {
                        var report = angular.copy($scope.Open.Report.Current);

                        if (report.Attr.WrongData.length > 0) {
                            Alert('表中某些数据不能小于上期数据');
                            return;
                        }

                        if (report.ReportTitle.SourceType == 1) {  //汇总
                            var aggacc_pagenos = "";
                            $.each($scope.Open.Report.Current.Attr.AggAcc.Content, function() {
                                aggacc_pagenos += this.id + ",";
                            });
                            report.ReportTitle.SPageNOs = aggacc_pagenos.substr(0, aggacc_pagenos.length - 1);
                        }
                        delete report.Attr;

                        if (report.ReportTitle.PageNO > 0) {
                            $.each(report, function(key) {
                                if (key.indexOf("SH0") > 0, $.isArray(this)) {
                                    $.each(this, function() {
                                        delete this.TBNO;
                                        delete this.EntityKey;
                                        delete this.EntityState;
                                    });
                                }
                            });
                        }

                        $.ajax({
                            url: 'SH/SaveReport',
                            type: 'POST',
                            dataType: 'json',
                            data: {
                                report: JSON.stringify(report)
                            },
                            success: function (response) {
                                if (Number(response) > 0) {
                                    Alert("保存成功！");

                                    $scope.Open.Report.Current.ReportTitle.State = 0;
                                    $scope.Open.Report.Current.ReportTitle.PageNO = response;
                                    if ($scope.Open.Report.Tree.Current.Tab != "MyRptTree") {
                                        //$scope.Open.Report.Tree.Fn.Switch("ReceivedRptTree", true).Refresh.Report().NodeState(); //没有显示MyRptTree，则从后台获取数据
                                        $scope.Open.Report.Tree.Data.Load(false, "MyRptTree");
                                    } else {
                                        $scope.Open.Report.Tree.Fn.Switch("MyRptTree", true).Refresh.Report().NodeState();
                                    }

                                    $scope.New.SameReport.Search();

                                    $scope.$apply();
                                } else {
                                    Alert(response);
                                    throw response;
                                }
                            }
                        });
                    },
                    Send: function () {
                        if (confirm("确认报送？")) {
                            var result = $scope.Fn.Ajax("SH/SendReport", {
                                pageno: $scope.Open.Report.Current.ReportTitle.PageNO,
                                limit: $scope.BaseData.Unit.Local.Limit
                            });
                            if (Number(result) > 0) {
                                $scope.Open.Report.Current.ReportTitle.State = 3;

                                if ($scope.Open.Report.Tree.Current.Tab == "MyRptTree") {
                                    $scope.Open.Report.Tree.Fn.Switch("MyRptTree", true).Refresh.Report().NodeState();
                                } else {
                                    $scope.Open.Report.Tree.Data.Load(false, "MyRptTree");
                                }

                                Alert("报送成功！");
                            } else {
                                Alert(result);
                            }
                        }
                    },
                    Distribute: function (fixed) {
                        fixed = fixed ? fixed : 2;
                        var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                        var sh011 = $scope.Fn.GetEvt().scope()[arr[0].toLowerCase()];

                        if (Number(sh011.NDZJYS) > 0) {
                            if (Number(sh011[arr[1]]) > 0 && Number(sh011[arr[1]]) <= Number(sh011.NDZJYS)) {
                                sh011[arr[1]] = parseFloat(sh011[arr[1]]).toFixed(fixed);
                                if (arr[1] == 'NDZJZY') {
                                    sh011.NDZJDF = App.Tools.Calculator.Subtraction(sh011.NDZJYS, sh011[arr[1]]);
                                    sh011.NDZJDF = parseFloat(sh011.NDZJDF).toFixed(fixed);
                                    sh011.NDZJDF = Number(sh011.NDZJDF) > 0 ? sh011.NDZJDF : 0;
                                } else if (arr[1] == 'NDZJDF') {
                                    sh011.NDZJZY = App.Tools.Calculator.Subtraction(sh011.NDZJYS, sh011[arr[1]]);
                                    sh011.NDZJZY = parseFloat(sh011.NDZJZY).toFixed(fixed);
                                    sh011.NDZJZY = Number(sh011.NDZJZY) > 0 ? sh011.NDZJZY : 0;
                                }
                            } else {
                                sh011[arr[1]] = undefined;
                            }
                        } else {
                            sh011.NDZJZY = undefined;
                            sh011.NDZJDF = undefined;
                        }
                    },
                    Print: function () {
                        window.ReportTitle = $scope.Open.Report.Current.ReportTitle;
                        window.open('/SH/Print', '', '', '');
                    },
                    Open: function (obj) {
                        var exit = undefined;
                        var index = -1;
                        exit = $scope.Open.Report.Opened.some(function (rpt, i) {
                            if (rpt.ReportTitle.PageNO == obj.pageno) {
                                index = i;
                                return true;
                            }
                        });

                        if (exit) {
                            $scope.Open.Report.Current = $scope.Open.Report.Opened[index];
                        } else {
                            var report = $scope.Fn.Ajax("SH/OpenReport", {
                                rptType: obj.rptType,
                                sourceType: obj.sourceType,
                                pageno: obj.pageno
                            });

                            report.Attr = {
                                TableIndex: 0,
                                ReportFooter: "ReportTitle",
                                AggAcc: {
                                    Content: [],
                                    Selected: undefined
                                }
                            };

                            if (report.SourceReport) {
                                report.Attr.AggAcc.Content = angular.copy(report.SourceReport);
                                report.Attr.AggAcc.isAdded = true;
                            }

                            $scope.Open.Report.Opened.push(report);
                            $scope.Open.Report.Current = report;

                            if (obj.is_cur_year && $scope.BaseData.Unit.Local.Limit < 4 && report.ReportTitle.ORD_Code != 'SH05') {
                                var arr = [], _this = undefined;
                                $.each($scope.BaseData.Unit.SH[$scope.Open.Report.Current.ReportTitle.ORD_Code], function (key) {
                                    if (this.length) {
                                        arr = [];
                                        $.each(this, function() {
                                            _this = $scope.Open.Report.Current[key].Find('UnitCode', this.UnitCode);
                                            if ($.isEmptyObject(_this)) {
                                                arr.push($.extend(App.Models.SH[key].Object(), this));
                                            } else {
                                                delete _this.EntityKey;
                                                delete _this.EntityState;
                                                arr.push(_this);
                                            }
                                        });

                                        if (['SH01', 'SH03'].In_Array($scope.Open.Report.Current.ReportTitle.ORD_Code)) {
                                            _this = $scope.Open.Report.Current[key].Find("UnitCode", "HJ");
                                            if ($.isEmptyObject(_this)) {
                                                arr.InsertAt(0, $.extend(App.Models.SH[key].Object(), {
                                                    DataOrder: 0,
                                                    UnitCode: 'HJ',
                                                    SSX: '合计'
                                                }));
                                            } else {
                                                arr.InsertAt(0, _this);
                                            }
                                        }

                                        $scope.Open.Report.Current[key] = arr;
                                    }
                                });
                            }

                            if (report.ReportTitle.ORD_Code == "SH03" || report.ReportTitle.ORD_Code == "SH04") {
                                var count = report.ReportTitle.ORD_Code == "SH03" ? 8 : 5;
                                for (var j = 1; j <= count; j++) {
                                    if (!report[report.ReportTitle.ORD_Code + j].length) {
                                        delete report[report.ReportTitle.ORD_Code + j];
                                    }
                                }
                            }

                            if ($scope.Open.Report.Current.ReportTitle.ORD_Code == 'SH03') {
                                $scope.Open.Report.Current.Attr.TableIndex = 5;
                            }
                        }

                        if (obj.apply) {
                            $scope.$apply();
                        }
                    }
                },
                Common: {
                    Check: function () {
                        var arr = $scope.Fn.GetEvt().attr('ng-bind').split(".");
                        var obj = $scope.Fn.GetEvt().scope();
                        if (obj[arr[0]][arr[1]]) {
                            obj[arr[0]][arr[1]] = undefined;
                        } else {
                            obj[arr[0]][arr[1]] = "√";
                        }
                    },
                    Number: function (fixed) {
                        var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                        var obj = $scope.Fn.GetEvt().scope()[arr[0].toLowerCase()];

                        if (Number(obj[arr[1]]) > 0) {
                            fixed = fixed > 0 ? fixed : 0;
                            obj[arr[1]] = parseFloat(obj[arr[1]]).toFixed(fixed);
                        } else {
                            obj[arr[1]] = undefined;
                        }
                    },
                    isCurYear: function (str) {
                        var date = new Date(str);

                        return date.getFullYear() == new Date().getFullYear() ? 1 : 0;
                    },
                    CurTable: {
                        Code: function() {
                            return $scope.Open.Report.Current.ReportTitle.ORD_Code +
                                ($scope.Open.Report.Current.Attr.TableIndex + 1);
                        }
                    }
                }
            }
        };
        if ($scope.Menu.Current == "Open") {
            $scope.Open.Init();
        }

        $scope.FCS = $scope.Open.Fn.Core.Sum;
        $scope.FCK = $scope.Open.Fn.Common.Check;
        $scope.FCKD = $scope.Open.Fn.Core.Check_Data;
        $scope.FCN = $scope.Open.Fn.Common.Number;
        $scope.FCD = $scope.Open.Fn.Core.Distribute;
        $scope.FCDVD = $scope.Open.Fn.Core.Divide;

        $scope.Report = {
            Open: $scope.Open.Report
        };
        $scope.$watch("Open.Report.Current.Attr.TableIndex", function (to, from) {
            if (!isNaN(from) && to != undefined && $scope.Open.Report.Current) {
                App.Plugin.TableFixed.FixedPage[from].Destroy();
            }
        });

        /*$scope.$watch("Open.Report.Current.ReportTitle.SourceType", function (to, from) {
            $timeout(function() {
                var zTree = $.fn.zTree.getZTreeObj('#ReceivedRptTree');

                if (to == 2) {
                    $scope.Open.Report.Tree.Current.Tab = 'ReceivedRptTree';
                    zTree.setting.check.enable = true;
                } else {
                    $scope.Open.Report.Tree.Current.Tab = 'MyRptTree';
                    zTree.setting.check.enable = false;
                }

                zTree.refresh();
            });
        });*/
        //-------------------------------------<Receive State>---------------------------------------------
        $scope.Receive = {
            Init: function() {
                var model = function() {
                    return {
                        UnitCode: "",
                        UnitName: "",
                        ORD_Code: "",
                        StatisticalCycType: "",
                        StartDateTime: App.Tools.Date.GetOneDay(-30),
                        EndDateTime: App.Tools.Date.GetToday()
                    };
                };
                this.Attr.Auditing = new model();
                this.Attr.Audited = new model();
                this.Attr.Refused = new model();
                //this.Init = angular.noop;
            },
            Attr: {
                CycTypeSelect: [$scope.New.Report.CycType], //.concat(baseData.Select.CycType["HP01"] ? baseData.Select.CycType["HP01"] : [])
                RptClass: $scope.BaseData.Select.RptClass,
                CheckAll: false,
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
                    var currentBox = $scope.Receive.Box[$scope.Receive.Box.Current];
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
                            var result = $scope.Fn.Ajax("Index/ReportOperate" + (action == 'Refuse' ? ('?unitcodes=' + unitcodes.toString() + "&time=" + time.toString()) : ""), data);
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
                                        currentBox = $scope.Receive.Box[boxName];
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
                                            $scope.Open.Tree.Switch("ReceivedRptTree", true, rptType);
                                        });
                                    }

                                    $scope.Receive.Attr.CheckAll = false;
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
                                        msgType: 2 //0表示催报消息，1表示发送报表消息，2表示拒收消息
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
                BoxState: function() {
                    switch ($scope.Receive.Box.Current) {
                    case "Auditing":
                        return 0;
                    case "Audited":
                        return 2;
                    case "Refused":
                        return 1;
                    }
                }
            }
        };

        if ($scope.Menu.Current == 'Receive') {
            $scope.Receive.Init();
            $scope.Fn.TabSearch.Refresh.Server('Receive');
        }

        window.$scope = $scope;
    }
]);
