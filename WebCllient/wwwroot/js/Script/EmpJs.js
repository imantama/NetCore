var table = null;


$(document).ready(function () {
    //debugger;
    table = $("#employee").DataTable({
        "processing": true,
        "responsive": true,
        "pagination": true,
        "stateSave": true,
        "ajax": {

            url: "/employees/LoadEmploy",
            type: "GET",
            dataType: "json",
            dataSrc: "",
        },
        "columns": [
            {
                "data": "employeeId",
                render: function (data, type, row, meta) {
                    //console.log(row);
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "employeeName" },
            { "data": "phone" },
            { "data": "address" },
            {
                "data": "createDate",
                "searchable": false,
                'render': function (jsonDate) {
                    var date = new Date(jsonDate);
                    if (date.getFullYear() != 0001) {
                        return moment(date).format('lll')
                    }
                }
               //searchable: false
            },
            {
                "sortable": false,
                "render": function (data, type, row, meta) {
                    //console.log(row);
                    $('[data-toggle="tooltip"]').tooltip();
                    return '<button class="btn btn-outline-info btn-circle" data-placement="left" data-toggle="tooltip" data-animation="false" title="Detail" onclick="return GetById(' + meta.row + ')" ><i class="fa fa-lg fa-info"></i></button>'
                        + '&nbsp;'+
                    '<button class="btn btn-outline-danger btn-circle" data-placement="right" data-toggle="tooltip" data-animation="false" title="Delete" onclick="return Delete(' + meta.row + ')" ><i class="fa fa-lg fa-times"></i></button>'
                    //'<button class="btn btn-outline-warning btn-circle" data-placement="left" data-toggle="tooltip" data-animation="false" title="edit" onclick="return getbyid(' + row.id + ')" ><i class="fa fa-lg fa-edit"></i></button>'
                    //    + '&nbsp;'
                    //    +
                }
            }
        ]
    });
});

function ClearScreen() {
    $('#Id').val('');
    $('#Name').val('');
    $('#update').hide();
    $('#add').show();
}

function Delete(number) {
    var id = table.row(number).data().employeeId;
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
                url: "/employees/Delete/",
                data: { Id: id }
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

function GetById(number) {
    //debugger;
    //console.log(table.row(number).data());
    var id = table.row(number).data().employeeId;
    $.ajax({
        url: "/employees/GetById/",
        data: { Id: id }
    }).then((result) => {
        //debugger;
        $('#Id').append(result.employeeId);
        $('#Name').append(result.employeeName);
        $('#Address').append(result.address);
        $('#Phone').append(result.phone);

        var date = new Date(result.createDate);
        $('#HireDate').append(moment(date).format('lll'));

        //$('#add').hide();
        $('#update').show();
        $('#myModal').modal('show');
    })
}