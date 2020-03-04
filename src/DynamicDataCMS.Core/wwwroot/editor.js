var editor = null;
(function () {
    JSONEditor.defaults.options.ajax = true;
    JSONEditor.defaults.options.theme = 'bootstrap4';
    JSONEditor.defaults.options.no_additional_properties = true;
    JSONEditor.defaults.options.disable_collapse = true;
    JSONEditor.defaults.options.disable_edit_json = true;
    JSONEditor.defaults.options.disable_properties = true;
    //JSONEditor.defaults.options.show_opt_in = true;

    JSONEditor.defaults.callbacks = {
        "autocomplete": {
            // This is callback functions for the "autocomplete" editor
            // Note: 1st parameter in callback is ALWAYS a reference to the current editor.
            "search_autocomplete": function search(jseditor_editor, input, test) {

                //Clear id when input is cleared
                if (input.length < 1) {
                    jseditor_editor.parent.editors.id.setValue(null);
                }

                var itemType = jseditor_editor.parent.options.itemType;
                var url = searchApiUrl + '/' + itemType + '?q=' + encodeURI(input);

                return new Promise(function (resolve) {
                    if (input.length < 3) {
                        return resolve([]);
                    }

                    fetch(url).then(function (response) {
                        return response.json();
                    }).then(function (data) {
                        resolve(data);
                    });
                });
            },
            "getResultValue_autocomplete": function getResultValue(jseditor_editor, result) {
                return result.title;
            },
            "onSubmit_autocomplete": function onSubmitValue(jseditor_editor, result) {
                if (result) {
                    jseditor_editor.parent.editors.id.setValue(result.id);
                }
            },
            "renderResult_autocomplete": function (jseditor_editor, result, props) {
                return ['<li ' + props + '>',
                '<div class="title">' + result.title + '</div>',
                    '</li>'].join('');
            }
        }
    };

    window.JSONEditor.defaults.callbacks.button = {
        "clear": function (jseditor, e) {
            var field = jseditor.options.field;
            jseditor.parent.editors[field].setValue('');
            jseditor.parent.editors[field].deactivate();
            jseditor.parent.editors[field].activate();
        }
    };

    // Specify upload handler
    JSONEditor.defaults.callbacks.upload = {
        "uploadHandler" : function (jseditor, type, file, cbs) {
            fileUploadUrl += '?fieldName=' + type.substr(5); //remote root. from typename
            var formData = new FormData();
            formData.set("file", file, file.name);

            fetch(fileUploadUrl, { // Your POST endpoint
                method: 'POST',
                //headers: {
                //  // Content-Type may need to be completely **omitted**
                //  // or you may need something
                //  "Content-Type": "multipart/form-data"
                //},
                body: formData // This is your file object
            }).then(
                response => response.json() // if the response is a JSON object
            ).then(
                filename => {
                    cbs.success(filename);
                    var event = new CustomEvent('uploadFinished', { detail: { id: filename, editor: jseditor } });
                    document.dispatchEvent(event);
                } // Handle the success response object
            ).catch(
                error => console.log(error) // Handle the error response object
            );
        }
    };

    // Initialize the editor
    editor = new JSONEditor(document.getElementById("editor_holder"), {
        iconlib: "fontawesome5",
        schema: schemaJson

    });

    if (setData) {
        editor.on('ready', function () {
            editor.setValue(data);
            jsonLoaded();
        });
    }
})();


function save() {

    // Validate
    var errors = editor.validate();
    if (errors.length) {
        // Not valid
        editor.setOption('show_errors', 'always');
        new Noty({ text: 'Please fix all validation errors.', type: 'warning', timeout: 2000, theme: 'bootstrap-v4', progressBar: true }).show();
        return;
    }

    var data = editor.getValue();

    fetch(saveUrl, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        //mode: 'cors', // no-cors, cors, *same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        //credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json',
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        //redirect: 'follow', // manual, *follow, error
        //referrer: 'no-referrer', // no-referrer, *client
        body: JSON.stringify(data), // body data type must match "Content-Type" header
    })
        .then(response => {
            if (!response.ok) { throw response }
            return response.json()  //we only get here if there is no error
        })
        .then(json => {
            new Noty({ text: 'Save success', type: 'success', timeout: 2000, theme: 'bootstrap-v4', progressBar: true }).show();
        })
        .catch(err => {
            new Noty({ text: 'Save failed', type: 'error', timeout: 2000, theme: 'bootstrap-v4', progressBar: true }).show();
        })

}

function load() {

    return fetch(loadUrl, {
        method: 'GET', // *GET, POST, PUT, DELETE, etc.
        //mode: 'cors', // no-cors, cors, *same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        //credentials: 'same-origin', // include, *same-origin, omit
        //headers: {
        //    'Content-Type': 'application/json',
        //    // 'Content-Type': 'application/x-www-form-urlencoded',
        //},
        //redirect: 'follow', // manual, *follow, error
        //referrer: 'no-referrer', // no-referrer, *client
        //body: JSON.stringify(data), // body data type must match "Content-Type" header
    })
        .then(response => response.json())
        .then(data => { editor.setValue(data) });
}

