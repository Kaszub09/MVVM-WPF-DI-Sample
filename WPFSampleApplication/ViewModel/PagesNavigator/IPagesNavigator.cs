using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace WPFSampleApplication.ViewModel {
    public interface IPagesNavigator : INotifyPropertyChanged {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        Page CurrentPage { get; }

        event EventHandler<Page> NavigatedTo;
        event EventHandler<Page> NavigateFrom;

        void AddPage(string key, Page page);
        void GoBack();
        void GoForward();
        void Navigate(string key);
        void Navigate(Type type);
        void Navigate(Page page);
    }
}