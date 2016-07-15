Date.prototype.Format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

App.Tools = {
    Date: {
        GetToday: function (format, date) {
            var result = "";
            date = date ? new Date(date) : new Date();
            var year = date.getFullYear();
            var month = (date.getMonth() + 1);
            var day = date.getDate();
            var time = undefined;
            month = month < 10 ? "0" + month : month;
            day = day < 10 ? "0" + day : day;

            switch (format) {
                case "dd日":
                    result = day + "日";
                    break;
                case "MM月dd日":
                    result = month + "月" + day + "日";
                    break;
                case "yyyy年MM月dd日":
                    result = year + "年" + month + "月" + day + "日";
                    break;
                case "yyyy-M-dd HH":
                    result = year + "-" + month + "-" + day + " " + date.getHours();
                    break;
                case "yyyy-MM-dd HH:mm:ss":
                    time = date.getMinutes();
                    time = time < 10 ? "0" + time : time;
                    result = year + "-" + month + "-" + day + " " + date.getHours() + ":" + time;
                    time = date.getSeconds();
                    time = time < 10 ? "0" + time : time;
                    result += ":" + time;
                    break;
                default:
                    result = year + "-" + month + "-" + day;
                    break;
            }

            return result;
        },
        Format: function(str, split) {
            split = split ? split : "-";
            var arr = str.split(split);
            arr[1] = Number(arr[1]);
            if (arr[1] < 10) {
                arr[1] = "0" + arr[1];
            }
            arr[2] = Number(arr[2]);
            if (arr[2] < 10) {
                arr[2] = "0" + arr[2];
            }

            return arr[0] + "年" + arr[1] + "月" + arr[2] + "日";
        },
        GetOneDay: function(days, date) {
            var d, m, r, t = "en";

            if (typeof date == "string") {
                if (date.indexOf("年") > 0 && date.indexOf("月") > 0 && date.indexOf("日") > 0) {
                    t = "cn";
                    date = date.replace("年", "-").replace("月", "-").replace("日", "");
                }
                d = new Date(date);
            } else {
                d = new Date();
            }

            d = d.valueOf();
            d = d + days * 24 * 60 * 60 * 1000;
            d = new Date(d);
            m = d.getMonth() + 1;
            m = m < 10 ? "0" + m : m;
            r = d.getDate();
            r = r < 10 ? "0" + r : r;

            if (t == "cn") {
                t = d.getFullYear() + "年" + m + "月" + r + "日";
            } else {
                t = d.getFullYear() + "-" + m + "-" + r;
            }

            return t;
        },
        GetDay: function (type) {
            var d = new Date();
            var y = d.getFullYear();
            var m = d.getMonth() + 1;
            var out = undefined;

            if (m < 10) {
                m = "0" + m;
            }

            switch (type) {
                case "Last":
                    d = new Date(y, m, 0);
                    out = y + "-" + m + "-" + d.getDate();
                    break;
                case "First":
                    out = y + "-" + m + "-" + "01";
            }

            return out;
        },
        UpdateModelAfter: function (callBack) {
            var arr = callBack.split(".");
            var i = arr.length;
            var model = undefined;
            if (i > 0) {
                model = $(document).scope()[arr[0]];
                i--;
                if (i > 0) {
                    model = model[arr[1]];
                }
            }
        }
    },
    CN_Name: function (enName, value) {
        var name = "未知";
        if (!isNaN(value)) {
            value = parseInt(value);
        }
        switch (enName) {
            case "SourceType":
                switch (value) {
                    case 0:
                        name = "录入";
                        break;
                    case 1:
                        name = "汇总";
                        break;
                    case 2:
                        name = "累计";
                        break;
                }
                break;
            case "ORD_Code":
                switch (value) {
                    case "HL01":
                        name = "洪涝";
                        break;
                    case "NP01":
                    case "HP01":
                        name = "蓄水";
                        break;
                }
                break;
        }
        return name;
    },
    Calculator: {
        Addition: function (arg1, arg2) {
            var r1,
                r2,
                m,
                n,
                result;
            arg1 = arg1 == undefined ? 0 : arg1;
            arg2 = arg2 == undefined ? 0 : arg2;
            try {
                r1 = arg1.toString().split(".")[1].length;
            } catch (e) {
                r1 = 0;
            }
            try {
                r2 = arg2.toString().split(".")[1].length;
            } catch (e) {
                r2 = 0;
            }
            m = Math.pow(10, Math.max(r1, r2));
            n = (r1 >= r2) ? r1 : r2;
            result = ((arg1 * m + arg2 * m) / m).toFixed(n);
            return Number(result) <= 0 ? undefined : result;
        },
        Subtraction: function (arg1, arg2) {
            var r1,
                r2,
                m,
                n,
                result;
            arg1 = arg1 == undefined ? 0 : arg1;
            arg2 = arg2 == undefined ? 0 : arg2;
            try {
                r1 = arg1.toString().split(".")[1].length;
            } catch (e) {
                r1 = 0;
            }
            try {
                r2 = arg2.toString().split(".")[1].length;
            } catch (e) {
                r2 = 0;
            }
            m = Math.pow(10, Math.max(r1, r2));
            n = (r1 >= r2) ? r1 : r2;
            result = ((arg1 * m - arg2 * m) / m).toFixed(n);
            return Number(result) <= 0 ? undefined : result;
        },
        /*        Multiplication: function(arg1, arg2) //乘法
        {
        var m = 0,
        s1 = arg1.toString(),
        s2 = arg2.toString();
        try {
        m += s1.split(".")[1].length
        } catch(e) {
        }
        try {
        m += s2.split(".")[1].length
        } catch(e) {
        }
        return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m);
        },*/
        Division: function (arg1, arg2, n) //除法
        {
            arg1 = arg1 == undefined ? 0 : arg1;
            arg2 = arg2 == undefined ? 0 : arg2;
            if (arg1 == 0 || arg2 == 0) {
                return 0; //此处不能返回undefined
            } else {
                var t1 = 0,
                    t2 = 0,
                    r1,
                    r2;
                try {
                    t1 = arg1.toString().split(".")[1].length;
                } catch (e) {
                }
                try {
                    t2 = arg2.toString().split(".")[1].length;
                } catch (e) {
                }
                with (Math) {
                    r1 = Number(arg1.toString().replace(".", ""));
                    r2 = Number(arg2.toString().replace(".", ""));
                    n = n == undefined ? 4 : n;
                    return parseFloat(((r1 / r2) * pow(10, t2 - t1)).toFixed(n));
                }
            }
        },
        Number: function (val, type) {  //type:true 表示仅仅转换，不用于计算，false则相反
            var result = Number(val);
            result = result > 0 ? result : (type ? undefined : 0);

            return result;
        }
    },
    Unit: {
        Info: function (obj) {
            var result = {};
            var tmp = undefined;

            if (obj.code) {
                result = {
                    Limit: undefined,
                    FullName: obj.rootName ? obj.rootName : ""
                };
                if (obj.code.slice(2) == "000000") { //省级
                    result.Limit = 2;
                } else {
                    if (obj.code.slice(4) == "0000") {
                        result.Limit = 3;
                        tmp = obj.units.Find("UnitCode", obj.code, "UnitName");
                        result.FullName += angular.isObject(tmp) ? "" : "-" + tmp;
                    } else {
                        tmp = obj.units.Find("UnitCode", obj.code.slice(0, 4) + "0000", "UnitName");
                        result.FullName += angular.isObject(tmp) ? "" : "-" + tmp;
                        if (obj.code.slice(6) == "00") {
                            result.Limit = 4;
                        } else {
                            result.Limit = 5;
                            tmp = obj.units.Find("UnitCode", obj.code.slice(0, 6) + "00", "UnitName");
                            result.FullName += angular.isObject(tmp) ? "" : "-" + tmp;
                        }
                        tmp = obj.units.Find("UnitCode", obj.code, "UnitName");
                        result.FullName += angular.isObject(tmp) ? "" : "-" + tmp;
                    }
                }
            }

            return result;
        },
        Limit: function (unitcode) {
            if (unitcode.slice(2) == "000000") { //省级
                return 2;
            } else if (unitcode.slice(4) == "0000") {
                return 3;
            } else if (unitcode.slice(6) == "00") {
                return 4;
            } else {
                return 5;
            }
        }
    },
    Convert: {
        ToChineseNumber: function (number) {
            if (isNaN(number)) {
                throw "ToChineseNumber的参数不能为字符";
            } else {
                number = number.toString();
            }

            var ary0 = ["零", "一", "二", "三", "四", "五", "六", "七", "八", "九"],
                ary1 = ["", "十", "百", "千"],
                ary2 = ["", "万", "亿", "兆"];
            var zero = "";
            var newary = "";
            var i4 = -1;
            var ary = [];

            for (var j = number.length; j >= 0; j--) {
                ary.push(number[j]);
            }
            ary = ary.join("");

            for (var i = 0; i < ary.length; i++) {
                if (i % 4 == 0) {   //首先判断万级单位，每隔四个字符就让万级单位数组索引号递增
                    i4++;
                    newary = ary2[i4] + newary;   //将万级单位存入该字符的读法中去，它肯定是放在当前字符读法的末尾，所以首先将它叠加入$r中，
                    zero = "";   //在万级单位位置的“0”肯定是不用的读的，所以设置零的读法为空

                }
                //关于0的处理与判断。
                if (ary[i] == '0') {   //如果读出的字符是“0”，执行如下判断这个“0”是否读作“零”
                    switch (i % 4) {
                        case 0:
                            break;
                        //如果位置索引能被4整除，表示它所处位置是万级单位位置，这个位置的0的读法在前面就已经设置好了，所以这里直接跳过     
                        case 1:
                        case 2:
                        case 3:
                            if (ary[i - 1] != '0') {
                                zero = "零";
                            }
                            //如果不被4整除，那么都执行这段判断代码：如果它的下一位数字（针对当前字符串来说是上一个字符，因为之前执行了反转）也是0，那么跳过，否则读作“零”
                            break;

                    }

                    newary = zero + newary;
                    zero = '';
                }
                else { //如果不是“0”
                    newary = ary0[parseInt(ary[i])] + ary1[i % 4] + newary; //就将该当字符转换成数值型,并作为数组ary0的索引号,以得到与之对应的中文读法，其后再跟上它的的一级单位（空、十、百还是千）最后再加上前面已存入的读法内容。
                }

            }

            if (newary.indexOf("零") == 0) {
                newary = newary.substr(1);
            } //处理前面的0

            if (number >= 10 && number < 20) {
                newary = newary.slice(1);
            }

            return newary;
        }
    }
};

/*App.Tools.CallBack = function() {
    this.Fns = [];
};
App.Tools.CallBack.prototype = {
    Add: function (fn) {
        this.Fns.push(fn);
        return this;
    },
    Clear: function () {
        this.Fns.splice(0);
    },
    Execute: function () {
        var length = this.Fns.length;
        for (var i = 0; i < length; i++) {
            if ($.isFunction(this.Fns[i])) {
                clearTimeout(this["Time" + i]);
                this["Time" + i] = setTimeout(this.Fns[i](), 0);
            }
        }
        this.Clear();
    }
}*/

function load_js(fileUrl) {
    var oScript, oHead = document.getElementsByTagName('HEAD').item(0);
    if (typeof fileUrl == "string") {
        oScript = document.createElement("script");
        oScript.type = "text/javascript";
        oScript.src = fileUrl;
        oHead.appendChild(oScript);
    } else {
        for (var i = 0; i < fileUrl.length; i++) {
            oScript = document.createElement("script");
            oScript.type = "text/javascript";
            oScript.src = fileUrl[i];
            oHead.appendChild(oScript);
        }
    }
};