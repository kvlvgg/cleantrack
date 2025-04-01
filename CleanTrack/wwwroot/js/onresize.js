const isResizeObserverActivated = false

function ctOnResize() {
    if (isResizeObserverActivated) return;

    window.visualViewport.addEventListener('resize', () => {
        const height = window.visualViewport?.height;
        DotNet.invokeMethodAsync('CleanTrack', 'OnViewportResize', height);
    });
}

