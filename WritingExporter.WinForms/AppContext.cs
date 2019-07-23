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
using WritingExporter.Common.StorySync;
using WritingExporter.Common.Gui;

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

            // Register system stuff
            RegisterSystem();

            // Add stuff to the container
            RegisterWdc();

            // Register all of the forms
            RegisterWinForms();

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

            // Shutdown
            _log.Info("Shutting down app context");

            return this;
        }

        private void RegisterSystem()
        {
            _log.Debug("Registering system services");
            _container.Register<IFileDumper, FileDumper>(Lifestyle.Singleton);
            _container.Register<IConfigProvider, ConfigProvider>(Lifestyle.Singleton);
            _container.Register<IStoryFileStore, XmlStoryFileStore>(Lifestyle.Singleton);
        }

        private void RegisterWdc()
        {
            _log.Debug("Registering WDC services");
            _container.Register<IWdcClient, WdcClient>(Lifestyle.Singleton);
            _container.Register<IWdcReader, WdcReader>(Lifestyle.Singleton);
            _container.Register<IWdcStoryContainer, WdcStoryContainer>(Lifestyle.Singleton);
            _container.Register<IStorySyncWorker, StorySyncWorker>(Lifestyle.Singleton);
            //_container.Register<IStorySyncWorker, DummyStorySyncWorker>(Lifestyle.Singleton);
        }

        private void RegisterWinForms()
        {
            _log.Debug("Registering all forms");

            _container.Register<IGuiContext, WinFormsGui>(Lifestyle.Singleton);

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
