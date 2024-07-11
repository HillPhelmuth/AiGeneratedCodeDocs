using AiGeneratedCodeDocs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace AiGeneratedCodeDocs;

public class AppState : INotifyPropertyChanged
{
    private string? userDataFolder;
    private AppSettings? activeApp;
    private UserData? _userData;
    private string theme = "standard";
    private string? baseRepoPath;
    private string? domainDescription;
    private string? technicalDetails;
    private string? previousSummaries;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? UserDataFolder
    {
        get => userDataFolder;
        set
        {
            userDataFolder = value;
            OnPropertyChanged();
        }
    }
    public bool HasLoggedOut { get; set; }
    public AppSettings? ActiveApp
    {
        get => activeApp;
        set
        {
            activeApp = value;
            OnPropertyChanged();
        }
    }
    public UserData? UserData
    {
        get => _userData;
        set
        {
            _userData = value;
            OnPropertyChanged();
        }
    }
    public string Theme
    {
        get => theme;
        set
        {
            if (theme == value) return;
            theme = value;
            OnPropertyChanged();
        }
    }
    public string? BaseRepoPath
    {
        get => baseRepoPath;
        set
        {
            if (baseRepoPath == value) return;
            baseRepoPath = value;
            OnPropertyChanged();
        }
    }
    public bool IsGpt4 { get; set; } = true;
    public string? DomainDescription
    {
        get => domainDescription; 
        set
        {
            domainDescription = value;
            OnPropertyChanged();
        }
    }
    public string? TechnicalDetails
    {
        get => technicalDetails; 
        set
        {
            technicalDetails = value;
            OnPropertyChanged();
        }
    }
    public string? PreviousSummaries
    {
        get => previousSummaries; 
        set
        {
            previousSummaries = value;
            OnPropertyChanged();
        }
    }
    public void SaveUser()
    {
        var json = JsonSerializer.Serialize(UserData);
        File.WriteAllText($@"{UserDataFolder}\userData.json", json);
    }

    public void LoadUser()
    {
        var path = $@"{UserDataFolder}\userData.json";
        if (!File.Exists(path)) return;
        var json = File.ReadAllText(path);
        UserData = JsonSerializer.Deserialize<UserData>(json);
        Theme = UserData?.Theme ?? "standard";
    }
    public void OnPropertyChanged([CallerMemberName] string propertyName = default!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
