using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WPFSampleApplication.Model {
    public class ApplicationSubtitles : IApplicationSubtitles {
        public event EventHandler<string> LanguageChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler LanguagesReloaded;

        private Dictionary<string, string> _currentLanguage;
        private Dictionary<string, Dictionary<string, string>> _allLanguages;
        private IGlobalSettings _settings;
        private string _directoryPath;

        public ApplicationSubtitles(IGlobalSettings globalSettings ,string directoryPath = @"ApplicationData\Languages") {
            _settings = globalSettings;
            _directoryPath = directoryPath;
            _settings.WindowsUI.PropertyChanged += WindowsUI_PropertyChanged;
            _currentLanguage = new Dictionary<string, string>() { { "language_id", null }, { "language_display_name", null } };

            ReloadAllLanguages(null);
        }

        private void WindowsUI_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName== "LanguageID")
                ChangeLanguage(_settings.WindowsUI.LanguageID);
        }

        public bool ChangeLanguage(string languageID) {
            if (languageID != null && _allLanguages.ContainsKey(languageID) && _currentLanguage["language_id"] != languageID) {
                _currentLanguage = _allLanguages[languageID];
                EventHandler<string> handler = LanguageChanged;
                handler?.Invoke(this, languageID);
                OnPropertyChanged("Item[]");
                _settings.WindowsUI.LanguageID = _currentLanguage["language_id"];
                return true;
            } else if (languageID != _currentLanguage["language_id"] && languageID == null) {
                _currentLanguage = new Dictionary<string, string>() { { "language_id", null }, { "language_display_name", null } };
                EventHandler<string> handler = LanguageChanged;
                handler?.Invoke(this, languageID);
                OnPropertyChanged("Item[]");
                _settings.WindowsUI.LanguageID = _currentLanguage["language_id"];
                return true;
            } else {
                _settings.WindowsUI.LanguageID = _currentLanguage["language_id"];
                return false;
            }
        }

        #region GetText
        public string this[string key] {
            get {
                return _currentLanguage.ContainsKey(key) ? _currentLanguage[key] : "[" + key + "]";
            }
        }

        public string GetText(string key, params object[] args) {
            var text = this[key];
            try {
                return string.Format(text, args);
            } catch (Exception e) {
                return text;
            }
        }

        #endregion

        public List<(string langID, string langName)> GetAvailableLanguages() {
            return _allLanguages.Select(x => (x.Key, x.Value["language_display_name"])).ToList();
        }

        public (string langID, string langName) GetCurrentLanguage() {
            return (_currentLanguage["language_id"], _currentLanguage["language_display_name"]);
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region LoadLanguages
        public void ReloadAllLanguages(string directoryPath = null) {
            if (directoryPath != null)
                _directoryPath = directoryPath;

            _allLanguages = new Dictionary<string, Dictionary<string, string>>();
            LoadEmbeddedLanguages();

            if (Directory.Exists(_directoryPath)) {
                LoadXMLLanguages();
                LoadTXTLanguages();
            }

            ChangeLanguage(_settings.WindowsUI.LanguageID);

            EventHandler handler = LanguagesReloaded;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void LoadEmbeddedLanguages() {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.EmbeddedLanguages);

            var langNodes = doc.GetElementsByTagName("Language");

            foreach (XmlElement langNode in langNodes) {
                var lang = new Dictionary<string, string>();

                foreach (var node in langNode.ChildNodes) {
                    if (node is XmlElement) {
                        lang[((XmlElement)node).GetAttribute("id")] = ((XmlElement)node).InnerText.Trim();
                    }
                }

                lang["language_id"] = langNode.GetAttribute("id");
                lang["language_display_name"] = langNode.GetAttribute("name");

                _allLanguages[lang["language_id"]] = lang;
            }
        }

        private void LoadXMLLanguages() {
            foreach (var file in Directory.GetFiles(_directoryPath, "*.xml")) {
                try {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);

                    var langNodes = doc.GetElementsByTagName("Language");

                    foreach (XmlElement langNode in langNodes) {
                        var lang = new Dictionary<string, string>();

                        foreach (var node in langNode.ChildNodes) {
                            if (node is XmlElement) {
                                lang[((XmlElement)node).GetAttribute("id")] = ((XmlElement)node).InnerText;
                            }
                        }
                        lang["language_id"] = langNode.GetAttribute("id");
                        lang["language_display_name"] = langNode.GetAttribute("name");

                        _allLanguages[lang["language_id"]] = lang;
                    }
                } catch (Exception e) {//TODO LOGGER?
                }
            }
        }

        private void LoadTXTLanguages() {
            foreach (var file in Directory.GetFiles(_directoryPath, "*.txt")) {
                try {
                    var lang = new Dictionary<string, string>();

                    var lines = File.ReadAllLines(file);

                    foreach (var line in lines) {
                        if (line.Trim().Length > 0 && !line.StartsWith("#") && line.Contains("==")) {
                            var sLine = line.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                            lang[sLine[0].Trim()] = System.Text.RegularExpressions.Regex.Unescape(sLine[1]);
                        }
                    }

                    if (lang.ContainsKey("language_id") == false)
                        lang["language_id"] = "Unspecified";
                    if (lang.ContainsKey("language_display_name") == false)
                        lang["language_display_name"] = "Unspecified";

                    _allLanguages[lang["language_id"]] = lang;
                } catch (Exception e) {//TODO LOGGER?
                }
            }
        }
        #endregion LoadLanguages

        ~ApplicationSubtitles() {
            _settings.WindowsUI.PropertyChanged -= WindowsUI_PropertyChanged;
        }
    }
}