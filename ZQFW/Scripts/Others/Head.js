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

    $(".Head .Bottom .Image").css('background-image', 'url(../CSS/Img/Public/Logo/' + $.cookie('unitcode').substr(0, 2) + '.png' + ')');
});