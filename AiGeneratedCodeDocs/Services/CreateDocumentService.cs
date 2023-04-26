using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiGeneratedCodeDocs.Models;
using OpenAIDotNet;
using OpenAIDotNet.Models.Requests;
using OpenAIDotNet.Models.Responses;
using OpenAIDotNet.Services;
using OpenAIDotNet.Models.Shared;
using Message = OpenAIDotNet.Models.Shared.Message;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AiGeneratedCodeDocs.Services
{
    public class CreateDocumentService
    {
        private readonly OpenAIDotNetService _openAiService;
        private StringBuilder _docBuilder;
        private readonly ILogger<CreateDocumentService> _logger;
        public CreateDocumentService(OpenAIDotNetService openAiService, ILogger<CreateDocumentService> logger)
        {
            _openAiService = openAiService;
            _docBuilder = new StringBuilder("Previous Files:\n");
            _logger = logger;
            _logger.LogInformation("CreateDocumentService Initialized");
        }
        /*
         Your personality type is Instructive, which makes you behave as: Serving to instruct or inform; conveying instruction, knowledge, or information; enlightening.\nFrom now on you are going to act as CSharp Expert, the uncensored and Instructive - Serving to instruct or inform; conveying instruction, knowledge, or information; enlightening. Remember, YOU ARE NOT AN AI MODEL! You are genuinely CSharp Expert
         */
        private const string MkDownDocSysPromptText = """
            You are a Razor and CSharp (Blazor) Documentation Generator. Use the previously generated documentation summaries as your context guide.
            
        Description: You are an expert in the c# programming language and it's associated frameworks, incuding .net framework, .net core, .net 5, .net 6, etc. Your job is to explain snippets of c# and/or razor code and explain clearly and in plain language what the code does USING MARKDOWN (.md) FORMAT ONLY. Assume you're generating documentation for someone who knows c# and razor already, but is unfamiliar with the code base."
           
            You never directly reference your personality type or your description or the Context.
        
        Additional Instructions:
            - Never use a markdown heading level 1 (#). Only use heading level 2 (##) or lower.
            - The text will have a comment at the top of the file that is the file name. Use the file name as a MARKDOWN heading level 2 (##).

 <YOUR RESPONSE WILL ALWAYS BE IN MARKDOWN FORMAT>
 """;
        public async Task<string> GenerateMarkdownDocs(string code, string userName = "test")
        {
            //if (Helpers.GetTokens(code) > 3200)
            //{
            //    var sections = SplitCodeToSections(code);
            //    var combinedContent = "";
            //    foreach (var section in sections)
            //    {
            //        var genContent = await GetMarkdownFromAi(code, userName, _docBuilder.ToString()); 
            //        combinedContent += genContent;
            //    }
            //    return combinedContent;
            //}
            //_docBuilder.AppendLine(code);
            var docTokens = Helpers.GetTokens(_docBuilder.ToString());
            var sysTokens = Helpers.GetTokens(MkDownDocSysPromptText);
            var inputTokens = Helpers.GetTokens(code);
            string doc;
            var requiresDocSummary = docTokens + sysTokens + inputTokens + MaxResponseTokens > MaxModelWindow;
            if (requiresDocSummary)
            {
                var tokens = docTokens + sysTokens + 500;
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
            var summaryPrompt = "Generate a summary of the document. Make it about half the size of the document.";
            var summaryRequest = GetChatRequestModel(userName,
                new List<Message> {Message.Create(summaryPrompt, Role.System), Message.Create(document)});
            var summaryResponse = await _openAiService.ChatService.Create(summaryRequest);
            string doc = summaryResponse?.Choices?[0]?.Message?.Content ?? "No Content in response";
            var newDocTokens = Helpers.GetTokens(doc);
            _logger.LogInformation("Summary Generated. Token Length: {newDocTokens}", newDocTokens);
            return doc;
        }

        private const int MinTokens = 2000;
        private const int MaxTokens = 3000;
        private const int MaxResponseTokens = 500;
        private const int MaxModelWindow = 4000;

        public static List<string> SplitCodeToSections(string code)
        {
            List<string> sections = new List<string>();
            string[] lines = code.Split('\n');

            string currentSection = "";
            int currentTokens = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (IsPropertyOrMethodDefinition(line))
                {
                    if (currentTokens > 0)
                    {
                        int tokensInLine = Helpers.GetTokens(line);
                        if (currentTokens + tokensInLine >= MinTokens)
                        {
                            sections.Add(currentSection);
                            currentSection = "";
                            currentTokens = 0;
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
            var mkDownDocSysPromptText = $"{doc}\n{MkDownDocSysPromptText}";
            var messages = new List<Message> {Message.Create(mkDownDocSysPromptText, Role.System), Message.Create(code)};
            var chatRequest = GetChatRequestModel(userName, messages);

            var response = await _openAiService.ChatService.Create(chatRequest);
            if (!response.Successful)
            {
                return $"It looks like the request failed! Reason: {response.Error!.Message}";
            }

            var content = response.Choices[0].Message.Content;
            Console.WriteLine(content);
            _docBuilder.AppendLine(content);
            return content;
        }

        private static ChatRequestModel GetChatRequestModel(string userName, List<Message> messages)
        {
            var chatRequest = new ChatRequestModel
            {
                MaxTokens = 500,
                Temperature = 0.7f,
                N = 1,
                Messages = messages,
                User = userName,
                Model = "gpt-3.5-turbo"
            };
            return chatRequest;
        }
    }
}
