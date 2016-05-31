var legend1 = null;   //是否第一次加载图例

//更新图例标题和文字（受灾类型，图例组）
var updataLegend = function () {
    var mapParams = window.parent.window.mapParams;
    var breakValueArr = [-1, 40, 60, 80];   //获取分类间断值;
    var colorArr = ['#FFFF00', '#FF7878', '#ffbc75', '#95ceff', '#a9ff96'];   //获取地图级别对应分类渲染的颜色, '#00FFFF'

    var json = []
//    var obj1 = {};
//    obj1.val = "20%以下"
//    obj1.color = colorArr[0];
//    json.push(obj1);

    var obj2 = {};
    obj2.val = "40%以下(较差)";
    obj2.color = colorArr[1];
    json.push(obj2);

    var obj3 = {};
    obj3.val = "40%-60%(一般)";
    obj3.color = colorArr[2];
    json.push(obj3);

    var obj4 = {};
    obj4.val = "60%-80%(较好)";
    obj4.color = colorArr[3];
    json.push(obj4);

    var obj5 = {};
    obj5.val = "80%以上(好)";
    obj5.color = colorArr[4];
    json.push(obj5);

    var title = "";
    if (!legend1) {    //第一次创建
        legend1 = new Legend({
            colors: json,
            rader: 'legend',
            title: title
        });
    } else {
//        legend1.resetitle(title); //第二次更改
        legend1.resetcolor(json);
    }
}

var Legend = function (settings) {
    var proprety = $.extend({
        colors: [],
        rader: '',
        title: '标题',
        disasterInfoType: "",
        level: ""
    }, settings);
    if (proprety.colors.length > 0) {
        var colors = $('<ul></ul>');
        var _setcolor = function () {
            for (var i = 0; i < proprety.colors.length; i++) {
                var li = $('<li></li>');
                li.append($('<div class="colorblocks" style="background:' + proprety.colors[i].color + '"><svg xmlns="http://www.w3.org/2000/svg" version="1.1" ><rect  style="fill:'
                + proprety.colors[i].color + ';fill-opacity:1;" width="100%" height="100%"  /></svg></div>')).append('<p>' + proprety.colors[i].val + '</p>');
                colors.append(li);
            }
        }
        _setcolor();
        var main = $('#' + proprety.rader)
                    .append(colors);
        return {
            html: main,
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
