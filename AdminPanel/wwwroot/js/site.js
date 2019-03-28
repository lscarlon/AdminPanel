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

$.loadPartialWithScripts = function (url, script) {
    $.ajax({
        url: url + (url.indexOf("?") > -1 ? "&" : "?") + "partial=true",
        type: 'GET',
        dataType: 'html',
        async: false,
        success: function (data) {
            $('.content-wrapper').html(data);
        }
    });
    $.getScript(script)
        .done(function () { })
        .fail(function () { });
    //reset active menu
    $('.sidebar-menu > #left-sidebar-menu').each(function (i, li) {
        if (url.toLowerCase().startsWith($(li).children('a').first().attr("href").toLowerCase())) {
            $(li).addClass("active");
        } else {
            $(li).removeClass("active");
        }
    });
    $('.sidebar-menu > .treeview').each(function (i, li) {
        var countActiveSub = 0;
        $(li).find('ul > li').each(function (j, subli) {
            if (url.toLowerCase().startsWith($(subli).children('a').first().attr("href").toLowerCase())) {
                $(subli).addClass("active");
                countActiveSub += 1;
            } else {
                $(subli).removeClass("active");
            }
        });
        if (countActiveSub>0) {
            $(li).addClass("active");
            $(li).children('ul').addClass("menu-open");
            $(li).children('ul').css({ display: "block" });
        } else {
            $(li).removeClass("active");
            $(li).children('ul').removeClass("menu-open");
            $(li).children('ul').css({ display: "none" });
        };
    });
};

$('#SignOutLink').click(function () {
    $('#logoutForm').submit();
});

$('.sidebar-menu .treeview-menu>li>a[href!="#"],.sidebar-menu li>a[href!="#"]').click(function (event) {
    event.preventDefault();
    $.loadPartialWithScripts($(this).attr("href"), $(this).parent().attr("scriptToRun"));
});
