using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common
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
            LogManager.DoLog(this, new LogEventArgs()
            {
                Level = loggingEvent.Level.ToString(),
                Message = loggingEvent.RenderedMessage,
                Timestamp = loggingEvent.TimeStamp,
                Exception = loggingEvent.ExceptionObject,
            });
        }
    }

    

    public class Log4NetLogFactory : ILogFactory
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

        public ILogger GetLogger(Type type) => new Log4NetLogger(log4net.LogManager.GetLogger(type));
        public ILogger GetLogger(string name) => new Log4NetLogger(log4net.LogManager.GetLogger(name));
        public event EventHandler<LogEventArgs> OnLogEvent;

        public Log4NetLogFactory()
        {
            // Set some defaults
            var hierarchy = GetLogHierarchy();

            // Set configured to false, it hasn't been finished yet
            hierarchy.Configured = false;

            // Add the simple appender
            SimpleAppender simpleAppender = new SimpleAppender();
            hierarchy.Root.AddAppender(simpleAppender);

#if DEBUG
            SetLogLevel("debug", silent: true);
#else
            SetLogLevel("info", silent: true);
#endif
        }

        public void EndConfig()
        {
            // Mark as configured
            GetLogHierarchy().Configured = true;

            // Announce that we're done
            ILogger log = this.GetLogger(typeof(Log4NetLogFactory));
            log.InfoFormat("Logging setup at level: {0}", GetLogLevel());
        }

        public Log4NetLogFactory SetLogLevel(string logLevel, bool silent = false)
        {
            Hierarchy hierarchy = GetLogHierarchy();

            // Set the logging level
            hierarchy.Root.Level = ParseLevel(logLevel);

            if (!silent)
            {
                ILogger log = this.GetLogger(typeof(Log4NetLogFactory));
                log.InfoFormat("Logging setup at level: {0}", GetLogLevel());
            }

            return this;
        }

        public string GetLogLevel()
        {
            return GetLogHierarchy().Root.Level.DisplayName;
        }

        public Log4NetLogFactory AddConsoleAppender() => AddConsoleAppender(LOG_LAYOUT_CONSOLE);

        public Log4NetLogFactory AddConsoleAppender(string logPattern)
        {

            PatternLayout consolePatternLayout = new PatternLayout(logPattern);
            consolePatternLayout.ActivateOptions();

            ConsoleAppender console = new ConsoleAppender();
            console.Layout = consolePatternLayout;
            console.ActivateOptions();
            GetLogHierarchy().Root.AddAppender(console);

            return this;
        }

        public Log4NetLogFactory AddFileAppender() => AddFileAppender(LOG_LAYOUT_LONG);

        public Log4NetLogFactory AddFileAppender(string logPattern)
        {
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = logPattern;
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
            GetLogHierarchy().Root.AddAppender(roller);

            return this;
        }

        [Obsolete]
        public void Setup(string logLevel)
        {

            //Hierarchy hierarchy = (Hierarchy)log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());
            Hierarchy hierarchy = GetLogHierarchy();

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

            // Our custom event-drive appender
            SimpleAppender simpleAppender = new SimpleAppender();
            hierarchy.Root.AddAppender(simpleAppender);

            // Set the logging level
            hierarchy.Root.Level = ParseLevel(logLevel);
            hierarchy.Configured = true;

            //ILogger log = LogManager.GetLogger(typeof(LogManager));
            //log.InfoFormat("Logging setup at level: {0}", hierarchy.Root.Level.DisplayName);
        }

        /// <summary>
        /// Parse a log level name into a Log Level enumeration.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private log4net.Core.Level ParseLevel(string level)
        {
            var loggerRepository = log4net.LogManager.GetAllRepositories().FirstOrDefault();

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
        
        private Hierarchy GetLogHierarchy()
        {
            return (Hierarchy)log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());
        }

    }
}
