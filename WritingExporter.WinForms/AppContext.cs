using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleInjector;
using WritingExporter.Common;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Storage;

namespace WritingExporter.WinForms
{
    public class AppContext
    {
        private Container _container;
        private ILogger _log;

        public AppContext()
        {
            _container = new Container();
        }

        public AppContext Setup()
        {
            // Setup logging
            var lf = new Log4NetLogFactory();
            lf.AddConsoleAppender().AddFileAppender().EndConfig();
            LogManager.SetLogFactory(lf);

            _log = LogManager.GetLogger(typeof(AppContext));
            _log.Debug("Starting setup of app context");

            // Setup GUI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Add stuff to the container


            // Register all of the forms
            RegisterForms();

            return this;
        }

        public AppContext Start()
        {
            _log.Debug("Starting app context");

            // Load saved settings from file
            _container.GetInstance<IConfigProvider>().LoadSettings();

            // Start the story file store
            _container.GetInstance<WdcStoryContainer>(); // Just fetching it should instantiate it and start it.

            // Start the story sync worker
            _container.GetInstance<WdcStorySyncWorker>().StartWorker();

            // Start the GUI
            Application.Run(_container.GetInstance<Forms.MainForm>());

            return this;
        }

        private void RegisterWdc()
        {
            _container.Register<IConfigProvider, ConfigProvider>(Lifestyle.Singleton);
            _container.Register<IWdcClient, WdcClient>(Lifestyle.Singleton);
            _container.Register<IWdcReader, WdcReader>(Lifestyle.Singleton);
            _container.Register<IStoryFileStore, XmlStoryFileStore>(Lifestyle.Singleton);
            _container.Register<IWdcStoryContainer>(Lifestyle.Singleton);
            _container.Register<WdcStorySyncWorker>(Lifestyle.Singleton); // TODO make interface
        }

        private void RegisterForms()
        {
            _log.Debug("Registering all forms");
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(Form)))
                {
                    _container.Register(t, t);

                    // Prevent IDisposable warnings for forms
                    _container.GetRegistration(t).Registration
                        .SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Ignore warnings for winforms");
                }
            }
        }
    }
}
