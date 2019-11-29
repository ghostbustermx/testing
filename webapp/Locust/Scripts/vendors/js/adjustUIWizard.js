var CURRENT_URL = window.location.href.split('#')[0].split('?')[0],
    $BODY = $('x_content'),
    $MENU_TOGGLE = $('#menu_toggle'),
    $SIDEBAR_MENU = $('#sidebar-menu'),
    $SIDEBAR_FOOTER = $('.sidebar-footer'),
    $LEFT_COL = $('.left_col'),
    $RIGHT_COL = $('.right_col'),
    $NAV_MENU = $('.nav_menu'),
    $FOOTER = $('footer');

function init_sidebar() {
    // TODO: This is some kind of easy fix, maybe we can improve this
    var setContentHeight = function () {
        // reset height
        $RIGHT_COL.css('min-height',
            1036);

        var bodyHeight = 1036,
            footerHeight = $BODY.hasClass('footer_fixed') ? -10 : $FOOTER.height() + 25,
            leftColHeight = $LEFT_COL.eq(1).height() + $SIDEBAR_FOOTER.height(),
            contentHeight = bodyHeight < leftColHeight ? leftColHeight : bodyHeight;

        // normalize content
        contentHeight -= $NAV_MENU.height() + footerHeight;

        $RIGHT_COL.css('min-height', contentHeight);
    };



    // toggle small or large menu 


    // check active menu
    //$SIDEBAR_MENU.find('a[href="' + CURRENT_URL + '"]').parent('li').addClass('current-page');

    //$SIDEBAR_MENU.find('a').filter(function () {
    //    return this.href == CURRENT_URL;
    //}).parent('li').addClass('current-page').parents('ul').slideDown(function () {
    //    setContentHeight();
    //}).parent().addClass('active');


    $(window).smartresize(function () {
        setContentHeight();
    });

    setContentHeight();

    // fixed sidebar
    //if ($.fn.mCustomScrollbar) {
    //    $('.menu_fixed').mCustomScrollbar({
    //        autoHideScrollbar: true,
    //        theme: 'minimal',
    //        mouseWheel: { preventDefault: true }
    //    });
    //}
};

$(document).ready(function () {
    init_sidebar();
});
