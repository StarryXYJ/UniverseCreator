using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Ursa.Controls;
using WorldCreator.Model.Time;

namespace WorldCreator.Control;

public partial class CustomTimePicker : UserControl
{
    public static readonly StyledProperty<CustomDateTime> DateTimeProperty =
        AvaloniaProperty.Register<CustomTimePicker, CustomDateTime>(
            nameof(Calendar));

    public CustomDateTime DateTime
    {
        get => GetValue(DateTimeProperty);
        set => SetValue(DateTimeProperty, value);
    }

    public static readonly StyledProperty<string> HeaderProperty = AvaloniaProperty.Register<CustomTimePicker, string>(
        nameof(Header));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }


    public CustomTimePicker()
    {
        InitializeComponent();
    }
}