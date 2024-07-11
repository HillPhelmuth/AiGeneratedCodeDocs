using AiGeneratedCodeDocs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Radzen;


namespace AiGeneratedCodeDocs
{
    public static class Startup
    {
        public static IServiceProvider? Services { get; private set; }

        public static void Init()
        {
            var host = Host.CreateDefaultBuilder()
                           .ConfigureServices(WireupServices)
                           .Build();
            Services = host.Services;
        }

        private static void WireupServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddWindowsFormsBlazorWebView();
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);
            var config = context.Configuration;
            services.AddSingleton(client);
            services.AddRadzenComponents();
            services.AddSingleton<AppState>();
            services.AddSingleton<CreateDocumentService>();
            services.AddSingleton<ILoggerFactory>(new Serilog.Extensions.Logging.SerilogLoggerFactory());

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
        }
    }
}