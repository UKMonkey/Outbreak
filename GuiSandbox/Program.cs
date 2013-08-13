using System;
using Psy.Core;
using Psy.Core.Configuration;
using Psy.Core.Console;
using Psy.Core.FileSystem;
using Psy.Core.Logging;
using Psy.Core.Logging.Loggers;

namespace GuiSandbox
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            StaticConfigurationManager.Initialize();
            StaticConfigurationManager.ConfigurationManager.SetString("LogDir", "VortexEditor");

            StaticConsole.Initialize();

            Logger.Add(new ConsoleLogger { LoggerLevel = LoggerLevel.Trace});
            Logger.Add(new FileLogger());

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;

            Lookup.AddPath(".");
            Lookup.AddPath(@"Layouts");
            Lookup.AddPath(@"..\..\OutbreakData", true);

            var windowAttributes = new WindowAttributes
                                   {
                                       Width = 800, 
                                       Height = 600,
                                       Title = "GuiSandbox",
                                       AllowResize = true
                                   };

            using (var sample = new EditorWindow(windowAttributes))
            {
                sample.Run();
            }
        }

        private static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                Logger.WriteException(exception);
            }

        }
    }
}
