using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WorldCreator.Core.Data;
using WorldCreator.Model;

namespace WorldCreator.ViewModels;

public partial class EditViewModel : ViewModelBase
{
    [ObservableProperty] private Entry _currentEntry = new Entry("New Entry", "Description", 1, null);
    private readonly EntryDataContext _dataContext;

    public EditViewModel(EntryDataContext entryDataContext)
    {
        entryDataContext.Database.EnsureCreated();
        _dataContext = entryDataContext;
    }
}