using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiGeneratedCodeDocs.Models;

public enum CodeLanguage
{
    [CodeFile(".cs")]
    [Description("C# code file")]
    CSharp,
    [CodeFile(".ts")]
    [CodeFile(".tsx")]
    [Description("Typescript code file (Angular/React)")]
    Typescript,
    [CodeFile(".html")]
    [Description("Html web file")]
    Html,
    [CodeFile(".css")]
    [CodeFile(".scss")]
    [CodeFile(".sass")]
    [CodeFile(".less")]
    [CodeFile(".styl")]
    [Description("CSS or CSS Preprocessor (ex. SASS)")]
    CssStyle,
    [CodeFile(".xml")]
    [CodeFile(".xaml")]
    [CodeFile(".hrbx")]
    [CodeFile(".aspx")]
    [Description("XML format markup file")]
    Xml,
    [CodeFile(".razor")]
    [CodeFile(".cshtml")]
    [Description("Razor Page or Razor Component file")]
    Razor,
    [CodeFile(".js")]
    [CodeFile(".jsx")]
    [Description("Standard javascript")]
    Javascript,
    [CodeFile(".yml")]
    [Description("YAML configuration file")]
    Yaml,
    [CodeFile(".json")]
    [Description("JSON format file")]
    Json,
}
