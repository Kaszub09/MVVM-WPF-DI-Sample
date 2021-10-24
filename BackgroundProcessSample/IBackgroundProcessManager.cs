using System;

namespace BackgroundProcessSample {
    public interface IBackgroundProcessManager {
        bool IsRunning { get; }
        IProgress<DateTime?> NextRun { set; }
        ProcessParameters Parameters { set; }
        IProgress<ProgressInfo> ProcessProgress { set; }

        bool ChangeInterval(double intervalMilisec);
        bool Start(bool alsoStartImmediately);
        bool Stop();
    }
}