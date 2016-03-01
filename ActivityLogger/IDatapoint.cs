using System;
using System.Collections.Generic;

namespace ActivityLogger
{
    public interface IDatapoint
    {
        string MeasurementName { get; }
        IDictionary<string, string> Tags { get; }
        IDictionary<string,double> Fields { get; }
        TimePrecision Precision { get; }
        DateTime UtcTimestamp { get; }
    }
}