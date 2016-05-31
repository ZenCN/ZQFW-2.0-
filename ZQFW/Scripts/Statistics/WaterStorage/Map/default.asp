<%@ enablesessionstate=false language=javascript %>
<%

// return "&#160;" for empty text and use server encoder otherwise
function encode(text) {
	return (text != "")? Server.HTMLEncode(text): "&#160;";
}

// extract parameter from the state string
function extractStatePar(state, name) {
	var stateItems = state.split("$");
	if (stateItems == null)
		return "";

	// locate parameter
	for (var stateIndex = 0; stateIndex < stateItems.length; stateIndex++) {
		var stateItem = stateItems[stateIndex];
		if (stateItem != null && stateItem.indexOf(name + "=") == 0)
			return stateItem.substr(name.length + 1, stateItem.length - name.length - 1);
	}
	return "";
}

// replace parameter within the state string
function replaceStatePar(state, name, value) {
	var stateItems = state.split("$");
	if (stateItems == null)
		return name + "=" + value + "$";
	var found = false;

	// locate parameter
	for (var stateIndex = 0; stateIndex < stateItems.length; stateIndex++) {
		var stateItem = stateItems[stateIndex];
		if (stateItem != null && stateItem.indexOf(name + "=") == 0) {
			stateItems[stateIndex] = name + "=" + value;
			found = true;
		}
	}
	return stateItems.join("$") + "$" + (found? "": (name + "=" + value + "$"));
}

// locate parameter in form
function parameter(name, startup) {
	return (Request.Form(name).Count > 0)? Request.Form(name)(1): startup;
}

// ensure the page is not cached
Response.Expires = 0;

// create export parameters
var findV = false;
var hyperlinksNewV = false;
var infoV = false;
var layersV = false;
var legendV = false;
var locationV = false;
var scaleV = false;
var viewsV = false;

// create parameters
var boxBandX = parameter("boxBandX", "");
var boxBandY = parameter("boxBandY", "");
var boxSizeX = parameter("boxSizeX", "");
var boxSizeY = parameter("boxSizeY", "");
var boxStopX = parameter("boxStopX", "");
var boxStopY = parameter("boxStopY", "");
var command = "render";  //parameter("command", "startup");
var count = parameter("count", "0");
var findP = parameter("findP", "y");
var layersP = parameter("layersP", "y");
var legendP = parameter("legendP", "y");
var mode = parameter("mode", "center");
var view = parameter("view", "");
var viewsP = parameter("viewsP", "y");
var what = parameter("what", "");
var where = parameter("where", "");
var queriesP = parameter("queriesP", "y");
var query = parameter("query", "");
var queryPars = parameter("queryPars", "");
var state = parameter("state", "");
var x = parameter("x", "");
var y = parameter("y", "");

// handle mapserver commands that change state string
if (command == "zoomBox") {
	var viewport = extractStatePar(state, "viewport");
	if (viewport != null && viewport != "") {

		// split viewport into pars
		var viewportPars = viewport.split(",");
		if (viewportPars != null && viewportPars.length == 4) {
			var scaleX = parseFloat(viewportPars[0]);
			var scaleY = parseFloat(viewportPars[1]);
			var centerX = parseFloat(viewportPars[2]);
			var centerY = parseFloat(viewportPars[3]);

			// replace centerX and centerY with box center
			var shiftX = boxBandX/2.0 + boxStopX/2.0 - boxSizeX/2.0;
			var shiftY = boxBandY/2.0 + boxStopY/2.0 - boxSizeY/2.0;
			centerX = centerX + scaleX*shiftX;
			centerY = centerY - scaleY*shiftY;

			// replace scaleX and scaleY with scale imposed by box
			var scaleIncX = Math.abs(boxBandX-boxStopX) / boxSizeX;
			var scaleIncY = Math.abs(boxBandY-boxStopY) / boxSizeY;
			var scaleInc = Math.max(Math.max(scaleIncX, scaleIncY), 0.05); // never zoom for more than 20x
			scaleX = scaleX * scaleInc;
			scaleY = scaleY * scaleInc;

			viewportPars[0] = scaleX;
			viewportPars[1] = scaleY;
			viewportPars[2] = centerX;
			viewportPars[3] = centerY;
			viewport = viewportPars.join(",");

			// replace viewport parameter within state string
			state = replaceStatePar(state, "viewport", viewport);
		}
	}
}

// create mapserver object
var mapserver;
mapserver = Server.CreateObject("Manifold.MapServer");
mapserver.Create(Server.MapPath("config.txt"), state, Server);

var index = 0;
var tableSrc = "";

// handle mapserver commands that do not change state string
if (command == "" && mode == "center") {
    mapserver.Center(parseInt(x), parseInt(y));
} else if (command == "" && mode == "info") {
    if (mapserver.HasInfoAt(parseInt(x), parseInt(y)))
        tableSrc = "table.asp?state=" + Server.URLEncode(state) + "&command=" + Server.URLEncode(command) + "&mode=" + Server.URLEncode(mode) + "&x=" + Server.URLEncode(x) + "&y=" + Server.URLEncode(y);
} else if (command == "" && mode == "zoomIn") {
    mapserver.ZoomIn(parseInt(x), parseInt(y));
} else if (command == "" && mode == "zoomOut") {
    mapserver.ZoomOut(parseInt(x), parseInt(y));
} else if (command == "find") {
    mapserver.Locate(what, where);
    tableSrc = "table.asp?command=" + Server.URLEncode(command) + "&what=" + Server.URLEncode(what) + "&where=" + Server.URLEncode(where);
} else if (command == "layers") {
    for (index = 0; index < parseInt(count); index++)
        mapserver.TurnLayer(index, parameter("layer" + index, "") == "on");
} else if (command == "view") {
    mapserver.GoToView(view);
} else if (command == "zoomToFit") {
    mapserver.ZoomToFit();
} else if (command == "query") {
    mapserver.Query(query, queryPars);
    tableSrc = "table.asp?command=" + Server.URLEncode(command) + "&query=" + Server.URLEncode(query) + "&queryPars=" + Server.URLEncode(queryPars);
}
else if (command == "render") {
    //根据灾情信息渲染地图
        var disasterValueArr = [['43010000',20],['43020000',46]];
        //eval("(" + disasterInfoValues + ")");
        var breakValueArr = [-1, 40, 60, 80];
        var colorArr = ['#FFFF00', '#FF0000', '#FF9900', '#0000FF', '#00FFFF'];
        var component = mapserver.Document.ComponentSet("City Drawing");  //获取当前行政区域面图层
        var table = component.OwnedTable;
        var rows = table.RecordSet;

        component.AreaBackground.SetUniqueValues('FID');   //使用单位代码对行政单位进行唯一值渲染
        var xmlDoc = Server.CreateObject("Msxml2.DOMDocument.3.0");
        xmlDoc.async = false;
        var strXML = component.AreaBackground.ToXML();   //将背景色进行唯一值渲染后的结果导出为xml
        xmlDoc.loadXML(strXML);


        //根据数据库灾情数据重新定义面的背景色
        if (xmlDoc.parseError.errorcode == null) {
            //var codeList = xmlDoc.selectSingleNode("//values").childNodes;
            var colorList = xmlDoc.selectSingleNode("//colors").childNodes;
            for (var i = 0; i < rows.Count; i++) {
                var row = rows(i)
                colorList[i].text = '#C2B8AE';
                for (var j = 0; j < disasterValueArr.length; j++) {
                    var code = row.Data("CODE");
                    if (code == disasterValueArr[j][0]) {
                        var disasterValue = disasterValueArr[j][1];
                        if (disasterValue < breakValueArr[0]) {
                            colorList[i].text = colorArr[0];
                        }
                        else if (disasterValue < breakValueArr[1]) {
                            colorList[i].text = colorArr[1];
                        }
                        else if (disasterValue < breakValueArr[2]) {
                            colorList[i].text = colorArr[2];
                        }
                        else if (disasterValue < breakValueArr[3]) {
                            colorList[i].text = colorArr[3];
                        }
                        else {
                            colorList[i].text = colorArr[4];
                        }
                        break;
                    }
                }
            }
        }
        component.AreaBackground.LoadFrom(xmlDoc.xml, true);  //根据修改后的xml渲染行政单位背景色
}

// create results
var copyright = mapserver.Copyright;
var cxhdr = mapserver.CX + 180;
var cxmap = mapserver.CX;
var cymap = mapserver.CY;
var fields = mapserver.Fields;
var layers = mapserver.Layers;
var location = "";
if (locationV)
	location = mapserver.Location;
var scaling = "";
if (scaleV)
	scaling = mapserver.Scale;
state = mapserver.State;
var subtitle = mapserver.Subtitle;
var target = mapserver.Target;
var title = mapserver.Title;
var views = mapserver.Views;
var queries = mapserver.Queries;

var queriesC = "";
var queryList = queries.split("\n");
var querySqno = 0;
var queryItem;
for (queryItem in queryList) {
	if (queryList[queryItem] != "") {
		queriesC = queriesC + "<tr><td><table><tr><td>" + encode(queryList[queryItem]) + "</td></tr>\n";

		var queryPar = mapserver.QueryParameters(querySqno);
		var parList = queryPar.split("\n");
		var parItem;
		var parNumber = 0;
		for (parItem in parList) {
			if (parList[parItem] != "")
				queriesC = queriesC + "<tr><td>" + encode(parList[parItem]) + "</td></tr><tr><td><input type=\"edit\" id=\"q" + querySqno + "p" + parNumber + "\" name=\"q" + querySqno + "p" + parNumber + "\" size=\"10\"></td></tr>\n";
			parNumber++;
		}		
		queriesC = queriesC + "<tr><td><input type=\"button\" class=\"panetext\" value=\"Query\" onclick=\"execQuery('"+ querySqno + "');\"></td></tr></table></td></tr>\n"
	}
	querySqno++;
}
if (queriesC != "")
	queriesC = "<table>" + queriesC + "</table>";

// release mapserver object
mapserver = null;

// redirect response if necessary
if (target != "" && !hyperlinksNewV)
	Response.Redirect(target);

// compose image URL
var image = "map.asp?state=" + Server.URLEncode(state) + "&command=" + Server.URLEncode(command) + "&mode=" + Server.URLEncode(mode) + "&query=" + Server.URLEncode(query) + "&queryPars=" + Server.URLEncode(queryPars) + "&what=" + Server.URLEncode(what) + "&where=" + Server.URLEncode(where) + "&x=" + Server.URLEncode(x) + "&y=" + Server.URLEncode(y);

// create find pane if necessary
var findC = "";
if (findV) {
	var  findItem, findList = fields.split("\n");
	for (findItem in findList) {
		if (findList[findItem] != "")
			findC = findC + "<option value=\"" + findList[findItem] + "\">" + encode(findList[findItem]) + "\n";
	}
	if (findC != "") {
		findC = "\n<table>\n" +
			"<tr><td class=\"panetext\" width=\"20%\">Find:</td><td width=\"80%\"><input id=\"what\" name=\"what\" class=\"panetext\" type=\"text\" size=\"10\"></td></tr>\n" +
			"<tr><td class=\"panetext\" width=\"20%\">Within:</td><td width=\"80%\"><select id=\"where\" name=\"where\" class=\"panetext\" size=\"1\">" + findC + "</select></td></tr>\n" +
			"<tr><td width=\"20%\"></td><td width=\"80%\"><input type=\"button\" class=\"panetext\" value=\"Go\" onclick=\"invokePane('find');\"></td></tr>\n" +
			"</table>\n";
	}
}

// create layers pane if necessary
var layersC = "";
if (layersV) {
	var  layerChck = "";
	var  layerDefv = (command == "startup")? "on": "";
	var  layerList = layers.split("\n");
	var  layerSqno = 0;
	var  layerItem;
	for (layerItem in layerList) {
		if (layerList[layerItem] != "") {
			layerChck = parameter("layer" + layerSqno, layerDefv);
			if (layerChck != "")
				layerChck = "checked";
			layersC = layersC + "<tr><td class=\"panetext\"><input id=\"layer" + layerSqno + "\" name=\"layer" + layerSqno + "\" " + layerChck + " type=\"checkbox\">" + encode(layerList[layerItem]) + "</td></tr>\n";
		}
		layerSqno++;
	}
	if (layersC != "") {
		layersC = "\n<table>\n" + layersC +
			"<tr><td><input type=\"hidden\" id=\"count\" name=\"count\" value=\"" + layerSqno + "\"></td></tr>\n" +
			"<tr><td><input type=\"button\" class=\"panetext\" value=\"Apply\" onclick=\"invokePane('layers');\"></td></tr>\n" +
			"</table>\n";
	}
}

// create legend pane if necessary
var legendC = "";
if (legendV) {
	legendC = "<img src=\"map.asp?type=legend\" alt=\"Legend\">";
}

// create views pane if necessary
var viewsC = "";
if (viewsV) {
	var  viewItem;
	var  viewList = views.split("\n");
	for (viewItem in viewList) {
		if (viewList[viewItem] != "")
			viewsC = viewsC + "<tr><td class=\"view\"><a href=\"#\" onclick=\"invokeView(unescape('" + escape(viewList[viewItem]) + "'));\">" + encode(viewList[viewItem]) + "</a></td></tr>\n";
	}
	if (viewsC != "") {
		viewsC = "\n<table>\n" + viewsC + "</table>\n";
	}
}

%>

<html>
<head><title><%= encode(title) %></title>

<!-- content scripting -->
<script type="text/javascript" language="javascript">
<!--

var opera = (navigator.userAgent.indexOf("Opera") >= 0)? true: false;
var ie = (document.all && !opera)? true: false;
var nn4 = (document.layers)? true: false;
var bandH = <%= cymap %>;
var bandW = <%= cxmap %>;
var bandX = -1;
var bandXOffset = 0;
var bandY = -1;
var bandYOffset = 0;

// cutoff tail from given string if any
function cutOff(text, tail) {
	var i = text.toLowerCase().lastIndexOf(tail.toLowerCase());
	if (i != -1 && text.substring(i).toLowerCase() == tail.toLowerCase())
		text = text.substring(0, i);
	return text;
}

// obtain absolute top position of element in screen coordinates
function GetOffsetTop(el)
{
	var res = 0;
	while (null != el) {
		res += el.offsetTop;
		el = el.offsetParent;
	}
	return res;
}

// obtain absolute left position of element in screen coordinates
function GetOffsetLeft(el)
{
	var res = 0;
	while (null != el) {
		res += el.offsetLeft;
		el = el.offsetParent;
	}
	return res;
}

// start tracking rubber band
function launchBand(evt) {
	var band = locateElement("band");
	var map = locateElement("map");
	if (map != null && band != null) {
		band.style.width = 0;
		band.style.height = 0;

		// set up band taking care of different browsers
		if (ie) {
			evt = event;
			if (1 != evt.button)
				return;
			bandX = evt.offsetX + GetOffsetLeft(map);
			bandXOffset = evt.offsetX;
			bandY = evt.offsetY + GetOffsetTop(map);
			bandYOffset = evt.offsetY;
			band.style.pixelLeft = bandX;
			band.style.pixelTop = bandY;
			band.style.visibility = "visible";
		} else {
			if (0 != evt.button)
				return;
			bandX = evt.pageX;
			bandXOffset = evt.pageX - GetOffsetLeft(map);
			bandY = evt.pageY;
			bandYOffset = evt.pageY - GetOffsetTop(map);
			band.style.left = bandX + "px";
			band.style.top = bandY + "px";
			band.style.visibility = "visible";
		}

		map.onmousemove = updateBand;
		band.onmousemove = updateBand;

		// capture mouse events
		if (ie)
			map.setCapture();
	}
}

// update rubber band
function updateBand(evt) {
	var band = locateElement("band");
	if (band != null) {

		// move band taking care of different browsers
		if (ie) {
			evt = event;
			var map = locateElement("map");
			if (evt.srcElement != map)
				return;
			band.style.pixelLeft = selectMin(bandX, evt.offsetX + GetOffsetLeft(map));
			band.style.pixelTop = selectMin(bandY, evt.offsetY + GetOffsetTop(map));
			band.style.width = selectMax(bandX, evt.offsetX + GetOffsetLeft(map)) - band.style.pixelLeft;
			band.style.height = selectMax(bandY, evt.offsetY + GetOffsetTop(map)) - band.style.pixelTop;
		} else {
			band.style.left = selectMin(bandX, evt.pageX);
			band.style.top = selectMin(bandY, evt.pageY);
			band.style.width = selectMax(bandX, evt.pageX) - parseInt(band.style.left);
			band.style.height = selectMax(bandY, evt.pageY) - parseInt(band.style.top);
		}
	}
}

// stop tracking rubber band
function finishBand(evt) {
	var band = locateElement("band");
	var map = locateElement("map");
	if (map != null && band != null && -1 != bandX) {
		var stopX = bandX;
		var stopY = bandY;

		// stop tracking band taking care of different browsers
		if (ie) {
			evt = event;
			stopX = evt.offsetX;
			stopY = evt.offsetY;
			band.style.visibility = "hidden";
		} else {
			stopX = evt.pageX - GetOffsetLeft(map);
			stopY = evt.pageY - GetOffsetTop(map);
			band.style.visibility = "hidden";
		}

		map.onmousemove = null;
		band.onmousemove = null;

		// stop capturing mouse events
		if (ie)
			map.releaseCapture();

		// reload form
		modifyHidden("boxSizeX", bandW);
		modifyHidden("boxSizeY", bandH);
		modifyHidden("boxBandX", selectMinMax(bandXOffset, 0, bandW-1));
		modifyHidden("boxBandY", selectMinMax(bandYOffset, 0, bandH-1));
		modifyHidden("boxStopX", selectMinMax(stopX, 0, bandW-1));
		modifyHidden("boxStopY", selectMinMax(stopY, 0, bandH-1));
		invokePane("zoomBox");
	}
}

function invokePane(name) {
	modifyHidden("command", name);
	reload();
}

function invokeView(view) {
	modifyHidden("command", "view");
	modifyHidden("view", view);
	reload();
}

// locate element by ID
function locateElement(name) {
	var element;

	// locate element in a manner compatible to all browsers
	if (document.getElementById)
		element = document.getElementById(name);
	else if (document.all)
		element = document.all[name];
	else if (document.layers) {
		for (var i = 0; i < document.forms[0].length; ++i) {
			if (document.forms[0].elements[i].name == name) {
				element = document.forms[0].elements[i];
				break;
			}
		}
	}
	return element;
}

// modify value of hidden input
function modifyHidden(name, value) {

	// locate hidden
	var hidden = locateElement(name);
	if (null == hidden)
		return null;

	// modify its value
	hidden.value = value;
}

// obtain value of hidden input
function obtainHidden(name) {

	// locate hidden
	var hidden = locateElement(name);
	if (null == hidden)
		return null;

	// obtain its value
	return hidden.value;
}

// reload page
function reload() {
	document.forms[0].submit();
}

function execQuery(queryNum) {
	modifyHidden("command", "query");
	modifyHidden("query", queryNum);
	var parStr = "";
	var i = 0;
	var element = locateElement("q" + queryNum + "p" + i);
	while (element != null) {
		if (i != 0)
			parStr = parStr + "\n";
		parStr = parStr + element.value;
		++i;
		element = locateElement("q" + queryNum + "p" + i);
	}
	modifyHidden("queryPars", parStr);
	reload();
}

// select maximum value
function selectMax(a, b) {
	return a > b? a: b;
}

// select minimum value
function selectMin(a, b) {
	return a < b? a: b;
}

// select value between minimum and maximum
function selectMinMax(v, lo, hi) {
	return v > hi? hi: v < lo? lo: v;
}

function switchTool(name) {
	modifyHidden("mode", name);
	updateToolbar();
}

// expand or collapse pane
function togglePane(name) {
	modifyHidden(name + "P", (obtainHidden(name + "P") == "y")? "n": "y");
	updatePane(name);
}

// update pane state
function updatePane(name) {
	var pane, paneState;

	// locate pane (table with two rows) and pane state hidden
	if (document.getElementById) {
		pane = document.getElementById(name);
		paneState = document.getElementById(name + "P");
	} else if (document.all) {
		pane = document.all[name];
		paneState = document.all[name + "P"];
	} else if (document.layers) {
		pane = document.layers[name];
		paneState = document.layers[name + "P"];
	}
	if (null == pane)
		return;

	// modify content
	var content = pane.rows[1];
	if ("n" == paneState.value)
		content.style.display = "none";
	else
		content.style.display = (ie)? "inline" : "table-row";

	// modify image and title
	var toggle = document.images[name+"_arrow"];
	var toggleImage = toggle.src;
	toggleImage = cutOff(toggleImage, ".png");
	toggleImage = cutOff(toggleImage, "P");
	toggleImage = toggleImage + ((paneState.value == "n")? "P.png" : ".png");
	toggle.src = toggleImage;
	toggle.title = (paneState.value == "n")? "Expand Pane" : "Collapse Pane";
}
	
// update tool icon
function updateTool(name, current) {

	// locate tool
	var tool = document.images[name];

	// modify tool icon
	var toolImage = tool.src;
	toolImage = cutOff(toolImage, ".png");
	toolImage = cutOff(toolImage, "P");
	toolImage = toolImage + ((name == current)? "P.png" : ".png");
	tool.src = toolImage;
}

// update tool event hooks
function updateToolHooks() {
	var map = locateElement("map");
	var band = locateElement("band");

	// hook or unhook mouse event handlers depending on current tool
	var tool = obtainHidden("mode");
	if ("zoomBox" == tool) {
		map.onmousedown = launchBand;
		map.onmouseup = finishBand;
		band.onmouseup = finishBand;
	} else {
		map.onmousedown = null;
		map.onmouseup = null;
		band.onmouseup = null;
	}
}

// update toolbar buttons and tool status pane
function updateToolbar() {
	if (nn4)
		return;

	var mode, tool, pane;

	// locate status pane and update its contents
	if (document.getElementById) {
		mode = document.getElementById("mode");
		tool = document.getElementById(mode.value);
		pane = document.getElementById("tool");
		pane.innerHTML = tool.title;
	} else if (document.all) {
		mode = document.all["mode"];
		tool = document.all[mode.value];
		pane = document.all["tool"];
		pane.innerHTML = tool.title;
	}

	// update toolbar buttons
	updateTool("center", mode.value);
<% if (infoV) { %>
	updateTool("info", mode.value);
<% } %>
	updateTool("zoomIn", mode.value);
	updateTool("zoomOut", mode.value);
	updateTool("zoomBox", mode.value);

	// update tool event hooks
	updateToolHooks();
}

// start page
function startup() {
	updatePane("find");
	updatePane("layers");
	updatePane("legend");
	updatePane("views");
	updatePane("queries");
	updateToolbar();

	<% if (target != "" && hyperlinksNewV) { %>
	window.open("<%= target %>");
	<% } %>

	<% if (tableSrc != "") { %>
	tablewindow = window.open("<%= tableSrc %>", null, "left=100, top=300, width=<%= cxmap %>, toolbar=no, menubar=no, location=no, scrollbars=yes, resizable=yes");
	tablewindow.focus();
	<% } %>
}

// -->
</script>
<link rel="stylesheet" type="text/css" href="default.css"></link>
</head>

<body link="black" vlink="black" alink="black" language="javascript" onload="startup();">

<!-- content -->
<form action="default.asp" method="post">
<table>

	<!-- header -->
	<% if (title != "") { %>
	<tr><td class="header" colspan="2" width="<%= cxhdr %>"><%= encode(title) %></td></tr>
	<% } %>
		
	<!-- subheader -->
	<% if (subtitle != "") { %>
	<tr><td class="subheader" colspan="2" width="<%= cxhdr %>"><%= encode(subtitle) %></td></tr>
	<% } %>

	<tr>
	<td valign="top"><table cellspacing="0">
		<tr><td>
		
			<!-- toolbar -->
			<table class="toolbar" cellspacing="0" cellpadding="0" width="100%"><tr>
			<td class="toolfiller">
				<table cellspacing="0" cellpadding="1"><tr>
				<td class="tool"><img name="center" id="center" src="images/ToolCenter.png" alt="Center" title="Center" border="0" onclick="switchTool('center');"></td>
				<td class="tool"><img name="zoomIn" id="zoomIn" src="images/ToolZoomIn.png" alt="Zoom In" title="Zoom In" border="0" onclick="switchTool('zoomIn');"></td>
				<td class="tool"><img name="zoomOut" id="zoomOut" src="images/ToolZoomOut.png" alt="Zoom Out" title="Zoom Out" border="0" onclick="switchTool('zoomOut');"></td>
				<td class="tool"><img name="zoomBox" id="zoomBox" src="images/ToolZoomBox.png" alt="Zoom Box" title="Zoom Box" border="0" onclick="switchTool('zoomBox');"></td>
				<td class="tool"><img name="zoomToFit" id="zoomToFit" src="images/ToolZoomToFit.png" alt="Zoom To Fit" title="Zoom To Fit" border="0" onclick="invokePane('zoomToFit');"></td>
				<% if (infoV) { %>
				<td class="tool"><img name="info" id="info" src="images/ToolInfo.png" alt="Info" title="Info" border="0" onclick="switchTool('info');"></td>
				<% } %>
				</tr></table>
			</td></tr></table>

		</td></tr><tr><td>

			<!-- map -->
			<input id="map" class="map" type="image" src="<%= image %>" width="<%= cxmap %>" height="<%= cymap %>" border="1">
			<input id="boxBandX" name="boxBandX" type="hidden" value="<%= boxBandX %>">
			<input id="boxBandY" name="boxBandY" type="hidden" value="<%= boxBandY %>">
			<input id="boxSizeX" name="boxSizeX" type="hidden" value="<%= boxSizeX %>">
			<input id="boxSizeY" name="boxSizeY" type="hidden" value="<%= boxSizeY %>">
			<input id="boxStopX" name="boxStopX" type="hidden" value="<%= boxStopX %>">
			<input id="boxStopY" name="boxStopY" type="hidden" value="<%= boxStopY %>">
			<input id="command" name="command" type="hidden" value="">
			<input id="findP" name="findP" type="hidden" value="<%= findP %>">
			<input id="layersP" name="layersP" type="hidden" value="<%= layersP %>">
			<input id="legendP" name="legendP" type="hidden" value="<%= legendP %>">
			<input id="mode" name="mode" type="hidden" value="<%= mode %>">
			<input id="state" name="state" type="hidden" value="<%= state %>">
			<input id="view" name="view" type="hidden" value="">
			<input id="viewsP" name="viewsP" type="hidden" value="<%= viewsP %>">
			<input id="queriesP" name="queriesP" type="hidden" value="<%= queriesP %>">
			<input id="query" name="query" type="hidden" value="">
			<input id="queryPars" name="queryPars" type="hidden" value="">

		</td></tr><tr><td>

			<!-- status bar -->
			<table cellspacing="0" width="100%"><tr>
			<td class="stat" width="80"><div id="tool" name="tool">Tool</div></td>
			<td class="statspace"></td>
			<td class="stat"><% if (locationV && location != "") { %><%= encode(location) %><% } %></td>
			<td class="statspace"></td>
			<td class="stat" width="80"><% if (scaleV && scaling != "") { %><%= encode(scaling) %><% } else { %>&nbsp;<% } %></td>
			</tr></table>

		</td></tr>

		<!-- footer -->
		<tr><td class="footer" colspan="2"><%= encode(copyright) %></td></tr>

	</table></td><td valign="top"><table cellspacing="0" width="180">

		<!-- find pane -->
		<% if (findV && findC != "") { %>
		<tr><td><table id="find" cellspacing="0" width="100%">
		<tr><td class="panecaption" width="90%">Find</td><td class="panearrow"><img name="find_arrow" src="images/Arrow.png" alt="Arrow" title="Collapse Pane" onclick="togglePane('find');"></td></tr>
		<tr><td class="pane" colspan="2"><%= findC %></td></tr>
		</table></td></tr>
		<% } %>

		<!-- layers pane -->
		<% if (layersV && layersC != "") { %>
		<tr><td><table id="layers" cellspacing="0" width="100%">
		<tr><td class="panecaption" width="90%">Layers</td><td class="panearrow"><img name="layers_arrow" src="images/Arrow.png" alt="Arrow" title="Collapse Pane" onclick="togglePane('layers');"></td></tr>
		<tr><td class="pane" colspan="2"><%= layersC %></td></tr>
		</table></td></tr>
		<% } %>

		<!-- legend pane -->
		<% if (legendV && legendC != "") { %>
		<tr><td><table id="legend" cellspacing="0" width="100%">
		<tr><td class="panecaption" width="90%">Legend</td><td class="panearrow"><img name="legend_arrow" src="images/Arrow.png" alt="Arrow" title="Collapse Pane" onclick="togglePane('legend');"></td></tr>
		<tr><td class="pane" colspan="2"><%= legendC %></td></tr>
		</table></td></tr>
		<% } %>

		<!-- views pane -->
		<% if (viewsV && viewsC != "") { %>
		<tr><td><table id="views" cellspacing="0" width="100%">
		<tr><td class="panecaption" width="90%">Views</td><td class="panearrow"><img name="views_arrow" src="images/Arrow.png" alt="Arrow" title="Collapse Pane" onclick="togglePane('views');"></td></tr>
		<tr><td class="pane" colspan="2"><%= viewsC %></td></tr>
		</table></td></tr>
		<% } %>

		<!-- queries pane -->
		<% if (queriesC != "") { %>
		<tr><td><table id="queries" cellspacing="0" width="100%">
		<tr><td class="panecaption" width="90%">Queries</td><td class="panearrow"><img name="queries_arrow" src="images/Arrow.png" alt="Arrow" title="Collapse Pane" onclick="togglePane('queries');"></td></tr>
		<tr><td class="pane" colspan="2"><%= queriesC %></td></tr>
		</table></td></tr>
		<% } %>
	</table></td>
	</tr>

</table>
</form>

<!-- band used by ZoomBox tool -->
<div id="band" name="band" style="border:1px dashed black; width: 0px; height: 0px; position: absolute; visibility: hidden;"></div>

</body>
</html>
