using Authi.Server.Extensions;
using Authi.Server.Services;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Authi.Server.Test
{
    [TestClass]
    public class AppHealthMonitorTests : ServerTestsBase
    {
        [TestMethod]
        public void AppHealthMonitorLoadsDataTest()
        {
            Directory.CreateDirectory(AppHealthMonitor.LogFolderPath);
            var filePath = AppHealthMonitor.LogFilePath;
            var @event = new AppHealthEvent(DateTimeOffset.UtcNow, "exception", "message");
            File.WriteAllLines(filePath, [JsonSerializer.Serialize(@event)]);

            var monitor = new AppHealthMonitor();

            var events = monitor.GetEvents();
            Assert.HasCount(1, events);
            Assert.AreEqual("message", events.First().Message);
        }

        [TestMethod]
        public void AppHealthMonitorReportAppendsEventTest()
        {
            DeleteLogFolder();
            var monitor = new AppHealthMonitor();
            var exception = new Exception("test");

            monitor.ReportEvent(exception);

            var events = monitor.GetEvents();
            Assert.HasCount(1, events);
            var loggedEvent = events.First();
            Assert.AreEqual("Exception", loggedEvent.ExceptionType);
            Assert.AreEqual("test", loggedEvent.Message);
        }

        [TestMethod]
        public void AppHealthMonitorReportDbErrorAppendsEventTest()
        {
            DeleteLogFolder();
            var monitor = new AppHealthMonitor();
            var exception = new Exception("test");
            var dbError = new InterceptedDbError("sql", exception);

            monitor.ReportEvent(dbError);

            var events = monitor.GetEvents();
            Assert.HasCount(1, events);
            var loggedEvent = events.First();
            Assert.AreEqual("InterceptedDbError", loggedEvent.ExceptionType);
            Assert.AreEqual("test", loggedEvent.Message);
            Assert.AreEqual("sql", loggedEvent.StackTrace);
        }

        [TestMethod]
        public void AppHealthMonitorReportTrimsEventsTest()
        {
            DeleteLogFolder();
            var monitor = new AppHealthMonitor();

            for (var i = 0; i < AppHealthMonitor.MaxEvents + 1; i++)
            {
                var exception = new Exception($"test {i}");
                monitor.ReportEvent(exception);
            }

            var events = monitor.GetEvents();
            Assert.HasCount(AppHealthMonitor.MaxEvents, events);
            Assert.AreEqual($"test 1", events.Last().Message);
            Assert.AreEqual($"test {AppHealthMonitor.MaxEvents}", events.First().Message);
        }

        [TestMethod]
        public void AppHealthMonitorSavesDataTest()
        {
            DeleteLogFolder();
            var monitor = new AppHealthMonitor();
            var exception = new Exception("test");
            monitor.ReportEvent(exception);

            monitor.Flush();

            string filePath = AppHealthMonitor.LogFilePath;
            Assert.IsTrue(File.Exists(filePath));
            var lines = File.ReadAllLines(filePath);
            Assert.AreEqual(1, lines.Length);
            Assert.Contains("test", lines[0]);
        }

        private static void DeleteLogFolder()
        {
            if (Directory.Exists(AppHealthMonitor.LogFolderPath))
            {
                Directory.Delete(AppHealthMonitor.LogFolderPath, true);
            }
        }
    }
}