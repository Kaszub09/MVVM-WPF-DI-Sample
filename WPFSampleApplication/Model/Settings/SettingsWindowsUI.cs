using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSampleApplication.Model {
    public class SettingsWindowsUI : ISettingsWindowsUI {
        public event PropertyChangedEventHandler PropertyChanged;
        [DoNotNotify]
        public bool AutosaveChanges { get; set; } = true;

        //Actual settings
        public string ThemeID { get; set; } = "Default";
        public string LanguageID { get; set; } = "pol";
        public int MenuAndStatusbarFontSize { get; set; } = 12;
        public int MainFontSize { get; set; } = 12;
        public bool ShowNotifications { get; set; } = true;
        public bool UpdateTaskbarIcon { get; set; } = true;
    }
}
