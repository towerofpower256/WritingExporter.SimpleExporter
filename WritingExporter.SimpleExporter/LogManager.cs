using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using System.Reflection;

namespace WritingExporter.SimpleExporter
{
    public class SimpleAppender : IAppender
    {
        public string Name { get; set; } = "SimpleAppender";

        public void Close()
        {
            // Do nothing
        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            LogManager.DoAppend(this, loggingEvent);
        }
    }

    public class AppendEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime Timestamp { get; set; }
        public Exception Exception { get; set; }
    }

    public static class LogManager
    {
#if DEBUG
        public const string DEFAULT_LOG_LEVEL = "debug";
#else
        public const string DEFAULT_LOG_LEVEL = "info";
#endif

        // E.g. 2018-09-27 [25] INFO TestLogFacade.Program - I am a message
        public const string LOG_LAYOUT_LONG = "%date [%thread] %-5level %logger - %message%newline";

        // E.g. 1895 INFO TestLogFacade.Program - I am a message
        public const string LOG_LAYOUT_CONSOLE = "%timestampms %-5level %logger - %message%newline";

        //public static ILogger GetLogger(string name) => new Logger(log4net.LogManager.GetLogger(name));
        public static ILogger GetLogger(Type type) => new Logger(log4net.LogManager.GetLogger(type));
        public static ILogger GetLogger(string repository, string name) => new Logger(log4net.LogManager.GetLogger(repository, name));
        public static ILogger GetLogger(string repository, Type type) => new Logger(log4net.LogManager.GetLogger(repository, type));
        public static ILogger GetLogger(Assembly assembly, string name) => new Logger(log4net.LogManager.GetLogger(assembly, name));
        public static ILogger GetLogger(Assembly assembly, Type type) => new Logger(log4net.LogManager.GetLogger(assembly, type));

        public static void Setup() => Setup(DEFAULT_LOG_LEVEL);

        public static event EventHandler<AppendEventArgs> OnAppend;

        internal static void DoAppend(object sender, LoggingEvent loggingEvent)
        {
            OnAppend?.Invoke(sender, new AppendEventArgs()
            {
                Level = loggingEvent.Level.Name,
                Message = loggingEvent.MessageObject.ToString(),
                Timestamp = loggingEvent.TimeStamp,
                Exception = loggingEvent.ExceptionObject
            });
        }

        public static void Setup(string logLevel)
        {
            Hierarchy hierarchy = (Hierarchy)log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = LOG_LAYOUT_LONG;
            patternLayout.ActivateOptions();

            // Log File appender
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = @"Logs\EventLog.txt";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "1GB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            // Console appender
            PatternLayout consolePatternLayout = new PatternLayout(LOG_LAYOUT_CONSOLE);
            consolePatternLayout.ActivateOptions();
            ConsoleAppender console = new ConsoleAppender();
            console.Layout = consolePatternLayout;
            console.ActivateOptions();
            hierarchy.Root.AddAppender(console);

            // Memory appender
            // Disable this to save memory, otherwise the RAM usage will shoot through the roof
            //MemoryAppender memory = new MemoryAppender();
            //memory.ActivateOptions();
            //hierarchy.Root.AddAppender(memory);

            // Our custom event-drive appender
            SimpleAppender simpleAppender = new SimpleAppender();
            hierarchy.Root.AddAppender(simpleAppender);

            // Set the logging level
            hierarchy.Root.Level = ParseLevel(logLevel);
            hierarchy.Configured = true;

            ILogger log = LogManager.GetLogger(typeof(LogManager));
            log.InfoFormat("Logging setup at level: {0}", hierarchy.Root.Level.DisplayName);
        }

        /// <summary>
        /// Parse a log level name into a Log Level enumeration.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static log4net.Core.Level ParseLevel(string level)
        {
            var loggerRepository = LoggerManager.GetAllRepositories().FirstOrDefault();

            if (loggerRepository == null)
            {
                throw new Exception("No logging repositories defined");
            }

            var stronglyTypedLevel = loggerRepository.LevelMap[level];

            if (stronglyTypedLevel == null)
            {
                throw new Exception("Invalid logging level specified");
            }

            return stronglyTypedLevel;
        }
    }
}
