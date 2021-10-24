using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using WPFSampleApplication.Model;

namespace WPFSampleApplication.ViewModel {
    public class WindowTaskbar : IWindowTaskbar {
        private TaskbarItemInfo _taskbar;
        private IGlobalSettings _settings;

        public WindowTaskbar(Window _window, IGlobalSettings globalSettings) {
            _settings = globalSettings;
            _settings.WindowsUI.PropertyChanged += WindowsUI_PropertyChanged;
            if (_window.TaskbarItemInfo == null)
                _window.TaskbarItemInfo = new TaskbarItemInfo();
            _taskbar = _window.TaskbarItemInfo;
        }

        private void WindowsUI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "UpdateTaskbarIcon") {
                if (_settings.WindowsUI.UpdateTaskbarIcon == false) {
                    _taskbar.ProgressState = TaskbarItemProgressState.None;
                    _taskbar.ProgressValue = 0;
                    _taskbar.Description = "";
                }
            }
        }

        public void ChangeState(double? value = null, TaskbarItemProgressState? state = null, string description = null) {
            if (_settings.WindowsUI.UpdateTaskbarIcon) {
                _taskbar.ProgressState = state == null ? _taskbar.ProgressState : state.Value;
                _taskbar.ProgressValue = value == null ? _taskbar.ProgressValue :
                    value.Value > 1 ? value.Value / 100 : value.Value;
                _taskbar.Description = description == null ? _taskbar.Description : description;
            }
        }

    }
}
