using System;
using System.Collections.Generic;

namespace WPFSampleApplication.Model {
    public interface IApplicationThemes {
        event EventHandler<string> ThemeChanged;
        event EventHandler ThemesReloaded;

        bool ChangeTheme(string themeName);
        List<string> GetAvailableThemes();
        string GetCurrentTheme();
        void ReloadAllThemes(string directoryPath = null);
    }
}