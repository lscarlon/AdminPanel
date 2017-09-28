(function ($) {
    function ApplicationRole() {
        var $this = this;

        function initilizeModel() {
            $("#modal-action-application-role").on('loaded.bs.modal', function (e) {

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initilizeModel();
        }
    }
    $(function () {
        var self = new ApplicationRole();
        self.init();
    })
}(jQuery))  

$(document).ready(function () {
    var table = $('#tb_roles').DataTable();

    $('#tb_roles tbody').on('click', 'td', function () {
        var data = table.row($(this).parents('tr')).data();
        if ($(this).closest('table').find('th').eq($(this).index())["0"].innerHTML !== "Action") {
            //alert('You clicked on ' + data[1] + '\'s row');
            $.redirect('../identityuser/index', { 'filterRole': data[0] });
        }
    });
});