var mblnAdd = false;

function btnGetChecks_onClick() {

    var DocumentCreationDate = $("#dpCheckDate").data("kendoDatePicker").value();

    if (DocumentCreationDate === null) {
        DocumentCreationDate = GetCurrentFileDocumentCreationDate("CS");
        $("#dpCheckDate").data("kendoDatePicker").value(DocumentCreationDate);
    }
    
    var grid = $('#gridChildSupport').data('kendoGrid');
    if (!documentNumberExists("CS", DocumentCreationDate)) {
        
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            url: '/Common/ProcessCSVPFile',
            data: JSON.stringify({
                FileType: 'CS',
                DocumentCreationDate: DocumentCreationDate
            }),
            async: false,
            success: function (response) {
                if (response === "") {
                    myAlert("'Child Support' file <b>NOT</b> found!");
                    return;
                }
                else if (response.BatchType === "ZZ") {
                    fileDocumentCreationDate = response.DocumentNumbers[0].DocumentCreationDate;
                    myAlert("<b>Check Date Mismatch</b>", "Date Mismatch", null, "Current 'Child Support' file is for <b>" + moment(fileDocumentCreationDate).format("MM/DD/YYYY") + "</b>!");
                    grid.dataSource.data([]);
                    grid.dataSource.read();
                    return;
                }
                grid.dataSource.read();
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
    else {
        var requestedDate = moment($("#dpCheckDate").data("kendoDatePicker").value()).format("MM/DD/YYYY");
        myAlert("Checks for '" + requestedDate + "' already exist.</br>Information retrieved from database.");        
        grid.dataSource.read();
    }

}

function btnGetChecksFromDB_onClick() {

    $("#dpCheckDate").data("kendoDatePicker").value(GetLatestBatch("CS"));

    var grid = $('#gridChildSupport').data('kendoGrid');
    grid.dataSource.read();
}

function btnSubmitChecks_onClick() {

    if ($("#dpCheckDate").data("kendoDatePicker").value() === null) {
        myAlert("Select a date to Post to FMS.", "Error");
        return;
    }

    kendo.ui.progress($(document.body), true);
    setTimeout(SubmitAPBatch("CS"), 100);

}

function btnUnSubmitChecks_onClick() {
        
    $("#confirmDialog").kendoDialog({
        title: "Payroll Vouchers - Confirmation",
        content: "This will remove all approvals and allow for the re-submittal of this batch. Are you sure?",
        width: 400,
        height: 200,
        buttonLayout: "normal",
        actions: [{
            text: "Yes",
            primary: true,
            action: function (e) {
                RemoveApprovals("CS");
            }
        },
        {
            text: "No",
            action: function (e) {
                //alert("NO");
            }
        }]
    });
    $("#confirmDialog").data("kendoDialog").open();
}

function DocumentCreationDate() {
    return { DocumentCreationDate: $("#dpCheckDate").data("kendoDatePicker").value() };    
}

function gridChildSupport_onDataBound() {
    var grid = ds = $("#gridChildSupport").data("kendoGrid");
    var ds = grid.dataSource;
    var lastRow = ds._data[ds._data.length - 1];

    if (lastRow === undefined) {
        divAPSubmitButton.style.display = "none";
        grid.hideColumn(0);
        $(".k-grid-toolbar").hide();
        $(".k-grid-footer").hide();
        lblBatchTotal.innerHTML = "";
        lblPeriod.innerHTML = "";
        lblYear.innerHTML = "";
        lblBatchID.innerHTML = "";
        lblFileDescription.innerHTML = "";       
        return;
    }
    else if (lastRow.BatchHeaderID > 0) {
        lblBatchTotal.innerHTML = lastRow.BatchTotal.toLocaleString("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 });
        lblPeriod.innerHTML = lastRow.Period;
        lblYear.innerHTML = lastRow.Year;
        lblFileDescription.innerHTML = lastRow.FileDescription1 + ": " + lastRow.FileDescription2;

        if (parseFloat(lastRow.BatchTotal) !== parseFloat(calcBatchTotal.innerHTML.replace("$", "").replace(",", ""))) {
            $("#lblBatchTotal").css({ 'color': 'red' });
        }
        else {
            $("#lblBatchTotal").css({ 'color': 'black' });
        }

        if (lastRow.BatchID === null) {
            $("#lblBatchID").text("");            
        }
        else {
            lblBatchID.innerHTML = lastRow.BatchID;            
        }
        // We have a batch, so set up Role based controls
        SetDisplayMode(lastRow);
    }
}

function gridChildSupport_onBeforeEdit(e) {
    if (e === undefined)
        return;
    if (e.model.BatchHeaderID === 0) {
        mblnAdd = true;
    }
    else {
        mblnAdd = false;
    }
}

function gridChildSupportDataSource_onSync() {
    var grid = $("#gridChildSupport").data("kendoGrid");
    grid.dataSource.read();
}

function UpdateCommonValues() {
    var data = $("#gridChildSupport").data("kendoGrid").dataSource._data[1];
    if (typeof data !== "undefined") {
        // Hidden Values
        $("#BankNumber").val(data.BankNumber).change();
        $("#BatchHeaderID").val(data.BatchHeaderID).change();
        $("#BatchTotal").val(data.BatchTotal).change();
        $("#BatchType").val(data.BatchType).change();
        $("#DocumentType").val(data.DocumentType).change();
        $("#TransCode").val(data.TransCode).change();
        $("#UserCode").val(data.UserCode).change();
    }
}

function btnCopy_onClick() {
    var data = $("#gridChildSupport").data("kendoGrid").dataSource._data[1];
    if (typeof data !== "undefined") {        
        $("#dpDocumentCreationDateAdd").data("kendoDatePicker").value(moment(data.DocumentCreationDate).format("MM/DD/YYYY"));
        $("#dpDocumentCreationDateAdd").data("kendoDatePicker").trigger("change");
        $("#CheckDescription").val(data.CheckDescription).change();
        $("#dpPayPeriodEndingDateAdd").data("kendoDatePicker").value(moment(data.PayPeriodEndingDate).format("MM/DD/YYYY"));
        $("#dpPayPeriodEndingDateAdd").data("kendoDatePicker").trigger("change");
        $("#dpCheckCreationDateAdd").data("kendoDatePicker").value(moment(data.CheckCreationDate).format("MM/DD/YYYY"));
        $("#dpCheckCreationDateAdd").data("kendoDatePicker").trigger("change");
        $("#ResponsibilityCode").val(data.ResponsibilityCode).change();
        $("#ObjectCode").val(data.ObjectCode).change();
        $("#ProgramCode").val(data.ProgramCode).change();
        $("#PaymentType").val(data.PaymentType).change();
        $("#PaymentStatus").val(data.PaymentStatus).change();
    }
}

function SetDisplayMode(batchRecord) {

    var grid = $("#gridChildSupport").data("kendoGrid");

    if (isAPProcessor.toLowerCase() === "true" && batchRecord.APApprover === null) {
        grid.showColumn(0);
        $(".k-grid-toolbar").show();
    }
    else {
        grid.hideColumn(0);
        $(".k-grid-toolbar").hide();
    }

    if (isGLProcessor.toLowerCase() === "true") {
        divAPCheckBox.style.display = "inline-block";
        divGLCheckBox.style.display = "inline-block";
        divReviewerCheckBox.style.display = "inline-block";
        if (batchRecord.APApprover === null || batchRecord.GLApprover !== null) {
            $("#chkGLProcessorApproved").prop("disabled", true);
        }
    }

    if (isReviewer.toLowerCase() === "true") {
        divAPCheckBox.style.display = "inline-block";
        divGLCheckBox.style.display = "inline-block";
        divReviewerCheckBox.style.display = "inline-block";
        if (batchRecord.GLApprover === null || batchRecord.FinalApprover !== null) {
            $("#chkReviewerApproved").prop("disabled", true);
        }
    }

    if (batchRecord.APApprover !== null) {
        $("#chkAPProcessorApproved").prop("checked", true);
        if (isAPProcessor.toLowerCase() === "true") {
            divAPSubmitButton.style.display = "none";
        }
        divAPCheckBox.style.display = "inline-block";
        divGLCheckBox.style.display = "inline-block";
        divReviewerCheckBox.style.display = "inline-block";
    }
    else {
        $("#chkAPProcessorApproved").prop("checked", false);
        if (isAPProcessor.toLowerCase() === "true") {
            divAPSubmitButton.style.display = "inline-block";
        }
    }

    if (batchRecord.GLApprover !== null) {
        $("#chkGLProcessorApproved").prop("checked", true);
    }
    else {
        $("#chkGLProcessorApproved").prop("checked", false);
    }

    if (batchRecord.FinalApprover !== null) {
        $("#chkReviewerApproved").prop("checked", true);
    }
    else {
        $("#chkReviewerApproved").prop("checked", false);
    }

    if (isGLProcessor.toLowerCase() === "true" && batchRecord.APApprover !== null) {
        if (typeof divRemoveApprovals !== "undefined") {
            divRemoveApprovals.style.display = "inline-block";
        }
    }    
}

function dpCheckDate_onChange() {
    divAPSubmitButton.style.display = "none";    
}