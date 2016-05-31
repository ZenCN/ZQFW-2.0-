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

//阻止事件冒泡,使成为捕获型事件触发机制.
function stopBubble(e) {
    //如果提供了事件对象，则这是一个非IE浏览器 
    if (e && e.stopPropagation)
    //因此它支持W3C的stopPropagation()方法 
        e.stopPropagation();
    else
    //否则，我们需要使用IE的方式来取消事件冒泡 
        window.event.cancelBubble = true;
}


//当按键后,不希望按键继续传递给如HTML文本框对象时,可以取消返回值.即停止默认事件默认行为.
//阻止浏览器的默认行为 
function stopDefault(e) {
    //阻止默认浏览器动作(W3C) 
    if (e && e.preventDefault)
        e.preventDefault();
    //IE中阻止函数器默认动作的方式 
    else
        window.event.returnValue = false;
    return false;
}

//验证是否为正数
function validatePosi(num) {
    var reg = /^\d+(?=\.{0,1}\d+$|$)/
    if (reg.test(num)) return true;
    return false;
}

//隐藏上下文菜单
function hideContextmenu() {
    return false;
}

//屏蔽元素的浏览器默认上下文菜单
function shieldDefContextmenu(elementId) {
    locateElement(elementId).oncontextmenu = hideContextmenu;
}

//重新定义图片大小
function ResizeImg(ImgD, xx, yy) {
    var image = new Image();
    image.src = ImgD.src;
    if (image.width > 0 && image.height > 0) {
        if (image.width / image.height >= xx / yy) {
            if (image.width > xx) {
                ImgD.width = xx;
                ImgD.height = (image.height * xx) / image.width;
            }
            else {
                ImgD.width = image.width;
                ImgD.height = image.height;
            }

        }
        else {
            if (image.height > yy) {
                ImgD.height = yy;
                ImgD.width = (image.width * yy) / image.height;
            }
            else {
                ImgD.width = image.width;
                ImgD.height = image.height;
            }
        }
    }
}

//json转字符串
function Obj2Str(o) {
    if (o == undefined) {
        return "";
    }
    var r = [];
    if (typeof o == "string") return "\'" + o.replace(/([\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\'";
    if (typeof o == "object") {
        if (!o.sort) {
            for (var i in o)
                r.push("\"" + i + "\":" + Obj2Str(o[i]));
            if (!!document.all && !/^\n?function\s*toString\(\)\s*\{\n?\s*\[native code\]\n?\s*\}\n?\s*$/.test(o.toString)) {
                r.push("toString:" + o.toString.toString());
            }
            r = "{" + r.join() + "}"
        } else {
            for (var i = 0; i < o.length; i++)
                r.push(Obj2Str(o[i]))
            r = "[" + r.join() + "]";
        }
        return r;
    }
    var str = o.toString().replace(/\"\:/g, '":""');
    return str;
}

//加载js文件
function loadScript(url, callback) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = url;
    document.getElementsByTagName("head")[0].appendChild(script);
    if (typeof (callback) == "function") {  //如果callback为函数
        if (script.readyState) {
            script.onreadystatechange = function() {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            }
        } else {
            script.onload = function() {
                callback();
            }
        }
    }
}