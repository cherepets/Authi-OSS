import { Loader } from '/helpers/loader.js';

const updateMs = 30000;

export const TotpGenerator = {
    async initAsync() {
        await Loader.loadScriptAsync('lib/otplib.buffer.js');
        return await Loader.loadScriptAsync('lib/otplib.index.js', () => window.otplib.authenticator);
    },

    getRemainingMs() {
        return updateMs - (Date.now() % updateMs);
    },

    getUpdateMs() {
        return updateMs;
    },

    async calculateTotpAsync(secret) {
        const authenticator = await this.initAsync();
        try {
            return authenticator.generate(secret);
        } catch (error) {
            console.warn('Can\'t calculate totp: ' + error);
            return null;
        }
    }
}
