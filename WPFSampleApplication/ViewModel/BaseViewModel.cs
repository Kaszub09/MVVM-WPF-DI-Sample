using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSampleApplication.ViewModel {

    public class BaseViewModel :INotifyPropertyChanged{
        public event PropertyChangedEventHandler PropertyChanged;
        public ApplicationViewModel AppViewModel { get; private set; }

        public BaseViewModel(ApplicationViewModel applicationViewModel) {
            AppViewModel = applicationViewModel;
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}