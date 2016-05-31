String.prototype.replaceAll = function(s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2); //g全局     
};

String.prototype.replaceAll2Excep = function(s1, s2) {
    var temp = this;
    while (temp.indexOf(s1) != -1) {
        temp = temp.replace(s1, s2);
    }
    return temp;
};

String.prototype.Contains = function(str) {
    if (typeof(str) == "string" && this.indexOf(str) >= 0) {
        return true;
    } else {
        return false;
    }
};

Number.prototype.toFixed = function(exponent) {
    if (exponent) {
        var result = (parseInt(this * Math.pow(10, exponent) + 0.5) / Math.pow(10, exponent)).toString();
        var count = 0;
        if (result.indexOf(".") > 0) {
            count = exponent - result.split(".")[1].length;
        } else {
            count = exponent;
            result += ".";
        }

        for (count; count > 0; count--) {
            result += "0";
        }

        return result;
    } else {
        return parseInt(this);
    }
}