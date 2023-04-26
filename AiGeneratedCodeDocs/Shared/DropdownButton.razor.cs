using Microsoft.AspNetCore.Components;

namespace AiGeneratedCodeDocs.Shared
{
    public partial class DropdownButton<TItem>
    {
        [Parameter]
        public string? Text { get; set; }
        [Parameter]
        public IReadOnlyList<TItem> Items { get; set; } = new List<TItem>();
        [Parameter]
        public string? NameProperty { get; set; }
        
        [Parameter]
        public EventCallback<TItem> Click { get; set; }
        private Dictionary<string, TItem> _itemNames = new();
        private string? _text;
        private bool _isOpen;
        protected override Task OnInitializedAsync()
        {
            _text = Text;
            return base.OnInitializedAsync();
        }
        protected override Task OnParametersSetAsync()
        {
            
            if (Items == null) return base.OnParametersSetAsync();
            if (NameProperty == null)
                _itemNames = Items.ToDictionary(x => x?.ToString() ?? nameof(x), x => x);
            else
                _itemNames = Items.ToDictionary(x => x.GetStringPropValue(NameProperty));
            return base.OnParametersSetAsync();
        }
        private void HandleClick()
        {
            if (_text is null) return;
            try
            {
                Click.InvokeAsync(_itemNames[_text]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR on {nameof(DropdownButton<TItem>)}\r\n{ex}");
            }
            
        }
        private void HandleSelect(string text)
        {
            _text = text;
            _isOpen = false;
        }
    }
}