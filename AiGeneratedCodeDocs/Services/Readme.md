## CreateDocumentService

This class is responsible for generating Markdown documentation based on code snippets. It contains methods for generating Markdown documentation and splitting code into sections.

### Constructor

The constructor initializes the `CreateDocumentService` class. It takes a logger and configuration as parameters. It initializes the `_docBuilder` variable with the string "Previous Files:\n". It also initializes the `_logger` and `_configuration` variables with the provided logger and configuration, respectively. Finally, it builds the `_kernel` using the `Kernel.Builder` class, passing the logger, OpenAI chat completion service parameters, and configuration.

### GenerateMarkdownDocs

This method generates Markdown documentation based on the provided code snippet. It takes the code and an optional username as parameters. It first checks if the code requires a document summary by calculating the total number of tokens in the existing documentation, system prompt, input code, and maximum response tokens. If the total exceeds the maximum model window, it generates a summary using the `Summarize` method and updates the `_docBuilder` with the generated summary.

Next, it checks if the code requires code truncation by calculating the total number of tokens in the existing documentation, input code, system prompt, and maximum response tokens. If the total exceeds the maximum model window, it splits the code into sections using the `SplitCodeToSections` method and generates Markdown documentation for each section using the `GetMarkdownFromAi` method. The generated content for each section is then combined and returned.

If the code does not require code truncation, it generates Markdown documentation for the entire code using the `GetMarkdownFromAi` method and returns the result.

### Summarize

This method generates a summary of the provided document. It takes a username and document as parameters. It imports the semantic skill from the specified directory and creates a new context. It then runs the skill's "Summarize" method using the `_kernel.RunAsync` method and retrieves the result. The result is returned as the generated summary.

### SplitCodeToSections

This method splits the provided code into sections based on property or method definitions. It takes the code and an optional minimum number of tokens as parameters. It initializes an empty list of sections. If the code is empty or null, it returns the empty list.

If a minimum number of tokens is not specified, it defaults to 2000. It splits the code into lines and iterates over each line. If a line represents a property or method definition (starts with "public", "private", or "protected"), it checks if the current section has enough tokens to meet the minimum requirement. If it does, it adds the current section to the list and resets the current section. Otherwise, it continues adding the line to the current section and updates the current token count.

After iterating over all lines, it adds the current section to the list if it is not empty. Finally, it returns the list of sections.

### IsPropertyOrMethodDefinition

This method checks if the provided line represents a property or method definition. It takes a line as a parameter. It trims the line and checks if it starts with "public", "private", or "protected". If it does, it returns true; otherwise, it returns false.

### GetMarkdownFromAi

This method generates Markdown documentation for the provided code using the AI model. It takes the code, username, and optional document as parameters. It imports the semantic skill from the specified directory and creates a new context. It sets the "summary" and "code" variables in the context to the provided document and code, respectively. It then runs the skill's "DocumentCode" method using the `_kernel.RunAsync` method and retrieves the result. The result is appended to the `_docBuilder` and returned as the generated Markdown documentation.
