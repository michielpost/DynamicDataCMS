function ready(callbackFunc) {
    if (document.readyState !== 'loading') {
        callbackFunc();
    } else {
        document.addEventListener('DOMContentLoaded', callbackFunc);
    }
}

//Depth first set style.display to parameter if expanded
function setChildren(container, list, display) {
    for (let item of list) {
        kids = container.getElementsByClassName(item.id)
        if (kids.length > 0 && item.classList.contains('expanded')) {
            setChildren(container, kids, display)
        }
        item.style.display = display
    }
}

ready(function () {
    //Prep rows display, carats and toggles
    rows = document.getElementById("treetable").tBodies[0].children
    //Subrows must be display:none; to work correctly
    for (let item of rows) {
        if (!item.classList.contains('treeroot')) {
            item.style.display = 'table-row';
        }
    }
    //Insert toggle carat and indent
    for (let item of rows) {
        if (item.classList.contains('treeroot')) {
            item.mytabledepth = 0
        }
        //Check if item has kids
        var cnt = 0;
        for (let r of rows) {
            if (r.classList.contains(item.id)) {
                cnt++;
                r.mytabledepth = item.mytabledepth + 1;
            }
        }
        if (cnt > 0) {
            item.children[0].insertAdjacentHTML('afterbegin', '<span class="treetoggle"></span>');
        }
        else {
            item.children[0].insertAdjacentHTML('afterbegin', '<span class="treetoggle-invisible"></span>');
        }
    }
    //Set leader indent depth
    for (let item of rows) {
        item.classList.toggle('expanded')
        el = item.children[0]
        el.setAttribute('style', el.style.cssText + '--depth: ' + 1 * item.mytabledepth + 'em');
    }

    //Add onclick listener
    toggles = document.getElementById("treetable").getElementsByClassName('treetoggle')
    for (let item of toggles) {
        item.addEventListener('click', function () {
            var tr = this.closest('tr')
            var children = this.closest('tbody').getElementsByClassName(tr.id)
            tr.classList.toggle('expanded')
            setChildren(tr.parentNode, children, tr.classList.contains('expanded') ? 'table-row' : 'none')
        });
    }

});
