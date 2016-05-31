<%@ Page AspCompat="true" Language="C#" AutoEventWireup="true" CodeBehind="EvaluateTable.aspx.cs" Inherits="ZQFW.Views.EvaluateTable"%>

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
    <script src="../../Mapserver/js/map/getData.js" type="text/javascript"></script>
    <script src="../../Mapserver/js/map/loadMap.js" type="text/javascript"></script>
    <script src="../../Mapserver/js/public.js" type="text/javascript"></script>
    <script src="../../Mapserver/mapParams/mapParams.js" type="text/javascript"></script>
    <script src="../../Mapserver/js/printMap.js" type="text/javascript"></script>
    <script src="../../Scripts/Statistics/Assessment/evaluatetable.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server" style=" background-color:White;">
    <img id="loading" src="../../CSS/Img/Statistics/Assessment/loader.gif" style=" position:absolute; left:50%; top:50%; margin:-14px 0 0 -14px; display:none;" />
        <div style="width:740px;display:none; background-color:White;"  id="eval_table_frame">
            <div id="evaluatetitle">
              <h1 id="t_title">灾情评估</h1>
              <p id="t_desc_content">没有找到相应的评估数据。</p>
            </div>
            <div id="evaluatesheet">
              <div id="img_warn"></div>
              <table class="evaluatetable" cellpadding="0" cellspacing="0" border="0">
                <thead>
                  <tr class="header">
                    <th>指标名称</th>
                    <th>灾情值</th>
                    <th>灾害等级</th>
                    <th>标注</th>
                  </tr>
                </thead>
                <div  class="aboutPrint" style="position: absolute;display:none;">
<%--                    <asp:Button ID="exportMap"  style=" display:none; height: 24px;margin:0;line-height: 24px; cursor:pointer"
                        runat="server"  onclick="exportMap_Click" Text="导出地图"/>--%>
                    <input id="btnExportMap" type="button" style="height: 24px;margin: 0;line-height: 24px; cursor:pointer" onclick="pirntMap()" value="导出地图" />
                    <%--<input id="hidImgName" name="hidImgName" type="hidden" runat="server" value=""/>
		            <input id="hidImgUrl" name="hidImgUrl" type="hidden"  runat="server" value=""/>
		            <input id="hidLegend" name="hidLegend" type="hidden"  runat="server" value=""/>
		            <input id="hidMapWidth" name="hidMapWidth" type="hidden"  runat="server" value=""/>
		            <input id="hidMapHeight" name="hidMapHeight" type="hidden"  runat="server" value=""/>--%>
                </div>
                <tbody id="t_table_content">
                </tbody>
              </table>
            </div>
            <div id="functionbutton">
                <div class="googlebuttons">
                    <div class="googlebutton" style="display:none"><a class="preview" >预览</a></div>
                    <div class="googlebutton" style="display:none" id="btn_publish"><a class="publish" onclick = "window.parent.$.fancybox.close();window.parent.SaveEvalTable();">发布</a></div>
                </div>
            </div>
        </div>
    </form>
    <form id="Form2" action="newMapThread" Method="post" style="display:none">
        <input type="submit" id="exportMap"  style="display:none" />
        <input id="hidImgName" name="hidImgName" type="hidden" runat="server" value=""/>
	    <input id="hidImgUrl" name="hidImgUrl" type="hidden"  runat="server" value=""/>
	    <input id="hidLegend" name="hidLegend" type="hidden"  runat="server" value=""/>
	    <input id="hidMapWidth" name="hidMapWidth" type="hidden"  runat="server" value=""/>
	    <input id="hidMapHeight" name="hidMapHeight" type="hidden"  runat="server" value=""/>
    </form>
</body>
</html>