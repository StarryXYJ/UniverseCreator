using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using WorldCreator.Core.Services;
using WorldCreator.Model;

namespace WorldCreator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsLeftVisible), nameof(IsBottomVisible))]
    private Rect _controlSize;

    /// <summary>
    /// 项目内有侧边栏的时候
    /// 侧边栏的索引
    /// </summary>
    [ObservableProperty] private int _selectedIndex;

    [ObservableProperty] private MenuItemModel? _selectedMenuItem;

    [ActivatorUtilitiesConstructor]
    public MainViewModel(NavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.SetDefaultAction(() =>
        {
            OnPropertyChanged(nameof(IsLeftVisible));
            OnPropertyChanged(nameof(IsBottomVisible));
        });
    }


    public MainViewModel()
    {
    }

    public NavigationService NavigationService { get; init; }

    // 计算属性：宽高比是否大于1
    public bool IsWidthGreaterThanHeight => ControlSize.Width > ControlSize.Height;

    public bool IsLeftVisible =>
        IsWidthGreaterThanHeight && NavigationService.CurrentViewModel is { ShowSideBar: true };

    public bool IsBottomVisible =>
        !IsWidthGreaterThanHeight && NavigationService.CurrentViewModel is { ShowSideBar: true };
}