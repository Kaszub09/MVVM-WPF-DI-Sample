using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WPFSampleApplication.Model;
using WPFSampleApplication.ViewModel;
using WPFSampleApplication.View;
using System.Windows.Controls;
using BackgroundProcessSample;

namespace WPFSampleApplication {
    public enum ApplicationView { ShowingMainWindow, MinimizedToTray }


    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }
        public IApplicationTray Tray { get; private set; }
        public IWindowTaskbar WindowTaskbar { get; private set; }

        public App() {
            Services = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.MainWindow = Services.GetRequiredService<MainWindow>();
            this.WindowTaskbar = new WindowTaskbar(this.MainWindow, Services.GetRequiredService<IGlobalSettings>());

            this.MainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            this.MainWindow.Show();
         
            SetTray();
        }

        public void SwitchView(ApplicationView view) {
            switch (view) {
                case ApplicationView.ShowingMainWindow:
                    Tray.HideTrayIcon();
                    this.MainWindow.Visibility = Visibility.Visible;
                    break;
                case ApplicationView.MinimizedToTray:
                    this.MainWindow.Visibility = Visibility.Collapsed;
                    Tray.ShowTrayIcon();
                    break;
                default:
                    throw new Exception("Application view not implemented");
            }
        }

        private void SetTray() {
            Tray = Services.GetRequiredService<IApplicationTray>();

            var cm = (ContextMenu)App.Current.Resources["TrayContextMenu"];
            cm.DataContext = Services.GetRequiredService<BaseViewModel>();

            Tray.ChangeIcon(WPFSampleApplication.Properties.Resources.trayIcon);
            Tray.ChangeContextMenu(cm);
        } 


        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices() {
            var services = new ServiceCollection();
            
            services.AddSingleton<ISettingsProcess, SettingsProcess>();
            services.AddSingleton<ISettingsWindowsUI, SettingsWindowsUI>();
            services.AddSingleton<IGlobalSettings, GlobalSettings>();
            services.AddSingleton<IApplicationSubtitles, ApplicationSubtitles>();
            services.AddSingleton<IApplicationThemes, ApplicationThemes>();

            services.AddSingleton<IBackgroundProcessModel, BackgroundProcessModel>();
            services.AddSingleton<IBackgroundProcessManager, BackgroundProcessManager>();

            services.AddSingleton<IWindowTaskbar, WindowTaskbar>();         
            services.AddSingleton<IApplicationTray, ApplicationTray>();
            services.AddSingleton<IPagesNavigator, PagesNavigator>();

            services.AddSingleton<ApplicationViewModel, ApplicationViewModel>();
            services.AddSingleton<BaseViewModel, BaseViewModel>();
            services.AddSingleton<MainWindowViewModel, MainWindowViewModel>();
            services.AddSingleton<ConfigPageViewModel, ConfigPageViewModel>();
            
            services.AddSingleton<MainWindow, MainWindow>();
            services.AddSingleton<ActionsPage, ActionsPage>();
            services.AddSingleton<AboutPage, AboutPage>();
            services.AddSingleton<ConfigPage, ConfigPage>();

            return services.BuildServiceProvider();
        }
    }
}
