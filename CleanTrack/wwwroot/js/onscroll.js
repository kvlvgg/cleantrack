const isScrollObserverActivated = false;

function ctOnScroll() {
    if (isScrollObserverActivated) return;

    document.addEventListener('scroll', e => {
        const headerEl = document.getElementById('header');
        const summaryPanel = document.getElementById('summary-panel');

        let lastPageYOffset = 0;
        const currentPageYOffset = window.pageYOffset;

        if (currentPageYOffset - lastPageYOffset > 0) {
            if (headerEl) headerEl.style.setProperty('top', 'calc(var(--header-height) * -1)');
            if (summaryPanel) summaryPanel.style.setProperty('top', '0');
            lastPageYOffset = currentPageYOffset
        }

        else {
            if (headerEl) headerEl.style.setProperty('top', '0');
            if (summaryPanel) summaryPanel.style.setProperty('top', 'var(--header-height)');
            lastPageYOffset = currentPageYOffset
        }
    });
}

