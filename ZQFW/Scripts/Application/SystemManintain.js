window.App = window.App ? window.App : {};

App.System = {
    Exit: function () {
        if (confirm("确定要退出？")) {
            location.href = location.href.indexOf('?debug=1') > 0 ? '/login?debug=1' : '/login';
        }
    },
    BackToMain: function () {
        if (confirm("确定要返回主界面？")) {
            location.href = location.href.indexOf('?debug=1') > 0 ? '/main?debug=1' : '/main';
        }
    }
};

$(function () {
    $(".TopLeft span").text($.cookie("realname"));

    $(".BackUp").mousedown(function () {
        if ($(".BackUp").hasClass("Run")) {
            $(".Message ul").append("<li>正在备份数据库，不能取消</li>");
            $(".BackUp").unbind("mouseup", fn);
        } else {
            $(".BackUp").addClass("Start");
        }
    }).bind("mouseup", function () {
        $(".Message ul").empty().append("<li>开始向服务器请求备份数据库...</li>");
        $.ajax({
            type: 'post',
            url: 'SystemMaintain/BackupData',
            beforeSend: function () {
                $(".BackUp").removeClass("Start").addClass("Run");
                $(".Message ul").append("<li>请求已发出，等待服务器响应...</li>");
            },
            success: function (data) {
                $(".BackUp").removeClass("Run");
                if (data == 1) {
                    $(".Message ul").append("<li>数据库备份成功！</li>");
                    $(".BackUp").addClass("Success");
                } else {
                    $(".Message ul").append("<li>数据库备份失败，详情：" + data + "</li>");
                    $(".BackUp").addClass("Error");
                }
            },
            error: function (xhr) {
                $(".Message ul").append("<li>http请求出错，状态：" + xhr.status + "</li>");
                $(".BackUp").addClass("Error");
            },
            complete: function () {
                $(".BackUp").removeClass("Success").removeClass("Error");
            }
        });
    });

    if (parseInt($.cookie('sso')) > 0) {
        $(".Head").find(".Bottom").hide();
        $(".MenuBar").css("top", "21px");
        $(".Frame").css("top", "55px");
    }
});







