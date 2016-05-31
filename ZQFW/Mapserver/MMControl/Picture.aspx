<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Picture.aspx.cs" Inherits="htmlcontronl_Picture" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title></title>
    <script src="../../JS/Jquery/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../JQuery/jquery.easing.1.3.js" type="text/javascript"></script>
    <script src="../js/public.js" type="text/javascript"></script>
    <link href="../css/mapControl/picture.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<script type="text/javascript">
	    var imagesPaths = "http://192.168.1.92/images/1.jpg|http://192.168.1.92/images/2.jpg|http://192.168.1.92/images/3.jpg|http://192.168.1.92/images/4.jpg|" + 
	    "http://192.168.1.92/images/5.jpg|http://192.168.1.92/images/6.jpg|http://192.168.1.92/images/7.jpg|http://192.168.1.92/images/8.jpg|" +
	    "http://192.168.1.92/images/9.jpg|http://192.168.1.92/images/10.jpg|http://192.168.1.92/images/11.jpg|http://192.168.1.92/images/12.jpg";
	var imagesPathArr = imagesPaths.split("|");
	var imagesNameArr = [];
	for (var i = 0; i < imagesPathArr.length; i++) {
	    var imagePath = imagesPathArr[i];
	    var imageName = imagePath.substring(imagePath.lastIndexOf("/") + 1, imagePath.lastIndexOf("."));
	    imagesNameArr.push(imageName);
	}

	document.write('<div id="preloader"><img src="../images/ajax-loader_dark.gif" width="32" height="32" /></div>');
	document.write('<div id="bg"><img src=' + imagesPathArr[0] + ' width="1680" height="1050"' +  //' title=' + imagesNameArr[0] + 
	'alt=' + imagesNameArr[0] + ' id="bgimg" /></div>');
	document.write('<div id="img_title">' + imagesNameArr[0] + '</div>');
    document.write('<div id="outer_container">');
    document.write('<div id="thumbScroller">');
    document.write('<div class="container">');
    for (var i = 0; i < imagesPathArr.length; i++) 
	{
	    document.write('<div class="content">' +
        	'<div><a href=' + imagesPathArr[i] + '><img src=' + imagesPathArr[i] + ' title=' + imagesNameArr[i] +
        	' alt=' + imagesNameArr[i] + ' class="thumb" onload="ResizeImg(this,500,100)"/></a></div></div>');
	}
	document.write('</div></div></div>');

$outer_container=$("#outer_container");
$thumbScroller=$("#thumbScroller");
$thumbScroller_container=$("#thumbScroller .container");
$thumbScroller_content=$("#thumbScroller .content");
$thumbScroller_thumb=$("#thumbScroller .thumb");
$preloader=$("#preloader");
$toolbar=$("#toolbar");
$toolbar_a=$("#toolbar a");
$bgimg="#bgimg";

$(window).load(function() {
	//thumbnail scroller
	sliderLeft=$thumbScroller_container.position().left;
	padding=$outer_container.css("paddingRight").replace("px", "");
	sliderWidth=$(window).width()-padding;
	$thumbScroller.css("width",sliderWidth);
	var totalContent=0;
	fadeSpeed=200;
	
	$thumbScroller_content.each(function () {
		var $this=$(this);
		totalContent+=$this.innerWidth();
		$thumbScroller_container.css("width",totalContent);
		$this.children().children().children(".thumb").fadeTo(fadeSpeed, 0.6);
	});

	$thumbScroller.mousemove(function(e){
		if($thumbScroller_container.width()>sliderWidth){
	  		var mouseCoords=(e.pageX - this.offsetLeft);
	  		var mousePercentX=mouseCoords/sliderWidth;
	  		var destX=-(((totalContent-(sliderWidth))-sliderWidth)*(mousePercentX));
	  		var thePosA=mouseCoords-destX;
	  		var thePosB=destX-mouseCoords;
	  		var animSpeed=600; //ease amount
	  		var easeType="easeOutCirc";
	  		if(mouseCoords>destX){
		  		//$thumbScroller_container.css("left",-thePosA); //without easing
		  		$thumbScroller_container.stop().animate({left: -thePosA}, animSpeed,easeType); //with easing
	  		} else if(mouseCoords<destX){
		  		//$thumbScroller_container.css("left",thePosB); //without easing
		  		$thumbScroller_container.stop().animate({left: thePosB}, animSpeed,easeType); //with easing
	  		} else {
				$thumbScroller_container.stop();  
	  		}
		}
	});

	$outer_container.fadeTo(fadeSpeed, 0.8);
	$outer_container.hover(
		function(){ //mouse over
			var $this=$(this);
			$this.stop().fadeTo("slow", 1);
		},
		function(){ //mouse out
			var $this=$(this);
			$this.stop().fadeTo("slow", 0);
		}
	);

	$thumbScroller_thumb.hover(
		function(){ //mouse over
			var $this=$(this);
			$this.stop().fadeTo(fadeSpeed, 1);
		},
		function(){ //mouse out
			var $this=$(this);
			$this.stop().fadeTo(fadeSpeed, 0.6);
		}
	);

	//on window resize scale image and reset thumbnail scroller
	$(window).resize(function() {
		FullScreenBackground($bgimg);
		$thumbScroller_container.stop().animate({left: sliderLeft}, 400,"easeOutCirc"); 
		var newWidth=$(window).width()-padding;
		$thumbScroller.css("width",newWidth);
		sliderWidth=newWidth;
	});

	FullScreenBackground($bgimg); //scale 1st image
});

$($bgimg).load(function() {
    $preloader.hide();
    //$preloader.fadeOut("fast"); //hide preloader
    var $this = $(this);
    $this.removeAttr("width").removeAttr("height").css({ width: "", height: "" });
    FullScreenBackground($this);
    var imageTitle = $("#img_title").data("imageTitle");
    if (imageTitle) {
        $this.attr("alt", imageTitle);  //.attr("title", imageTitle)
        $("#img_title").html(imageTitle);
    } else {
        $("#img_title").html($(this).attr("title"));
    }
    $preloader.hide();
    $this.fadeIn("slow"); //fadein background image
});

//mouseover toolbar
$toolbar.fadeTo("fast", 0.4);
$toolbar.hover(
	function(){ //mouse over
		var $this=$(this);
		$this.stop().fadeTo("fast", 1);
	},
	function(){ //mouse out
		var $this=$(this);
		$this.stop().fadeTo("fast", 0.4);
	}
);

//Clicking on thumbnail changes the background image
	$("#outer_container a").click(function(event) {
	    event.preventDefault();
	    var $this = $(this);
	    if ($($bgimg).attr("src") != $this.children("img").attr("src")) {
	        $preloader.fadeIn("fast"); //show preloader
	        var title_attr = $this.children("img").attr("title"); //get image title attribute
	        $("#img_title").data("imageTitle", title_attr); //store image title
	        $($bgimg).css("display", "none");
	        $($bgimg).attr("src", this); //change image source
	    }
	    else {
	        $preloader.fadeOut("fast"); //hide preloader
	    }
	});

//Image scale function
function FullScreenBackground(theItem){
var winWidth=$(window).width();
var winHeight=$(window).height();
var imageWidth=$(theItem).width();
var imageHeight=$(theItem).height();
var picHeight = imageHeight / imageWidth;
var picWidth = imageWidth / imageHeight;
if($toolbar.data("imageViewMode")=="full"){ //fullscreen size image mode
	if ((winHeight / winWidth) < picHeight) {
		$(theItem).css("width",winWidth);
		$(theItem).css("height",picHeight*winWidth);
	} else {
		$(theItem).css("height",winHeight);
		$(theItem).css("width",picWidth*winHeight);
	};
} else { //normal size image mode
	if ((winHeight / winWidth) > picHeight) {
		$(theItem).css("width",winWidth);
		$(theItem).css("height",picHeight*winWidth);
	} else {
		$(theItem).css("height",winHeight);
		$(theItem).css("width",picWidth*winHeight);
	};
}
$(theItem).css("margin-left",(winWidth-$(theItem).width())/2);
$(theItem).css("margin-top",(winHeight-$(theItem).height())/2);
}
    </script>
</body>
</html>
