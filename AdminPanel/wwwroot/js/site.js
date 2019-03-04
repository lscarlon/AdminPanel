

//$("li[id^='left-sidebar-menu']").on("click", function () {
//    var $this = $(this);
//    var checkElement = $this.next();
//    var parent_li = $this.parent("ul");
//    if (!$this.hasClass('active')) {

//        $(".content-wrapper").empty();

//        var url = $this.attr("controllerInfo");
//        $.get(url).done(function (response) {
//            $(".content-wrapper").html(response).after(function () {

//                var scripturl = $this.attr("scriptToRun");

//                //Check if a script is defined for the controller
//                if (scripturl) {
//                    //Handle multiple scripts.
//                    $.getMultiScripts(scripturl.split(',')).done(function () {
//                        // all scripts loaded
//                        window.console.log('Done loading scripts');
//                    }).fail(function (jqxhr, settings, exception) { //fail(function (error) {
//                        // one or more scripts failed to load
//                        window.console.log('One or more scripts failed to load');
//                    }).always(function () {
//                        // always called, both on success and error
//                    });
//                }
//            }
//                );

//            //Clear any previous menu option
//            $("li[id^='left-sidebar-menu']").removeClass('active');


//            if (!parent_li.hasClass('treeview-menu')) {


//                var treeElement = $(".treeview.active"); //Parent treeview
//                if (treeElement) {

//                    treeElement.removeClass('active');

//                    var firstchild = treeElement.children('ul')
//                    if (firstchild.lenght !== 0) {
//                        var animationSpeed = $.AdminLTE.options.animationSpeed;
//                        //Close the menu
//                        firstchild.slideUp(animationSpeed, function () {
//                            firstchild.removeClass('menu-open');

//                        });
//                    }                    
//                }
//            }

//            //Set that the menu option is now active
//            $this.addClass("active");
//        });

//    };
//});


//http://stackoverflow.com/questions/11803215/how-to-include-multiple-js-files-using-jquery-getscript-method
$.getMultiScripts = function (arr, path) {
    var _arr = $.map(arr, function (scr) {
        return $.getScript((path || "") + scr);
    });

    _arr.push($.Deferred(function (deferred) {
        $(deferred.resolve);
    }));

    return $.when.apply($, _arr);
};

$('#SignOutLink').click(function () {
    $('#logoutForm').submit();
});
