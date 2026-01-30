import { MainPage } from '/components/main-page.js';

import { Navigator } from '/helpers/navigator.js';
import { Localization } from '/helpers/localization.js';
import { Theme } from '/helpers/theme.js';

document.addEventListener('DOMContentLoaded', async () => {
    Theme.applySystemTheme(document.getElementsByTagName('head')[0]);
    await Localization.initAsync();
    await Navigator.navigateAsync(MainPage);
});
