using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleInjector;
using WritingExporter.Common;
using WritingExporter.Common.Wdc;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Storage;
using WritingExporter.Common.StorySyncWorker;

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
            RegisterWdc();

            // Register all of the forms
            RegisterForms();

            // Validate
            _container.Verify();

            return this;
        }

        public AppContext Start()
        {
            _log.Debug("Starting app context");

            // Load saved settings from file
            _container.GetInstance<IConfigProvider>().LoadSettings();

            // Start the story file store
            _container.GetInstance<IWdcStoryContainer>().Start();

            // Start the story sync worker
            _container.GetInstance<IStorySyncWorker>().StartWorker();

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
            _container.Register<IWdcStoryContainer, WdcStoryContainer>(Lifestyle.Singleton);
            // _container.Register<IWdcStorySyncWorker, WdcStorySyncWorker>(Lifestyle.Singleton);
            _container.Register<IStorySyncWorker, DummyStorySyncWorker>(Lifestyle.Singleton);
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
