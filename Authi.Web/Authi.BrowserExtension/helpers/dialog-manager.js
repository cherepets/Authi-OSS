import { Animator } from '/helpers/animator.js';

export class DialogManager {
    static async showDialog(options) {
        return new Promise((resolve) => {
            const dialog = document.createElement('dialog');
            dialog.setAttribute('closedby', 'none');

            const titleElement = document.createElement('h2');
            titleElement.textContent = options.title;
            dialog.appendChild(titleElement);

            const paragraphs = options.message.split('\n');
            for (const paragraph of paragraphs) {
                const p = document.createElement('p');
                p.textContent = paragraph;
                dialog.appendChild(p);
            }

            const buttonContainer = document.createElement('div');
            buttonContainer.classList.add('flex-justify', 'dialog-button-container');
            const buttonsWrapper = document.createElement('div');
            buttonsWrapper.classList.add('right-aligned');
            buttonsWrapper.style.marginTop = '20px';

            if (options.primaryButtonText) {
                const primaryButton = document.createElement('button');
                primaryButton.type = 'button';
                primaryButton.classList.add('accent-button');
                primaryButton.textContent = options.primaryButtonText;
                primaryButton.style.outline = 0;

                primaryButton.addEventListener('click', () => {
                    if (options.onPrimary) options.onPrimary();
                    dialog.close();
                    resolve();
                });

                buttonsWrapper.appendChild(primaryButton);
            }

            if (options.cancelButtonText) {
                const cancelButton = document.createElement('button');
                cancelButton.type = 'button';
                cancelButton.textContent = options.cancelButtonText;
                cancelButton.autofocus = true;
                cancelButton.style.marginLeft = '8px';
                cancelButton.style.outline = 0;

                cancelButton.addEventListener('click', () => {
                    if (options.onCancel) options.onCancel();
                    dialog.close();
                    resolve();
                });

                buttonsWrapper.appendChild(cancelButton);
            }

            buttonContainer.appendChild(buttonsWrapper);
            dialog.appendChild(buttonContainer);

            document.body.appendChild(dialog);
            dialog.showModal();
            Animator.fadeInAsync(dialog, 90, 90);
        });
    }
}