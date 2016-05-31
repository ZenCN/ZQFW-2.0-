Array.prototype.In_Array = function (e) {   //e可为string、number类型或等于null、undefined
    var exist = false;
    if (typeof e == "string") {
        var s = String.fromCharCode(2);
        exist = new RegExp(s + e + s).test(s + this.join(s) + s);
    } else {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == e) {
                exist = true;
                break;
            }
        }
    }

    return exist;
};

Array.prototype.RemoveBy = function(key, value) {
    var arr = [];
    angular.forEach(this, function(obj) {
        if (obj[key] != value) {
            arr.push(obj);
        }
    });
    
    return arr;
};

Array.prototype.RemoveAttr = function(arr,fn) {
    angular.forEach(this, function (obj) {
        if ($.isFunction(fn)) {
            fn(obj);
        }
        angular.forEach(arr, function(key) {
            delete obj[key];
        });
    });

    return this;
};

Array.prototype.MapBy = function (key) {
    var arr = [];
    angular.forEach(this, function (obj) {
        if (obj[key] != null && obj[key] != undefined) {
            arr.push(obj[key]);
        }
    });
    
    return arr;
};

Array.prototype.UnitCodeConvert = function (limit) {
    if (limit != 5) {
        var length = 0;
        var partialCode = "";
        switch (limit) {
            case 3:
                length = 4;
                partialCode = "0000";
                break;
            case 4:
                length = 6;
                partialCode = "00";
                break;
        }
        angular.forEach(this, function(obj) {
            obj.UnitCode = obj.UnitCode.slice(0, length) + partialCode;
        });
    }
    
    return this;
};

Array.prototype.Find = function(key, value, keyForReturn, remove) {
    var result = {};
    var arr = [];
    var realValue = undefined;
    var type = "string";
    var found = false;
    var _this = this;

    if (key.indexOf(".") > 0) {
        arr = key.split(".");
    }

    if (Number(value) >= 0) {
        type = "number";
        value = Number(value);
    }

    angular.forEach(this, function(obj, i) {

        if (arr.length > 0) {
            realValue = obj[arr[0]][arr[1]];
        } else {
            realValue = obj[key];
        }

        if (type == "number") {
            found = Number(realValue) == value;
        } else {
            if (value.indexOf("%") >= 0) {
                found = realValue.indexOf(value.replaceAll("%", "")) >= 0 ? true : false;
            } else {
                found = realValue == value;
            }
        }

        if (found) {
            if (keyForReturn) {
                result = obj[keyForReturn];
            } else {
                result = obj;
                if (remove) {
                    _this.splice(i, 1);
                }
            }

            return false;
        }
    });

    return result;
};

Array.prototype.SomeBy = function(key, value) {
    var exist = false;
    var arr = [];
    var realValue = undefined;
    var type = undefined;
    
    if (key.indexOf(".") > 0) {
        arr = key.split(".");
    }

    if (Number(value) >= 0) {
        type = "number";
        value = Number(value);
    }

    angular.forEach(this, function (obj) {
        
        if (arr.length > 0) {
            realValue = obj[arr[0]][arr[1]];
        } else {
            realValue = obj[key];
        }

        if (type == "number") {
            realValue = Number(realValue);
        }

        if (realValue == value) {
            exist = true;
            
            return false;
        }
        
    });

    return exist;
};

Array.prototype.BubbleSort = function (attr, order) {
    order = order ? order : "desc";
    if (this.length > 0) {
        var tmp = undefined;
        if (attr) { //BubbleSort an Object Array
            if (this.length > 1) { //两个及两个以上才能冒泡排序
                for (var i = 0; i < this.length; i++) { //内层循环，找到第i大的元素，并将其和第元i个素交换
                    for (var j = i; j < this.length; j++) {
                        this[i][attr] = Number(this[i][attr]) > 0 ? Number(this[i][attr]) : 0;
                        this[j][attr] = Number(this[j][attr]) > 0 ? Number(this[j][attr]) : 0;
                        if (order == "desc") {
                            if (this[i][attr] < this[j][attr]) {
                                tmp = this[i]; //交换两个元素的位置
                                this[i] = this[j];
                                this[j] = tmp;
                            }
                        } else {
                            if (this[i][attr] > this[j][attr]) {
                                tmp = this[i]; //交换两个元素的位置
                                this[i] = this[j];
                                this[j] = tmp;
                            }
                        }
                    }
                }
                return this;
            } else {
                return this;
            }
        }
    } else {
        return [];
    }
};

Array.prototype.Every = function(attr) {
    angular.forEach(this, function(obj) {
        angular.extend(obj, attr);
    });
};

Array.prototype.Clear = function() {
    this.length = 0;
};

Array.prototype.InsertAt = function(index, val) {
    this.splice(index, 0, val);
};

Array.prototype.Last = function () {
    if (this.length > 0) {
        return this[this.length - 1];
    } else {
        return null;
    }
};

Array.prototype.Next = function (key, value) {
    if (isNaN(key)) {
        var obj = this;
        angular.forEach(this, function(object, i) {
            if (object[key] == value) {
                obj = obj[i + 1]; //不存在返回undefined
                return false;
            }
        });
        return obj;
    } else {
        return this[Number(key) + 1];
    }
};

Array.prototype.Slice = function(startindex, count) {
    startindex = startindex ? startindex : 0;
    count = count && count < this.length ? count : this.length;
    var arr = [];
    for (var i = startindex; i < startindex + count; i++) {
        arr.push(this[i]);
    }

    return arr;
};

Array.prototype.New = function(attrs) {
    var arr = [], obj;
    attrs = attrs.split(",");

    if (attrs.length) {
        for (var j = 0; j < this.length; j++) {
            obj = {};
            for (var i = 0; i < attrs.length; i++) {
                obj[attrs[i]] = this[attrs[i]];
            }
            arr.push(obj);
        }
    }

    return this;
};

Array.prototype.GetAll = function (attr) {
    var str = "";
    
    for (var j = 0; j < this.length; j++) {
        str += this[j][attr] + ",";
    }

    str = str.substr(0, str.length - 1);
    return str;
};