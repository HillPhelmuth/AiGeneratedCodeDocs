using AiGeneratedCodeDocs.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AiGeneratedCodeDocs.Components;

public partial class SettingsWindow
{
    [Inject]
    private AppState AppState { get; set; } = default!;
    [Inject]
    private DialogService DialogService { get; set; } = default!;
    [Inject]
    private CreateDocumentService CreateDocumentService { get; set; } = default!;
    private bool _isBusy;
    private class SettingsForm
    {
        public string? DomainDescription { get; set; }
        public string? TechnicalDetails { get; set; }
        public List<string> PreviousSummaries { get; set; } = [];
    }
    private void OpenFilePicker()
    {
        var files = FilePickerService.ShowOpenMultipleFileDialog();
        if (files is null) return;
        _settingsForm.PreviousSummaries.AddRange(files);
    }
    private SettingsForm _settingsForm = new();
    protected override void OnInitialized()
    {
        _settingsForm.DomainDescription = AppState.DomainDescription;
        _settingsForm.TechnicalDetails = AppState.TechnicalDetails;
        base.OnInitialized();
    }
    private async void Submit(SettingsForm settingsForm)
    {
        _isBusy = true;
        StateHasChanged();
        await Task.Delay(1);
        AppState.TechnicalDetails = settingsForm.TechnicalDetails;
        AppState.DomainDescription = settingsForm.DomainDescription;
        AppState.PreviousSummaries = await CreateDocumentService.SummarizeMarkdownFiles(settingsForm.PreviousSummaries);
        _isBusy = true;
        StateHasChanged();
        await Task.Delay(500);
        DialogService.Close();
    }
}