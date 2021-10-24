using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPFSampleApplication.Model;
using BackgroundProcessSample;
using System.Windows.Shell;

namespace WPFSampleApplication.ViewModel {

    public class ApplicationViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public IApplicationSubtitles AppSubtitles { get; private set; }
        public IApplicationThemes AppThemes { get; private set; }
        public IGlobalSettings Settings { get; private set; }
        public IBackgroundProcessModel BackgroundProcessModel { get; private set; }

        public BindingList<LanguageItem> AllLanguagesItems { get; private set; } = new BindingList<LanguageItem>();
        public BindingList<ThemeItem> AllThemesItems { get; private set; } = new BindingList<ThemeItem>();

        #region Commands
        public ICommand StartProcess { get; private set; }
        public ICommand StopProcess { get; private set; }
        public ICommand ChangeLanguage { get; private set; }
        public ICommand ChangeTheme { get; private set; }
        public ICommand RefreshLanguages { get; private set; }
        public ICommand RefreshThemes { get; private set; }
        public ICommand MinimizeAppToTray { get; private set; }
        public ICommand RestoreAppFromTray { get; private set; }
        public ICommand ShowNotification { get; private set; }
        public ICommand ExitApplication { get; private set; }
        #endregion

        private Progress<ProgressInfo> _appProgressInfo;

        public ApplicationViewModel(IApplicationSubtitles applicationSubtitles, IApplicationThemes applicationThemes, IGlobalSettings globalSettings, IBackgroundProcessModel backgroundProcessModel) {
            AppSubtitles = applicationSubtitles;
            AppThemes = applicationThemes;
            Settings = globalSettings;
            BackgroundProcessModel = backgroundProcessModel;

            #region Commands definition
            StartProcess = new Command((obj) =>  BackgroundProcessModel.Start(), 
                (obj) => { return BackgroundProcessModel.CurrentState == CurrentState.Stopped; });

            StopProcess = new Command((obj) => BackgroundProcessModel.Stop(),
                (obj) => { return BackgroundProcessModel.CurrentState == CurrentState.Started; });

            ChangeLanguage = new Command((obj) => {
                if (obj is string)
                    AppSubtitles.ChangeLanguage((string)obj);
            });

            ChangeTheme = new Command((obj) => {
                if (obj is string)
                    AppThemes.ChangeTheme((string)obj);
            });

            RefreshLanguages = new Command((obj) => {
                AppSubtitles.ReloadAllLanguages();
            });

            RefreshThemes = new Command((obj) => {
                AppThemes.ReloadAllThemes();
            });

            MinimizeAppToTray = new Command((Object obj) => {
                App.Current.SwitchView(ApplicationView.MinimizedToTray);
            });

            RestoreAppFromTray = new Command((Object obj) => {
                App.Current.SwitchView(ApplicationView.ShowingMainWindow);
            });

            ShowNotification = new Command((Object obj) => {
                if (obj is Notification) {
                    var notification = (Notification)obj;
                    App.Current.Tray.ShowNotification(notification.Title, notification.Message, notification.Icon);
                }
            });

            ExitApplication = new Command((obj) => {
                App.Current.Shutdown();
            });
            #endregion

            AppSubtitles.LanguageChanged += ApplicationSubtitles_LanguageChanged;
            AppSubtitles.LanguagesReloaded += ApplicationSubtitles_LanguagesReloaded;
            AppThemes.ThemesReloaded += ApplicationThemes_ThemesReloaded;
            AppThemes.ThemeChanged += ApplicationThemes_ThemeChanged;

            BackgroundProcessModel.CurrentStateChanged += BackgroundProcessModel_CurrentStateChanged;
            BackgroundProcessModel_CurrentStateChanged(BackgroundProcessModel, BackgroundProcessModel.CurrentState);

            ReloadLanguages();
            ReloadThemes();

            _appProgressInfo = new Progress<ProgressInfo>((p) => {
                double progressValue = p.AllItemsNumber == 0 ? 100 : p.ProcessedItems * 100 / p.AllItemsNumber;
                string textToDisplay = "";
                TaskbarItemProgressState taskbarState = TaskbarItemProgressState.Normal;
                Hardcodet.Wpf.TaskbarNotification.BalloonIcon trayIcon = Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info;
                switch (p.Status) {
                    case Status.InProgress:
                        textToDisplay = AppSubtitles.GetText("process_status_in_progress", p.ProcessedItems, p.AllItemsNumber);
                        break;
                    case Status.Completed:
                        taskbarState = TaskbarItemProgressState.Indeterminate;
                        textToDisplay = AppSubtitles.GetText("process_status_completed", p.ProcessedItems, p.AllItemsNumber, DateTime.Now);
                        break;
                    case Status.Cancelled:
                        taskbarState = TaskbarItemProgressState.Paused;
                        textToDisplay = AppSubtitles.GetText("process_status_cancelled", p.ProcessedItems, p.AllItemsNumber, DateTime.Now);
                        break;
                    case Status.Error:
                        taskbarState = TaskbarItemProgressState.Error;
                        trayIcon = Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning;
                        textToDisplay = AppSubtitles.GetText("process_status_error", p.ProcessedItems, p.AllItemsNumber, DateTime.Now, p.ErrorMessage);
                        break;
                    default:
                        break;
                }
                App.Current.WindowTaskbar.ChangeState(progressValue, taskbarState, textToDisplay);
                if(p.Status!=Status.InProgress)
                    App.Current.Tray.ShowNotification(AppSubtitles["tray_progress_info_header"], textToDisplay, trayIcon);
            });

            BackgroundProcessModel.RegisterIProgressToUpdate(_appProgressInfo);
        }

        private void BackgroundProcessModel_CurrentStateChanged(object sender, CurrentState e) {
            switch (e) {
                case CurrentState.Started:
                    App.Current.WindowTaskbar.ChangeState(state: TaskbarItemProgressState.Indeterminate);
                    break;
                case CurrentState.Stopped:
                    App.Current.WindowTaskbar.ChangeState(state: TaskbarItemProgressState.Paused);
                    break;
            }
        }

        private void ApplicationThemes_ThemeChanged(object sender, string e) {
            Settings.WindowsUI.ThemeID = e;
        }

        private void ApplicationSubtitles_LanguagesReloaded(object sender, EventArgs e) {
            ReloadLanguages();
        }

        private void ApplicationThemes_ThemesReloaded(object sender, EventArgs e) {
            ReloadThemes();
        }

        private void ApplicationSubtitles_LanguageChanged(object sender, string e) {
            ReloadThemes();
            Settings.WindowsUI.LanguageID = e;
        }

        private void ReloadLanguages() {
            var currentLanguage = Settings.WindowsUI.LanguageID;
            var languages = AppSubtitles.GetAvailableLanguages().Select(
                l => {
                    return new LanguageItem {
                        ID = l.langID, DisplayName = l.langName,
                        ChangeLanguage = new Command(obj => { AppSubtitles.ChangeLanguage(l.langID); })
                    };
                })
                .ToList();
            AllLanguagesItems.Clear();
            foreach (var item in languages) {
                AllLanguagesItems.Add(item);
            }
            Settings.WindowsUI.LanguageID = currentLanguage;
        }

        private void ReloadThemes() {
            var curentTheme = Settings.WindowsUI.ThemeID;
            var themes = AppThemes.GetAvailableThemes().Select(
                themeID => {
                    return new ThemeItem {
                        ID = themeID, DisplayName = AppSubtitles["theme_" + themeID],
                        ChangeTheme = new Command(obj => { AppThemes.ChangeTheme(themeID); })
                    };
                })
                .ToList();
            AllThemesItems.Clear();
            foreach (var item in themes) {
                AllThemesItems.Add(item);
            }
            Settings.WindowsUI.ThemeID = curentTheme;
        }

        ~ApplicationViewModel() {
            AppSubtitles.LanguageChanged-= ApplicationSubtitles_LanguageChanged;
            AppSubtitles.LanguagesReloaded -= ApplicationSubtitles_LanguagesReloaded;
            AppThemes.ThemesReloaded -= ApplicationThemes_ThemesReloaded;
            AppThemes.ThemeChanged -= ApplicationThemes_ThemeChanged;
            BackgroundProcessModel.CurrentStateChanged-= BackgroundProcessModel_CurrentStateChanged;

            BackgroundProcessModel.DeregisterIProgressToUpdate(_appProgressInfo);
        }
    }
}