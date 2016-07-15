window.App = angular.module('ViewUnderReport', []);
App.service('baseData', function () {
    return window.basedata;
});

App.controller("MainCtrl", function ($scope, baseData, $http, $timeout) {
    window.$scope = $scope;
    $scope.NameSapce = "View";
    $scope.$parentScope = window.opener.$scope;
    $scope.Report = {};
    $scope.Report.Current = baseData.Report;
    $scope.SysUserCode = $scope.$parentScope.SysUserCode;
    $scope.BaseData = {};
    $scope.BaseData.Select = window.opener.$scope.BaseData.Select;
    $scope.BaseData.RiverCode = window.opener.$scope.BaseData.RiverCode;
    $scope.BaseData.isRiverRpt = location.href.indexOf("&RiverRpt=true") > 0 ? true : false;
    $scope.BaseData.RiverUnits = baseData.RiverUnits;
    $scope.BaseData.Unit = {
        Local: {
            UnitCode: "0",
            UnitName: "合计",
            Limit: undefined
        },
        Unders: baseData.Unders
    };

    if ($scope.BaseData.isRiverRpt || window.opener.$scope.CurrentUrl == "/RecycleBin" && window.opener.$scope.RecycleBin.Box.Current == "CurrentDept") {  //回收站查看本级报表
        $scope.BaseData.Unit.Local.Limit = window.opener.$scope.BaseData.Unit.Local.Limit;
    } else {
        $scope.BaseData.Unit.Local.Limit = parseInt(window.opener.$scope.BaseData.Unit.Local.Limit) + 1;
    }

    angular.forEach(window.location.search.split("&"), function (str) {
        if (str.toLowerCase().Contains("level")) {
            $scope.BaseData.Unit.Local.Limit = str.split("=")[1];
        }
    });

    $scope.BaseData.Reservoir = baseData.Reservoir;
    $scope.BaseData.Field = baseData.Field;

    $scope.Fn = {
        GetEvt: function() {
            return $(event.target || event.srcElement);
        },
        CN_Name: function(key, value) {
            var name = "";
            switch (key) {
            case "StatisticalCycType":
                var cycTypeArr = $scope.BaseData.Select.CycType;
                name = cycTypeArr.HL01.concat(cycTypeArr.HP01 ? cycTypeArr.HP01 : []).Find("value", value, "name");
                break;
            }
            return name;
        },
        Send: function () {
            if ($scope.Report.Current.Attr.Data_Changed) {
                Alert("报表的数据有变动，请先保存再报送");
                return false;
            }
            $scope.$parentScope.Open.Report.Fn.Core.Send(window, $scope.Report.View.Current.ReportTitle);
        },
        Delete: {
            Affix: function(index, tbno, url) {
                if (confirm("确认删除么？")) {
                    $scope.Report.Current.Attr.DelAffixTBNO.push(tbno);
                    $scope.Report.Current.Attr.DelAffixURL.push(url);
                    $scope.Report.Current.Affix.splice(index, 1);
                }
            },
            Report: function() {
                if (confirm("确认删除么？")) {
                    var result = $scope.$parentScope.Fn.Ajax("/Index/DeleteReport", {
                        type: "0",
                        pageno: $scope.Report.Current.ReportTitle.PageNO,
                        state: $scope.Report.Current.ReportTitle.State,
                        limit: $.cookie("limit")
                    });

                    if (result > 0) {
                        var river_code = window.location.search.slice(window.location.search.indexOf('&RiverCode') + 11);
                        $scope.$parentScope.$apply(function() {
                            $scope.$parentScope.Open.Report.Current.Attr.RiverData = $scope.$parentScope.Open.Report.Current.Attr.RiverData.RemoveBy('RiverCode', river_code);
                            $scope.$parentScope.Dialog.Attr.Content = $scope.$parentScope.Dialog.Attr.Content.RemoveBy('RiverCode', river_code);
                            if (!$scope.$parentScope.Dialog.Attr.Content.length) {
                                $scope.$parentScope.Dialog.Attr.Show = false;
                            }
                            window.close();
                        });
                        Alert("删除成功");
                    } else {
                        Alert(result);
                    }
                }
            }
        },
        Upload_Affix: function() {
            var uploadify = $('#file_upload');
            uploadify.uploadify("settings", "formData", {
                'PageNo': $scope.Report.Current.ReportTitle.PageNO,
                'limit': $scope.BaseData.Unit.Local.Limit,
                'unitcode': $scope.BaseData.Unit.Local.UnitCode,
                'rptType': $scope.Report.Current.ReportTitle.ORD_Code
            });
            uploadify.uploadify('upload', '*');
        },
        Save: function () {
            var report = {};
            report.DelAffixTBNO = $scope.Report.Current.Attr.DelAffixTBNO.toString();
            report.DelAffixURL = $scope.Report.Current.Attr.DelAffixURL.toString();
            report.ReportTitle = $scope.Report.Current.ReportTitle;
            report.HL011 = $scope.Report.Current.HL011;
            report.HL012 = $scope.Report.Current.HL012;
            report.HL013 = $scope.Report.Current.HL013;
            report.HL014 = $scope.Report.Current.HL014;

            var count = $scope.Report.Current.Attr.DelAffixTBNO.length;

            $.ajax({
                url: "/Index/SaveUpdateReport",
                data: { report: angular.toJson(report) },
                method: 'post',
                success: function (result) {
                    if (!isNaN(result)) {
                        Alert("保存完成", 1000);

                        if (count) {
                            $http.get('/Index/DelAffix?tbnos=' + $scope.Report.Current.Attr.DelAffixTBNO.toString() +
                                    '&urls=' + $scope.Report.Current.Attr.DelAffixURL.toString() + '&pageno=' + $scope.Report.Current.ReportTitle.PageNO)
                                .success(function(result) {
                                    if (Number(result) > 0) {
                                        if ($(".Boxes #file_upload-queue").children().length > 0) { //存在附件
                                            $scope.Fn.Upload_Affix();
                                        } else {
                                            Alert("保存完成", 1000);
                                        }
                                    } else {
                                        Alert(result);
                                        throw result;
                                    }
                                });
                        } else if ($(".Boxes #file_upload-queue").children().length > 0) {
                            $scope.Fn.Upload_Affix();
                        }
                    } else {
                        Alert(result);
                    }

                    delete $scope.Open.Report.Current.Attr.Data_Changed;
                }
            });
        },
        UnitIndex: function(tablename, unitcode) {
            var j = -1;
            $.each($scope.Report.Current[tablename.toUpperCase()], function(i) {
                if (this.UnitCode == unitcode) {
                    j = i;
                    return false;
                }
            });

            return j;
        }
    };
    var rptType = $scope.Report.Current.ReportTitle.ORD_Code;
    var arr = [];
    var tmpObj = {};
    var tmpArr = [];
    $scope.Report.Current.Attr = {
        TableIndex: 0,
        ReportFooter: "ReportTitle",
        SSB: false
    };
    switch (rptType) {
    case "HL01":
        arr = ["HL011", "HL014"];
        if (parseInt($scope.Report.Current.ReportTitle.StatisticalCycType) === 0) {
            $scope.Report.Current.Attr.SSB = true;
            $scope.Report.Current.Attr.TableIndex = 8;
        }
        $.each($scope.Report.Current.HL012, function() {
            if (typeof(this.BZ) == "string") {
                this.BZ = this.BZ.replaceAll("{/r-/n}", " ");
            }
        });
        break;
    case "HP01":
        arr = ["HP011"];
        $scope.Report.Current.HP012.Fake = {};
        angular.forEach(["Large", "Middle"], function(key) {
            if ($scope.Report.Current.HP012.Real[key].length > 0) {
                tmpArr.splice(0);
                tmpArr.push($scope.Report.Current.HP012.Real[key][0]); //合计行
                tmpObj = {};
                angular.forEach($scope.BaseData.Reservoir[key], function(obj) {
                    tmpObj = $scope.Report.Current.HP012.Real[key].Find("DXSKMC", obj.DXSKMC);
                    if ($.isEmptyObject(tmpObj)) {
                        tmpObj = window.opener.App.Models.HP.HP01.HP012.Object((key == "Large" ? "1" : "2"), obj, window.$scope);
                    }
                    tmpArr.push(tmpObj);
                });
                $scope.Report.Current.HP012.Real[key] = angular.copy(tmpArr);
            } else {
                $scope.Report.Current.HP012.Real[key] = window.opener.App.Models.HP.HP01.HP012.Real[key]($scope);
            }
        });
        $scope.Report.Current.HP012.Fake.Large = window.opener.App.Models.HP.HP01.HP012.Fake.Large($scope.Report.Current.HP012.Real.Large, $scope.BaseData.Unit.Unders.length);
        $scope.Report.Current.HP012.Fake.Middle = window.opener.App.Models.HP.HP01.HP012.Fake.Middle($scope.Report.Current.HP012.Real.Middle, $scope.BaseData.Unit.Unders.length);
        break;
    }

    if (window.location.href.indexOf('RiverRpt=true') < 0) {
        var exit = false;
        angular.forEach(arr, function(key) {
            tmpArr.splice(0);
            if ($scope.Report.Current[key].length == 0) {
                tmpArr = window.opener.App.Models[rptType.slice(0, 2)][rptType][key].Array($scope);
            } else {
                /*if (key == "HP011" && !$.isEmptyObject($scope.Report.Current[key].Find("DW", "全区/县"))) {
                    tmpArr = tmpArr.concat($scope.Report.Current[key]);
                } else {
                    tmpArr.push($scope.Report.Current[key][0]);
                    $scope.Report.Current[key].splice(0, 1);
                }*/
                tmpArr.push($scope.Report.Current[key][0]);
                $scope.Report.Current[key].splice(0, 1);
                angular.forEach($scope.BaseData.Unit.Unders, function(unit) {
                    exit = false;
                    exit = $scope.Report.Current[key].some(function(obj, i) {
                        if (unit.UnitCode == obj.UnitCode) {
                            tmpArr.push(obj);
                            $scope.Report.Current[key].splice(i, 1);
                            return true;
                        }
                    });
                    if (!exit) {
                        tmpArr.push(window.opener.App.Models[rptType.slice(0, 2)][rptType][key].Object(unit.UnitCode, unit.UnitName, $scope));
                    }
                });
            }
            $scope.Report.Current[key] = angular.copy(tmpArr);
        });
    }

    $scope.Attr = {
        NameSpace: "View"
    };

    $scope.Report = {
        View: $scope.Report,
        Current: $scope.Report.Current
    };

    $scope.Report.Current.Attr = $scope.Report.Current.Attr || {};
    $scope.Report.Current.Attr.DelAffixTBNO = [];
    $scope.Report.Current.Attr.DelAffixURL = [];

    if ($scope.BaseData.isRiverRpt) {
        angular.forEach(window.location.search.split("&"), function(str) {
            if (str.toLowerCase().Contains("rivercode")) {
                $scope.Report.Current.Attr.RiverName = $scope.BaseData.RiverCode[str.split("=")[1]] || "流域";
            }
        });
        
        $scope.BaseData.Equality = $scope.$parentScope.BaseData.RelationCheck.Formula.Equality;
        $.each([$scope.Report.Current.HL012, $scope.Report.Current.HL013], function () {
            $.each(this, function () {
                this.RiverSelect = $scope.$parentScope.Open.Report.Fn.Comm.ToRiverArr(this.RiverCode);
            });
        });
        $scope.BaseData.Unit.Unders = baseData.RiverUnits;

        $scope.$watch('Report.Current.Attr.TableIndex', function (to) {
            if (to == 4 || to == 5) {
                $timeout(function () {
                    $("select[ng-model='hl012.UnitCode'], select[ng-model='hl013.UnitCode']").attr('disabled', 'disabled');
                });
            }
        });

        angular.forEach(["HL011", "HL014"], function(key) {
            if ($scope.Report.Current[key].length == 0) {
                var obj = window.opener.App.Models.HL.HL01[key].Object($.cookie("unitcode").slice(0, 2) + "000000", "合计");
                $scope.Report.Current[key].push(obj);
            }
        });
        var _this = undefined;
        $.each($scope.BaseData.RiverUnits, function (i) { //添加缺省的流域单位
            _this = $scope.Report.Current.HL011.Find("UnitCode", this.UnitCode);
            if ($.isEmptyObject(_this)) {
                $scope.Report.Current.HL011.InsertAt(1, window.opener.App.Models.HL.HL01.HL011.Object(this.UnitCode, this.UnitName, 0 , {
                    DataOrder: i + 1
                }));
            } else {
                _this.DataOrder = i + 1;
            }
            _this = $scope.Report.Current.HL014.Find("UnitCode", this.UnitCode);
            if ($.isEmptyObject(_this)) {
                $scope.Report.Current.HL014.InsertAt(1, window.opener.App.Models.HL.HL01.HL014.Object(this.UnitCode, this.UnitName, 0 , {
                    DataOrder: i + 1
                }));
            } else {
                _this.DataOrder = i + 1;
            }
        });

        $scope.Report.Current.HL011.BubbleSort("DataOrder", "asc");
        $scope.Report.Current.HL014.BubbleSort("DataOrder", "asc");

        $scope.FCA = function(fixed, field_info) {
            fixed = fixed ? fixed : 0;
            var table_field = $.isArray(field_info) ? field_info : $scope.Fn.GetEvt().attr('ng-model').split(".");
            var arr = $scope.Report.Current[table_field[0].toUpperCase()];
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
                if (Number(total) == 0) {
                    total = undefined;
                }
            }
            $scope.Report.Current[table_field[0].toUpperCase()][0][table_field[1]] = total;
        };
        $scope.FCS = function (arr, path, fixed) {
            var $target = $(event.srcElement || event.target);
            var tableField = arr ? arr : $target.attr("ng-model").split(".");
            tableField[0] = tableField[0].toUpperCase();
            var sumValue = 0;
            var fixedVal = 0;
            var tmp = undefined;
            var data = $scope.Report.Current[tableField[0]];

            if (path) {
                tmp = data;
                for (var j = 0; j < path.length; j++) {
                    if (Object.hasOwnProperty.call(path, j)) {
                        tmp = tmp[path[j]];
                    }
                }
                data = tmp;
            }

            var go = false;
            for (var i = 1; i < data.length; i++) {
                go = true;
                switch ($scope.Report.Current.ReportTitle.ORD_Code) {
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

                if (go) {
                    if (Number(data[i][tableField[1]]) > 0) { //Number("")、Number(null)、Number(undefined)、Number("x") > 0 (返回false)
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
        };
        $scope.FCRC = function(leftField, rightVal, sumType, fixed, formulaType, report) {
            var obj = undefined;
            var right = undefined;
            var arr = undefined;
            var field = undefined;
            report = angular.isObject(report) ? report : $scope.Report.Current;
            if (!formulaType) {
                var leftTableName = undefined;
                var tmp = undefined;
                var $target = $(event.srcElement || event.target);
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

                $.each($scope.BaseData.Equality, function() {
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
                                    report[leftTableName][$scope.Fn.UnitIndex(tableName, scope[tableName].UnitCode)][obj.Left] = right;
                                    tableName = leftTableName;
                                } else {
                                    scope[tableName][obj.Left] = right;
                                }
                            }
                        }
                    }
                });

                if (field == "QXHJ" || field == "ZJXJ") {
                    $scope.FCS(["HL014", field]);
                } else if (field == "ZYRK") {
                    $scope.FCS(["HL011", "ZYRK"]);
                } else if (field == 'ZJJJZSS') {
                    $scope.FCS(["HL011", "ZJJJZSS"]);
                }
            }
        };
        $scope.N = App.Tools.Calculator.Number;
        $scope.FCQ = function () {
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
                        .replaceAll("&", "").replaceAll(" ", "");
                }
            } catch (e) {
                throw e;
            }
        };
        $scope.FCL = function(len) {
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
        };
        $scope.FCN = function(len, fixed) {
            var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
            var obj = $scope.Fn.GetEvt().scope()[arr[0].toLowerCase()];
            fixed = fixed ? fixed : 0;
            if (Number(obj[arr[1]]) > 0) {
                obj[arr[1]] = parseFloat(obj[arr[1]]).toFixed(fixed);
                if (len && obj[arr[1]].length > Number(len)) {
                    obj[arr[1]] = obj[arr[1]].slice(0, len);
                }
            } else {
                if ($scope.SysORD_Code == 'NP01' && obj[arr[1]] == 0) {
                    return;
                }
                obj[arr[1]] = undefined;
            }
        };
        $scope.FCE = function() {
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
                    obj[arr[arr.length - 1]] = obj[arr[arr.length - 1]].replaceAll('[\n|\r]', '');
                }
            } catch (e) {
                throw e;
            }
        };
        $scope.FCM = function(fixed, arr) {
            try {
                var max = 0;
                arr = arr ? arr : $scope.Fn.GetEvt().attr("ng-model").split(".");
                var field = arr[1];
                fixed = fixed ? fixed : 0;
                arr = $scope.Report.Current[arr[0].toUpperCase()];

                if (arr.length > 0) {
                    if ($scope.SysUserCode == "33" && $scope.BaseData.Unit.Local.Limit == 4 && ["1", "2"].In_Array($scope.Report.Current.ReportTitle.SourceType) && !$scope.Fn.GetEvt().is("[ng-blur=FCM(1)]")) {
                        $.each($scope.Report.Current.HL013[1].Max, function () {
                            if (this[field] > Number(max)) {
                                max = parseFloat(this[field]).toFixed(fixed);
                            }
                        });
                        $scope.Report.Current.HL013[1][field] = Number(max) > 0 ? max : undefined;
                    } else {
                        $.each(arr, function (i) {
                            if (Number(this[field]) > 0 && i > 0) {
                                this[field] = parseFloat(this[field]).toFixed(fixed);
                                this[field] = Number(this[field]) > 0 ? this[field] : undefined;
                                if (Number(this[field]) > Number(max)) {
                                    max = this[field];
                                }
                            }
                        });
                    }

                    arr[0][field] = Number(max) > 0 ? max : undefined;
                }
            } catch (e) {
                throw e;
            }
        };
        $scope.FCHS = function() {
            if ($scope.Report.Current.Attr.SSB) {
                var hl011 = $scope.Fn.GetEvt().scope().hl011;

                if ($scope.Report.Current.HL014.length == 0) {
                    $scope.Report.Current.HL014.push(window.opener.App.Models.HL.HL01.HL014.Object(hl011.UnitCode.slice(0, 2) + "000000", "合计"));
                }

                if ($scope.Fn.UnitIndex("Hl014", hl011.UnitCode) > 0) {
                    $scope.Report.Current.HL014[$scope.Fn.UnitIndex("Hl014", hl011.UnitCode)].XYZYQT = hl011.ZYRK;
                } else {
                    var obj = window.opener.App.Models.HL.HL01.HL014.Object(hl011.UnitCode, hl011.DW);
                    obj.XYZYQT = hl011.ZYRK;
                    $scope.Report.Current.HL014.push(obj);
                }
                
                /*$scope.Fn.GetEvt().data('NoCallUnitEntity', true);
                $scope.Fn.GetEvt().data('NoCheck', true);*/
                //$scope.Fn.GetEvt().data('disabledCallBack', ["UnitEntity", "RelationCheck"]);
                $scope.FCS(["HL014", "XYZYQT"]);
            }
        };

        $scope.FCOT = function (type, unitcode) { //死亡类型改变后
            if ($scope.BaseData.Unit.Local.UnitCode != unitcode) {
                var newField = type == "死亡" ? "SWRK" : "SZRKR";
                var oldField = type == "死亡" ? "SZRKR" : "SWRK";
                var report = $scope.Report;
                var index = $scope.Fn.UnitIndex('HL011', unitcode);
                var obj = undefined;
                $.each([0, index], function (key, value) {
                    obj = report.Current.HL011[value];
                    obj[newField] = Number(obj[newField]) > 0 ? Number(obj[newField]) + 1 : 1;
                    obj[oldField] = parseInt(obj[oldField]) - 1;
                    obj[oldField] = obj[oldField] == 0 ? undefined : obj[oldField];
                });
            }
        }

        $scope.FCM($scope.$parentScope.BaseData.Field.GCYMLS.DecimalCount, ["HL013", "GCYMLS"]);
        $scope.FCM($scope.$parentScope.BaseData.Field.ZYZJZDSS.DecimalCount, ["HL013", "ZYZJZDSS"]);
        $scope.FCM($scope.$parentScope.BaseData.Field.GCLJJYL.DecimalCount, ["HL013", "GCLJJYL"]);
        //$scope.Open.Report.Fn.Core.HL01.Avg(baseData.Field.YMFWBL.DecimalCount, ["HL013", "YMFWBL"]);

        /*$scope = $scope.$parentScope.Open.Report.Fn.Comm.OnChange.Type;
        $scope.FCOF = $scope.$parentScope.Open.Report.Fn.Comm.OnFocus.Field;
        $scope.CTF = $scope.$parentScope.Fn.ConvertToFloat;
        $scope.Divide = App.Tools.Calculator.Division;
        
        $scope.FCHSR = $scope.$parentScope.Open.Report.Fn.Core.HP01.Sum.Reservoir;
        
        $scope.FCHOX = $scope.$parentScope.Open.Report.Fn.Core.HP01.OnChange.XXSLZJ;*/

        if ($scope.$parentScope.SysUserCode == '33') {
            $scope.SW = function(unit) {
                var tmp = $scope.Report.Fn.UnitIndex;
                if ($scope.Fn.GetEvt()[0].checked) {
                    $scope.Report.Current.HL011[tmp("HL011", unit.UnitCode)].SYCS = 1;
                    $scope.Report.Current.HL011[0].SYCS = 1;

                    if (typeof $scope.Report.Current.HL013[1].CSMC == "string") {
                        $scope.Report.Current.HL013[1].CSMC += unit.UnitName + ";";
                    } else {
                        $scope.Report.Current.HL013[1].CSMC = unit.UnitName + ";";
                    }
                } else {
                    $scope.Report.Current.HL011[tmp("HL011", unit.UnitCode)].SYCS = undefined;
                    tmp = 0;
                    $.each($scope.Report.Current.HL011, function(i) {
                        if (i > 0) {
                            tmp = tmp + App.Tools.Calculator.Number(this.SYCS);
                        }
                    });
                    if (tmp > 0) {
                        $scope.Report.Current.HL011[0].SYCS = 1;
                    } else {
                        $scope.Report.Current.HL011[0].SYCS = undefined;
                    }

                    $scope.Report.Current.HL013[1].CSMC = $scope.Report.Current.HL013[1].CSMC.replace(unit.UnitName + ";", "");
                }
            };
        }
    }
});

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