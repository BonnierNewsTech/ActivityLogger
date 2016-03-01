using System;
using System.Linq;
using Xunit;

namespace ActivityLogger.Tests
{
    public class ActivityLoggerServiceSendingTests
    {
        private readonly IActivityLoggerService _service;
        private readonly InfluxFailureClientMock _mock;

        public ActivityLoggerServiceSendingTests()
        {
            _mock = new InfluxFailureClientMock();
            _service = new ActivityLoggerService(_mock, 5, TimeSpan.FromMilliseconds(1000), "dbname");
        }

        [Fact]
        public void Should_handle_posting_failure()
        {
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            _service.Add(CreateDatapoint());
            Assert.Equal(0, _service.Datapoints.Count());
            Assert.Equal(5, _service.Sending.Count());
        }

        private static Datapoint CreateDatapoint()
        {
            return new MyDatapoint("specific", 0.65);
        }

    }


}