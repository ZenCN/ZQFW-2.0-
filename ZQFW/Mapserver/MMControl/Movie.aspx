<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Movie.aspx.cs" Inherits="MoiveControl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>灾情视频</title>
    <script src="../../JS/Jquery/jquery-1.7.2.min.js" type="text/javascript"></script>
    <link href="../flv/CuPlayer/Images/common.css" rel="stylesheet" type="text/css" />
    <script src="../flv/CuPlayer/Images/swfobject.js" type="text/javascript"></script>
    <script type="text/javascript">
        var videoPath = "http://localhost/flv/8.6下午(刚刚)长沙突降暴雨后我家门口的景象.flv"
    function startup( )
    {
//        window.onresize = function() {
//            changeplayerBoxSize();
//        }

//        function changeplayerBoxSize() {
//            $("#object1").width($("#playerBox").width())
//            $("#object1").height($("#playerBox").height())
//            $("#embed1").width($("#playerBox").width())
//            $("#embed1").height($("#playerBox").height())
        //        }
        //text-align:center;
    }
    </script>
</head>
<body id="body1" style=" margin:0px; padding:0px; border:0px; " onload="startup()" > 
    <form id="form1" runat="server">
        <div class="list" style=" float:left ">
    <ul class="mylist" style="height:400px;width:200px;overflow-x:auto;overflow-y:auto; margin-top:0px " >
      <li><a href="#"  onclick="changeStream(0);">8.6下午(刚刚)长沙突降暴雨</a></li>
      <li><a href="#"  onclick="changeStream(1);">9.6下午(刚刚)长沙突降暴雨</a></li>
	  <li><a href="#"  onclick="changeStream(2);">10.6下午(刚刚)长沙突降暴雨</a></li>
	  <li><a href="#"  onclick="changeStream(3);">11.6下午(刚刚)长沙突降暴雨</a></li>
	  <li><a href="#"  onclick="changeStream(4);">12.6下午(刚刚)长沙突降暴雨</a></li>
    </ul>
  </div>
    <div style="right:0;top:0; position:absolute; overflow:hidden; float:right " id = "CuPlayer">
<%--    <script type="text/javascript">
        var flvplayer = "../flv/flvplayer.swf"
        var swf_width =600;// <% = width %>
        var swf_height =400;// <% = height %>
        var texts = '测试视频'
        var files = "http://192.168.1.92/flv/8.6下午(刚刚)长沙突降暴雨后我家门口的景象.flv" // "<% = videoPath %>"
        document.write('<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" ' +
                  'codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" width="' + swf_width + '" height="' + swf_height + '">');
        document.write('<param name="movie" value=' + flvplayer + '><param name="quality" value="high">');
        document.write('<param name="menu" value="false"><param name="allowFullScreen" value="true" />');
        document.write('<param name="FlashVars" value="vcastr_file=' + files + '&vcastr_title=' + texts + '">');
        document.write('<embed src=' + flvplayer + ' allowFullScreen="true" FlashVars="vcastr_file=' +
        files + '&vcastr_title=' + texts + '" menu="false" quality="high" width="' + swf_width + '" height="' + swf_height +
        '" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />');
        document.write('</object>');
    </script>--%>
    
<%--        <object id="object1" name="object1" width=555 height=240 classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" >
        <param name="movie" value= "../flv/flvplayer.swf" />
        <param name="quality" value="high" />
        <param name="allowFullScreen" value="true" />
        <param name="FlashVars" value="vcastr_file=http://192.168.1.92/flv/8.6下午(刚刚)长沙突降暴雨后我家门口的景象.flv&LogoText=www.zizo.com.cn&BufferTime=1" />
        <embed id="embed1" name="embed1" width=555 height=240 src="../flv/flvplayer.swf" allowfullscreen="true" 
        flashvars="vcastr_file=http://192.168.1.92/flv/8.6下午(刚刚)长沙突降暴雨后我家门口的景象.flv&LogoText=www.zizo.com.cn" quality="high" 
        pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash" ></embed>
        </object>--%> 
  
<script type="text/javascript">
<!--
    //酷播迷你：官方连播代码示例//
    var CuPlayerList = "http://192.168.1.92/video/8.6下午突降暴雨.flv" +
    "|http://192.168.1.92/video/湖南新化洪水.mp4" +
    "|http://192.168.1.92/video/拍客  湖南永州特大洪水记实.flv" +
    "|http://192.168.1.92/video/2010  湖南—沩江  洪水.flv" +
    "|http://192.168.1.92/video/2006年湖南7.15洪水.flv";
    var sp = CuPlayerList.split("|")
    var num = sp.length;
    var video_index = 0;
    function getNext(pars) {
        if (video_index < num - 1) {
            video_index++;
            so.addVariable("CuPlayerFile", sp[video_index]);
            so.write("CuPlayer");
        }
        else {
            video_index = 0;
            so.addVariable("CuPlayerFile", sp[video_index]);
            so.write("CuPlayer");
        }
    }
    function changeStream(CuPlayerFile) {
        so.addVariable("CuPlayerFile", sp[CuPlayerFile]);
        so.write("CuPlayer");
    }

    CuPlayerFile = sp[video_index];
    var width = "500";
    var height = "400";
    var so = new SWFObject("../flv/CuPlayer/CuPlayerMiniV3_Black_S.swf", "CuPlayer", width, height, "9", "#000000");
    so.addParam("allowfullscreen", "true");
    so.addParam("allowscriptaccess", "always");
    so.addParam("wmode", "opaque");
    so.addParam("quality", "high");
    so.addParam("salign", "lt");
    so.addVariable("CuPlayerFile", CuPlayerFile);
    so.addVariable("CuPlayerImage", "../flv/CuPlayer/Images/flashChangfa2.jpg");
    so.addVariable("CuPlayerLogo", "../flv/CuPlayer/Images/logo.png");
    so.addVariable("CuPlayerShowImage", "true");
    so.addVariable("CuPlayerWidth", width);
    so.addVariable("CuPlayerHeight", height);
    so.addVariable("CuPlayerAutoPlay", "true");
    so.addVariable("CuPlayerAutoRepeat", "false");
    so.addVariable("CuPlayerShowControl", "true");
    so.addVariable("CuPlayerAutoHideControl", "false");
    so.addVariable("CuPlayerAutoHideTime", "6");
    so.addVariable("CuPlayerVolume", "80");
    so.addVariable("CuPlayerGetNext", "true");
    so.write("CuPlayer");	

//-->
</script>
  <!--酷播迷你 CuPlayerMiniV2.0 代码结束-->
        
            </div>
    </form>
</body>
</html>
