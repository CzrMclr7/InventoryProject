let $frm_change_pass = $("#frm_change_pass");

//$(function () {
//    frmChangeRebindValidator();
//    function frmChangeRebindValidator () {
//        let $form = $frm_change_pass;

//        $form.unbind();
//        $form.data("validator", null);
//        $.validator.unobtrusive.parse($form);
//        $form.validate($form.data("unobtrusiveValidation").options);
//        $form.data("validator").settings.ignore = "";

//        $frm_change_pass.submit(function (e) {
//            e.preventDefault();

//            var btn_submit = $("#frm_change_pass button[type='submit']");

//            if (!$(this).valid()) {
//                messageBox("Please fill up all required fields.", "error", true);
//                return;
//            }

//            $.ajax({
//                url: $frm_change_pass.attr("action"),
//                method: $frm_change_pass.attr("method"),
//                data: $frm_change_pass.serialize(),
//                beforeSend: function () {
//                    btn_submit.attr({ disabled: true });
//                },
//                success: function () {
//                    messageBox("Password changed successfully!", "success", true);
//                    btn_submit.attr({ disabled: false });
//                    $("#changePassModal").modal("hide");
//                },
//                error: function (error) {
//                    messageBox(error.responseText, "error", true);
//                    btn_submit.attr({ disabled: false });
//                }
//            });
//        });
//    }

    //$(".input-group-text").on("click", function () {
    //    const $input = $(this).closest(".input-group").find("input");
    //    const type = $input.attr("type") === "password";

    //    $input.attr("type", type ? "text" : "password");
    //    $(this).attr('data-password', type ? 'true' : 'false');
    //    if (!type) {
    //        $(this).addClass('show-password');
    //    } else {
    //        $(this).removeClass('show-password');
    //    }
    //});
//});

function openChangePassModal(userId, userName) {
    ResetFormValidation($frm_change_pass);
    $("[name='UserId']").val(userId);
    $("[name='Username']").val(userName);
    $("#changePassModal").modal("show");
    //$(".input-group-text").each(function () {
    //    $(this).removeClass('show-password')
    //        .addClass('show-password')
    //});
    //$(".form-control").each(function () {
    //    $(this).attr('type', 'password')
    //});
}