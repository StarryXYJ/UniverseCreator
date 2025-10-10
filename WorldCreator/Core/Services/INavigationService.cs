using System;
using WorldCreator.ViewModels;

namespace WorldCreator.Core.Services;

public interface INavigationService
{  
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void NavigateTo<TViewModel>(Action<TViewModel> initAction) where TViewModel : ViewModelBase;
    void GoBack();
}