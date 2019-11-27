document.addEventListener('uploadFinished', submitToMicrio, false);

function submitToMicrio(e) {
    alert('wow');
    console.log('Uploaded file: ' + e.detail);
}