using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;

namespace ActivityLogger
{
    public class DataPointDispatcher
    {
        private readonly IInfluxDBClient _client;
        private readonly string _dbName;
        private readonly IEnumerable<IDatapoint> _datapoints;

        public DataPointDispatcher(IInfluxDBClient client, string dbName, IEnumerable<IDatapoint> datapoints)
        {
            _client = client;
            _dbName = dbName;
            _datapoints = datapoints;
        }

        public void Send(Action<bool, IEnumerable<IDatapoint>> onCompleted)
        {
            try
            {
                _client.PostPointsAsync(_dbName, CreateInfluxDatapoints(_datapoints)).ContinueWith((task, state) =>
                {
                    onCompleted(task.Result, state as IEnumerable<IDatapoint>);
                }, _datapoints, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (HttpRequestException e)
            {
                onCompleted(false, _datapoints);
            }
        }

        private IEnumerable<InfluxDatapoint<double>> CreateInfluxDatapoints(IEnumerable<IDatapoint> datapoints)
        {
            return datapoints.Select(x =>
            {
                var datapoint = new InfluxDatapoint<double>()
                {
                    MeasurementName = x.MeasurementName,
                    Precision =
                        (AdysTech.InfluxDB.Client.Net.TimePrecision)
                            Enum.Parse(typeof(AdysTech.InfluxDB.Client.Net.TimePrecision), x.Precision.ToString()),
                };
                x.Fields.ToList().ForEach(f => datapoint.Fields.Add(f.Key, f.Value));
                x.Tags.ToList().ForEach(f => datapoint.Tags.Add(f.Key, f.Value));
                return datapoint;
            });
        }
    }
}