using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundProcessSample {
    public class Process {
        public IProgress<ProgressInfo> ProcessProgress { private get; set; }
        public ProcessParameters Parameters { private get; set; }

        private CancellationToken? _token;
       
        public Process(ProcessParameters parameters) {
            Parameters = parameters;
        }

        public void Run(CancellationToken? token = null) {
            _token = token;
            var itemsToProcecss = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var progInfo = new ProgressInfo() { AllItemsNumber = itemsToProcecss.Count, Status = Status.InProgress, ProcessedItems = 0 };
            ProcessProgress?.Report(progInfo);

            try {
                foreach (var item in itemsToProcecss) {
                    if (_token.HasValue && _token.Value.IsCancellationRequested == true) {
                        progInfo.Status = Status.Cancelled;
                        break;
                    }

                    Thread.Sleep(500);
                    progInfo.ProcessedItems++;
                    ProcessProgress?.Report(progInfo);
                }
            } catch (Exception e) {
                progInfo.Status = Status.Error;
                progInfo.ErrorMessage = e.Message;
            } finally {
                if(progInfo.Status==Status.InProgress)
                    progInfo.Status = Status.Completed;
                ProcessProgress?.Report(progInfo);
            }
        }

    }
}
