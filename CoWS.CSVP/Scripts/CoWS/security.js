function btnResetPassword_onClick() {

    var userid = $("#ddlUsers").data("kendoDropDownList").value();
    if (userid === "") {
        myAlert("Select a User.", "Error Dialog");
        return;
    }

    $("#confirmDialog").kendoDialog({
        title: "Reset Password Confirmation",
        content: "<div style='text-align:center'>You are about to reset the password for '<b>" + $("#ddlUsers").data("kendoDropDownList").text() + "</b>'.</br>Are you sure?</div>",
        width: 500,
        height: 200,
        buttonLayout: "normal",
        actions: [{
            text: "Yes",
            primary: true,
            action: function (e) {
                ResetPassword(userid);
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

function ResetPassword(userid) {
    
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: "/Security/ResetPassword",
        data: JSON.stringify({ userID: userid }),
        success: function (response) {
            if (response === "Success") {
                successMessage.innerHTML = "Password Reset Successful.";
                errorMessage.innerHTML = "";
            }
            else {
                successMessage.innerHTML = "";
                errorMessage.innerHTML = "Password Reset Failed.";
            }
        },
        error: function (e) {
            console.log(e);
            successMessage.innerHTML = "";
            errorMessage.innerHTML = "Password Reset Failed.";
        }
    });
}

function ddlModifyUsers_onChange() {

    var ddl = $("#ddlModifyUsers").data("kendoDropDownList");
    if (ddl.selectedIndex !== 0) {
        GetUser(ddl.value());
    }
}

function GetUser(userID) {
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: "/Security/ModifyUser",
        data: JSON.stringify({ userID: userID }),
        success: function (data) {
            $("body").html(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}