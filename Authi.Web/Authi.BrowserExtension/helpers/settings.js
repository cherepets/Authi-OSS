export const Settings = {
    save(value) {
        const json = JSON.stringify(value);
        localStorage.setItem('settings', json);
    },

    load() {
        const value = localStorage.getItem('settings');
        return JSON.parse(value);
    }
}
