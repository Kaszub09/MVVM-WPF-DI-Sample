using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFSampleApplication.ViewModel {

        public struct LanguageItem {
            public string ID { get; set; }
            public string DisplayName { get; set; }
            public ICommand ChangeLanguage { get; set; }

        }
    }