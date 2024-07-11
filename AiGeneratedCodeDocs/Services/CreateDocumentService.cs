using System.Text;
using AiGeneratedCodeDocs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net;

namespace AiGeneratedCodeDocs.Services
{
    public class CreateDocumentService
    {
        private StringBuilder _docBuilder;
        private StringBuilder _summaryBuilder;
        private readonly ILogger<CreateDocumentService> _logger; 
        private readonly IConfiguration _configuration;
        private Kernel _kernel;
        private readonly AppState _appState;
        public CreateDocumentService(ILogger<CreateDocumentService> logger, IConfiguration configuration, AppState appState)
        {
            _appState = appState;
            _docBuilder = new StringBuilder("Previous Files:\n");
            _summaryBuilder = new StringBuilder();
            _logger = logger;
            _logger.LogInformation("CreateDocumentService Initialized");
            _configuration = configuration;
            var deploymentName = _configuration["AzureOpenAI:Gpt4DeploymentName"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            if (deploymentName is null || apiKey is null || endpoint is null)
                throw new ArgumentException($"Azure OpenAI Deployment name, endpoint, and/or Api key is missing.");
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.ConfigureHttpClientDefaults(c =>
            {
                c.ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(4);
                });
                c.AddStandardResilienceHandler().Configure(o =>
                {
                    o.Retry.ShouldHandle = args => ValueTask.FromResult(args.Outcome.Result?.StatusCode is HttpStatusCode.TooManyRequests);
                    o.Retry.BackoffType = DelayBackoffType.Exponential;
                    o.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(5) };
                    o.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);
                    o.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(20) };
                });
            });
            _kernel = kernelBuilder
                //.WithOpenAIChatCompletionService("gpt-3.5-turbo-16k", _configuration["OPENAI_API_KEY"], alsoAsTextCompletion: true)
                .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
                .Build();
            _appState = appState;
        }
        public static string PluginDirectoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory(), "Plugins");
        
        /// <summary>
        /// Uses Azure OpenAI with Semantic Kernel to generate Markdown documents from code
        /// </summary>
        /// <param name="code">The code to document</param>
        /// <returns></returns>
        public async Task<string> GenerateMarkdownDocs(string code)
        {
            
            var docTokens = Helpers.GetTokens(_docBuilder.ToString());
            var sysTokens = 550;
            var inputTokens = Helpers.GetTokens(code);
            string doc;
            var requiresDocSummary = docTokens + sysTokens + inputTokens + MaxResponseTokens > MaxModelWindow;
            if (requiresDocSummary)
            {
                var tokens = docTokens + sysTokens + MaxResponseTokens;
                _logger.LogInformation("Generateing Summary. Token Length: {tokens}", tokens);
                doc = await Summarize(_docBuilder.ToString());
                _docBuilder = new StringBuilder(doc);
            }
            else            
                doc = _docBuilder.ToString();
            var requiresCodeTruncation = Helpers.GetTokens(doc) + inputTokens + sysTokens + MaxResponseTokens > MaxModelWindow;
            if (requiresCodeTruncation)
            {
                var sections = SplitCodeToSections(code);
                var combinedContent = "";
                foreach (var section in sections)
                {
                    var genContent = await GetMarkdownFromAi(section, _docBuilder.ToString());
                    combinedContent += genContent;
                }
                return combinedContent;
            }
            return await GetMarkdownFromAi(code, doc);
        }
        public async Task<string> SummarizeMarkdownFiles(IEnumerable<string> markdownFilePaths)
        {
            var sb = new StringBuilder();
            foreach (var path in markdownFilePaths)
            {
                var content = await File.ReadAllTextAsync(path);
                //var summary = await Summarize(content);
                //sb.AppendLine($"## {Path.GetFileNameWithoutExtension(path)}");
                sb.AppendLine(content);
            }
            return sb.ToString();
        }
        private async Task<string> Summarize(string document)
        {
            var oldDocTokens = Helpers.GetTokens(document);
            _logger.LogInformation("Summarizing Document of Token Length: {oldDocTokens}", oldDocTokens);
            //var summerizer = _kernel.ImportPluginFromPromptDirectory(Path.Combine(PluginDirectoryPath, "CodeDocGenPlugin"), "CodeDocGenPlugin");
            var plugin = _kernel.Plugins.FirstOrDefault(p => p.Name == "CodeDocGenPlugin");
            if (plugin == null)
            {
                plugin = _kernel.ImportPluginFromPromptDirectory(Path.Combine(PluginDirectoryPath, "CodeDocGenPlugin"), "CodeDocGenPlugin");
            }
            var result = await _kernel.InvokeAsync(plugin["Summarize"], new() { ["input"] = document});
            var content = result.GetValue<string>();
            _logger.LogInformation("Document summary completed.\nSummary:\n{content}", content);
            var newDocTokens = Helpers.GetTokens(content);
            _logger.LogInformation("Summary Generated. Token Length: {newDocTokens}", newDocTokens);
            return content;
        }

        private const int MinTokens = 2000;
        private const int MaxTokens = 3000;
        private const int MaxResponseTokens = 4000;
        private const int MaxModelWindow = 120000;

        public static List<string> SplitCodeToSections(string code, int minTokens = 0)
        {
            var sections = new List<string>();
            if (string.IsNullOrEmpty(code)) return sections;
            if (minTokens == 0)
                minTokens = MinTokens;
            var lines = code.Split('\n');

            var currentSection = "";
            var currentTokens = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (IsPropertyOrMethodDefinition(line))
                {
                    if (currentTokens > 0)
                    {
                        var tokensInLine = Helpers.GetTokens(line);
                        if (currentTokens + tokensInLine >= minTokens)
                        {
                            sections.Add(currentSection);
                            currentSection = "";
                            //currentTokens = 0;
                        }
                    }
                }

                currentSection += line + "\n";
                currentTokens = Helpers.GetTokens(currentSection);

                if (i == lines.Length - 1)
                {
                    sections.Add(currentSection);
                    break;
                }
            }

            return sections;
        }

        private static bool IsPropertyOrMethodDefinition(string line)
        {
            var trimmed = line.TrimStart();
            return trimmed.StartsWith("public") || trimmed.StartsWith("private") || trimmed.StartsWith("protected");
        }
        private async Task<string> GetMarkdownFromAi(string code, string doc)
        {
            var plugin = _kernel.Plugins.FirstOrDefault(p => p.Name == "CodeDocGenPlugin");
            if (plugin == null)
            {
                plugin = _kernel.ImportPluginFromPromptDirectory(Path.Combine(PluginDirectoryPath, "CodeDocGenPlugin"), "CodeDocGenPlugin");
            }
            _summaryBuilder = new StringBuilder();
            _summaryBuilder.AppendLine("Previous Summaries: \n");
            _summaryBuilder.AppendLine(_appState.PreviousSummaries);
            _summaryBuilder.AppendLine("Current Summary: \n");
            _summaryBuilder.AppendLine(doc);
            var args = new KernelArguments
            {
                ["summary"] = doc,
                ["code"] = code,
                ["domainDescription"] = _appState.DomainDescription,
                ["technicalDescription"] = _appState.TechnicalDetails
            };
            var result = await _kernel.InvokeAsync(plugin["DocumentCode"], args);
            var content = result.GetValue<string>();
            Console.WriteLine(content);
            _docBuilder.AppendLine(content);
            _docBuilder.AppendLine();
            return content;
            
        }

       
    }
}
