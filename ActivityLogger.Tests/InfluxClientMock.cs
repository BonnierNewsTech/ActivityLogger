using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;

namespace ActivityLogger.Tests
{
    public class InfluxClientMock : IInfluxDBClient
    {

        public int SentItems { get; set; }

        public Task<bool> CreateDatabaseAsync(string dbName)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<string>> GetInfluxDBNamesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Dictionary<string, List<string>>> GetInfluxDBStructureAsync(string dbName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostRawValueAsync(string dbName, AdysTech.InfluxDB.Client.Net.TimePrecision precision,
            string content)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostRawValueAsync(string dbName, TimePrecision precision, string content)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostPointAsync(string dbName, IInfluxDatapoint point)
        {
            throw new System.NotImplementedException();
        }

        public Func<IEnumerable<IInfluxDatapoint>,InfluxClientMock,bool> WhenPost = (points, instance) =>
        {
            instance.SentItems = points.Count();
            return true;
        };

        public Task<bool> PostPointsAsync(string dbName, IEnumerable<IInfluxDatapoint> points)
        {
            return Task<bool>.Factory.StartNew(() => WhenPost(points, this));
        }

        public Task<List<dynamic>> QueryDBAsync(string dbName, string measurementQuery)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostValueAsync(string dbName, string measurement, long timestamp, AdysTech.InfluxDB.Client.Net.TimePrecision precision, string tags,
            string field, double value)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostValuesAsync(string dbName, string measurement, long timestamp, AdysTech.InfluxDB.Client.Net.TimePrecision precision, string tags,
            IDictionary<string, double> values)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostValueAsync(string dbName, string measurement, long timestamp, TimePrecision precision, string tags,
            string field, double value)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PostValuesAsync(string dbName, string measurement, long timestamp, TimePrecision precision, string tags,
            IDictionary<string, double> values)
        {
            throw new System.NotImplementedException();
        }
    }
}