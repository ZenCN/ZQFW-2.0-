App.directive('tooltipster', function () {

    return function ($scope, $element, $attrs   ) {
        var field = undefined;
        
        if ($attrs["ngModel"]) {
            field = $attrs["ngModel"].substr($attrs["ngModel"].lastIndexOf(".") + 1);
        } else {
            field = $attrs["tooltipster"];
        }

        $element.attr("title", $scope.BaseData.Field[field].Title);
        
        $(function() {
            $element.tooltipster({
                animation: "grow",
                arrow: true,
                maxWidth: 300,
                position: "right",
                theme: ".tooltipster-shadow",
                trigger: "click"
            });
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

$(function() {
    $(document).click(function() {
        if (!$((event.srcElement?event.srcElement:event.target)).is("[tooltipster]")) {
            $("[tooltipster]").tooltipster("hide");
        }
    }).keydown(function() {
        if ((event.keyCode == 27 || event.which == 27) && $(".Shadow").width()) {
            $scope.New.Report.Fn.DeadPeople.Hide();
        }
    });
});

window.onresize = function () {
    if ($(".Shadow").width()) {
        $scope.New.Report.Fn.DeadPeople.Show();
    }
};