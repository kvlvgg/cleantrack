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
    },

    toUnsignedIntAndNotZero(inputId) {
        const input = document.getElementById(inputId);
        if (!input) return;

        input.value = input.value.replace(/[\D]/, '')
        if (input.value === '0') input.value = '';

        window.setTimeout(() => {
            if (input.value === '0') input.value = '';
        }, 10)
    },

    setInputValue(inputId, value) {
        const input = document.getElementById(inputId);
        if (!input) return;

        input.value = value;
    },

    setStyleProperty(elId, name, value) {
        console.log(name, value)
        const el = document.getElementById(elId);
        el.style.setProperty(name, value);

        alert(value)
    }
}

window.CT = {
    utils
};