export const Clipboard = {
    async writeAsync(value) {
        try {
            await navigator.clipboard.writeText(value);
        } catch {
            const textarea = document.createElement('textarea');
            textarea.value = value;
            document.body.appendChild(textarea);
            textarea.select();
            document.execCommand('copy');
            document.body.removeChild(textarea);
        }
    }
}
