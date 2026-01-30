let localization = {};

export class Localization {
    static async initAsync() {
        const response = await fetch('localization/en.json');
        localization = await response.json();
    }

    static get(key) {
        return localization[key];
    }
}
