const utils = {
    switchTheme(currentTheme, newTheme, linkElementId) {
        if (currentTheme === newTheme) return currentTheme;

        const linkElement = document.getElementById(linkElementId);
        if (!linkElement) throw Error(`no theme link with id ${linkElementId} in DOM`);

        const cloneLinkElement = linkElement.cloneNode(true);
        const oldThemeUrl = linkElement.getAttribute('href');
        if (!oldThemeUrl) throw Error(`no href attribute in link with id ${linkElementId}`);

        const newThemeUrl = oldThemeUrl.replace(currentTheme, newTheme);
        cloneLinkElement.setAttribute('id', `${linkElementId}-clone`);
        cloneLinkElement.setAttribute('href', newThemeUrl);
        cloneLinkElement.addEventListener('load', () => {
            linkElement.remove();
            cloneLinkElement.setAttribute('id', linkElementId);
        });

        linkElement.parentNode?.insertBefore(cloneLinkElement, linkElement.nextSibling);

        return newTheme;
    }
}

window.CT = {
    utils
};