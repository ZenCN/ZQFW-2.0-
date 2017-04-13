App.controller('HeadCtrl', [
    '$rootScope', '$http', '$timeout', function($rootScope, $http, $timeout) {
        $rootScope.SysUserCode = $.cookie('unitcode').slice(0, 2);
        $rootScope.BaseData = {
            Unit: {
                Local: {
                    Limit: parseInt($.cookie("limit")), //浙江市级、县级也有基础数据
                    UnitName: $.cookie("unitname"),
                    RealName: $.cookie("realname")
                }
            }
        };

        if ($rootScope.SysUserCode == "43") {
            $rootScope.BaseData.Reservoir = {
                Select: {
                    Year: {
                        Array: [],
                        Value: undefined
                    },
                    Time: {
                        Array: ["———————"],
                        Value: "———————"
                    }
                },
                Source: {},
                Search: function() {
                    if ($scope.BaseData.Reservoir.Select.Time.Value != "———————") {
                        $http.get('BaseData/GetHpDataResult?time=' + $scope.BaseData.Reservoir.Select.Year.Value + "年" + $scope.BaseData.Reservoir.Select.Time.Value)
                            .success(function (data) {
                            $scope.BaseData.Reservoir.Source = data;
                        });
                    }
                },
                Save: function() {
                    if ($scope.BaseData.Reservoir.Select.Time.Value != "———————") {
                        $http.post('BaseData/UpdateHpData?json=' + angular.toJson(angular.copy($scope.BaseData.Reservoir.Source).RemoveAttr(["UnitName"])))
                            .success(function(result) {
                            if (Number(result) > 0) {
                                Alert("保存成功！");
                            } else {
                                Alert("保存失败！");
                                throw result;
                            }
                        });
                    }
                },
                Sum: function (_this) {
                    var sum = parseFloat(_this.ZJLNXSL);

                    if (sum > 0) {
                        _this.ZJLNXSL = sum.toFixed(2);
                    } else {
                        _this.ZJLNXSL = undefined;
                    }

                    sum = 0;
                    $.each($scope.BaseData.Reservoir.Source, function (i) {
                        if (i > 0) {
                            sum = App.Tools.Calculator.Addition(sum, this.ZJLNXSL);
                        }
                    });
                    $scope.BaseData.Reservoir.Source[0].ZJLNXSL = sum;
                }
            };

            var select = $rootScope.BaseData.Reservoir.Select;

            $http.get('BaseData/GetHpDateResult').success(function (data) {
                angular.forEach(JSON.parse(data), function (time) {
                    select.Time.Array.push(time.split("-")[1]);
                });
            });

            for (var i = 2014; i < new Date().getFullYear(); i++) {
                select.Year.Array.push(i);
            }
            select.Year.Value = select.Year.Array.Last();
        }

        $rootScope.Attr = {
            SSO: parseInt($.cookie('sso')),
            RootName: "BaseData"
        };
        $rootScope.Fn = {
            GetEvt: function() {
                return $(event.target || event.srcElement);
            }
        };

        $rootScope.Unit = {
            BaseData: angular.copy(App.Data.Base),
            Unders: {
                Array: [],
                CheckAll: false
            },
            Tabs: {
                Current: 'BaseData',
                TurnTo: function(name) {
                    this.Current = name;
                    if ($scope.Unit.SelectedCode) {
                        if (name == 'InferiorManager' && !$scope.Unit.Unders.Array.length) {
                            $scope.Unit.Fn.Read.Unders();
                        } else if (name == 'BaseData' && !Object.hasOwnProperty.call($scope.Unit.BaseData[0], "TBNO")) {
                            $scope.Unit.Fn.Read.BaseData();
                        }
                    }
                }
            },
            SelectedCode: undefined,
            Fn: {
                Other: {
                    Current: {
                        Scope: function() {
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

                            return {
                                value: obj[arr[arr.length - 1]],
                                name: arr[arr.length - 1],
                                self: obj
                            };
                        }
                    }
                },
                Check: {
                    Number: function(len) {
                        var model = $scope.Unit.Fn.Other.Current.Scope();
                        if (Number(model.value) > 0) {
                            model.self[model.name] = parseFloat(model.value).toFixed(len ? len : 0);
                        } else {
                            model.self[model.name] = undefined;
                        }
                    },
                    Unique: function(key, val) {
                        var count = 0;
                        $.each($scope.Unit.Unders.Array, function() {
                            if (this[key] == val) {
                                count++;
                            }
                        });
                        if (count > 1) {
                            $scope.Fn.GetEvt().parent().addClass("Warning");
                            //Alert("不能与其它单位相同");
                        } else {
                            $scope.Fn.GetEvt().parent().removeClass("Warning");
                        }
                    },
                    String: function() {
                        var arr = $scope.Fn.GetEvt().attr('ng-model').split(".");
                        var obj = $scope.Fn.GetEvt().scope()[arr[0]];
                        var rel = new RegExp("^([\u4E00-\uFA29]|[\uE7C7-\uE7F3]|[a-zA-Z0-9])*$");
                        if (typeof obj[arr[1]] == "string" && !rel.test(obj[arr[1]])) {
                            $scope.Fn.GetEvt().parent().addClass("Warning");
                        } else {
                            $scope.Fn.GetEvt().parent().removeClass("Warning");
                        }
                    }
                },
                Save: function() {
                    if ($scope.Unit.Tabs.Current == 'BaseData') {
                        $http({
                            method: 'post',
                            url: '/BaseData/SaveBaseData',
                            params: {
                                unitCode: $scope.Unit.SelectedCode,
                                jsonStr: angular.toJson(angular.copy($scope.Unit.BaseData).RemoveAttr(['Name', 'Decimal']))
                            }
                        }).success(function(result) {
                            if (Number(result) > 0) {
                                Alert("保存成功");
                            } else {
                                Alert(result);
                            }
                        });
                    } else if ($scope.Unit.Tabs.Current == 'InferiorManager') {
                        var $tr = $(".Table tbody tr"), update_arr = [], add_arr = [], scope;
                        var exit = false;
                        $tr.each(function(i) {
                            scope = $(this).scope();
                            if (scope.tr.$dirty) {
                                if ($(this).find("td.Warning").length) { //信息尚未完善
                                    exit = true;
                                } else if (scope._this.Name.trim().length == 0) {
                                    exit = $(this).find("input[ng-model='_this.Name']").parent();
                                    if (!exit.hasClass("Warning")) {
                                        exit.addClass("Warning");
                                    }
                                    exit = true;
                                } else if (scope._this.Code.trim().length == 0) {
                                    exit = $(this).find("input[ng-model='_this.Code']").parent();
                                    if (!exit.hasClass("Warning")) {
                                        exit.addClass("Warning");
                                    }
                                    exit = true;
                                }

                                if (exit) {
                                    return false;
                                }

                                if (Number(scope._this.ID) > 0) { //New
                                    scope = angular.copy(scope._this);
                                    delete scope.ID;
                                    add_arr.push(scope);
                                } else {
                                    update_arr.push(scope._this);
                                }
                            }
                        });

                        if (!exit) {
                            if (update_arr.length == 0 && add_arr.length == 0) {
                                Alert("没有要保存的信息");
                            } else {
                                $http.post('BaseData/Update_Add_Units?update_units=' + angular.toJson(update_arr) + '&add_units=' + angular.toJson(add_arr)).success(function(result) {
                                    if (result == 'success') {
                                        $tr.each(function() {
                                            scope = $(this).scope();
                                            if (scope.tr.$dirty) {
                                                delete scope._this.ID;
                                                scope.tr.$setPristine();
                                            }
                                        });
                                        //$scope.Unit.Unders.Array.BubbleSort("Order", "asc");
                                        Alert("保存成功");
                                    } else {
                                        Alert(result);
                                        throw result;
                                    }
                                });
                            }
                        } else {
                            Alert("信息尚未完善");
                        }
                    } else {
                        $scope.BaseData.Reservoir.Save();
                    }
                },
                New: function() {
                    $scope.Unit.Unders.Array.push({
                        Name: '',
                        Code: '',
                        ParentCode: $scope.Unit.SelectedCode,
                        RiverCode: $scope.Unit.Unders.Array[0].RiverCode, //新增行的一级流域值是第一行的一级流域值
                        Order: parseInt($scope.Unit.Unders.Array.Last().Order) + 1,
                        ID: $scope.Unit.Unders.Array.length
                    });
                    $timeout(function() {
                        $(".Table tbody tr:last").scope().tr.$setDirty();
                    });
                },
                Delete: function(_this) {
                    if (Number(_this.ID) >= 0) {
                        $scope.Unit.Unders.Array = $scope.Unit.Unders.Array.RemoveBy('ID', _this.ID);
                        Alert("删除成功");
                    } else {
                        if (confirm("确定要删除？")) {
                            $http.post("BaseData/DeleteUnit?unitCode=" + _this.Code).success(function(result) {
                                if (Number(result) > 0) {
                                    $scope.Unit.Unders.Array = $scope.Unit.Unders.Array.RemoveBy('Code', _this.Code);
                                    Alert("删除成功");
                                } else {
                                    Alert(result);
                                }
                            });
                        }
                    }
                },
                Reset: function(type, code) {
                    if (confirm("确定要重置密码？")) {
                        var param = {};
                        if (type == 'all') {
                            param.unitCode = $scope.Unit.Unders.Array.GetAll("Code");
                            param.limit = App.Tools.Unit.Limit($scope.Unit.SelectedCode) + 1;
                        } else {
                            param.unitCode = code;
                            param.limit = App.Tools.Unit.Limit(code);
                        }
                        $http({ url: 'BaseData/ResetPassword', params: param }).success(function(result) {
                            if (Number(result) > 0) {
                                Alert("重置成功");
                            } else {
                                Alert(result);
                            }
                        });
                    }
                },
                Read: {
                    Unders: function() {
                        $http.post("BaseData/GetUnits?unitCode=" + $scope.Unit.SelectedCode).success(function(json) {
                            $scope.Unit.Unders.Array = json.lowerunits;
                            if (!App.Plugin.TableFixed.FixedPage[4]) {
                                $timeout(function() {
                                    App.Plugin.TableFixed.Fix({
                                        Index: 4,
                                        Element: $(".InferiorManager table"),
                                        Top: 30
                                    });
                                });
                            }
                        });
                    },
                    BaseData: function() {
                        var arr = [], obj;
                        $http.post("/BaseData/GetBaseData?unitCode=" + $scope.Unit.SelectedCode).success(function(json) {
                            $.each(angular.copy(App.Data.Base), function() {
                                obj = json.DataList.Find("FieldDefine_NO", this.FieldDefine_NO);
                                this.FieldDefine_NO = parseInt(this.FieldDefine_NO);
                                arr.push($.extend(obj, this));
                            });
                            $scope.Unit.BaseData = arr;
                        });
                    }
                }
            }
        };
        window.$scope = $rootScope;
    }
]).run([
    '$http', function($http) {
        $http.defaults.responseType = 'json';
        $http.defaults.method = 'get';
        $http.defaults.cache = true;
    }
]);

App.controller('MainCtrl', [
    '$scope', function($scope) {
        $.fn.zTree.init($(".ztree"), {
            async: {
                enable: true,
                url: "/BaseData/GetTreeUnits",
                type: "post",
                dataFilter: function(treeId, parentNode, childrenNode) {
                    childrenNode = eval("(" + childrenNode + ")");
                    return childrenNode;
                }
            },
            view: {
                selectedMulti: false
            },
            callback: {
                onClick: function(event, treeId, treeNode) {
                    $scope.Unit.SelectedCode = treeNode.unitCode;
                    if ($scope.Unit.Tabs.Current == 'BaseData') {
                        $scope.Unit.Fn.Read.BaseData();
                    } else {
                        $scope.Unit.Fn.Read.Unders();
                    }
                }
            }
        }, null);

        $scope.FCN = $scope.Unit.Fn.Check.Number;
        $scope.FCU = $scope.Unit.Fn.Check.Unique;
        $scope.FCS = $scope.Unit.Fn.Check.String;
    }
]);

App.directive("posTop", function() {
    return function($scope, $element, $attr) {
        if ($scope.Attr.SSO > 0) {
            $element.css("top", $attr["posTop"] + "px");
        }
    };
});

App.Data = App.Data || {};
App.Data.Base = [
    { FieldDefine_NO: 23, Name: "区县个数（个）", Decimal: 0 },
    { FieldDefine_NO: 24, Name: "乡镇个数（个）", Decimal: 0 },
    { FieldDefine_NO: 25, Name: "总人口（万人）", Decimal: 4 },
    { FieldDefine_NO: 32, Name: "耕地面积（千公顷）", Decimal: 4 },
    { FieldDefine_NO: 44, Name: "工矿企业（个）", Decimal: 0 },
    { FieldDefine_NO: 53, Name: "中型水库（座）", Decimal: 0 },
    { FieldDefine_NO: 54, Name: "小I型水库（座）", Decimal: 0 },
    { FieldDefine_NO: 55, Name: "小II型水库（座）", Decimal: 0 },
    { FieldDefine_NO: 62, Name: "塘坝（座）", Decimal: 0 },
    { FieldDefine_NO: 57, Name: "堤防总计（千米）", Decimal: 3 },
    { FieldDefine_NO: 61, Name: "涵闸（处）", Decimal: 0 },
    { FieldDefine_NO: 65, Name: "机电井（眼）", Decimal: 0 },
    { FieldDefine_NO: 66, Name: "机电泵站（座）", Decimal: 0 },
    { FieldDefine_NO: 63, Name: "灌区设施（处）", Decimal: 0 }
];