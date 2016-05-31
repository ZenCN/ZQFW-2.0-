App.Models = App.Models || {};

App.Models.ReportTitle = function(obj) {

    return $.extend({
        PageNO: undefined,
        ORD_Code: undefined,
        RPTType_Code: undefined,
        StatisticalCycType: undefined,
        UnitName: undefined,
        UnitCode: undefined,
        StartDateTime: undefined,
        EndDateTime: undefined,
        UnitPrincipal: undefined,
        StatisticsPrincipal: undefined,
        WriterName: undefined,
        WriterTime: App.Tools.Date.GetToday(),
        SendTime: undefined,
        Remark: undefined,
        Del: 0,
        State: undefined,
        SourceType: undefined,
        ReceiveTime: undefined,
        AssociatedPageNO: undefined,
        OperateReportNO: undefined,
        DisasterTypeName: undefined,
        DisasterDescribe: undefined,
        DisasterSummary: undefined,
        ExceptPageNo: undefined,
        RBType: undefined,
        LastUpdateTime: undefined,
        ReceiveState: undefined,
        CloudPageNO: undefined,
        CopyPageNO: 0,
        SendOperType: -1
    }, obj);
};