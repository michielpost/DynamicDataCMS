document.addEventListener('uploadFinished', submitToMicrio, false);

function submitToMicrio(e) {
    console.log('Uploaded file: ' + e.detail.id);

    var micrioField = jseditor_editor.parent.options.micrioField;
    if (!micrioField)
        return;

    var micrioUrl = '/cms/api/micrio/add';
    fetch(micrioUrl, { // Your POST endpoint
        method: 'POST',
        body: e.detail // This is the file id
    }).then(
        response => response.json() // if the response is a JSON object
    ).then(
        micrioResponse => {
            console.log(micrioResponse);
            if (jseditor_editor.parent.editors[micrioField]) {
                jseditor_editor.parent.editors[micrioField].setValue(micrioResponse.shortId);
            }
        } // Handle the success response object
    ).catch(
        error => console.log(error) // Handle the error response object
    );

}