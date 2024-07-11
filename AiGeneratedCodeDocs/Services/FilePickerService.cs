namespace AiGeneratedCodeDocs.Services;

public class FilePickerService
{
    public static string? ShowOpenFileDialog(int filterIndex = 2)
    {
        using OpenFileDialog openFileDialog = new();
        openFileDialog.Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|markdown files (*.md)|*.md|All files (*.*)|*.*";
        openFileDialog.FilterIndex = filterIndex;
        openFileDialog.RestoreDirectory = true;

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            return openFileDialog.FileName;
        }

        return null;
    }
    public static IEnumerable<string>? ShowOpenMultipleFileDialog(int filterIndex = 2)
    {
        using OpenFileDialog openFileDialog = new();
        openFileDialog.Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|markdown files (*.md)|*.md|All files (*.*)|*.*";
        openFileDialog.FilterIndex = filterIndex;
        openFileDialog.RestoreDirectory = true;
        openFileDialog.Multiselect = true;

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            return openFileDialog.FileNames;
        }

        return null;
    }
    public static string? ShowOpenFolderDialog()
    {
        using FolderBrowserDialog folderBrowserDialog = new();
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            return folderBrowserDialog.SelectedPath;
        }
        return null;
    }
}
