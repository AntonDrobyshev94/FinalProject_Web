$(document).ready(function () {
    $('.block-title').click(function () {
        $(this).next('.block-text').slideToggle(300);
    });
});