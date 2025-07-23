'use strict';
let frmUserInfo = $('#frm-user-info');
let frmChangePassword = $('#frm-change-password');
let userId = $('#user-id').val();

$(async function () {
    frmUserInfo.on('submit', async function (e) {
        e.preventDefault();
        if (!frmUserInfo.valid()) return;
        let formData = new FormData(e.target);

        const url = frmUserInfo.attr("action");
        const method = frmUserInfo.attr("method");

        Swal.fire({
            title: 'Saving...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: formData,
            contentType: false,
            cache: false,
            processData: false, 
            success: function (response) {
                Swal.fire("Success!", "Saved successfully.", "success");
            },
            error: function (xhr) {
                let message = "Saving failed.";
                if (xhr.responseText) {
                    message = xhr.responseText;
                }
                Swal.fire("Error", message, "error");
            }
        });

    });

    frmChangePassword.on('submit', function (e) {
        e.preventDefault();
        if (!frmChangePassword.valid()) return;
        console.log(frmChangePassword.userId);
        const url = frmChangePassword.attr("action");
        const method = frmChangePassword.attr("method");
        const data = frmChangePassword.serialize();
        //const data = frmUserInfo.serialize();
        //const dataArray = frmChangePassword.serializeArray();
        //const dataObj = {
        //    NewPassword: $('#User.NewPassword').val(),
        //    ConfirmPassword: $('#User.ConfirmPassword').val()
        //    // Add other properties as needed
        //};

        console.log(data, frmChangePassword.serializeArray());

        Swal.fire({
            title: 'Updating...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: data,
            success: function (response) {
                Swal.fire("Success!", "Account updated successfully.", "success");
                $("#changePasswordModal").modal("hide");
                frmChangePassword[0].reset();
            },
            error: function (xhr) {
                let message = "Account update failed.";
                if (xhr.responseText) {
                    message = xhr.responseText;
                }
                Swal.fire("Error", message, "error");
            }
        });
 
    });

    $("#User_ProfilePictureFile").change(function () {
        let container = $("#imagePreview");
        readUrl(this, container);
    });
});