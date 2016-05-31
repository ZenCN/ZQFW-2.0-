window.App = window.App || {};

App.DisasterReviewModel = {
    StartDateTime: undefined,   //开始时间
    EndDateTime: undefined,     //结束时间
    DisasterTypeName: undefined, //气象因素
    JYQDDX: undefined,      //全省出现XX降雨强度
    JYFW: undefined,        //降雨范围
    PJYL: 0,    //平均雨量
    CPJYL: 0,   //超过降雨量
    CPJYLZS: 0,
    FGMJ: 0,
    GCZDDZYL: 0,
    RZDDZYL: 0,
    YQZDDZYL: 0,
    JMC: undefined,
    HMC: undefined,
    CLSZS: 0,
    CBZZS: 0,
    CJJZS: 0,
    ZQSM: undefined,
    ZQJZRQ: undefined,
    SZDSMC: undefined,
    SZFWDSS: 0,
    SZFWX: 0,
    SZFWZ: 0,
    SZRK: 0,
    SYCS: 0,
    SWRK: 0,
    ZYRYSWRK: 0,
    SZRKR: 0,
    ZYRK: 0,
    DTFW: 0,
    ZJJJZSS: 0,
    SHMJXJ: 0,
    CZMJXJ: 0,
    JSMJXJ: 0,
    YZJCLS: 0,
    SWDSC: 0,
    SCYZMJ: 0,
    SCYZSL: 0,
    NLMYZJJJSS: 0,
    TCGKQY: 0,
    TLZD: 0,
    GLZD: 0,
    JCGKGT: 0,
    GDZD: 0,
    TXZD: 0,
    JJZWSS: 0,  //经济作物损失
    SCYZSS: 0,  //水产养殖损失
    NYZJJJSS:0,  //农业直接经济损失
    GJYSZJJJSS: 0,  //工业交通业直接经济损失
    SHSKD: 0,
    SKKBD: 0,
    SHDFCS: 0,
    SHDFCD: 0,
    SHHAC: 0,
    SHSZ: 0,
    CHTB: 0,
    SHGGSS: 0,
    SHSWCZ: 0,
    SHJDBZ: 0,
    SHSDZ: 0,
    SHJDJ: 0, //损坏机电井
    SLSSZJJJSS: 0,  //水利工程水毁直接经济损失
    SYCSS: 0,
    YMFWMJ: 0,
    YMFWBL: 0,
    GCYMLS: 0,
    GCHSWRKR: 0,
    GCJJZYRK: 0,
    ZYZJZDSS: 0,
    SMXGS: 0,
    SMXGD: 0,
    SMXGQ: 0,
    SMXJT: 0,
    JZWSYFW: 0,
    JZWSYDX: 0,
    CQZJJJSS: 0,
    ZQTD: undefined,
    ZQDQ: undefined,
    KZJZDX: undefined,
    ZYRKSWRK: undefined,
    GCHSWKRK: 0, //被水围困人数
    SimpleUnitName: undefined,    //所有的单位
    Limit: undefined,   //单位级别
    UnitName: undefined,     //单位名称

    /*******************HL014*************************/
    QXHJ:0, //抢险人数(合计)
    SBJX:0, //投入动力（机械设备） 
    WZBZD:0, //编织袋 
    WZBZB:0, //编织布 
    WZSSL:0, //砂石料 
    WZMC:0, //木材
    WZGC:0,  //钢材
    WZY:0, //抗灾用油
    WZD:0, //用电 
    WZZXH:0, //总物资消耗折算资金 
    XYJYGD:0, //减淹面积(减淹耕地) 
    XYJMLSJS:0, //避免粮食减收
    XYJSSZRK:0, //减少受灾人口 
    XYJJQZ:0, //解救洪水围困群众 
    XYJZJJXY: 0  //减灾经济效益
};