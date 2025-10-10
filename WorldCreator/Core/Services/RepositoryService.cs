using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WorldCreator.Model;
using WorldCreator.ViewModels;

namespace WorldCreator.Core.Services;

/// <summary>
///     获取所有本地仓库
///     并保存当前打开的仓库
/// </summary>
public class RepositoryService : ObservableObject, IRepositoryService
{
    private readonly NavigationService? _navigationService;

    /// <summary>
    ///     仓库列表
    /// </summary>
    private SortableObservableCollection<WorldRepository> _repositories = new();

    public RepositoryService(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    /// <summary>
    ///     当前打开的仓库
    /// </summary>
    public WorldRepository CurrentRepository { get; private set; } = null!;

    /// <summary>
    ///     仓库列表
    /// </summary>
    public SortableObservableCollection<WorldRepository> Repositories
    {
        get => _repositories;
        set => SetProperty(ref _repositories, value);
    }

    /// <summary>
    ///     加载所有仓库
    /// </summary>
    public void Load()
    {
        _repositories.Clear();
        var uri = Common.NativeReposPath();
        if (!Directory.Exists(uri))
        {
            Directory.CreateDirectory(uri);
            return;
        }

        var list = Directory.GetDirectories(uri);
        foreach (var v in list)
        {
            Log.Information("Load" + v);
            LoadSingleRepo(v);
        }

        Log.Information("加载完毕，共" + list.Length + "个仓库");
    }

    /// <summary>
    ///     打开仓库作为项目
    /// </summary>
    /// <param name="repository"></param>
    public void OpenRepository(WorldRepository repository)
    {
        CurrentRepository = repository;
        _navigationService.NavigateTo<EditViewModel>();
        //await MessageBox.ShowAsync($"正在打开{repository.Name}");
    }

    /// <summary>
    ///     导入外部仓库
    /// </summary>
    /// <param name="uri">压缩包位置</param>
    public void Import(string uri)
    {
        try
        {
            ZipFile.ExtractToDirectory(uri, Common.NativeReposPath());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }


        // var path = Environment.CurrentDirectory + @"/repos" + uri.Split('/')[^1];

        // try
        // {
        //    LoadSingleRepo(path);
        // }
        // catch (Exception e)
        // {
        //     Log.Error("加载失败"+"  "+e.Message);
        //     File.Delete(path);
        // }
        Load();
    }

    //加载单个世界
    public void LoadSingleRepo(string uri)
    {
        //获取config.json文件
        var configPath = uri.EndsWith("config.json") ? uri : Path.Combine(uri, "config.json");

        if (!File.Exists(configPath)) Log.Error("加载" + uri + "失败，未找到配置文件");

        var json = File.ReadAllText(configPath);
        try
        {
            var repo = JsonSerializer.Deserialize<WorldRepository>(json);
            if (repo == null)
                throw new Exception("repo is null");
            Repositories.Add(repo);
            Log.Information("Succeed to load repo" + repo);
        }
        catch (Exception e)
        {
            Log.Error(e.Message + "\nJson反序列化配置文档失败");
        }
    }
}