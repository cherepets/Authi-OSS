export const Navigator = {
    async navigateAsync(componentClass) {
        const component = await componentClass.createAsync();
        window.scrollTo(0, 0);
        document.body.innerHTML = '';
        document.body.appendChild(component);
    }
}
