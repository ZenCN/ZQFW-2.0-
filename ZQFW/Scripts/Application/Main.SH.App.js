$(document).keypress(function (e) {
    if (e.keyCode == 13) { //回车键
        if ($(".dialog").is(":visible")) {
            App.Main.Login.Validate();
        }
    }
});

window.App = window.App ? window.App : {};

App.System = {
    Exit: function() {
        if (confirm("确定要退出？")) {
            location.href = location.href.indexOf('?debug=1') > 0 ? '/login?debug=1' : '/login';
        }
    },
    BackToMain: function() {
        if (confirm("确定要返回主界面？")) {
            location.href = location.href.indexOf('?debug=1') > 0 ? '/main?debug=1' : '/main';
        }
    }
};

$(function () {
    if (window.location.pathname.toLowerCase().indexOf('/main') == -1) {
        $("#BackToMain").show();  //不在主界面显示“返回主界面”
    } else {
        $("#BackToMain").hide();
    }

    $(".Head .Bottom .Image").css('background-image', 'url(../CSS/Img/Public/Logo/15_SH.png' + ')');

    App.Main.Resize();
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
    Resize: function () {
        var config = {
            width: '708px',
            height: '213px',
            marginTop: '300px'
        };

        $(".Main").css(config);
    },
    Report: {
        Disaster: function () {
            location.href = location.href.indexOf('?debug=1') > 0 ? '/sh?debug=1' : '/sh';
        }
    }
};