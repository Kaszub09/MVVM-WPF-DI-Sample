using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace WPFSampleApplication.Model {
    public class ApplicationThemes : IApplicationThemes {
        public event EventHandler<string> ThemeChanged;
        public event EventHandler ThemesReloaded;

        private Dictionary<string, ResourceDictionary> _allThemes;
        private Collection<ResourceDictionary> _mergedDictionaries;

        private string _currentThemeName;
        private string _directoryPath;
        private IGlobalSettings _settings;

        public ApplicationThemes(IGlobalSettings globalSettings, string directoryPath = @"ApplicationData\Themes") {
            _settings = globalSettings;
            _directoryPath = directoryPath;
            _mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            _settings.WindowsUI.PropertyChanged += WindowsUI_PropertyChanged;

            ReloadAllThemes(null);

            ChangeTheme(_settings.WindowsUI.ThemeID);
        }

        private void WindowsUI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "ThemeID")
                ChangeTheme(_settings.WindowsUI.ThemeID);
        }

        public bool ChangeTheme(string themeName) {
            if (themeName != _currentThemeName && themeName != null && _allThemes.ContainsKey(themeName)) {
                if (_currentThemeName != null)
                    _mergedDictionaries.Remove(_allThemes[_currentThemeName]);

                _mergedDictionaries.Add(_allThemes[themeName]);
                _currentThemeName = themeName;
                ThemeChanged?.Invoke(this, themeName);
                _settings.WindowsUI.ThemeID = _currentThemeName;
                return true;
            } else if (themeName != _currentThemeName && themeName == null) {
                if (_currentThemeName != null)
                    _mergedDictionaries.Remove(_allThemes[_currentThemeName]);

                _currentThemeName = null;
                ThemeChanged?.Invoke(this, null);
                _settings.WindowsUI.ThemeID = _currentThemeName;
                return true;
            } else {
                _settings.WindowsUI.ThemeID = _currentThemeName;
                return false;
            }
        }

        public List<string> GetAvailableThemes() {
            return _allThemes.Select(x => x.Key).ToList();
        }

        public string GetCurrentTheme() {
            return _currentThemeName;
        }

        #region LoadThemes
        public void ReloadAllThemes(string directoryPath = null) {
            if (directoryPath != null)
                _directoryPath = directoryPath;

            _allThemes = new Dictionary<string, ResourceDictionary>();

            LoadEmbeddedThemes();
            if (Directory.Exists(_directoryPath))
                LoadThemesInFolder();

            EventHandler handler = ThemesReloaded;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void LoadEmbeddedThemes() {
            var themesDirectory = @"ApplicationData/Themes/";
            try {
                _allThemes.Add("Default", new ResourceDictionary() {
                    Source = new Uri(themesDirectory + "Default.xaml", UriKind.RelativeOrAbsolute)
                });
            } catch (Exception e) {//TODO LOGGER?
                _allThemes.Add("Default", new ResourceDictionary());
            }

            try {
                _allThemes.Add("Dark", new ResourceDictionary() {
                    Source = new Uri(themesDirectory + "Dark.xaml", UriKind.RelativeOrAbsolute)
                });
            } catch (Exception e) {//TODO LOGGER?
            }

            try {
                _allThemes.Add("DSIII", new ResourceDictionary() {
                    Source = new Uri(themesDirectory + "DSIII.xaml", UriKind.RelativeOrAbsolute)
                });
            } catch (Exception e) { //TODO LOGGER?
            }
        }

        private void LoadThemesInFolder() {
            var regEx = new Regex(@"\\([^\\]+?).xaml");

            foreach (var file in Directory.GetFiles(_directoryPath, "*.xaml")) {
                try {
                    var res = new ResourceDictionary();
                    res.Source = new Uri("pack://siteoforigin:,,,/" + file.Replace(@"\\", "/"), UriKind.RelativeOrAbsolute);

                    _allThemes[regEx.Match(file).Groups[1].Value] = res;
                } catch (Exception e) {//TODO LOGGER?
                }
            }
        }
        #endregion LoadThemes

    }
}
