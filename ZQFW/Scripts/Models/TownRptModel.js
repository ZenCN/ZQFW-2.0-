App.Models = App.Models || {};

App.Models.Report = function() {

    return {
        Affix: [],
        HL011: [App.Models.HL011()],
        HL012: [],
        ReportTitle: App.Models.ReportTitle(),
        DeletedAffix: []
    };
};

App.Models.HL011 = function() {

    return {
        SZFWZ: undefined,
        SHMJXJ: undefined,
        SWRK: undefined,
        SZRK: undefined,
        SZRKR: undefined,
        ZYRK: undefined,
        DTFW: undefined,
        ZJJJZSS: undefined,
        SLSSZJJJSS: undefined
    };
};

App.Models.HL012 = function() {

    return {
        Checked: false,
        SWXM: undefined,
        DataType: undefined,
        SWXB: undefined,
        SWNL: undefined,
        SWHJ: undefined,
        SWDD: undefined,
        SWSJ: undefined,
        DeathReason: undefined,
        DeathReasonCode: undefined
    };
};

App.Models.ReportTitle = function() {
    
    return {
        StartDateTime: undefined,
        EndDateTime: undefined,
        PageNO: undefined,
        Remark: undefined,
        State: undefined
    };
};