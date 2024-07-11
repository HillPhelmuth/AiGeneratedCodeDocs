using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Tiktoken;

namespace AiGeneratedCodeDocs.Models
{
    public static class Helpers
    {
        private static Encoding? _encoding;
        public static int GetTokens(string text)
        {
            _encoding ??= Encoding.ForModel("gpt-3.5-turbo");
            var tokens = _encoding.CountTokens(text);
            return tokens;
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
