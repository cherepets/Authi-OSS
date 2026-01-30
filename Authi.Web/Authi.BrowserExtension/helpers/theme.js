export const Theme = {
    applySystemTheme(element) {
        this.applyTheme(element, this.getSystemTheme());
    },

    applyTheme(element, theme) {
        if (element) {
            var link = document.createElement('link');
            link.href = 'styles/style.' + theme + '.css';
            link.rel = 'stylesheet';
            element.appendChild(link);
        }

        let extension = chrome ?? browser;
        if (extension) {
            let action = extension.action;
            if (action) {
                action.setIcon({ path: "icons/icon_" + theme + ".png" });
            }
        }
    },

    getSystemTheme() {
        return (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches)
            ? 'dark'
            : 'light';
    }
}
