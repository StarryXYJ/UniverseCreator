using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldCreator.ViewModels;

public abstract class ViewModelBase : ObservableRecipient
{
    public ViewModelBase()
    {
    }

    public bool ShowSideBar { get; set; } = true;
}