"use strict"
let tblSales;
let tblSalesDetails;
let frmSales = $('#frm-sales');
let btnEdit = $("#btnEdit");
let btnDelete = $("#btnDelete");
let btnReset = $("#btnReset");
let btnRefresh = $("#btnRefresh");
let productOptions = "";

let tbl_details_tbody = $("#tbl_sales_details tbody");

const btn_add_detail = $("#btn_add_detail");

$(async function () {
    resetForm();
    //initializeDetailSelectize();

    tblSales = $("#tbl_sales").DataTable({
        ajax: {
            url: '/Sales/Inq',
            dataSrc: ''
        },
        columns: [
            { data: "Name" },
            { data: "Quantity" },
            { data: "TotalPrice" }
        ],
        select: true,
        rowId: 'Id',
        processing: true
    });

    // Enable/disable buttons based on selection
    tblSales.on('select deselect draw', function () {
        const selected = tblSales.rows({ selected: true, search: 'applied' });
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
        resetForm();
        const id = $(this).data("id");
        const data = await loadSales(id);
        if (data) {
            $("[name='Sales.Id']").val(data.Id);
            $("[name='Sales.Name']").val(data.Name);
            $("[name='Sales.Quantity']").val(data.Quantity);
            $("[name='Sales.TotalPrice']").val(data.TotalPrice);
            $("#frm-sales-title").text("Update Sales");
        }


        try {
            const response = await $.ajax({
                url: baseUrl + "SalesDetail/Inq",
                data: { id: data.Id },
                method: "GET"
            });

            if (response) {
                for (let data of response) {
                    await addDetailRow(data); 
                }
            }
        } catch (err) {
            console.error("Failed to load details:", err);
        }

        //tblSalesDetails = $("#tbl_sales_details").DataTable({
        //    ajax: {
        //        url: `/SalesDetail/Inq`,
        //        data: {
        //            id: data.Id
        //        },
        //        dataSrc: ''
        //    },
        //    columns: [
        //        { data: "Name" },
        //        { data: "Quantity" },
        //        { data: "Price" }

        //    ]
        //});


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
                    url: `/Sales/Delete?id=${ids}`,
                    method: "DELETE"
                }).then(() => {
                    tblSales.ajax.reload();
                    resetForm();
                }).catch(xhr => {
                    Swal.showValidationMessage(
                        `Delete failed: ${xhr.responseText || 'Server error'}`
                    );
                });
            }
        }).then(result => {
            if (result.isConfirmed) {
                Swal.fire('Deleted!', 'Sales deleted successfully.',
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
        tblSales.ajax.reload();
    });
    // Form submit → AJAX + Swal feedback
    frmSales.on('submit', function (e) {
        e.preventDefault();
        if (!$(this).valid()) return;

        const url = frmSales.attr("action");
        const method = frmSales.attr("method");
        const formData = new FormData(this);

        Swal.fire({
            title: 'Saving...',
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: url,
            method: method,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
        })
            .done(() => {
                Swal.fire('Saved!', 'Sales saved successfully.', 'success');
                tblSales.ajax.reload();
                resetForm();
            })
            .fail(xhr => {
                Swal.fire('Error', xhr.responseText || 'Save failed.', 'error');
            });
    });

    // Add detail button → add a new detail row
    btn_add_detail.on("click", async function () {
        await addDetailRow();

    });

    $(document).on("click", ".btn_remove_detail", function () {
        $(this).closest("tr").remove();
    });

});

// Reset form and title
function resetForm() {
    frmSales[0].reset();
    $('#Id').val(0);
    $("#frm-sales-title").text("Add Sales");
    $(".details_row").remove()
}
// Load a sales by ID
async function loadSales(id) {
    try {
        return await $.ajax({ url: `/Sales/Get?id=${id}`, method: "GET" });
    } catch (err) {
        console.error("Failed to load sales:", err);
        Swal.fire('Error', 'Failed to load sales data.', 'error');
        return null;
    }
}

async function addDetailRow(item = {}) {
    let index = $("#tbl_sales_details tbody tr").length;

    const productOptions = await loadProductOptions(item);

    let newRow = `
            <tr class="details_row">
                <td>
                    <input type="hidden" name="SalesDetails[${index}].Id" value="${item.Id ?? 0}" class="form-control" />
                    <input type="hidden" name="SalesDetails[${index}].SalesId" value="${item.SalesId ?? 0}" class="form-control" />
                    <select name="SalesDetails[${index}].ProductId" class="form-control product-dropdown"> data-productid="${item.ProductId}"
                        ${productOptions}
                    </select>
                </td>
                <td>
                    <input type="number" name="SalesDetails[${index}].Quantity" value="${item.Quantity}" class="form-control" required min="1" step="1"/>
                </td>
                <td>
                    <input type="number" name="SalesDetails[${index}].Price" value="${item.Price}" class="form-control" id="sales_detail_price" required />
                </td>
                <td>
                    <button type="button"  class="btn btn-danger btn-sm btn_remove_detail">Remove</button>
                </td>
            </tr>`;

    $("#tbl_sales_details tbody").append(newRow);
}

function updateTotalPrice() {
    let totalPrice = 0;

    // Loop through all quantity inputs inside the sales details table
    $("#tbl_sales_details tbody tr").each(function () {
        let price = parseFloat($(this).find("input[name*='Price']").val()) || 0;
        totalPrice += price;
    });

    // Update the disabled input
    $("#sales_totalprice").val(totalPrice);
    $("#sales_total_price").val(totalPrice);
}

function updateTotalQty() {
    let totalQty = 0;

    // Loop through all quantity inputs inside the sales details table
    $("#tbl_sales_details tbody tr").each(function () {
        let qty = parseFloat($(this).find("input[name*='Quantity']").val()) || 0;
        totalQty += qty;
    });

    // Update the disabled input
    $("#sales_total_qty").val(totalQty);
}

async function updateDetailPrice(id, qty, row) {
    let detailPrice = 0;

    $.ajax({ url: `/Product/Get?id=${id}`, method: "GET" })

    const data = await $.ajax({
        url: `/Product/Get?id=${id}`,
        method: 'GET'
    });

    //let price = parseFloat($(this).find("input[name*='Qty']").val()) || 0;
    detailPrice = qty * data.Price;

    row.find("#sales_detail_price").val(detailPrice.toFixed(2)).trigger("input");
}


// Get price change event
$(document).on("input", "input[name*='Price']", function () {
    updateTotalPrice();
});

$(document).on("input", "input[name*='Quantity']", function () {
    const row = $(this).closest("tr");

    const qty = parseFloat(row.find("input[name*='Quantity']").val());
    const productId = parseFloat(row.find("select[name*='.ProductId']").val());

    //const productId = $(this).closest("tr").find("select[name*='.ProductId']").val();
    //const qty = row.find("input[name*='Quantity']").val();
    updateDetailPrice(productId, qty, row);
    updateTotalQty();
});

async function loadProductOptions(item) {
    ///    if (productOptions != '') return "";
    if (item.ProductId != null) {

        try {
            const data = await $.ajax({
                url: '/Product/Inq',
                method: 'GET'
            });

            const selectedId = parseInt(item.ProductId);

            $.each(data, function (i, product) {
                let selected = parseInt(product.Id) === selectedId ? "selected" : "";
                productOptions += `<option value="${product.Id}" ${selected}>${product.Name}</option>`;
            });

            return productOptions;
        } catch (err) {
            console.error("❌ Failed to load product options:", err);
            Swal.fire('Error', 'Failed to load product list.', 'error');
            return "";
        }
    }
    else {
        try {
            const data = await $.ajax({
                url: '/Product/Inq',
                method: 'GET'
            });
            productOptions = '';
            $.each(data, function (i, item) {
                productOptions += `<option value="${item.Id}">${item.Name}</option>`;
            });
            return productOptions;
        } catch (err) {
            console.error("❌ Failed to load product options:", err);
            Swal.fire('Error', 'Failed to load product list.', 'error');
            return "";
        }
    }
}

function buildDetailRow(item = {}) {

    let count = $(".details_row").length;
    let elementToAdd = "";

    elementToAdd = `
        <tr class="details_row">
            <td class="align-middle" style="min-width: 150px;">
                <input type="hidden" name="SalesDetails[${count}].Id" id="SalesDetails_Id_[${count}]" value="${item.Id ?? 0}"/>
                <input type="hidden" name="SalesDetails[${count}].SalesId" id="SalesDetails_SalesId_[${count}]" value="${item.SalesId ?? 0}"/>
                <select name="SalesDetails[${count}].ProductId" class="form-control form-control-sm" id="SalesDetails_ProductId_[${count}]">
                    <option value="">Select Product...</option>
                </select>
            </td>
            <td class="align-middle">
                <input type="number" name="SalesDetails[${count}].Price" id="SalesDetails_Price_[${count}]" class="form-control" value="${item.Price}"/>
            </td>
            <td class="align-middle">
                <input type="number" name="SalesDetails[${count}].Quantity" id="SalesDetails_Quantity_[${count}]" class="form-control" value="${item.Quantity}"/>
            </td>
            <td class="align-middle text-center">
                <button type="button" class="btn btn-danger btn-sm btn_remove_detail">Remove</button>
            </td>
        </tr>
    `;

    $(tbl_details_tbody).append(elementToAdd);

    let selectizeDropdown, $selectizeDropdown;

    $selectizeDropdown = $(`[name='SalesDetails[${count}].ProductId']`).selectize({
        valueField: 'Id',
        labelField: 'Name',
        searchField: 'Name',
        dropdownParent: "body",
        preload: true,
        load: function (query, callback) {
            $.ajax({
                url: baseUrl + "Product/Inq/",
                success: function (results) {
                    try {
                        callback(results);
                    } catch (e) {
                        callback();
                    }
                },
                error: function () {
                    callback();
                }
            });
        },
        render: {
            item: function (item, escape) {
                // Show Name inside input with truncation
                return (`<div class='text-truncate' style='max-width:90%;'>${escape(item.Name)}</div>`);
            },

            // Customize how the dropdown options appear
            option: function (item, escape) {
                return (`<div class='py-1 px-2'>${escape(item.Name)}</div>`);
            }
        }
    });

    // Get the actual Selectize instance (if needed later)
    selectizeDropdown = $selectizeDropdown[0].selectize;

    selectizeDropdown.on("load", function () {
        selectizeDropdown.off("load");
        selectizeDropdown.setValue(item.ProductId);
    });
}

//function addNewDetailRow() {
//    $.ajax({
//        url: '/Product/Inq',
//        method: 'GET',
//        success: function (data) {
//            // Build the product dropdown options once
//            $.each(data, function (i, item) {
//                productOptions += `<option value="${item.Id}">${item.Name}</option>`;
//            });
//        },
//        error: function () {
//            alert('Failed to load products.');
//        }
//    });

//    // 2. Set up "Add Detail" button
//    $("#btn_add_detail").on("click", function () {
//        if (productOptions === "") {
//            alert("Products are still loading. Please wait...");
//            return;
//        }

//        let newRow = `
//            <tr class="details_row">
//                <td>
//                    <input type="hidden" name="SalesDetails[].Id" class="form-control" />
//                    <input type="hidden" name="SalesDetails[].SalesId" class="form-control" />
//                    <select name="SalesDetails[].ProductId" class="form-control product-dropdown"> 
//                        ${productOptions}
//                    </select>
//                </td>
//                <td>
//                    <input type="number" name="SalesDetails[].Quantity" class="form-control" />
//                </td>
//                <td>
//                    <input type="number" name="SalesDetails[].Price" class="form-control" />
//                </td>
//                <td>
//                    <button type="button"  class="btn btn-danger btn-sm btn_remove_detail">Remove</button>
//                </td>
//            </tr>`;

//        $("#tbl_sales_details tbody").append(newRow);
//    });

//    // 3. Remove row on click
//    $(document).on("click", ".btn_remove_detail", function () {
//        $(this).closest("tr").remove();
//    });

//}

// not applicable for now
function initializeDetailPlugins() {
    let productConfig = selectizeProductConfig;

    productConfig.dropdownParent = null;

    productConfig.onChange = function (value) {
        var dropdown = this;
        var value = dropdown.getValue();
        var data = this.options[value];

        if (data == null) return;

        $(dropdown.$control).closest("tr").find(".normalBalanceInput").val(data.NormalBalance);

        let objectName = "SalesDetails";
        let rowClass = "details_row";
    }
}

$(`[id^="Details_EwtTypeId_["]`).each(function () {
    var dropdown, $dropdown;
    var itemVal = $(this).attr("data-value");

    $dropdown = $(this).selectize(selectizeEwtTypeConfig);

    dropdown = $dropdown[0].selectize;
    dropdown.on('load', function (options) {
        dropdown.setValue(itemVal || "");
        dropdown.off("load");
    });
});

$(document).on("click", ".btn_remove_detail", function (e) {
    e.preventDefault();

    removeDetail(this);
    updateTotalPrice();
    updateTotalQty();


    //$row.css({
    //    transition: "opacity 0.6s, transform 0.6s",
    //    transform: "scale(0.1)",
    //    opacity: 0
    //});

    //setTimeout(() => {

    // rebindValidators();
    //    totalSumItems();
    //    totalSumEntries();
    //}, 300);
});

function removeDetail(item) {
    let $row = $(item).closest(".details_row");
    $row.remove();
}

//function rebindValidators() {
//    let $form = $(frmSales);
//    let id = $("[name='SalesDetails_Id']").val();

//    $form.unbind();
//    $form.data("validator", null);
//    $.validator.unobtrusive.parse($form);
//    $form.validate($form.data("unobtrusiveValidation").options);
//    $form.data("validator").settings.ignore = "";

//    //logValidationMessages("#frmSales");

//    $form.on("submit", async function (e) {
//        e.preventDefault();

//        fixElementSequence(details_row); // fix dynamic row ordering
//        let formData = new FormData(this);

//        // Frontend validation
//        if (!$form.valid()) {
//            //radioBtnValidator();
//            toastr.error("Please fill out all required fields.", "Validation Error");
//            return;
//        }

//        // Comment check for update mode
//        //if (id != 0) {
//        //    const shouldBlock = await commentManager.hasPendingComment();
//        //    if (shouldBlock) return;
//        //}

//        // Custom business logic validation
//        //if (!customValidateForm()) return;

//        // Submit via AJAX
//        submitForm($form, formData);
//    });
//}