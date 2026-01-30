const scripts = {};
const templates = {};

export const Loader = {
    async loadScriptAsync(fileName, getter) {
        if (!getter) {
            getter = () => ({});
        }
        if (!scripts[fileName]) {
            scripts[fileName] = new Promise((resolve, reject) => {
                const script = document.createElement('script');
                script.type = 'text/javascript';
                script.src = fileName;
                script.onload = () => resolve(scripts[fileName] = getter());
                script.onerror = reject;
                document.head.appendChild(script);
            })
        }

        return scripts[fileName];
    },
    
    async loadTemplateAsync(fileName) {
        if (!templates[fileName]) {
            templates[fileName] = new Promise(async resolve => {
                const response = await fetch(fileName);
                const data = await response.text();
                const template = document.createElement('template');
                template.innerHTML = data;
                resolve(template)
            })
        }

        return templates[fileName];
    }
}
