App.directive('ngFirstreadonly', function () {

    return function ($scope, $element, $attr) {
        if ($scope.$index == 0) {
            $element.find("input").attr("readonly", true);
            $element.find("td").addClass("ReadOnly");
        }

        if ($scope.hl011 && $scope.hl011.DW == "全区/县") {
            $element.hide();
        } else if ($scope.hl014 && $scope.hl014.DW == "全区/县") {
            $element.hide();
        }
    };
});

App.directive('mutiselect', function () {

    return function($scope, $element) {
        angular.forEach($scope.BaseData.Unit.Unders, function(obj) {
            $element.append("<option value='" + obj.UnitCode + "'>" + obj.UnitName + "</option>");
        });

        $element.multiSelect({
            keepOrder: true,
            selectableOptgroup: true,
            selectableHeader: "<span style='font-size: 15px; font-weight: bolder;'>未选择</span><br/><input type='text' class='search-input' autocomplete='on' placeholder='快速搜索'>",
            selectionHeader: "<span style='font-size: 15px; font-weight: bolder;'>已选择</span><br/><input type='text' class='search-input' autocomplete='on' placeholder='快速搜索'>",
            afterInit: function(ms) {
                var that = this,
                $selectableSearch = that.$selectableUl.prev(),
                $selectionSearch = that.$selectionUl.prev(),
                selectableSearchString = '#' + that.$container.attr('id') + ' .ms-elem-selectable:not(.ms-selected)',
                selectionSearchString = '#' + that.$container.attr('id') + ' .ms-elem-selection.ms-selected';

                that.qs1 = $selectableSearch.quicksearch(selectableSearchString).on('keydown', function(e) {
                    if (e.which === 40) {
                        that.$selectableUl.focus();
                        return false;
                    }
                });

                that.qs2 = $selectionSearch.quicksearch(selectionSearchString).on('keydown', function(e) {
                    if (e.which == 40) {
                        that.$selectionUl.focus();
                        return false;
                    }
                });

                if ($scope.UrgeReport.Attr.Unit.length > 0) {
                    this.$element.multiSelect('select', $scope.UrgeReport.Attr.Unit);
                }
            },
            afterSelect: function(values) {
                $.each(values, function(i) {
                    if (!$scope.UrgeReport.Attr.Unit.In_Array(values[i])) {
                        $scope.UrgeReport.Attr.Unit.push(values[i]);
                    }
                });
                this.qs1.cache();
                this.qs2.cache();
            },
            afterDeselect: function(values) {
                $.each(values, function(i) {
                    $.each($scope.UrgeReport.Attr.Unit, function (j) {
                        if (values[i] == $scope.UrgeReport.Attr.Unit[j]) {
                            $scope.UrgeReport.Attr.Unit.splice(j, 1);
                            return false;
                        }
                    });
                });
                this.qs1.cache();
                this.qs2.cache();
            }
        });
    };
});

App.directive('autofileupload', function () {

    return function ($scope, $element) {
        $(function () {
            $element.click(function () {
                $("#ImportExcel").click();
            });

            $("#ImportExcel").change(function () {
                var val = $(this).val();
                if (val != "" && val.substr(val.lastIndexOf(".") + 1).toLowerCase() == "xls") {
                    $(this).parent().ajaxSubmit({
                        type: 'post',
                        resetForm: true,
                        url: "ReportOperate/ImportExcel?rptType=" + $scope.Open.Report.Current.ReportTitle.ORD_Code,
                        success: function (data) {
                            if (data.Message) {
                                alert(data.Message);
                            } else {
                                val = $scope.BaseData.Unit.Unders.Find("UnitName", data.data[0][1][0]);

                                if ($.isEmptyObject(val)) {
                                    Alert("只能导入本级单位的报表");
                                } else {
                                    var indexArr = undefined;
                                    var obj = undefined;
                                    var field = undefined;
                                    var map = undefined;
                                    var isSSB = false;  //$scope.Open.Report.Current.ReportTitle.StatisticalCycType == 0 ? true : false

                                    if (data.data.length == 2) { //实时报导实时报、实时报导过程报  (isSSB && data.data.length == 2) || (!isSSB && data.data.length == 2)
                                        map = App.Config.Field.Fn.GetMap('HL01.' + $scope.SysUserCode + '.SSB');
                                        isSSB = true;
                                    } else { //过程报导过程报、过程报导实时报
                                        map = App.Config.Field.Fn.GetMap('HL01.' + $scope.SysUserCode);
                                    }

                                    $scope.Open.Report.Current.HL012 = [];
                                    $scope.Open.Report.Current.HL013 = [];
                                    $.each($scope.Open.Report.Current, function (key) {
                                        if (isSSB) {
                                            if (key == "HL011") {
                                                indexArr = [0];
                                            } else if (key == "HL012") {
                                                indexArr = [1];
                                            } else {
                                                return true;
                                            }
                                        } else {
                                            indexArr = $scope.Open.Report.Fn.Other.CurTable.Name(key);
                                        }
                                        
                                        if (indexArr) {
                                            for (var i = indexArr[0]; i <= indexArr[indexArr.length - 1]; i++) { //循环Sheet
                                                $.each(data.data[i], function(j) { //循环每行

                                                    if (["HL012", "HL013"].In_Array(key)) {
                                                        obj = {};
                                                    }

                                                    $.each(this, function (k, val) { //循环每列
                                                        if (!isNaN(val)) {
                                                            val = Number(val);
                                                        }

                                                        if (["HL011", "HL014"].In_Array(key)) { //过滤DW
                                                            if (k > 0) {
                                                                $scope.Open.Report.Current[key][j][map[key][i + "-" + (k - 1)]] = val > 0 ? val : undefined;
                                                            }
                                                        } else {
                                                            field = map[key][i + "-" + k];
                                                            obj.Checked = false;

                                                            if (key == "HL013" && j == 0) {
                                                                if (field == "UnitCode") {
                                                                    obj.DW = "合计";
                                                                } else {
                                                                    obj[field] = val;
                                                                }
                                                            } else {
                                                                switch (field) {
                                                                case "UnitCode":
                                                                    obj.UnitCode = val.split("|")[1];
                                                                    obj.DW = val.split("|")[0];
                                                                    break;
                                                                case "RiverCode":
                                                                    obj.RiverCode = val.split("|")[1];
                                                                    obj.RiverSelect = [{ Code: obj.RiverCode, Name: val.split("|")[0] }];
                                                                    break;
                                                                case "DeathReason":
                                                                    obj.DeathReason = val.split("|")[0];
                                                                    obj.DeathReasonCode = val.split("|")[1]; //
                                                                    break;
                                                                default:
                                                                    obj[field] = val;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    });

                                                    if (["HL012", "HL013"].In_Array(key)) {
                                                        $scope.Open.Report.Current[key].push(obj);
                                                    }
                                                });
                                            }
                                        }
                                    });

                                    if ($scope.Open.Report.Current.HL013.length == 0) {
                                        $scope.Open.Report.Current.HL013.push($.extend(App.Models.HL.HL01.HL013(), { DW: '合计' }));
                                    }

                                    if ($scope.SysUserCode == '33' && $scope.BaseData.Unit.Local.Limit == 4) {  //浙江县级表6只有一行
                                        if ($scope.Open.Report.Current.HL013.length == 1) {
                                            $scope.Open.Report.Current.HL013.push(App.Models.HL.HL01.HL013());
                                        }
                                        $scope.Open.Report.Current.HL013[1].Max = [];
                                    }

                                    if (data.data.length == 2) {  //导入的是实时报
                                        $.each($scope.Open.Report.Current.HL011, function (i) {
                                            if (Number(this.ZYRK) > 0) {
                                                $scope.Open.Report.Current.HL014[i].XYZYQT = this.ZYRK;
                                            }
                                        });
                                    }

                                    $scope.Open.Report.Current.Attr.Data_Changed = true;

                                    $scope.$apply();
                                }
                            }
                        },
                        error: function (xhr) {
                            Alert("导入Excel出错了！");
                        }
                    });
                } else {
                    Alert("只能导入Excel文件");
                }
            });
        });
    };
});

App.directive('stretch', function() {
    return function($scope, $element, $attr) {
        $(function () {
            if ($element.is('div')) {
                $(document).click(function() {
                    if ($scope.Fn.GetEvt().is("." + $attr["class"]) || $.contains($element[0], $scope.Fn.GetEvt()[0])) { //点击的是自己或子级DOM
                        $element.addClass("Stretch-On");
                        $element.animate({ 'height': $attr['stretch'].split("-")[1] + 'px' }, 200);
                    } else {
                        $element.css({ 'height': $attr['stretch'].split("-")[0] + 'px' });
                        $element.removeClass("Stretch-On");
                    }
                });
            } else {
                $element.focus(function() {
                    $scope.Fn.GetEvt().addClass("Stretch-On");
                    $scope.Fn.GetEvt().animate({ 'height': $attr['stretch'].split("-")[1] + 'px' }, 200);
                });
                $element.blur(function() {
                    $scope.Fn.GetEvt().css({ 'height': $attr['stretch'].split("-")[0] + 'px' });
                    $scope.Fn.GetEvt().removeClass("Stretch-On");
                });
            }
        });
    };
});

App.directive("ngInitdeathtree", function () {
    return function ($scope, $element) {
        $(function () {
            if (!$scope.BaseData.DeathReason.zTree) {
                var dr = $scope.BaseData.DeathReason;
                dr.zTree = $(document.createElement("ul")).addClass("ztree").attr("id", "DeathTree");
                var sirz = function (pReasonCode, defualtOpen) {
                    var result = [];
                    for (var i in dr.Data) {
                        if (dr.Data[i].PReasonCode == pReasonCode) {
                            var node = {};
                            node["name"] = dr.Data[i].ReasonName;
                            node["ReasonCode"] = dr.Data[i].ReasonCode;
                            node["children"] = sirz(dr.Data[i].ReasonCode, defualtOpen);
                            if ("string" == typeof (defualtOpen) && dr.Data[i].ReasonCode.indexOf(defualtOpen) >= 0) {
                                node["open"] = true;
                            }

                            result.push(node);
                        }
                    }
                    return result;
                };
                dr.Data = sirz("", "JY");
                $element.append(dr.zTree).hide();
            } else {
                $element.append($scope.BaseData.DeathReason.zTree).hide();
            }

            $scope.InitTree($scope.BaseData.DeathReason.zTree, angular.copy($scope.BaseData.DeathReason.Data), {
                callback: {
                    onClick: function (event, treeId, treeNode) {
                        if (!treeNode.isParent) {
                            var getDeathreasonStr = function (str, node) {
                                var parent = node.getParentNode();
                                if (parent != null) {
                                    str = parent.name + "-" + str;
                                    str = getDeathreasonStr(str, parent);
                                }
                                return str;
                            };
                            var dr = $scope.BaseData.DeathReason;
                            dr.zTree.parent().hide();
                            $(document.body).unbind("mousedown", dr.Hide);
                            dr.Model.DeathReason = getDeathreasonStr(treeNode.name, treeNode);
                            dr.Model.DeathReasonCode = treeNode.ReasonCode;
                            $scope.$apply();
                        } else {
                            Alert("只能选择最底层的死亡原因");
                        }
                    }
                }
            });
        });
    };
});

App.directive('ngViewXsqk', function ($http) {
    return function($scope, $element) {
        $(function() {
            $element.click(function() {
                var codes = [],
                    obj = undefined,
                    code = undefined,
                    config = {
                        PageNO: $scope.Open.Report.Current.ReportTitle.PageNO,
                        Title: "全区蓄水情况",
                        Layout: "Histogram",
                        Left: '29%',
                        Top: "20%",
                        Data: {
                            UnitNames: [],
                            DQXSL: []
                        }
                    };
                config.Inited = $scope.Dialog.Attr ? $scope.Dialog.Attr.Inited : {
                    Element: false
                };
                config.Inited.Code = true;

                if ($scope.BaseData.Unit.Local.Limit == 3) {
                    obj = {};
                    $.each($scope.Open.Report.Current.NP011, function () {
                        if (this.RSCode) {
                            code = this.RSCode.substr(0, 6);
                            obj[code] = App.Tools.Calculator.Addition(obj[code], this.DQXSL);
                        }
                    });
                }

                $.each($scope.BaseData.Unit.Unders, function() {
                    if ($scope.BaseData.Unit.Local.Limit == 2) {
                        if (this.UnitCode != '15250000' && this.UnitCode != '15260000') {
                            config.Data.UnitNames.push(this.UnitName);
                            codes.push(this.UnitCode);
                        }
                    } else {
                        config.Data.UnitNames.push(this.UnitName);
                        config.Data.DQXSL.push(obj[this.UnitCode.substr(0, 6)] > 0 ? Number(obj[this.UnitCode.substr(0, 6)]) : null);
                    }
                });

                $scope.Dialog.Config(config);

                var fn = function() {
                    $("#highcharts").highcharts({
                        chart: {
                            type: 'column',
                            margin: [50, 0, 75, 24],
                            options3d: {
                                enabled: true,
                                alpha: 10,
                                beta: 25,
                                depth: 70
                            }
                        },
                        title: {
                            text: $scope.BaseData.Unit.Local.UnitName + App.Tools.Date.Format($scope.Open.Report.Current.ReportTitle.StartDateTime) +
                                "至" + App.Tools.Date.Format($scope.Open.Report.Current.ReportTitle.EndDateTime) + "蓄水情况"
                        },
                        plotOptions: {
                            column: {
                                depth: 25
                            }
                        },
                        xAxis: {
                            labels: {
                                rotation: 20,
                                align: 'left',
                                style: {
                                    fontSize: '12px',
                                    fontFamily: 'Verdana, sans-serif',
                                    fontWeight: 'bolder'
                                }
                            },
                            categories: $scope.Dialog.Attr.Data.UnitNames
                        },
                        yAxis: {
                            opposite: true
                        },
                        series: [
                            {
                                name: '蓄水量（单位：万立方米）',
                                data: $scope.Dialog.Attr.Data.DQXSL
                            }
                        ]
                    });
                };

                if ($scope.BaseData.Unit.Local.Limit == 2) {
                    $http.get('/sh/getxsqkdata?pageno=' + $scope.Open.Report.Current.ReportTitle.PageNO + "&unitcodes=" + codes.toString()).then(function(response) {
                        $scope.Dialog.Attr.Data.DQXSL = response.data;
                        fn();
                    });
                }

                $scope.$watchCollection('[Dialog.Attr.Inited.Element, Dialog.Attr.Inited.Code]', function(to) {
                    if (to[0] && to[1]) { //initing
                        fn();
                    }
                });

                $scope.$watch('Dialog.Attr.Show', function(to) {
                    if (to === false) {
                        if ($scope.Dialog.Attr.PageNO != $scope.Open.Report.Current.ReportTitle.PageNO) {
                            $scope.Dialog.Attr.Inited.Code = false;
                        }
                    }
                });

                $scope.$safeApply();
            });
        });
    };
});

App.directive('rptDelta', function ($timeout) {
    return function ($scope, $element) {
        if ($scope.Open.Report.Current.ReportTitle.SourceType > 0) {  //the source type not record
            $timeout(function () {
                $element.find('td input[type=text]').focus(function () {
                    $(this).data('prev_val', this.value);
                }).change(function () {
                    var $this = $(this);
                    var arr = $this.attr('ng-model').split('.');
                    var obj, prev_val, cur_val;
                    arr[0] = arr[0].toUpperCase();

                    if ($scope.Open.Report.Current.Delta) {
                        prev_val = $this.data('prev_val');
                        prev_val = isNaN(prev_val) ? 0 : Number(prev_val); //get the previous value

                        cur_val = $this.val();
                        cur_val = isNaN(cur_val) ? 0 : Number(cur_val); //get the current value

                        //差值表HL011、HL014没有合计行故index需减1
                        obj = $scope.Open.Report.Current.Delta[arr[0]][$this.parent().parent().index() - 1]; //get the previous delta model

                        if (!obj) {
                            obj = $this.parent().parent().scope()[arr[0].toLowerCase()];
                            obj = App.Models[arr[0].slice(0, 2)][arr[0].slice(0, 4)][arr[0]].Object(obj.UnitCode, obj.DW);
                        }

                        obj[arr[1]] = isNaN(obj[arr[1]]) ? 0 : obj[arr[1]];
                        obj[arr[1]] = cur_val - prev_val + obj[arr[1]];
                        obj[arr[1]] = obj[arr[1]] > 0 ? obj[arr[1]] : undefined;
                    }
                });
            });
        };
    }
});