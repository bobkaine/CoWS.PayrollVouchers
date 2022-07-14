$(document).ready(function () {

    $(".toggle-password").click(function () {

        $(this).toggleClass("k-i-lock k-i-unlock");
        var input = $("[id='" + $(this).attr("toggle") + "']");

        if (input.attr("type") === "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });

    PopulateGeneralSettings();
    PopulateWebServiceSettings();

});

function btnSave_onClick() {

    if (!webSettingsChanged && !generalSettingsChanged) {
        myAlert("Nothing has changed.", "Information");
        return;
    }

    if (webSettingsChanged) {
        var wsResult = SaveWebServiceSettings();
        if (wsResult === 0) {
            webSettingsChanged = false;
        }
    }

    if (generalSettingsChanged) {
        var gsResult = SaveGeneralSettings();
        if (gsResult === 0) {
            generalSettingsChanged = false;
        }
    }

    if (wsResult !== undefined && wsResult !== 0 || gsResult !== undefined && gsResult !== 0) {
        var alertText = "";
        if (wsResult !== undefined && wsResult !== 0) {
            alertText = "Web Service Settings were not saved.</br>";
        }
        if (gsResult !== undefined && gsResult !== 0) {
            alertText += "General Settings were not saved.";
        }
        myAlert(alertText, "Error");        
    }

    if (wsResult === 0 && gsResult === 0) {
        lblStatus.innerHTML = "All Settings Saved!";
    }
    else if (wsResult === 0) {
        lblStatus.innerHTML = "Web Service Settings Saved!";
    }
    else if (gsResult === 0) {
        lblStatus.innerHTML = "General Settings Saved!";
    }

}

function SaveWebServiceSettings() {

    var wsSettings = {
        FMSUser: txtFMSUser.value,
        FMSPassword1: txtFMSPassword1.value,
        FMSPassword2: txtFMSPassword2.value,
        FMSPassword3: txtFMSPassword3.value,
        Ledger: txtLedger.value,
        OSUser: txtOSUser.value,
        OSPassword: txtOSPassword.value,
        OutputDevice: txtOutputDevice.value,
        WebServiceURL: txtWebServiceAddress.value,
        ID: wsID.innerHTML,
        WhoModified: 1
    };

    var result = -1;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Admin/SaveWebServiceSettings',
        data: JSON.stringify(wsSettings),
        async: false,
        success: function (response) {
            result = response;
        },
        error: function (e) {
            result = -1;
            console.log(e);
        }
    });
    return result;
}

function SaveGeneralSettings() {

    var gSettings = {
        BankNumber: txtBankNumber.value,
        ChildSupportDesc: txtChildSupportDesc.value,
        FilePath: txtFilePath.value,
        UserCode: txtUserCode.value,
        VouchersPayableDesc: txtVouchersPayableDesc.value,
        TempPassword: txtTempPassword.value,
        ID: gsID.innerHTML
    };

    var result = -1;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Admin/SaveGeneralSettings',
        data: JSON.stringify(gSettings),
        async: false,
        success: function (response) {
            result = response;            
        },
        error: function (e) {
            result = -1;
            console.log(e);
        }
    });
    return result;
}

function btnCancel_onClick() {

    if (webSettingsChanged) {        
        PopulateWebServiceSettings();
        webSettingsChanged = false;
    }

    if (generalSettingsChanged) {
        PopulateGeneralSettings();
        generalSettingsChanged = false;
    }
}

function PopulateGeneralSettings() {

    var gs = GetGeneralSettings();

    txtBankNumber.value = gs.BankNumber;
    txtChildSupportDesc.value = gs.ChildSupportDesc;
    txtFilePath.value = gs.FilePath;
    txtUserCode.value = gs.UserCode;
    txtVouchersPayableDesc.value = gs.VouchersPayableDesc;
    txtTempPassword.value = gs.TempPassword;
    gsID.innerHTML = gs.ID;

}

function PopulateWebServiceSettings() {

    ws = GetWebServiceSettings();

    txtFMSUser.value = ws.FMSUser;
    txtFMSPassword1.value = ws.FMSPassword1;
    txtFMSPassword2.value = ws.FMSPassword2;
    txtFMSPassword3.value = ws.FMSPassword3;
    txtLedger.value = ws.Ledger;
    txtOSUser.value = ws.OSUser;
    txtOSPassword.value = ws.OSPassword;
    txtOutputDevice.value = ws.OutputDevice;
    txtWebServiceAddress.value = ws.WebServiceURL;
    wsID.innerHTML = ws.ID;
}

function GetWebServiceSettings() {

    var fmsWebServiceSettings;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Admin/GetWebServiceSettings',
        async: false,
        success: function (response) {
            if (response !== null) {
                fmsWebServiceSettings = response;
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
    return fmsWebServiceSettings;
}

function GetGeneralSettings() {

    var generalSettings;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/Admin/GetGeneralSettings',
        async: false,
        success: function (response) {
            if (response !== null) {
                generalSettings = response;
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
    return generalSettings;
}