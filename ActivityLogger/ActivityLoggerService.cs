using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdysTech.InfluxDB.Client.Net;

namespace ActivityLogger
{
    public class ActivityLoggerService : IActivityLoggerService, IDisposable
    {
        private readonly IInfluxDBClient _client;
        private readonly int _bufferSize;
        private readonly TimeSpan _sendEvery;
        private TimeSpan _currentSendEvery;
        private readonly string _dbName;
        private readonly ConcurrentQueue<IDatapoint> _datapoints = new ConcurrentQueue<IDatapoint>();
        private readonly ConcurrentQueue<List<IDatapoint>> _sending = new ConcurrentQueue<List<IDatapoint>>();
        private readonly Timer _sendTimer;
        public IEnumerable<IDatapoint> Datapoints => _datapoints;
        public IEnumerable<IDatapoint> Sending => _sending.SelectMany(x => x.ToArray());

        public ActivityLoggerService(IInfluxDBClient client, int bufferSize, TimeSpan sendEvery, string dbName)
        {
            _client = client;
            _bufferSize = bufferSize;
            _sendEvery = sendEvery;
            _currentSendEvery = sendEvery;
            _dbName = dbName;
            _sendTimer = new Timer(SendUnsentItems, null, TimeSpan.Zero, sendEvery);
        }

        public void Add(IDatapoint datapoint)
        {
            _datapoints.Enqueue(datapoint);
            if(_datapoints.Count >= _bufferSize)
                SendWaitingItems();
        }

        private void SendWaitingItems()
        {
            var pointsToSend = new List<IDatapoint>();
            for (var i = 0; i < _bufferSize; i++)
            {
                IDatapoint dp;
                if (_datapoints.TryDequeue(out dp))
                {
                    pointsToSend.Add(dp);
                }
                else
                {
                    break;
                }
            }
            _sending.Enqueue(pointsToSend);
            Send();
        }

        private void SendUnsentItems(object state)
        {
            if (_datapoints == null || _datapoints.IsEmpty)
                return;

            SendWaitingItems();
        }

        public void Send()
        {
            if (Sending == null || !Sending.Any())
                return;

            List<IDatapoint> itemsToSend;
            if (_sending.TryDequeue(out itemsToSend))
            {
                var dispatcher = new DataPointDispatcher(_client, _dbName, itemsToSend);
                try
                {
                    dispatcher.Send((success, datapoints) =>
                    {
                        if (!success)
                        {
                            _sending.Enqueue(datapoints.ToList());
                            _currentSendEvery = _currentSendEvery.Add(_currentSendEvery);
                        }
                        else
                        {
                            _currentSendEvery = _sendEvery;
                        }
                        _sendTimer.Change(TimeSpan.Zero, _currentSendEvery);
                    });
                }
                catch (Exception ex)
                {
                    _currentSendEvery = _currentSendEvery.Add(_currentSendEvery);
                }
            }
        }

        public void Dispose()
        {
            if (_datapoints != null && !_datapoints.IsEmpty)
            {
                SendWaitingItems();
            }
            _sendTimer.Dispose();
        }
    }
}