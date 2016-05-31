var ruler1;          //标尺
var mapInitialized = false;  //地图是否初始化

var showmap = function(sidetype,cyctype, year,mapType) {//创建某一年份的标尺(灾害类型（洪涝或抗旱）、时段类型、年份、地图类型)
    $.ajax({
        type: 'post',
        data: { sidetype: sidetype, year: year, cyctype: cyctype },
        url: '/Statistics/GetScaleData',
        success: function(scaleData) {
            if (scaleData.length > 0) {//当年对应时段类型所有报表数据时间
                var startcount;         //标尺数据显示起始指针
                var _interval;          //计时器事件
                var getdata = function() {     //获得起始指针开始的指定数量的标尺数据       （指针为显示区间的开始，即左端，区间为12位报表数据，指针后12位即为区间的末端）
                    var usedata = new Array;
                    if (scaleData.length <= 12) {  //如果当年所有数据小于12条则不现实标尺的前后箭头
                        usedata = scaleData;      //实际显示的数据就等于当年所有数据
                        $('.right_pointer').hide();
                        $('.left_pointer').hide();
                    } else {
                        var count = 0;         //计数器归零
                        if (0 < startcount && startcount < scaleData.length - 12) {//如果标尺指针大于0且指针后12为数据还没有到大最后一位数据，表示当前显示的数据区间前后都还有数据
                            $('.right_pointer').show(); //显示前箭头
                            $('.left_pointer').show(); //显示后箭头
                        } else {
                            if (startcount == 0) {        //如果标尺指针到了最前面
                                $('.left_pointer').hide();
                                $('.right_pointer').show();
                            }
                            if (startcount == scaleData.length - 12) {  //如果区间的末端到了数据的末端
                                $('.right_pointer').hide();
                                $('.left_pointer').show();
                            }
                            if (_interval) { clearInterval(_interval); }
                        }


                        for (var i = startcount; i < startcount + 12; i++) {
                            usedata[count] = scaleData[i];          //从所有数据中提取指定区间的数据
                            count++;
                        }
                    }
                    return usedata
                }

                ruler1 = false;
                $('#ruler').empty();

                startcount = scaleData.length - 12      //指针初始值为数据总数的前12位

                ruler1 = new ruler({    //初始化标尺实例
                    renderTo: 'ruler',
                    data: getdata(startcount),   //标尺数据为上面函数返回的区间数据
                    event: function() {           //此事件是标尺中每个标点的点击事件
                        refreshMap(ruler1.getSelected().pageno, $('.listli.selected').attr('type'),getMapType());  //刷新地图
                    }
                });

                getdata(startcount);

                var _next = function() {      //标尺数据区间在当年全部数据中向左移
                    startcount--;
                    ruler1.SetPointList({
                        data: getdata(startcount)
                    })
                }

                var _per = function() {       //标尺数据区间在当年全部数据中向右移
                    startcount++;
                    ruler1.SetPointList({
                        data: getdata(startcount)
                    })
                }


                $('.left_pointer').click(function() { _next(); })
                $('.right_pointer').click(function() { _per(); })

                $('.left_pointer').mousedown(function() {
                    _interval = setInterval(function() { _next(); }, 200);       //增加鼠标延时连续触发事件
                });
                $('.right_pointer').mousedown(function() {
                    _interval = setInterval(function() { _per(); }, 200);
                });
                $('.left_pointer').mouseup(function() {
                    clearInterval(_interval);
                });
                $('.right_pointer').mouseup(function() {
                    clearInterval(_interval);
                });
                //ruler1.getLast().click();
                if (mapInitialized == false) {
                    initializingMap(ruler1.getSelected().pageno, $('.listli.selected').attr('type'),mapType);  //初始化地图
                    mapInitialized = true;
                } else {
                    refreshMap(ruler1.getSelected().pageno, $('.listli.selected').attr('type'),mapType);  //刷新地图
                }


            } else {
                ruler1 = false;
                $('#ruler').empty();
                if (mapInitialized == false) {
                    initializingMap("", $('.listli.selected').attr('type'),mapType);  //初始化地图
                    mapInitialized = true;
                } else {
                    refreshMap("", $('.listli.selected').attr('type'),mapType);  //刷新地图
                }
            }
        },
        error: function() {
            ruler1 = false;
            $('#ruler').empty();
            alert('连接数据库超时');

        }
    })
}