using System.Text;
using AiGeneratedCodeDocs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using System.Reflection;

namespace AiGeneratedCodeDocs.Services
{
    public class CreateDocumentService
    {
        //private readonly OpenAIDotNetService _openAiService;
        private StringBuilder _docBuilder;
        private readonly ILogger<CreateDocumentService> _logger; 
        private readonly IConfiguration _configuration;
        private IKernel _kernel;
        public CreateDocumentService(ILogger<CreateDocumentService> logger, IConfiguration configuration)
        {
            _docBuilder = new StringBuilder("Previous Files:\n");
            _logger = logger;
            _logger.LogInformation("CreateDocumentService Initialized");
            _configuration = configuration;
            _kernel = Kernel.Builder
                .WithLogger(_logger)
                .WithOpenAIChatCompletionService("gpt-3.5-turbo-16k", _configuration["OPENAI_API_KEY"], alsoAsTextCompletion: true)
                .Build();
        }
        public static string SkillsDirectoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory(), "Skills");
        private const string MkDownDocSysPromptText = """
 You are a Code Documentation Generator. Use the previously generated documentation summary as your context guide.
    <summary>
    {{$summary}}
    </summary>
Description: You are an expert in the all programming language and their associated frameworks. Your job is to explain snippets of code and explain clearly and in plain language what the code does USING MARKDOWN (.md) FORMAT ONLY. Assume you're generating documentation for someone who knows how to code, but is unfamiliar with the code base."
                      
Additional Instructions:
    - Never use a markdown heading level 1 (#). Only use heading level 2 (##) or lower.
    - The text will have a comment at the top of the file that is the file name. Use the file name as a MARKDOWN heading level 2 (##). 
    
YOUR RESPONSE WILL ALWAYS BE IN MARKDOWN FORMAT

<code>
{{$code}}
</code>
""";
        public async Task<string> GenerateMarkdownDocs(string code, string userName = "test")
        {
            
            var docTokens = Helpers.GetTokens(_docBuilder.ToString());
            var sysTokens = Helpers.GetTokens(MkDownDocSysPromptText);
            var inputTokens = Helpers.GetTokens(code);
            string doc;
            var requiresDocSummary = docTokens + sysTokens + inputTokens + MaxResponseTokens > MaxModelWindow;
            if (requiresDocSummary)
            {
                var tokens = docTokens + sysTokens + MaxResponseTokens;
                _logger.LogInformation("Generateing Summary. Token Length: {tokens}", tokens);
                doc = await Summarize(userName, _docBuilder.ToString());
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
                    var genContent = await GetMarkdownFromAi(section, userName, _docBuilder.ToString());
                    combinedContent += genContent;
                }
                return combinedContent;
            }
            return await GetMarkdownFromAi(code, userName, doc);
        }

        private async Task<string> Summarize(string userName, string document)
        {
            var oldDocTokens = Helpers.GetTokens(document);
            _logger.LogInformation("Summarizing Document of Token Length: {oldDocTokens}", oldDocTokens);
            var docSkill = _kernel.ImportSemanticSkillFromDirectory(SkillsDirectoryPath, "CodeDocGenSkill");
            var ctx = _kernel.CreateNewContext();
            
            var result = await _kernel.RunAsync(ctx.Variables, docSkill["Summarize"]);
            var content = result.Result;
            //var summaryPrompt = "Generate a summary of the document. Make it about half the size of the document.";
            //var summaryRequest = GetChatRequestModel(userName,
            //    new List<Message> {Message.Create(summaryPrompt, Role.System), Message.Create(document)});
            //var summaryResponse = await _openAiService.ChatService.Create(summaryRequest);
            //string doc = summaryResponse?.Choices?[0]?.Message?.Content ?? "No Content in response";
            var newDocTokens = Helpers.GetTokens(content);
            _logger.LogInformation("Summary Generated. Token Length: {newDocTokens}", newDocTokens);
            return content;
        }

        private const int MinTokens = 2000;
        private const int MaxTokens = 3000;
        private const int MaxResponseTokens = 1500;
        private const int MaxModelWindow = 16000;

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
        private async Task<string> GetMarkdownFromAi(string code, string userName, string? doc)
        {
            var docSkill = _kernel.ImportSemanticSkillFromDirectory(SkillsDirectoryPath, "CodeDocGenSkill");
            var ctx = _kernel.CreateNewContext();
            ctx["summary"] = doc;
            ctx["code"] = code;
            var result = await _kernel.RunAsync(ctx.Variables, docSkill["DocumentCode"]);
            var content = result.Result;
            Console.WriteLine(content);
            _docBuilder.AppendLine(content);
            _docBuilder.AppendLine();
            return content;
            //var mkDownDocSysPromptText = $"{doc}\n{MkDownDocSysPromptText}";
            //var messages = new List<Message> {Message.Create(mkDownDocSysPromptText, Role.System), Message.Create(code)};
            //var chatRequest = GetChatRequestModel(userName, messages);

            //var response = await _openAiService.ChatService.Create(chatRequest);
            //if (!response.Successful)
            //{
            //    return $"It looks like the request failed! Reason: {response.Error!.Message}";
            //}

            //var content = response.Choices[0].Message.Content;

        }

        //private static ChatRequestModel GetChatRequestModel(string userName, List<Message> messages)
        //{
        //    var chatRequest = new ChatRequestModel
        //    {
        //        MaxTokens = 500,
        //        Temperature = 0.7f,
        //        N = 1,
        //        Messages = messages,
        //        User = userName,
        //        Model = "gpt-3.5-turbo-16k",
        //        TopP = 0.5f
        //    };
        //    return chatRequest;
        //}
    }
}
