"use strict";

let tblProductAdjustment;
let frmAdjustmentIn = $("#frm-adjustment-in");
let frmAdjustmentOut = $("#frm-adjustment-out");


$(async function () {
    tblProductAdjustment = $("#tbl-adjustments").DataTable({
        ajax: {
            url: '/ProductAdjustment/Inq',
            dataSrc: ''
        },
        columns: [
            { data: "DateCreatedFormatted" },
            { data: "Name" },
            { data: "Action" },
            { data: "Quantity" }
        ],
        order: [[0, 'desc']],
        select: true,
        rowId: 'Id',
        processing: true
    });

    await addAdjustmentInRow();
    await addAdjustmentOutRow();

    $("#btn_add_in").on("click", async function () {
        await addAdjustmentInRow();
    });

    $("#btn_add_out").on("click", async function () {
        await addAdjustmentOutRow();
    });

    $(document).on("click", ".btn_remove_detail", function () {
        $(this).closest("tr").remove();
    });

    frmAdjustmentIn.on('submit', function (e) {
        e.preventDefault();
        if (!$(this).valid()) return;

        const url = frmAdjustmentIn.attr("action");
        const method = frmAdjustmentIn.attr("method");
        const data = frmAdjustmentIn.serialize();

        Swal.fire({
            title: 'Saving...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: data
        })
            .done(() => {
                Swal.fire({
                    icon: 'success',
                    title: 'Saved!',
                    text: 'Adjustment IN saved successfully.',
                });

                $('#modalIN').modal('hide'); 
                tblProductAdjustment.ajax.reload();
                frmAdjustmentIn[0].reset();
            })
            .fail(xhr => {
                Swal.fire('Error', xhr.responseText || 'Save failed.', 'error');
            });
    });

    frmAdjustmentOut.on('submit', function (e) {
        e.preventDefault();
        if (!$(this).valid()) return;

        const url = frmAdjustmentOut.attr("action");
        const method = frmAdjustmentOut.attr("method");
        const data = frmAdjustmentOut.serialize();

        Swal.fire({
            title: 'Saving...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: data
        })
            .done(() => {
                Swal.fire({
                    icon: 'success',
                    title: 'Saved!',
                    text: 'Adjustment OUT saved successfully.',
                });

                $('#modalOUT').modal('hide');
                tblProductAdjustment.ajax.reload();
                frmAdjustmentOut[0].reset();
            })
            .fail(xhr => {
                Swal.fire('Error', xhr.responseText || 'Save failed.', 'error');
            });
    });

    $(document).on("click", ".btn_remove_adjustment", function () {
        $(this).closest("tr").remove();
    });
});

async function addAdjustmentInRow(item = {}) {
    const productOptions = await loadProductOptions(item);

    const newRow = `
        <tr class="details_row">
            <td>
                <select name="ProductId" class="form-control product-dropdown" data-productid="${item.ProductId || ''}">
                    ${productOptions}
                </select>
            </td>
            <td>
                <input type="number" name="Quantity" asp-for"Quantity" value="${item.Quantity || ''}" class="form-control" required min="1" step="1"/>
            </td>
            <td>
                <input type="text" name="Action" value="${item.Action || 'IN'}" class="form-control" readonly />
            </td>
            <td>
                 <button type="button"  class="btn btn-danger btn-sm btn_remove_adjustment">Remove</button>
            </td>
        </tr>
    `;

    $("#tbl_adjustment_in tbody").append(newRow);
}

async function addAdjustmentOutRow(item = {}) {
    const productOptions = await loadProductOptions(item);

    const newRow = `
        <tr class="details_row">
            <td>
                <select name="ProductId" class="form-control product-dropdown" data-productid="${item.ProductId || ''}">
                    ${productOptions}
                </select>
            </td>
            <td>
                <input type="number" name="Quantity" value="${item.Quantity || ''}" class="form-control" required />
            </td>
            <td>
                <input type="text" name="Action" value="${item.Action || 'OUT'}" class="form-control" readonly />
            </td>
             <td>
                 <button type="button"  class="btn btn-danger btn-sm btn_remove_adjustment">Remove</button>
            </td>
        </tr>
    `;

    $("#tbl_adjustment_out tbody").append(newRow);
}

async function loadProductOptions(item) {
    try {
        const data = await $.ajax({
            url: '/Product/Inq',
            method: 'GET'
        });

        const selectedId = parseInt(item.ProductId);
        let productOptions = '';

        $.each(data, function (i, product) {
            const selected = parseInt(product.Id) === selectedId ? "selected" : "";
            productOptions += `<option value="${product.Id}" ${selected}>${product.Name}</option>`;
        });

        return productOptions;
    } catch (err) {
        console.error("❌ Failed to load product options:", err);
        Swal.fire('Error', 'Failed to load product list.', 'error');
        return "";
    }
}
