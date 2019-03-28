(function ($) {
    $(function () {
        $("#modal-action-application-role")
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
    var table = $('#tb_roles').DataTable({
        'autoWidth': false
    });

    $('#tb_roles tbody').on('click', 'td', function () {
        var data = table.row($(this).parents('tr')).data();
        if ($(this).closest('table').find('th').eq($(this).index())["0"].innerHTML !== "Action") {
            $.loadPartialWithScripts("/identityuser/index?filterRole=" + data[0], "/js/IdentityUser-Index.js");
        }
    });

});