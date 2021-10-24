using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFSampleApplication.Model;

namespace WPFSampleApplication.ViewModel {
    public class ApplicationTray : IApplicationTray {

        private bool _alwaysVisible;
        public bool AlwaysVisible {
            get {
                return _alwaysVisible;
            }
            set {
                if (value == true)
                    _trayIcon.Visibility = Visibility.Visible;
                _alwaysVisible = value;
            }
        }
        private TaskbarIcon _trayIcon;
        private IApplicationSubtitles _subtitles;
        private IGlobalSettings _settings;

        public ApplicationTray(IApplicationSubtitles subtitles,IGlobalSettings globalSettings) {
            _subtitles = subtitles;
            _settings = globalSettings;

            _trayIcon = new TaskbarIcon();
            _trayIcon.LeftClickCommand = new Command(obj => App.Current.SwitchView(ApplicationView.ShowingMainWindow));
            _trayIcon.NoLeftClickDelay = true;
            _trayIcon.Visibility = Visibility.Collapsed;
            _trayIcon.ContextMenu = new ContextMenu();

            AlwaysVisible = false;
        }

        #region Some magic to update themes in Context Menu
        //Just so it would resize the menu to fit items. Must be in this event, doesn't work in MinimizeToTray. Need to be called only once per theme change
        private void ContextMenu_Opened(object sender, RoutedEventArgs e) {
            _trayIcon.ContextMenu.Items.Add(new MenuItem() { Visibility = Visibility.Hidden });
            _trayIcon.ContextMenu.UpdateLayout();
            _trayIcon.ContextMenu.Items.RemoveAt(_trayIcon.ContextMenu.Items.Count - 1);
            _trayIcon.ContextMenu.Opened -= ContextMenu_Opened;
        }
        #endregion

        #region Commands
        public void ShowTrayIcon() {
            _trayIcon.ContextMenu.Resources.MergedDictionaries.Clear();
            _trayIcon.ContextMenu.Opened += ContextMenu_Opened;

            _trayIcon.Visibility = Visibility.Visible;

            ShowNotification(_subtitles["app_name"], _subtitles["app_minimised_ballontip"], BalloonIcon.Info);
        }

        public void HideTrayIcon() {
            if(!AlwaysVisible)
                _trayIcon.Visibility = Visibility.Hidden;
        }

        public void ShowNotification(string title, string message, BalloonIcon icon = BalloonIcon.None) {
            if(_settings.WindowsUI.ShowNotifications)
                _trayIcon.ShowBalloonTip(title, message, icon);
        }

        public void SetTooltipText(string text) {
            _trayIcon.ToolTipText = text;
        }
        #endregion


        #region Settings
        public void ChangeContextMenu(ContextMenu contextMenu) {
            if (contextMenu != null)
                _trayIcon.ContextMenu = contextMenu;
        }
        public void ChangeIcon(System.Drawing.Icon icon) {
            if (icon != null)
                _trayIcon.Icon = icon;
        }
        #endregion 
    }
}

