using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;


namespace WorldCreator.ViewModels;

public abstract class ViewModelBase : ObservableRecipient
{
    public ViewModelBase()
    {
        IsActive = true;
    }
}