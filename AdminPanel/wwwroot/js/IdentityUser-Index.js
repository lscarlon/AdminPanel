(function ($) {
    $(function () {
        $("#modal-action-user")
            //.on('loaded.bs.modal', function (e) {
            //})
            //.on('show.bs.modal', function (e) {
            //})
            .on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
    })
}(jQuery))

$(document).ready(function () {
    var table = $('#tb_users').DataTable({
        'autoWidth': false
    });

});