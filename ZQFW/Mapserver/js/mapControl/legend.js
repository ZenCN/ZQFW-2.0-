var legend1 = null;   //是否第一次加载图例

//更新图例标题和文字（受灾类型，图例组）
var updataLegend = function (disasterInfoType, level) {
    var mapParams = window.parent.window.mapParams;
    var title = mapParams.disasterInfoTypes[disasterInfoType].name + "(" + window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].unit + ")";    //获取当前标题
    var breakValueArr = mapParams.disasterInfoTypes[disasterInfoType].breakValues[level];   //获取分类间断值;
    var colorArr = mapParams.colors[level];   //获取地图级别对应分类渲染的颜色

    var json = []
    if (mapParams.mapType == 3) {    //如果渲染灾情评估中的地图
        title = "灾情等级"
        var obj1 = {};
        obj1.val = "轻灾"
        obj1.color = colorArr[1];
        json.push(obj1);

        var obj2 = {};
        obj2.val = "一般灾害"
        obj2.color = colorArr[2];
        json.push(obj2);

        var obj3 = {};
        obj3.val = "重大灾害"
        obj3.color = colorArr[3];
        json.push(obj3);

        var obj4 = {};
        obj4.val = "特大灾害"
        obj4.color = colorArr[4];
        json.push(obj4);
    }
    else {       //如果渲染灾情分布中的地图
        for (var i = 0; i <= breakValueArr.length; i++) {
            var obj = {};
            if (i == 0) {
                obj.val = "小于" + breakValueArr[i];
            }
            else if (i == breakValueArr.length) {
                obj.val = "大于" + breakValueArr[i - 1];
            }
            else {
                obj.val = breakValueArr[i - 1] + "-" + breakValueArr[i];
            }
            obj.color = colorArr[i];
            json.push(obj);
        }
    }

    if (!legend1) {    //第一次创建
        legend1 = new legend({
            colors: json,
            rader: 'legend',
            title: title,
            disasterInfoType: disasterInfoType, 
            level:level
        });
    } else {
        legend1.resetitle(title); //第二次更改
        legend1.resetcolor(json);
    }


    ////    var breakValues = window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].breakValues;
    ////    for (var i = 0; i < window.parent.window.mapParams.disasterInfoTypes.length; i++) {

    ////    }
}

var legend = function (settings) {
    var proprety = $.extend({
        colors: [],
        rader: '',
        title: '标题',
        disasterInfoType: "",
        level: ""
    }, settings);
    if (proprety.colors.length > 0) {
        var zoomer = $('<span id="zoomer" class="notprint"></span>');
        var editor = $('<span id="editor" class="notprint"></span>');  //图例编辑启动按钮
        var title = $('<label>' + proprety.title + '</label>');

        var colors = $('<ul></ul>');
        var _setcolor = function () {
            for (var i = 0; i < proprety.colors.length; i++) {
                var li = $('<li></li>');
                //                li.append($('<span class="colorblocks"></span>').css('background', proprety.colors[i].color)).append('<p>' + proprety.colors[i].val + '</p>');
                //                li.append($('<div class="colorblocks"></div>').css('background', proprety.colors[i].color)
                //                .append($('<svg version="1.1" ></svg>').append('<rect  style="fill:' + proprety.colors[i].color
                //                + ';fill-opacity:0.7;" width="100%" height="100%"  />'))).append('<p>' + proprety.colors[i].val + '</p>');
                //                li.append($('<div class="colorblocks" style="background:' + proprety.colors[i].color + '"><svg xmlns="http://www.w3.org/2000/svg" version="1.1" ><rect  style="fill:'
                //                + proprety.colors[i].color + ';fill-opacity:0.7;" width="100%" height="100%"  /></svg></div>')).append('<p>' + proprety.colors[i].val + '</p>');
                //                colors.append(li);
                li.append($('<div class="colorblocks" style="background:' + proprety.colors[i].color + '"></div>')).append('<p>' + proprety.colors[i].val + '</p>');
                colors.append(li);
            }
        }
        _setcolor();
        var main = $('#' + proprety.rader)
                    .append(zoomer)
                    .append(title)
                    //.append(editor)   //添加图例编辑按钮
                    .append(colors);

        zoomer.click(function () {
            zoomer.toggleClass('small');
            title.toggle(!zoomer.hasClass('small'));
            colors.toggle(!zoomer.hasClass('small'));
            editor.toggle(!zoomer.hasClass('small'));
        })
        var firstLoad = true;
        editor.click(function () {    //图例编辑启动按钮事件
            if (firstLoad == true) {
                loadLegend(proprety.disasterInfoType, proprety.level);
                firstLoad = false;
            }
            $("#legendEditor").show();
        })

        return {
            html: main,
            resetitle: function (retitle) {
                title.html(retitle);
            },
            resetcolor: function (newcolor) {
                proprety.colors = newcolor;
                colors.empty();
                _setcolor();
            },
            zoom: function () {
                $('#' + proprety.rader)
            }
        }

    } else {
        return false;
    }
}
