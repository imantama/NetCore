var table = null;
$(document).ready(function () {
    //debugger;
    table = $('#department').DataTable({
        "processing": true,
        "responsive": true,
        "pagination": true,
        "stateSave": true,
        "ajax": {
            url: "/departments/LoadDepart",
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
                    $('[data-toggle="tooltip"]').tooltip();
                    return '<button class="btn btn-link btn-md btn-warning " data-placement="left" data-toggle="tooltip" data-animation="false" title="Edit" onclick="return GetById(' + row.id + ')" ><i class="fa fa-lg fa-edit"></i></button>'
                        + '&nbsp;'
                        + '<button class="btn btn-link btn-md btn-danger" data-placement="right" data-toggle="tooltip" data-animation="false" title="Delete" onclick="return Delete(' + row.id + ')" ><i class="fa fa-lg fa-times"></i></button>'
                }
            }
        ]
    });
    table.on('order.dtable search.dtable', function () {
        table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
});
function ClearScreen() {
    $('#id').val('');
    $('#name').val('');
    $('#update').hide();
    $('#add').show();
}

function GetById(id) {
    //debugger;
    $.ajax({
        url: "/departments/GetById/",
        data: { id: id }
    }).then((result) => {
        //debugger;
        $('#id').val(result.id);
        $('#name').val(result.name);
        $('#add').hide();
        $('#update').show();
        $('#myModal').modal('show');
    })
}

function Save() {
    //debugger;
    var Dept = new Object();
    Dept.id = 0;
    Dept.name = $('#name').val();
    $.ajax({
        type: 'POST',
        url: "/departments/InsertOrUpdate/",
        cache: false,
        dataType: "json",
        data: Dept
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
    debugger;
    var Dept = new Object();
    Dept.id = $('#id').val();
    Dept.name = $('#name').val();
    $.ajax({
        type: 'POST',
        url: "/Departments/InsertOrUpdate/",
        cache: false,
        dataType: "json",
        data: Dept
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
    }).then((result) => {
        if (result.value) {
            //debugger;
            $.ajax({
                url: "/Departments/Delete/",
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