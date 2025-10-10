using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldCreator.ViewModels;

namespace WorldCreator.Core.Services;

/// <summary>
/// 导航服务
/// </summary>
public partial class NavigationService : ObservableObject,INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<ViewModelBase> _navigationStack = new();
    
    // 绑定到这个属性以显示当前视图
    [ObservableProperty]
    private ViewModelBase? _currentViewModel;
    
    
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        NavigateTo<TViewModel>(_ => { });
    }
    
    public void NavigateTo<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModelBase
    {
        // 获取视图模型实例
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        
        // 初始化视图模型
        initAction(viewModel);
        
        // 当前视图模型入栈（用于回退）
        if (CurrentViewModel != null)
        {
            _navigationStack.Push(CurrentViewModel);
        }
        
        // 设置当前视图模型
        CurrentViewModel = viewModel;
    }
    
    public void GoBack()
    {
        if (_navigationStack.Count > 0)
        {
            CurrentViewModel = _navigationStack.Pop();
        }
    }
}