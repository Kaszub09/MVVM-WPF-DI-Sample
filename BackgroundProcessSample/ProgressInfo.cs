using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundProcessSample {
    public enum Status {
        InProgress,
        Completed,
        Cancelled,
        Error
    }

    public struct ProgressInfo {
        public int ProcessedItems;
        public int AllItemsNumber;
        public string ErrorMessage;
        public Status Status;
    }
}
