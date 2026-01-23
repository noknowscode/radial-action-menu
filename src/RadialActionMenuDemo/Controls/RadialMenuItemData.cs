using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

namespace RadialActionMenuDemo.Controls;

public class RadialMenuItemData
{
    public IconElement? Icon { get; set; }
    public string? Glyph { get; set; }
    public string Label { get; set; } = string.Empty;
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public bool IsEnabled { get; set; } = true;
}
