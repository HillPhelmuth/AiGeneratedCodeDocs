namespace AiGeneratedCodeDocs.Models;

public record AppSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? AppPath { get; set; }
    public string? TechnicalDescription { get; set; }
    public string? DomainDescription { get; set; }
    public string? AppFramework { get; set; }
    public List<CodeLanguage> Languages { get; set; } = new();
   
}
