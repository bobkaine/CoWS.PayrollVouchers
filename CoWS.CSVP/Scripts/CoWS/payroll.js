var mblnAdd = false;

function btnGetChecks_onClick() {

    var transactionDate = $("#dpCheckDate").data("kendoDatePicker").value();

    if (transactionDate === null) {
        transactionDate = GetCurrentFileTransactionDate();
        $("#dpCheckDate").data("kendoDatePicker").value(transactionDate);
    }

    var grid = $('#gridPayroll').data('kendoGrid');
    if (!TransactionDetailExists(transactionDate)) {
        ProcessGeneralLedgerFile(transactionDate);
    }
    else {
        var requestedDate = moment($("#dpCheckDate").data("kendoDatePicker").value()).format("MM/DD/YYYY");
        myAlert("Checks for '" + requestedDate + "' already exist.</br>Information retrieved from database.");
        grid.dataSource.read();
    }
}

function btnGetChecksFromDB_onClick() {
    $("#dpCheckDate").data("kendoDatePicker").value(GetLatestLedgerEntry());

    var grid = $('#gridPayroll').data('kendoGrid');
    grid.dataSource.read();
}

function btnSubmitChecks_onClick() {

    if ($("#dpCheckDate").data("kendoDatePicker").value() === null) {
        myAlert("Select a date to Post to FMS.", "Error");
        return;
    }

    kendo.ui.progress($(document.body), true);
    setTimeout(SubmitActualsBatch(), 100);

}

function ProcessGeneralLedgerFile(transactionDate) {

    var fileType = $("input[name='payrollType']:checked").val();
    var grid = $("#gridPayroll").data("kendoGrid");
    kendo.ui.progress($(document.body), true);

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Payroll/ProcessGeneralLedgerFile',
        data: JSON.stringify({
            FileType: fileType,
            TransactionDate: transactionDate
        }),
        success: function (data) {
            kendo.ui.progress($(document.body), false);
            if (data === "") {
                myAlert("Requested 'General Ledger' file not found!");
                grid.dataSource.data([]);
                SetDisplayMode(false);
                return;
            }
            else if (data.BatchType === "ZZ") {
                fileTransactionDate = data.TransactionDetails[0].TransactionDate;
                myAlert("<b>Check Date Mismatch</b>", "Date Mismatch", null, "Current 'General Ledger' file is for <b>" + moment(fileTransactionDate).format("MM/DD/YYYY") + "</b>!");
                grid.dataSource.data([]);
                SetDisplayMode(false);
                return;
            }
            grid.dataSource.read();
            SetDisplayMode(true);
        },
        error: function (e) {
            kendo.ui.progress($(document.body), false);
            console.log(e);
            SetDisplayMode(false);
        }
    });
}

function gridPayroll_onDataBound() {
    var grid = $("#gridPayroll").data("kendoGrid");
    var ds = grid.dataSource;
    var lastRow = ds._data[ds._data.length - 1];

    if (lastRow === undefined) {
        divSubmitButton.style.display = "none";
        grid.hideColumn(0);
        $(".k-grid-toolbar").hide();
        $(".k-grid-footer").hide();
        lblBatchTotal.innerHTML = "";
        lblPeriod.innerHTML = "";
        lblYear.innerHTML = "";
        //lblComments.innerHTML = "";
        $("#txtComments").css({ 'display': 'none' });
        txtComments.value = "";
        lblBatchID.innerHTML = "";
        SetDisplayMode(false);
        return;
    }
    else if (lastRow.GLID > 0) {
        divSubmitButton.style.display = "inline-block";

        lblBatchTotal.innerHTML = lastRow.BatchTotal.toLocaleString("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 });
        //lblComments.innerHTML = "<i>General Ledger: </i>" + lastRow.GLComments + ", <i>Journal</i>: " + lastRow.JournalComments;
        txtComments.value = lastRow.GLComments;
        $("#txtComments").css({ 'display': 'inline-block' });
        lblPeriod.innerHTML = lastRow.Period;
        lblYear.innerHTML = lastRow.Year;

        if (parseFloat(lastRow.BatchTotal) !== 0) {
            $("#lblBatchTotal").css({ 'color': 'red' });
        }
        else {
            $("#lblBatchTotal").css({ 'color': 'black' });
        }

        if (lastRow.BatchNumber === null) {
            lblBatchID.innerHTML = "";            
        }
        else {
            lblBatchID.innerHTML = lastRow.BatchNumber;
        }

        SetDisplayMode(true);
    }
}

function gridPayroll_onBeforeEdit(e) {
    if (e === undefined)
        return;
    if (e.model.GLID === 0) {
        mblnAdd = true;
    }
    else {
        mblnAdd = false;
    }
}

function gridPayrollDataSource_onSync() {
    var grid = $("#gridPayroll").data("kendoGrid");
    grid.dataSource.read();
}

function SubmitActualsBatch() {

    var transactionDate = $("#dpCheckDate").data("kendoDatePicker").value();
    var glType = $("input[name='payrollType']:checked").val().substr(0,1);

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Payroll/SubmitActualsBatch',
        data: JSON.stringify({
            GeneralLedgerType: glType,
            TransactionDate: transactionDate            
        }),
        success: function (data) {
            UpdateUI(data, "GL");
            kendo.ui.progress($(document.body), false);
        },
        error: function (e) {
            kendo.ui.progress($(document.body), false);
            console.log(e);           
        }
    });
}

function GetCurrentFileTransactionDate() {

    var fileType = $("input[name='payrollType']:checked").val();

    var transactionDate;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Payroll/GetCurrentFileTransactionDate',
        data: JSON.stringify({ fileType: fileType }),
        async: false,
        success: function (response) {
            transactionDate = moment(response).format("MM/DD/YYYY");
        },
        error: function (e) {
            console.log(e);
        }
    });
    return transactionDate;
}

function TransactionDetailExists(transactionDate) {

    var glType = $("input[name='payrollType']:checked").val().substr(0, 1);

    blnTDExists = true;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Payroll/TransactionDetailExists',
        data: JSON.stringify({ GLType: glType, TransactionDate: transactionDate }),
        async: false,
        success: function (response) {
            if (response.toLowerCase() === "false") {
                blnTDExists = false;
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
    return blnTDExists;
}

function GetLatestLedgerEntry() {
    var result = "";
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Payroll/GetLatestLedgerEntry',
        async: false,
        success: function (response) {
            result = moment(response).format("MM/DD/YYYY");
        },
        error: function (e) {
            console.log(e);
        }
    });
    return result;
}

function UpdateCommonValues() {
    var data = $("#gridPayroll").data("kendoGrid").dataSource._data[1];
    if (typeof data !== "undefined") {
        // Hidden Values
        $("#GLID").val(data.GLID).change();
        $("#BatchTotal").val(data.BatchTotal).change();
        $("#BatchType").val(data.BatchType).change();
        $("#UserCode").val(data.UserCode).change();
    }
}

function btnCopy_onClick() {
    var data = $("#gridPayroll").data("kendoGrid").dataSource._data[1];
    if (typeof data !== "undefined") {
        $("#dpTransactionDateAdd").data("kendoDatePicker").value(moment(data.TransactionDate).format("MM/DD/YYYY"));
        $("#dpTransactionDateAdd").data("kendoDatePicker").trigger("change");
        $("#Comments").val(data.Comments).change();
        $("#Fund").val(data.Fund).change();
        $("#ResponsibilityCode").val(data.ResponsibilityCode).change();
        $("#ObjectCode").val(data.ObjectCode).change();
        $("#ProgramCode").val(data.ProgramCode).change();        
    }
}   

function TransactionDate() {
    return { TransactionDate: $("#dpCheckDate").data("kendoDatePicker").value() };
}

function SetDisplayMode(hasData) {

    var grid = $("#gridPayroll").data("kendoGrid");
    if (hasData) {
        $("#divCheckDate").css({ 'width': '1355px' });
        $("#divPayrollGrid").css({ 'max-width': '1355px' });
        $("#divPayrollGrid").css({ 'width': '1355px' });
        $("#divPayrollHeader").css({ 'width': '1355px' });
        grid.showColumn(0);
        $(".k-grid-toolbar").show();
    }
    else {
        $("#divCheckDate").css({ 'width': '1133px' });
        $("#divPayrollGrid").css({ 'max-width': '1133px' });
        $("#divPayrollGrid").css({ 'width': '1133px' });
        $("#divPayrollHeader").css({ 'width': '1133px' });
        grid.hideColumn(0);
        $(".k-grid-toolbar").hide();
    }
    grid.resize();

}

//function DeleteConfirmation(e) {

//    e.preventDefault();
//    var grid = this;
//    var data = grid.dataItem($(e.target).parents('tr'));

//    $("#confirmDialog").kendoDialog({
//        title: "Payroll Vouchers - Delete Confirmation",
//        content: "Are you sure you want to delete the current record?",
//        width: 400,
//        height: 200,
//        buttonLayout: "normal",
//        actions: [{
//            text: "Yes",
//            primary: true,
//            action: function () {
//                grid.dataSource.remove(data);
//                grid.dataSource.sync();
//                $("#confirmDialog").data("kendoDialog").close();
//            }
//        },
//        {
//            text: "No",
//            action: function () {
//                $("#confirmDialog").data("kendoDialog").close();
//            }
//        }]
//    });
//    $("#confirmDialog").data("kendoDialog").open();
//}

function dpCheckDate_onChange() {
    divSubmitButton.style.display = "none";
}

function UpdateBatchComment() {
    var grid = $("#gridPayroll").data("kendoGrid");
    var ds = grid.dataSource;
    var glID = ds._data[ds._data.length - 1].GLID;

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Payroll/UpdateBatchComment',
        data: JSON.stringify({
            GLID: glID,
            Comments: txtComments.value
        }),
        success: function (data) {
            ds.read();
        },
        error: function (e) {
            myAlert("Comment Update Failed");
            ds.read();
            console.log(e);
        }
    });
}