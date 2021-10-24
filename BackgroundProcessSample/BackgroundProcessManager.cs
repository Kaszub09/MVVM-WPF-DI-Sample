using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BackgroundProcessSample {
    public class BackgroundProcessManager : IBackgroundProcessManager {
        public bool IsRunning { get; private set; }

        private double _intervalMilisec = 0;
        private TimerPlus _processTimer;
        private Timer _timeUpdate;

        public IProgress<DateTime?> NextRun { private get; set; }
        public IProgress<ProgressInfo> ProcessProgress { private get; set; }
        public ProcessParameters Parameters { private get; set; }

        private CancellationTokenSource _token;
        private Task _process;

        public BackgroundProcessManager() {
            IsRunning = false;

            _timeUpdate = new Timer(500);
            _timeUpdate.Elapsed += _timeUpdate_Elapsed;
            

            _processTimer = new TimerPlus();
            _processTimer.AutoReset = true;
            _processTimer.Elapsed += _processTimer_Elapsed;
        }

        private void _timeUpdate_Elapsed(object sender, ElapsedEventArgs e) {
            NextRun?.Report(_processTimer.NextRun());
        }

        private void _processTimer_Elapsed(object sender, ElapsedEventArgs e) {
            if (_process != null) {
                if (_process.IsCompleted == false) {
                    return;
                }
            }

            _process = new Task(() => {
                var p = new Process(Parameters);
                p.ProcessProgress = ProcessProgress;
                p.Run(_token.Token);
            });
            _process.Start();
        }

        public bool Start(bool alsoStartImmediately) {
            if (IsRunning == false) {
                _token = new CancellationTokenSource();

                //Start process timer
                _processTimer.Interval = _intervalMilisec;
                _processTimer.Start();
                IsRunning = true;

                //Start info about next run timer
                _timeUpdate.Start();

                if (alsoStartImmediately)
                    _processTimer_Elapsed(null, null);
                return true;
            } else {
                return false;
            }
        }

        public bool Stop() {
            if (IsRunning == true) {
                _processTimer.Stop();   //Stop timer
                _token.Cancel();    //Request proces to stop
                _process?.Wait();    //Wait for process to stop
                IsRunning = false;
                //Stop info about next run timer
                _timeUpdate.Stop();
                NextRun?.Report(_processTimer.NextRun());
                return true;
            } else {
                return false;
            }
        }

        public bool ChangeInterval(double intervalMilisec) {
            if (intervalMilisec > 0) {
                _intervalMilisec = intervalMilisec;
                _processTimer.Interval = intervalMilisec;
                return true;
            } else {
                return false;
            }
        }

    }
}
