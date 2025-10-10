using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using WorldCreator.ViewModels;

namespace WorldCreator;

public class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Avalonia.Controls.Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            //return (Control)Activator.CreateInstance(type)!;
            var view = (Avalonia.Controls.Control)_serviceProvider.GetService(type);

            // 如果容器中没有注册，则回退到直接创建
            if (view == null) view = (Avalonia.Controls.Control)Activator.CreateInstance(type)!;

            view.DataContext = param;
            return view;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}