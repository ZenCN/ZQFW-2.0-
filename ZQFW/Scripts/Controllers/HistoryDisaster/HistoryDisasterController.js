App.service('BaseData', function () {
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

App.controller('HeadCtrl', ['$rootScope', 'BaseData', 'screen', '$timeout', 'loading', function ($rootScope, baseData, screen, $timeout, loading) {
    //-------------------------------------<Public State>---------------------------------------------
    $rootScope.NameSpace = "Head.Menu";
    $rootScope.BaseData = baseData;
    $rootScope.SysUserCode = baseData.Unit.Local.UnitCode.slice(0, 2);
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
        },
        ViewUnderRpt: function (obj) {
            obj.queryUnderUnits = obj.queryUnderUnits ? 1 : 0;
            var url = "Index/ViewUnderReport?rptType=" + obj.ORD_Code + "&sourceType=" + obj.SourceType + "&level=" + obj.Limit + "&unitcode=" + obj.UnitCode + "&pageno=" + obj.PageNO + "&queryUnderUnits=" + obj.queryUnderUnits;

            if (obj.isRiverRpt) {
                url += "&RiverRpt=true";
            }

            window.open(url, "", "_blank", "");
        },
        InitTree: function (tree, simpleData, setting) {
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
    };

    window.$scope = $rootScope;
    //-------------------------------------</Public State>---------------------------------------------
    //-------------------------------------</View State>-----------------------------------------------
    $rootScope.View = {
        ReportTitle: {
            ORD_Code: "HL01",
            StatisticalCycType: "",
            UnitCode: baseData.Unit.Local.UnitCode
        },
        Attr: {
            UnderUnits: {
                HL01: {},
                HP01: {}
            },
            Hide: {
                Cased: (["15", "22", "23", "33", "35", "44", "45"].In_Array($rootScope.SysUserCode) && $rootScope.BaseData.Unit.Local.Limit == 4) || $rootScope.SysORD_Code == 'NP01'
            }
        },
        Report: {
            Screen: {
                State: 'inhreit',
                Fn: screen
            },
            Opened: [],
            Current: undefined,
            Attr: {
                Instruction: undefined
            },
            Fn: {
                Core: {
                    Close: function (pageno) {
                        pageno = pageno == undefined ? $rootScope.View.Report.Current.ReportTitle.PageNO : pageno;

                        $.each($rootScope.View.Report.Opened, function (i) {
                            if (this.ReportTitle.PageNO == pageno) {
                                $rootScope.View.Report.Opened.splice(i, 1);
                                return false;
                            }
                        });

                        var length = $rootScope.View.Report.Opened.length;
                        if (length > 0) {
                            $rootScope.View.Report.Current = $rootScope.View.Report.Opened[length - 1];
                        } else {
                            $scope.View.Report.Current = undefined;
                            $scope.View.Report.Screen.State = 'inhreit';
                        }
                    },
                    HL01: {
                        ViewRiverRpt: function() {
                            var riverData = undefined;

                            if ($scope.View.Report.Current.Attr.RiverData && $scope.View.Report.Current.Attr.RiverData.length > 0) {
                                riverData = $scope.View.Report.Current.Attr.RiverData;
                            } else {
                                riverData = $scope.Fn.Ajax("Index/GetRiverPageNOByPageNO", { pageNO: $scope.View.Report.Current.ReportTitle.PageNO }).Distribute;
                                $scope.View.Report.Current.Attr.RiverData = riverData;
                            }

                            if (riverData.length > 0) {
                                $scope.Dialog.Config({
                                    Layout: "DIV",
                                    Message: "",
                                    Title: "流域数据",
                                    Class: "div",
                                    Content: riverData,
                                    CallBack: function(pageno) {
                                        $scope.Fn.ViewUnderRpt({
                                            ORD_Code: $scope.View.Report.Current.ReportTitle.ORD_Code,
                                            SourceType: $scope.View.Report.Current.ReportTitle.SourceType,
                                            Limit: $scope.BaseData.Unit.Local.Limit,
                                            UnitCode: $scope.BaseData.Unit.Local.UnitCode,
                                            PageNO: pageno,
                                            queryUnderUnits: 1,
                                            isRiverRpt: true
                                        });
                                        //$scope.Dialog.Attr.Show = false;
                                    }
                                });
                            } else {
                                Alert("该报表没有流域数据");
                            }
                        }
                    },
                    NP01: {
                        Sort: function (np011) {
                            np011 = np011 ? $scope.View.Report.Current.NP011 : np011;
                            var index = undefined;
                            var count = undefined;
                            var arr = [];
                            switch ($scope.BaseData.Unit.Local.Limit) {
                                case 2:
                                    arr = arr.concat(App.Models.NP.NP01.NP011.Array(false, $scope.View.Report.Current.NP011));
                                    $scope.View.Report.Current.NP011 = arr;
                                    break;
                                case 3:
                                    index = 1;
                                    count = 0;
                                    var name = $scope.View.Report.Current.NP011[0].UnitName;
                                    $.each($scope.View.Report.Current.NP011, function (i) {
                                        if (name != this.UnitName) {
                                            arr.push($.extend(App.Models.NP.NP01.NP011.Object(), {
                                                No: App.Tools.Convert.ToChineseNumber(index),
                                                RSName: name,
                                                UnitName: count
                                            }));
                                            arr = arr.concat($scope.View.Report.Current.NP011.Slice(i - count, count));
                                            index++;
                                            count = 0;
                                            name = this.UnitName;
                                        }
                                        this.No = ++count;
                                    });
                                    $scope.View.Report.Current.NP011 = arr;
                                    break;
                                case 4:
                                    $.each($scope.View.Report.Current.NP011, function (i) {
                                        this.No = i + 1;
                                    });
                                    $scope.View.Report.Current.NP011.InsertAt(0, $.extend(App.Models.NP.NP01.NP011.Object(), {
                                        No: "一",
                                        RSName: $scope.BaseData.Unit.Local.UnitName,
                                        UnitName: $scope.View.Report.Current.NP011.length
                                    }));
                                    break;
                            }
                        }
                    },
                    Open: function (obj) {
                        loading.run();
                        obj.digest = obj.digest == undefined ? true : obj.digest;
                        var exit = undefined;
                        var index = -1;
                        exit = $rootScope.View.Report.Opened.some(function (rpt, i) {
                            if (rpt.ReportTitle.PageNO == obj.pageno) {
                                index = i;
                                return true;
                            }
                        });

                        if (exit) {
                            $scope.View.Report.Current = $scope.View.Report.Opened[index];
                        } else {
                            var report = $rootScope.Fn.Ajax("Index/OpenReport", {
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
                            }

                            delete report.SourceReport;
                            var arr = undefined;
                            var tmpObj = {};
                            var tmpArr = [];

                            angular.forEach(report.Attr.AggAcc.Content, function (obj) { //Start  剔除重复值时要去掉
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
                                    arr = ["HL012", "HL013"];
                                    for (var i in arr) {
                                        if (Object.hasOwnProperty.call(arr, i)) {
                                            $.each(report[arr[i]], function () {
                                                this.Checked = false;
                                                this.RiverSelect = $scope.View.Report.Fn.Comm.ToRiverArr(this.RiverCode);
                                            });
                                        }
                                    }
                                    if (report.HL013.length == 0) {
                                        report.HL013.push($.extend(App.Models.HL.HL01.HL013(), { DW: "合计" }));
                                    }
                                    arr = ["HL011", "HL014"];
                                    break;
                                case "HP01":
                                    arr = ["HP011"];
                                    report.HP012.Fake = {};
                                    angular.forEach(["Large", "Middle"], function (key) {
                                        if (report.HP012.Real[key].length > 0) {
                                            tmpArr.splice(0);
                                            tmpArr.push(report.HP012.Real[key][0]);
                                            tmpObj = {};
                                            angular.forEach($rootScope.BaseData.Reservoir[key], function (obj) {
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
                            angular.forEach(arr, function (key) {
                                tmpArr.splice(0);
                                if (report[key].length == 0) {
                                    tmpArr = App.Models[rptType.slice(0, 2)][rptType][key].Array();
                                } else {
                                    tmpArr.push(report[key][0]);
                                    report[key].splice(0, 1);
                                    angular.forEach($rootScope.BaseData.Unit.Unders, function (unit) {
                                        exit = false;
                                        exit = report[key].some(function (object, i) {
                                            if (unit.UnitCode == object.UnitCode) {
                                                tmpArr.push(object);
                                                report[key].splice(i, 1);
                                                return true;
                                            }
                                        });
                                        if (!exit) {
                                            tmpArr.push(App.Models[rptType.slice(0, 2)][rptType][key].Object(unit.UnitCode, unit.UnitName));
                                        }
                                    });
                                }
                                report[key] = angular.copy(tmpArr);
                            });

                            $scope.View.Report.Opened.push(report);
                            $scope.View.Report.Current = report;

                            switch (obj.rptType) {
                            case "HL01":
                                $scope.View.Report.Current.HL011 = $scope.View.Report.Current.HL011.RemoveBy("DW", "全区/县");
                                $scope.View.Report.Current.HL014 = $scope.View.Report.Current.HL014.RemoveBy("DW", "全区/县");
                                break;
                            case "NP01":
                                switch ($scope.BaseData.Unit.Local.Limit) {
                                case 2:
                                    $scope.View.Report.Fn.Core.NP01.Sort();
                                    break;
                                case 3:
                                    if ($scope.View.Report.Current.NP011.length > 0) { //市级目前只能根据“已填过的水库的数量”来判断下级是否已全部填过
                                        var length = 0;
                                        $.each($scope.BaseData.Reservoir, function() {
                                            length += this.Unders.length;
                                        });
                                        if ($scope.View.Report.Current.NP011.length < length) {
                                            $scope.View.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array(false, $scope.View.Report.Current.NP011);
                                        } else {
                                            $scope.View.Report.Fn.Core.NP01.Sort();
                                        }
                                    } else {
                                        $scope.View.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array();
                                    }
                                    break;
                                case 4:
                                    if ($scope.View.Report.Current.NP011.length > 0) {
                                        $scope.View.Report.Fn.Core.NP01.Sort();
                                    } else {
                                        $scope.View.Report.Current.NP011 = App.Models.NP.NP01.NP011.Array();
                                    }
                                    break;
                                }
                            }
                        }

                        if (obj.digest) {
                            $rootScope.$apply();
                        }
                    },
                    Print: function () {
                        if ($scope.View.Report.Current.ReportTitle.ORD_Code == 'HL01') {
                            window.ReportTitle = $scope.View.Report.Current.ReportTitle;
                            window.open('Index/Print', '', '', '');
                        } else {
                            Alert("暂不支持蓄水在线打印功能，请导出Excel之后，使用Excel打印");
                        }
                    }
                },
                Comm: {
                    ToRiverArr: function (riverStr) {
                        var rivers = [];
                        if (riverStr && riverStr != "") {
                            $.each(riverStr.split(","), function (key, val) {
                                rivers.push({ Name: $rootScope.BaseData.RiverCode[val], Code: val });
                            });
                        }

                        return rivers;
                    },
                    SelectNode: function (pageno) {
                        pageno = pageno == undefined ? $rootScope.View.Report.Current.ReportTitle.PageNO : pageno;
                        if (parseInt(pageno) != 0) {
                            var zTree = $.fn.zTree.getZTreeObj($rootScope.View.Tree.Current.TreeId);
                            var node = zTree.getNodeByParam("id", pageno, zTree.getNodeByParam("isFirstNode", true).getNextNode());
                            zTree.selectNode(node);
                        }
                    }
                }
            }
        },
        Tree: {
            Data: {
                HL01: {},
                HP01: {},
                NP01: {},
                Load: function (rptType, treeType) {
                    var params = {
                        unitCode: baseData.Unit.Local.UnitCode,
                        unitLimit: baseData.Unit.Local.Limit,
                        rptClass: rptType,
                        cycType: $scope.View.ReportTitle.StatisticalCycType,
                        limitType: treeType == "MyRptTree" ? 0 : 1,
                        isCurYear: 0
                    };

                    if (treeType == 'ReceivedRptTree' && $scope.View.ReportTitle.UnitCode != baseData.Unit.Local.UnitCode) {
                        params.unitCode = $scope.View.ReportTitle.UnitCode;
                    }

                    if (!this[rptType][treeType]) {
                        loading.run();
                    }

                    this[rptType][treeType] = $scope.Fn.Ajax('Index/GetTreeData', params, 'post');

                    if (!$scope.View.Tree.State[rptType][treeType]) {  //尚未初始化报表树的节点状态模型
                        var years = {}, index;
                        $.each(this[rptType][treeType], function () {
                            index = this.name.indexOf('年');
                            years[this.name.slice(0, index).trim()] = {
                                Expand: false,
                                Months: new App.Models.TreeModel()
                            };
                        });
                        $scope.View.Tree.State[rptType][treeType.replace('Tree', '')] = years;
                    }
                }
            },
            Current: {
                TreeId: undefined,
                SimpleData: undefined,
                GetType: function () {
                    var type = undefined;

                    if (this.TreeId) {
                        type = {};
                        if (this.TreeId.indexOf("HL01") >= 0) {
                            type.report = "HL01";
                        } else {
                            type.report = "HP01";
                        }
                        type.tree = this.TreeId.replace(type.report, "").replace("Tree", "");
                    }

                    return type;
                }
            },
            State: {
                HL01: {
                    MyRpt: undefined,
                    ReceivedRpt: undefined
                },
                HP01: {
                    MyRpt: undefined,
                    ReceivedRpt: undefined
                },
                NP01: {
                    MyRpt: undefined,
                    ReceivedRpt: undefined
                }
            },
            SwitchType: undefined,
            Switch: function (treeType, refresh, rptType) {
                treeType = treeType == undefined ? this.Current.TreeId.slice(4) : treeType;
                refresh = refresh == undefined ? false : refresh;
                rptType = rptType == undefined ? $scope.View.ReportTitle.ORD_Code : rptType;
                if (!this.Data[rptType][treeType] || refresh) {
                    this.Data.Load(rptType, treeType);
                }
                this.Current.TreeId = rptType + treeType;
                this.Current.SimpleData = this.Data[rptType][treeType];
                $scope.View.Selected.Box.Tree = treeType;
            },
            Refresh: {
                Report: function () {
                    var openReport = undefined;
                    if ($scope.View.Tree.Current.TreeId.indexOf("MyRptTree") >= 0) {
                        openReport = function (event, treeId, treeNode) {
                            if (treeNode.id) {
                                var obj = {
                                    rptType: $scope.View.ReportTitle.ORD_Code,
                                    sourceType: treeNode.SourceType,
                                    pageno: treeNode.id
                                };
                                $scope.View.Report.Fn.Core.Open(obj);
                            }
                        };
                    } else {
                        openReport = function (event, treeId, treeNode) {
                            if (treeNode.id) {
                                var queryUnderUnits = false;
                                if (!$scope.View.Attr.UnderUnits[$scope.View.ReportTitle.ORD_Code][treeNode.UnitCode]) {
                                    queryUnderUnits = true;
                                }    //如果已存在下级单位
                                $scope.Fn.ViewUnderRpt({
                                    ORD_Code: $scope.View.ReportTitle.ORD_Code,
                                    SourceType: treeNode.SourceType,
                                    Limit: parseInt($scope.BaseData.Unit.Local.Limit) + 1,
                                    UnitCode: treeNode.UnitCode,
                                    PageNO: treeNode.id,
                                    queryUnderUnits: queryUnderUnits
                                });
                            }
                        };
                    }

                    $scope.Fn.InitTree($scope.View.Tree.Current.TreeId, $scope.View.Tree.Current.SimpleData, {
                        check: { enable: false },
                        callback: {
                            onClick: openReport,
                            onExpand: function (event, treeId, treeNode) {
                                $scope.View.Tree.Set.NodeState(treeNode).Expand = true;
                            },
                            onCollapse: function (event, treeId, treeNode) {
                                $scope.View.Tree.Set.NodeState(treeNode).Expand = false;
                            }
                        }
                    });
                },
                NodeState: function () {
                    $timeout(function () {
                        var nodes, type, state, rootNodes, yearNode, zTreeObj = $.fn.zTree.getZTreeObj($scope.View.Tree.Current.TreeId);
                        type = $scope.View.Tree.Current.GetType();
                        state = $scope.View.Tree.State[type.report][type.tree];
                        rootNodes = zTreeObj.getNodesByFilter(function (node) {
                            return node.level == 0;
                        });

                        $.each(state, function (year) {
                            if (state[year].Expand) {
                                $.each(rootNodes, function (i) {
                                    if (this.name.indexOf(year + '年') >= 0) {
                                        yearNode = this;
                                        zTreeObj.expandNode(this, true); //年份保持之前的节点展开状态
                                        rootNodes.splice(i, 1);  //移除已处理过的rootNode
                                        return false;
                                    }
                                });

                                $.each(this.Months, function (month) {
                                    if (this.Expand) {
                                        nodes = zTreeObj.getNodesByParam("name", month + "月", yearNode);
                                        if (nodes.length > 0) {
                                            if (nodes[0].children && nodes[0].children.length > 0) {
                                                zTreeObj.expandNode(nodes[0], true);
                                            } else {
                                                this.Expand = false;
                                            }
                                        }
                                    }
                                });
                            }
                        });
                    });
                }
            },
            Set: {
                NodeState: function (node) {  //月份level = 1，年份level = 0
                    var result, year, parentNode, type = $scope.View.Tree.Current.GetType(), state = $scope.View.Tree.State[type.report][type.tree];

                    if (node.level) {
                        parentNode = node.getParentNode();
                        year = parentNode.name.slice(0, parentNode.name.indexOf('年')).trim();
                    } else {
                        year = node.name.slice(0, node.name.indexOf('年')).trim();
                    }


                    $.each(state, function (i) {
                        if (i == year) {
                            result = this;
                            if (node.level) {
                                $.each(this.Months, function (month) {
                                    if (node.name == month + '月') {
                                        result = this;
                                        return false;
                                    }
                                });
                            } else {
                                return false;
                            }
                        }
                    });

                    return result || {};
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

    $rootScope.$watch("View.ReportTitle.ORD_Code", function (to, from) {
        if (to != from && !$scope.View.Tree.SwitchType) {
            $scope.View.Tree.Switch($scope.View.Selected.Box.Tree);
            $scope.View.ReportTitle.StatisticalCycType = "";
        }

        delete $scope.View.Tree.SwitchType;
    });
    $rootScope.$watch("View.ReportTitle.StatisticalCycType", function (to, from) {
        if (to != from) {
            $scope.View.Tree.Switch($scope.View.Selected.Box.Tree, true);
            $scope.View.Tree.Refresh.Report();
        }
    });
    $rootScope.$watch("View.ReportTitle.UnitCode", function (to, from) {
        if (to != from) {
            $scope.View.Tree.Switch($scope.View.Selected.Box.Tree, true);
            $scope.View.Tree.Refresh.Report();
        }
    });
    $rootScope.$watch("View.Report.Current.ReportTitle.SourceType", function (to, from) {
        if ($scope.View.Report.Current) {
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

            if ($scope.View.Report.Current.ReportTitle.ORD_Code != $scope.View.ReportTitle.ORD_Code) {   //不同类型的报表切换
                $scope.View.Tree.SwitchType = "Double";
                $scope.View.Tree.Switch(treeType, false, $scope.View.Report.Current.ReportTitle.ORD_Code);
                $scope.View.ReportTitle.StatisticalCycType = "";
                $scope.View.ReportTitle.ORD_Code = $scope.View.Report.Current.ReportTitle.ORD_Code;
            }
        }
    });
    $rootScope.$watch("View.Report.Current.ReportTitle.ORD_Code", function (to, from) {
        if (to != from && to != undefined) {
            $scope.View.Report.Attr.Instruction = baseData.Select.CycType[to].Find('value', $scope.View.Report.Current.ReportTitle.StatisticalCycType);

            if (from) { //HL01和HP01之间的报表树切换
                var treeType = undefined;
                var sourceType = parseInt($scope.View.Report.Current.ReportTitle.SourceType);
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
                $scope.View.Tree.SwitchType = "Double";
                $scope.View.Tree.Switch(treeType, false, to);
                $scope.View.ReportTitle.StatisticalCycType = "";
                $scope.View.ReportTitle.ORD_Code = to;
            }
        }
    });
    $rootScope.$watch("View.Report.Current.ReportTitle.StatisticalCycType", function (to, from) {
        var report = $scope.View.Report;
        to = parseInt(to);

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
        }
    });
    $rootScope.$watch("View.Report.Current.Attr.TableIndex", function (to, from) {
        if (!isNaN(from) && to != undefined && $scope.View.Report.Current) {
            if ($scope.View.Report.Current.ReportTitle.ORD_Code == 'HP01' && to == 0) {
                $timeout(function () {
                    App.Plugin.TableFixed.Fix({
                        Index: $scope.Report[$scope.Attr.NameSpace].Current.Attr.TableIndex,
                        Element: $("table#HP011-1")
                    });
                }, 0, false);
            }

            if ($scope.View.Report.Current.ReportTitle.ORD_Code == 'HP01' && from < 2 || $scope.View.Report.Current.ReportTitle.ORD_Code == 'HL01') {
                App.Plugin.TableFixed.FixedPage[from].Destroy();
            }
        }
    });
    $rootScope.BaseData.Select.River = $rootScope.View.Report.Fn.Comm.ToRiverArr($rootScope.BaseData.Unit.Local.RiverCode);
    $rootScope.Report = $rootScope.Report || {}; //可能存在多个Report
    $rootScope.Report.View = $rootScope.View.Report;
    loading.stop();
    //-------------------------------------<View State>------------------------------------------------
}]);

App.controller('MenuCtrl', ['$scope', function ($scope) {
    
}]);

App.controller('HistoryDisasterViewCtrl', [
    '$scope', function($scope) {
        var type = undefined;

        if ($scope.View.Tree.Current.TreeId) {
            if ($scope.View.Report.Current) {
                switch (parseInt($scope.View.Report.Current.ReportTitle.SourceType)) {
                case 1:
                    type = "ReceivedRptTree";
                    break;
                default:
                    type = "MyRptTree";
                    break;
                }

                if ($scope.View.Tree.Current.TreeId != ($scope.View.Report.Current.ReportTitle.ORD_Code + type)) {
                    if ($scope.View.ReportTitle.ORD_Code == $scope.View.Report.Current.ReportTitle.ORD_Code) { //同类型的报表树切换
                        $scope.View.Tree.Switch(type);
                    } else { //不同类型的报表树切换
                        $scope.View.Selected.Box.Tree = type;
                        $scope.View.ReportTitle.ORD_Code = $scope.View.Report.Current.ReportTitle.ORD_Code;
                    }
                }
            }
        } else { //第一次默认初始化洪涝HL01MyRptTree
            type = "HL01";
            if ($scope.View.Report.Current) {
                type = $scope.View.Report.Current.ReportTitle.ORD_Code;
                $scope.View.ReportTitle.ORD_Code = type;
            }
            $scope.View.Tree.Data.Load(type, "MyRptTree");
            $scope.View.Tree.Current.TreeId = type + "MyRptTree";
            $scope.View.Tree.Current.SimpleData = $scope.View.Tree.Data[type].MyRptTree;
        }

    }
]);

switch ($.cookie("unitcode").slice(0, 2)) {
    case "43":
        $.getScript("../../Scripts/Models/HPModel.js");
        break;
    case "15":
        $.getScript("../../Scripts/Models/NPModel.js");
        break;
}