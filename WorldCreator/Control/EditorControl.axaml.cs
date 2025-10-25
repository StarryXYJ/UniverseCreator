using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using WorldCreator.Model;

namespace WorldCreator.Control;

public partial class EditorControl : UserControl
{
    public ObservableCollection<Entry> Entries =>
    [
        new Entry("him", "wtf", 3, null),
        new Entry("h", "wtf", 3, null),
        new Entry("him1", "wtf", 3, null),
        new Entry("him2hhhhhhhhhhhhhhhhh", "wtaaaaaaaaaaaaaaaaaaaaaaaaf", 3, null),
        new Entry("him3mmmmmmmmmmmmmmmmm", "wtaaaaaaaaf", 3, null),
    ];

    public EditorControl()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true; // 阻止冒泡
    }
    
}