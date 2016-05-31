/*-------------------------------------
#Description:      页面上的菜单滑动效果
#Version:          1.0
#Author:           dingyumei
#Recent:           2012-12-05
-------------------------------------*/
var curr="create";
var ver = $.cookie("cookiesVersion");
var Url = {
    create:"nomove",
    open :"nomove",
    inbox : "Inbox.aspx?ver="+ver,
    map:"/Statistics/Map?ver="+ver,
    analysis: "/Statistics/Analysis?ver=" + ver,
    assessment: "/Statistics/Assessment?ver=" + ver,
    trash:"Recycle.aspx?ver="+ver, //刘志 添加回收站、工作秘书页面
    secretary:"WorkSecretary.aspx?ver="+ver,
    leadaudit:"LeaderAduit.aspx?ver=" + ver,
    notready:"NotReady.aspx?ver="+ver
}
var agoto=function(to)
{
    if(curr!=to.find("i").attr("class")){
        $('.floatblock').stop();
        var pleft = to.offset().left;
        var pwidth = to.width() + 20;
        $('.floatblock').animate({width:pwidth+'px',left:pleft+'px'},{step:function(now,event){
            switch(event.prop)
            {
                case "left": 
                $('.floatblock').css("background-position","-" + now + "px -70px");
                break;
            }
        }});        
    }    
}
var slidepage = function(target)
{
    if(curr!=target.find("i").attr("class")){
        var targetname = target.find("i").attr("class")  == "open" ? "create" : target.find("i").attr("class"); 
        target = $("#"+targetname);
        $(".slide_page").stop().css({"z-index":"1"});
        target.css({"z-index":"5","left":"-900px"}).show();
        target.animate({left:"0"},"fast",function(){
            $(".slide_page").not(target).hide();
        });    
    }
}
$(function(){
    $(".menu li:not(.flo)").click(function(){
        var Name = $(this).find("i").attr("class");
        var _this = this;
        var Click = function()
        {
            $(".menu li").removeClass("selected");
            $(_this).addClass("selected");
            if($("#" + Name + " iframe").length ==0 && Name!="create" && Name != "open")
            {
                var src = $(Url).attr(Name);
                src = src == undefined ? Url.notready : src;
                $("#" + Name).html("<iframe height='100%' width='100%' frameborder='0' src="+src+" name="+Name+" id='iframe"+Name+"'></iframe>");
            }
            else if(Name == "create")
            {
                $("#create .report_box,#create .post_form,#create #create_table,.Slider").removeClass("opening").addClass("creating");
                
            }
            else if(Name == "open")
            {
                $("#create .report_box,#create .post_form,#create #create_table,.Slider").removeClass("creating").addClass("opening");
            }
            agoto($(_this));
            var Namein = $.inArray(Name,["create","open"])>=0;
            var Currin = $.inArray(curr,["create","open"])>=0;
                    
            if(!Namein || !Currin)
            {
                slidepage($(_this));
            }
            
            curr=$(_this).find("i").attr("class");
        }
        if(Name =="create" && $("#post_table").css("display") == "block")
        {
            if(BackToCreate())
            {
                Click();
            }
        }
        else
        {
            Click();
        }           
        
        ///* *******************************刘志 每次切换页面的时候，均按上次页面的数据重新查找刷新一次 ***********************///////////
        ///* 不足之处：没有任何数据变动的时候，也刷新了
//        if(Name =="open")
//        {
//            CrateTreeList();
//        } 
//        else if(Name =="inbox")
//        {
//            top.inbox.searchRpt(0,true);
//        }
//        else if(Name=="trash")
//        {   
//            top.trash.searchRpt(true);
//        }
        
    })
}) 