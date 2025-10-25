using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using WorldCreator.Model;

namespace WorldCreator.Control;

public partial class EditControl : UserControl
{
    public static readonly StyledProperty<Entry> EntryProperty =
        AvaloniaProperty.Register<MdViewer, Entry>(nameof(Entry));

    public static readonly StyledProperty<bool> IsMarkdownProperty = AvaloniaProperty.Register<EditControl, bool>(
        nameof(IsMarkdown), false);

    public bool IsMarkdown
    {
        get => GetValue(IsMarkdownProperty);
        set => SetValue(IsMarkdownProperty, value);
    }

    public EditControl()
    {
        InitializeComponent();
        TextInputOptions.SetMultiline(MdTextBox, true);
        TextInputOptions.SetReturnKeyType(MdTextBox, TextInputReturnKeyType.Return);
    }

    public Entry Entry
    {
        get => GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }

    private void ChangeView(object? sender, RoutedEventArgs e)
    {
        IsMarkdown = !IsMarkdown;
    }
}