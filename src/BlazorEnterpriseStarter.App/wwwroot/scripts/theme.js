window.blazorEnterpriseStarterTheme = {
    getCurrentTheme: () => {
        const explicitTheme = document.documentElement.getAttribute('data-theme');

        if (explicitTheme === 'light' || explicitTheme === 'dark') {
            return explicitTheme;
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    },

    setTheme: (theme) => {
        if (theme !== 'light' && theme !== 'dark') {
            return window.blazorEnterpriseStarterTheme.getCurrentTheme();
        }

        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('bes-theme', theme);
        return theme;
    }
};
