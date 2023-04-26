using AiGeneratedCodeDocs.Models;
using AiGeneratedCodeDocs.Services;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
        private List<DirInfo> Repos => Directory.EnumerateDirectories(_repoPath).Select(x => new DirInfo(Path.GetFileName(x), x)).ToList();

        private class SelectRepoForm
        {
            public DirInfo? RepoDir { get; set; }
        }
        private SelectRepoForm _repoForm = new();
        private record DirInfo(string DirName, string DirFullPath);

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
            await File.WriteAllTextAsync($"{_repoForm.RepoDir?.DirName}docs.md", markdown);
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
    }
}
