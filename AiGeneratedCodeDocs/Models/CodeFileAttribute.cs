using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiGeneratedCodeDocs.Models;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class CodeFileAttribute : Attribute
{
    public CodeFileAttribute(string extension)
    {
        Extension = extension;
    }
    public string Extension { get; set; }
}
