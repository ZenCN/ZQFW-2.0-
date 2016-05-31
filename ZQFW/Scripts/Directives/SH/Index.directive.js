angular.module('index.directive', [])
    .directive('ngWdatepicker', [
        '$parse', function($parse) {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function($scope, $element, $attrs, ngModelCtrl) {
                    $(function() {
                        $element.click(function() {
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
                                    $element.scope().$apply(function() {
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
                                            $parse(setting.callback)($scope)(true);
                                        }
                                    });
                                }
                            });
                        });
                    });
                }
            };
        }
    ])
    .directive('ngRptTypeTree', function() {
        return function($scope, $element) {
            var zTree = $.fn.zTree.init($element, {
                data: {
                    simpleData: { enable: true }
                },
                view: {
                    selectedMulti: false
                },
                callback: {
                    onClick: function(event, treeId, treeNode) {
                        if (!treeNode.isParent && treeNode.id != $scope.New.Report.Type.Code) {
                            $scope.New.Report.Type.Code = treeNode.id;
                            $scope.New.Report.Type.Name = treeNode.name;
                            $scope.New.SameReport.Search();
                            $scope.$apply();
                        }
                    }
                }
            }, $scope.New.Report.TreeData);

            zTree.selectNode(zTree.getNodesByParam("id", $scope.New.Report.Type.Code, null)[0]);
        }
    })
    .directive('ngTree', function() {
        return function($scope) {
            $scope[$scope.Attr.NameSpace].Report.Tree.Fn.Refresh.NodeState();
            $scope[$scope.Attr.NameSpace].Report.Tree.Fn.Refresh.Report();

            //$scope[$scope.Attr.NameSpace].Report.Tree.Fn.Refresh.CheckBox();
        };
    })
    .directive('tableFixed', [
        '$timeout', function($timeout) {
            return function($scope, $element, $attr) {
                if (App.Plugin) {
                    $timeout(function() {
                        App.Plugin.TableFixed.Fix({
                            Index: $scope[$scope.Attr.NameSpace].Report.Current.Attr.TableIndex,
                            Element: $element,
                            Type: $attr["tableFixed"] == null ? "All" : $attr["tableFixed"]
                        });
                    }, 0, false);
                }
            };
        }
    ])
    .directive('ngFirstReadonly', function () {
        return function ($scope, $element, $attr) {
            if ($scope.$index == 0) {
                $element.find("input").attr("readonly", true);
                $element.find("td").addClass("ReadOnly");
            }
        };
    });
$(document).on("keydown", function (e) {

    if (event.keyCode == 8) {
        //屏蔽报表操作时按BackSpace后退
        var ev = e || window.event; //获取event对象     
        var obj = ev.target || ev.srcElement; //获取事件源     

        var t = obj.type || obj.getAttribute('type'); //获取事件源类型    

        //获取作为判断条件的事件类型  
        var vReadOnly = obj.getAttribute('readonly');
        var vEnabled = obj.getAttribute('enabled');
        //处理null值情况  
        vReadOnly = (vReadOnly == null) ? false : vReadOnly;
        vEnabled = (vEnabled == null) ? true : vEnabled;

        //当敲Backspace键时，事件源类型为密码或单行、多行文本的，  
        //并且readonly属性为true或enabled属性为false的，则退格键失效  
        var flag1 = (ev.keyCode == 8 && (t == "password" || t == "text" || t == "textarea")
        && (vReadOnly == true || vEnabled != true)) ? true : false;

        //当敲Backspace键时，事件源类型非密码或单行、多行文本的，则退格键失效  
        var flag2 = (ev.keyCode == 8 && t != "password" && t != "text" && t != "textarea")
        ? true : false;

        //判断  
        if (flag2) {
            return false;
        }
        if (flag1) {
            return false;
        }
    }

    if (event.keyCode >= 37 && event.keyCode <= 40 || event.keyCode == 13) {
        var targetInput = undefined;
        var thCount = $scope.Fn.GetEvt().parents("tr").find("th").length; //每行的固定列数
        switch (event.keyCode) {
            case 37: //←
                targetInput = $scope.Fn.GetEvt().parent().prev().find("input");
                break;
            case 38: //↑
                var inputIndex = $scope.Fn.GetEvt().parent().index() - thCount;
                targetInput = $scope.Fn.GetEvt().parent().parent().prev().find("td:eq(" + inputIndex + ") input");
                break;
            case 39: //→
                targetInput = $scope.Fn.GetEvt().parent().next().find("input");
                break;
            case 40: //↓
                var inputIndex2 = $scope.Fn.GetEvt().parent().index() - thCount;
                targetInput = $scope.Fn.GetEvt().parent().parent().next().find("td:eq(" + inputIndex2 + ") input");
                break;
            case 13: //回车与↓一样
                var inputIndex3 = $scope.Fn.GetEvt().parent().index() - thCount;
                targetInput = $scope.Fn.GetEvt().parent().parent().next().find("td:eq(" + inputIndex3 + ") input");
                break;
            default:
        }
        if (targetInput != undefined) {
            targetInput.focus();
        }
    }
});

/*$.getScript("../../Scripts/Library/jquery-migrate-1.2.1.min.js", function() {
    $(".Rpt-Content table input").live("paste", function() {
        var $pCell = $scope.Fn.GetEvt();
        var arr = undefined;
        try {
            arr = $pCell.attr("ng-model").split(".");
            var obj = event.clipboardData || window.clipboardData;
            var textStr = obj.getData("text");
            obj = $pCell.parent().scope()[$scope.Open.Report.Fn.Other.CurTable.Name().toLowerCase()]; //每个单位的的数据实体对象
            var colIndex = $pCell.parent().index();
            var rowIndex = $pCell.parents("tr").index();
            var unitArr = textStr.split("\n"); //换行符划分为分多行
            var fieldArr = undefined;
            var $tr = undefined;
            var $input = undefined;
            var isChanged = false;

            $.each(unitArr, function(i, val) { //循环行
                if (val.toString().length === 0) { //Excel空格覆盖原有值
                    return true;
                }

                fieldArr = unitArr[i].split("	"); //1个tab键划分为多列
                $tr = $pCell.parents("table").find("tbody tr:eq(" + (rowIndex + i) + ")");

                if ($tr.length === 0) {
                    return true; //continue
                }

                obj = $tr.scope()[arr[0]];

                $.each(fieldArr, function(m) { //循环列
                    $input = $tr.find("td").eq(colIndex + m - 1).find("input");

                    if ($input.length === 0 || $input.is("[readonly]")) {
                        return true; //continue
                    }

                    arr = $input.attr("ng-model").split(".");

                    if (fieldArr[m].trim() == "") {
                        fieldArr[m] = undefined;
                    }

                    obj[arr[1]] = fieldArr[m];
                    isChanged = true; //UnitEntity is Changed

                    if (i == unitArr.length - 1 || ($input.attr("ng-blur") && $input.attr("ng-blur").indexOf("FCRC") > 0)) { //最后一行算合计。每一个Input校核

                        $input.blur();
                    }
                });
            });
        } catch (e) {
            throw "粘贴出现错误：" + e;
        }

        return false;
    });
});*/