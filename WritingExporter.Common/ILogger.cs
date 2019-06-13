using System;
using System.Collections.Generic;
using System.Text;

namespace WritingExporter.Common
{
    public interface ILogger
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void DebugFormat(string format, params object[] parms);

        void Info(object message);
        void Info(object message, Exception exception);
        void InfoFormat(string format, params object[] parms);

        void Warn(object message);
        void Warn(object message, Exception exception);
        void WarnFormat(string format, params object[] parms);

        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormat(string format, params object[] parms);

        void Fatal(object message);
        void Fatal(object message, Exception exception);
        void FatalFormat(string format, params object[] parms);
    }
}
