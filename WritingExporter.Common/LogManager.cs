using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common
{
    public static class LogManager
    {
        private const string MSG_EX_NO_FACTORY = "A logger was requested from the log manager, but a logger factory has not been set";

        private static ILogFactory _logFactory;

        public static event EventHandler<LogEventArgs> OnLogEvent;

        public static void SetLogFactory(ILogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        public static ILogger GetLogger(Type type)
        {
            if (_logFactory == null) throw new NullReferenceException(MSG_EX_NO_FACTORY);
            return _logFactory.GetLogger(type);
        }

        public static ILogger GetLogger(string name)
        {
            if (_logFactory == null) throw new NullReferenceException(MSG_EX_NO_FACTORY);
            return _logFactory.GetLogger(name);
        }

        public static bool IsReady()
        {
            return _logFactory != null;
        }

        public static void DoLog(object sender, LogEventArgs args)
        {
            OnLogEvent?.Invoke(sender, args);
        }
    }

    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
        public Exception Exception { get; set; }
    }
}
