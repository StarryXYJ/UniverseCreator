using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace WorldCreator.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     菜单栏动画控制
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Collapse_OnClick(object? sender, RoutedEventArgs e)
    {
        //var (from, to) = Collapse.IsChecked ?? false ? (40, 200) : (200, 40);
        var (from, to) = Collapse.IsChecked ?? false ? (200.0, 40.0) : (40.0, 200.0);
        var animation = new Animation
        {
            FillMode = FillMode.Forward,
            Easing = new SineEaseInOut(),
            Duration = TimeSpan.FromSeconds(0.3)
        };
        KeyFrame fromKey = new() { Cue = new Cue(0.0), Setters = { new Setter(WidthProperty, from) } };
        KeyFrame toKey = new() { Cue = new Cue(1.0), Setters = { new Setter(WidthProperty, to) } };
        animation.Children.AddRange([fromKey, toKey]);
        await animation.RunAsync(SelectionList);
    }
}