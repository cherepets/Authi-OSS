let isPreventingClicks = false;

export const Task = {
    async sleepAsync(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    },

    async preventClicks(task) {
        isPreventingClicks = true;
        try {
            await task();
        } finally {
            isPreventingClicks = false;
        }
    }
};

export class CancellationToken {
    constructor() {
        this.cancelled = false;
        this.oncancel = () => { };
    }

    cancel() {
        this.cancelled = true;
        this.oncancel();
    }
}

document.addEventListener('click', e => {
    if (isPreventingClicks) {
        e.stopPropagation();
        e.preventDefault();
    }
}, true);
