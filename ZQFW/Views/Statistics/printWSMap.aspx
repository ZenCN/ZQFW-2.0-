<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title>打印</title>
    <link href="../../MapServer/css/mapControl/button.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/mapControl/mapinfo.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/mapControl/mapControl.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/Statistics/WaterStorage/legend.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../MapServer/js/public.js" type="text/javascript"></script>
     <script type="text/javascript">
         //加载并显示地图
         function showMap(title, imgUrl, width, height, legend) {
             $("#title p").html(title);
             $("#content").css({ width: width + 2, height: height + 32 });   //62为地图标题高度
             $("#mapContainer").css({ width: width, height: height });
             $("#map").attr("src", imgUrl)
             $("#mapContainer").append(legend)
         }
         
    </script>
</head>
<body style=" background-color:White;margin:0px"> 
    <form id="form1" runat="server">
        <div id="content" style="position:absolute;background-color:White;">
            <div id="title" style=" background-color:White;text-align: center;margin:10px 0px 0px 0px;">
            <p style=" font-size:20px;color: #000;font-family:Lucida Grande, Lucida Sans Unicode, Verdana, Arial, Helvetica, sans-serif;"></p>
            </div>
            <div id="mapContainer" style="position:absolute;margin:0px;top:30px;overflow: hidden;">  
                 <input id="map" class="map" style="margin:0" type="image" src="" border="0" />
            </div>
        </div>
    </form>
</body>
</html>
