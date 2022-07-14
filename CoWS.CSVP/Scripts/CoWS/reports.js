function btnExport_onClick() {
    selectReportByType();
}

function selectReportByType() {
    // Which Area is Selected
    switch ($("#ddlReportTypes").val()) {
        case "1": // Child Support
            printChildSupportReports();
            break;
        case "2": // Vouchers Payable
            printVouchersPayableReports();
            break;
        case "3": // Payroll
            printPayrollReports();
            break;
    }
}

function printChildSupportReports() {
    var reportParams = [];
    var reportName = $("#ddlReportNames").data("kendoDropDownList").text();

    // Child Support Reports
    switch (reportName) {
        case "Preliminary Summary":
            // Parameters
            reportParams.push({ Name: 'BatchType', Value: 'CS' });
            reportParams.push({ Name: 'CreateDate', Value: dpReportDate.value });
            break;
        case "Post Summary":
            // Parameters
            reportParams.push({ Name: 'BatchType', Value: 'CS' });
            reportParams.push({ Name: 'CreateDate', Value: dpReportDate.value });
            break;
        case "Voucher Pages":            
            // Parameters
            reportParams.push({ Name: 'BatchType', Value: 'CS' });
            reportParams.push({ Name: 'CreateDate', Value: dpReportDate.value });
            break;
    }
    // Report
    generateReport(reportName, reportParams);
}

function printVoucherPayableReports() {
    var reportParams = [];
    var reportName = $("#ddlReportNames").data("kendoDropDownList").text();

    // Vouchers Payable Reports
    switch (reportName) {
        case "Preliminary Summary":
            // Parameters
            reportParams.push({ Name: 'BatchType', Value: 'PV' });
            reportParams.push({ Name: 'CreateDate', Value: dpReportDate.value });
            break;
        case "Post Summary":
            // Parameters
            reportParams.push({ Name: 'BatchType', Value: 'PV' });
            reportParams.push({ Name: 'CreateDate', Value: dpReportDate.value });
            break;
        case "Voucher Pages":
            // Parameters
            reportParams.push({ Name: 'BatchType', Value: 'PV' });
            reportParams.push({ Name: 'ReportDate', Value: dpReportDate.value });
            break;
    }
    // Report
    generateReport(reportName, reportParams);
}

function printPayrollReports() {
    var reportParams = [];
    var reportName = $("#ddlReportNames").data("kendoDropDownList").text();

    // Payroll Reports
    switch (reportName) {
        case "Post Summary":
            // Parameters
            reportParams.push({ Name: 'Parameter1Name', Value: 'Parameter1Value' });
            reportParams.push({ Name: 'CreateDate', Value: dpReportDate.value });
            break;
    }
    // Report
    generateReport(reportName, reportParams);
}

function ddlReportTypes_onChange() {
    $("#ddlReportNames").data("kendoDropDownList").trigger("change");
}

function ddlReportNames_onChange() {

}

function updateReportFilters(intReportType, intReportName) {

}

function setFilterDisplayMode(intMode) {


}

function hideAllReportFilters() {

}

function filterReports() {
    return {
        ReportType: $("#ddlReportTypes").val()
    };
}