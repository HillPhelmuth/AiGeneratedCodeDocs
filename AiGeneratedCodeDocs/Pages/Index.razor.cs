using System.IO;
using System.Text;
using AiGeneratedCodeDocs.Models;
using AiGeneratedCodeDocs.Services;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace AiGeneratedCodeDocs.Pages
{
    public partial class Index
    {
        private bool _showPage = true;
        private async Task Reset()
        {
            _showPage = false;
            await Task.Delay(1000);
            StateHasChanged();
            _showPage = true;
            StateHasChanged();
        }
    }
}
