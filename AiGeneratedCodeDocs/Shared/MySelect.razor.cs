using Microsoft.AspNetCore.Components;

namespace AiGeneratedCodeDocs.Shared
{
    public partial class MySelect<TItem>
    {
        
        [Parameter]
        public string? Label { get; set; }
        [Parameter]
        public IReadOnlyList<TItem>? OptionsList { get; set; }

        [Parameter]
        public TItem? SelectedValue { get; set; }

        [Parameter]
        public string DisplayPropertyName { get; set; } = "";
       
        [Parameter]
        public EventCallback<TItem> OnSelectItem { get; set; }
        [Parameter]
        public EventCallback<TItem> SelectedValueChanged { get; set; }

        private Dictionary<string, TItem> _optionPairs = new();
        
        protected override Task OnParametersSetAsync()
        {
            
            if (OptionsList == null) return base.OnParametersSetAsync();

            if (_optionPairs.Count != 0) return base.OnParametersSetAsync();
            if(SelectedValue == null)
                _optionPairs = OptionsList.ToDictionary(_ => Guid.NewGuid().ToString(), x => x);
            else
            {
                _optionPairs.Add(Guid.NewGuid().ToString(), SelectedValue);
                foreach (var option in OptionsList.Where(x => !x!.Equals(SelectedValue)))
                {
                    if (!option.Equals(SelectedValue))
                    {
                        _optionPairs.Add(Guid.NewGuid().ToString(), option);
                    }
                }
            }

            return base.OnParametersSetAsync();
        }

        private async void SelectionChanged(ChangeEventArgs args)
        {
            string id = args.Value?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(id)) return;
            SelectedValue = _optionPairs[id];
            await OnSelectItem.InvokeAsync(_optionPairs[id]);
            await SelectedValueChanged.InvokeAsync(_optionPairs[id]);
        }
    }
    public static class Extentions
    {
        public static string GetStringPropValue<TItem>(this TItem item, string propName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(propName))
                    return item.ToString();
                var property = typeof(TItem).GetProperty(propName);
                return property.GetValue(item)?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}\n{ex.StackTrace}");
                throw new Exception(
                    $"Property {propName} not found in type {nameof(item)}\n{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
