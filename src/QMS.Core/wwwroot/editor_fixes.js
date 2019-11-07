function fixUploader(editor) {
    const isImage = editor.schema.links[0].mediaType == 'image';

    if (isImage) editor.uploader.setAttribute('accept', 'image/jpeg, image/png');

    editor.uploader.onchange = () => {
        setTimeout(() => {
            const _btn = editor.container.querySelector('button.json-editor-btn-upload');
            if (isImage) {
                _btn.previousSibling.previousSibling.style.display = 'none';
                _btn.previousSibling.style.display = 'none';
            }
            _btn.style.display = 'none';
            _btn.click();
        }, 100)
    }
};

function editorChange() {
    for (let x in editor.editors) {
        const e = editor.editors[x];
        if (e._fixed) continue;
        e._fixed = true;

        if (e.schema.format == 'url' && e.schema && e.schema.links)
            fixUploader(e);
    }
}

addEventListener('DOMContentLoaded', () => {
    if (!window.editor) return;
    window.editor.on('change', editorChange);
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