## DocGenerator

This code file contains the implementation of the `DocGenerator` component. The `DocGenerator` component is responsible for generating Markdown documentation based on code snippets.

### RadzenRow

The `RadzenRow` component represents a row in a Radzen Blazor layout. It contains two `RadzenColumn` components, one with a size of 5 and the other with a size of 7. The columns are used to display the form and the generated documentation.

### RadzenColumn

The `RadzenColumn` component represents a column in a Radzen Blazor layout. It is used to define the layout of the form and the generated documentation.

### RadzenTemplateForm

The `RadzenTemplateForm` component is used to create a form with template-based input fields. It takes a `Submit` event handler and a `TItem` generic parameter. The form is bound to the `_repoForm` object and has two input fields: "Repo Path" and "Output Directory".

### RadzenFormField

The `RadzenFormField` component represents a form field in a Radzen Blazor form. It contains a label and a child content element. The label text is set to "Repo Path" or "Output Directory" depending on the field. The child content element is a `RadzenTextBox` component that is bound to the corresponding property in the `_repoForm` object.

### RadzenTextBox

The `RadzenTextBox` component represents a text input field in a Radzen Blazor form. It is used to input the repository path and the output directory.

### RadzenRequiredValidator

The `RadzenRequiredValidator` component is used to validate the input fields in the form. It displays an error message if the input is empty.

### RadzenCard

The `RadzenCard` component represents a card in a Radzen Blazor layout. It is used to display the generated documentation and the selected directory.

### RadzenTree

The `RadzenTree` component is used to display a tree structure of directories and files. It takes a `ValueChanged` event handler and a `Data` property that contains the directory structure. The tree is populated with the directories and files in the specified path.

### RadzenTreeLevel

The `RadzenTreeLevel` component represents a level in a Radzen Blazor tree. It takes a `Text` property that specifies the text to display for each node in the tree. It also takes a `Template` property that specifies the template to use for rendering each node.

### RadzenButton

The `RadzenButton` component represents a button in a Radzen Blazor form. It is used to submit the form and generate the documentation.

### RadzenText

The `RadzenText` component represents a text element in a Radzen Blazor layout. It is used to display the selected directory and the total input tokens.

### CreateDocumentService

The `CreateDocumentService` is a service used to generate Markdown documentation based on code snippets. It is injected into the `DocGenerator` component.

### ILogger

The `ILogger` interface is used for logging messages. It is injected into the `DocGenerator` component.

### Markdig

The `Markdig` library is used to convert Markdown to HTML. It is used to render the generated documentation in the `RadzenCard` component.

### Microsoft.AspNetCore.Components

The `Microsoft.AspNetCore.Components` namespace contains classes and interfaces for building Blazor components.

### Microsoft.Extensions.Logging

The `Microsoft.Extensions.Logging` namespace contains classes and interfaces for logging messages.

### Radzen.Blazor

The `Radzen.Blazor` namespace contains classes and components for building Blazor applications with Radzen UI components.

### System

The `System` namespace contains fundamental types and base types that define commonly-used value and reference data types, events and event handlers, interfaces, attributes, and processing exceptions.

### System.Collections.Generic

The `System.Collections.Generic` namespace contains interfaces and classes that define generic collections, which allow users to create strongly typed collections that provide better type safety and performance than non-generic strongly typed collections.

### System.Linq

The `System.Linq` namespace provides classes and interfaces that support queries that use Language-Integrated Query (LINQ).

### System.Text

The `System.Text` namespace contains classes for encoding and decoding strings, converting characters, and performing other text-related operations.

### AiGeneratedCodeDocs.Models

The `AiGeneratedCodeDocs.Models` namespace contains classes that define the models used in the code documentation generation process.

### AiGeneratedCodeDocs.Services

The `AiGeneratedCodeDocs.Services` namespace contains classes that provide services for generating code documentation.

### Helpers

The `Helpers` class contains helper methods for working with code snippets. It includes a `GetTokens` method that calculates the number of tokens in a code snippet.

### OnInitializedAsync

The `OnInitializedAsync` method is an override method that is called when the component is initialized. It sets the `entries` property to the result of the `GetRepos` method.

### GetRepos

The `GetRepos` method retrieves the list of repositories from the specified repository path. It filters out certain directories and returns a list of `DirInfo` objects representing the repositories.

### DirInfo

The `DirInfo` class represents a directory in the file system. It has properties for the directory name, directory full path, and subdirectories.

### Submit

The `Submit` method is an event handler that is called when the form is submitted. It generates the code documentation for the selected repository directory.

### GenerateCodeDoc

The `GenerateCodeDoc` method generates the code documentation for the specified directory. It retrieves the files in the directory, reads their contents, and generates the code documentation using the `CreateDocumentService` service. The generated documentation is appended to the `_markdown` variable and saved to a file in the output directory.

### ReadFilesInDirectory

The `ReadFilesInDirectory` method reads the files in the specified directory and returns a dictionary of file names and their contents.

### ToMarkdownHtml

The `ToMarkdownHtml` method converts Markdown to HTML using the `Markdig` library.

### LoadFiles

The `LoadFiles` method is an event handler that is called when a directory node in the tree is expanded. It populates the children of the expanded node with the directories and files in the directory.

### GetTextForNode

The `GetTextForNode` method returns the text to display for each node in the tree.

### OnSelected

The `OnSelected` method is an event handler that is called when a node in the tree is selected. It updates the selected directory and the form's repository directory and output directory properties.

### FileOrFolderTemplate

The `FileOrFolderTemplate` is a render fragment that defines the template for rendering each node in the tree. It displays an icon and the text for each node.

This is the documentation for the `DocGenerator` component and its associated classes and methods.
