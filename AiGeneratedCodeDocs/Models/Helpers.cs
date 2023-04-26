using AI.Dev.OpenAI.GPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiGeneratedCodeDocs.Models
{
    public class Helpers
    {
        public static int GetTokens(string text)
        {
            var tokens = GPT3Tokenizer.Encode(text);
            return tokens.Count;
        }
    }
}
