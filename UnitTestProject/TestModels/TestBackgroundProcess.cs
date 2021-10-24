using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackgroundProcessSample;

namespace UnitTestProject.TestModels {
    public class TestBackgroundProcess : IBackgroundProcessManager {
        public bool IsRunning { get; set; } = false;

        public IProgress<DateTime?> NextRun { get; set; }
        public ProcessParameters Parameters { get; set; }
        public IProgress<ProgressInfo> ProcessProgress { get; set; }

        public bool ChangeInterval(double intervalMilisec) {
            if (intervalMilisec > 0) {
                return true;
            } else {
                return false;
            }
        }

        public bool Start(bool alsoStartImmediately) {
            if (IsRunning==false) {
                IsRunning = true;
                return true;
            } else {
                return false;
            }
        }

        public bool Stop() {
            if (IsRunning == true) {
                IsRunning = false;
                return true;
            } else {
                return false;
            }
        }
    }
}
