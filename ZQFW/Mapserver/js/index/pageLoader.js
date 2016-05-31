$(function() {
    // remove layerX and layerY
    var all = $.event.props, 
    len = all.length, 
    res = [];
    while (len--) {
        var el = all[len];
        if (el != 'layerX' && el != 'layerY')
            res.push(el);
    }
    $.event.props = res;
}());


var pageLoader = function(property) {
    if (property.Items) {
        var Itemscount = 0;
        var loderItemsHtml = {};
        var preCalcLoTs = [];
        var normWoH = [];
        var _html = function() {
            Itemscount = property.Items.length;
            for (var i = 0; i < Itemscount; i++) {
                preCalcLoTs[i] = [];
                if (i == 0) {
                    loderItemsHtml.content_item = '<div id="left" class="content_item" style="width:' + (property.ScreenWidth / Itemscount - 2) + 'px;"><div class="img_con" style="width:' + property.ScreenWidth / Itemscount + 'px;"><img src="' + property.Items[i].img + '"' + 
                    ' style=" margin-left:-'+property.Items[i].ImgWidth/2+'px" /></div><h1><img src="' + property.Text + '" style=" margin-top:0px;"/></h1><div class="loader">' + 
                    '<img src="image/index/loading.gif" /></div></div>';
                    loderItemsHtml.screen = '<div id="leftscreen"></div>';
                    for (var j = 1; j < Itemscount - 1; j++) {
                        if (i == j) {
                            preCalcLoTs[i][j] = j * 0 + (j * 0);
                        } else {
                            preCalcLoTs[i][j] = (j <= i ? (j * 0) : (j - 1) * 0 + property.ScreenWidth) + (j * 0);
                        }
                        ;
                    
                    }
                    ;
                    normWoH[i] = property.ScreenWidth / Itemscount - 2;
                
                } else if (i > 0 && i < Itemscount - 1 && i < Itemscount - 2) {
                    loderItemsHtml.content_item += '<div id="mid' + (i - 1) + '" class="content_item mid" style="width:' + (property.ScreenWidth / Itemscount - 3) + 'px;left:' + (i * property.ScreenWidth / Itemscount) + 'px;"><div class="img_con" style="width:' + property.ScreenWidth / Itemscount + 'px;"><img src="' + property.Items[i].img + '"' + 
                    ' style=" margin-left:-'+property.Items[i].ImgWidth/2+'px" /></div><h1><img src="' + property.Text + '" style=" margin-top:' + -42 * i + 'px;"/></h1><div class="loader">' + 
                    '<img src="image/index/loading.gif" /></div></div><div id="midline' + (i - 1) + '" class="midline" style="left:' + (i * property.ScreenWidth / Itemscount + property.ScreenWidth / Itemscount - 1) + 'px;"></div>';
                    loderItemsHtml.screen += '<div id="mid' + (i - 1) + 'screen"></div>';
                    for (var j = 1; j < Itemscount - 1; j++) {
                        if (i == j) {
                            preCalcLoTs[i][j] = j * 0 + (j * 0);
                        } else {
                            preCalcLoTs[i][j] = (j <= i ? (j * 0) : (j - 1) * 0 + property.ScreenWidth) + (j * 0);
                        }
                        ;
                    
                    }
                    ;
                    normWoH[i] = property.ScreenWidth / Itemscount - 3;
                } else if (i > 0 && i < Itemscount - 1) {
                    loderItemsHtml.content_item += '<div id="mid' + (i - 1) + '" class="content_item mid" style="width:' + (property.ScreenWidth / Itemscount - 2) + 'px;left:' + (i * property.ScreenWidth / Itemscount) + 'px;"><div class="img_con" style="width:' + property.ScreenWidth / Itemscount + 'px;"><img src="' + property.Items[i].img + '"' + 
                    ' style=" margin-left:-'+property.Items[i].ImgWidth/2+'px" /></div><h1><img src="' + property.Text + '" style=" margin-top:' + -42 * i + 'px;"/></h1><div class="loader">' + 
                    '<img src="image/index/loading.gif" /></div></div>';
                    loderItemsHtml.screen += '<div id="mid' + (i - 1) + 'screen"></div>';
                    for (var j = 1; j < Itemscount - 1; j++) {
                        if (i == j) {
                            preCalcLoTs[i][j] = j * 0 + (j * 0);
                        } else {
                            preCalcLoTs[i][j] = (j <= i ? (j * 0) : (j - 1) * 0 + property.ScreenWidth) + (j * 0);
                        }
                        ;
                    
                    }
                    ;
                    normWoH[i] = property.ScreenWidth / Itemscount - 2;
                } else if (i == Itemscount - 1) {
                    loderItemsHtml.content_item += '<div id="right" class="content_item" style="width:' + (property.ScreenWidth / Itemscount - 2) + 'px;"><div class="img_con" style="width:' + property.ScreenWidth / Itemscount + 'px;"><img src="' + property.Items[i].img + '"' + 
                    ' style=" margin-left:-'+property.Items[i].ImgWidth/2+'px" /></div><h1><img src="' + property.Text + '" style=" margin-top:' + -42 * i + 'px;"/></h1><div class="loader">' + 
                    '<img src="image/index/loading.gif" /></div></div>';
                    loderItemsHtml.screen += '<div id="rightscreen"></div>';
                    for (var j = 1; j < Itemscount - 1; j++) {
                        if (i == j) {
                            preCalcLoTs[i][j] = j * 0 + (j * 0);
                        } else {
                            preCalcLoTs[i][j] = (j <= i ? (j * 0) : (j - 1) * 0 + property.ScreenWidth) + (j * 0);
                        }
                        ;
                    
                    }
                    ;
                    normWoH[i] = property.ScreenWidth / Itemscount - 2
                }
                ;                
            
            }
            ;
        };
        var _randerTo = function() {
            $('#' + property.rander.content).append(loderItemsHtml.content_item);
            $('#' + property.rander.screen).append(loderItemsHtml.screen);
        };
        
        var setevent = function() {
            var kwicks = $(".content_item");
            kwicks.each(function(i) {
                var kwick = $(this);
                kwick.bind('click', function() {
                    $('#main_frame').attr('src',property.Items[i].url);
                    var prevLoTs = [];
                    var prevWoHs = [];
                    if (!$(kwick).hasClass('selected')) {
                        kwicks.stop().removeClass('selected');
                        kwick.children('.loader').show();
                        for (j = 0; j < kwicks.size(); j++) {
                            prevWoHs[j] = kwicks.eq(j).css('width').replace(/px/, '');
                            prevLoTs[j] = kwicks.eq(j).css('left').replace(/px/, '');
                        }
                        ;
                        var maxDif = property.ScreenWidth - prevWoHs[i];
                        var prevWoHsMaxDifRatio = prevWoHs[i] / maxDif;
                        kwick.addClass('selected').animate({width: property.ScreenWidth + 'px'}, {
                            step: function(now) {
                                // calculate animation completeness as percentage
                                var percentage = maxDif != 0 ? now / maxDif - prevWoHsMaxDifRatio : 1;
                                // adjsut other elements based on percentage
                                kwicks.each(function(j) {
                                    if (j != i) {
                                        kwicks.eq(j).css('width', prevWoHs[j] - ((prevWoHs[j] - 0) * percentage) + 'px');
                                    }
                                    if (j > 0 && j < kwicks.size() - 1) { // if not the first or last kwick
                                        kwicks.eq(j).css('left', prevLoTs[j] - ((prevLoTs[j] - preCalcLoTs[i][j]) * percentage) + 'px');
                                    }
                                    if (j > 0 && j < kwicks.size() - 2) {
                                        $('#midline' + (j - 1)).css('display', 'none');
                                    }
                                });
                            },
                            complete: function() {
                                $('#go_back').fadeIn('slow'); 
                                kwick.children('.loader').hide();
                                $('#content').hide();   
                                $('#screen').fadeIn('slow');
                            }
                        });
                    }
                    ;
                });
            });
        };
        
        var _Goback = function() {
            var kwicks = $(".content_item");
            var prevWoHs = [];
            var prevLoTs = [];
            kwicks.removeClass('selected').stop();
            for (i = 0; i < kwicks.size(); i++) {
                prevWoHs[i] = kwicks.eq(i).css('width').replace(/px/, '');
                prevLoTs[i] = kwicks.eq(i).css('left').replace(/px/, '');
            }
            var aniObj = {};
            var normDif = normWoH[0] - prevWoHs[0];
            kwicks.eq(0).animate({width:normWoH[0]+'px'}, {
                step: function(now) {
                    var percentage = normDif != 0 ? (now - prevWoHs[0]) / normDif : 1;
                    for (i = 1; i < kwicks.size(); i++) {
                        kwicks.eq(i).css('width', prevWoHs[i] - ((prevWoHs[i] - normWoH[i]) * percentage) + 'px');
                        if (i < kwicks.size() - 1) {
                            kwicks.eq(i).css('left', prevLoTs[i] - ((prevLoTs[i] - ((i * (property.ScreenWidth / kwicks.size())) + (i * 0))) * percentage) + 'px');
                        }

                    }
                },
                complete:function(){
                    for (i = 1; i < kwicks.size(); i++) {
                        if (i > 0 && i < kwicks.size() - 2) {
                            $('#midline' + (i - 1)).css('display', 'block');
                        };
                    };
                }
            });
        };
        
        return {
            init: function() {
                loderItems = _html();
                _randerTo();
                setevent();
                
            },
            Goback: function() {
                $('#screen').hide();
                $('#go_back').hide();
                $('#content').fadeIn('slow');
                $('#main_frame').attr('src','');
                _Goback();
            },
            open:function(){
                $(".content_item").eq(1).click();
            }
        };
    
    
    } else {
        return undefined;
    }
}
