
using System;

namespace WPFSampleApplication.Model {
    public interface IGlobalSettings {
        bool AutosaveAnyChanges { get; set; }
        ISettingsProcess Process { get; }
        ISettingsWindowsUI WindowsUI { get; }

        event EventHandler AnySettingsChanged;

        void LoadFromFiles(object oldSettings = null);
        void ResetToDefault(object oldSettings = null);
        void SaveToFiles(object settingsToSave = null);
    }
}