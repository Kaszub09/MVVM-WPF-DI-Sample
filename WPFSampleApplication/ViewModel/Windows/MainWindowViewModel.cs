using BackgroundProcessSample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPFSampleApplication.Model;
using WPFSampleApplication.View;

namespace WPFSampleApplication.ViewModel {
    public class MainWindowViewModel :BaseViewModel {
        public double ProgressBarValue { get; set;}
        public string LeftStatusBar { get; set; }
        public string MiddleStatusBar { get; set; }

        public IPagesNavigator PagesNavigator { get; private set; }

        public ICommand NavigateForward { get; private set; }
        public ICommand NavigateBack { get; private set; }
        public ICommand NavigateTo { get; private set; }

        private Progress<ProgressInfo> _mainWindowProgressInfo;
        private Progress<DateTime?> _mainWindowNextRun;

        public MainWindowViewModel(ApplicationViewModel applicationViewModel,IPagesNavigator pagesNavigator) : base(applicationViewModel) {
            PagesNavigator = pagesNavigator;

            #region Commands
            NavigateForward = new Command((obj) => { PagesNavigator.GoForward(); }, (obj) => PagesNavigator.CanGoForward);

            NavigateBack = new Command((obj) => { PagesNavigator.GoBack(); }, (obj) => PagesNavigator.CanGoBack);

            NavigateTo = new Command((obj) => {
                var p = obj as Page;
                if (obj is string) {


                    PagesNavigator.Navigate((string)obj);
                } else if (p != null) {
                    p.DataContext = this;
                    PagesNavigator.Navigate(p);
                } else if (obj is Type) {
                    if (((Type)obj).IsSubclassOf(typeof(Page))) {
                        p = (Page)App.Current.Services.GetService((Type)obj);
                        if (((Type)obj).Equals(typeof(ConfigPage))) {
                            p.DataContext = App.Current.Services.GetService(typeof(ConfigPageViewModel));
                        } else {
                            p.DataContext = this;
                        }
                        PagesNavigator.Navigate(p);
                    }
                } else {
                    if (obj.GetType().IsSubclassOf(typeof(Page))) {
                        p = (Page)App.Current.Services.GetService(obj.GetType());
                        p.DataContext = this;
                        PagesNavigator.Navigate(p);
                    }
                }
            });
#endregion

            NavigateTo.Execute(typeof(ActionsPage));

            _mainWindowProgressInfo = new Progress<ProgressInfo>((p)=> {
                double progressValue = p.AllItemsNumber == 0 ? 100 : p.ProcessedItems * 100 / p.AllItemsNumber;
                string textToDisplay = "";
                switch (p.Status) {
                    case Status.InProgress:
                        textToDisplay = AppViewModel.AppSubtitles.GetText("process_status_in_progress", p.ProcessedItems, p.AllItemsNumber);
                        break;
                    case Status.Completed:
                        textToDisplay = AppViewModel.AppSubtitles.GetText("process_status_completed", p.ProcessedItems, p.AllItemsNumber, DateTime.Now);
                        break;
                    case Status.Cancelled:
                        textToDisplay = AppViewModel.AppSubtitles.GetText("process_status_cancelled", p.ProcessedItems, p.AllItemsNumber, DateTime.Now);
                        break;
                    case Status.Error:
                        textToDisplay = AppViewModel.AppSubtitles.GetText("process_status_error", p.ProcessedItems, p.AllItemsNumber, DateTime.Now, p.ErrorMessage);
                        break;
                    default:
                        break;
                }
                MiddleStatusBar = textToDisplay;
                ProgressBarValue = progressValue;
            });
            _mainWindowNextRun = new Progress<DateTime?>((dt)=> {
                if (dt.HasValue == true) {
                    LeftStatusBar = AppViewModel.AppSubtitles.GetText("left_status_bar_next_run_time" , dt.Value.Subtract(DateTime.Now));
                } else {
                    LeftStatusBar = AppViewModel.AppSubtitles.GetText("left_status_bar_next_run_never");
                }
            });

            AppViewModel.BackgroundProcessModel.RegisterIProgressToUpdate(_mainWindowProgressInfo);
            AppViewModel.BackgroundProcessModel.RegisterIProgressToUpdate(_mainWindowNextRun);
        }

        ~MainWindowViewModel() {
            AppViewModel.BackgroundProcessModel.DeregisterIProgressToUpdate(_mainWindowProgressInfo);
            AppViewModel.BackgroundProcessModel.DeregisterIProgressToUpdate(_mainWindowNextRun);
        }
    }
}
