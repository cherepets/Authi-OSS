import { Theme } from '/helpers/theme.js';

chrome.runtime.onStartup.addListener(() => {
    Theme.applySystemTheme();
});

chrome.runtime.onInstalled.addListener(() => {
    Theme.applySystemTheme();
});

Theme.applySystemTheme();
