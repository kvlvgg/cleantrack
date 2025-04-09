const maxWaiting = 5000;
const wait = (seconds) => new Promise((resolve) => window.setTimeout(() => resolve(seconds), seconds));

async function start() {
    let currentWaiting = 0;
    const waitingStep = 200;

    const appEl = document.querySelector("#app");
    const loadingEl = document.querySelector(".loading");

    while (appEl.childNodes.length == 0) {
        currentWaiting += await wait(waitingStep);
        if (currentWaiting > maxWaiting) break;
    }

    loadingEl.style.setProperty("display", "none");
    const parent = loadingEl.parentElement;
    if (parent) parent.removeChild(loadingEl);
}

start();
