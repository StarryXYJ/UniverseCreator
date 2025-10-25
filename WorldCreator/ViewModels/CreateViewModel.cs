using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Ursa.Controls;
using WorldCreator.Core;
using WorldCreator.Core.Services;
using WorldCreator.Lang;
using WorldCreator.Model;

namespace WorldCreator.ViewModels;

public partial class CreateViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;

    private readonly IRepositoryService _repositoryService;

    [ObservableProperty] private WorldRepository _repository = new();

    public CreateViewModel()
    {
        ShowSideBar = false;
    }

    [ActivatorUtilitiesConstructor]
    public CreateViewModel(NavigationService navigationService, IRepositoryService repositoryService)
    {
        ShowSideBar = false;
        _navigationService = navigationService;
        _repositoryService = repositoryService;
    }

    [RelayCommand]
    public void Cancel()
    {
        _navigationService.GoBack();
    }

    [RelayCommand]
    public async void Create()
    {
        if (Directory.Exists(Common.NativeRepoPath(Repository.Name)))
        {
            await MessageBox.ShowAsync(Resources.CreateError);
        }
        else
        {
            Repository.Save();
            _navigationService.GoBack();
            _repositoryService.Load();
        }
    }
}