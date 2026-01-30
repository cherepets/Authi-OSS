export const Animator = {
    async animateAsync(element, ms, from, to, easing, onfinish, ctx) {
        return new Promise(resolve => {
            const animation = element.animate(
                [from, to],
                { duration: ms, easing: easing }
            );

            if (ctx) {
                ctx.oncancel = () => {
                    animation.cancel();
                    resolve();
                }
            }

            animation.onfinish = () => {
                if (!ctx?.cancelled && onfinish) {
                    onfinish();
                }
                resolve();
            }
        });
    },

    async fadeInAsync(element, ms, y, ctx) {
        return this.animateAsync(
            element, ms,
            { opacity: 0, transform: 'translateY(' + y + 'px)' },
            { opacity: 1, transform: 'translateY(0)' },
            'ease-in', null, ctx
        );
    },

    async fadeOutAsync(element, ms, y, ctx) {
        return this.animateAsync(
            element, ms,
            { opacity: 1, transform: 'translateY(0)' },
            { opacity: 0, transform: 'translateY(' + y + 'px)' },
            'ease-out',
            () => { element.style.opacity = 0; },
            ctx
        );
    },

    async spinAsync(element, ms, deg, ctx) {
        return this.animateAsync(
            element, ms,
            { transform: 'rotateZ(0)' },
            { transform: 'rotateZ(' + deg + 'deg)' },
            'ease-in', null, ctx
        );
    }
};
