using AI.Dev.OpenAI.GPT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiGeneratedCodeDocs.Models
{
    public static class Helpers
    {
        public static int GetTokens(string text)
        {
            var tokens = GPT3Tokenizer.Encode(text);
            return tokens.Count;
        }
        public static string ToEnumDescription<T>(this T value) where T : Enum
        {
            var fi = value.GetType().GetField(value.ToString());


            if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}
