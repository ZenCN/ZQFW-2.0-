//设置光标走向
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

if ($.cookie('ord_code') == 'HL01') {
    $.getScript("../../Scripts/Library/jquery-migrate-1.2.1.min.js", function() {
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
                var isUnitEntity = undefined;
                var isChanged = false;
                var index = undefined;

                if (Number($scope.BaseData.Unit.Local.Limit) > 2 && ["HL011", "HL014"].In_Array($scope.Open.Report.Fn.Other.CurTable.Name())) {
                    isUnitEntity = true;
                } else {
                    isUnitEntity = false;
                }

                unitArr.splice($pCell.parents("table").find("tr:last").index() - rowIndex + 1);  //去除多余的行，确保行数对应

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
                            //event. $target = $input;
                            $scope.Fn.GetEvt().data('target', $input);
                            //$scope.Fn.GetEvt().data('NoCheck', true);  //粘贴时不进行校核
                            /*$scope.Fn.GetEvt().data('NoCheck', true);
                            //$scope.Fn.GetEvt().data('NoCallUnitEntity', true);  //粘贴时不调用UnitEntity方法
                            $scope.Fn.GetEvt().data('NoCallUnitEntity', true);*/
                            $scope.Fn.GetEvt().data('disabledCallBack', ["UnitEntity", "RelationCheck"]);
                            $input.blur();
                        }
                    });

                    if (isUnitEntity && isChanged) { //非表5表6粘贴，每一行，调用OnChange.UnitEntity
                        index = $scope.Open.Report.Fn.Other.UnitIndex(obj.UnitCode);
                        obj = $.extend(angular.copy($scope.Open.Report.Current.HL011[index]), $scope.Open.Report.Current.HL014[index]);

                        if (!($scope.SysUserCode == "33" && $scope.Open.Report.Fn.Other.CurTable.Name() == "HL014")) {
                            $scope.Open.Report.Fn.Comm.OnChange.UnitEntity(obj);
                        }
                    }
                });
            } catch (e) {
                throw "粘贴出现错误：" + e;
            }

            $scope.Fn.GetEvt().removeData('target');
            $scope.Fn.GetEvt().removeData('disabledCallBack');

            return false;
        });
    });
}

switch ($.cookie("unitcode").slice(0, 2)) {
    case "43":
        $.getScript("../../Scripts/Models/HPModel.js");
        break;
    case "15":
        $.getScript("../../Scripts/Models/NPModel.js");
        break;
}