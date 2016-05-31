<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title>无标题页</title>    
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script language="javascript">
        $(function () {
            $("#btnExportP3").click(function () {
                $("#Form1").submit();
            })
        })
        </script>
</head>
<body style=" background-color:White;">
<form id="Form1" runat="server" action="newP3Thread">
   <%-- <input type="submit" id="btnExportP3" onclick='this.form.action="<%=Url.Action("newThread") %>";'/>--%>
    <input type="submit" id="btnExportP3" style="display:none"/>
    <input id="hidTitle" name="hidTitle" type="hidden" runat="server" value=""/>
    <input id="hidSubtitle" name="hidSubtitle" type="hidden"  runat="server" value=""/>
    <input id="hidNames" name="hidNames" type="hidden"  runat="server" value=""/>
    <input id="hidDatas" name="hidDatas" type="hidden"  runat="server" value=""/>
    <input id="hidDataUnit" name="hidDataUnit" type="hidden"  runat="server" value=""/>
    <input id="hidImgName" name="hidImgName" type="hidden"  runat="server" value=""/>
    <input id="hidWidth" name="hidWidth" type="hidden"  runat="server" value=""/>
    <div id="floatbox" style=" background-color:White;"></div>
      <% if (Context.Request["type"] == "table")
                                                                       {
             Response.Write("<script src='../../MapServer/js/map/chart/TableTitle_JS.js' type='text/javascript'></script>" +
                 "<script src='../../MapServer/js/map/chart/TableDate.js' type='text/javascript'></script>");
       }%>
    <% 
    if (Context.Request["type"] == "bar") {
        Response.Write("<script >clickchart(0,$('.listli.selected').attr('type'),ruler1.getSelected());</script>");
    }
    else if (Context.Request["type"] == "pie")                                          
    {
        Response.Write("<script >clickchart(1,$('.listli.selected').attr('type'),ruler1.getSelected());</script>");
    }
    else if (Context.Request["type"] == "p3")
    {
       
        Response.Write("<script>clickchart(2,$('.listli.selected').attr('type'),ruler1.getSelected());</script>");
    }
    else if (Context.Request["type"] == "table")
    {
        Response.Write("<script >newBBTable(ruler1.getSelected().pageno,window.frames[mapParams.mapContainerName].window.getMapLevel(),window.frames[mapParams.mapContainerName].window.getRegionCode());</script>");
    }%> 
</form>
</body>
</html>
