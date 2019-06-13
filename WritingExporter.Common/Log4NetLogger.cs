using System;
using System.Collections.Generic;
using System.Text;

namespace WritingExporter.Common
{
    public class Log4NetLogger : ILogger
    {
        protected log4net.ILog _logger;

        public Log4NetLogger(log4net.ILog logger)
        {
            _logger = logger;
        }

        public void Debug(object message) => _logger.Debug(message);
        public void Debug(object message, Exception exception) => _logger.Debug(message, exception);
        public void DebugFormat(string format, params object[] parms) => _logger.DebugFormat(format, parms);

        public void Info(object message) => _logger.Info(message);
        public void Info(object message, Exception exception) => _logger.Info(message, exception);
        public void InfoFormat(string format, params object[] parms) => _logger.InfoFormat(format, parms);

        public void Warn(object message) => _logger.Warn(message);
        public void Warn(object message, Exception exception) => _logger.Warn(message, exception);
        public void WarnFormat(string format, params object[] parms) => _logger.WarnFormat(format, parms);

        public void Error(object message) => _logger.Error(message);
        public void Error(object message, Exception exception) => _logger.Error(message, exception);
        public void ErrorFormat(string format, params object[] parms) => _logger.ErrorFormat(format, parms);

        public void Fatal(object message) => _logger.Fatal(message);
        public void Fatal(object message, Exception exception) => _logger.Fatal(message, exception);
        public void FatalFormat(string format, params object[] parms) => _logger.FatalFormat(format, parms);
    }
}
