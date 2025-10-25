using Avalonia;
using Avalonia.Controls;
using WorldCreator.Model;

namespace WorldCreator.Control;

public partial class MdViewer : UserControl
{
    public static readonly StyledProperty<Entry> EntryProperty =
        AvaloniaProperty.Register<MdViewer, Entry>(nameof(Entry));

    public MdViewer()
    {
        InitializeComponent();
    }

    public Entry Entry
    {
        get => GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }
}