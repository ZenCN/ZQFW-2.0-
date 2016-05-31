  
var ShowTableDate=function(strdata,tableid){
    var str=strdata;
    var table=$("#"+tableid);
    var arr = str.split("!");
    var arrlen = arr.length;
    for (var i = 0; i < arrlen; i++) {
        var tr = $(document.createElement("tr")).addClass("tableTrCss");
        var arr2 = arr[i].split(",");
        var arr2len = arr2.length;
        for (var j = 0; j < arr2len ; j++) {
            var td = $(document.createElement("td"));
            td.append(arr2[j]);
            tr.append(td);
        }
   table.append(tr);
   }            
}

var newBBTable = function(pageno,mapLevel,unitcode){
            $.ajax({
	    	        type: 'post',
	    	        url: "BBData",
	    	        data: { pageno: pageno, mapLevel: mapLevel,unitCode:unitcode },
	    	        success: function(json) {
//	    	            var jsonobj = eval("(" + json + ")");	 	            
//	    	           $('#leftcon').html(new ShowTableTile(jsonobj.TableTitle,"frontTable0",jsonobj.SpanTitle));	    	            
//	    	            ShowTableDate(jsonobj.TableData,"frontTable0");
                            
                            var jsonobj = eval("(" + json + ")");	
                            
                            $('#floatbox').html(new ShowTableTile(jsonobj.hlTitle,"frontTable0",""));
                            ShowTableDate(jsonobj.hlData,"frontTable0");
	    	            },
	    	        error: function(){
	    	        
	    	            }
	    	        });
        }