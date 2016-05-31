/*Object.defineProperty(Object.prototype, '_clone', {
    value: function () {
        var objClone;
        if (this.constructor == Object) {
            objClone = new this.constructor();
        } else {
            objClone = new this.constructor(this.valueOf());
        }
        for (var key in this) {
            if (objClone[key] != this[key]) {
                if (typeof (this[key]) == 'object') {
                    objClone[key] = this[key].Clone();
                } else {
                    objClone[key] = this[key];
                }
            }
        }
        objClone.toString = this.toString;
        objClone.valueOf = this.valueOf;
        return objClone;
    },
    writable: true
});*/


Object.defineProperty(Object.prototype, 'isEmpty', {
    value: function () {
        var isEmpty = true;
        for (var key in this) {
            if (this[key] != undefined) {
                isEmpty = false;
            }
        }
        
        return isEmpty;
    },
    writable: true
});
