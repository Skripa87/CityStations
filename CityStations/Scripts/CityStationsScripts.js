function findstart() {
    const button = document.getElementById("searchBoxButton");
    if (button) {
        button.click();
    }
}

function selectSearchResult(stationId, str) {
    const textBox = document.getElementById("searchBoxId");
    if (textBox) {
        textBox.value = str;
    }
    const inputId = document.getElementById("selectStationId");
    if (inputId) {
        inputId.value = stationId;
    }
    const button = document.getElementById("selectStationButton");
    if (button) {
        button.click();
    }
}

function setCaretDropdown(id) {
    const wrapper = document.getElementById(id + "WrapperId");
    if (wrapper) {
        wrapper.classList.add("open");
    }
}

function unsetCaretDropdown(id) {
    const wrapper = document.getElementById(id + "WrapperId");
    if (wrapper) {
        wrapper.classList.remove("open");
    }
}

function formActivateInformationTableSubmit() {
    const button = document.getElementById("activateInformationTableButtonId");
    if (button) {
        button.click();
    }
}

function reloadAllContentTriggerGo() {
    const button = document.getElementById("specButton");
    if (button) {
        button.click();
    }
}

function changeVisibleContent(timeOut) {
    const timeoutms = +timeOut * 1000;
    const t = setTimeout(changeVisibleContentFunction, timeoutms);
}

function tryReloadInformationTableAfterError() {
    const timeoutms = 60000;
    const t = setTimeout(tryReloadInformationTableAfterErrorFunction, timeoutms);
}

function tryReloadInformationTableAfterErrorFunction() {
    const button = document.getElementById("tryReload");
    if (button) {
        button.click();
    }
}

function changeVisibleContentFunction() {
    const button = document.getElementById("updateBlockContentInformationTabloId");
    if (button) {
        button.click();
    }
}

function removeFadeIn() {
    const bodys = document.getElementsByTagName("body");
    let body = undefined;
    if (bodys) {
        body = bodys[0];
    }
    if (body) {
        body.classList = null;
    }
    const nodes = document.getElementsByClassName("modal-backdrop fade in");
    if (nodes) {
        count = nodes.length;
        for (let i = 0; i < count; i++) {
            nodes[i].remove();
        }
    }
}

function saveOptionsAndContentInformationTable() {
    const button = document.getElementById("buttonSaveOptionsAndContent");
    if (button) {
        button.click();
    }
    const buttonsContentSaved = document.getElementsByClassName("btn btn-link buttonSaved buttonContentSaved");
    let buttonsContentSavedCount = undefined;
    if (buttonsContentSaved) {
        buttonsContentSavedCount = buttonsContentSaved.length;
    }
    for (let i = 0; i < buttonsContentSavedCount; i++) {
        if (buttonsContentSaved[i]) {
            buttonsContentSaved[i].click();
        }
    }
}

function buferedIdRemovedContent(contentId) {
    const input = document.getElementById('removedContentIdBufferId');
    if (input) {
        input.value = contentId;
    }
}

function setPaddingLogOnWindow() {
    let winInHeight = window.innerHeight;
    let mainContent = document.getElementById('loginForm');
    if (mainContent) {
        let padding = winInHeight / 4 + "px";
        mainContent.style.paddingTop = padding;
    }
    window.onload = window.onresize = function () {
        let winInHeight = window.innerHeight;
        let mainContent = document.getElementById('loginForm');
        if (mainContent) {
            let padding = winInHeight / 4 + "px";
            mainContent.style.paddingTop = padding;
        }
    }
}

function setMaxHeightOnWrapperAfterLoadReloadWindow() {
    let winInHeight = window.innerHeight;
    let mainContentList = document.getElementsByClassName('page-body-wrapper');
    let mainContent;
    if (mainContentList) {
        mainContent = mainContentList[0];
    }
    if (mainContent) {
        let height = winInHeight - 337 + "px";
        mainContent.style.maxHeight = height;
    }
    window.onload = window.onresize = function() {
        let winInHeight = window.innerHeight;
        let mainContentList = document.getElementsByClassName('page-body-wrapper');
        let mainContent;
        if (mainContentList) {
            mainContent = mainContentList[0];
        }
        if (mainContent) {
            let height = winInHeight - 337 + "px";
            mainContent.style.maxHeight = height;
        }
    }
}

function setClickCheckBoxOnlyActivateStations(ch) {
    let button = document.getElementById("buttonSetSubmitOnSelectOnlyActivateStations");
    if (button) {
        button.click();
        }
}
