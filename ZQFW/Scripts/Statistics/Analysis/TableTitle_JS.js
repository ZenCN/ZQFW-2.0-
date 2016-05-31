        function init()
        {
           
            var xmlhttp;
            
            //创建XMLHTTP对象,相当于
            try{
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            catch(e)
            {
                xmlhttp = new XMLHttpRequest();
            }
            if(!xmlhttp)
            {
                alert("创建失败");
                return false;
            }
            //准备向服务器的Defaultaspx发出POST的请求,
            var url = "../../GeneralProcedure/LL/TestTableTitle.ashx";
            xmlhttp.open("POST",url,true);
            
            //XMLHTTP默认(也推荐)不是同步请求的,也就是open方法并不像WebClient的DownloadString那样把服务器返回
            //数据拿到才返回,是异步的,因此需要监听onreadystattechang事件
            xmlhttp.onreadystatechange = function()
            {
                if(xmlhttp.readyState == 4)
                {
                    if(xmlhttp.status == 200)
                    {
                        var jsonstr = xmlhttp.responseText;
                        //var teststr = "地区|||甲,受灾范围|县（市、区）|（个）|1,受灾范围|乡（镇）|（个）|2,农作物受灾面积|小计|（千公顷）|1,受灾人口||（万人）|3,死亡人口||（人）|6,失踪人口||（人）|7,转移人口||（人）|8,倒塌房屋||（间）|5,直接经济总损失||（亿元）|9,水利设施直接经济损失||（亿元）|18";
                        //
                        
                        
                        ShowTableTile(jsonstr,"showTableTitle","tableTitle");
                        //document.getElementById('showTableTitle').innerHTML = jsonstr;
                    }
                    else
                    {
                        alert(xmlhttp.readyState);
                        alert(xmlhttp.status);
                        alert("Ajax服务器返回错误");
                    }
                }
                
            }
            //这时候才开始发送请求
            xmlhttp.send();
            
        }

//数据,表格div,表格id名字
var ShowTableTile = function (str,tableID,spanTitle)
{
        //alert(spanTitle);
		var rowNumber = 1;
		var colNumber = 1;
		var strone = str.split(",");
		var strdata = "";
		var tablehtml = "<table border='0' cellspacing='1' cellpadding='0' id="+tableID+" class='tableCss'>";
		var newTr = "";
		var strdataArrtwo = strone[0].split("|");
		var jsonstrLenthArr = ['','','','','','','','','','','','','','','','','','','','','']; 
		var strdataArrThe;
		var strdataArrtwoLength;

		strdataArrtwoLength = strdataArrtwo.length;
		for(var k =0;k<strdataArrtwoLength;k++)
		{
		    
		    colNumber = 1;
			newTr += "<tr class='tableTrTitleCss'>";
			for(var i=0;i<strone.length;i++)
			{
				var strdataArr = strone[i].split("|");
				rowNumber = GetStrTwo(strone[i],"|",k);
				if(strdataArr[k]!="")
				{
				
					if(i==strone.length-1)
					{
						strdataArrTwo = strone[0].split("|");
					}
					else
					{
						strdataArrTwo = strone[i+1].split("|");
					}
					if(strdataArrTwo[k]!= strdataArr[k])
					{
						
						if(colNumber == 1)
						{
							if(rowNumber == 1)
							{
							    if(k==0)
							    {
							        
    							    newTr +="<td widtn='"+jsonstrLenthArr[i]+"'>"+strdataArr[k]+"</td>";
							    }
							    else
							    {
							        newTr +="<td >"+strdataArr[k]+"</td>";
							    }
								
							}
							else
							{  
							    if (rowNumber == 2){
							        if(k==0)
							        {
							            newTr +="<td widtn='"+jsonstrLenthArr[i]+"' rowspan="+rowNumber+" >"+strdataArr[k]+"</td>";
							        }
							        else
							        {
							            newTr +="<td  rowspan="+rowNumber+" >"+strdataArr[k]+"</td>";
							        }
								    
								    
								}else{
								    if(k==0)
								    {
								        newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  rowspan="+rowNumber+" >"+strdataArr[k]+"</td>";
								    }
								    else
								    {
								        newTr +="<td  rowspan="+rowNumber+" >"+strdataArr[k]+"</td>";
								    }
								    
								}
							}
						}
						else
						{
							if(rowNumber == 1)
							{
							    if(k==0)
							    {
							        if(strdataArr[k] == "千公顷")
							        {
//							            var stroption = "<select style='width:150px;' id="'+tableID+'select" onchange=\'UpMJWM("'+tableID+'",3,2,4,"'+tableID+'select");\'><option>千公顷</option><option>万亩</option></select>";
//							            newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  colspan="+colNumber+">"+stroption+"</td>";
                                        var stroption = '<select style="width:150px;" id="'+tableID+'select" onchange=\'UpMJWM("'+tableID+'",3,2,5,"'+tableID+'select");\'><option>千公顷</option><option>万亩</option></select>';
							            newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  colspan="+colNumber+">"+stroption+"</td>";
							        }
							        else
							        {
							            newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  colspan="+colNumber+">"+strdataArr[k]+"</td>";
							        }
							        
							    }
							    else
							    {
							        if(strdataArr[k] == "千公顷")
							        {
//							            var stroption = "<select style='width:150px;' id='select"+tableID+"' onchange=\'UpMJWM('"+tableID+"',2,2,'select"+tableID+"');\'><option>千公顷</option><option>万亩</option></select>";
//							            newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  colspan="+colNumber+">"+stroption+"</td>";
                                        var stroption = '<select style="width:150px;" id="'+tableID+'select" onchange=\'UpMJWM("'+tableID+'",3,2,5,"'+tableID+'select");\'><option>千公顷</option><option>万亩</option></select>';
							            newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  colspan="+colNumber+">"+stroption+"</td>";
							        }
							        else
							        {
							            newTr +="<td  colspan="+colNumber+">"+strdataArr[k]+"</td>";
							        }
							        
							    }
								
							}
							else
							{
							    if(k==0)
							    {
							        newTr +="<td widtn='"+jsonstrLenthArr[i]+"'  colspan="+colNumber+" rowspan="+rowNumber+">"+strdataArr[k]+"</td>";
							    }
							    else
							    {
							        newTr +="<td  colspan="+colNumber+" rowspan="+rowNumber+">"+strdataArr[k]+"</td>";
							    }
                                
							}
							
						}
						colNumber = 1;
					}
					else if(strdataArrTwo[k]== strdataArr[k])
					{
					    
						colNumber++;
						continue;
					}
				}
				else
				{
				    var  ss = strdataArr[k];
//				    newTr += "<td style='width:50px;'></td>";
				}
				
			}
			newTr += "</tr>";
		}
		tablehtml = tablehtml +newTr +"</table>";
		//document.getElementById('showTrueJSON').innerHTML = tablehtml;
		//var showTableDiv = $('<div class="centerCss" style="height:40px;line-height:40px;position:relative;"><span style="position:absolute; left:50%; margin-left:-140px;">'+spanTitle+'</span></div>'+tablehtml);
		var showTableDiv = $(tablehtml);
		//$("body").append(showTableDiv);
		return showTableDiv;
}

var tableData = function(AtableDataTds){
    var dataTr = $(document.createElement('tr'));
    for (var i=0;i<AtableDataTds.length;i++){
        dataTr.append(AtableDataTds[i]);
    }
    return  dataTr;   
}

var tableDataTd = function(Avalue){
    var dataTd = $(document.createElement('td')).append(Avalue);
    return dataTd;
}

//得到某个字符在字符串里面出现的次数
	function GetStrTwo(str,findstr,k){
		var strone = str.split(findstr);
		//alert(strone[3]);
		var number = 1;
		for(var i=k;i<strone.length;i++)
		{
			if(strone[i+1] == "")
			{
			  
			    number = number +1;
			}
			else if(strone[i+1]!= "")
			{
				return number;
			}
		}
		
		return number;
	}
	var cellArr = new  Array();
	var sName = "";
	var sSelectName = "";
//把指定列的数据更换单位
//tableID(修改表格ID),rowStart(开始行),cellStart(开始列),cellEnd(改变几列的值),selectName(下拉框ID)
function UpMJWM(tableID,rowStart,cellStart,cellEnd,selectName)
{
    //alert(tableID.toString() +"     "+ selectName);
        //获得表格对象
		var tableObj = document.getElementById(tableID);
		//获得表格总共多少行
		var rowsNumber = tableObj.rows.length;
		//改变第二行第二列和第三行第二列的值
		//获得单位是千公顷还是万亩
		var unitNumber = "";
		if(sName != tableID && cellArr.length == 0)
		{
		    unitNumber =document.getElementById(selectName).options[document.getElementById(selectName).selectedIndex].text;
		    sName = tableID;
		    sSelectName = selectName;
		}
		else if(sName != tableID && cellArr.length > 0)
		{
		    try{
		        var tableObj1 = document.getElementById(sName);
			    //把多行的第二列的值转换成千公顷
			    for(var i=0;i<cellArr.length;i++)
			    {
			        
				    var cellArr2 = cellArr[i];
				    
				    for(var j=cellStart;j<=cellArr2.length;j++)
				    {
				        
					    tableObj1.rows[rowStart+i].cells[j].innerText = cellArr2[j-cellStart+1].toFixed(2);
    					
				    }
			    }
		    }
		    catch(e)
		    {
    		
		    }
		    cellArr = new  Array();
		    unitNumber =document.getElementById(selectName).options[document.getElementById(selectName).selectedIndex].text;
		    document.getElementById(sSelectName).options.selectedIndex = 0;
		    sName = tableID;
		    sSelectName = selectName;
		}
		else
		{
		    unitNumber =document.getElementById(selectName).options[document.getElementById(selectName).selectedIndex].text;
		    sName = tableID;
		    sSelectName = selectName;
		}
		 
		
		if(unitNumber == "万亩")
		{
			//把多行的第二列的值转换成万亩
			for(var i=rowStart;i<rowsNumber;i++)
			{
				var cellArr2 = new Array();
				for(var j=cellStart;j<=cellEnd;j++)
				{
					
					var erCellData = tableObj.rows[i].cells[j].innerText;
					
					cellArr2[j-1] = erCellData;
					var erCellData2 = (erCellData * 1000 * 15) / 10000;
					
					tableObj.rows[i].cells[j].innerText = erCellData2.toFixed(2);
				}
				cellArr[i-rowStart] = cellArr2;
			}
			
		}
		else
		{
		    //alert(cellArr.length);
		    try{
			    //把多行的第二列的值转换成千公顷
			    for(var i=0;i<cellArr.length;i++)
			    {
				    var cellArr2 = cellArr[i];
				    for(var j=cellStart;j<=cellArr2.length;j++)
				    {
    				    
					    tableObj.rows[rowStart+i].cells[j].innerText =cellArr2[j-cellStart+1];
    					
				    }
			    }
		    }
		    catch(e)
		    {
    		
		    }
		}
		
		//alert(tableObj.rows[3].cells[2].innerText);
}	
	
	