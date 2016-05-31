App.Plugin = App.Plugin || {};

App.Plugin.TableFixed = function () {
    var page = function (pageSetting) {
        var group = function (groupSetting) {
            var copy = function (queryElement) {
                var copyObj = {};
                copyObj.CopyElement = $(document.createElement("div")).addClass("Fixed");

                copyObj.Pos = function () {
                    if (queryElement && queryElement.length > 0) {
                        var tableOffset = table.offset();
                        var queryOffset = queryElement.offset();
                        var queryLeft = queryOffset.left - tableOffset.left - leftFix;
                        var queryTop = queryOffset.top - tableOffset.top - topFix;
                        var queryHeight = queryElement.outerHeight();
                        var queryWidth = queryElement[0].clientWidth;

                        if ((navigator.userAgent.indexOf('MSIE') >= 0) && (navigator.userAgent.indexOf('Opera') < 0)) {
                            queryLeft--;
                            queryTop--;
                        }
                        /*else if (navigator.userAgent.indexOf('Firefox') >= 0) {
                            alert('你是使用Firefox');
                        } else if (navigator.userAgent.indexOf('Opera') >= 0) {
                            alert('你是使用Opera');
                        } */

                        copyObj.CopyElement.css({
                            "left": queryLeft + "px",
                            "top": queryTop + "px",
                            "height": queryHeight + "px",
                            "line-height": queryHeight + "px",
                            "width": queryWidth + "px"
                        });
                    }
                };
                copyObj.Text = function () {
                    if (queryElement && queryElement.length > 0) {
                        copyObj.CopyElement.text(queryElement.text());
                    }
                };
                copyObj.Destroy = function () {
                    copyObj.CopyElement.remove();
                };

                if (groupElement && groupElement.length > 0) {
                    groupElement.append(copyObj.CopyElement);
                }
                copyObj.Pos();
                copyObj.Text();

                return copyObj;
            };

            var groupObj = {};
            var copys = [];
            var groupElement = $(document.createElement("div")).addClass("Group " + groupSetting.GroupType).hide();  //.attr("index", pageObj.Index)
            var moveString = groupSetting.MoveString;  //Ver Hor HorVer
            var topFix = groupSetting.TopFix || 0;
            var leftFix = groupSetting.LeftFix || 0;

            groupObj.GroupType = groupSetting.GroupType;  //Unit DW Head

            var moveInMoveString = function (move, moveStr) {
                return moveStr.indexOf(move) >= 0;
            };

            groupObj.AddCopy = function (queryElement) {
                var copyObj = new copy(queryElement);
                copys.push(copyObj);
                return copyObj;
            };
            groupObj.EachCopy = function (eachCallBack) {
                if (typeof (eachCallBack) == "function") {
                    for (var i = 0; i < copys.length; i++) {
                        eachCallBack(i, copys[i]);
                    }
                }
            };
            groupObj.Toggle = function (showOrHide) {
                if (showOrHide == undefined) showOrHide = false;
                groupElement.toggle(showOrHide);
            };
            groupObj.Resize = function () {
                groupObj.EachCopy(function (i, copyObj) {
                    copyObj.Pos();
                });
            };
            groupObj.Pos = function () {
                var tableScrollTop = tableScroller.scrollTop();
                var tableScrollLeft = tableScroller.scrollLeft();

                if (groupElement && moveInMoveString("Ver", moveString)) {
                    if (tableScrollLeft >= leftFix) {
                        groupObj.Toggle(true);
                        groupElement.css({ "left": tableScrollLeft + "px" });
                    } else {
                        groupObj.Toggle(false);
                    }
                }
                if (groupElement && moveInMoveString("Hor", moveString)) {
                    if (tableScrollTop >= topFix) {
                        groupObj.Toggle(true);
                        groupElement.css({ "top": tableScrollTop + "px" });
                    } else {
                        groupObj.Toggle(false);
                    }
                }
                if (moveInMoveString("Ver", moveString) && moveInMoveString("Hor", moveString)) {
                    if (tableScrollTop >= topFix || tableScrollLeft >= leftFix) {
                        groupObj.Toggle(true);
                    } else {
                        groupObj.Toggle(false);
                    }
                    if (tableScrollLeft == 0) {
                        var realLeft = tableScrollLeft + leftFix;
                        groupElement.css({ "left": realLeft + "px" });
                    }
                    if (tableScrollTop == 0) {
                        var realTop = tableScrollTop + topFix;
                        groupElement.css({ "top": realTop + "px" });
                    }
                }
            };
            groupObj.Destroy = function () {
                groupObj.EachCopy(function (i, copyObj) {
                    copyObj.Destroy();
                });
                groupElement.remove();
            };

            if (tableScroller.length > 0) {
                tableScroller.append(groupElement);
            }

            return groupObj;
        };

        var pageObj = {};
        var groups = {};
        var table = pageSetting.Table;
        var tableScroller = pageSetting.Table.parent();
        pageObj.Index = pageSetting.Index;

        pageObj.AddGroup = function (groupSetting) {
            if (groupSetting.GroupType) {
                groups[groupSetting.GroupType] = groups[groupSetting.GroupType] || new group(groupSetting);
            }
            return groups[groupSetting.GroupType];
        };
        pageObj.GetGroup = function (groupType) {
            return groups[groupType];
        };
        pageObj.EachGroup = function (eachCallBacks) {
            if (typeof (eachCallBacks) == "function") {
                for (var groupType in groups) {
                    if (Object.prototype.hasOwnProperty.call(groups, groupType)) {
                        eachCallBacks(groupType, groups[groupType]);
                    }
                }
            }
        };
        pageObj.ToggleGroup = function (showOrHide) {
            if (showOrHide == undefined) showOrHide = false;
            pageObj.EachGroup(function (groupType, groupObj) {
                if (groupObj) groupObj.Toggle(showOrHide);
            });
        };
        pageObj.Resize = function () {
            pageObj.EachGroup(function (groupType, groupObj) {
                if (groupObj) groupObj.Resize();
            });
        };
        pageObj.Pos = function () {  //固定元素
            pageObj.EachGroup(function (groupType, groupObj) {
                if (groupObj) groupObj.Pos();
            });
        };
        pageObj.Destroy = function () {
            pageObj.EachGroup(function (groupType, groupObj) {
                groupObj.Destroy();
            });
            tableScroller.off("scroll", scrollEvent);
            $(window).off("resize", resizeEvent);
        };

        var resizeEvent = function () {
            if (["Index", "HistoryDisaster"].In_Array($scope.Attr.RootName)) {
                if ($scope.Report[$scope.Attr.NameSpace] && $scope.Report[$scope.Attr.NameSpace].Current && $scope.Report[$scope.Attr.NameSpace].Current.Attr.TableIndex == pageObj.Index) {
                    pageObj.Resize();
                }
            } else {
                pageObj.Resize();
            }
        };
        var scrollEvent = function () {
            if (["Index", "HistoryDisaster"].In_Array($scope.Attr.RootName)) {
                if ($scope.Report[$scope.Attr.NameSpace].Current.Attr.TableIndex == pageObj.Index) {
                    if (pageObj.Index == 4 || pageObj.Index == 5) {
                        pageObj.Resize();
                    }
                    pageObj.Pos();
                }
            } else {
                pageObj.Pos();
            }
        };

        tableScroller.on("scroll", scrollEvent);
        $(window).on("resize", resizeEvent);

        return pageObj;
    };

    var tableFixed = {
        FixedPage: {}
    };

    var eachPage = function (eachCallBack) {
        if (typeof (eachCallBack) == "function") {
            for (var pageIndex in tableFixed.FixedPage) {
                if (Object.prototype.hasOwnProperty.call(tableFixed.FixedPage, pageIndex)) {
                    eachCallBack(pageIndex, tableFixed.FixedPage[pageIndex]);
                }
            }
        }
    };

    tableFixed.Fix = function (table) {
        var pageIndex = table.Index;
        var pageSetting = {
            Index: pageIndex,
            Table: table.Element,
            Top: table.Top == undefined ? 17 : table.Top
        };
        tableFixed.FixedPage[pageIndex] = new page(pageSetting);
        var dwQuery = [];
        if (["Index", "HistoryDisaster"].In_Array($scope.Attr.RootName)) {
            if ($scope.Report[$scope.Attr.NameSpace].Current.ReportTitle.ORD_Code.indexOf('SH0') >= 0) {
                if ($scope.Report[$scope.Attr.NameSpace].Current.ReportTitle.ORD_Code == 'SH01') {
                    dwQuery = pageSetting.Table.find("thead tr th:first");
                }
            } else {
                dwQuery = pageSetting.Table.find("thead tr th").filter(function() {
                    return $(this).text().indexOf($scope.Report[$scope.Attr.NameSpace].Current.ReportTitle.ORD_Code == 'HL01' ? "地区" : "单位") >= 0;
                });
            }
        }

        //var unitGroupObj, dwGroupObj;
        /*if ($scope.Report[$scope.Attr.NameSpace].Current.ReportTitle.ORD_Code.indexOf('SH0') > 0) {
            unitGroupObj = tableFixed.FixedPage[pageIndex].AddGroup({ GroupType: "Unit", MoveString: "Ver" });
            pageSetting.Table.find("tbody tr th").each(function () {  //遍历非表5、表6的“地区”列
                unitGroupObj.AddCopy($(this));
            });

            dwGroupObj = tableFixed.FixedPage[pageIndex].AddGroup({ GroupType: "DW", MoveString: "HorVer", TopFix: pageSetting.Top });
            dwQuery.each(function () {
                dwGroupObj.AddCopy($(this));
            });
        } else {*/
        if (!(pageIndex == 4 || pageIndex == 5) && table.Type != "Head" && $scope.Report[$scope.Attr.NameSpace].Current.ReportTitle.ORD_Code.indexOf('SH0') < 0
            || $scope.Report[$scope.Attr.NameSpace].Current.ReportTitle.ORD_Code == 'SH01') {

            var unitGroupObj = tableFixed.FixedPage[pageIndex].AddGroup({ GroupType: "Unit", MoveString: "Ver" });
            pageSetting.Table.find("tbody tr th").each(function() { //遍历非表5、表6的“地区”列
                unitGroupObj.AddCopy($(this));
            });

            var dwGroupObj = tableFixed.FixedPage[pageIndex].AddGroup({ GroupType: "DW", MoveString: "HorVer", TopFix: pageSetting.Top });
            dwQuery.each(function() {
                dwGroupObj.AddCopy($(this));
            });
        }
        //}

        var headGroupObj = tableFixed.FixedPage[pageIndex].AddGroup({ GroupType: "Head", MoveString: "Hor", TopFix: pageSetting.Top });
        pageSetting.Table.find("thead tr th").each(function () {
            if ($(this).text().trim().length > 0) {
                headGroupObj.AddCopy($(this));
            }
        });

        tableFixed.FixedPage[pageIndex].Pos();
        tableFixed.FixedPage[pageIndex].Resize();
    };
    tableFixed.Destroy = function () {
        eachPage(function (pageIndex, pageObj) {
            pageObj.Destroy();
        });
        TableFixed.FixedPage = {};
    };

    return tableFixed;
} ();
    