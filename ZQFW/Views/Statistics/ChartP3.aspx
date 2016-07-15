<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChartP3.aspx.cs" Inherits="ZQFW.Views.ChartP3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title>导出三维饼图</title>
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../MapServer/js/map/chart/chart.js?t=0620" type="text/javascript"></script>
<%--        <script src="../../MapServer/js/cookie.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        //        $(function() {
        //            var title = getCookie("title");
        //            var subtitle = getCookie("subtitle");
        //            var strNames = getCookie("names");
        //            var strDatas = getCookie("datas");
        //            var dataUnit = getCookie("dataUnit");
        //            var width = getCookie("width");

        //            showPieChart(title, subtitle, strNames, strDatas, dataUnit, width); //生成并显示三维饼图
        //        })

//        生成三维饼图
        function showPieChart(title, subtitle, strNames, strDatas, dataUnit, width) {
            var names = strNames.split(",");
            var datas = strDatas.split(",");
            $("#floatbox").width(width);
            var property = {
                renderTo: 'floatbox',
                title: title,
                subtitle: subtitle,
                names: names,
                datas: datas,
                dataUnit: dataUnit,
                showTool: false
            };
            var chartp3_1 = new chartp3(property);
        }
    </script>
</head>
<body style=" background-color:White;margin:0px">
<form id="Form1" runat="server">
    <div id="floatbox" style=" background-color:White;"></div>
</form>
</body>
</html>

