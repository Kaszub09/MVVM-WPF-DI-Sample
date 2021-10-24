using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace WPFSampleApplication.Model {
    public class GlobalSettings : IGlobalSettings {
        //All settings groups
        public ISettingsWindowsUI WindowsUI { get; private set; }
        public ISettingsProcess Process { get; private set; }

        //Other
        public event EventHandler AnySettingsChanged;
        public bool AutosaveAnyChanges { get; set; } = true;
        private string _directoryPath;
        private IEnumerable<PropertyInfo> _settingsProperties;
        public GlobalSettings(ISettingsWindowsUI settingsWindowsUI, ISettingsProcess settingsProcess, string directoryPath = @"ApplicationData\Settings") {
            WindowsUI = settingsWindowsUI;
            Process = settingsProcess;
            _directoryPath = directoryPath;

            //Prepare directory for settings
            try {
                Directory.CreateDirectory(_directoryPath);
            } catch (Exception e) {//TODO LOGGER
            }

            //Hook to the events
            _settingsProperties = typeof(GlobalSettings).GetProperties().Where(prop => prop.PropertyType.Name.Contains("ISettings"));
            foreach (var setProp in _settingsProperties) {
                var set = setProp.GetValue(this) as INotifyPropertyChanged;
                if (set != null)
                    set.PropertyChanged += SingleSettingChanged;
            }

            //Load settings from files
            LoadFromFiles();
        }

        /// <summary>
        /// Allows to explicitly save settings to files if autosave is set to false. Passing null will save all settings, passing settings property will save only those settings.
        /// </summary>
        /// <param name="settingsToSave">GSettings settings property, e.g. GSettings.WindowsUI</param>
        public void SaveToFiles(object settingsToSave = null) {
            SaveLoadSettings(settingsToSave, true);
        }
        /// <summary>
        /// Allows to change settings to default values. Passing null will read all settings, passing settings property will read only those settings.
        /// </summary>
        /// <param name="oldSettings">GSettings settings property, e.g. GSettings.WindowsUI</param>
        public void ResetToDefault(object oldSettings = null) {
            SaveLoadSettings(oldSettings, false, false);
        }
        /// <summary>
        /// Allows to read settings from files. Passing null will read all settings, passing settings property will read only those settings.
        /// </summary>
        /// <param name="oldSettings">GSettings settings property, e.g. GSettings.WindowsUI</param>
        public void LoadFromFiles(object oldSettings = null) {
            SaveLoadSettings(oldSettings, false, true);
        }

        private void SaveLoadSettings(object settings, bool save, bool fromFiles = true) {
            if (settings == null) {
                foreach (var setProp in _settingsProperties) {
                    if (save) {
                        SaveSettingsToFile(setProp.GetValue(this));
                    } else {
                        LoadSetting(setProp.GetValue(this), fromFiles);
                    }
                }
            } else {
                if (save) {
                    SaveSettingsToFile(settings);
                } else {
                    LoadSetting(settings, fromFiles);
                }
            }
        }

        private void LoadSetting(object oldSettings, bool fromFile = true) {
            var newSettings = fromFile ? ReadSettingsFromFile(oldSettings) : Activator.CreateInstance(oldSettings.GetType());
            var properties = oldSettings.GetType().GetProperties().Where(prop => prop.Name != "AutosaveChanges");

            //Copy each settings property value
            foreach (var prop in properties)
                prop.SetValue(oldSettings, prop.GetValue(newSettings));
        }


        #region XMLserializer
        private object ReadSettingsFromFile(object settingsClassObject) {
            object retVal = settingsClassObject;
            try {
                var filePath = _directoryPath + @"\" + settingsClassObject.GetType().Name + ".xml";

                XmlSerializer serializer = new XmlSerializer(settingsClassObject.GetType());
                using FileStream file = File.OpenRead(filePath);
                retVal = serializer.Deserialize(file);

                file.Close();
            } catch (Exception e) {//TODO LOGGER
            }
            return retVal;
        }

        private void SaveSettingsToFile(object settingsClassObject) {
            object retVal = settingsClassObject;
            try {
                var filePath = _directoryPath + @"\" + settingsClassObject.GetType().Name + ".xml";
                if (File.Exists(filePath)) 
                    File.Delete(filePath);
                using FileStream file = File.Create(filePath);

                XmlSerializer serializer = new XmlSerializer(settingsClassObject.GetType());
                serializer.Serialize(file, settingsClassObject);

                file.Close();
            } catch (Exception e) {//TODO LOGGER
            }
        }
        #endregion XMLserializer

        private void SingleSettingChanged(object sender, PropertyChangedEventArgs e) {
            try {
                dynamic settingsSet = sender;
                //If true, serialize class
                if (AutosaveAnyChanges && settingsSet.AutosaveChanges) {
                    SaveSettingsToFile(sender);
                }
            } catch (Exception ex) {//TODO LOGGER
            }
            EventHandler handler = AnySettingsChanged;
            handler?.Invoke(sender, e);
        }


    }

}
