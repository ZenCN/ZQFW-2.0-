<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvaluateTableOfYear.aspx.cs" Inherits="ZQFW.Views.EvaluateTableOfYear" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title></title>
    <link href="../../CSS/Statistics/Assessment/evaluatetable.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
            var sUnitCode = "<%=UnitCode %>"  //使用该系统的行政单位代码
    </script>
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/Plugins/jquery.cookie.js" type="text/javascript"></script>
    <script src="../../Scripts/Statistics/Assessment/evaluatetableofyear.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <img id="loading" src="../../CSS/Img/Statistics/Assessment/loader.gif" style=" position:absolute; left:50%; top:50%; margin:-14px 0 0 -14px; display:none;" />
    <div style="width:740px;display:none; background-color:White;" id="year_eval_table_frame">
    <div id="evaluatetitle">
        <h1 id="t_title">
            灾情评估</h1>
        <p id="t_desc_content">
            没有找到相应的评估数据。</p>
    </div>
    <div id="evaluatesheet">
        <div id="img_warn"></div>
        <table class="evaluatetable" cellpadding="0" cellspacing="0" border="0">
            <thead>
                <tr class="header">
                    <th>
                        指标名称
                    </th>
                    <th>
                        总值
                    </th>
                    <th>
                        灾害等级
                    </th>
                    <th>
                        标注
                    </th>
                </tr>
            </thead>
            <tbody id="t_table_content">
            </tbody>
        </table>
    </div>
    </div>
    <div id="functionbutton">
        <div class="googlebuttons">
            <div class="googlebutton" style="display: none">
                <a class="preview">预览</a></div>
            <div class="googlebutton" style="display: none" id="btn_publish_year">
                <a class="publish" onclick="window.parent.$.fancybox.close();window.parent.PublishYearEval();">
                    发布</a></div>
        </div>
    </div>
    </form>
</body>
</html>
