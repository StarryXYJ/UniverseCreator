using System;
using System.Collections.Generic;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WorldCreator.Core.Services;
using WorldCreator.Model;

namespace WorldCreator.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;

    [Obsolete("只用于xaml绘制")]
    public StartViewModel()
    {
        ShowSideBar = false;
    }

    [ActivatorUtilitiesConstructor]
    public StartViewModel(NavigationService navigationService, IRepositoryService repositoryService)
    {
        ShowSideBar = false;
        _navigationService = navigationService;
        RepositoryService = repositoryService;
    }

    public IRepositoryService RepositoryService { get; }


    [RelayCommand]
    public void OpenRepository(WorldRepository repository)
    {
        RepositoryService.OpenRepository(repository);
    }


    [RelayCommand]
    public void New()
    {
        _navigationService.NavigateTo<CreateViewModel>();
    }

    [RelayCommand]
    public async void Import()
    {
        var storage = App.StorageProvider;
        var options = new FilePickerOpenOptions
        {
            Title = "打开 JSON 文件",
            AllowMultiple = false, // 是否允许多选
            SuggestedStartLocation = await storage.TryGetWellKnownFolderAsync(WellKnownFolder.Documents), // 建议的起始目录
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("世界文件") { Patterns = new[] { "*.UniverseProj" } }
                //new FilePickerFileType("所有文件") { Patterns = new[] { "*" } }
            }
        };

        var result = await storage.OpenFilePickerAsync(options);
        if (result.Count > 0)
        {
            var file = result[0]; // 获取第一个文件

            // 获取文件路径
            var filePath = file.Path.LocalPath;

            // 读取文件内容
            RepositoryService.Import(filePath);
        }
    }
}