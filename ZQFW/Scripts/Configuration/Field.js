App.Config = App.Config || {};

App.Config.Field = {
    HL01: {
        Common: {
            Map: undefined,   //HL011: { "0-0": "SZFWX", ...}, HL012: {...}, HL013: {...}, HL014: {...}
            Model: undefined,   //HL011: { SZFWX: undefined, ...}, HL012: {...}, HL013: {...}, HL014: {...}
            HL011: [
                {
                    SZFWX: {
                        Position: "0-0"
                    },
                    SZFWZ: {
                        Position: "0-1"
                    },
                    SZRK: {
                        Position: "0-2"
                    },
                    SYCS: {
                        Position: "0-3"
                    },
                    DTFW: {
                        Position: "0-4"
                    },
                    SWRK: {
                        Position: "0-5"
                    },
                    SZRKR: {
                        Position: "0-6"
                    },
                    ZYRK: {
                        Position: "0-7"
                    },
                    ZJJJZSS: {
                        Position: "0-8"
                    }
                },
                {
                    SHMJXJ: {
                        Position: "1-0"
                    },
                    SHMJLS: {
                        Position: "1-1"
                    },
                    CZMJXJ: {
                        Position: "1-2"
                    },
                    CZMJLS: {
                        Position: "1-3"
                    },
                    JSMJXJ: {
                        Position: "1-4"
                    },
                    JSMJLS: {
                        Position: "1-5"
                    },
                    YZJCLS: {
                        Position: "1-6"
                    },
                    JJZWSS: {
                        Position: "1-7"
                    },
                    SWDSC: {
                        Position: "1-8"
                    },
                    SCYZMJ: {
                        Position: "1-9"
                    },
                    SCYZSL: {
                        Position: "1-10"
                    },
                    NLMYZJJJSS: {
                        Position: "1-11"
                    }
                },
                {
                    TCGKQY: {
                        Position: "2-0"
                    },
                    TLZD: {
                        Position: "2-1"
                    },
                    GLZD: {
                        Position: "2-2"
                    },
                    JCGKGT: {
                        Position: "2-3"
                    },
                    GDZD: {
                        Position: "2-4"
                    },
                    TXZD: {
                        Position: "2-5"
                    },
                    GJYSZJJJSS: {
                        Position: "2-6"
                    },
                },
                {
                    SHSKD: {
                        Position: "3-0"
                    },
                    SHSKX: {
                        Position: "3-1"
                    },
                    SKKBD: {
                        Position: "3-2"
                    },
                    SKKBX1: {
                        Position: "3-3"
                    },
                    SKKBX2: {
                        Position: "3-4"
                    },
                    SHDFCS: {
                        Position: "3-5"
                    },
                    SHDFCD: {
                        Position: "3-6"
                    },
                    DFJKCS: {
                        Position: "3-7"
                    },
                    DFJKCD: {
                        Position: "3-8"
                    },
                    SHHAC: {
                        Position: "3-9"
                    },
                    SHSZ: {
                        Position: "3-10"
                    },
                    CHTB: {
                        Position: "3-11"
                    },
                    SHGGSS: {
                        Position: "3-12"
                    },
                    SHSWCZ: {
                        Position: "3-13"
                    },
                    SHJDJ: {
                        Position: "3-14"
                    },
                    SHJDBZ: {
                        Position: "3-15"
                    },
                    SHSDZ: {
                        Position: "3-16"
                    },
                    SLSSZJJJSS: {
                        Position: "3-17"
                    }
                }
            ],
            HL012: {
                UnitCode: {
                    Position: "4-0"
                },
                DataType: {
                    Position: "4-1"
                },
                SWXM: {
                    Position: "4-2"
                },
                SWXB: {
                    Position: "4-3"
                },
                SWNL: {
                    Position: "4-4"
                },
                SWHJ: {
                    Position: "4-5"
                },
                SWSJ: {
                    Position: "4-6"
                },
                SWDD: {
                    Position: "4-7"
                },
                DeathReason: {
                    Position: "4-8"
                },
                BZ: {
                    Position: "4-9"
                },
                RiverCode: {
                    Position: "4-10"
                },
            },
            HL013: {
                UnitCode: {
                    Position: "5-0"
                },
                CSMC: {
                    Position: "5-1"
                },
                YMFWMJ: {
                    Position: "5-2"
                },
                YMFWBL: {
                    Position: "5-3"
                },
                SZRK: {
                    Position: "5-4"
                },
                SWRK: {
                    Position: "5-5"
                },
                GCJSSJ: {
                    Position: "5-6"
                },
                GCYMLS: {
                    Position: "5-7"
                },
                GCLJJYL: {
                    Position: "5-8"
                },
                GCHSWKRK: {
                    Position: "5-9"
                },                
                GCJJZYRK: {
                    Position: "5-10"
                },
                ZYZJZDSS: {
                    Position: "5-11"
                },
                SMXGS: {
                    Position: "5-12"
                },
                SMXGD: {
                    Position: "5-13"
                },
                SMXGQ: {
                    Position: "5-14"
                },
                SMXJT: {
                    Position: "5-15"
                },
                JZWSYFW: {
                    Position: "5-16"
                },
                JZWSYDX: {
                    Position: "5-17"
                },
                CQZJJJSS: {
                    Position: "5-18"
                },
                RiverCode: {
                    Position: "5-19"
                }
            },
            HL014: [
                {
                    WZBZD: {
                        Position: "6-0"
                    },
                    WZBZB: {
                        Position: "6-1"
                    },
                    WZDSSS: {
                        Position: "6-2"
                    },
                    WZSSL: {
                        Position: "6-3"
                    },
                    WZMC: {
                        Position: "6-4"
                    },
                    WZGC: {
                        Position: "6-5"
                    },
                    WZJSY: {
                        Position: "6-6"
                    },
                    WZY: {
                        Position: "6-7"
                    },
                    WZD: {
                        Position: "6-8"
                    },
                    WZQT: {
                        Position: "6-9"
                    },
                    WZZXH: {
                        Position: "6-10"
                    },
                    QXHJ: {
                        Position: "6-11"
                    },
                    QXBDGB: {
                        Position: "6-12"
                    },
                    QXDFRY: {
                        Position: "6-13"
                    },
                    QXFXJD: {
                        Position: "6-14"
                    },
                    SBQXZ: {
                        Position: "6-15"
                    },
                    SBYS: {
                        Position: "6-16"
                    },
                    SBJX: {
                        Position: "6-17"
                    }
                },
                {
                    ZJXJ: {
                        Position: "7-0"
                    },
                    ZJZY: {
                        Position: "7-1"
                    },
                    ZJSJ: {
                        Position: "7-2"
                    },
                    ZJSJYS: {
                        Position: "7-3"
                    },
                    ZJQZ: {
                        Position: "7-4"
                    },
                    XYJYGD: {
                        Position: "7-5"
                    },
                    XYJMLSJS: {
                        Position: "7-6"
                    },
                    XYJSSZRK: {
                        Position: "7-7"
                    },
                    XYJJQZ: {
                        Position: "7-8"
                    },
                    XYJMSWC: {
                        Position: "7-9"
                    },
                    XYJMSWR: {
                        Position: "7-10"
                    },
                    XYZYSH: {
                        Position: "7-11"
                    },
                    XYZYTF: {
                        Position: "7-12"
                    },
                    XYZYQT: {
                        Position: "7-13"
                    },
                    XYBMSY: {
                        Position: "7-14"
                    },
                    XYJZJJXY: {
                        Position: "7-15"
                    }
                }
            ],
            SSB: {
                HL011: {
                    SZFWX: {
                        Position: "0-0"
                    },
                    SZFWZ: {
                        Position: "0-1"
                    },
                    SHMJXJ: {
                        Position: "0-2"
                    },
                    SZRK: {
                        Position: "0-3"
                    },
                    SWRK: {
                        Position: "0-4"
                    },
                    SZRKR: {
                        Position: "0-5"
                    },
                    ZYRK: {
                        Position: "0-6"
                    },
                    DTFW: {
                        Position: "0-7"
                    },
                    ZJJJZSS: {
                        Position: "0-8"
                    },
                    SLSSZJJJSS: {
                        Position: "0-9"
                    }
                },
                HL012: {
                    UnitCode: {
                        Position: "1-0"
                    },
                    DataType: {
                        Position: "1-1"
                    },
                    SWXM: {
                        Position: "1-2"
                    },
                    SWXB: {
                        Position: "1-3"
                    },
                    SWNL: {
                        Position: "1-4"
                    },
                    SWHJ: {
                        Position: "1-5"
                    },
                    SWSJ: {
                        Position: "1-6"
                    },
                    SWDD: {
                        Position: "1-7"
                    },
                    DeathReason: {
                        Position: "1-8"
                    },
                    BZ: {
                        Position: "1-9"
                    },
                    RiverCode: {
                        Position: "1-10"
                    }
                }
            }
        }
    },
    Fn: {
        GetMap: function(namespace) { //"HL01.33"
            var obj, tmp;
            var name = namespace.split(".");

            if (name.length > 2) { //HL01.33.HL011
                if (angular.isObject(App.Config.Field[name[0]][name[1]].Map) && angular.isObject(App.Config.Field[name[0]][name[1]].Map[name[2]])) {
                    return App.Config.Field[name[0]][name[1]].Map[name[2]];
                } else {
                    obj = {};
                    tmp = App.Config.Field[name[0]][name[1]][name[2]];

                    if (angular.isArray(tmp)) {
                        $.each(tmp, function() {
                            $.each(this, function(field) {
                                obj[this.Position] = field;
                            });
                        });
                    } else if (angular.isObject(tmp)) {
                        $.each(tmp, function(key) {
                            if (!obj[key]) {
                                obj[key] = {};
                            }
                            $.each(this, function(field) {
                                obj[key][this.Position] = field;
                            });
                        });
                    } else {
                        $.each(tmp, function(field) {
                            obj[this.Position] = field;
                        });
                    }

                    App.Config.Field[name[0]][name[1]].Map = App.Config.Field[name[0]][name[1]].Map || {};
                    App.Config.Field[name[0]][name[1]].Map[name[2]] = obj;

                    return obj;
                }
            } else {
                App.Config.Field[name[0]][name[1]].Map = App.Config.Field[name[0]][name[1]].Map || {};   //必须全部重写，有些没有

                angular.forEach(["HL011", "HL012", "HL013", "HL014"], function(key) {
                    if (!angular.isObject(App.Config.Field[name[0]][name[1]].Map[key])) {
                        obj = {};
                        tmp = App.Config.Field[name[0]][name[1]][key];

                        if ($.isArray(tmp)) {
                            $.each(tmp, function() {
                                $.each(this, function(field) {
                                    obj[this.Position] = field;
                                });
                            });
                        } else {
                            $.each(tmp, function(field) {
                                obj[this.Position] = field;
                            });
                        }

                        App.Config.Field[name[0]][name[1]].Map[key] = obj;
                    }
                });

                return App.Config.Field[name[0]][name[1]].Map;
            }
        },
        GetModel: function(name) {  //HL01.33.HL011
            var obj , tmp;
            name = name.split(".");
            App.Config.Field[name[0]][name[1]].Model = App.Config.Field[name[0]][name[1]].Model || {};

            if (!angular.isObject(App.Config.Field[name[0]][name[1]].Model[name[2]])) {
                obj = {};
                tmp = App.Config.Field[name[0]][name[1]][name[2]];

                if ($.isArray(tmp)) {
                    $.each(tmp, function() {
                        $.each(this, function(field) {
                            obj[field] = undefined;
                        });
                    });
                } else {
                    $.each(tmp, function(field) {
                        obj[field] = undefined;
                    });
                }

                App.Config.Field[name[0]][name[1]].Model[[name[2]]] = obj;
            }

            return angular.copy(App.Config.Field[name[0]][name[1]].Model[[name[2]]]);
        },
        FieldIndex: function(name) {   //HL01.HL011.SZFWX
            var index = -1;
            name = name.split(".");

            $.each(App.Config.Field.Fn.GetMap(name[0] + "." + $scope.SysUserCode + "." + name[1]), function(position, field) {
                if (field == name[2]) {
                    index = Number(position.split("-")[0]);
                    return false;
                }
            });

            return index;
        }
    }
};

switch ($.cookie("unitcode").substr(0, 2)) {
case "33":
    App.Config.Field.HL01["33"] = {
        Map: undefined,
        Model: undefined,
        ReportTitle: {
            Remark: {
                Length: 127
            }
        },
        HL011: [
            App.Config.Field.HL01.Common.HL011[0],
            App.Config.Field.HL01.Common.HL011[1],
            App.Config.Field.HL01.Common.HL011[2],
            {
                SHSKD: {
                    Position: "3-0"
                },
                SHSKX: {
                    Position: "3-1"
                },
                SKKBD: {
                    Position: "3-2"
                },
                SKKBX1: {
                    Position: "3-3"
                },
                SKKBX2: {
                    Position: "3-4"
                },
                SHDFCS: {
                    Position: "3-5"
                },
                SHDFCD: {
                    Position: "3-6"
                },
                DFJKCS: {
                    Position: "3-7"
                },
                DFJKCD: {
                    Position: "3-8"
                },
                SHHTCS: {
                    Position: "3-9"
                },
                SHHTCD: {
                    Position: "3-10"
                },
                HTJKCS: {
                    Position: "3-11"
                },
                HTJKCD: {
                    Position: "3-12"
                },
                SHHAC: {
                    Position: "3-13"
                },
                SHSZ: {
                    Position: "3-14"
                },
                CHTB: {
                    Position: "3-15"
                },
                SHGGSS: {
                    Position: "3-16"
                },
                SHSWCZ: {
                    Position: "3-17"
                },
                SHJDJ: {
                    Position: "3-18"
                },
                SHJDBZ: {
                    Position: "3-19"
                },
                SHSDZ: {
                    Position: "3-20"
                },
                SLSSZJJJSS: {
                    Position: "3-21"
                }
            }
        ],
        HL012: App.Config.Field.HL01.Common.HL012,
        HL013: App.Config.Field.HL01.Common.HL013,
        HL014: App.Config.Field.HL01.Common.HL014
    };
    break;
default:
    App.Config.Field.HL01[$.cookie("unitcode").substr(0, 2)] = App.Config.Field.HL01.Common;
    break;
}