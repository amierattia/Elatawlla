    document.addEventListener("contextmenu", function(event) {
    event.preventDefault();
});

    document.addEventListener("keydown", function(event) {
    if (event.key === "F12" ||
    (event.ctrlKey && event.shiftKey && (event.key === "I" || event.key === "J")) ||
    (event.ctrlKey && event.key === "U")) {
    event.preventDefault();
}
});

    (function() {
    var devtools = /./;
    devtools.toString = function() {
    window.location.replace("about:blank");
};
    console.log("%c", devtools);
})();
