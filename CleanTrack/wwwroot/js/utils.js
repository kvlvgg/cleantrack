const utils = {
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
    }
}

window.CT = {
    utils
};