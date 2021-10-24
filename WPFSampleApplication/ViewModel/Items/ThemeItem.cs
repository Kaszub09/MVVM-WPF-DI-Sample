using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace WPFSampleApplication.ViewModel {
    public struct ThemeItem {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public ICommand ChangeTheme { get; set; }

        public void UpdateDisplayName(string newName) {
            DisplayName = newName;
        }
    }
}
