using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace AiGeneratedCodeDocs
{
    public partial class Form1 : Form
    {
        BlazorWebView _blazor;
        public Form1()
        {
            InitializeComponent();

            _blazor = new BlazorWebView()
            {
                Dock = DockStyle.Fill,
                HostPage = "wwwroot/index.html",
                Services = Startup.Services
            };

            _blazor.RootComponents.Add<App>("#app");
            Controls.Add(_blazor);
        }
        private void HandleWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            //e.EnvironmentOptions.AllowSingleSignOnUsingOSPrimaryAccount = true;
            var appstate = _blazor.Services.GetRequiredService<AppState>();
            e.UserDataFolder =
                @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\OpenAiCodeBehindAnalysis\BlazorWebView\User Data";
            var userData = e.UserDataFolder;
            appstate.UserDataFolder = userData;
        }
    }
}