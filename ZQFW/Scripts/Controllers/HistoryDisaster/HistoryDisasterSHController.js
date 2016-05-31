App.controller('HeadCtrl', ['$rootScope', 'screen', 'loading', '$timeout', function ($rootScope, screen, loading, $timeout) {
    //-------------------------------------<Public State>---------------------------------------------
    $rootScope.NameSpace = "Head.Menu";
    $rootScope.InitData = window.init_data;
    $rootScope.BaseData = window.init_data.BaseData;
    $rootScope.SysUser = {
        Name: $rootScope.InitData.BaseData.Unit.Local.UnitName
    };
    $rootScope.SysUserCode = $rootScope.BaseData.Unit.Local.UnitCode.slice(0, 2);
    $rootScope.CurrentUrl = window.location.hash.replace("#", "");
    $rootScope.Tools = App.Tools;

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
    $rootScope.Attr = {
        SSO: parseInt($.cookie('sso')),
        NameSpace: "View",
        RootName: "HistoryDisaster"
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
        }
    $rootScope.Fn = {
        isNaN: isNaN,
        Ajax: function (url, param, type, async) {
            var result = null;
            async = async == undefined ? false : async;
            param = param ? param : undefined;
            type = type ? type : 'post';
            $.ajax({
                url: url,
                data: param,
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
                                var result = $scope.Ajax("Head/ModifyPwd", {
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

    window.$scope = $rootScope;
    //-------------------------------------</Public State>---------------------------------------------
    //-------------------------------------</View State>-----------------------------------------------
    $rootScope.View = {
        Init: function() {
            $scope.BaseData.Select = {
                RptClass: [],
                CycType: [$scope.InitData.New.Report.CycType]
            };

            /*$.each($scope.InitData.New.Report.TreeData.children, function() {
                $scope.BaseData.Select.RptClass.push({
                    Value: this.id,
                    Name: this.name
                });
            });*/

            $scope.BaseData.Select.RptClass = [    //有时间需要改写，直接从TB16_OperateReportDefine读取
                { "Value": "SH01", "Name": "资金情况统计表" },
                { "Value": "SH02", "Name": "山洪沟治理进度统计表" },
                { "Value": "SH03", "Name": "非工程措施补充完善表" },
                { "Value": "SH04", "Name": "调查评价表" }
            ];

            $scope.View.Report.Selected.StatisticalCycType = $scope.BaseData.Select.CycType[0].Code;

            $scope.View.Report.Tree.Data.Load();
        },
        Report: {
            Selected: {
                ORD_Code: "SH01",
                StatisticalCycType: undefined,
                UnitCode: $rootScope.BaseData.Unit.Local.UnitCode
            },
            Screen: {
                State: 'inhreit',
                Fn: screen
            },
            Opened: [],
            Current: undefined,
            Tree: {
                Data: {
                    Load: function(rptType, treeType) {
                        rptType = rptType ? rptType : $scope.View.Report.Selected.ORD_Code;
                        treeType = treeType ? treeType : $scope.View.Report.Tree.Current.Tab;

                        var params = {
                            unitCode: $scope.BaseData.Unit.Local.UnitCode,
                            unitLimit: $scope.BaseData.Unit.Local.Limit,
                            rptClass: rptType,
                            cycType: $scope.View.Report.Selected.StatisticalCycType,
                            limitType: treeType == "MyRptTree" ? 0 : 1,
                            minYear: -1
                        };

                        if (treeType == 'ReceivedRptTree' && $scope.View.Report.Selected.UnderUnitCode != $scope.BaseData.Unit.Local.UnitCode) {
                            params.unitCode = $scope.View.Report.Selected.UnderUnitCode;
                        }

                        $.ajax({
                            url: '/SH/GetTreeData',
                            data: params,
                            async: false,
                            success: function(response) {
                                $scope.View.Report.Tree.Data[rptType] = $scope.View.Report.Tree.Data[rptType] || {};
                                $scope.View.Report.Tree.Data[rptType][treeType] = JSON.parse(response);
                                $scope.View.Report.Tree.Current.SimpleData = $scope.View.Report.Tree.Data[rptType][treeType];
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
                    Switch: function(treeType, refresh, rptType) {
                        treeType = treeType ? treeType : $scope.View.Report.Tree.Current.Tab;
                        refresh = refresh ? refresh : false;
                        rptType = rptType ? rptType : $scope.View.Report.Selected.ORD_Code;

                        $scope.View.Report.Tree.Data[rptType] = $scope.View.Report.Tree.Data[rptType] || {};
                        if (!$scope.View.Report.Tree.Data[rptType][treeType] || refresh) {
                            $scope.View.Report.Tree.Data.Load(rptType, treeType);
                        }
                        $scope.View.Report.Tree.Current.SimpleData = $scope.View.Report.Tree.Data[rptType][treeType];
                        $scope.View.Report.Tree.Current.Tab = treeType;

                        return this;
                    },
                    Refresh: {
                        Report: function() {
                            var openReport = undefined;
                            if ($scope.View.Report.Tree.Current.Tab == "MyRptTree") {
                                openReport = function(event, treeId, treeNode) {
                                    if (treeNode.id) {
                                        var obj = {
                                            rptType: $scope.View.Report.Selected.ORD_Code,
                                            sourceType: treeNode.SourceType,
                                            pageno: treeNode.id,
                                            apply: true
                                        };
                                        $scope.View.Fn.Core.Open(obj);
                                    }
                                };
                            } else {
                                openReport = function(event, treeId, treeNode) {
                                    if (treeNode.id) {
                                        $scope.View.Fn.Core.ViewUnderRpt({
                                            ORD_Code: $scope.View.Report.Selected.ORD_Code,
                                            SourceType: treeNode.SourceType,
                                            Limit: parseInt($scope.BaseData.Unit.Local.Limit) + 1,
                                            UnitCode: treeNode.UnitCode,
                                            PageNO: treeNode.id
                                        });
                                    }
                                };
                            }

                            this.CheckBox($scope.InitTree($scope.View.Report.Tree.Current.Tab, $scope.View.Report.Tree.Current.SimpleData, {
                                check: { enable: true },
                                callback: {
                                    onClick: openReport,
                                    onExpand: function(event, treeId, treeNode) {
                                        if (treeNode.level == 1) {
                                            $scope.View.Report.Tree.Fn.FindNodeState(treeId, treeNode.name.replace("月", "")).Expand = true;
                                        }
                                    },
                                    onCollapse: function(event, treeId, treeNode) {
                                        if (treeNode.level == 1) {
                                            $scope.View.Report.Tree.Fn.FindNodeState(treeId, treeNode.name.replace("月", "")).Expand = false;
                                        }
                                    }
                                },
                                data: {
                                    simpleData: { enable: true },
                                    key: { title: "title" }
                                }
                            }));

                            if (angular.isFunction($scope.View.Report.Tree.Current.CallBack)) {
                                $scope.View.Report.Tree.Current.CallBack(); //尽量不要往CallBack中加参数
                                delete $scope.View.Report.Tree.Current.CallBack;
                            }

                            return this;
                        },
                        CheckBox: function(zTree) {
                            zTree = zTree ? zTree : $.fn.zTree.getZTreeObj($scope.View.Tree.Current.TreeId);
                            var enable = false;
                            if ($scope.View.Report.Current) {
                                switch ($scope.View.Report.Current.ReportTitle.SourceType) {
                                case "1":
                                    if ($scope.View.Report.Tree.Current.Tab == "MyRptTree") {
                                        enable = false;
                                    } else {
                                        enable = true;
                                    }
                                    break;
                                case "2":
                                    if ($scope.View.Report.Tree.Current.Tab == "MyRptTree") {
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
                            tree_id = tree_id ? tree_id : $scope.View.Report.Tree.Current.Tab;
                            $timeout(function() {
                                var zTreeObj = $.fn.zTree.getZTreeObj(tree_id);
                                var nodes = undefined;
                                $.each($scope.View.Report.Tree.Fn.FindNodeState(tree_id), function(month) {
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
                        if (!$scope.View.Report.Tree.State[$scope.View.Report.Selected.ORD_Code]) {
                            $scope.View.Report.Tree.State[$scope.View.Report.Selected.ORD_Code] = {
                                MyRptTree: new App.Models.TreeModel(),
                                ReceivedRptTree: new App.Models.TreeModel()
                            };
                        }

                        if (nodeName) {
                            return $scope.View.Report.Tree.State[$scope.View.Report.Selected.ORD_Code][$scope.View.Report.Tree.Current.Tab][nodeName];
                        } else {
                            return $scope.View.Report.Tree.State[$scope.View.Report.Selected.ORD_Code][$scope.View.Report.Tree.Current.Tab];
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
                            ORD_Code: $scope.View.Report.Selected.ORD_Code,
                            SourceType: _this.SourceType,
                            UnitCode: _this.UnitCode,
                            PageNO: _this.id,
                            queryUnderUnits: true
                        };

                        if ($scope.View.Report.Current.ReportTitle.SourceType == 1) { //汇总
                            obj.Limit = parseInt($scope.BaseData.Unit.Local.Limit) + 1;
                        }

                        $scope.View.Fn.Core.ViewUnderRpt(obj);
                    }
                },
                ViewUnderRpt: function (obj) {
                    obj.queryUnderUnits = obj.queryUnderUnits ? 1 : 0;
                    var url = "/SH/ViewUnderReport?rptType=" + obj.ORD_Code + "&sourceType=" + obj.SourceType + "&level=" + obj.Limit + "&unitcode="
                            + obj.UnitCode + "&pageno=" + obj.PageNO + "&queryUnderUnits=" + obj.queryUnderUnits;

                    if (obj.isRiverRpt) {
                        url += "&RiverRpt=true&RiverCode=" + obj.RiverCode;
                    }

                    if (window.location.href.toLowerCase().indexOf("debug=1") > 0) {
                        url += "&debug=1";
                    }

                    window.open(url, "", "_blank", "");
                },
                Print: function () {
                    window.ReportTitle = $scope.Open.Report.Current.ReportTitle;
                    window.open('/SH/Print', '', '', '');
                },
                Close: {
                    Report: function (pageno) {
                        pageno = pageno == undefined ? $scope.View.Report.Current.ReportTitle.PageNO : pageno;
                        if (Number(pageno) == 0) {
                            if (!confirm("报表尚未保存，确认关闭么？")) {
                                return;
                            }
                        }

                        $.each($scope.View.Report.Opened, function (i) {
                            if (this.ReportTitle.PageNO == pageno) {
                                $scope.View.Report.Opened.splice(i, 1);
                                return false;
                            }
                        });

                        var length = $scope.View.Report.Opened.length;
                        if (length > 0) {
                            $scope.View.Report.Current = $scope.View.Report.Opened[length - 1];
                        } else {
                            $scope.View.Report.Current = undefined;
                            $scope.View.Report.Screen.State = 'inhreit';
                        }

                        //$scope.View.Tree.Refresh.CheckBox();
                    }
                },
                Open: function (obj) {
                    var exit = undefined;
                    var index = -1;
                    exit = $scope.View.Report.Opened.some(function (rpt, i) {
                        if (rpt.ReportTitle.PageNO == obj.pageno) {
                            index = i;
                            return true;
                        }
                    });

                    if (exit) {
                        $scope.View.Report.Current = $scope.View.Report.Opened[index];
                    } else {
                        var report = $scope.Fn.Ajax("/SH/OpenReport", {
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
                            delete report.SourceReport;
                        }

                        $scope.View.Report.Opened.push(report);
                        $scope.View.Report.Current = report;

                        if (obj.apply) {
                            $scope.$apply();
                        }

                        /*if ($scope.BaseData.Unit.Local.Limit < 4) {
                            var arr = [], obj = undefined;
                            $.each($scope.BaseData.Unit.SH[$scope.View.Report.Current.ReportTitle.ORD_Code], function (key) {
                                if (this.length) {
                                    arr = [];
                                    $.each(this, function (i) {
                                        obj = $scope.View.Report.Current[key].Find('UnitCode', this.UnitCode);
                                        if ($.isEmptyObject(obj)) {
                                            arr.push($.extend(App.Models.SH[key].Object(), this));
                                        } else {
                                            arr.push(obj);
                                        }
                                        arr.Last().DataOrder = i + 1;
                                    });
                                    $scope.View.Report.Current[key] = arr;
                                }
                            });
                        }*/
                    }
                }
            }
        }
    };
    $rootScope.View.Init();

    $scope.$watch("View.Report.Current.Attr.TableIndex", function (to, from) {
        if (!isNaN(from) && to != undefined && $scope.View.Report.Current) {
            App.Plugin.TableFixed.FixedPage[from].Destroy();
        }
    });

    $rootScope.Open = $rootScope.Open || {}; //可能存在多个Report
    $rootScope.Open.Report = $rootScope.View.Report;
    $rootScope.Report = {
        View: $rootScope.View.Report
    };
    //-------------------------------------<View State>------------------------------------------------
} ]);

App.controller('MenuCtrl', [
    '$scope', function($scope) {

    }
]);

App.controller('HistoryDisasterViewCtrl', [
    '$scope', function($scope) {

    }
]);

App.directive('ngTree', function() {
    return function($scope) {
        $scope[$scope.Attr.NameSpace].Report.Tree.Fn.Refresh.NodeState();
        $scope[$scope.Attr.NameSpace].Report.Tree.Fn.Refresh.Report();
    };
});