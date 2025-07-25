"use strict";
let frmUserAccess = $('#frm-user-access');

$(async function () {
    const userAccessData = await loadUserAccess();

    populateUserAccessTable(userAccessData);

    $(frmUserAccess).on('submit', function (e) {
        e.preventDefault();

        const accessList = [];
        $("#tbl-user-access tbody tr").each(function () {
            const row = $(this);
            const id = row.find(".module-id").val();
            accessList.push({
                Id: id,
                CanView: row.find(".chk-canview").is(":checked"),
                CanCreate: row.find(".chk-canadd").is(":checked"),
                CanEdit: row.find(".chk-canedit").is(":checked"),
                CanDelete: row.find(".chk-candelete").is(":checked")
            });
        });

        $.ajax({
            url: '/UserModuleAccess/SaveBulk',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(accessList)
        }).done(() => {
            Swal.fire('Saved!', 'User Access updated.', 'success');
        }).fail(xhr => {
            Swal.fire('Error', xhr.responseText || 'Save failed.', 'error');
        });
    });
});

async function loadUserAccess() {
    try {
        const response = await $.ajax({
            url: '/UserModuleAccess/GetUserModuleAccess',
            method: 'GET'
        });
        return response;
    } catch (err) {
        console.error('Failed to load user access data:', err);
        Swal.fire('Error', 'Failed to load access data.', 'error');
        return [];
    }
}

function populateUserAccessTable(data) {
    const tbody = $("#tbl-user-access tbody");
    tbody.empty();

    data.forEach(item => {
        const row = `
            <tr>
                <td>
                    ${item.ModuleName}
                    <input type="hidden" class="module-id" value="${item.Id ?? 0}" />                 
                </td>
                <td  class="text-center">
                    <input type="checkbox" class="form-check-input chk-canview" ${item.CanView ? "checked" : ""}>
                </td>
                <td  class="text-center">
                    <input type="checkbox" class="form-check-input chk-canadd" ${item.CanCreate ? "checked" : ""}>
                </td>
                <td  class="text-center">
                    <input type="checkbox" class="form-check-input chk-canedit" ${item.CanEdit ? "checked" : ""}>
                </td>
                <td  class="text-center">
                    <input type="checkbox" class="form-check-input chk-candelete" ${item.CanDelete ? "checked" : ""}>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}