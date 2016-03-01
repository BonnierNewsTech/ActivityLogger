using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace ActivityLogger.Tests
{
    public class ActivityLoggerServiceTests
    {
        private readonly IActivityLoggerService _service;
        private readonly InfluxClientMock _mock;

        public ActivityLoggerServiceTests()
        {
            _mock = new InfluxClientMock();
            _service = new ActivityLoggerService(_mock, 5, TimeSpan.FromMilliseconds(200), "dbname", 1);
        }

        [Fact]
        public void Should_be_able_to_create_Instance()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void Should_be_able_to_store_datapoints()
        {
            _service.Add(CreateDatapoint());
            Assert.Equal(1, _service.Datapoints.Count());
        }

        private static Datapoint CreateDatapoint()
        {
            return new MyDatapoint("specific", 0.65);
        }

        [Fact]
        public void Should_empty_DatapointsList_when_BufferSize_has_reached()
        {
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            Assert.Equal(0, _service.Datapoints.Count());
        }

        [Fact]
        public void Should_send_SendingList_when_BufferSize_is_reached()
        {
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());

            _service.HaveRunningTasks?.WaitOne();

            Assert.Equal(5, _mock.SentItems);
        }

        [Fact]
        public void Should_empty_SendingList_after_successful_post()
        {
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.HaveRunningTasks?.WaitOne();
            Assert.Equal(0, _service.Sending.Count());
        }

        [Fact]
        public void Should_send_after_specified_timespan_even_if_BufferSize_has_not_been_reached()
        {
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            Thread.Sleep(500);
            _service.HaveRunningTasks?.WaitOne();
            Assert.Equal(0, _service.Datapoints.Count());
            Assert.Equal(0, _service.Sending.Count());
        }

        [Fact(Skip = "Unreliable test, timing issue.")]
        public void Should_not_send_before_specified_timespan()
        {
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());

            Assert.Equal(3, _service.Datapoints.Count());
            Assert.Equal(0, _service.Sending.Count());
        }
    }

    public class MyDatapoint : Datapoint
    {
        public MyDatapoint(string name, double value)
        {
            Tags = new Dictionary<string, string> {{"MyTag", $"app.machine.interactor.{name}"}};
            Fields = new Dictionary<string, double> {{"Elapsed", value}};
        }

        public override string MeasurementName { get; } = "Interactor";
    }
}