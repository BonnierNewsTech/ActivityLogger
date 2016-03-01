using System.Collections.Generic;
using System.Threading;

namespace ActivityLogger
{
    public interface IActivityLoggerService
    {
        IEnumerable<IDatapoint> Datapoints { get; }
        IEnumerable<IDatapoint> Sending { get; }
        void Add(IDatapoint datapoint);
        WaitHandle HaveRunningTasks { get; }
    }
}