using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSampleApplication.Model {
    public class SettingsProcess : ISettingsProcess {
        public event PropertyChangedEventHandler PropertyChanged;
        [DoNotNotify]
        public bool AutosaveChanges { get; set; } = true;

        public DateTime ChosedDate { get; set; } = DateTime.Today;
        //It's better place for checking than converter, since it can read negative from xml file
        private double _intervalSec = 60;
        public double IntervalSec {
            get { return _intervalSec; }
            set {
                if (value > 0 && value != _intervalSec) {
                    _intervalSec = value;
                }
            }
        }
        public string EmailCredName { get; set; } = "EmailCredentials";
        public string XServiceCredName { get; set; } = "XServiceCredentials";
        public bool RunProcessImmediately { get; set; } = false;
    }
}
