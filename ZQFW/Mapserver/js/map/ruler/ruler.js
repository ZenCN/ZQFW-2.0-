var normal_ponit = function(){          //普通标尺点原型
    var normal_ponit_li = $('<li class="normal_ponit"></li>');
    return normal_ponit_li;
}
var pointer = function(type){           //前后标尺箭头原型
    var pointer_li = $('<li class ="'+type+'_pointer"></li>');
    return pointer_li;
}
var top_tip = function(property){       //顶部提示气泡原型
    var top_tip_span = $('<span class="top_tip"></span>');
    var top_tip_a = $('<a>'+property.text+'</a>');
    top_tip_span.html(top_tip_a);
    if (property.left){                 //由参数设置气泡不同属性
        top_tip_span.css({'margin-left':'-'+property.left+'px'});
    }
    if (property.width){
        top_tip_span.css({'width':property.width-6+'px'});
        top_tip_a.css({'width':property.width+'px'})
    }
    return top_tip_span;
}
var bottom_tip = function(property){    //底部提示气泡
    var bottom_tip_span = $('<span class="bottom_tip">' + property.text + '</span>');
    return bottom_tip_span;
}

var ruler = function (porperty) {
    if (porperty.renderTo) {//必要属性
        var line = $('<ul class="line"></ul>');
        var normal_ponit_list = new Array;
        var left_pointer = new pointer('left'), right_pointer = new pointer('right');
        var monthtitle = 0;         //月份记录器
        var selected;               //当前被选标点实例
        var last;                   //最后一个标点实例
        var selectedPageno;         //当前被选的报表编号
        var interval;               //计时器事件

        var backToSelected = function () {                //将底部气泡返回到被选报表标点下方
            $('.bottom_tip').remove();
            var textstr = selected.startmonth + '月' + selected.startday + '日-' + selected.endmonth + '月'
                + selected.endday + '日 ' + selected.sorcetype + selected.updateTime;
            //                var textstr = curr.updateTime + curr.sorcetype;
            selected.append(new bottom_tip({ text: textstr }));
        }

        $("#ruler").mouseleave(function () {
            backToSelected();
        });

        $('#' + porperty.renderTo).append(line); //将标尺ul添加进属性中指定id的元素中

        line.append(left_pointer);
        if (porperty.data) { //如果存在数组数据   
            for (var i = 0; i < porperty.data.length; i++) {
                normal_ponit_list[i] = new normal_ponit;
                var startdate = porperty.data[i].StartDate.split(',');  //报表开始时间
                var enddate = porperty.data[i].EndDate.split(','); //报表结束时间
                var writeTime = porperty.data[i].WriteTime;
                normal_ponit_list[i].updateTime = writeTime;
                normal_ponit_list[i].startyear = startdate[0];              //报表起日期年
                normal_ponit_list[i].startmonth = startdate[1];             //报表起日期月
                normal_ponit_list[i].startday = startdate[2];               //报表起日期日

                normal_ponit_list[i].endyear = enddate[0];                  //报表止日期年
                normal_ponit_list[i].endmonth = enddate[1];                 //报表止日期月
                normal_ponit_list[i].endday = enddate[2];                   //报表止日期日

                normal_ponit_list[i].pageno = porperty.data[i].Pageno; //报表页号
                normal_ponit_list[i].sorcetype = porperty.data[i].SorceTypeName; //报表类型

                normal_ponit_list[i].attr('id', i);

                if (normal_ponit_list[i].endmonth != monthtitle) {  //生成月份顶部标签
                    monthtitle = normal_ponit_list[i].endmonth;
                    if (normal_ponit_list[i].endmonth.length > 1) {  //根据月份位数调整标签宽度
                        normal_ponit_list[i].append(new top_tip({ text: normal_ponit_list[i].endmonth + '月', width: 30, left: 3 }));
                    } else {
                        normal_ponit_list[i].append(new top_tip({ text: normal_ponit_list[i].endmonth + '月', width: 25 }));
                    }
                }

                //鼠标悬停和离开事件
                normal_ponit_list[i].hover(
                    function (taget) {  //鼠标悬停时
                        //                        clearInterval(interval);
                        var curr = normal_ponit_list[$(taget.currentTarget).attr('id')];
                        var textstr = curr.startmonth + '月' + curr.startday + '日-' + curr.endmonth + '月' + curr.endday + '日 ' + curr.sorcetype + curr.updateTime;
                        $('.bottom_tip').remove();
                        $(this).append(new bottom_tip({ text: textstr }));
                    },
                    function () {   //鼠标离开时
                        //                        interval = setTimeout(function () {
                        //                            backToSelected();
                        //                        }, 500);
                    });

                normal_ponit_list[i].click(function (taget) {  //     标点点击事件
                    var curr = normal_ponit_list[$(taget.currentTarget).attr('id')];
                    var textstr = curr.startmonth + '月' + curr.startday + '日-' + curr.endmonth + '月' + curr.endday + '日 ' + curr.sorcetype + curr.updateTime;
                    //                    var textstr = curr.updateTime + curr.sorcetype;
                    $('.bottom_tip').remove();
                    $('.normal_ponit').removeClass('selected');
                    $(taget.currentTarget).addClass('selected');

                    $(this).append(new bottom_tip({ text: textstr }));

                    selected = curr;                        //设置当前选择报表实例
                    selectedPageno = selected.pageno;       //设置当前选择报表编号
                    //alert(selectedPageno);
                    if (porperty.event) {                    //如果存在用户定义事件  则执行用户定义事件
                        porperty.event();
                    }
                })

                var setlast = function (last_) {
                    var curr = last_;
                    var textstr = curr.startmonth + '月' + curr.startday + '日-' + curr.endmonth + '月' + curr.endday + '日 ' + curr.sorcetype + curr.updateTime;
                    //                    var textstr = curr.updateTime + curr.sorcetype;
                    $('.bottom_tip').remove();
                    $('.normal_ponit').removeClass('selected');
                    $(last_).addClass('selected');

                    $(last_).append(new bottom_tip({ text: textstr }));

                    selected = curr;                        //设置当前选择报表实例
                    selectedPageno = selected.pageno;       //设置当前选择报表编号
                }

                line.append(normal_ponit_list[i]);
                //$('#'+porperty.renderTo).css('margin-left',-14*i+'px');
            }
            normal_ponit_list[0].addClass('first_ponit');
            normal_ponit_list[porperty.data.length - 1].addClass('last_ponit'); //click();
            last = normal_ponit_list[porperty.data.length - 1];

            setlast(last);
        }

        line.append(right_pointer);
        this.getSelected = function () {
            return selected;
        };
        this.getLast = function () {
            return last;
        }
        this.GetPointList = function () { //返回标尺报表点数组
            return normal_ponit_list;
        };
        this.SetPointList = function (newlist) {  //重新设置报表点数组            
            if (newlist.data) {
                monthbegin = 0;
                $('.top_tip').remove();
                $('.bottom_tip').remove();
                $('.normal_ponit').removeClass('selected');
                for (var i = 0; i < newlist.data.length; i++) {
                    var startdate = newlist.data[i].StartDate.split(',');  //报表开始时间
                    var enddate = newlist.data[i].EndDate.split(','); //报表结束时间

                    normal_ponit_list[i].startyear = startdate[0];
                    normal_ponit_list[i].startmonth = startdate[1];
                    normal_ponit_list[i].startday = startdate[2];

                    normal_ponit_list[i].endyear = enddate[0];
                    normal_ponit_list[i].endmonth = enddate[1];
                    normal_ponit_list[i].endday = enddate[2];

                    normal_ponit_list[i].pageno = newlist.data[i].Pageno; //报表页号
                    normal_ponit_list[i].sorcetype = newlist.data[i].SorceTypeName; //报表类型

                    normal_ponit_list[i].attr('id', i);

                    if (normal_ponit_list[i].pageno == selectedPageno) {
                        normal_ponit_list[i].addClass('selected');
                        selected = normal_ponit_list[i];
                        backToSelected();
                    }

                    if (monthbegin != normal_ponit_list[i].startmonth) {  //生成月份顶部标签
                        monthbegin = normal_ponit_list[i].startmonth;
                        if (normal_ponit_list[i].startmonth.length > 1) {  //根据月份位数调整标签宽度
                            normal_ponit_list[i].prepend(new top_tip({ text: normal_ponit_list[i].endmonth + '月', width: 30, left: 3 }));
                        } else {
                            normal_ponit_list[i].prepend(new top_tip({ text: normal_ponit_list[i].endmonth + '月', width: 25 }));
                        }
                    }

                }

            }
        }
    } else {
        return false;
    }
}