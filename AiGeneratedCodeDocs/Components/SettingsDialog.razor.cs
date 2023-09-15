using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiGeneratedCodeDocs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace AiGeneratedCodeDocs.Components;

public partial class SettingsDialog
{
    [Parameter]
    public AppSettings? ActiveApp { get; set; }
    protected override Task OnParametersSetAsync()
    {
        if (ActiveApp != null)
        {
            _settingsForm = ActiveApp;
        }
        return base.OnParametersSetAsync();
    }
    private AppSettings _settingsForm = new();
    private record LanguageOptions(CodeLanguage Language, string Description);
    private readonly List<LanguageOptions> _languageOptions = Enum.GetValues<CodeLanguage>().ToList().Select(x => new LanguageOptions(x, x.ToEnumDescription())).ToList();
    
    private void Submit(AppSettings appSettings)
    {
        AppState.ActiveApp = appSettings;
        if (AppState.UserData.Apps.Any(x => x.Id == appSettings.Id))
        {
            AppState.UserData.Apps.Remove(AppState.UserData.Apps.First(x => x.Id == appSettings.Id));
            AppState.UserData.Apps.Add(appSettings);
            AppState.SaveUser();
        }
        else 
        {
            AppState.UserData.Apps.Add(appSettings);
            AppState.SaveUser();
        } 
        DialogService.Close(true);
    }
}