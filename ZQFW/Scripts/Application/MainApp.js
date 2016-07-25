$(document).keypress(function (e) {
    if (e.keyCode == 13) { //回车键
        if ($(".dialog").is(":visible")) {
            App.Main.Login.Validate();
        }
    }
});

window.App = window.App || {};
App.Main = {
    Login: {
        Dialog: {
            Open: function () {
                easyDialog.open({
                    container: 'Dialog'
                });
                $("#lgn_pwd").focus();
            },
            Close: function () {
                easyDialog.close();
                $("#lgn_pwd").val("");
                $("#lgn_error_msg").text("");
            }
        },
        Validate: function () {
            var pwd = $("#lgn_pwd").val();
            if (pwd == "") {
                $("#lgn_error_msg").text("密码不能为空");
                return;
            }
            $.ajax({
                url: "/BaseData/BaseDataLogin",
                type: "post",
                data: "pwd=" + pwd,
                success: function (flag) {
                    if (flag == "success") {
                        location.href = location.href.indexOf('?debug=1') > 0 ? '/basedata?debug=1' : '/basedata';
                    } else {
                        $("#lgn_error_msg").text("密码错误");
                    }
                }
            });
        }
    },
    Resize: function (){
        if (parseInt($.cookie("authority")) == 1) {
            return;
        }

        var config = {
            width: '1180px',
            height: '213px',
            marginTop: '300px'
        };

        if (document.body.clientWidth < 1180) {
            config = {
                width: '710px',
                height: '426px',
                marginTop: '13%'
            };
        }

        $(".Main").css(config);
    },
    Report: {
        Disaster: function () {
            $.cookie('ord_code', 'HL01');
            location.href = '/index';
        },
        Reservoir: function () {
            if (sys_code == "43") {
                $.cookie('ord_code', 'HP01');
            } else if (sys_code == "15") {
                $.cookie('ord_code', 'NP01');
            }
            location.href = '/index';
        }
    }
};

var sys_code = $.cookie("unitcode").slice(0, 2);

$(function () {
    if (sys_code == "43" || sys_code == "15") {
        $(".Main div").find("#ReservoirReport").parent().show();
    }

    if ($.cookie("limit") != 2) { //浙江市级、县级有基础数据、历年灾情、基础数据
        $('#StatisticAnalysis, #BaseData, #SystemSetting').parent().hide(); //市级、县级有基础数据、历年灾情  .parent().width('473px')

        if (sys_code == "33") {
            $(".Main").find("#StatisticAnalysis, #SystemSetting").parent().hide();  //.parent().width('473px')
        } /* else if (sys_code == "43" || sys_code == "15") {
                    $(".Main").width("709px");
                }*/

        $(".Main").width($("div.Main div:visible").length * 236 + 'px');
    } else {
        window.onresize = App.Main.Resize;
    }

    if (parseInt($.cookie('sso')) > 0) {
        $(".Head").find(".Bottom").hide().end().find(".Top").css("borderRadius", "6px");
    }

    if ($.cookie("limit") == 2 && parseInt($.cookie("authority")) == 1) {
        $('#BaseData').parent().remove();
        $('#SystemSetting').parent().remove();
        $('.Main').css({
            width: '710px',
            height: '213px',
            marginTop: '300px'
        });
    }
});