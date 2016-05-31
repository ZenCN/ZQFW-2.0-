<%@ enablesessionstate=false language=javascript %>
<%

// locate parameter in query string
function parameter(name, startup) {
	return (Request.QueryString(name).Count > 0)? Request.QueryString(name): startup;
}

Response.ContentType = "image/png";
Response.Expires = 0;

// create mapserver
var mapserver;
mapserver = Server.CreateObject("Manifold.MapServer");
mapserver.Create(Server.MapPath("config.txt"), parameter("state", ""), Server);

var type = parameter("type", "");

// inject pngfile into the response
if (type == "legend")
	Response.BinaryWrite(mapserver.RenderLegend());
else {
	var command = parameter("command", "startup");
	var mode = parameter("mode", "center");
	var query = parameter("query", "");
	var queryPars = parameter("queryPars", "");
	var what = parameter("what", "");
	var where = parameter("where", "");
	var x = parameter("x", "");
	var y = parameter("y", "");

	if (command == "query")
		mapserver.Query(query, queryPars);
	else if (command == "find")
		mapserver.Locate(what, where);
	else if (command == "" && mode == "info")
		mapserver.Click(x, y);

	Response.BinaryWrite(mapserver.Render());
}

// release mapserver object
mapserver = null;

%>
