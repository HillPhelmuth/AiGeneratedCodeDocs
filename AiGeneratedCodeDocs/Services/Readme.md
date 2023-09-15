## CreateDocumentService

This file contains the implementation of the `CreateDocumentService` class. This class is responsible for generating Markdown documents from code using Azure OpenAI with Semantic Kernel.

### Dependencies

- `System.Text`
- `AiGeneratedCodeDocs.Models`
- `Microsoft.Extensions.Logging`
- `Microsoft.Extensions.Configuration`
- `Microsoft.SemanticKernel`
- `System.Reflection`

### Constructor

The `CreateDocumentService` class has a constructor that takes in a logger and a configuration object. It initializes the `_docBuilder` variable as a `StringBuilder` and logs an information message. It also retrieves the deployment name, API key, and endpoint from the configuration object. If any of these values are missing, an `ArgumentException` is thrown. Finally, it builds the `IKernel` object using the provided logger and Azure chat completion service.

### Properties

- `SkillsDirectoryPath`: A static property that returns the path to the directory containing the skills.

### Methods

- `GenerateMarkdownDocs`: This method takes in a code string and generates Markdown documentation from it. It first checks if the document summary is required based on the length of the existing document and the code. If so, it summarizes the document using the `Summarize` method. Then, it checks if code truncation is required based on the length of the document, code, and system prompt. If so, it splits the code into sections and generates Markdown for each section using the `GetMarkdownFromAi` method. Finally, it generates Markdown for the code using the `GetMarkdownFromAi` method.

- `Summarize`: This method takes in a document string and summarizes it using the Semantic Kernel. It imports the `CodeDocGenSkill` semantic skill from the skills directory and runs the `Summarize` skill on the document. The result is returned as a string.

- `SplitCodeToSections`: This method takes in a code string and splits it into sections based on property or method definitions. It returns a list of code sections.

- `IsPropertyOrMethodDefinition`: This method takes in a line of code and checks if it is a property or method definition. It returns a boolean value indicating the result.

- `GetMarkdownFromAi`: This method takes in a code string and a document string and generates Markdown from them using the Semantic Kernel. It imports the `CodeDocGenSkill` semantic skill from the skills directory, creates a new context, sets the `summary` and `code` variables in the context, and runs the `DocumentCode` skill on the context. The result is returned as a string.

Please note that the code snippets in this documentation are not executable and are only meant for demonstration purposes.
