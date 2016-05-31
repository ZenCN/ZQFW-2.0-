//$(document).ready(function() {
//    mapType = 0;
//    $mapTypes = $("#divMapType span[title]")
//    $mapTypes.mouseover(function() {
////        var showType = $mapTypes.index(this)
////        if (showType == 0 || (showType == 1 && mapType == 0)) {
////            for (var i = 0; i < $mapTypes.length; i++) {
////                if (i != showType && i != mapType) {
////                    $mapTypes.eq(i).animate({ "right": "62px" })
////                }
////                else {
////                    $mapTypes.eq(i).css("right", "4px")
////                }
////            }
////        }

//        $(this).children().css({ 'border-color': '#83A1FF' });
//        $(this).children().children().first().css({ 'background-color': '#83A1FF', 'opacity': 1 })
//    })

//    $mapTypes.mouseout(function() {
//        $(this).children().css({ 'border-color': 'gray' });
//        $(this).children().children().first().css({ 'background-color': 'gray', 'opacity': 0.5 })
//    })

//    $("#divMapType").mouseenter(function() {
//        $(this).width(110);
//        for (var i = 0; i < $mapTypes.length; i++) {
//            if (i != mapType) {
//                if (i < mapType) {
//                    var right = i * 58;
//                    $mapTypes.eq(i).animate({ "right": right });
//                }
//                else {
//                    var right = (i - 1) * 58;
//                    $mapTypes.eq(i).animate({ "right": right });
//                }
//            }
//        }
//    })

//    $("#divMapType").mouseleave(function() {
//        $(this).width(49);
//        for (var i = 0; i < $mapTypes.length; i++) {
//            $mapTypes.eq(i).animate({ "right": "0px" })
//        }
//    })

//    $mapTypes.click(function() {
//        var omapType = mapType;
//        mapType = $mapTypes.index(this);

//        //        if (mapType > 0) {
//        //            var right = 4 + (mapType - 1) * 58
//        //            $mapTypes.eq(mapType).css({ "right": right });
//        //        }
//        $mapTypes.eq(mapType).hide();
//        $mapTypes.eq(omapType).show();
//        for (var i = 0; i < $mapTypes.length; i++) {
//            if (i != mapType) {
//                if (i < mapType) {
//                    var right = i * 58;
//                    $mapTypes.eq(i).css({ "right": right });
//                }
//                else {
//                    var right = (i - 1) * 58;
//                    $mapTypes.eq(i).css({ "right": right });
//                }
//            }
//        }
//    })
//})