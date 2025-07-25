"use strict"
//let tblProduct;
let frmLogin = $('#frm-login');
let frmRegister = $('#frm-register');
let frmUpdate = $('#frm-update');
let liLogout = $('#logout');
//let btnEdit = $("#btnEdit");
//let btnDelete = $("#btnDelete");
//let btnReset = $("#btnReset");
//let btnRefresh = $("#btnRefresh");

// Form submit → AJAX + Swal feedback 
$(async function () {
    liLogout.hide();

    frmLogin.on('submit', function (e) {
        e.preventDefault();
        if (!frmLogin.valid()) return;

        const url = frmLogin.attr('action') + '?returnUrl=' + $("#returnUrl").html();
        const method = frmLogin.attr("method");
        const data = frmLogin.serialize();

        Swal.fire({
            title: 'Logging In...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: data,
            success: function (response) {
                Swal.fire("Success!", "Logged In successfully.", "success");
                var logoff = "/Account/LogOff";
                if ($("#returnUrl").html() == "" || $("[name='ReturnUrl']").val() == logoff) {
                    window.location.href = "/";
                } else {
                    window.location.href = $("[name='ReturnUrl']").val();
                }
                //const loginTab = new bootstrap.Tab(document.querySelector('#login-tab'));
                //loginTab.show();
            },
            error: function (xhr) {
                let message = "Log In failed.";
                $("#Password").val("");
                if (xhr.responseText) {
                    message = xhr.responseText;
                }
                Swal.fire("Error", message, "error");
            }
        });
        //$frmR[0].reset();
    });

    frmRegister.on('submit', async function (e) {
        e.preventDefault();
        if (!frmRegister.valid()) return;


        const url = frmRegister.attr("action");
        const method = frmRegister.attr("method");
        const data = frmRegister.serialize();

        Swal.fire({
            title: 'Registering...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: data,
            //success: function (response) {
            //    Swal.fire("Success!", "Account created successfully.", "success");
            //    const loginTab = new bootstrap.Tab(document.querySelector('#login-tab'));
            //    loginTab.show();
            //},
            success: function (response) {
                Swal.fire("Success!", "Account created successfully.", "success").then(() => {
                    const loginTab = new bootstrap.Tab(document.querySelector('#login-tab'));
                    loginTab.show();
                });
            },
            error: function (xhr) {
                let message = "Account creation failed.";
                if (xhr.responseText) {
                    message = xhr.responseText;
                }
                Swal.fire("Error", message, "error");
            }
        });
        frmRegister[0].reset();
    });

    frmUpdate.on('submit', function (e) {
        e.preventDefault();
        if (!frmUpdate.valid()) return;

        const url = frmUpdate.attr("action");
        const method = frmUpdate.attr("method");
        const data = frmUpdate.serialize();

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
                const loginTab = new bootstrap.Tab(document.querySelector('#login-tab'));
                loginTab.show();
            },
            error: function (xhr) {
                let message = "Account update failed.";
                if (xhr.responseText) {
                    message = xhr.responseText;
                }
                Swal.fire("Error", message, "error");
            }
        });
        frmUpdate[0].reset(); 
    });
    
});
//$("#formLogin").submit(function (e) {
//    e.preventDefault();
//const username = $("#loginUsername").val().trim();
//const password = $("#loginPassword").val().trim();

//$.post("/Account/Login", JSON.stringify({Username: username, Password: password }), function (res) {
//            if (res.success) {
//    window.location.href = "/Dashboard";
//            } else {
//    $("#loginError").text(res.message).show();
//            }
//        }).fail(() => $("#loginError").text("Login failed.").show());
//    });

//$("#formRegister").submit(function (e) {
//    e.preventDefault();
//const username = $("#registerUsername").val().trim();
//const password = $("#registerPassword").val().trim();

//$.post("/Account/Register", JSON.stringify({Username: username, Password: password }), function (res) {
//    $("#registerMessage").text(res.message).css("color", res.success ? "green" : "red").show();
//        });
//    });

//$("#formUpdate").submit(function (e) {
//    e.preventDefault();
//const username = $("#updateUsername").val().trim();
//const password = $("#updatePassword").val().trim();

//$.post("/Account/UpdatePassword", JSON.stringify({Username: username, Password: password }), function (res) {
//    $("#updateMessage").text(res.message).css("color", res.success ? "green" : "red").show();
//        });
//    });
