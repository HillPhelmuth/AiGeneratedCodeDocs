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

namespace AiGeneratedCodeDocs.Components
{
    public partial class DocGenerator : ComponentBase
    {
        [Inject]
        private CreateDocumentService CreateDocumentService { get; set; } = default!;
        [Inject]
        private ILogger<DocGenerator> Logger { get; set; } = default!;
        private string _markdown = string.Empty;
        private string? _repoPath = @"C:\Users\adamh\source\repos";
        private IEnumerable<DirInfo> entries;

        protected override Task OnInitializedAsync()
        {
            entries = GetRepos();
            return base.OnInitializedAsync();
        }

        private List<DirInfo> GetRepos()
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

            return result;
            var repos = dirs.Select(x => new DirInfo(Path.GetFileName(x), x)).ToList();
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
            public string RepoBase { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
            public DirInfo? RepoDir { get; set; }
            public string? OutputDir { get; set; }
        }
        private SelectRepoForm _repoForm = new();

        private record DirInfo(string DirName, string DirFullPath)
        {
            public List<DirInfo>? SubDirectories { get; set; }
        }

        private bool _isBusy;
        private async void Submit(SelectRepoForm selectRepoForm)
        {
            _isBusy = true;
            StateHasChanged();
            await Task.Delay(1);
            await GenerateCodeDoc(selectRepoForm.RepoDir.DirFullPath);
            _isBusy = false;
            StateHasChanged();
        }

        private int _inputTokens = 0;
        private async Task GenerateCodeDoc(string path)
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
            var markdown = sb.ToString();
            var fileName = $"{_repoForm.RepoDir?.DirName}docs.md";
            if (!Directory.Exists(_repoForm.OutputDir))
                Directory.CreateDirectory(_repoForm.OutputDir);
            await File.WriteAllTextAsync(Path.Combine(_repoForm.OutputDir ?? "", fileName), markdown);
        }
        public Dictionary<string, string> ReadFilesInDirectory(string directoryPath)
        {
            Logger.LogInformation("Reading files in directory: {directoryPath}", directoryPath);
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
                        fileTexts[fileName] += File.ReadAllText(file);
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

                return !name.StartsWith(".") && name != "bin" && name != "obj";
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
        private void OnSelected(object item)
        {
            if (item is not DirInfo dirInfo) return;
            selectedDir = dirInfo.DirFullPath;
            _repoForm.RepoDir = dirInfo;
            _repoForm.OutputDir ??= dirInfo.DirFullPath;
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
