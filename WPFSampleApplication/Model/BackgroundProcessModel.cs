using BackgroundProcessSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSampleApplication.Model {
    public enum CurrentState {
        Started,
        Stopped
    }

    public class BackgroundProcessModel : IBackgroundProcessModel {
        private CurrentState _currentState = CurrentState.Stopped;
        public CurrentState CurrentState {
            get {
                return _currentState;
            }
            private set {
                if (_currentState != value) {
                    _currentState = value;
                    CurrentStateChanged?.Invoke(null, value);
                }
            }
        }
        public event EventHandler<CurrentState> CurrentStateChanged;

        private IGlobalSettings _settings;
        private IBackgroundProcessManager _bgProcessManager;
        private List<IProgress<ProgressInfo>> _progressInfoToUpdate = new List<IProgress<ProgressInfo>>();
        private List<IProgress<DateTime?>> _nextRunToUpdate = new List<IProgress<DateTime?>>();

        public BackgroundProcessModel(IBackgroundProcessManager backgroundProcessManager, IGlobalSettings globalSettings) {
            _settings = globalSettings;
            _bgProcessManager = backgroundProcessManager;

            UpdateProcessSettings();
            _bgProcessManager.NextRun = new Progress<DateTime?>((dt) => {
                foreach (var progToUpdate in _nextRunToUpdate) {
                    try {
                        progToUpdate.Report(dt);
                    } catch (Exception e) {//TODO LOGGER
                    }
                }
            });
            _bgProcessManager.ProcessProgress = new Progress<ProgressInfo>((pi) => {
                foreach (var progToUpdate in _progressInfoToUpdate) {
                    try {
                        progToUpdate.Report(pi);
                    } catch (Exception e) {//TODO LOGGER
                    }
                }
            });
        }

        public void UpdateProcessSettings() {
            _bgProcessManager.Parameters = new ProcessParameters() {
                Date = _settings.Process.ChosedDate,
                EmailCredentials = _settings.Process.EmailCredName,
                TMSServiceCredentials = _settings.Process.TMSServiceCredName
            };
            _bgProcessManager.ChangeInterval(_settings.Process.IntervalSec*1000);
        }

        public void RegisterIProgressToUpdate(IProgress<ProgressInfo> progress) {
            if (!_progressInfoToUpdate.Contains(progress))
                _progressInfoToUpdate.Add(progress);
        }
        public void RegisterIProgressToUpdate(IProgress<DateTime?> progress) {
            if (!_nextRunToUpdate.Contains(progress))
                _nextRunToUpdate.Add(progress);
        }

        public void DeregisterIProgressToUpdate(IProgress<ProgressInfo> progress) {
            if (_progressInfoToUpdate.Contains(progress))
                _progressInfoToUpdate.Remove(progress);
        }
        public void DeregisterIProgressToUpdate(IProgress<DateTime?> progress) {
            if (_nextRunToUpdate.Contains(progress))
                _nextRunToUpdate.Remove(progress);
        }

        public void Start() {
            if (_bgProcessManager.IsRunning == false) {
                _bgProcessManager.Start(_settings.Process.RunProcessImmediately);
                CurrentState = CurrentState.Started;
            }
        }

        public void Stop() {
            if (_bgProcessManager.IsRunning == true) {
                _bgProcessManager.Stop();
                CurrentState = CurrentState.Stopped;
            }
        }

    }
}
