using BackgroundProcessSample;
using System;

namespace WPFSampleApplication.Model {
    public interface IBackgroundProcessModel {
        CurrentState CurrentState { get; }

        event EventHandler<CurrentState> CurrentStateChanged;

        void DeregisterIProgressToUpdate(IProgress<DateTime?> progress);
        void DeregisterIProgressToUpdate(IProgress<ProgressInfo> progress);
        void RegisterIProgressToUpdate(IProgress<DateTime?> progress);
        void RegisterIProgressToUpdate(IProgress<ProgressInfo> progress);
        void Start();
        void Stop();
        void UpdateProcessSettings();
    }
}