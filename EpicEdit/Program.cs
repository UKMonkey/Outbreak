using System;
using System.Reflection;
using Psy.Core;
using Psy.Core.Configuration;
using Psy.Core.Configuration.Sources;
using Psy.Core.Console;
using Psy.Core.FileSystem;
using Psy.Core.Logging;
using Psy.Core.Logging.Loggers;

namespace EpicEdit
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            StaticConfigurationManager.Initialize();
            StaticConfigurationManager.ConfigurationManager.RegisterSource(new FileConfigurationSource("epicedit.cfg"));

            StaticConsole.Initialize();
            StandardCommands.Attach();

            StaticConfigurationManager.ConfigurationManager.SetString("LogDir", "EpicEdit");

            Logger.Add(new CommandPromptLogger {LoggerLevel = LoggerLevel.Trace} );
            Logger.Add(new FileLogger { FlushAfterEachWrite = true, LoggerLevel = LoggerLevel.Debug } );
            Logger.Add(new ConsoleLogger { LoggerLevel = LoggerLevel.Info} );

            var dataPath = StaticConfigurationManager.ConfigurationManager.GetString("DataPath");

            Lookup.AddPath(".", true);

            if (!string.IsNullOrEmpty(dataPath))
            {
                Lookup.AddPath(dataPath, true);
            }

            Lookup.DumpPaths();

            var windowAttributes = new WindowAttributes
            {
                Width = 1024,
                Height = 768,
                Title = GetWindowTitle(),
                AllowResize = true
            };

            AppDomain.CurrentDomain.UnhandledException += 
                (sender, eventArgs) => Logger.WriteException((Exception)eventArgs.ExceptionObject);

            using (var window = new EditorWindow(windowAttributes))
            {
                window.Run();
            }
        }

        private static string GetWindowTitle()
        {
            return string.Format("Vortex Engine :: EpicEdit :: [{0}]", Assembly.GetExecutingAssembly().GetName().Version);
        }
    }

}
