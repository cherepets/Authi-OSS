import { dotnet } from '/wasm/dotnet.js'

export class Wasm {
    static #assemblyExportsPromise = null;

    static async initAsync() {
        if (this.#assemblyExportsPromise) {
            return this.#assemblyExportsPromise;
        }
        this.#assemblyExportsPromise = (async () => {
            const { getAssemblyExports, getConfig } = await dotnet
                .withDiagnosticTracing(false)
                .create();

            const config = getConfig();
            return await getAssemblyExports(config.mainAssemblyName);
        })();
        return this.#assemblyExportsPromise;
    }
}
