using System.Windows.Shell;

namespace WPFSampleApplication.ViewModel {
    public interface IWindowTaskbar {
        void ChangeState(double? value = null, TaskbarItemProgressState? state = null, string description = null);
    }
}