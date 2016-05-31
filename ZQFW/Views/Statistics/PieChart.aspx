<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="PieChart.aspx.cs" Inherits="ZQFW.Views.PieChart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="webkit" name="renderer"/>
    <title>无标题页</title>
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/Plugins/highchart/highcharts.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/Plugins/highchart/modules/exporting.js" type="text/javascript"></script>
    <script src="../../Mapserver/mapParams/mapParams.js" type="text/javascript"></script>
</head>
<body style=" margin:0; padding:0;">
    <div id="chartContiner" style=" width:698px; height:440px;"></div>
    <script src="../../Scripts/Statistics/Assessment/evalChart.js" type="text/javascript"></script>
    <script>
        $(document).ready(function() {
            SetChartValues(window.parent.window.parent.currentPageNO, 'SWRK');
        });
    </script>
</body>
</html>
