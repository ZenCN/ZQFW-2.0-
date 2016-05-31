//加载图例标题和文字（受灾类型，图例组）
var loadLegend = function (disasterInfoType, level) {
    var mapParams = window.parent.window.mapParams;
    var title = mapParams.disasterInfoTypes[disasterInfoType].name + "(" + window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].unit + ")";    //获取当前标题
    var breakValueArr = mapParams.disasterInfoTypes[disasterInfoType].breakValues[level];   //获取分类间断值;
    var colorArr = mapParams.colors[level];   //获取地图级别对应分类渲染的颜色

    var json = []

    for (var i = 0; i <= breakValueArr.length; i++) {
        var obj = {};
        if (i == 0) {
            obj.lowerVal = 0;
            obj.higherVal = breakValueArr[i];
        }
        else if (i == breakValueArr.length) {
            obj.lowerVal = breakValueArr[i - 1];
            obj.higherVal = "";  
        }
        else {
            obj.lowerVal = breakValueArr[i - 1];
            obj.higherVal = breakValueArr[i];  
        }
        obj.color = colorArr[i];
        json.push(obj);
    }

    legendEditor1 = new legendEditor({
        colors: json,
        rader: 'legendEditor',
        title: title
    });
}

var legendEditor = function (settings) {
    var proprety = $.extend({
        colors: [],
        rader: '',
        title: '标题'
    }, settings);
    if (proprety.colors.length > 0) {

        var title = $('<label>' + proprety.title + '</label>');
        var colors = $('<ul></ul>');
        var _setcolor = function () {
            for (var i = 0; i < proprety.colors.length; i++) {
                var li = $('<li></li>');
                var readOnly = "";
                if (i == proprety.colors.length - 1) {
                    readOnly = "readonly";
                }
                var eleLegend = $("<p><input type='text' class='lowerVal' order=" + i + " value='" + proprety.colors[i].lowerVal +
                 "' /><span>至</span><input type='text' class='higherVal' order=" + i + " value='" + proprety.colors[i].higherVal + "' " + readOnly + "/></p>");
                li.append($('<div class="colorblocks" style="background:' + proprety.colors[i].color + '"></div>')).append(eleLegend);
                colors.append(li);
            }
        }

        _setcolor();

        var btnOK = $("<button  type='button'>确定</button>");
        var btnCancel = $("<button  type='button'>取消</button>");
        var bottom = $("<div class='bottom'></div>").append(btnOK).append(btnCancel);
        var main = $('#' + proprety.rader).append(title).append(colors).append(bottom);

//        $("#" + proprety.rader + " input").keydown(function (e) {
//            //            var evt = window.event ? window.event : e;
//            //            if (evt.keyCode == 8 | evt.keyCode == 46 | evt.keyCode == 110) {     //如果是退格、删除或小键盘删除键
//            //                var preIndex = $(".addrContainer .addr").index($("#" + id)) - 1;
//            //                $("#" + id).remove()        //删除元素
//            //                $("#i" + id).remove()        //删除间隔元素
//            //                if ($(".addrContainer .addr").length < 1) {
//            //                    $(".addrContainer").html(tip1);
//            //                }
//            //                if (preIndex >= 0) {
//            //                    $(".addrContainer a:eq(" + preIndex + ")").focus();
//            //                }
//            //                else {
//            //                    $(".addrContainer").focus();
//            //                }
//            //                if (evt.keyCode == 8) {
//            //                    stopDefault(evt);  //阻止浏览器默认事件
//            //                }
//            //            }
//        });


        $("#" + proprety.rader + " input").keyup(function () {
            this.value = this.value.replace(/\D/g, '');
        });

//        $("#" + proprety.rader + " input").afterpaste(function () {
//            this.value = this.value.replace(/\D/g, '')
//        });

        $("#" + proprety.rader + " input").blur(function () {
            var order = parseInt($(this).attr("order"));
            if ($(this).hasClass("lowerVal") && order >= 1) {
                $(".higherVal[order=" + (order - 1) + "]").val($(this).val());
            }
            else if ($(this).hasClass("higherVal")) {
                $(".lowerVal[order=" + (order + 1) + "]").val($(this).val());
            }
        });

        btnOK.click(function () {
            $("#" + proprety.rader).hide();
        });

        btnCancel.click(function () {
            $("#" + proprety.rader).hide();
        });

        return {
            html: main,
            resetitle: function (retitle) {
                title.html(retitle);
            },
            resetcolor: function (newcolor) {
                proprety.colors = newcolor;
                colors.empty();
                _setcolor();
            }
        }

    } else {
        return false;
    }
}
