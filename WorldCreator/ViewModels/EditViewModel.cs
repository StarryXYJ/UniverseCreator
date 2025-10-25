using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorldCreator.Model;

namespace WorldCreator.ViewModels;

public partial class EditViewModel : ViewModelBase
{
    [ObservableProperty]
    private Entry _currentEntry = new Entry("New Entry", "Description", 1, null);
}