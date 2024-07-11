using AiGeneratedCodeDocs.Models;
using AiGeneratedCodeDocs.Services;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen.Blazor;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AiGeneratedCodeDocs.Components
{
    public partial class DocGenerator : ComponentBase
    {
        [Inject]
        private CreateDocumentService CreateDocumentService { get; set; } = default!;
        [Inject]
        private ILogger<DocGenerator> Logger { get; set; } = default!;
        [Inject]
        private DialogService DialogService { get; set; } = default!;
        private string _markdown = string.Empty;
        private string? _repoPath = @"";
        private IEnumerable<DirInfo> entries;

        protected override Task OnInitializedAsync()
        {
            //entries = GetRepos();
            return base.OnInitializedAsync();
        }
        private void AddDetails()
        {
            DialogService.Open<SettingsWindow>("Add Settings");
        }
        private Task<List<DirInfo>> GetRepos()
        {
            var dirs = Directory.EnumerateDirectories(_repoPath).Where(entry =>
            {
                var name = Path.GetFileName(entry);

                return !name.StartsWith(".") && name != "bin" && name != "obj";
            });
            var result = new List<DirInfo>();
            foreach (var dir in dirs)
            {
                var dirInfo = new DirInfo(Path.GetFileName(dir), dir);
                if (Directory.EnumerateDirectories(dir)?.Any() == true)
                {
                    dirInfo.SubDirectories = GetSubDirectories(dirInfo);
                }
                result.Add(dirInfo);

            }

            return Task.FromResult(result);
            //var repos = dirs.Select(x => new DirInfo(Path.GetFileName(x), x)).ToList();
        }

        private List<DirInfo> GetSubDirectories(DirInfo dirInfo)
        {
            var result = new List<DirInfo>();
            var dirs = Directory.EnumerateDirectories(dirInfo.DirFullPath).Where(entry =>
            {
                var name = Path.GetFileName(entry);

                return !name.StartsWith(".") && name != "bin" && name != "obj";
            }).Select(x => new DirInfo(Path.GetFileName(x), x)).ToList();
            foreach (var dir in dirs)
            {
                if (Directory.EnumerateDirectories(dir.DirFullPath)?.Any() == true)
                {
                    dir.SubDirectories = GetSubDirectories(dir);
                }
                result.Add(dir);
            }
            return result;
        }
        private class SelectRepoForm
        {
            public string RepoBase { get; set; } = @"";
            public DirInfo? RepoDir { get; set; }
            public string? OutputDir { get; set; }
            public string? OutputFileName { get; set; }
            public bool UseSelectedAsOutput { get; set; }
        }
        private SelectRepoForm _repoForm = new();
        private class SetRepoForm
        {
            public string? RepoPath { get; set; }
        }
        private SetRepoForm _setRepoForm = new();
        private async void SubmitRepo(SetRepoForm form)
        {
            _isBusy = true;
            StateHasChanged();
            await Task.Delay(1);
            _repoPath = form.RepoPath;
            _repoForm.RepoBase = form.RepoPath;
            entries = await GetRepos();
            _isBusy = false;
            StateHasChanged();
        }
        private record DirInfo(string DirName, string DirFullPath)
        {
            public List<DirInfo>? SubDirectories { get; set; }
        }

        private bool _isBusy;
        private async void Submit(SelectRepoForm selectRepoForm)
        {
            if (selectRepoForm.RepoDir == null) return;
            if (selectRepoForm.UseSelectedAsOutput)
                selectRepoForm.OutputDir = selectRepoForm.RepoDir.DirFullPath;
            _isBusy = true;
            StateHasChanged();
            await Task.Delay(1);
            await GenerateCodeDocFromDir(selectRepoForm.RepoDir.DirFullPath);
            _isBusy = false;
            StateHasChanged();
        }

        private int _inputTokens = 0;
        private string? _outputPath;
        private async Task GenerateCodeDocFromDir(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.*", searchOption: SearchOption.AllDirectories).Where(x => x.EndsWith(".razor") || x.EndsWith(".cs") && !x.EndsWith(".g.cs"));
            var sb = new StringBuilder();
            var dic = ReadFilesInDirectory(path);
            foreach (var (title, value) in dic)
            {
                var text = $"//{title}\n\n{value}";
                _inputTokens += Helpers.GetTokens(text);
                var content = await CreateDocumentService.GenerateMarkdownDocs(text);
                _markdown += content;
                sb.AppendLine(content);
                StateHasChanged();

            }
            _repoForm.OutputDir ??= path;
            var markdown = sb.ToString();
            var fileName = $"{_repoForm.OutputFileName}Readme.md";
            string? dirPath = Path.GetDirectoryName(_repoForm.OutputDir);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string outputPath = Path.Combine(dirPath ?? "", fileName);
            _outputPath = outputPath;
            await File.WriteAllTextAsync(outputPath, markdown);
        }
        private void OpenMarkdownFile()
        {
            if (string.IsNullOrWhiteSpace(_outputPath)) return;
            var vscodePath = @"C:\Users\a876302\AppData\Local\Programs\Microsoft VS Code\Code.exe";
            Process.Start(vscodePath, _outputPath);
        }
        public Dictionary<string, string> ReadFilesInDirectory(string directoryPath)
        {
            Logger.LogInformation("Reading files in directory: {directoryPath}", directoryPath);
            if (File.Exists(directoryPath))
                return ReadFile(directoryPath);
            var csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories)
                .ToList();

            var razorFiles = Directory.GetFiles(directoryPath, "*.razor", SearchOption.AllDirectories)
                .ToList();
            var files = csFiles.Concat(razorFiles).Where(x => !x.EndsWith(".g.cs") && !x.Contains("AssemblyAttributes") && !x.Contains("AssemblyInfo")).OrderBy(x => x).ToList();
            var fileTexts = new Dictionary<string, string>();
            foreach (var file in files)
            {
                var isPartial = file.EndsWith(".razor.cs") || (file.EndsWith(".razor") && files.Contains($"{file}.cs"));

                var fileName = Path.GetFileNameWithoutExtension(file).Replace(".razor", "");

                if (isPartial)
                {
                    if (fileTexts.ContainsKey(fileName))
                    {
                        fileTexts[fileName] += $"\n@code {{\n{File.ReadAllText(file)}\n}}";
                    }
                    else
                    {
                        fileTexts.Add(fileName, File.ReadAllText(file));
                    }
                }
                else
                {
                    fileTexts[fileName] = File.ReadAllText(file);
                }
            }

            return fileTexts;
        }
        private Dictionary<string, string> ReadFile(string path)
        {
            var fileTexts = new Dictionary<string, string>();
            var fileInfo = new System.IO.FileInfo(path);
            if (fileInfo.Extension == ".razor")
            {
                fileTexts.Add(fileInfo.Name, File.ReadAllText(fileInfo.FullName));
                var directory = fileInfo.Directory!.FullName;
                string file = $"{fileInfo.FullName}.cs";
                var hasPartial = Directory.GetFiles(directory).Contains(file);
                if (hasPartial)
                    fileTexts[fileInfo.Name] += $"\n@code {{\n{File.ReadAllText(file)}\n}}";
            }
            else
            {
                fileTexts.Add(fileInfo.Name, File.ReadAllText(fileInfo.FullName));
            }


            return fileTexts;
        }
        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        private static string ToMarkdownHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown)) return markdown;
            var result = Markdown.ToHtml(markdown, MarkdownPipeline);
            return result;
        }

        private void LoadFiles(TreeExpandEventArgs args)
        {
            if (args.Value is not DirInfo directory) return;
            args.Children.Data = !Directory.Exists(directory.DirFullPath) ? Enumerable.Empty<object>() : Directory.EnumerateFileSystemEntries(directory.DirFullPath).Where(entry =>
            {
                var name = Path.GetFileName(entry);

                return !name.StartsWith('.') && name != "bin" && name != "obj";
            }).Select(x => new DirInfo(Path.GetFileName(x), x)).ToList();
            args.Children.Text = GetTextForNode;
            if (directory?.DirFullPath?.Contains(".csproj") == true)
            {
                Console.WriteLine(".csproj file");
            }
            args.Children.HasChildren = (path) =>
            {
                if (path is not DirInfo dirInfo) return false;
                var exists = Directory.Exists(dirInfo.DirFullPath);
                return exists;

            };
            args.Children.Template = FileOrFolderTemplate;
        }

        private string GetTextForNode(object data)
        {
            string text;
            if (data is DirInfo dirInfo)
                text = Path.GetFileName(dirInfo.DirFullPath);
            else
                text = string.Empty;
            return text;
        }

        private string selectedDir = "";
        private void SelectDir()
        {
            var dir = FilePickerService.ShowOpenFolderDialog();
            _setRepoForm.RepoPath = dir;
        }
        private void OnSelected(object item)
        {
            if (item is not DirInfo dirInfo) return;
            selectedDir = dirInfo.DirFullPath;
            _repoForm.RepoDir = dirInfo;

            StateHasChanged();

        }
        private RenderFragment<RadzenTreeItem> FileOrFolderTemplate = (context) => builder =>
        {
            var path = context.Value as DirInfo;
            bool isDirectory = Directory.Exists(path?.DirFullPath ?? "");
            if (path?.DirFullPath?.Contains(".csproj") == true)
            {
                Console.WriteLine(".csproj file");
            }
            builder.OpenComponent<RadzenIcon>(0);
            builder.AddAttribute(1, nameof(RadzenIcon.Icon), isDirectory ? "folder" : "insert_drive_file");
            builder.CloseComponent();
            builder.AddContent(3, context.Text);
        };
    }
}
