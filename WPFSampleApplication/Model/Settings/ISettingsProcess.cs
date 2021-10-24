using System;
using System.ComponentModel;

namespace WPFSampleApplication.Model {
    public interface ISettingsProcess : INotifyPropertyChanged {
        bool AutosaveChanges { get; set; }
        DateTime ChosedDate { get; set; }
        string EmailCredName { get; set; }
        double IntervalSec { get; set; }
        string TMSServiceCredName { get; set; }
        bool RunProcessImmediately { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}