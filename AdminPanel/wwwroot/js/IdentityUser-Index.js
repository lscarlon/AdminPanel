(function ($) {
    function User() {
        var $this = this;

        function initilizeModel() {
            $("#modal-action-user").on('loaded.bs.modal', function (e) {

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initilizeModel();
        }
    }
    $(function () {
        var self = new User();
        self.init();
    })
}(jQuery))  

$(document).ready(function () {
    var table = $('#tb_users').DataTable();

    //$('#tb_users tbody').on('click', 'td', function () {
    //    var data = table.row($(this).parents('tr')).data();
    //    if ($(this).closest('table').find('th').eq($(this).index())["0"].innerHTML !== "Action") {
    //        //alert('You clicked on ' + data[1] + '\'s row');
    //        $.redirect('../IdentityRole/Index', { 'filterUser': data[0] });
    //    }
    //});
});