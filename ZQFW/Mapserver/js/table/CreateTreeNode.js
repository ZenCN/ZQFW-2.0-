var bbTreePageNo = "";
//页面加载的时候,获得下拉框的值
$(function (){

     $.ajax({
     type:'get',
     url:'GeneralProcedure/GetYearSelect.ashx',
     success:function(str){
        //alert(str);
        var strArr = str.split("!");
        var strArray = strArr[0].split(',');
        var strArrayTwo = strArr[2].split(',');
        
        var yearSelect = $("#yearSelect");
        for(var i=0;i<strArray.length;i++)
        {
//            var options = $(document.createElement('option'));
//            options.append(strArray[i]);
            var options= "<option value='"+strArrayTwo[i]+"!"+strArray[i]+"'>"+strArray[i]+"</option>";
            //options.val(strArrayTwo[i]);
            yearSelect.append(options);
        }
        new ShowTree(strArr[1]);
     },
     error:function(){
        alert('获得数据出错');
     }
     });
     
});
//
var ChangSelectYear = function(){
    //alert(document.getElementById('yearSelect').value);
    var strValue = document.getElementById('yearSelect').value;
    var strValueArr = strValue.split("!");
    
    var strtitle = "本级共["+strValueArr[0]+ "]张报表";
    $("#spanTitle").text(strtitle);
    $("#yeargjd li[class=liName2]").hide();
}
//生成树形菜单
var ShowTree = function (titleName){
		var ulObj = CreateUl("yeargjd");
		var liObj = CreateLi("yearLi",1);
		var spantitle = "本级共["+titleName + "]张报表";
		
		ulObj.append(CreateDiv("showDiv","yeargjd",spantitle));
		
		
		for(var i=1;i<=12;i++)
		{
			var ulObjs = CreateUl("monthGjd"+i);
			var liObjs = CreateLi("yearLi"+i,0);
			//添加子节点
			//liObjs.append(CreateTrueNodeDiv("trueNode","yearLi","2011年11月15日-2011年11月16日"));
			
			ulObjs.append(CreateDiv("showDiv"+i,"monthGjd"+i,i+"月",i));
			
			ulObjs.append(liObjs);
			liObj.append(ulObjs);
		}
		ulObj.append(liObj);
		$("#report").html(ulObj);
	}
//创建ul,并给ul添加id名字
	function CreateUl(ulid)
	{
		var ulObj = $(document.createElement("ul")).attr('id',ulid);
		return ulObj;
	}
//创建li,并给li添加id名字
	function CreateLi(liid,number)
	{
		var liObj;
		if(number == 0)
		{
			liObj = $(document.createElement("li")).attr({'id':liid}).addClass("liName2").css('cursor','pointer');
		}
		else
		{
			liObj = $(document.createElement("li")).attr('name',liid).addClass("liName1").css('cursor','pointer');
		}
		
		return liObj;
	}
    //创建子节点
	function CreateTrueNodeDiv(divid,ulid,spanVal,numberType,pageNo)
	{
	    
		var divObj = $(document.createElement("div")).attr({'id':divid,'onclick':'NodePageNo("'+pageNo+'")'});					
		//创建图片对象,并添加到divObj里面
		var divImg1 = $(document.createElement("img")).attr({'src':'images/tree/arrow_collapsed.gif'}).addClass("arrow");
		
		//创建图片对象,并添加到divObj里面
		var imgName = "";
		switch(numberType)
		{
		    case "0":
		        imgName =  "images/tree/shi.gif";
		        break;
		    case "1":
		        imgName =  "images/tree/ri.gif";
		    break;
		    case "2":
		        imgName =  "images/tree/xun.gif";
		    break;
		    case "3":
		        imgName =  "images/tree/yue.gif";
		    break;
		    case "4":
		        imgName =  "images/tree/lei.gif";
		    break;
		    case "5":
		        imgName =  "images/tree/zhong.gif";
		    break;
		    case "6":
		        imgName =  "images/tree/guo.gif";
		    break;
		    default:
		        imgName =  "images/tree/chu.gif";
		        break;
		    
		}
		var divImg2 = $(document.createElement("img")).attr({'src':imgName}).addClass("folder");
		//创建span对象,并赋值,并添加到divObj里面
		var divSpan = $(document.createElement("span")).css('cursor','pointer');
		divSpan.append(spanVal);
		divObj.append(divImg1);
		divObj.append(divImg2);
		divObj.append(divSpan);
		return divObj;
	}
	
	//创建Div,并给Div赋值
	function CreateDiv(divid,ulid,spanVal,number)
	{
		var divObj = $(document.createElement("div")).attr({'id':divid,'onclick':'HiddenLi("'+ulid+'",'+number+',"'+divid+'")'});					
		//创建图片对象,并添加到divObj里面
		var divImg1 = $(document.createElement("img")).attr({'src':'images/tree/arrow_collapsed.gif'}).addClass("arrow");
		//创建图片对象,并添加到divObj里面
		var divImg2 = $(document.createElement("img")).attr({'src':'images/tree/folder_closed.gif'}).addClass("folder");
		//创建span对象,并赋值,并添加到divObj里面
		var divSpan;
		if(ulid == "yeargjd")
		{
		    divSpan = $(document.createElement("span")).attr({'id':'spanTitle'});
		}
		else
		{
		    divSpan = $(document.createElement("span")).attr({'id':'spanTitle1'});
		}
		
		divSpan.append(spanVal);
		divObj.append(divImg1);
		divObj.append(divImg2);
		divObj.append(divSpan);
		return divObj;
	}

	function HiddenLi(name,number)
	{
		if(name != "yeargjd")
		{
			if($("#"+name+" li").css('display') == "none")
			{
				//alert($("#"+name+" li").children().length);
				//这里面添加数据//当下面没有数据的时候,去获得数据
				if($("#"+name+" li").children().length >0)
				{
					$("#"+name+" li").empty();
				}
				var strValue = document.getElementById('yearSelect').value;
                var strValueArr = strValue.split("!");
				
				
				ajaxTreeNode(strValueArr[1],number);
				
				$("#"+name+" li").show();

			}
			else
			{
				$("#"+name+" li").hide();
			}
		}
		else
		{
			if($("#"+name+" li").css('display') == "none")
			{
				$("#"+name+" li[name=yearLi]").show();
				
			}
			else
			{
				$("#"+name+" li[name=yearLi]").hide();
			}
		}
	}


var ajaxTreeNode = function(year,month)
{
//        alert(year+month);
        $.ajax({
               type:'get',
               url:'GeneralProcedure/CreateTreeNodeData.ashx?year='+year+'&month='+month,
               success:function(json){
                    //yearLi
                    //alert(str);
                     if(json != "")
                     {
                        var o = eval("("+json+")"); 
                   
                        if(o.length > 0)
                        {
                            for(var i=0;i<o.length;i++)
                            {
                                var liObjs = $("#yearLi"+month);
                                var spanName = o[i].Starttime + "-" + o[i].Endtime;
                                liObjs.append(CreateTrueNodeDiv("trueNode","yearLi",spanName,o[i].SorceType,o[i].Pageno));
                            }
                            
                        }
                     }
                    else
                    {
                        alert("没有"+year +"年"+month+"月的数据");
                    }
                    
                    //alert(o[0].Starttime);
                    
                        
               },
               error:function(){
                  alert('获得数据出错');
               }
          });
}

//var NodePageNo = function(pageNo){
//    gettable(pageNo);
//    bbTreePageNo = pageNo;
//}


















