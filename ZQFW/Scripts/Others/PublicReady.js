App.JGrowlMessage = {
    Attr: {
        Urge: [],
        Send: []
    },
    Fn: {
        Show: function (element) {
            element = $(element);
            $scope.Dialog.Config({
                Title: "催报消息",
                Layout: "Message",
                Content: App.JGrowlMessage.Attr.Urge,
                Selected: App.JGrowlMessage.Attr.Urge.Find("TBNO", element.attr("id")),
                Width: '520px'
            });
            $scope.$apply();
        },
        Read: function (element) {
            var result = $scope.Fn.Ajax('UrgeReport/ReadUrgeReport', { TBNO: $(element).attr("id") }, 0, true);

            if (isNaN(result)) {
                alert(result);
                throw result;
            }
        },
        Open: function (text, pageno) {
            if ($.cookie('limit') < 5) {
                if ($scope.CurrentUrl != '/Open') {
                    $scope.RedictUrl('/Open');
                }

                if (!$scope.Open.Report.Current || $scope.Open.Report.Current.ReportTitle.PageNO != pageno) {
                    var sourceType;
                    if (text.indexOf("[汇总]") > 0) {
                        sourceType = 1;
                    } else if (text.indexOf("[累计]") > 0) {
                        sourceType = 2;
                    } else {
                        sourceType = 0;
                    }
                    $scope.Open.Report.Fn.Core.Open({
                        rptType: 'HL01',
                        sourceType: sourceType,
                        pageno: pageno,
                        notice: "拒收原因：" + text.split("&&")[0]
                    });
                } else {
                    $scope.Open.Report.Current.Attr.Notice = "拒收原因：" + text.split("&&")[0];
                    $scope.$apply();
                }
            } else {
                $scope.New.Report.Fn.Open(pageno);
                $scope.New.Report.Current.Attr.Notice = text;
                $scope.$apply();
            }
        }
    }
};

window.onload = function() {
    $.jGrowl.defaults.closerTemplate = "<div style=' width: 180px'>关闭所有消息</div>";

    $.ajax({
        url: "UrgeReport/GetUrgeReportList?fresh=" + Math.random(),
        type: "post",
        success: function(data) {
            if (data.length > 0) {
                try {
                    data = eval("({" + data + "})");
                    angular.forEach(data.urgeReport, function (obj) {
                        obj.UrgeRptContent = obj.UrgeRptContent.replaceAll("{/r-/n}", "\n");  //.replaceAll("&syh;", "\"").replaceAll("&dyh;", "\'")
                        App.JGrowlMessage.Attr.Urge.push(obj);
                    });
                    var htmlText = undefined;
                    $.each(App.JGrowlMessage.Attr.Urge, function () {
                        htmlText = "<a href='javascript:void(0)' onclick='App.JGrowlMessage.Fn.Show(this)' class='Link' title='" + this.UrgeRptContent +
                        "' id='" + this.TBNO + "'>催报单位：<span name='UnitNmae'>" + this.SendUnitName + "</span><br/>&nbsp;&nbsp;&nbsp;催报人：<span name='PersonName'>" +
                        this.UrgeRptPersonName + "</span><br/>催报日期：<span name='Date'>" + this.UrgeRptDateTime + "</span>" + "</a>";
                        $("#jGrowlMessage").jGrowl(htmlText, {
                            pool: 6,
                            //sticky: true,
                            life: 300000,
                            position: 'bottom-right',
                            //theme: 'iphone',
                            close: function (e, m) {
                                App.JGrowlMessage.Fn.Read($(m)[0]);
                            }
                        });
                    });
                } catch (e) {
                    e = "获取催报消息出错，详情：" + e;
                    Alert(e);
                    throw e;
                } 
            }
        }
    });


    if ($.cookie("ord_code") == 'HL01') {
        setInterval(function() {
            $.ajax({
                url: "Index/ReadMsg?fresh=" + Math.random(),
                type: "post",
                success: function(d) {
                    if (d.indexOf("{") >= 0) {
                        try {
                            var data = eval("(" + d + ")");
                            var htmlText, arr, detials_arr;

                            if (parseInt($.cookie("limit")) < 5) {
                                $.each(data.Send.Details, function() {
                                    htmlText = "<div class='Center'>" + this.SendUnitName + "发来" + this.Count + "套报表</div>";
                                    $("#jGrowlMessage").jGrowl(htmlText, {
                                        pool: 6,
                                        sticky: true,
                                        life: 60000,
                                        position: 'bottom-right'
                                    });
                                });

                                if (data.Send.Count > 0) {
                                    $(".MenuBar .InboxMsg").text(data.Send.Count);
                                }
                            }

                            $.each(data.Refuse.Details, function() {
                                arr = this.Info.split("||");
                                _this = this;
                                $.each(arr, function(i, detials) {
                                    detials_arr = detials.split("&&");
                                    htmlText = _this.SendUnitName + "拒收了" + detials_arr[2];
                                    htmlText = "<div style='cursor: pointer;' title='点击打开报表' onclick=App.JGrowlMessage.Fn.Open('" + detials_arr[0] + "','" + detials_arr[1] + "')>" + htmlText + "</div>";
                                    $("#jGrowlMessage").jGrowl(htmlText, {
                                        pool: 6,
                                        sticky: true,
                                        //life: 60000,
                                        position: 'bottom-right'
                                    });
                                });
                            });
                        } catch (e) {
                            e = "获取消息出错，详情：" + e;
                            Alert(e);
                            throw e;
                        }
                    }
                },
                error: function() {
                    Alert("获取催报错误！");
                }
            });
        }, 1000 * 60 * 10); //10分钟刷新一次 1000 * 60 * 10
    }

/*    setInterval(function () {
        $.ajax({
            url: "Login/LoginSession",
            type: "post",
            success: function (json) {
                if (Number(json) == 0) {
                    window.location.href = "/Login";
                }
            }
        });
    }, 1000 * 60 * 1.5);*/
};