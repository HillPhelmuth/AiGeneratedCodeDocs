using System.Globalization;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging.File;

namespace AiGeneratedCodeDocs
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"Logs/log-{DateTime.Now:yyyyMMdd_HHmmss}.txt", flushToDiskInterval:TimeSpan.FromDays(1))
                .CreateLogger();
            Startup.Init();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}