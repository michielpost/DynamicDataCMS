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

        if (e.simplemde_instance)
            markdownEditors.push(e.simplemde_instance);
    }
}

addEventListener('DOMContentLoaded', () => {
    if (!window.editor) return;
    window.editor.on('change', editorChange);
});