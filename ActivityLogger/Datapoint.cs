using System;
using System.Collections.Generic;

namespace ActivityLogger
{
    public abstract class Datapoint : IDatapoint
    {
        public abstract string MeasurementName { get; }
        public IDictionary<string, string> Tags { get; protected set; } = new Dictionary<string, string>();
        public IDictionary<string, double> Fields { get; protected set; } = new Dictionary<string, double>();
        public virtual TimePrecision Precision { get; protected set; } = TimePrecision.Milliseconds;
        public DateTime UtcTimestamp { get; protected set; } = DateTime.UtcNow;
    }
}