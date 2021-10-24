using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFSampleApplication.Model {
    public interface IApplicationSubtitles: INotifyPropertyChanged {
        string this[string key] { get; }

        event EventHandler<string> LanguageChanged;
         event EventHandler LanguagesReloaded;

        bool ChangeLanguage(string languageID);
        List<(string langID, string langName)> GetAvailableLanguages();
        (string langID, string langName) GetCurrentLanguage();
        string GetText(string key, params object[] args);
        void ReloadAllLanguages(string directoryPath = null);
    }
}