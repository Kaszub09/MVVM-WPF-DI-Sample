using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFSampleApplication.ViewModel {
    public class PagesNavigator : INotifyPropertyChanged, IPagesNavigator {
        public event EventHandler<Page> NavigatedTo;
        public event EventHandler<Page> NavigateFrom;
        public event PropertyChangedEventHandler PropertyChanged;

        public Page CurrentPage { get; private set; }

        private Dictionary<string, Page> _allPages;
        private List<Page> _navigationHistory;
        private int _currentPageNumber;

        public bool CanGoBack {
            get {
                if (_navigationHistory.Count > 0 && _currentPageNumber > 0) {
                    return true;
                }
                return false;
            }
        }
        public bool CanGoForward {
            get {
                if (_currentPageNumber < _navigationHistory.Count - 1) {
                    return true;
                }
                return false;
            }
        }

        public PagesNavigator() {
            _allPages = new Dictionary<string, Page>();
            _navigationHistory = new List<Page>();
            _currentPageNumber = -1;
        }

        public void AddPage(string key, Page page) {
            _allPages[key] = page;
        }

        public void Navigate(string key) {
            if (_allPages.ContainsKey(key)) {
                if (_navigationHistory.Count - 1 > _currentPageNumber) {
                    _navigationHistory.RemoveRange(_currentPageNumber + 1, _navigationHistory.Count - _currentPageNumber - 1);
                }

                if (CurrentPage != _allPages[key]) {
                    _currentPageNumber++;
                    _navigationHistory.Add(_allPages[key]);
                }

                ChangePage(_allPages[key], null);
            } else {
                try {
                    Navigate(Type.GetType(key));
                } catch (Exception r) { //TODO LOGGER?
                }
            }
        }

        public void Navigate(Type type) {
            foreach (var item in _allPages) {
                if (item.Value.GetType() == type) {
                    Navigate(item.Key);
                    return;
                }
            }

            if (type.IsSubclassOf(typeof(Page))) {
                _allPages[type.Name] = (Page)Activator.CreateInstance(type);
                Navigate(type.Name);
            }
        }

        public void Navigate(Page page) {
            var key = page.GetType().Name;
            if (_allPages.ContainsKey(key)) {
                Navigate(key);
            } else {
                _allPages[key] = page;
                Navigate(key);
            }
        }

        public void GoBack() {
            if (_navigationHistory.Count > 0 && _currentPageNumber > 0) {
                _currentPageNumber--;
                ChangePage(_navigationHistory[_currentPageNumber], null);
            }
        }

        public void GoForward() {
            if (_currentPageNumber < _navigationHistory.Count - 1) {
                _currentPageNumber++;
                ChangePage(_navigationHistory[_currentPageNumber], null);
            }
        }

        private void ChangePage(Page page, object parameter) {
            EventHandler<Page> handler = NavigateFrom;
            handler?.Invoke(this, CurrentPage);
            CurrentPage = page;
            handler = NavigatedTo;
            handler?.Invoke(this, page);
        }

    }

}

