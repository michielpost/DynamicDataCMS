function editorChange() {
    var tabs = document.getElementsByClassName("tab-pane");
    if (tabs && tabs.length > 0) {
        tabs[0].classList.add("active");
    }
}

addEventListener('DOMContentLoaded', () => {
    if (!window.editor)
        return;

    editorChange();
    window.editor.on('ready', editorChange);
});


(function () {
    var allimgs = document.images;
    for (var i = 0; i < allimgs.length; i++) {
        allimgs[i].onerror = function () {
            this.style.visibility = "hidden"; // Other elements aren't affected.
        }
        allimgs[i].onload = function () {
            this.style.visibility = ""; // Other elements aren't affected.
        }
    }
})();