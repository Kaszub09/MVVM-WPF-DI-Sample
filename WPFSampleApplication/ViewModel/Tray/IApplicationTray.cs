using Hardcodet.Wpf.TaskbarNotification;
using System.Drawing;
using System.Windows.Controls;

namespace WPFSampleApplication.ViewModel {
    public interface IApplicationTray {
        bool AlwaysVisible { get; set; }

        void ChangeContextMenu(ContextMenu contextMenu);
        void ChangeIcon(Icon icon);
        void HideTrayIcon();
        void SetTooltipText(string text);
        void ShowNotification(string title, string message, BalloonIcon icon = BalloonIcon.None);
        void ShowTrayIcon();
    }
}