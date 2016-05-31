//受灾类型设置
var disasterTypeArr;
var typeNumberold="";
//获取渲染的数据getCityDisasterData
function  getCityDisasterData(pageno,typeNumber,disasterType,unitCode,year){
        var bool = false;
        getTL(disasterType,typeNumber,unitCode)
         $.ajax({
             type:'get',
             url:'../GeneralProcedure/GetMapDataJson.ashx?disaster='+disasterType+'&pageno='+pageno+'&unitCode=' + unitCode+'&year='+year,
             success:function(jsonstr){ 
             
            if(jsonstr!="")
            {
                
                var sobj = eval("("+jsonstr+")");
                //alert(sobj['divdata']);
                disasterTypeArr= sobj['mapdata']; 
                zhdb(sobj['divdata']);
                bool = true;
            } 
            else if(unitCode=="")
            {
                alert("市级无数据！！")
            }else
            {
                alert("县级无数据！请选择其他指标或报表！")
            }
        
        },
         error:function(){
            alert('获得灾害类型数据出错');
         },async:false
     });
     return bool
}
//MapDiv数据
function zhdb(str)//灾害类型数据表
{
    var disaterType=str.split("@");
    var starttime=disaterType[1].split(",")[0];
    var endtime=disaterType[1].split(",")[1];
    var zhdb=disaterType[2].split("!");
    
    var htmlstr ='';
    htmlstr+="<table cellspacing='0'><thead><tr><td colspan='2'>"+disaterType[0]+"</td></tr><tr><td colspan='2'>"+starttime+"至"+endtime+"</td></tr></thead><tbody>";
    
    for(var i=0;i<zhdb.length;i++)
    {
         htmlstr+="<tr "+(i==zhdb.length-1?"class='last'":"")+"><th><span>"+zhdb[i].split(",")[0]+"</span></th><td>"+zhdb[i].split(",")[1]+"</td></tr>";
    }
    htmlstr+="</tbody></table>";
    
    window.parent.fillstr(htmlstr);
}


//图例获取
var tlarr=new Array();
function getTL(disasterType,typeNumber,unitcode)
{
    //图列两类数组获取
    if(unitcode=="")//市级
    {
        if(typeNumber == 1)
        {
            switch (disasterType)
            {
                case  "szl":tlarr=[0.1,15,40,60] ;break;
                case  "szrk":tlarr=[0.1,20,40,60] ;break;
                case  "zyrk":tlarr=[0.01,2,5,10] ;break;
                case  "zjjjzss":tlarr=[0.001,5,10,15] ;break;
                default:tlarr=[0.1,15,40,60];
            }
        }
        else
        {
            switch (disasterType)
           {
                case  "shl":tlarr=[0.1,15,40,60] ;break;
                case  "shyxrs":tlarr=[0.1,20,40,60] ;break;
                case  "ysknrk":tlarr=[0.01,2,8,15] ;break;
                case  "KHzjjzss":tlarr=[0.001,1,5,10] ;break;
                default:tlarr=[20,60,100,300] ;
            }
        }
    }
    else{//县级
        if(typeNumber == 1)
        {
            switch (disasterType)
            {
                case  "szl":tlarr=[0.1,15,40,60] ;break;
                case  "szrk":tlarr=[0.1,3,10,30] ;break;
                case  "zyrk":tlarr=[0.01,1,3,6] ;break;
                case  "zjjjzss":tlarr=[0.001,1.5,5,10] ;break;
                default:tlarr=[0.1,15,40,60] ;
            }
        }
        else
        {
            switch (disasterType)
           {
                case  "shl":tlarr=[0.1,15,40,60];break;
                case  "shyxrs":tlarr=[0.1,3,10,30] ;break;
                case  "ysknrk":tlarr=[0.001,0.5,2,5] ;break;
                case  "KHzjjzss":tlarr=[0.001,0.5,2.5,5] ;break;
                default:tlarr=[0.1,15,40,60] ;
            }
        }
    }
   
    UpdataTL(disasterType,typeNumber,tlarr)
    
}
//图例框框标题更新
var UpdataTL = function(disasterType,typeNumber,tlarr){
    var title="";
    switch(disasterType)
    {
        case "szrk":title="受灾人口(万人)";break;
        case "szl":title="受灾率(百分比)";break;
        case "zjjjzss":title="直接经济总损失(亿元)";break;
        case "zyrk":title="转移人口(万人)";break;
        case "shl":title="受旱率(百分比)";break;
        case "shyxrs":title="受灾面积率(百分比)";break;
        case "KHzjjzss":title="抗旱经济总损失(亿元)";break;
        case "ysknrs":title="抗旱灾害类型";break;
        default:title="受灾人口(万人)";
    }
    
    //图例框框范围值获取
//    document.getElementById('mapTitleSpan').innerHTML = title
//    document.getElementById('greenSpan').innerHTML = "0-"+tlarr[0];
//    document.getElementById('yellowSpan').innerHTML = tlarr[0]+"-"+tlarr[1];
//    document.getElementById('orangeSpan').innerHTML = tlarr[1]+"-"+tlarr[2];
//    document.getElementById('RedSpan').innerHTML = tlarr[2]+"-"+tlarr[3];
//    //图例框框颜色获取
//    if(typeNumber == 1)
//    {        
//         $("#TL1").css("background-color","#cddf8d");
//         $("#TL2").css("background-color","#efd784");
//         $("#TL3").css("background-color","#efb684");
//         $("#TL4").css("background-color","#ec948c");
//    }
//    else
//    {
//         $("#TL1").css("background-color","#eee");
//         $("#TL2").css("background-color","#777");
//         $("#TL3").css("background-color","#222");
//         $("#TL4").css("background-color","#000");
//    }
}