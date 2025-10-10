using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Fizzler;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WorldCreator.Core.Services;


namespace WorldCreator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public NavigationService NavigationService { get; init; }

    [ActivatorUtilitiesConstructor]
    public MainViewModel(NavigationService navigationService)
    {
        NavigationService = navigationService;
        
    }
    

    public MainViewModel()
    {
        
    }

}