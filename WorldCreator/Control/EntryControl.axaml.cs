using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using WorldCreator.Model;

namespace WorldCreator.Control;

/// <summary>
/// 显示模式
/// </summary>
public enum DisplayMode
{
    EditorOnly,
    PreviewOnly,

    /// <summary>
    /// 风格模式，都显示
    /// </summary>
    SplitView
}

/// <summary>
/// 分隔的分布方向
/// </summary>
public enum SplitLayoutDirection
{
    /// <summary>
    /// 左右分布
    /// </summary>
    Horizontal,

    /// <summary>
    /// 上下分布
    /// </summary>
    Vertical
}

public class ButtonItem : AvaloniaObject
{
    public static readonly StyledProperty<Geometry?> IconProperty =
        AvaloniaProperty.Register<ButtonItem, Geometry?>(nameof(Icon));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ButtonItem, ICommand?>(nameof(Command));

    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}

public partial class EntryControl : UserControl
{
    public EntryControl()
    {
        InitializeComponent();
        EntryProperty.Changed.AddClassHandler<EntryControl>(NotifyUIVisibility);
        CurrentDisplayModeProperty.Changed.AddClassHandler<EntryControl>(NotifyUIVisibility);
        CurrentSplitLayoutDirectionProperty.Changed.AddClassHandler<EntryControl>(NotifyUIVisibility);
        SwapPanelsProperty.Changed.AddClassHandler<EntryControl>(NotifyUIVisibility);
    }

    /// <summary>
    /// 在点击布局按钮时通知UI是否显示属性的变更
    /// </summary>
    /// <param name="x"></param>
    /// <param name="e"></param>
    private void NotifyUIVisibility(EntryControl x, AvaloniaPropertyChangedEventArgs e)
    {
        x.RaisePropertyChanged(IsEditorVisibleProperty, x.IsEditorVisible, x.IsEditorVisible);
        x.RaisePropertyChanged(IsPreviewVisibleProperty, x.IsPreviewVisible, x.IsPreviewVisible);
        x.RaisePropertyChanged(IsHorizontalVisibleProperty, x.IsHorizontalVisible, x.IsHorizontalVisible);
        x.RaisePropertyChanged(IsVerticalVisibleProperty, x.IsVerticalVisible, x.IsVerticalVisible);
    }

    /// <summary>
    /// 当前显示的词条
    /// </summary>
    public static readonly StyledProperty<Entry> EntryProperty =
        AvaloniaProperty.Register<EntryControl, Entry>(nameof(Entry));

    public static readonly StyledProperty<DisplayMode> CurrentDisplayModeProperty =
        AvaloniaProperty.Register<EntryControl, DisplayMode>(nameof(CurrentDisplayMode));

    public static readonly StyledProperty<SplitLayoutDirection> CurrentSplitLayoutDirectionProperty =
        AvaloniaProperty.Register<EntryControl, SplitLayoutDirection>(nameof(CurrentSplitLayoutDirection));

    public static readonly StyledProperty<bool> SwapPanelsProperty =
        AvaloniaProperty.Register<EntryControl, bool>(nameof(SwapPanels));

    public static readonly DirectProperty<EntryControl, bool> IsEditorVisibleProperty =
        AvaloniaProperty.RegisterDirect<EntryControl, bool>(
            nameof(IsEditorVisible),
            o => o.IsEditorVisible);

    public static readonly DirectProperty<EntryControl, bool> IsPreviewVisibleProperty =
        AvaloniaProperty.RegisterDirect<EntryControl, bool>(
            nameof(IsPreviewVisible),
            o => o.IsPreviewVisible);

    public static readonly DirectProperty<EntryControl, bool> IsHorizontalVisibleProperty =
        AvaloniaProperty.RegisterDirect<EntryControl, bool>(
            nameof(IsHorizontalVisible),
            o => o.IsHorizontalVisible);

    public static readonly DirectProperty<EntryControl, bool> IsVerticalVisibleProperty =
        AvaloniaProperty.RegisterDirect<EntryControl, bool>(
            nameof(IsVerticalVisible),
            o => o.IsVerticalVisible);

    /// <summary>
    /// 当前显示的词条
    /// </summary>
    public Entry Entry
    {
        get => GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }


    public DisplayMode CurrentDisplayMode
    {
        get => GetValue(CurrentDisplayModeProperty);
        set => SetValue(CurrentDisplayModeProperty, value);
    }


    public SplitLayoutDirection CurrentSplitLayoutDirection
    {
        get => GetValue(CurrentSplitLayoutDirectionProperty);
        set => SetValue(CurrentSplitLayoutDirectionProperty, value);
    }


    public bool SwapPanels
    {
        get => GetValue(SwapPanelsProperty);
        set => SetValue(SwapPanelsProperty, value);
    }

    public bool IsEditorVisible => CurrentDisplayMode == DisplayMode.EditorOnly;
    public bool IsPreviewVisible => CurrentDisplayMode == DisplayMode.PreviewOnly;

    public bool IsHorizontalVisible => CurrentDisplayMode == DisplayMode.SplitView &&
                                       CurrentSplitLayoutDirection == SplitLayoutDirection.Horizontal;

    public bool IsVerticalVisible => CurrentDisplayMode == DisplayMode.SplitView &&
                                     CurrentSplitLayoutDirection == SplitLayoutDirection.Vertical;

    private void ToggleSplitLayoutDirection(object? sender, RoutedEventArgs e)
    {
        CurrentSplitLayoutDirection = CurrentSplitLayoutDirection == SplitLayoutDirection.Horizontal
            ? SplitLayoutDirection.Vertical
            : SplitLayoutDirection.Horizontal;
    }

    private void ToggleSwapPanels(object? sender, RoutedEventArgs e)
    {
        SwapPanels = !SwapPanels;
    }

    [RelayCommand]
    private void SwitchModeEdit()
    {
        CurrentDisplayMode = DisplayMode.EditorOnly;
    }

    [RelayCommand]
    private void SwitchModePreview()
    {
        CurrentDisplayMode = DisplayMode.PreviewOnly;
    }

    [RelayCommand]
    private void SwitchModeSplit()
    {
        CurrentDisplayMode = DisplayMode.SplitView;
    }
}