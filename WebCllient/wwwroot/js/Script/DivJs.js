var table = null;
var arrDepart = [];

$(document).ready(function () {
    //debugger;
    table = $("#division").DataTable({
        "processing": true,
        "responsive": true,
        "pagination": true,
        "stateSave": true,
        "ajax": {
        
            url: "/divisions/LoadDiv",
            type: "GET",
            dataType: "json",
            dataSrc: "",
        },
        "columns": [
            {
                "data": null,
                "width": "50px",
                "sClass": "text-center",
                "orderable": false,
            },
            { "data": "name" },
            { "data": "department.name" },
            {
                "data": "createDate",
                'render': function (jsonDate) {
                    var date = new Date(jsonDate);
                    if (date.getFullYear() != 0001) {
                        return moment(date).format('lll')
                    }
                }
            },
            {
                "data": "updateDate",
                'render': function (jsonDate) {
                    var date = new Date(jsonDate);
                    if (date.getFullYear() != 0001) {
                        return moment(date).format('lll');
                    }
                    else {
                        return "Not Updated Yet"
                    }
                }
            },
            {
                "sortable": false,
                "render": function (data, type, row) {
                    //console.log(row);
                    $('[data-toggle="tooltip"]').tooltip();
                    return '<button class="btn btn-outline-warning btn-circle" data-placement="left" data-toggle="tooltip" data-animation="false" title="Edit" onclick="return GetById(' + row.id + ')" ><i class="fa fa-lg fa-edit"></i></button>'
                        + '&nbsp;'
                        + '<button class="btn btn-outline-danger btn-circle" data-placement="right" data-toggle="tooltip" data-animation="false" title="Delete" onclick="return Delete(' + row.id + ')" ><i class="fa fa-lg fa-times"></i></button>'
                }
            }
        ],
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" data-placement="Bottom" data-toggle="tooltip" data-animation="false" title="PDF" ></i>',
                className: 'btn btn-danger',
                title: 'Division List',
                filename: 'cek ' + moment(),
                exportOptions: {
                    //format: {
                    //    body: function (data, row, column, node) {
                    //        // Strip $ from salary column to make it numeric
                    //        //return column === 5 ? data.replace(/[$,]/g, '') : data;
                    //        return column === 2 ? data.replace(/[$,]/g, '') : data;
                    //    }
                    //},
                    columns: [0, 1, 2, 3, 4],
                    search: 'applied',
                    order: 'applied',
                    modifier: {
                        page: 'current',
                    },
                },
                customize: function (doc) {
                    //doc.content.splice(1, 0, {
                    //    margin: [0, 0, 0, 12],
                    //    alignment: 'center',
                    //});
                    debugger;
                    var rowCount = doc.content[1].table.body.length;
                    for (i = 1; i < rowCount; i++) {
                        doc.content[1].table.body[i][2].alignment = 'center';
                    };
                    doc.content[1].table.body[0][0].text = 'No.';
                    doc.content[1].table.body[0][2].text = 'Department';
                    doc['footer'] = (function (page, pages) {
                        return {
                            columns: [
                                'This is your left footer column',
                                {
                                    // This is the right column
                                    alignment: 'right',
                                    text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                }
                            ],
                            margin: [10, 0]
                        }
                    });
                }
            },
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel" data-placement="Bottom" data-toggle="tooltip" data-animation="false" title="Excel" ></i>',
                className: 'btn btn-success',
                title: 'Division List',
                filename: 'cek ' + moment(),
                exportOptions: {
                    columns: [0, 1, 2, 3, 4],
                    search: 'applied',
                    order: 'applied',
                    modifier: {
                        page: 'current',
                    },
                },
                customize: function (excel) {
                    debugger;
                    var sheet = excel.xl.worksheets['sheet1.xml'];
                    // jQuery selector to add a border
                    //$('col c[r*="10"]', sheet).attr('s', '25');
                    $('c[r=A2] t', sheet).text('No.');
                    $('c[r=C2] t', sheet).text('Department');
                }
            },
            {
                extend: 'csvHtml5',
                text: '<i class="fas fa-file-csv" data-placement="Bottom" data-toggle="tooltip" data-animation="false" title="CSV"></i>',
                className: 'btn btn-info',
                title: 'Division List',
                filename: 'cek ' + moment(),
                exportOptions: {
                    columns: [0, 1, 2, 3, 4],
                    search: 'applied',
                    order: 'applied',
                    modifier: {
                        page: 'current',
                    },
                },
                customize: function (excel) {
                    debugger;
                    var sheet = excel.xl.worksheets['sheet1.xml'];
                    // jQuery selector to add a border
                    //$('col c[r*="10"]', sheet).attr('s', '25');
                    $('c[r=A2] t', sheet).text('No.');
                    $('c[r=C2] t', sheet).text('Department');
                }
            },
            {
                extend: 'print',
                text: '<i class="fas fa-print" data-placement="Bottom" data-toggle="tooltip" data-animation="false" title="Print"></i>',
                className: 'btn btn-default',
                title: 'Division List',
            }
        ],
        initComplete: function () {
            this.api().columns(2).every(function () {
                var column = this;
                var select = $('<select><option value""></option></select>')
                    .appendTo($(column.header()).empty())
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );
                        column
                            .search(val ? '^' + val + '$' : '', true, false).draw();
                    });
                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>')
                });
            });
        }
    });
    table.on('order.dtable search.dtable', function () {
        table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
});

function ClearScreen() {
    $('#Id').val('');
    $('#Name').val('');
    $('#update').hide();
    $('#add').show();
}

function LoadDepart(element) {
    //debugger;
    if (arrDepart.length === 0) {
        $.ajax({
            type: "Get",
            url: "/departments/LoadDepart",
            success: function (data) {
                arrDepart = data;
                renderDepart(element);
            }
        });
    }
    else {
        renderDepart(element);
    }
}

function renderDepart(element) {
    //debugger;
    var $option = $(element);
    $option.empty();
    $option.append($('<option/>').val('0').text('Select Department').hide());
    $.each(arrDepart, function (i, val) {
        $option.append($('<option/>').val(val.id).text(val.name))
    });
}

LoadDepart($('#DepartOption'))

function GetById(id) {
    //debugger;
    $.ajax({
        url: "/divisions/GetById/",
        data: { id: id }
    }).then((result) => {
        //debugger;
        $('#Id').val(result.id);
        $('#Name').val(result.name);
        $('#DepartOption').val(result.departmentId)
        $('#add').hide();
        $('#update').show();
        $('#myModal').modal('show');
    })
}

function Save() {
    //debugger;
    var Div = new Object();
    Div.id = 0;
    Div.name = $('#Name').val();
    Div.departmentId = $('#DepartOption').val();
    $.ajax({
        type: 'POST',
        url: "/divisions/InsertOrUpdate/",
        cache: false,
        dataType: "JSON",
        data: Div
    }).then((result) => {
        if (result.statusCode == 200) {
            Swal.fire({
                position: 'center',
                icon: 'success',
                title: 'Data inserted Successfully',
                showConfirmButton: false,
                timer: 1500,
            })
            table.ajax.reload(null, false);
        } else {
            Swal.fire('Error', 'Failed to Input', 'error');
            ClearScreen();
        }
    })
}

function Update() {
    //debugger;
    var Div = new Object();
    Div.id = $('#Id').val();
    Div.name = $('#Name').val();
    Div.departmentId = $('#DepartOption').val();
    $.ajax({
        type: 'POST',
        url: "/divisions/InsertOrUpdate/",
        cache: false,
        dataType: "JSON",
        data: Div
    }).then((result) => {
        //debugger;
        if (result.statusCode == 200) {
            Swal.fire({
                position: 'center',
                icon: 'success',
                title: 'Data Updated Successfully',
                showConfirmButton: false,
                timer: 1500,
            });
            table.ajax.reload(null, false);
        } else {
            Swal.fire('Error', 'Failed to Input', 'error');
            ClearScreen();
        }
    })
}

function Delete(id) {
    //debugger;
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
    }).then((resultSwal) => {
        if (resultSwal.value) {
            //debugger;
            $.ajax({
                url: "/divisions/Delete/",
                data: { id: id }
            }).then((result) => {
                //debugger;
                    if (result.statusCode == 200) {
                    //debugger;
                    Swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Delete Successfully',
                        showConfirmButton: false,
                        timer: 1500,
                    });
                    table.ajax.reload(null, false);
                } else {
                    Swal.fire('Error', 'Failed to Delete', 'error');
                    ClearScreen();
                }
            })
        };
    });
}