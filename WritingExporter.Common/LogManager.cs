using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common
{
    public static class LogManager
    {
        private static ILogFactory _logFactory;

        public static void SetLogFactory(ILogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        public static ILogger GetLogger(Type type) => _logFactory.GetLogger(type);
        public static ILogger GetLogger(string name) => _logFactory.GetLogger(name);

        public static void DoLog(object sender, LogEventArgs args)
        {
            OnLogEvent?.Invoke(sender, args);
        }

        public static event EventHandler<LogEventArgs> OnLogEvent;
    }

    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime Timestamp { get; set; }
        public Exception Exception { get; set; }
    }
}
