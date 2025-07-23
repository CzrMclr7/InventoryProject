"use strict"

$(function () {

    var tbl_report;
    //const load_tbl_report = () => {
        tbl_report = $("#tbl_product_summary_report").DataTable({
            ajax: {
                url: baseUrl + "Report/ProductsSummaryReportData",
                //data: function (d) {
                //    //d.DateFrom = $("[name='ReportFilter.DateFrom']").val().toString();
                //    //d.DateTo = $("[name='ReportFilter.DateTo']").val().toString();
                //    //d.AsOfDate = $("[name='ReportFilter.AsOfDate']").val().toString();
                //    //d.DateRangeType = $("[name='ReportFilter.DateRangeType']:checked").val();
                //    //d.Accounts = $("[name='ReportFilter.Accounts']").val().toString();
                //    //d.Vendors = $("[name='ReportFilter.Vendors']").val().toString();
                //    //d.DateType = $("[name='ReportFilter.DateType']:checked").val();
                //},
                //dataSrc: ""
                dataSrc: ''
                    },
            //columns: [
            //    { data: "Name" },
            //    { data: "Qty" },
            //    { data: "Price" }
            //],
            
            language: {
                emptyTable: "No Products Record",
                processing: "<div class='text-center'><span class='spinner-border spinner-border-sm'></span> Loading...</div>"
            },
            columns: [
                {
                    data: "Id",
                    visible: false,
                },
                {
                    data: "Name",
                    class: "text-center",
                    //render: function (data, row, type) {
                    //    return `<a href="${baseUrl}AccountsPayable/Details/${data}" target="_blank">${data}</a>`;
                    //}
                },
                {
                    data: "Qty"
                },
                {
                    data: "Price",
                    class: "text-right",
                    //render: function (data, row, type) {
                    //    return convertDate(data);
                    //}
                },
            ],
        });
    //};
});