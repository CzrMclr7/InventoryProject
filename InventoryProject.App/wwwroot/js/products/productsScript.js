"use strict"
let tblProduct;
let frmProduct = $('#frm-product');
let btnEdit = $("#btnEdit");
let btnDelete = $("#btnDelete");
let btnReset = $("#btnReset");
let btnRefresh = $("#btnRefresh");

$(async function () {
    resetForm();

    tblProduct = $("#tbl-products").DataTable({
        ajax: {
            url: '/Product/Inq',
            dataSrc: ''
        },
        columns: [
            { data: "Name" },
            { data: "Qty" },
            { data: "Price" } 
        ],
        select: true,
        rowId: 'Id',
        processing: true 
    });

    // Enable/disable buttons based on selection 
    tblProduct.on('select deselect draw', function () {
        const selected = tblProduct.rows({ selected: true, search: 'applied' }); 
        const count = selected.count();
        const ids = selected.data().pluck("Id").toArray().toString();
        const names = selected.data().pluck("Name").toArray().join(', ');

        btnEdit.prop("disabled", count !== 1).data("id", ids);
        btnDelete.prop("disabled", count < 1)
            .attr("data-id", ids)
            .attr("data-desc", names);
    });

    // Edit button → load and populate form 
    btnEdit.on('click', async function () {
        const id = $(this).data("id");
        const data = await loadProduct(id);
        if (data) {
            $("[name='Id']").val(data.Id);
            $("[name='Name']").val(data.Name);
            $("[name='Qty']").val(data.Qty);
            $("[name='Price']").val(data.Price);
            $("#frm-product-title").text("Update Product");
        }
    }); 

    // Delete button → Swal confirm + AJAX call inside 
    btnDelete.on('click', function () {
        const ids = $(this).data("id");
        const desc = $(this).data("desc");

        Swal.fire({
            title: 'Are you sure?',
            html: `You are about to delete:<br><b>${desc}</b>`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete!',
            cancelButtonText: 'Cancel',
            preConfirm: () => {
                // Return the AJAX promise so SweetAlert2 waits for it 
                return $.ajax({
                    url: `/Product/Delete?id=${ids}`,
                    method: "DELETE"
                }).then(() => {
                    tblProduct.ajax.reload();
                    resetForm();
                }).catch(xhr => {
                    Swal.showValidationMessage(
                        `Delete failed: ${xhr.responseText || 'Server error'}`
                    );
                });
            }
        }).then(result => {
            if (result.isConfirmed) {
                Swal.fire('Deleted!', 'Product(s) deleted successfully.',
                    'success');
            } 
        });
    });
    // Reset button → clear form 
    btnReset.on('click', function (e) {
        e.preventDefault();
        resetForm();
    });
    // Refresh button → reload DataTable 
    btnRefresh.on('click', function (e) {
        e.preventDefault();
        tblProduct.ajax.reload();
    });
    // Form submit → AJAX + Swal feedback 
    frmProduct.on('submit', function (e) {
        e.preventDefault();
        if (!$(this).valid()) return;

        const url = frmProduct.attr("action");
        const method = frmProduct.attr("method");
        const data = frmProduct.serialize();

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
                Swal.fire('Saved!', 'Product saved successfully.', 'success');
                tblProduct.ajax.reload();
                resetForm();
            })
            .fail(xhr => {
                Swal.fire('Error', xhr.responseText || 'Save failed.', 'error');
            });
    }); 
});

// Reset form and title
function resetForm() {
    frmProduct[0].reset();
    $('#Id').val(0);
    $("#frm-product-title").text("Add Product");
    removeDetail();
}

// Load a product by ID
async function loadProduct(id) {
    try {
        return await $.ajax({ url: `/Product/Get?id=${id}`, method: "GET" });
    } catch (err) {
        console.error("Failed to load product:", err);
        Swal.fire('Error', 'Failed to load product data.', 'error');
        return null;
    }
} 
function removeDetail() {
    let $row = $(this).closest(".details_row");
    $row.remove();
}