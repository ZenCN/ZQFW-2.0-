var evalListItemsbase = function (settings) {
    var title = settings.title;
    var subtitle = settings.subtitle;
    var level = settings.level;
    var index = settings.index;
    var pageno = settings.pageno;

    var levelTosrc = function (levelpar) {
        switch (levelpar) {
            case 1:
                return '../../CSS/Img/Statistics/Assessment/1.png';
                break;

            case 2:
                return '../../CSS/Img/Statistics/Assessment/2.png';
                break;

            case 3:
                return '../../CSS/Img/Statistics/Assessment/3.png';
                break;

            case 4:
                return '../../CSS/Img/Statistics/Assessment/4.png';
                break;

            default:
                return '../../CSS/Img/Statistics/Assessment/1.png';
                break;
        }
    };

    var icoObject = $('<img src= "' + levelTosrc(level) + '"/>');
    var titleObject = $('<div class="eval_info"><span class="eval_title" title="' + title + '">' + title + '</span>' +
							'<span class="eval_subtitle" title="' + subtitle + '">' + subtitle + '</span></div>');
    var openButtonObject = $('<span class="eval_operation"><a class="open_" id="' + index + '" pageno="' + pageno + '" >查看</a></span>');
    var htmlObject = $('<li></li>').append(icoObject).append(titleObject.append(openButtonObject));

    openButtonObject.bind('click', function () {
        currentPageNO = $(this.firstChild).attr('pageno'); //获取当前页号
        evaluationGrade = level; //获取灾情评估等级
        var src = settings.type == 'single' ? '/Statistics/EvaluateTable' : '/Statistics/EvaluateTableOfYear';
        $('#displayframe').attr('src', src);
    });

    return {
        getIndex: function () {
            return index;
        },
        getPageno: function () {
            return pageno;
        },
        getOpenButton: function () {
            return openButtonObject;
        },
        getHtml: function () {
            return htmlObject;
        }
    };
}

	var evalListbase = function (settings) {
	    var type = settings.type;

	    var listcount = 0;
	    var currntpage = 1;
	    var totalcount = 0;
	    var animating = false;
	    var totheend = false;

	    //	    var title = type == 'single' ? '单场灾情评估' : '年度灾情评估';
	    //	    var titleObject = $('<label class="eval_box_title">' + title + '</label>');
	    //var listTitle = $('<label class="eval_box_title">评估数据</label>');
	    var lists = { listsArray: [], lists: $('<ul></ul>') };
	    var listsObject = $('<div class="eval_list"></div>').css('height', settings.showcount * 46 + 'px').append(lists.lists);
	    var upButton = $('<span class="page_up"></span>').addClass('unable');
	    var downButton = $('<span class="page_down"></span>');
	    var buttonsObject = $('<div class="eval_box_buttons"></div>').append(upButton).append(downButton);
	    var htmlObject = $('<div class="evalinfo_box"></div>')
	    var dateObject = "";
	    //	    var queryButton = "";
	    if (type == "single") {
	        //var queryTitle = $('<label class="eval_box_title">时间选择</label>');
	        var startDate = $('<p style="margin:8px auto 0px auto;"></p>').append('<label for="startDate">起始日期:</label>')
            .append('<input id="startDate" name="startDate" style="width: 100px;" class="Wdate" />');
	        var endDate = $('<p style="margin:5px auto 0px auto"></p>').append('<label for="endDate">结束日期:</label>')
            .append('<input id="endDate" name="endDate" style="width: 100px;" class="Wdate" />');
	        var queryButton = $('<input type="button" value="查询" style="margin:5px"/>')
	        dateObject = $('<div class="eval_date"></div>')
            //.append(queryTitle)
            .append(startDate)
            .append(endDate)
            .append(queryButton);
	        dateObject.find(".Wdate").click(function () {
	            WdatePicker();
	        })
	    }
	    else {
	        htmlObject.show();
	        //	        var startDate = $('<p></p>').append('<label for="startYear">起始年份:</label>')
	        //	                .append('<select id="startYear" name="startYear"></select>');
	        //	        var endDate = $('<p></p>').append('<label for="endYear">结束年份:</label>')
	        //	                .append('<select id="endYear" name="endYear"></select>');
	        //	        dateObject = $('<div class="eval_date"></div>').append(startDate).append(endDate);
	        //	        getYears(dateObject.find("#startYear"), dateObject.find("#endYear"));
	    }

	    htmlObject.append(dateObject) //.append(titleObject)
        //.append(listTitle)
        .append(listsObject)
        .append(buttonsObject);

	    settings.container.append(htmlObject);

	    var setbuttonstate = function () {
	        downButton.removeClass('unable');
	        upButton.removeClass('unable');
	        totheend = false;

	        if (totalcount == 0) {
	            buttonsObject.hide();
	        }
	        else {
	            buttonsObject.show();
	        }

	        if (currntpage * settings.showcount >= totalcount) {
	            downButton.addClass('unable');
	            totheend = true;
	        }

	        if (currntpage == 1) {
	            upButton.addClass('unable');
	        }
	    }

	    return {
	        show: function () {  //显示元素
	            return htmlObject.show();
	        },
	        hide: function () {  //隐藏元素
	            return htmlObject.hide();
	        },
	        getStartDate: function () {
	            return dateObject.find("#startDate").val();
	        },
	        getEndDate: function () {
	            return dateObject.find("#endDate").val();
	        },
	        getQueryButton: function () {  //获取查询按钮
	            return queryButton;
	        },
	        getType: function () {
	            return type;
	        },
	        getCount: function () {
	            return listcount;
	        },
	        getEnd: function () {
	            return isEnd;
	        },
	        getPageUp: function () {
	            return upButton;
	        },
	        getPageDown: function () {
	            return downButton;
	        },
	        getLists: function () {
	            return lists;
	        },
	        getIsAnimating: function () {
	            return animating;
	        },
	        Add: function (addsetting) {  //添加评估数据
	            lists.listsArray[listcount] = new evalListItemsbase({
	                title: addsetting.title,
	                subtitle: addsetting.subtitle,
	                level: addsetting.level,
	                index: listcount,
	                pageno: addsetting.pageno,
	                type: type
	            });
	            lists.lists.append(lists.listsArray[listcount].getHtml());
	            listcount++;
	        },
	        EmptyLists: function () {
	            lists.lists.html("<div style='margin: 20px;text-align: center;'>没有评估数据！</div>");
	        },
	        ClearLists: function () {  //清空评估数据
	            lists.listsArray = [];
	            lists.lists.html("");
	            lists.lists.css("margin-top", 0);
	            totalcount = 0;
	            listcount = 0;
	            currntpage = 1;
	        },
	        findListItemByIndex: function (indexpar) {
	            return lists.listsArray[indexpar];
	        },
	        changeTotal: function (total) {
	            totalcount = total;
	        },
	        setButtonState: function () {
	            setbuttonstate();
	        },
	        up: function () {
	            if (currntpage > 1) {
	                var nowtop = parseInt(lists.lists.css('margin-top'));
	                var totop = settings.showcount * 46 + nowtop;
	                animating = true;
	                lists.lists.animate({ marginTop: totop + "px" }, { complete: function () { animating = false; } });
	                currntpage--;
	                setbuttonstate();
	                return true;

	            } else {
	                setbuttonstate();
	                return false;
	            }
	        },
	        down: function () {
	            if ((currntpage) * settings.showcount < totalcount) {
	                var nowtop = parseInt(lists.lists.css('margin-top'));
	                var totop = settings.showcount * (-46) + nowtop;
	                animating = true;
	                lists.lists.animate({ marginTop: totop + "px" }, { complete: function () { animating = false; } });
	                currntpage++;
	                setbuttonstate();
	                if (!totheend && (currntpage + 2) * settings.showcount > listcount) { //如果后面还有数据且往后2页的数据还未查询
	                    return true;
	                } else {
	                    return false;
	                }
	            } else {
	                setbuttonstate();
	                return false;
	            }
	        }
	    }
	}

	var getEvalData = function (settings) {
	    var listcount = settings.listcount;   //当前已查出的数据条数
	    var percount = settings.percount;    //此次查询数据条数
	    if (settings.object.getType() == 'single') {
	        var startdate = settings.object.getStartDate();
	        var endDate = settings.object.getEndDate();
            var params = { evalType: 0, currentCount: listcount, queryCount: percount, startDate: startdate, endDate: endDate };
	    }
	    else {
	        var params = { evalType: 1, currentCount: listcount, queryCount: percount, startDate: "", endDate: "" };
	    }

	    $.ajax({
	        type: 'post',
	        url: '/Statistics/GetDisasterAssessmentTitle',
	        data: params,
	        success: function (jsonData) {
	            if ($.isEmptyObject(jsonData)) {
	                settings.object.setButtonState();
	                settings.object.EmptyLists();
	            } else {
	                var evalTitle = jsonData.items;   //评估项目标题
	                for (var i = 0; i < jsonData.items.length; i++) {
	                    settings.object.Add({
	                        title: evalTitle[i].title,
	                        subtitle: evalTitle[i].subtitle,
	                        level: evalTitle[i].itemlevel,
	                        index: listcount + i + 1,
	                        pageno: evalTitle[i].pageno
	                    });
	                }
	                settings.object.changeTotal(jsonData.totalcount);
	                settings.object.setButtonState();
	                //在第一次加载时显示第一条年度灾情评估数据详情
	                if (settings.object.getType() == 'year' && settings.object.getCount() >= 1 && firstLoad == true) {
	                    firstLoad = false;
	                    settings.object.findListItemByIndex(0).getOpenButton().click();
	                };
	            }
	            if (settings.object.getType() == "single") {
	                settings.object.getQueryButton().removeAttr("disabled");
	            }
	        },
	        error: function (e) {
	            if (settings.object.getType() == "single") {
	                settings.object.getQueryButton().removeAttr("disabled");
	            }
	            alert(e);
	        }
	    });
	}

//获取有数据的年份,并填充到下拉选项中
function getYears($startYear,$endYear)
{
    $.ajax({
        type: 'post',
        url: '/Statistics/GetYears',
        success: function(listYear) {
            if (listYear.length == 0) {
                $startYear.html("");
                $endYear.html("");
            }
            else {
                for (var i = 0; i < listYear.length; i++) {
                    var option = "<option value='" + listYear[i] + "'>" + listYear[i] + "</option>";
                    $startYear.append(option);
                    $endYear.append(option);
                }
            }
        },
        error: function() {
            alert('获得年份出错！');
        }
    });
}