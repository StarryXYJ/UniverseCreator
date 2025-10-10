using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using WorldCreator.Model;

namespace WorldCreator.ViewModels;

public partial class EditViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsWidthGreaterThanHeight))]
    private Rect _controlSize;


    [ObservableProperty] private string? _markDownText;
    [ObservableProperty] private int _selectedIndex;
    [ObservableProperty] private MenuItemModel? _selectedMenuItem;

    // 计算属性：宽高比是否大于1
    public bool IsWidthGreaterThanHeight => ControlSize.Width > ControlSize.Height;
}