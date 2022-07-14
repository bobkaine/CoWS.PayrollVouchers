$(document).ready(function () {

    var myAlert = $("#myAlert");

    myAlert.kendoWindow({
        title: "Payroll Vouchers - ",
        visible: false,
        draggable: true,
        resizable: false,
        modal: true
    });

    $("#btnWindowCancel").bind("click", function () {
        myAlert.data("kendoWindow").close();
    });

    $("#btnWindowOK").bind("click", function () {
        myAlert.data("kendoWindow").close();
    });

    $("#btnWindowNo").bind("click", function () {
        myAlert.data("kendoWindow").close();
    });

    var xmlWindow = $("#xmlWindow");
    xmlWindow.kendoWindow({
        title: "Payroll Vouchers - XML Request/Response",
        visible: false,
        draggable: true,
        resizable: false,
        modal: true,
        width: 1100,
        height: 750
    });

    $("#btnXmlWindowOK").bind("click", function () {
        xmlWindow.data("kendoWindow").close();
    });

    var div = document.getElementById('myAlert');
    if (detectIE()) {
        div.removeAttribute('style');
    }

});

// The intention is to make the creation of the KendoWindow cleaner from code
// 'primaryMessage' (required)
// 'title' will default to "PICS - Confirmation Dialog" (optional)
// 'twoButtons' is boolean, if true "Confirm" and "Cancel" button will be displayed, 
// if false only the "OK" button displayed (optional)
// 'onclose' can be used to execute a function upon the dialog closing (optional)
// example: var onClose = function () { txtTextBox1.value = "" };
// 'width' and 'height' are numeric, any value less than width(400) or height(200) will be ignored (optional)
// if you need to pass a value to an optional downline parameter, pass null for missing prior parameters
// if 'yesno' = true and twoButtons is true "Confirm" and "Cancel" will be replaced with "Yes" and "No"
function myAlert(primaryMessage, title, twoButtons, secondaryMessage, onClose, width, height, yesno) {

    var myAlert = $("#myAlert").data("kendoWindow");

    if (!divPrimaryMessage) {
        myAlert.title("Payroll Vouchers - Dialog Create Failed!");
        divPrimaryMessage.innerHTML = "The 'primaryMessage' parameter is required";
        myAlert.center().open();
        return;
    }

    divPrimaryMessage.innerHTML = primaryMessage;

    if (twoButtons) {
        divOkButton.style.display = "none";
        divConfirmCancelButtons.style.display = "inline";
        divYesNoButtons.style.display = "none";
    }
    else {
        divOkButton.style.display = "inline";
        divConfirmCancelButtons.style.display = "none";
        divYesNoButtons.style.display = "none";
    }

    if (yesno) {
        divOkButton.style.display = "none";
        divConfirmCancelButtons.style.display = "none";
        divYesNoButtons.style.display = "inline";
    }

    if (secondaryMessage) {
        divSecondaryMessage.innerHTML = secondaryMessage;
    }
    else {
        divSecondaryMessage.innerHTML = "";
    }

    if (title) {
        myAlert.title("Payroll Vouchers - " + title);
    }
    else {
        myAlert.title("Payroll Vouchers - Confirmation Dialog");
    }

    if (width && width > 450) {
        $("#myAlert")[0].style.width = width + "px";
    }
    else {
        $("#myAlert")[0].style.width = "450px";
    }

    if (height && height > 200) {
        $("#myAlert")[0].style.height = height + "px";
    }
    else {
        $("#myAlert")[0].style.height = "200px";
    }

    if (onClose) {
        myAlert.bind("close", onClose);
    }
    else {
        myAlert.bind("close", function () { });
    }

    myAlert.center().open();
}

function detectIE() {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return true;
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return true;
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return true;
    }

    // other browser
    return false;
}

function GetCurrentFileDocumentCreationDate(documentType) {

    var documentCreationDate;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/GetCurrentFileDocumentCreationDate',
        data: JSON.stringify({ DocumentType: documentType }),
        async: false,
        success: function (response) {
            documentCreationDate = moment(response).format("MM/DD/YYYY");
        },
        error: function (e) {
            console.log(e);
        }
    });
    return documentCreationDate;
}

function documentNumberExists(documentType, documentCreationDate) {

    blnDocNoExists = true;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/DocumentNumberExists',
        data: JSON.stringify({ DocumentType: documentType, DocumentCreationDate: documentCreationDate }),
        async: false,
        success: function (response) {
            if (response.toLowerCase() === "false") {
                blnDocNoExists = false;
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
    return blnDocNoExists;
}

function RemoveApprovals(fileType) {

    var grid;
    switch (fileType) {
        case "CS":
            grid = $("#gridChildSupport").data("kendoGrid");
            break;
        case "PV":
            grid = $("#gridVouchersPayable").data("kendoGrid");
            break;
    }

    var batchID = lblBatchID.innerHTML;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/RemoveApprovals',
        data: JSON.stringify({ BatchID: batchID }),
        async: false,
        success: function (response) {
            if (response === "Success") {
                myAlert("All Approvals removed. This batch can now be edited and re-submitted.");                
                grid.dataSource.read();
            }
            else {
                myAlert(response);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function UpdateGLProcessorApproval(approved, fileType) {

    var grid;
    switch (fileType) {
        case "CS":
            grid = $("#gridChildSupport").data("kendoGrid");
            break;
        case "PV":
            grid = $("#gridVouchersPayable").data("kendoGrid");
            break;
    }

    var data = grid.dataSource._data[1];
    var batchHeaderID;
    if (typeof data !== "undefined") {
        batchHeaderID = data.BatchHeaderID;
    }

    var result = "";
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/UpdateGLProcessorApproval',
        data: JSON.stringify({ Approved: approved, BatchHeaderID: batchHeaderID }),
        async: false,
        success: function (response) {
            result = response;
        },
        error: function (e) {
            console.log(e);
        }
    });
    return result;
}

function UpdateReviewerApproval(approved, fileType) {

    var grid;
    switch (fileType) {
        case "CS":
            grid = $("#gridChildSupport").data("kendoGrid");
            break;
        case "PV":
            grid = $("#gridVouchersPayable").data("kendoGrid");
            break;
    }

    var data = grid.dataSource._data[1];
    var batchHeaderID;
    if (typeof data !== "undefined") {
        batchHeaderID = data.BatchHeaderID;
    }

    var result = "";
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/UpdateReviewerApproval',
        data: JSON.stringify({ Approved: approved, BatchHeaderID: batchHeaderID }),
        async: false,
        success: function (response) {
            result = response;
        },
        error: function (e) {
            console.log(e);
        }
    });
    return result;
}

function GetLatestBatch(batchType) {
    var result = "";
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/GetLatestBatch',
        data: JSON.stringify({ BatchType: batchType }),
        async: false,
        success: function (response) {
            result = moment(response).format("MM/DD/YYYY");
            if (result === '01/01/0001') {
                myAlert("No Records found in Database.");
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
    return result;
}

function UpdateUI(response, fileType) {

    var grid;
    switch (fileType) {
        case "CS":
            grid = $("#gridChildSupport").data("kendoGrid");
            break;
        case "PV":
            grid = $("#gridVouchersPayable").data("kendoGrid");
            break;
        case "GL":
            grid = $("#gridPayroll").data("kendoGrid");
            break;
    }

    for (var i = 0; i < response.length; i++) {
        if (response[i].Key === "BatchID") {
            lblBatchID.innerHTML = response[i].Value;
            if (response[i].Value.substring(0, 2) !== "PA") {
                divAPSubmitButton.style.display = "none";
                $("#chkAPProcessorApproved").prop("checked", true);
                $("#chkAPProcessorApproved").prop("disabled", true);
                divAPCheckBox.style.display = "inline-block";
                divGLCheckBox.style.display = "inline-block";
                divReviewerCheckBox.style.display = "inline-block";
                grid.hideColumn(0);
                $(".k-grid-toolbar").hide();
            }
        }
        else if (response[i].Key === "ErrorMessage") {
            myAlert("Post to FMS was not successful.</br>", "Error", null, "<div style='font-size:small; '>" + response[i].Value + "</div>", null, null, 250);
        }
    }
}

function SubmitAPBatch(fileType) {

    var DocumentCreationDate;
    switch (fileType) {
        case "CS":
            DocumentCreationDate = $("#dpCheckDate").data("kendoDatePicker").value();
            break;
        case "PV":
            DocumentCreationDate = $("#dpVoucherDate").data("kendoDatePicker").value();
    }

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/SubmitAPBatch',
        data: JSON.stringify({ FileType: fileType, DocumentCreationDate: DocumentCreationDate }),
        success: function (response) {
            UpdateUI(response, fileType);
            kendo.ui.progress($(document.body), false);
        },
        error: function (e) {
            console.log(e);
            kendo.ui.progress($(document.body), false);
        }
    });
}

function ReadSoapXML(batchType) {
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Common/ReadSoapXML',
        data: JSON.stringify({ BatchType: batchType }),
        success: function (data) {
            var window = $("#xmlWindow").data("kendoWindow");
            xmlWindow_wnd_title.innerHTML = "Payroll Vouchers - XML Request/Response (" + batchType + ")";
            SoapRequest.innerHTML = FormatXml(data.Request);
            SoapResponse.innerHTML = FormatXml(data.Response);
            FileDescription.innerHTML = data.FileDescription;
            FileDate.innerHTML = moment(data.FileDate).format("MM/DD/YYYY") === '01/01/0001 12:00' ? "" : moment(data.FileDate).format("MM/DD/YYYY");
            DatePosted.innerHTML = moment(data.DatePosted).format("MM/DD/YYYY hh:mm") === '01/01/0001 12:00' ? "" : moment(data.DatePosted).format("MM/DD/YYYY hh:mm");
            window.center().open();
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function FormatXml(xml) {
    var formatted = '';
    var reg = /(>)(<)(\/*)/g;
    xml = xml.replace(reg, '$1\r\n$2$3');
    var pad = 0;
    jQuery.each(xml.split('\r\n'), function (index, node) {
        var indent = 0;
        if (node.match(/.+<\/\w[^>]*>$/)) {
            indent = 0;
        } else if (node.match(/^<\/\w/)) {
            if (pad !== 0) {
                pad -= 1;
            }
        } else if (node.match(/^<\w([^>]*[^\/])?>.*$/)) {
            indent = 1;
        } else {
            indent = 0;
        }

        var padding = '';
        for (var i = 0; i < pad; i++) {
            padding += '  ';
        }

        formatted += padding + node + '\r\n';
        pad += indent;
    });

    return formatted;
}

function DeleteConfirmation(e) {

    e.preventDefault();
    var grid = this;
    var data = grid.dataItem($(e.target).parents('tr'));

    $("#confirmDialog").kendoDialog({
        title: "Payroll Vouchers - Delete Confirmation",
        content: "Are you sure you want to delete the current record?",
        width: 400,
        height: 200,
        buttonLayout: "normal",
        actions: [{
            text: "Yes",
            primary: true,
            action: function () {
                grid.dataSource.remove(data);
                grid.dataSource.sync();
                $("#confirmDialog").data("kendoDialog").close();
            }
        },
        {
            text: "No",
            action: function () {
                $("#confirmDialog").data("kendoDialog").close();
            }
        }]
    });
    $("#confirmDialog").data("kendoDialog").open();
}
