using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldCreator.ViewModels;

public abstract class ViewModelBase : ObservableRecipient
{
    public ViewModelBase()
    {
        IsActive = true;
    }

    public bool ShowSideBar { get; set; } = true;
}