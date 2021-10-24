using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BackgroundProcessSample {

    public class TimerPlus : Timer {
        DateTime? _nextEvent;
        public new double Interval {
            get { return base.Interval; }
            set {
                this._nextEvent = DateTime.Now.AddMilliseconds(value);
                base.Interval = value;
            }
        }

        public TimerPlus() : base() {
            this.Elapsed += TimerPlus_Elapsed;
        }

        public TimerPlus(double interval) : base(interval) {
            this.Elapsed += TimerPlus_Elapsed;
        }

        public DateTime? NextRun() {
            return _nextEvent;
        }

        public double? MilisecTillNextRun() {
            if (_nextEvent == null) {
                return null;
            } else {
                return _nextEvent.Value.Subtract(DateTime.Now).TotalMilliseconds;
            }
        }

        public new void Start() {
            this._nextEvent = DateTime.Now.AddMilliseconds(this.Interval);
            base.Start();
        }

        public new void Stop() {
            _nextEvent = null;
            base.Stop();
        }

        protected new void Dispose() {
            this.Elapsed -= this.TimerPlus_Elapsed;
            base.Dispose();
        }

        private void TimerPlus_Elapsed(object sender, ElapsedEventArgs e) {
            if (this.AutoReset == true)
                _nextEvent = e.SignalTime.AddMilliseconds(this.Interval);
            else
                _nextEvent = null;
        }

    }

}