export const Cache = {
    save(value) {
        const json = JSON.stringify(value);
        localStorage.setItem('cache', json);
    },

    load() {
        const value = localStorage.getItem('cache');
        return JSON.parse(value);
    }
}
