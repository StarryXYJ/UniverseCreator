using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using WorldCreator.Core.Data;
using WorldCreator.Core.Services;
using WorldCreator.ViewModels;
using WorldCreator.Views;

namespace WorldCreator;

public class App : Application
{
    public static IStorageProvider StorageProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    ///     创建Host
    /// </summary>
    /// <returns></returns>
    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            //注入日志
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
                    //.WriteTo.Console()
                    .CreateLogger();
                logging.Services.AddSingleton(Log.Logger);
            })
            .ConfigureAppConfiguration((context, config) => { config.AddJsonFile("appsettings.json", true, false); })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ViewLocator>();
                services.AddSingleton<NavigationService>();
                services.AddSingleton<MainViewModel>();

                services.AddSingleton<IRepositoryService, RepositoryService>();
                services.AddDbContext<EntryDataContext>((provider, options) =>
                {
                    var repo = provider.GetService<IRepositoryService>()!.CurrentRepository;
                    options.UseSqlite(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");
                });
                services.AddTransient<StartViewModel>();
                services.AddTransient<EditViewModel>();
                services.AddTransient<CreateViewModel>();
            });
    }

    /// <summary>
    ///     初始化
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        //本地化
        //Lang.Resources.Culture = new("zh-hans");
        //注册Hosting
        var host = CreateHostBuilder().Build();
        host.Start();
        if (Current != null)
        {
            var viewLocator = host.Services.GetRequiredService<ViewLocator>();
            Current.DataTemplates.Add(viewLocator);
        }


        //主页面初始化
        host.Services.GetRequiredService<IRepositoryService>().Load();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var mainWindow = new MainWindow
            {
                DataContext = host.Services.GetRequiredService<MainViewModel>()
            };
            desktop.MainWindow = mainWindow;
            desktop.Exit += async (s, e) =>
            {
                await host.StopAsync();
                host.Dispose();
            };
            StorageProvider = TopLevel.GetTopLevel(mainWindow)!.StorageProvider;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainView = new MainView
            {
                DataContext = host.Services.GetRequiredService<MainViewModel>()
            };
            singleViewPlatform.MainView = mainView;
            StorageProvider = TopLevel.GetTopLevel(mainView)!.StorageProvider;
        }

        //设定初始界面
        var navigationService = host.Services.GetRequiredService<NavigationService>();
        navigationService.NavigateTo<StartViewModel>();
        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}